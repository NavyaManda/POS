using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentService.Services
{
    /// <summary>
    /// Payment service for processing transactions
    /// </summary>
    public interface IPaymentService
    {
        Task<Models.ProcessPaymentResponse> ProcessPaymentAsync(Models.ProcessPaymentRequest request);
        Task<Models.PaymentDetailsResponse> GetPaymentAsync(string paymentId);
        Task<Models.ProcessPaymentResponse> ProcessRefundAsync(Models.ProcessRefundRequest request);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IEventPublisher _eventPublisher;
        private readonly IIdempotencyKeyStore _idempotencyKeyStore;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IPaymentGateway paymentGateway,
            IEventPublisher eventPublisher,
            IIdempotencyKeyStore idempotencyKeyStore)
        {
            _paymentRepository = paymentRepository;
            _paymentGateway = paymentGateway;
            _eventPublisher = eventPublisher;
            _idempotencyKeyStore = idempotencyKeyStore;
        }

        public async Task<Models.ProcessPaymentResponse> ProcessPaymentAsync(Models.ProcessPaymentRequest request)
        {
            // Check idempotency key for duplicate requests
            var cachedResult = await _idempotencyKeyStore.GetAsync(request.Idempotency_Key);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            // Create payment record
            var payment = new Models.Payment
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Method = request.Method,
                TransactionId = Guid.NewGuid().ToString()
            };

            payment.Status = Models.PaymentStatus.PROCESSING;
            await _paymentRepository.CreateAsync(payment);

            // Publish PaymentInitiated event
            await _eventPublisher.PublishAsync(new POS.Shared.Models.EventMessage
            {
                EventType = "payment.initiated.v1",
                Source = "PaymentService",
                Payload = new Dictionary<string, object>
                {
                    { "PaymentId", payment.PaymentId },
                    { "OrderId", request.OrderId },
                    { "Amount", request.Amount }
                }
            });

            try
            {
                // Process payment through gateway
                var gatewayResponse = await _paymentGateway.ChargeAsync(payment, request.Details);

                if (gatewayResponse.IsSuccessful)
                {
                    payment.Status = Models.PaymentStatus.COMPLETED;
                    payment.CompletedAt = DateTime.UtcNow;
                    payment.GatewayReference = gatewayResponse.TransactionId;
                    payment.GatewayMessage = "Payment successful";
                }
                else
                {
                    payment.Status = Models.PaymentStatus.FAILED;
                    payment.GatewayMessage = gatewayResponse.ErrorMessage;
                }

                await _paymentRepository.UpdateAsync(payment);

                // Publish PaymentCompleted or PaymentFailed event
                var eventType = payment.Status == Models.PaymentStatus.COMPLETED 
                    ? "payment.completed.v1" 
                    : "payment.failed.v1";

                await _eventPublisher.PublishAsync(new POS.Shared.Models.EventMessage
                {
                    EventType = eventType,
                    Source = "PaymentService",
                    Payload = new Dictionary<string, object>
                    {
                        { "PaymentId", payment.PaymentId },
                        { "OrderId", request.OrderId },
                        { "Amount", request.Amount },
                        { "Status", payment.Status.ToString() }
                    }
                });

                var response = new Models.ProcessPaymentResponse
                {
                    PaymentId = payment.PaymentId,
                    TransactionId = payment.TransactionId,
                    Status = payment.Status,
                    Amount = request.Amount,
                    Message = payment.GatewayMessage,
                    ProcessedAt = DateTime.UtcNow
                };

                // Cache idempotent response
                await _idempotencyKeyStore.StoreAsync(request.Idempotency_Key, response, TimeSpan.FromHours(24));

                return response;
            }
            catch (Exception ex)
            {
                payment.Status = Models.PaymentStatus.FAILED;
                payment.GatewayMessage = $"Error: {ex.Message}";
                await _paymentRepository.UpdateAsync(payment);

                throw;
            }
        }

        public async Task<Models.PaymentDetailsResponse> GetPaymentAsync(string paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new InvalidOperationException("Payment not found");
            }

            return new Models.PaymentDetailsResponse
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                CreatedAt = payment.CreatedAt,
                CompletedAt = payment.CompletedAt
            };
        }

        public async Task<Models.ProcessPaymentResponse> ProcessRefundAsync(Models.ProcessRefundRequest request)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId);
            if (payment == null)
            {
                throw new InvalidOperationException("Payment not found");
            }

            if (payment.Status != Models.PaymentStatus.COMPLETED)
            {
                throw new InvalidOperationException("Can only refund completed payments");
            }

            // Create refund record
            var refund = new Models.Refund
            {
                PaymentId = request.PaymentId,
                Amount = request.Amount,
                Reason = request.Reason
            };

            await _paymentRepository.CreateRefundAsync(refund);

            // Process refund through gateway
            var gatewayResponse = await _paymentGateway.RefundAsync(payment, request.Amount);

            if (gatewayResponse.IsSuccessful)
            {
                refund.Status = Models.PaymentStatus.COMPLETED;
                refund.CompletedAt = DateTime.UtcNow;
                payment.Status = Models.PaymentStatus.REFUNDED;
            }
            else
            {
                refund.Status = Models.PaymentStatus.FAILED;
            }

            await _paymentRepository.UpdateRefundAsync(refund);
            await _paymentRepository.UpdateAsync(payment);

            // Publish RefundProcessed event
            await _eventPublisher.PublishAsync(new POS.Shared.Models.EventMessage
            {
                EventType = "payment.refund.v1",
                Source = "PaymentService",
                Payload = new Dictionary<string, object>
                {
                    { "RefundId", refund.RefundId },
                    { "PaymentId", request.PaymentId },
                    { "Amount", request.Amount }
                }
            });

            return new Models.ProcessPaymentResponse
            {
                PaymentId = payment.PaymentId,
                Status = refund.Status,
                Amount = request.Amount,
                Message = "Refund processed",
                ProcessedAt = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Repository interface for payment persistence
    /// </summary>
    public interface IPaymentRepository
    {
        Task<Models.Payment> GetByIdAsync(string paymentId);
        Task CreateAsync(Models.Payment payment);
        Task UpdateAsync(Models.Payment payment);
        Task CreateRefundAsync(Models.Refund refund);
        Task UpdateRefundAsync(Models.Refund refund);
    }

    /// <summary>
    /// Payment gateway integration (Stripe, Square, etc.)
    /// </summary>
    public interface IPaymentGateway
    {
        Task<GatewayResponse> ChargeAsync(Models.Payment payment, Models.PaymentDetails details);
        Task<GatewayResponse> RefundAsync(Models.Payment payment, decimal amount);
    }

    public class GatewayResponse
    {
        public bool IsSuccessful { get; set; }
        public string TransactionId { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Event publisher interface
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync(POS.Shared.Models.EventMessage eventMessage);
    }

    /// <summary>
    /// Idempotency key store for preventing duplicate processing
    /// </summary>
    public interface IIdempotencyKeyStore
    {
        Task<Models.ProcessPaymentResponse> GetAsync(string key);
        Task StoreAsync(string key, Models.ProcessPaymentResponse response, TimeSpan expiry);
    }
}
