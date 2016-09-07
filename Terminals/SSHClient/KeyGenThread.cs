using System;
using System.Threading;
using System.Windows.Forms;
using Granados.PKI;

namespace Terminals.SSHClient
{
    internal class KeyGenThread
    {
        private readonly PublicKeyAlgorithm _algorithm;
        private readonly Int32 _bits;
        private readonly KeyGenForm _parent;
        private readonly KeyGenRandomGenerator _rnd;
        private int _mouseMoveCount;

        public KeyGenThread(KeyGenForm p, PublicKeyAlgorithm a, Int32 b)
        {
            this._parent = p;
            this._algorithm = a;
            this._bits = b;
            this._rnd = new KeyGenRandomGenerator();
        }

        public void Start()
        {
            Thread t = new Thread(this.EntryPoint);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void SetAbortFlag()
        {
            this._rnd.SetAbortFlag();
        }

        private void EntryPoint()
        {
            this._mouseMoveCount = 0;
            KeyPair kp;
            if (this._algorithm == PublicKeyAlgorithm.DSA)
                kp = DSAKeyPair.GenerateNew(this._bits, this._rnd);
            else
                kp = RSAKeyPair.GenerateNew(this._bits, this._rnd);
            this._parent.SetResultKey(new SSH2UserAuthKey(kp));
        }

        public void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (this._parent.needMoreEntropy)
            {
                int n = (int) (DateTime.Now.Ticks & 0x8fffffff);
                n ^= (args.X << 16);
                n ^= args.Y;
                n ^= 0x31031293;

                this._rnd.Refresh(n);
                this._parent.SetProgressValue(++this._mouseMoveCount);
            }
        }

        private class KeyGenRandomGenerator : Random
        {
            private bool _abortFlag;
            private Random _internal;
            private int _internalAvailableCount;

            public KeyGenRandomGenerator()
            {
                this._internalAvailableCount = 0;
                this._abortFlag = false;
            }

            public override double NextDouble()
            {
                while (this._internalAvailableCount == 0)
                {
                    Thread.Sleep(100);
                    if (this._abortFlag) throw new Exception("key generation aborted");
                }
                this._internalAvailableCount--;
                return this._internal.NextDouble();
            }


            public void Refresh(int seed)
            {
                this._internal = new Random(seed);
                this._internalAvailableCount = 50;
            }

            public void SetAbortFlag()
            {
                this._abortFlag = true;
            }
        }
    }
}