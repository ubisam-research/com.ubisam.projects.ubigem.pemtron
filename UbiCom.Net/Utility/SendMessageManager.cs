using System;
using System.Collections.Generic;

namespace UbiCom.Net.Utility
{
    internal class SendMessageManager : IDisposable
    {
        private Dictionary<uint, Structure.SECSMessage> _messageMgr;

        private bool _disposed;

        public SendMessageManager()
        {
            this._messageMgr = new Dictionary<uint, Structure.SECSMessage>();

            this._disposed = false;
        }

        ~SendMessageManager()
        {
            Dispose(false);
        }

        public bool Exists(uint systemBytes)
        {
            bool result;

            if (this._messageMgr.Count <= 0)
            {
                result = false;
            }
            else
            {
                result = this._messageMgr.ContainsKey(systemBytes);
            }

            return result;
        }

        public void Add(Structure.SECSMessage message)
        {
            this._messageMgr.Add(message.SystemBytes, message);
        }

        public void Remove(uint systemBytes)
        {
            this._messageMgr.Remove(systemBytes);
        }

        public void Clear()
        {
            if (this._messageMgr != null)
            {
                this._messageMgr.Clear();
            }
            else
            {
                this._messageMgr = new Dictionary<uint, Structure.SECSMessage>();
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    if (this._messageMgr != null)
                    {
                        this._messageMgr.Clear();
                        this._messageMgr = null;
                    }
                }

                this._disposed = true;
            }
        }

        public Structure.SECSMessage GetMessage(uint systemBytes)
        {
            Structure.SECSMessage result;

            if (this._messageMgr != null)
                result = this._messageMgr.ContainsKey(systemBytes) == true ? this._messageMgr[systemBytes] : null;
            else
                result = null;

            return result;
        }
    }
}