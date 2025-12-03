using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UbiSam.Net.KeyLock.Structure
{
    public class ActiveProductInfo
    {
        #region [Properties]
        public int PID { get; set; }
        public ushort IID { get; set; }
        public string UniqueKey { get; set; }
        public string ProductCode { get; set; }
        public DateTime LastDT { get; private set; }
        #endregion
        #region [Constructors]
        public ActiveProductInfo(string uniqueKey, Product product, DateTime? lastDT = null)
        {
            UniqueKey = uniqueKey;
            ProductCode = ProductConverter.Convert(product);
            if (lastDT == null || lastDT.HasValue == false)
            {
                LastDT = DateTime.Now;
            }
            else
            {
                LastDT = lastDT.Value;
            }            
        }
        public ActiveProductInfo(string uniqueKey, string productCode, DateTime? lastDT = null)
        {
            UniqueKey = uniqueKey;
            ProductCode = productCode;
            if (lastDT == null || lastDT.HasValue == false)
            {
                LastDT = DateTime.Now;
            }
            else
            {
                LastDT = lastDT.Value;
            }
        }
        #endregion
        #region [Methods]
        public void ChangeProduct(Product product)
        {
            ProductCode = ProductConverter.Convert(product);
        }
        public void UpdateLastDT()
        {
            LastDT = DateTime.Now;
        }
        public override string ToString()
        {
            return $"[PID={PID},IID={IID},UniqueKey={UniqueKey},Product={ProductCode},LastDT={LastDT:yyyyMMdd HHmmss.fff}]";
        }
        #endregion
    }
    public class ActiveProductCollection : List<ActiveProductInfo>
    {
        #region [Properties]
        public ActiveProductInfo this[string uniqueKey]
        {
            get
            {
                return this.FirstOrDefault(t => t.UniqueKey == uniqueKey);
            }
        }
        #endregion
        #region [Methods]
        public void Remove(string uniqueKey)
        {
            ActiveProductInfo item;

            item = this[uniqueKey];

            if (item != null)
            {
                this.Remove(item);
            }
        }
        #endregion
    }
}
