namespace weatherstation.Application.LogicInterfaces
{
    public interface IRecommendationLogic
    {
        Task<string> GetRecommendation(dynamic data, Dictionary<string, string> token);
    }
}
