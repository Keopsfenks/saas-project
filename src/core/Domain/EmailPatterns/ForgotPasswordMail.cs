namespace Domain.EmailPatterns;

public record ForgotPasswordMail(string otp)
{
    public string Subject => "Şifre sıfırlama";
    public string Body => $@"
			Şifrenizi değiştirmek için kodunuz. 
			{otp}
			</a>";
}