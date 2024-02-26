using FluentScheduler;

namespace AIdol.Timer
{
    internal class DataSyncJob : IJob
    {
        void IJob.Execute()
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }
    }
}
