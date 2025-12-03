namespace UbiSam.Net.KeyLock.Structure
{
    public enum Product : int
    {
        KeyIn = -1,
        None = 0x00000000,
        UbiCOM = 0x00000001,
        UbiGEM = 0x00000002,
        UbiGEM_PAC = 0x00000004,
        IntegratedDriver = 0x00000008,
    }

    public class ProductInfo
    {
        #region [Properties]
        public bool MegaType { get; private set; }
        public string ProductCode
        {
            get; set;
        }
        public int Count { get; set; }
        #endregion
        #region [Constructor]
        public ProductInfo()
        {
            this.MegaType = false;
            ProductCode = string.Empty;
            Count = 0;
        }
        public ProductInfo(bool megaType)
        {
            this.MegaType = megaType;
            ProductCode = string.Empty;
            Count = 0;
        }

        #endregion
        #region [Methods]
        public override string ToString()
        {
            return $"[Product={ProductCode},Count={Count}]";
        }
        #endregion
    }

    public class ProductConverter
    {
        public static Product ConvertToProduct(string code)
        {
            Product product;

            switch (code)
            {
                case "UC":
                    product = Product.UbiCOM;
                    break;
                case "UG":
                    product = Product.UbiGEM;
                    break;
                case "UP":
                    product = Product.UbiGEM_PAC;
                    break;
                case "ID":
                    product = Product.IntegratedDriver;
                    break;
                default:
                    product = Product.None;
                    break;
            }

            return product;
        }
        public static string Convert(Product product)
        {
            string result;

            switch (product)
            {
                case Product.UbiCOM:
                    result = "UC";
                    break;
                case Product.UbiGEM:
                    result = "UG";
                    break;
                case Product.UbiGEM_PAC:
                    result = "UP";
                    break;
                case Product.IntegratedDriver:
                    result = "ID";
                    break;
                case Product.None:
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }
        public static string Convert(System.Collections.Generic.List<ProductInfo> list)
        {
            System.Text.StringBuilder builder;

            builder = new System.Text.StringBuilder();

            if (list != null)
            {
                foreach (ProductInfo productInfo in list)
                {
                    if (string.IsNullOrEmpty(productInfo.ProductCode) == false)
                    {
                        builder.Append($"{productInfo.ProductCode}@{productInfo.Count},");
                    }
                }
            }

            return builder.ToString();
        }
        public static System.Collections.Generic.List<ProductInfo> ConvertMega(string data)
        {
            System.Collections.Generic.List<ProductInfo> result;
            string[] productSplitted;
            string[] productCountSplitted;
            string productCode;

            result = new System.Collections.Generic.List<ProductInfo>();

            if (string.IsNullOrEmpty(data) == false)
            {
                productSplitted = data.Split(',');

                if (productSplitted != null && productSplitted.Length > 0)
                {
                    foreach (string productCount in productSplitted)
                    {
                        if (string.IsNullOrEmpty(productCount) == false)
                        {
                            productCountSplitted = productCount.Split('@');

                            if (productCountSplitted != null && productCountSplitted.Length == 2)
                            {
                                productCode = productCountSplitted[0];

                                if (string.IsNullOrEmpty(productCode) == false && int.TryParse(productCountSplitted[1], out int count) == true && count > 0)
                                {
                                    result.Add(new ProductInfo(true)
                                    {
                                        ProductCode = productCode,
                                        Count = count,
                                    });
                                }
                            }
                        }
                    }
                }

            }

            return result;
        }
    }

}
