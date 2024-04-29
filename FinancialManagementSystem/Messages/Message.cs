using CommunityToolkit.Mvvm.Messaging;

namespace FinancialManagementSystem.Messages;

public class Message
{
    private static IMessenger _instance;
    private static readonly object _block = new object();

    private Message()
    {
    }

    public static IMessenger Instance
    {
        get
        {
            lock (_block)
            {
                if (_instance == null)
                {
                    _instance = WeakReferenceMessenger.Default;
                }

                return _instance;
            }
        }
    }
}