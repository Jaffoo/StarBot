using IdolBot.Entity;
using IdolBot.IService;
using IdolBot.Repository;

namespace IdolBot.Service
{
    /// <summary>
    /// 口袋消息
    /// </summary>
    public class PoketMessages : Repository<PoketMessage>, IPoketMessage
    {
    }
}
