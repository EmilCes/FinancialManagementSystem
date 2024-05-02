namespace FinancialManagementSystem.Services.Credit.Dto;

public class CreditByIdResponse
{
    public string ClientName { get; set; }
    public string CreditType { get; set; }
    public string ClientRfc { get; set; }
    public string Term { get; set; }
    public int CreditNumber { get; set; }

}