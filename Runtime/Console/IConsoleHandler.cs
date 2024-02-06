namespace CT.Console
{
    public interface IConsoleHandler
    {
        int LogCount { get; }

        void Subscribe();
        void Unsubscribe();
        void Clear();

        ConsoleLog GetLog(int logID);
    }
}
