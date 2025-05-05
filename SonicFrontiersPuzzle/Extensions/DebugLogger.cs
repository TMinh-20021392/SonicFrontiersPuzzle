using System.Diagnostics;

namespace SonicFrontiersPuzzle.Extensions
{
    public static class DebugLogger
    {
        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "GraphPuzzleDebug.log");

        static DebugLogger()
        {
            // Clear the log file when the application starts
            try
            {
                File.WriteAllText(LogFilePath, $"Debug Log Started: {DateTime.Now}\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize debug log: {ex.Message}", "Debug Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Log(string message)
        {
            // Write to Debug output
            Debug.WriteLine(message);

            // Also write to our log file
            try
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now:HH:mm:ss.fff}: {message}\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to write to log file: {ex.Message}");
            }

            // Optional: Show in a message box for immediate feedback during testing
            // MessageBox.Show(message, "Debug Info");
        }
    }
}