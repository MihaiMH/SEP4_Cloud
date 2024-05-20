namespace weatherstation.Utils
{
    public interface ISocketManager
    {
        Task<string> SendMessageAndWaitForResponseAsync(string message);
    }
}
