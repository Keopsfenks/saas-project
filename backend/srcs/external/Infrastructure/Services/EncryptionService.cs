using System.Security.Cryptography;
using System.Text;
using Application.Services;
using Infrastructure.Settings.SecuritySettings;

namespace Infrastructure.Services;

public sealed class EncryptionService(ISecuritySettings securitySettings) : IEncryptionService {
	private static readonly int KeySize = 32;
	private static readonly int IvSize  = 16;

	private byte[] GenerateKey(string encryptionKey, string salt) {
		using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(salt));
		return hmac.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey)).Take(KeySize).ToArray();
	}

	public string Encrypt(string plainText) {
		byte[] key = GenerateKey(securitySettings.EncryptionKey, securitySettings.Salt);

		using Aes aesAlg = Aes.Create();
		aesAlg.Key = key;
		aesAlg.GenerateIV();
		byte[] iv = aesAlg.IV;

		using MemoryStream msEncrypt = new();
		msEncrypt.Write(iv, 0, iv.Length);

		using CryptoStream csEncrypt = new(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write);
		using StreamWriter swEncrypt = new(csEncrypt);
		swEncrypt.Write(plainText);
		swEncrypt.Flush();
		csEncrypt.FlushFinalBlock();

		return Convert.ToBase64String(msEncrypt.ToArray());
	}

	public string Decrypt(string cipherText) {
		byte[] key        = GenerateKey(securitySettings.EncryptionKey, securitySettings.Salt);
		byte[] fullCipher = Convert.FromBase64String(cipherText);

		using MemoryStream msDecrypt = new(fullCipher);

		byte[] iv = new byte[IvSize];
		msDecrypt.ReadExactly(iv, 0, iv.Length);

		using Aes aesAlg = Aes.Create();
		aesAlg.Key = key;
		aesAlg.IV  = iv;

		using CryptoStream csDecrypt = new(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read);
		using StreamReader srDecrypt = new(csDecrypt);
		return srDecrypt.ReadToEnd();
	}
}