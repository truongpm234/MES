using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMMS.Application.Interfaces
{
    public interface ITaskQrTokenService
    {
        string CreateToken(int taskId, TimeSpan ttl);
        bool TryValidate(string token, out int taskId, out string reason);
    }
}
