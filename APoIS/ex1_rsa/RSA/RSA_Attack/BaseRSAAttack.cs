using System;
using System.Numerics;

namespace RSA_Attack
{
    public abstract class BaseRSAAttack
    {
        protected readonly Action<float> _updateCallback;
        protected readonly Action<bool> _endCallback;

        public BaseRSAAttack()
        {
            _updateCallback = null;
            _endCallback = null;
        }

        public BaseRSAAttack(Action<float> updateCallback, Action<bool> endCallBack)
        {
            _updateCallback = updateCallback;
            _endCallback = endCallBack;
        }

        public abstract RSAAttackResult Attack(BigInteger modulus, BigInteger publicExponent);
    }
}
