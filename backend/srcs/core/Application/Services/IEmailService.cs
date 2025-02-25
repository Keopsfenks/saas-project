using Domain.Entities;

namespace Application.Services;

public interface IEmailService {
	Task          SendEmailAsync(string to, string subject, string body);
	public string GenerateOtp(string email, TimeSpan expiry);
}