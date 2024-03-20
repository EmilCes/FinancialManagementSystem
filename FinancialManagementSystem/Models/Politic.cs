namespace FinancialManagementSystem.Models;

public class Politic
{
    public string name { get; set; }
    public string description { get; set; }
    public string state { get; set; }
    
    public int politicId { get; set; }

    public Politic(string name, string description, string state)
    {
        this.name = name;
        this.description = description;
        this.state = state;
    }

    public Politic(string name, string description, string state, int id)
    {
        this.name = name;
        this.description = description;
        this.state = state;
        this.politicId = id;
    }

    public Politic()
    {
    }
}