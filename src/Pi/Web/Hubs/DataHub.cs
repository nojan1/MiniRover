using System.Threading.Tasks;
using Core.Services.Models;
using Microsoft.AspNetCore.SignalR;
using Rebus.Handlers;
using Autofac;

namespace Web.Hubs
{
    public class DataHubBusAdapter : IHandleMessages<SodarUpdate>
    {
        private IHubContext<DataHub> _hubContext;

        public DataHubBusAdapter(IHubContext<DataHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task Handle(SodarUpdate message) => 
            _hubContext.Clients.All.SendAsync("SodarUpdate", message);
    }

    public class DataHub : Hub
    {
    }
}