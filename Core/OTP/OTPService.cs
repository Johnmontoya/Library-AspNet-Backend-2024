using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Core.OTP
{
    /// <summary>
    /// Clase para la generacion de codigo OTP
    /// </summary>
    public class OTPService : IOTPService
    {
        private static readonly Random _random = new Random();
        private readonly Dictionary<string, string> _otpStore = new Dictionary<string, string>();

        /// <summary>
        /// Genera el codigo OTP
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<string> GenerateAndSendOTP(string phoneNumber)
        {
            var otp = _random.Next(100000, 999999).ToString();
            _otpStore[phoneNumber] = otp;

            await Task.CompletedTask;

            return otp;
        }
    }
}
