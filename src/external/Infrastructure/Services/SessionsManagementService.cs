using Application.Services;
using Domain.Entities;
using Infrastructure.Settings.SecuritySettings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;

public sealed class SessionsManagementService(
	ISecuritySettings    settings,
	IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		while (!stoppingToken.IsCancellationRequested) {
			using var scope             = serviceScopeFactory.CreateScope();
			var       sessionRepository = scope.ServiceProvider.GetRequiredService<IRepositoryService<Session>>();

			await DeleteExpiredSessionsAsync(sessionRepository);

			await Task.Delay(TimeSpan.FromSeconds(settings.ExpiredTokenControlMinutes), stoppingToken);
		}
	}

	private async Task DeleteExpiredSessionsAsync(IRepositoryService<Session> sessionRepository) {
		try {
			var now             = DateTime.UtcNow;
			var expiredSessions = await sessionRepository.FindAsync(session => session.ExpiryTime <= now);

			List<Session?> sessions = expiredSessions.ToList();
			foreach (var session in sessions) {
				if (session is not null) {
					await sessionRepository.DeleteOneAsync(s => s.Id == session.Id);
				}
			}
			Console.WriteLine($"Deleted {sessions.Count} expired sessions.");
		} catch (Exception ex) {
			Console.WriteLine($"Error deleting expired sessions: {ex.Message}");
		}
	}
}