using FocusProcess;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace SharpFocus.Tests;

//[TestSubject(typeof(LeetcodeClient))]
public class LeetcodeClientTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public LeetcodeClientTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async void IntegrationTest()
    {
        var leetcodeClient = new LeetcodeClient();
        var task = await leetcodeClient.CheckLeetCodeTaskCompletionAsync("");
        _testOutputHelper.WriteLine(task.ToString());
    }
}