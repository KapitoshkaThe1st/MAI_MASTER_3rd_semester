using System;
using System.Diagnostics;
using System.Numerics;

using RSA;
using RSA_Attack;
using RSA.PrimalityTest;

namespace WienerAttackBench
{
    class Program
    {
        // Герман О.Н, Нестеренко Ю.В.Теоретико-числовые методы в криптографии.
        // Сложность алгоритма определяется количеством подходящих дробей для непрерывной дроби числа e/N,
        // что есть величина порядка O(log(n)). То есть число d восстанавливается за линейное время от длины ключа.

        static void Main(string[] args)
        {
            RSAKeyType[] RSAKeyTypes = new RSAKeyType[] {
                RSAKeyType.RSA16,
                RSAKeyType.RSA32,
                RSAKeyType.RSA64,
                RSAKeyType.RSA128,
                RSAKeyType.RSA256,
                RSAKeyType.RSA512,
                RSAKeyType.RSA1024,
                RSAKeyType.RSA2048
            };

            const int nRuns = 10;

            foreach (var keyType in RSAKeyTypes)
            {
                RSAKeyGenerator keyGenerator = new RSAKeyGenerator(new MillerRabinTest(0.995f), keyType);

                var keys = new (BigInteger modulus, BigInteger publicExponent, BigInteger privateExponent)[nRuns];

                for (int i = 0; i < nRuns; ++i)
                {
                    keyGenerator.GenerateVulnerable(out BigInteger modulus, out BigInteger publicExponent, out BigInteger privateExponent);
                    keys[i] = (modulus, publicExponent, privateExponent);
                }

                Stopwatch sw = Stopwatch.StartNew();

                WienerAttack attack = new WienerAttack();

                int totalConvergentsChecked = 0;

                int k = 0;
                foreach (var (modulus, publicExponent, privateExponent) in keys)
                {
                    var result = attack.Attack(modulus, publicExponent, out int convergentsChecked);

                    if (result.Success)
                    {
                        if (result.PrivateExponent != privateExponent)
                        {
                            Console.WriteLine($"k: {k}, attack failed! Modulus: {modulus}, public exponent: {publicExponent}, private exponent: {privateExponent}");
                            break;
                        }
                    }
                    totalConvergentsChecked += convergentsChecked;
                    k++;
                }

                sw.Stop();
                Console.WriteLine($"keyType: {(int)keyType * 2}");

                Console.WriteLine($"total time: {sw.ElapsedMilliseconds} ms, average time: {(double)sw.ElapsedMilliseconds / nRuns:0.00} ms");
                Console.WriteLine($"total convergents checked: {totalConvergentsChecked}, average convergents checked: {(double)totalConvergentsChecked / nRuns:0.00}");
            }
        }
    }
}
