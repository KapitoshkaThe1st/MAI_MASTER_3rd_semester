using System;
using System.IO;
using CommandLine;
using CommandLine.Text;
using System.Linq;

namespace CryptoTask1
{
    class BaseOptions
    {
        [Option('n', "number", Required = true, HelpText = "Number to perform operation on")]
        public uint Number { get; set; }
    }

    [Verb("get", HelpText = "Get bit of number at index")]
    class KthBitOptions : BaseOptions
    {
        [Option('i', "index", Required = true, HelpText = "Index of bit to get")]
        public int Index { get; set; }
    }

    [Verb("set", HelpText = "Get bit of number at index to 1")]
    class SetBitOptions : BaseOptions
    {
        [Option('i', "index", Required = true, HelpText = "Index of bit to set")]
        public int Index { get; set; }
    }

    [Verb("reset", HelpText = "Set bit of number at index to 0")]
    class ResetBitOptions : BaseOptions
    {
        [Option('i', "index", Required = true, HelpText = "Index of bit to reset")]
        public int Index { get; set; }
    }

    [Verb("swap", HelpText = "Swap bits of number at indices <index1> and <index2>")]
    class SwapBitsOptions : BaseOptions
    {
        [Option("index1", Required = true, HelpText = "First index")]
        public int Index1 { get; set; }

        [Option("index2", Required = true, HelpText = "Second index")]
        public int Index2 { get; set; }
    }


    [Verb("clear-bits", HelpText = "Clear <count> least significant bits setting them to 0")]
    class ClearBitsOptions : BaseOptions
    {
        [Option('c', "count", Required = true, HelpText = "Count of least significant bits to clear")]
        public int Count { get; set; }
    }

    [Verb("drop-middle", HelpText = "Drop middle of binary represenation of <number> preserving <count> least- and most significant bits")]
    class DropMiddleBitsOptions : BaseOptions
    {
        [Option('c', "count", Required = true, HelpText = "Count of most significant and least significant bits to preserve")]
        public int Count { get; set; }
    }

    [Verb("retrieve-middle", HelpText = "Drop middle <count> least- and most significant bits preserving the middle of <number>")]
    class RetrieveMiddleBitsOptions : BaseOptions
    {
        [Option('c', "count", Required = true, HelpText = "Count of most significant and least significant bits to drop")]
        public int Count { get; set; }
    }

    [Verb("parity", HelpText = "Compute parity of <number> (1 if an odd number of bits set, 0 otherwise)")]
    class ParityOptions : BaseOptions { }

    [Verb("rotate-left", HelpText = "Perform left rotate (circular shift) of <number> by <count> positions")]
    class RotateLeftOptions : BaseOptions
    {
        [Option('c', "count", Required = true, HelpText = "rotate left by <count> positions")]
        public int Count { get; set; }
    }

    [Verb("rotate-right", HelpText = "Perform right rotate (circular shift) of <number> by <count> positions")]
    class RotateRightOptions : BaseOptions
    {
        [Option('c', "count", Required = true, HelpText = "rotate right by <count> positions")]
        public int Count { get; set; }
    }

    class Program
    {
        //static uint ReadUintInBinaryForm()
        //{
        //    uint result = 0;
        //    for (int i = 0; i < 32; ++i)
        //    {
        //        int charCode = Console.Read();
        //        char @char = Convert.ToChar(charCode);

        //        if (charCode == -1 || char.IsWhiteSpace(@char))
        //        {
        //            break;
        //        }

        //        result <<= 1;

        //        if (@char == '1')
        //        {
        //            result |= 1;
        //        }
        //        else if (@char == '0')
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            throw new IOException($"An IO error occurred while reading uint in binary form.\n Unknown character: {charCode}");
        //        }
        //    }

        //    return result;
        //}

        static void GetKthBit(KthBitOptions options)
        {
            Console.WriteLine(BitOperations.GetBit(options.Number, options.Index));
        }

        static void SetKthBit(SetBitOptions options)
        {
            Console.WriteLine(BitOperations.SetBit(options.Number, options.Index));
        }

        static void ResetKthBit(ResetBitOptions options)
        {
            Console.WriteLine(BitOperations.ResetBit(options.Number, options.Index));
        }

        static void SwapBits(SwapBitsOptions options)
        {
            Console.WriteLine(BitOperations.SwapBits(options.Number, options.Index1, options.Index2));
        }

        static void ClearBits(ClearBitsOptions options)
        {
            Console.WriteLine(BitOperations.ClearLeastSignificantBits(options.Number, options.Count));
        }

        static void DropMiddleBits(DropMiddleBitsOptions options)
        {
            Console.WriteLine(BitOperations.DropMiddle(options.Number, options.Count));
        }

        static void RetrieveMiddleBits(RetrieveMiddleBitsOptions options)
        {
            Console.WriteLine(BitOperations.DropMiddle(options.Number, options.Count));
        }
        static void Parity(ParityOptions options)
        {
            Console.WriteLine(BitOperations.Parity(options.Number));
        }

        static void RotateLeft(RotateLeftOptions options)
        {
            Console.WriteLine(BitOperations.RotateLeft(options.Number, options.Count));
        }

        static void RotateRight(RotateRightOptions options)
        {
            Console.WriteLine(BitOperations.RotateRight(options.Number, options.Count));
        }

        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<KthBitOptions, SetBitOptions, ResetBitOptions, SwapBitsOptions, ClearBitsOptions,
                DropMiddleBitsOptions, RetrieveMiddleBitsOptions, ParityOptions, RotateLeftOptions, RotateRightOptions>(args);

            result
                .WithParsed<KthBitOptions>(opts => GetKthBit(opts))
                .WithParsed<SetBitOptions>(opts => SetKthBit(opts))
                .WithParsed<ResetBitOptions>(opts => ResetKthBit(opts))
                .WithParsed<SwapBitsOptions>(opts => SwapBits(opts))
                .WithParsed<ClearBitsOptions>(opts => ClearBits(opts))
                .WithParsed<DropMiddleBitsOptions>(opts => DropMiddleBits(opts))
                .WithParsed<RetrieveMiddleBitsOptions>(opts => RetrieveMiddleBits(opts))
                .WithParsed<ParityOptions>(opts => Parity(opts))
                .WithParsed<RotateLeftOptions>(opts => RotateLeft(opts))
                .WithParsed<RotateRightOptions>(opts => RotateRight(opts))
                .WithNotParsed(errors =>
                {
                    Console.Error.WriteLine($"Command line argument parsing errors occurred: {errors.Count()}");
                });
        }
    }
}
