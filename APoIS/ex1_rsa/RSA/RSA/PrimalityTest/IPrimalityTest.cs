using System.Numerics;

namespace RSA.PrimalityTest
{
    public interface IPrimalityTest
    {
        bool Test(BigInteger num);
    }
}
