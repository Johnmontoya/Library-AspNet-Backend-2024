namespace Backend.Core
{
    public interface IOTPService
    {
        Task<string> GenerateAndSendOTP(string phoneNumber);
    }
}
