using System.Collections.Generic;

namespace FinancialManagementSystem.Models;

public class CreditType
{
    public int CreditId { get; set; }
    public string Description { get; set; }
    public float InterestRate { get; set; }
    public string State { get; set; }
    public string Term { get; set; }
    public float Iva { get; set; }
    public List<Politic> Politics { get; set; }
}