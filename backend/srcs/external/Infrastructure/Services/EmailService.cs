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
	private string CreateBody(User user) {
		string body = $@"
		Mail adresinizi onaylamak için aşağıdaki linkle tıklayın. 
		<a href='http://localhost:4200/confirm-email/{user.Email}' target='_blank'>Maili Onaylamak için tıklayın
		</a>
		";
		return body;
	}

}