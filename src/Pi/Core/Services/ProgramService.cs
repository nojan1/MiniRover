using System.Threading;
using System.Threading.Tasks;
using Core.Runtime;
using Core.Services.Models;
using Rebus.Handlers;

namespace Core.Services
{
    public enum ProgramState
    {
        New,
        Running,
        Finished,
        Stopped
    }

    public class ProgramService : BaseService, IHandleMessages<ProgramRunRequest>, IHandleMessages<ProgramStopRequest>
    {
        private IProgram _program;
        private ProgramState _programState = ProgramState.Stopped;
        private object _programSwapLock = new { };
        private IProgramResolver _programResolver;

        public ProgramService(IProgramResolver programResolver, Rebus.Bus.IBus bus)
        {
            _programResolver = programResolver;

        }

        public Task Handle(ProgramRunRequest message)
        {
            return Task.Run(() =>
            {
                lock (_programSwapLock)
                {
                    if (_program != null && (_programState == ProgramState.Running || message == null))
                    {
                        _program.Teardown();
                        _programState = ProgramState.Stopped;
                    }

                    if (message != null)
                    {
                        _programState = ProgramState.New;
                        _program = _programResolver.ResolveByName(message.ProgramName);
                    }
                }
            });
        }

        public Task Handle(ProgramStopRequest message)
        {
            return Handle((ProgramRunRequest)null);
        }

        public override Task Run(CancellationToken token)
        {
            return Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    lock (_programSwapLock)
                    {
                        if (_program != null)
                        {
                            if (_programState == ProgramState.New)
                                _program.Setup();

                            if (_program.IsFinished)
                            {
                                _programState = ProgramState.Finished;
                                _program.Teardown();
                            }
                            else
                            {
                                _program.Loop(token);
                            }

                        }
                    }

                    if (_program == null || _programState > ProgramState.Running)
                        Task.Delay(100, token).Wait();
                }

                if (_program != null && _programState == ProgramState.Running)
                {
                    _program.Teardown();
                }
            });
        }
    }
}