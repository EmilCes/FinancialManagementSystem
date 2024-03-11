namespace FinancialManagementSystem.Models;

public class Employee
{
    private string Name { get; set; }
    private string LastName { get; set; }
    private string AccessToken { get; set; }

    public Employee(string name, string lastName, string accessToken)
    {
        Name = name;
        LastName = lastName;
        AccessToken = accessToken;
    }
}