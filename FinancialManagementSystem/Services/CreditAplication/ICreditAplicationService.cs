using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.CreditAplication;

public interface ICreditAplicationService
{
    [Get("/clientExistance")]
    Task<VerifyClientExistenceResponse> VerifyExistanceAsync([Body] VerifyClientRequest request);
    
    [Get("/clientRegular")]
    Task<VerifyRegularClientResponse> VerifyRegularAsync([Body] VerifyClientRequest request);

    [Get("/creditTypes")]
    Task<List<CreditType>> GetCreditTypesAsync([Body] object dummy = null);

    [Post("/createAplication")]
    Task<CreateAplicationResponse> CreateAplicationAsync([Body] CreditAplicationRequest request);
}

public class VerifyClientExistenceResponse
{
    public bool isClientRegistered { get; set; }
}

public class VerifyClientRequest
{
    public string rfc { get; set; }
}

public class VerifyRegularClientResponse
{
    public bool clientIsRegular { get; set; }
    public string refreshToken { get; set; }
}

public class CreditType
{
    public string description { get; set; }
    public int months { get; set; }
    public string amount { get; set; }
    public string interest { get; set; }
}

public class Reference
{
    public string name { get; set; }
    public string firstLastname { get; set; }
    public string secondLastname { get; set; }
    public string telephone { get; set; }
}

public class CreditAplicationRequest
{
    public string clientRfc { get; set; }
    public CreditType selectedCredit { get; set; }
    public Reference firstReference { get; set; } 
    public Reference secondReference { get; set; }
    public byte[] IdentificationPdf { get; set; }
    public byte[] MoneyComprobantPdf { get; set; }
    public byte[] LocationComprobantPdf { get; set; }
}

public class CreateAplicationResponse
{
    public string refreshToken { get; set; }
}
