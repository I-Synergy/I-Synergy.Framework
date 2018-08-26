using PubnubApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IPubNubService
    {
        Task<PNStatus> PublishAsync(string channel, object message, Dictionary<string, object> meta = null);
        Task AddAndSubscribeToChannelsAsync(string accountId, string userId);
    }
}
