using Application.Services;
using Domain.Dtos;
using Domain.Entities;
using Infrastructure.Settings.DatabaseSetting;
using MongoDB.Driver;

namespace Infrastructure.Services;

public sealed class UserService : IUserService {
	private readonly IMongoCollection<AppUser> _userCollection;

	public UserService(IDatabaseSettings settings) {
		var client   = new MongoClient(settings.ConnectionString);
		var database = client.GetDatabase(settings.DatabaseName);

		_userCollection = database.GetCollection<AppUser>(settings.UserCollectionName);
	}

	public async Task RegisterAsync(RegisterUserDto registerUserDto) {
		AppUser user = new() {
			Name     = registerUserDto.Name,
			Surname  = registerUserDto.Surname,
			Email    = registerUserDto.Email,
			Password = registerUserDto.Password
		};

		await _userCollection.InsertOneAsync(user);
	}

	public Task<string> LoginAsync(string email, string password) {
		throw new NotImplementedException();
	}
}