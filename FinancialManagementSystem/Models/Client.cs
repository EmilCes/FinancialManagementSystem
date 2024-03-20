using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace FinancialManagementSystem.Models;

public class Client
{
    public int ClientId { get; set; }
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string Surname { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Rfc { get; set; }
    public float MonthlySalary { get; set; }
    public Address Address { get; set; }
    public Workplace Workplace { get; set; }
    public List<BankAccount> BankAccounts { get; set; }
}