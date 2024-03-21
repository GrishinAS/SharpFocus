namespace FocusProcess;

public class LeetcodeStateTracker
{
    public bool State { get; } = false;
    private DateTime expirationTime;
    private Timer timer;

    public LeetcodeStateTracker()
    {
        timer = new Timer(CheckExpired, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        timer = new Timer(CheckAndUpdateState, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        expirationTime = DateTime.Today.AddDays(1).Date.AddHours(0);
    }
    
    private void CheckExpired(object state)
    {
        if (DateTime.Now >= expirationTime)
        {
            // Expired, set the state to false
            state = false;
        }
    }
    
    private void CheckAndUpdateState(object state)
    {
        if (DateTime.Now >= expirationTime)
        {
            // Expired, set the state to false
            state = false;
        }
    }
}