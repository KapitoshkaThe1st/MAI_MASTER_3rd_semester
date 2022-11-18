using System;
using System.Linq;

using CommandLine;

using GF_Operations.CommandLineArgsParsing.Options;
using GF_Operations.CommandLineArgsParsing.Options.BinaryPolynomialOperationsOptions;

namespace GF_Operations
{
    class Application
    {
        private static string BinaryRepresentation(uint num)
        {
            return Convert.ToString(num, 2).PadLeft(sizeof(uint) * 8, '0');
        }

        private static string HexRepresentation(uint num)
        {
            return Convert.ToString(num, 16).PadLeft(sizeof(uint) * 8 / 4, '0');
        }

        private static string BinaryRepresentation(ulong num)
        {
            return Convert.ToString((long)num, 2).PadLeft(sizeof(ulong) * 8, '0');
        }

        private static string HexRepresentation(ulong num)
        {
            return Convert.ToString((long)num, 16).PadLeft(sizeof(ulong) * 8 / 4, '0');
        }

        private static void GFPolynimialFormParsing(GFPolynomialFormParsingOptions options)
        {
            Console.WriteLine($"dec: {options.GFElement}, hex: 0x{HexRepresentation(options.GFElement)}, bin: 0b{BinaryRepresentation(options.GFElement)}");
        }

        private static void GFStringRepresentation(GFStringRepresentationOptions options)
        {
            Console.WriteLine(GF256.StringRepresentation(options.GFElement));
        }

        private static void GFAddition(GFAdditionOptions options)
        {
            Console.WriteLine(GF256.StringRepresentation(GF256.Add(options.GFElement1, options.GFElement2)));
        }

        private static void GFMultiplication(GFMultiplicationOptions options)
        {
            Console.WriteLine(GF256.StringRepresentation(GF256.Multiply(options.GFElement1, options.GFElement2, options.GFModuloPolynomial)));
        }

        private static void GFExtendedGCD(GFExtendedGCDOptions options)
        {
            uint gcd = GF256.ExtendedGreatestCommonDivisor(options.GFElement1, options.GFElement2, options.GFModuloPolynomial, out uint u, out uint w);
            Console.WriteLine($"GCD: {GF256.StringRepresentation(gcd)}\ncoefficients:\nu: {GF256.StringRepresentation(u)} w: {GF256.StringRepresentation(w)}");
        }

        private static void GFMultiplicativeInverse(GFMultiplicativeInverseOptions options)
        {
            uint inverse = GF256.Inverse(options.GFElement, options.GFModuloPolynomial);
            Console.WriteLine($"inverse: {GF256.StringRepresentation(inverse)}");
        }

        private static void BinaryPolynomialPolynimialFormParsing(BinaryPolynomialParsingOptions options)
        {
            Console.WriteLine($"dec: {options.Polynomial}, hex: 0x{HexRepresentation(options.Polynomial)}, bin: 0b{BinaryRepresentation(options.Polynomial)}");
        }

        private static void BinaryPolynomialStringRepresentation(BinaryPolynomialStringRepresentationOptions options)
        {
            Console.WriteLine(BinaryPolynomial32.StringRepresentation(options.Polynomial));
        }

        private static void BinaryPolynomialAddition(BinaryPolynomialAdditionOptions options)
        {
            Console.WriteLine(BinaryPolynomial32.StringRepresentation(BinaryPolynomial32.Add(options.Polynomial1, options.Polynomial2)));
        }

        private static void BinaryPolynomialMultiplication(BinaryPolynomialMultiplicationOptions options)
        {
            Console.WriteLine(BinaryPolynomial32.StringRepresentation(BinaryPolynomial32.Multiply(options.Polynomial1, options.Polynomial2)));
        }

        public void Process(string[] args)
        {
            var result = Parser.Default.ParseArguments<GFPolynomialFormParsingOptions, GFStringRepresentationOptions, GFAdditionOptions,
                GFMultiplicationOptions, GFExtendedGCDOptions, GFMultiplicativeInverseOptions,
                BinaryPolynomialParsingOptions, BinaryPolynomialStringRepresentationOptions,
                BinaryPolynomialAdditionOptions, BinaryPolynomialMultiplicationOptions>(args);

            try
            {
                result
                    .WithParsed<GFPolynomialFormParsingOptions>(opts => GFPolynimialFormParsing(opts))
                    .WithParsed<GFStringRepresentationOptions>(opts => GFStringRepresentation(opts))
                    .WithParsed<GFAdditionOptions>(opts => GFAddition(opts))
                    .WithParsed<GFMultiplicationOptions>(opts => GFMultiplication(opts))
                    .WithParsed<GFExtendedGCDOptions>(opts => GFExtendedGCD(opts))
                    .WithParsed<GFMultiplicativeInverseOptions>(opts => GFMultiplicativeInverse(opts))
                    .WithParsed<BinaryPolynomialParsingOptions>(opts => BinaryPolynomialPolynimialFormParsing(opts))
                    .WithParsed<BinaryPolynomialStringRepresentationOptions>(opts => BinaryPolynomialStringRepresentation(opts))
                    .WithParsed<BinaryPolynomialAdditionOptions>(opts => BinaryPolynomialAddition(opts))
                    .WithParsed<BinaryPolynomialMultiplicationOptions>(opts => BinaryPolynomialMultiplication(opts))
                    .WithNotParsed(errors =>
                    {
                        Console.Error.WriteLine($"Command line argument parsing errors occurred: {errors.Count()}");
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AN ERROR OCCURRED: {ex.Message}");
            }
        }
    }
}
