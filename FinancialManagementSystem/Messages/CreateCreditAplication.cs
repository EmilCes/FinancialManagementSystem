using CommunityToolkit.Mvvm.Messaging.Messages;
using FinancialManagementSystem.Models;

namespace FinancialManagementSystem.Messages;

public class CreateCreditApplication(string clientRfc): ValueChangedMessage<string>(clientRfc);