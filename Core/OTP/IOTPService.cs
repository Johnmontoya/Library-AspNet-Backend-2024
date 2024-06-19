namespace Backend.Core.OTP
{
    /// <summary>
    /// Interfaz para implementar el OTP
    /// </summary>
    public interface IOTPService
    {
        /// <summary>
        /// Implementacion para generar el número OTP
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<string> GenerateAndSendOTP(string phoneNumber);
    }
}
