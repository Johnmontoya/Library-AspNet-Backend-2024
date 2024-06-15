
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Core
{
    public class OTPService : IOTPService
    {
        private static readonly Random _random = new Random();
        private readonly Dictionary<string, string> _otpStore = new Dictionary<string, string>();
        private readonly ILogger<OTPService> _logger;

        public OTPService(ILogger<OTPService> logger, UserManager<Authentication> userManager)
        {
            _logger = logger;
        }

        public async Task<string> GenerateAndSendOTP(string phoneNumber)
        {
            var otp = _random.Next(100000, 999999).ToString();
            _otpStore[phoneNumber] = otp;

            _logger.LogInformation($"Generated OTP: {otp} for phone number: {phoneNumber}");

            // Simulate sending OTP via email or SMS
            await Task.CompletedTask;

            return otp;
        }
    }
}
