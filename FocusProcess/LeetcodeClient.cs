using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

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
            UserCalendar? submissionsCalendar = await RetrieveSubmissionsCalendar(username);
            return submissionsCalendar?.SubmissionCalendar.ContainsKey(DateTime.UtcNow.Date) ?? false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

    private static async Task<UserCalendar?> RetrieveSubmissionsCalendar(string username)
    {
        var client = new GraphQLHttpClient(LEETCODE_API, new NewtonsoftJsonSerializer());

        var getStreakCounterRequest = new GraphQLRequest
        {
            Query = SubmissionsCalendarQuery,
            OperationName = "userProfileCalendar",
            Variables = new
            {
                username,
                year = DateTime.Now.Year.ToString()
            }
        };

        GraphQLResponse<StreakCounterData> response =
            await client.SendQueryAsync(getStreakCounterRequest, () => new StreakCounterData());

        if (response.Errors != null)
           throw new Exception(
                $"GraphQL request failed: {response.Errors.Select(e => e.Message).Aggregate((a, b) => $"{a}, {b}")}");


        UserCalendar? submissionsCalendar = response.Data.MatchedUser?.UserCalendar;
        return submissionsCalendar;
    }

    private const string SubmissionsCalendarQuery = @"
            query userProfileCalendar($username: String!, $year: Int) {  
                matchedUser(username: $username) 
	            {   userCalendar(year: $year) 
	            	{      
	            	submissionCalendar    
	            	}  
	            }
            }";
}