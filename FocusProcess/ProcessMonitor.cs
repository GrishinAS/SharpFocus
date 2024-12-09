using System.Diagnostics;
using System.Management;

namespace FocusProcess;

public class ProcessMonitor
{
    private readonly List<string> _restrictedProcesses = new();
    private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "restrictedPrograms.txt");
    private readonly ShowMsgDelegate _showMessageDelegate;
    private readonly LeetcodeClient _leetcodeClient;

    public ProcessMonitor(ShowMsgDelegate showMessageDelegate, LeetcodeClient leetcodeClient)
    {
        _showMessageDelegate = showMessageDelegate;
        _leetcodeClient = leetcodeClient;
    }

    public delegate void ShowMsgDelegate(string message);

    public void Start(CancellationTokenSource token)
    {
        try
        {
            LoadItemsFromStorage();
            const string query = "SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'";
            using var watcher = new ManagementEventWatcher(query);
            //StartListenForNewProcesses(watcher);
            while (!token.IsCancellationRequested)
            {
                Process[] processes = Process.GetProcesses();

                List<Process> restrictedProcessesRun = processes.Where(p => _restrictedProcesses.Contains(p.ProcessName)).ToList();
            
                foreach (Process process in restrictedProcessesRun)
                {
                    Console.WriteLine($"Restricted process '{process.ProcessName}' detected.");
                    Console.WriteLine("Checking time.");
                    bool late = DateTime.Now.Hour > 16;
                    Console.WriteLine("Checking leetcode status.");
                    Task<bool> leetcodeSolved = _leetcodeClient.CheckLeetCodeTaskCompletionAsync("");

                    if (late || leetcodeSolved.Result)
                    {
                        token.Cancel();
                        return;
                    }

                    process.Kill();
                
                    _showMessageDelegate?.Invoke($"Restricted process '{process.ProcessName}' was killed. Solve your daily Leetcode task to run it");
                }
            

                // Delay for a short interval before checking processes again
                Thread.Sleep(8000); 
            }
            watcher.Stop();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private void StartListenForNewProcesses(ManagementEventWatcher watcher)
    {
        watcher.EventArrived += (sender, e) =>
        {
            var targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string processName = targetInstance["Name"]?.ToString();
            if (_restrictedProcesses.Any(app => app.Equals(processName?.Replace(".exe", ""), StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    int processId = Convert.ToInt32(targetInstance["ProcessId"]);
                    var process = Process.GetProcessById(processId);
                    process.Kill();
                    Console.WriteLine($"Blocked and terminated: {processName}");
                    _showMessageDelegate?.Invoke($"Restricted process '{process.ProcessName}' was killed. Solve your daily Leetcode task to run it");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to block {processName}: {ex.Message}");
                }
            }
        };

        Console.WriteLine("Listening for new processes...");
        watcher.Start();
    }
    
    private void LoadItemsFromStorage()
    {
        if (File.Exists(_filePath))
        {
            List<string> items = File.ReadAllLines(_filePath).ToList();
            foreach (string item in items)
            {
                _restrictedProcesses.Add(item);
            }
        }
        else
        {
            _showMessageDelegate?.Invoke("No processes to track configured. Add some processes.");
        }
    }
}