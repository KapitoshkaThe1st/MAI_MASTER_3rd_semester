using System;

namespace GF_Operations
{
    class Program
    {
        static void Main(string[] args)
        {
            var application = new Application();
            application.Process(args);

            //bool success = BinaryPolynomial32.TryParse("x^2 + x^32 + 1", out ulong polynomial);

            //Console.WriteLine(polynomial);
        }
    }
}
