using Newtonsoft.Json;

namespace FocusProcess;

public class SubmissionsCalendarResponse
{
    [JsonProperty("data")]
    public StreakCounterData? Data { get; set; }

    public override string ToString()
    {
        return Data?.ToString() ?? "No data";
    }
}

public class StreakCounterData
{
    [JsonProperty("matchedUser")]
    public MatchedUser? MatchedUser { get; set; }

    public override string ToString()
    {
        return MatchedUser?.ToString() ?? "No matched user";
    }
}

public class MatchedUser
{
    [JsonProperty("userCalendar")]
    public UserCalendar? UserCalendar { get; set; }

    public override string ToString()
    {
        return UserCalendar?.ToString() ?? "No user calendar";
    }
}

public class UserCalendar
{
    [JsonProperty("submissionCalendar")]
    public string? SubmissionCalendarRaw { get; set; }

    [JsonIgnore]
    public Dictionary<DateTime, int> SubmissionCalendar
    {
        get
        {
            var dictionary = new Dictionary<DateTime, int>();
            if (!string.IsNullOrEmpty(SubmissionCalendarRaw))
            {
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, int>>(SubmissionCalendarRaw);
                foreach (KeyValuePair<string, int> kvp in parsed!)
                {
                    DateTime date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(kvp.Key)).DateTime;
                    dictionary[date] = kvp.Value;
                }
            }
            return dictionary;
        }
    }

    public override string ToString()
    {
        var submissions = SubmissionCalendar;
        return submissions != null
            ? string.Join(Environment.NewLine, submissions)
            : "No submissions";
    }
}