using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.CreditAplication;

public interface ICreditApplicationService
{
    [Get("/regularClient")]
    Task<VerifyRegularClientResponse> VerifyRegularAsync([Query] string rfc);

    [Get("/creditTypes")]
    Task<List<CreditType>> GetCreditTypesAsync();

    [Post("/createAplication")]
    Task CreateAplicationAsync([Body] CreditAplicationRequest request);
}

public class VerifyClientRequest
{
    public string rfc { get; set; }
}

public class VerifyRegularClientResponse
{
    public bool clientIsRegular { get; set; }
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
