using CommunityToolkit.Mvvm.Messaging.Messages;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;

namespace FinancialManagementSystem.Messages;

public class ViewPaymentMessage(PaymentRecord result) : ValueChangedMessage<PaymentRecord>(result);

public class ViewPaymentMessageWithoutPayment (string rfc) : ValueChangedMessage<string>(rfc);