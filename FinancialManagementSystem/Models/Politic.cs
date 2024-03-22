namespace FinancialManagementSystem.Models;

public class Politic
{
    public string name { get; set; }
    public string description { get; set; }
    public string state { get; set; }
    
    public bool isSelected { get; set; }
    
    public int politicId { get; set; }
    public bool cbPoliticState { get; set; }

    public Politic(string name, string description, string state, bool isSelected, int politicId)
    {
        this.name = name;
        this.description = description;
        this.state = state;
        this.isSelected = isSelected;
        this.politicId = politicId;
    }

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