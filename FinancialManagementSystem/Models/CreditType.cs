using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FinancialManagementSystem.Models;

public class CreditType: ObservableObject
{
    public int CreditTypeId { get; set; }
    public string Description { get; set; }
    public float InterestRate { get; set; }
    public string State { get; set; }
    public string Term { get; set; }
    public float Iva { get; set; }
    public List<Politic> Politics { get; set; }

    public override string ToString()
    {
        return Description + " " + Term;
    }
}