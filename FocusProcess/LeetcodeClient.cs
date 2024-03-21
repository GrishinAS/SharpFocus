using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;

namespace FocusProcess;

public class LeetcodeClient
{
    private const string LEETCODE_API = "https://leetcode.com/graphql/";
    
    public async Task<bool> CheckLeetCodeTaskCompletionAsync(string username)
    {
        try
        {
            // Create a GraphQLHttpClient with the endpoint URL and configure headers
            var client = new GraphQLHttpClient(LEETCODE_API, new NewtonsoftJsonSerializer());
            //client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "YOUR_ACCESS_TOKEN");
            client.HttpClient.DefaultRequestHeaders.Add("x-csrftoken", "IqpuESC6Pkuuji5j4FoqCEOiryMX3BIbnO1Sr2NQQpclH8plsptF91JYomLbMH0o;");
            client.HttpClient.DefaultRequestHeaders.Add("Cookie", "LEETCODE_SESSION=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJfYXV0aF91c2VyX2lkIjoiMzU0MDA0MSIsIl9hdXRoX3VzZXJfYmFja2VuZCI6ImFsbGF1dGguYWNjb3VudC5hdXRoX2JhY2tlbmRzLkF1dGhlbnRpY2F0aW9uQmFja2VuZCIsIl9hdXRoX3VzZXJfaGFzaCI6IjJkNzM3OWMxODAzNzRjZjcyNTYxMDBlOWU4MDZiZGE3OTJmM2I2ZTY3MDJjMDc0MTZlMjlhNmZlNzc2ODZlYzkiLCJpZCI6MzU0MDA0MSwiZW1haWwiOiJnaGpkdGhyZmFmYmtqQGdtYWlsLmNvbSIsInVzZXJuYW1lIjoiZ2hqZHRocmZhZmJraiIsInVzZXJfc2x1ZyI6ImdoamR0aHJmYWZia2oiLCJhdmF0YXIiOiJodHRwczovL2Fzc2V0cy5sZWV0Y29kZS5jb20vdXNlcnMvZ2hqZHRocmZhZmJrai9hdmF0YXJfMTYwNDEzNTc1Ny5wbmciLCJyZWZyZXNoZWRfYXQiOjE3MDg5Mjc5NzYsImlwIjoiNDcuMTQ2LjE2MC4xMDciLCJpZGVudGl0eSI6IjlmZWE3MDFhNjI3YTU3ZDBjNDU4ZGIyZTFjYjYwZDYyIiwic2Vzc2lvbl9pZCI6NTYwNjA1NjN9.C82Rcun64KdvB1GMnf7h60AS3mgEpDqw03Ol6RknFzI;");
            
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
}