using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GF_Operations;
using System.IO;

using Rijndael.Options;
using System.Threading;

namespace Rijndael
{
    class Program
    {
        static void Main(string[] args)
        {
            Application application = new Application();
            application.Process(args);
        }
    }
}
