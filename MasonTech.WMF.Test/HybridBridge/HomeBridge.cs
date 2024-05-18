using System.Text.Json.Serialization;

namespace MasonTech.WMF.Test.HybridBridge
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }

    public class PagingRequest
    {
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }

    public class HomeBridge : IHybridBridge
    {
        public Task<int[]> ReturnArrayAsync(int startPosition)
        {
            return Task.FromResult(Enumerable.Range(startPosition, 3).ToArray());
        }

        public Task<List<User>> GetUsers(PagingRequest request)
        {
            List<User> users = new List<User>();
            for (int i = 0; i < request.PageSize; i++)
            {
                users.Add(new User()
                {
                    Name = "User " + ((request.PageIndex * request.PageSize) + i + 1).ToString(),
                });
            }
            return Task.FromResult(users);
        }
    }
}
