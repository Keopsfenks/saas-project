using Application.Services;
using Domain.Entities;
using FluentEmail.Core;

namespace Infrastructure.Services;

public sealed class EmailService(IFluentEmail email) : IEmailService {
	public async Task SendEmailAsync(string to, string subject, string body) {
		await email
			.To(to)
			.Subject(subject)
			.Body(body)
			.SendAsync();
	}

	public string GenerateOtp() {
		Random random = new Random();
		return random.Next(100000, 999999).ToString();
	}
}