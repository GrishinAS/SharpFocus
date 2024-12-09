using System.Diagnostics;
using SharpFocusUI;

namespace FocusProcess;

public class ProcessMonitor
{
    private readonly List<string> _restrictedProcesses = new();
    private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "restrictedPrograms.txt");
    private readonly ShowMsgDelegate _showMessageDelegate;
    private readonly StopWork _stopWork;
    private readonly LeetcodeClient _leetcodeClient;
    private readonly AppSettings _loadSettings;

    public ProcessMonitor(ShowMsgDelegate showMessageDelegate, StopWork stopWork, LeetcodeClient leetcodeClient, AppSettings loadSettings)
    {
        _showMessageDelegate = showMessageDelegate;
        _stopWork = stopWork;
        _leetcodeClient = leetcodeClient;
        _loadSettings = loadSettings;
    }

    public delegate void ShowMsgDelegate(string message);
    public delegate void StopWork();

    public void Start(CancellationToken token)
    {
        try
        {
            LoadItemsFromStorage();
            while (!token.IsCancellationRequested)
            {
                Process[] processes = Process.GetProcesses();

                List<Process> restrictedProcessesRun = processes.Where(p => _restrictedProcesses
                    .Contains(p.ProcessName))
                    .DistinctBy(p => p.ProcessName).ToList();
            
                foreach (Process process in restrictedProcessesRun)
                {
                    Console.WriteLine($"Restricted process '{process.ProcessName}' detected.");
                    Console.WriteLine("Checking time.");
                    bool late = DateTime.Now.Hour > 16;
                    if (late)
                    {
                        Console.WriteLine("It's late, access granted.");
                        _stopWork.Invoke();
                        return;
                    }
                    Console.WriteLine("Checking leetcode status.");
                    Task<bool> leetcodeSolved = _leetcodeClient.CheckLeetCodeTaskCompletionAsync(_loadSettings.LeetCodeUsername);
                    if (leetcodeSolved.Result)
                    {
                        Console.WriteLine("Leetcode task completed.");
                        _stopWork.Invoke();
                        return;
                    }

                    process.Kill();
                
                    _showMessageDelegate?.Invoke($"Restricted process '{process.ProcessName}' was killed. Solve your daily Leetcode task to run it");
                }
            

                // Delay for a short interval before checking processes again
                Thread.Sleep(8000); 
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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