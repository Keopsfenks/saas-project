namespace Domain.EmailPatterns;

public record VerificationMail(string otp)
{
    public string Subject => "Lütfen e-posta adresinizi doğrulayın";

    public string Body => $@"
			Mail adresinizi onaylamak için kodunuz. 
			{otp}	
			</a>";
}