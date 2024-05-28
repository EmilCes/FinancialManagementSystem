using CommunityToolkit.Mvvm.Messaging.Messages;
using FinancialManagementSystem.Models;

namespace FinancialManagementSystem.Messages;

public class PaymentUploadMessage(): ValueChangedMessage<User>(null!);
