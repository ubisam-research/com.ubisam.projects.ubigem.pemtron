using System.Collections.Generic;
using System.ComponentModel;
using UbiSam.Net.KeyLock.Structure;

namespace UbiSam.Net.KeyLock.Maker.Info
{
    public class ProductMakeInfo : INotifyPropertyChanged
    {
        #region [Events]
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) == false)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        #region [Properties]
        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                if (this._isSelected != value)
                {
                    this._isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }
        public Product Product { get; }
        public int Count
        {
            get
            {
                return this._count;
            }
            set
            {
                if (this._count != value)
                {
                    this._count = value;
                    NotifyPropertyChanged("Count");
                }
            }
        }
        public static List<Product> PredefinedProducts
        {
            get
            {
                return new List<Product>() { Product.UbiCOM, Product.UbiGEM, Product.UbiGEM_PAC, Product.IntegratedDriver };
            }
        }
        public bool Predefined
        {
            get
            {
                return PredefinedProducts.Contains(this.Product) == true;
            }
        }
        public string ProductCode
        {
            get
            {
                if (this.Product == Product.KeyIn)
                {
                    return this._productCode;
                }
                else
                {
                    return ProductConverter.Convert(this.Product);
                }
            }
            set
            {
                if (value != null)
                {
                    if (this.Product == Product.KeyIn)
                    {
                        this._productCode = value;
                        NotifyPropertyChanged("ProductCode");
                    }
                }
            }
        }
        #endregion
        #region [Member Variables]
        private bool _isSelected;
        private int _count;
        private string _productCode;
        #endregion
        #region [Constructors]
        public ProductMakeInfo(Product product)
        {
            this._isSelected = false;
            this.Product = product;
            this._productCode = ProductConverter.Convert(product);
            this._count = 0;
        }
        #endregion
    }
}
