using weatherstation.Domain.Model;

namespace weatherstation.Application.LogicInterfaces
{
    public interface IAccountLogic
    {
        Task<string> LoginAccount(dynamic data);
        Task RegisterAccount(dynamic data);
        Task<User?> GetUserByEmail(string email);
        Task<string> UpdateAccount(dynamic data, Dictionary<string, string> token);
    }
}
