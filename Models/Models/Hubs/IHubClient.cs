using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Hubs
{
    public interface IHubClient
    {
        Task BroadcastMessage();
    }
}
