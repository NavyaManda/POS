using System;
using System.Collections.Generic;

namespace PaymentService.Models
{
    /// <summary>
    /// Payment method enumeration
    /// </summary>
    public enum PaymentMethod
    {
        CARD,
        CASH,
        DIGITAL_WALLET,
        CHECK
    }

    /// <summary>
    /// Payment status enumeration
    /// </summary>
    public enum PaymentStatus
    {
        INITIATED,
        PROCESSING,
        COMPLETED,
        FAILED,
        REFUNDED,
        CANCELLED
    }

    /// <summary>
    /// Payment transaction entity
    /// </summary>
    public class Payment
    {
        public string PaymentId { get; set; } = Guid.NewGuid().ToString();
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.INITIATED;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public string GatewayReference { get; set; }
        public string GatewayMessage { get; set; }
    }

    /// <summary>
    /// Refund entity
    /// </summary>
    public class Refund
    {
        public string RefundId { get; set; } = Guid.NewGuid().ToString();
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.INITIATED;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// Process payment request
    /// </summary>
    public class ProcessPaymentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public string Idempotency_Key { get; set; } // For idempotent requests
        public PaymentDetails Details { get; set; }
    }

    /// <summary>
    /// Payment details (card, digital wallet, etc.)
    /// </summary>
    public class PaymentDetails
    {
        // For CARD: last 4 digits, token
        // For DIGITAL_WALLET: wallet type, token
        // For CASH: amount
        public string CardToken { get; set; }
        public string WalletType { get; set; }
        public string WalletToken { get; set; }
    }

    /// <summary>
    /// Process payment response
    /// </summary>
    public class ProcessPaymentResponse
    {
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    /// <summary>
    /// Payment details response
    /// </summary>
    public class PaymentDetailsResponse
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// Refund request
    /// </summary>
    public class ProcessRefundRequest
    {
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string Idempotency_Key { get; set; }
    }
}
