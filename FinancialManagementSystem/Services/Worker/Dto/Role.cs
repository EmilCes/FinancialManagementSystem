using System.Collections.Generic;

namespace FinancialManagementSystem.Services.Worker.Dto;

public class Role
{
    public string Name { get; }
    public string Description { get; }
    public bool CbRoleState { get; set; }
    
    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }

}
