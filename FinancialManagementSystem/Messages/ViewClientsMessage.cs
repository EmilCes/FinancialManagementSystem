using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using FinancialManagementSystem.Models;

namespace FinancialManagementSystem.Messages;

public class ViewClientsMessage() : ValueChangedMessage<Client>(null!);
