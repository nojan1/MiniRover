using System;
using Core.Services.Models;

namespace Programs
{
    public class Test : IProgram
    {
        public bool IsFinished {get; set;}

        public void Loop()
        {
            Console.WriteLine("Entering loop");
            IsFinished = true;
        }

        public void Setup()
        {
            Console.WriteLine("Entering setup");
        }

        public void Teardown()
        {
            Console.WriteLine("Entering teardown");
        }
    }
}
