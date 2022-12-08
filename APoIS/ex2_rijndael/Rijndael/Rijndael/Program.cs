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
            //var key = new byte[]
            //{
            //    0x2b, 0x7e, 0x15, 0x16,
            //    0x28, 0xae, 0xd2, 0xa6,
            //    0xab, 0xf7, 0x15, 0x88,
            //    0x09, 0xcf, 0x4f, 0x3c,
            //    0xac, 0xbe, 0xda, 0x13,
            //    0x88, 0x77, 0x66, 0x55,
            //};


            //var iv = new uint[] 
            //{
            //    0x01234567U,
            //    0x89abcdefU,
            //    0x01234567U,
            //    0x89abcdefU,
            //    0x01234567U,
            //    0x89abcdefU,
            //};

            ////Rijndael rijndael = new CFBRijndael(iv, key, Rijndael.BlockLength.Bit192);
            //Rijndael rijndael = new OFBRijndael(iv, key, Rijndael.BlockLength.Bit192);
            ////Rijndael rijndael = new CBCRijndael(iv, key, Rijndael.BlockLength.Bit192);

            //string s = "1234567890123456789012345678901234567890";

            //byte[] encodedS = rijndael.EncodeString(s);
            //string decodedS = rijndael.DecodeString(encodedS);

            //Console.WriteLine($"s = {s}");
            //Console.WriteLine($"decodedS = {decodedS}");

            //Console.WriteLine($"s == decodedS: {s == decodedS}");

            //rijndael.EncodeFile("TestFile.txt", "TestFile.txt.rij");
            //rijndael.DecodeFile("TestFile.txt.rij", "decoded_TestFile.txt");

            Application application = new Application();
            application.Process(args);
        }
    }
}
