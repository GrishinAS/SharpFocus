using System.Diagnostics;
using System.Management;

namespace FocusProcess;

public class ProcessMonitor
{
    private string[] restrictedProcesses = { "notepad", "calc" };
    private readonly ProcessKillDelegate _processKillDelegate;
    private readonly LeetcodeClient _leetcodeClient;

    public ProcessMonitor(ProcessKillDelegate processKillDelegate, LeetcodeClient leetcodeClient)
    {
        _processKillDelegate = processKillDelegate;
        _leetcodeClient = leetcodeClient;
    }

    public delegate void ProcessKillDelegate(string processName);

    public void Start(CancellationTokenSource token)
    {
        try
        {
            const string query = "SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'";
            using var watcher = new ManagementEventWatcher(query);
            StartListenForNewProcesses(watcher);
            while (!token.IsCancellationRequested)
            {
                Process[] processes = Process.GetProcesses();

                List<Process> restrictedProcessesRun = processes.Where(p => restrictedProcesses.Contains(p.ProcessName)).ToList();
            
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
                
                    _processKillDelegate?.Invoke(process.ProcessName);
                }
            

                // Delay for a short interval before checking processes again
                Thread.Sleep(5000); 
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
            if (restrictedProcesses.Any(app => app.Equals(processName?.Replace(".exe", ""), StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    int processId = Convert.ToInt32(targetInstance["ProcessId"]);
                    var process = Process.GetProcessById(processId);
                    process.Kill();
                    Console.WriteLine($"Blocked and terminated: {processName}");
                    _processKillDelegate?.Invoke(process.ProcessName);
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
}