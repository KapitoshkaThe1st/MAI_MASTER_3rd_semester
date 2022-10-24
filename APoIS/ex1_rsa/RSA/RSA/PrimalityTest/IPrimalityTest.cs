using System.Numerics;

namespace RSA.PrimalityTest
{
    interface IPrimalityTest
    {
        bool Test(BigInteger num);
    }
}
