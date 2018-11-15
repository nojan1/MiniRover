using System.Threading;
using System.Threading.Tasks;
using Core.Runtime;
using Core.Runtime.CommandBus;
using Core.Services.Models;

namespace Core.Services
{
    public enum ProgramState
    {
        New,
        Running,
        Finished,
        Stopped
    }

    public class ProgramService : BaseService, IHandleMessage<ProgramRunMessage>, IHandleMessage<ProgramStopMessage>, IHandleRequest<ProgramStateRequestResponse, object>
    {
        private IProgram _program;
        private ProgramState _programState = ProgramState.Stopped;
        private object _programSwapLock = new { };
        private IProgramResolver _programResolver;
        private CancellationTokenSource _programCancellationTokenSource;

        public ProgramService(IProgramResolver programResolver)
        {
            _programResolver = programResolver;
        }

        public Task Handle(ProgramRunMessage message)
        {
            return Task.Run(() =>
            {
                if (message == null && _programCancellationTokenSource != null)
                    _programCancellationTokenSource.Cancel();

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

        public Task Handle(ProgramStopMessage message)
        {
            return Handle((ProgramRunMessage)null);
        }

        public Task HandleRequest(object parameter, TaskCompletionSource<ProgramStateRequestResponse> completionSource)
        {
            return Task.Run(() => completionSource.SetResult(new ProgramStateRequestResponse
            {
                State = _programState
            }));
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
                            {
                                _program.Setup();
                                _programCancellationTokenSource = new CancellationTokenSource();
                                _programState = ProgramState.Running;
                            }

                            if (_program.IsFinished)
                            {
                                _programState = ProgramState.Finished;
                                _program.Teardown();
                            }
                            else if (_programState == ProgramState.Running)
                            {
                                _program.Loop(_programCancellationTokenSource.Token);
                            }

                        }
                    }

                    if (_program == null || _programState > ProgramState.Running)
                        Task.Delay(100, token).Wait();
                }

                if (_program != null && _programState == ProgramState.Running)
                {
                    _programCancellationTokenSource.Cancel();
                    _program.Teardown();
                }
            });
        }
    }
}