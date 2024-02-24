using FluentScheduler;

namespace IdolBot.Timer
{
    public class Program
    {
        static void Main(string[] args)
        {
            //��ʼ�����������
            JobManager.Initialize(new FluentSchedulerFactory());
            Console.ReadKey();
        }
    }

    /// <summary>
    /// �������FluentScheduler����
    /// </summary>
    public class FluentSchedulerFactory : Registry
    {
        public FluentSchedulerFactory()
        {
            //Ĭ�϶��߳�
            //��Job���е��߳��ܣ�����û����ʱ���ظ�ִ�С�(ȫ��)
            //NonReentrantAsDefault();
            //��Job���е��߳��ܣ�����û����ʱ���ظ�ִ�С�(��������)
            //Schedule<DataSyncJob>().NonReentrant().ToRunNow().AndEvery(5).Seconds();

            //����ִ��ÿ5��һ�εļƻ����񡣣�ָ��һ��ʱ�������У������Լ����󣬿������롢�֡�ʱ���졢�¡���ȡ���
            Schedule<DataSyncJob>().ToRunEvery(5).Seconds();

            ////����ִ��һ��ÿ���µ�һ������һ18��00�ļƻ�����
            //Schedule<DataSyncJob>().ToRunNow().AndEvery(1).Months().OnTheFirst(DayOfWeek.Monday).At(18, 0);

            ////�ӳ�5��ִ�е�һ�μƻ����񡣣�ָ��һ��ʱ�������У������Լ����󣬿������롢�֡�ʱ���졢�¡���ȡ���
            //Schedule<DataSyncJob>().ToRunOnceIn(5).Seconds();

            ////ָ��ʱ��ִ�мƻ�������ã���������ÿ��18��00ִ�С���
            //Schedule(() => Console.WriteLine("It's 18:00 now.")).ToRunEvery(1).Days().At(18, 0);

            ////��ͬһ���ƻ���ִ�ж������
            //Schedule<DataSyncJob>().AndThen<TestJob>().ToRunNow().AndEvery(5).Seconds();
        }
    }
}