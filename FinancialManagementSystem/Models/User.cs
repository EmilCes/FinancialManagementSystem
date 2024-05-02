namespace FinancialManagementSystem.Models;

public class User
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string UserNumber { get; set; }
    public string Email { get; set; }
    public string Rfc { get; set; }
    public string Role { get; set; }

    public string Fullname
    {
        get
        {
            return Firstname + " " + Lastname;
        }
    }

}