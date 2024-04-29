using CommunityToolkit.Mvvm.Messaging.Messages;
using FinancialManagementSystem.Models;

namespace FinancialManagementSystem.Messages;

public class ViewClientMessage(Client result) : ValueChangedMessage<Client>(result);