using System.Diagnostics;

namespace FocusProcess;

public class ProcessMonitor
{
    private string[] restrictedProcesses = { "notepad.exe", "calc.exe" };
    private readonly ProcessKillDelegate _processKillDelegate;
    private readonly LeetcodeClient _leetcodeClient;

    public ProcessMonitor(ProcessKillDelegate processKillDelegate, LeetcodeClient leetcodeClient)
    {
        _processKillDelegate = processKillDelegate;
        _leetcodeClient = leetcodeClient;
    }

    public delegate void ProcessKillDelegate(string processName);

    public void Start(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Process[] processes = Process.GetProcesses();

            var restrictedProcessesRun = processes.Where(p => restrictedProcesses.Contains(p.ProcessName)).ToList();
            
            foreach (var process in restrictedProcessesRun)
            {
                // Check if any restricted processes are running
                Console.WriteLine($"Restricted process '{process.ProcessName}' detected.");
                Console.WriteLine("Checking time.");
                bool late = DateTime.Now.Hour > 17;
                Console.WriteLine("Checking leetcode status.");
                var leetcodeSolved = _leetcodeClient.CheckLeetCodeTaskCompletionAsync("");

                if (late || leetcodeSolved.Result)
                {
                    return;
                }

                process.Kill();
                
                _processKillDelegate?.Invoke(process.ProcessName);
            }
            

            // Delay for a short interval before checking processes again
            Thread.Sleep(5000); 
        }
    }
}