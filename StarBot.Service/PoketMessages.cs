using StarBot.Entity;
using StarBot.IService;
using StarBot.Repository;

namespace StarBot.Service
{
    /// <summary>
    /// 口袋消息
    /// </summary>
    public class PoketMessages : Repository<PoketMessage>, IPoketMessage
    {
    }
}
