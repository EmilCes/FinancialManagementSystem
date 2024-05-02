using CommunityToolkit.Mvvm.Messaging.Messages;
using FinancialManagementSystem.Models;

namespace FinancialManagementSystem.Messages;

public class ViewWorkerMessage(User result) : ValueChangedMessage<User>(result);