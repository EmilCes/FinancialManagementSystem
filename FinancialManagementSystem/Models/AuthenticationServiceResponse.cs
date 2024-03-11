using System;
using System.Net;

namespace FinancialManagementSystem.Models;

public class AuthenticationServiceResponse
{
    public HttpStatusCode RespondeCode { get; set; }
    public Exception Exception { get; set; }
    public Employee Employee { get; set; }
}