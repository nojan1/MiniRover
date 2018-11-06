namespace Core.Services.Models
{
    public interface IProgram
    {
        bool IsFinished { get; }
        void Setup();
        void Loop();
        void Teardown();
    }
}