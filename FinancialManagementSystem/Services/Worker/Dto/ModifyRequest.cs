namespace FinancialManagementSystem.Services.Worker.Dto;

public class ModifyRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Rfc { get; set; }
    public string UserNumber { get; set; }
    public Role Role { get; set; }
}