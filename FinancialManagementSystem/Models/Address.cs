namespace FinancialManagementSystem.Models;

public class Address
{
    public int AddressId { get; set; }
    public string Street { get; set; }
    public string Neighborhood { get; set; }
    public int ExteriorNumber { get; set; }
    public int InteriorNumber { get; set; }
    public string PostalCode { get; set; }
    public string Municipality { get; set; }
    public string State { get; set; }
}