using CommunityToolkit.Mvvm.Messaging.Messages;
using FinancialManagementSystem.Models;

namespace FinancialManagementSystem.Messages;

public class SearchWorkerMessage(): ValueChangedMessage<User>(null!);