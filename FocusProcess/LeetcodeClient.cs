using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;

namespace FocusProcess;

public class LeetcodeClient
{
    private const string LEETCODE_API = "https://leetcode.com/graphql/";

    public static void Main()
    {
        var task = new LeetcodeClient().CheckLeetCodeTaskCompletionAsync("");
        Console.WriteLine(task.Result);
    }
    
    public async Task<bool> CheckLeetCodeTaskCompletionAsync(string username)
    {
        try
        {
            // Create a GraphQLHttpClient with the endpoint URL and configure headers
            var client = new GraphQLHttpClient(LEETCODE_API, new NewtonsoftJsonSerializer());
            //client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "YOUR_ACCESS_TOKEN");
            //client.HttpClient.DefaultRequestHeaders.Add("x-csrftoken", "");
            //client.HttpClient.DefaultRequestHeaders.Add("Cookie", "LEETCODE_SESSION=");
            
            var getStreakCounterRequest = new GraphQLRequest {
                Query = DAILY_STAT_QUERY,
                OperationName = "getStreakCounter",
                Variables = new {
                    username
                }
            };

            // Make the GraphQL request with the query, variables, and headers
            var response = await client.SendQueryAsync(getStreakCounterRequest, () => new StreakCounterResponse() );

            // Handle the response
            Console.WriteLine(response.Errors != null
                ? $"GraphQL request failed: {response.Errors}"
                : $"Response: {response.Data}");

            return response.Data.Data.StreakCounter.CurrentDayCompleted;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

    private const string DAILY_STAT_QUERY = @"
            query getStreakCounter {
                streakCounter {
                    currentDayCompleted
                    }
            }";
}

public class StreakCounterResponse
{
    [JsonProperty("data")]
    public StreakCounterData Data { get; set; }

    public override string ToString()
    {
        return $"{nameof(Data)}: {Data}";
    }
}

public class StreakCounterData
{
    [JsonProperty("streakCounter")]
    public StreakCounter StreakCounter { get; set; }
}

public class StreakCounter
{
    [JsonProperty("streakCount")] 
    public bool CurrentDayCompleted { get; set; }

    public override string ToString()
    {
        return $"{nameof(CurrentDayCompleted)}: {CurrentDayCompleted}";
    }
}