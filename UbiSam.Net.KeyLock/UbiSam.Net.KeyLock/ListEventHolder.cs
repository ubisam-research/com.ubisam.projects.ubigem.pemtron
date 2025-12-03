using System;
using System.Collections.Generic;

namespace UbiSam.Net.KeyLock
{
    public class ListEventHolder<TDelegate> : IDisposable where TDelegate : Delegate
    {
        private readonly List<TDelegate> _collection;
        public ListEventHolder()
        {
            this._collection = new List<TDelegate>();
        }
        public void Dispose()
        {
            lock (this._collection)
            {
                this._collection.Clear();
            }
        }
        public void Add(TDelegate @delegate)
        {
            lock (this._collection)
            {
                if (this._collection.Contains(@delegate) == false)
                {
                    this._collection.Add(@delegate);
                }
            }
        }
        public void Remove(TDelegate @delegate)
        {
            lock (this._collection)
            {
                if (this._collection.Contains(@delegate) == true)
                {
                    this._collection.Remove(@delegate);
                }
            }
        }
        public void RaiseEvent(params object[] param)
        {
            List<TDelegate> errors = new List<TDelegate>();
            List<TDelegate> delegates = new List<TDelegate>();

            lock (this._collection)
            {
                delegates.AddRange(this._collection);
            }

            if (delegates.Count > 0)
            {
                foreach (TDelegate @delegate in delegates)
                {
                    try
                    {
                        @delegate.DynamicInvoke(param);
                    }
                    catch
                    {
                        errors.Add(@delegate);
                    }
                }
            }

            if (errors.Count > 0)
            {
                lock (this._collection)
                {
                    foreach (TDelegate @delegate in errors)
                    {
                        this._collection.Remove(@delegate);
                    }
                }
            }
        }
    }
}
