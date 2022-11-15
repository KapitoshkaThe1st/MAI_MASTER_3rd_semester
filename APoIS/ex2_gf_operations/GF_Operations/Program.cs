using System;

namespace GF_Operations
{
    class Program
    {

        static void Main(string[] args)
        {
            bool success = GF256.TryParse("x^10", out uint gfElement);

            if (success)
            {
                Console.WriteLine($"Success: gf(256) element: {gfElement}");
            }
            else
            {
                Console.WriteLine($"Fail");
            }

            Console.WriteLine("DONE");

            Console.ReadKey();
        }
    }
}
