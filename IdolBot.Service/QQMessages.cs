using IdolBot.Entity;
using IdolBot.IService;
using IdolBot.Repository;

namespace IdolBot.Service
{
    /// <summary>
    /// qq消息
    /// </summary>
    public class QQMessages : Repository<QQMessage>, IQQMessage
    {
    }
}
