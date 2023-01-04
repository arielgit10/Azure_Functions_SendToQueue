

using FunctionAppAriel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionAppTimerTrigger
{
    public interface IServerRepository
    {
        Task<List<Account>> GetAccounts();
    }
}
