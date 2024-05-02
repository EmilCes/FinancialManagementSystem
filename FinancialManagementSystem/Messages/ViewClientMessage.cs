using CommunityToolkit.Mvvm.Messaging.Messages;
using FinancialManagementSystem.Models;

namespace FinancialManagementSystem.Messages;

public class ViewClientMessage(Client result) : ValueChangedMessage<Client>(result);

public class ViewClientMessageFromValidation(ClientAndCredit result) : ValueChangedMessage<ClientAndCredit>(result);

public class ClientAndCredit
{
    public Client Client {get; set; }
    public CreditApplication CreditApplication { get; set; }
}