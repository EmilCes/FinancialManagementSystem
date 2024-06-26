using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.CreditApplication;

public interface ICreditApplicationService
{
    [Get("/regularClient")]
    Task<VerifyRegularClientResponse> VerifyRegularAsync([Query] string rfc);

    [Post("/applicate")]
    Task CreateAplicationAsync([Body] CreditApplicationRequest request);
    
    [Get("/")]
    Task<List<Models.CreditApplication>> GetCreditAplicationsTypesAsync();
    
    [Get("/{id}")]
    Task<Models.CreditApplication> GetCreditAplicationByIdAsync(int id);
}

public class VerifyClientRequest
{
    public string rfc { get; set; }
}

public class VerifyRegularClientResponse
{
    public bool clientIsRegular { get; set; }
}

public class Reference
{
    public string Name { get; set; }
    public string FirstLastname { get; set; }
    public string SecondLastname { get; set; }
    public string Telephone { get; set; }
}

public class CreditApplicationRequest
{
    public string ClientRfc { get; set; }
    public int idCreditType { get; set; }
    public Models.CreditType SelectedCredit { get; set; }
    public Reference FirstReference { get; set; } 
    public Reference SecondReference { get; set; }
    public byte[] IdentificationPdf { get; set; }
    public byte[] ProofOfIncomePdf { get; set; }
    public byte[] ProofOfAddressPdf { get; set; }
}
