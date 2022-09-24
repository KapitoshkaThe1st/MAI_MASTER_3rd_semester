using System;
using System.Linq;

using CommandLine;
using CryptoTask1.CommandLineArgsParsing.Options;

namespace CryptoTask1
{
    public class Application
    {
        private static string BinaryRepresentation(uint num)
        {
            return Convert.ToString(num, 2).PadLeft(sizeof(uint) * 8, '0');
        }

        private static void GetKthBit(KthBitOptions options)
        {
            Console.WriteLine(BitOperations.GetBit(options.Number, options.Index));
        }

        private static void SetKthBit(SetBitOptions options)
        {
            Console.WriteLine(BinaryRepresentation(BitOperations.SetBit(options.Number, options.Index)));
        }

        private static void ResetKthBit(ResetBitOptions options)
        {
            Console.WriteLine(BinaryRepresentation(BitOperations.ResetBit(options.Number, options.Index)));
        }

        private static void SwapBits(SwapBitsOptions options)
        {
            Console.WriteLine(BinaryRepresentation(BitOperations.SwapBits(options.Number, options.Index1, options.Index2)));
        }

        private static void ClearBits(ClearBitsOptions options)
        {
            Console.WriteLine(BinaryRepresentation(BitOperations.ClearLeastSignificantBits(options.Number, options.Count)));
        }

        private static void OuterBits(OuterBitsOptions options)
        {
            Console.WriteLine(BinaryRepresentation(BitOperations.OuterBits(options.Number, options.Count)));
        }

        private static void InnerBits(InnerBitsOptions options)
        {
            Console.WriteLine(BinaryRepresentation(BitOperations.InnerBits(options.Number, options.Count)));
        }

        private static void Parity(ParityOptions options)
        {
            Console.WriteLine(BitOperations.Parity(options.Number));
        }

        private static void RotateLeft(RotateLeftOptions options)
        {
            Console.WriteLine(BinaryRepresentation(BitOperations.RotateLeft(options.Number, options.Count)));
        }

        private static void RotateRight(RotateRightOptions options)
        {
            Console.WriteLine(BinaryRepresentation(BitOperations.RotateRight(options.Number, options.Count)));
        }

        public void Process(string[] args)
        {
            var result = Parser.Default.ParseArguments<KthBitOptions, SetBitOptions, ResetBitOptions, SwapBitsOptions, ClearBitsOptions,
                OuterBitsOptions, InnerBitsOptions, ParityOptions, RotateLeftOptions, RotateRightOptions>(args);

            try
            {
                result
                    .WithParsed<KthBitOptions>(opts => GetKthBit(opts))
                    .WithParsed<SetBitOptions>(opts => SetKthBit(opts))
                    .WithParsed<ResetBitOptions>(opts => ResetKthBit(opts))
                    .WithParsed<SwapBitsOptions>(opts => SwapBits(opts))
                    .WithParsed<ClearBitsOptions>(opts => ClearBits(opts))
                    .WithParsed<OuterBitsOptions>(opts => OuterBits(opts))
                    .WithParsed<InnerBitsOptions>(opts => InnerBits(opts))
                    .WithParsed<ParityOptions>(opts => Parity(opts))
                    .WithParsed<RotateLeftOptions>(opts => RotateLeft(opts))
                    .WithParsed<RotateRightOptions>(opts => RotateRight(opts))
                    .WithNotParsed(errors =>
                    {
                        Console.Error.WriteLine($"Command line argument parsing errors occurred: {errors.Count()}");
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
            }
        }
    }
}
