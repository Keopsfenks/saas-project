using System.Security.Cryptography;
using System.Text;
using Application.Services;
using Infrastructure.Settings.SecuritySettings;

namespace Infrastructure.Services;

public sealed class EncryptionService(ISecuritySettings securitySettings) : IEncryptionService {
	private static readonly int KeySize = 32;
	private static readonly int IvSize  = 16;

	[Obsolete("Obsolete")]
	private (byte[] Key, byte[] IV) GenerateKeyAndIv(string plainVar) {
		using (var pbkdf2 = new Rfc2898DeriveBytes(plainVar, Encoding.UTF8.GetBytes(securitySettings.HashAlgorithmKey), 10000)) {
			byte[] key = pbkdf2.GetBytes(KeySize);
			byte[] iv  = pbkdf2.GetBytes(IvSize);
			return (key, iv);
		}
	}

	[Obsolete("Obsolete")]
	public string Encrypt(string plainVar) {
		var (key, iv) = GenerateKeyAndIv(plainVar);

		using Aes aesAlg = Aes.Create();
		aesAlg.Key = key;
		aesAlg.IV  = iv;

		using MemoryStream msEncrypt = new();
		using CryptoStream csEncrypt = new(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write);
		using (StreamWriter swEncrypt = new(csEncrypt)) {
			swEncrypt.Write(plainVar);
		}
		return Convert.ToBase64String(msEncrypt.ToArray());
	}

	[Obsolete("Obsolete")]
	public string Decrypt(string cipherVar) {
		var (key, iv) = GenerateKeyAndIv(cipherVar);

		using Aes aesAlg = Aes.Create();
		aesAlg.Key = key;
		aesAlg.IV  = iv;

		using MemoryStream msDecrypt = new(Convert.FromBase64String(cipherVar));
		using CryptoStream csDecrypt = new(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read);
		using StreamReader srDecrypt = new(csDecrypt);
		return srDecrypt.ReadToEnd();
	}
}
