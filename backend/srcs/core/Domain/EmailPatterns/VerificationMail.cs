namespace Domain.EmailPatterns;

public readonly struct VerificationMail(string otp) {
	public string Subject => "Lütfen e-posta adresinizi doğrulayın";

	public string Body => $@"
			Mail adresinizi onaylamak için kodunuz. 
			{otp}	
			</a>";
}