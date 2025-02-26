namespace Application.Services;

public interface IEncryptionService
{
	string Encrypt(string                plainText);
	string Decrypt(string                cipherText);
	string DecryptWithGeneratedIV(string cipherText, string originalPlainText);
}
