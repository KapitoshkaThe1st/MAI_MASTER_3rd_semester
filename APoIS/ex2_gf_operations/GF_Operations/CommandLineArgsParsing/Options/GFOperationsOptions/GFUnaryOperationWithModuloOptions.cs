using System;
using CommandLine;

namespace GF_Operations.CommandLineArgsParsing.Options
{
    public class GFUnaryOperationWithModuloOptions : GFUnaryOperationOptions
    {
        private string _gfModuloPolynomialString;
        [Option('m', "modulo", Required = true, HelpText = "modulo -- irreducible polynomial in hex format with 0x- prefix")]
        public string GFModuloPolynomialString
        {
            get
            {
                ThrowNotSupported();
                return string.Empty;
            }
            set
            {
                _gfModuloPolynomialString = value;
            }
        }

        private bool _gfModuloPolynomialParsed = false;
        private uint _gfModuloPolynomial;
        public uint GFModuloPolynomial
        {
            get
            {
                if (_gfModuloPolynomialParsed)
                {
                    return _gfModuloPolynomial;
                }

                _gfModuloPolynomial = Convert.ToUInt32(_gfModuloPolynomialString, 16);

                _gfModuloPolynomialParsed = true;
                return _gfModuloPolynomial;
            }
        }
    }
}
