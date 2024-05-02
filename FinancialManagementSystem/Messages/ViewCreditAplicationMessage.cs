using CommunityToolkit.Mvvm.Messaging.Messages;
using FinancialManagementSystem.Models;

namespace FinancialManagementSystem.Messages;

public class ViewCreditAplicationMessage(CreditApplication result) : ValueChangedMessage<CreditApplication>(result);