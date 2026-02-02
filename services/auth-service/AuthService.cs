using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services
{
    /// <summary>
    /// JWT token generation and validation service
    /// </summary>
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, string email, List<string> roles);
        string GenerateRefreshToken();
        POS.Shared.Models.TokenClaims ValidateToken(string token);
    }

    public class TokenService : ITokenService
    {
        private readonly string _jwtSecret;
        private readonly int _accessTokenExpiryMinutes;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public TokenService(string jwtSecret, int accessTokenExpiryMinutes = 15, 
                           string jwtIssuer = "pos-system", string jwtAudience = "pos-api")
        {
            _jwtSecret = jwtSecret ?? throw new ArgumentNullException(nameof(jwtSecret));
            _accessTokenExpiryMinutes = accessTokenExpiryMinutes;
            _jwtIssuer = jwtIssuer;
            _jwtAudience = jwtAudience;
        }

        public string GenerateAccessToken(string userId, string email, List<string> roles)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim("sub", userId)
            };

            // Add roles as separate claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            // Generate a cryptographically secure random token
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public POS.Shared.Models.TokenClaims ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                var email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                var roles = jwtToken.Claims
                    .Where(x => x.Type == ClaimTypes.Role)
                    .Select(x => x.Value)
                    .ToList();

                return new POS.Shared.Models.TokenClaims
                {
                    UserId = userId,
                    Email = email,
                    Roles = roles,
                    IssuedAt = (long)jwtToken.ValidFrom.Subtract(DateTime.UnixEpoch).TotalSeconds,
                    ExpiresAt = (long)jwtToken.ValidTo.Subtract(DateTime.UnixEpoch).TotalSeconds
                };
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Authentication service for user login/registration
    /// </summary>
    public interface IAuthService
    {
        Task<Models.LoginResponse> LoginAsync(Models.LoginRequest request);
        Task<Models.LoginResponse> RefreshTokenAsync(string refreshToken);
        Task RegisterAsync(Models.RegisterRequest request);
        Task<Models.ValidateTokenResponse> ValidateTokenAsync(string token);
    }

    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            ITokenService tokenService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Models.LoginResponse> LoginAsync(Models.LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var accessToken = _tokenService.GenerateAccessToken(user.UserId, user.Email, user.Roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Store refresh token in database
            await _refreshTokenRepository.CreateAsync(new Models.RefreshToken
            {
                UserId = user.UserId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            return new Models.LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 900,
                User = new Models.UserInfo
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = user.Roles
                }
            };
        }

        public async Task<Models.LoginResponse> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User not found or inactive");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user.UserId, user.Email, user.Roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Revoke old refresh token
            storedToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            // Create new refresh token
            await _refreshTokenRepository.CreateAsync(new Models.RefreshToken
            {
                UserId = user.UserId,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            return new Models.LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = 900,
                User = new Models.UserInfo
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = user.Roles
                }
            };
        }

        public async Task RegisterAsync(Models.RegisterRequest request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists");
            }

            var user = new Models.User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = HashPassword(request.Password),
                Roles = new List<string> { "customer" }
            };

            await _userRepository.CreateAsync(user);
        }

        public async Task<Models.ValidateTokenResponse> ValidateTokenAsync(string token)
        {
            var claims = _tokenService.ValidateToken(token);
            if (claims == null)
            {
                return new Models.ValidateTokenResponse
                {
                    IsValid = false,
                    Message = "Invalid or expired token"
                };
            }

            return new Models.ValidateTokenResponse
            {
                IsValid = true,
                UserId = claims.UserId,
                Roles = claims.Roles,
                Message = "Token is valid"
            };
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }

    /// <summary>
    /// Repository interfaces for data access
    /// </summary>
    public interface IUserRepository
    {
        Task<Models.User> GetByIdAsync(string userId);
        Task<Models.User> GetByEmailAsync(string email);
        Task CreateAsync(Models.User user);
        Task UpdateAsync(Models.User user);
    }

    public interface IRefreshTokenRepository
    {
        Task<Models.RefreshToken> GetByTokenAsync(string token);
        Task CreateAsync(Models.RefreshToken token);
        Task UpdateAsync(Models.RefreshToken token);
    }
}
