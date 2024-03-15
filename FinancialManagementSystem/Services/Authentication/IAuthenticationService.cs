using System.Net;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.Authentication;

public interface IAuthenticationService
{
    [Post("/verify")]
    Task<VerificationResponse> VerifyAsync([Body] VerificationRequest request);

    [Post("/authenticate")]
    Task<AuthenticationResponse> AuthenticateAsync([Body] AuthenticationRequest request);

    [Post("/enable-2fa")]
    Task<Enable2FaResponse> Enable2FaAsync([Body] AuthenticationRequest request);
}

public class VerificationRequest
{
    public string email { get; set; }
    public string code { get; set; }
}

public class VerificationResponse
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string accessToken { get; set; }
    public string role { get; set; }
}

public class AuthenticationRequest
{
    public string email { get; set; }
    public string password { get; set; }
}

public class AuthenticationResponse
{
    public string accessToken { get; set; }
    public string refreshToken { get; set; }
    public bool mfaEnabled { get; set; }
}

public class Enable2FaResponse
{
    public string accessToken { get; set; }
    public string refreshToken { get; set; }
    public bool mfaEnabled { get; set; }
    public string secretImageUri { get; set; }
}