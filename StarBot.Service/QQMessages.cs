using StarBot.Entity;
using StarBot.IService;
using StarBot.Repository;

namespace StarBot.Service
{
    /// <summary>
    /// qq消息
    /// </summary>
    public class QQMessages : Repository<QQMessage>, IQQMessage
    {
    }
}
