namespace FinancialManagementSystem.Models;

public class Employee
{
    private static Employee instance;
    private static readonly object block = new object();

    public int EmployeeId { get; set; }
    public string AccessToken { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }

    private Employee() { }

    public static Employee Instance
    {
        get
        {
            lock (block)
            {
                if (instance == null)
                {
                    instance = new Employee();
                }
                
                return instance;
            }
        }
    }
    
    public override string ToString()
    {
        return $"Usuario: {FirstName} {LastName}, Rol: {Role}, AccessToken: {AccessToken}";
    }
}