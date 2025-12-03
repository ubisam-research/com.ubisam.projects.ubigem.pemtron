using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace UbiGEM.Net.Simulator.Host.Common
{
    public class DataGridProxy : Freezable
    {
        #region [Properties]
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(DataGridProxy), new UIPropertyMetadata(null));
        #endregion

        #region CreateInstanceCore
        protected override Freezable CreateInstanceCore()
        {
            return new DataGridProxy();
        }

        #endregion
    }
}