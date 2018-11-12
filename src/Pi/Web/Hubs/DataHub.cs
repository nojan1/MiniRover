using System.Threading.Tasks;
using Core.Services.Models;
using Microsoft.AspNetCore.SignalR;
using Autofac;
using Core.Drivers;
using System;
using Core.Runtime.CommandBus;

namespace Web.Hubs
{
    public class DataHubBusAdapter : IHandleMessage<SodarUpdate>, IHandleMessage<IMUReading>, IHandleMessage<VisionUpdate>
    {
        private IHubContext<DataHub> _hubContext;

        public DataHubBusAdapter(IHubContext<DataHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task Handle(SodarUpdate message) => 
            _hubContext.Clients.All.SendAsync("SodarUpdate", message);
            
        public Task Handle(IMUReading message) => 
            _hubContext.Clients.All.SendAsync("IMUReading", message);

        public Task Handle(VisionUpdate message)
        {
            var imageBase64Data = Convert.ToBase64String(message.Image);
            return _hubContext.Clients.All.SendAsync("VisionUpdate", imageBase64Data);
        }
    }

    public class DataHub : Hub
    {
    }
}