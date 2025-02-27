using Application.Services;
using FluentEmail.Core;

namespace Infrastructure.Services;

public sealed class EmailService(IFluentEmail email, ICacheService cacheService) : IEmailService {
	public async Task SendEmailAsync(string to, string subject, string body) {
		await email
			.To(to)
			.Subject(subject)
			.Body(body)
			.SendAsync();
	}

	public string GenerateOtp(string email, TimeSpan expiry) {
		Random random = new Random();
		string otp = random.Next(100000, 999999).ToString();
		cacheService.Set(email, otp, expiry);
		return otp;
	}
}