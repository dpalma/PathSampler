using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace PathFind.Models
{
   public class Map : INotifyPropertyChanged
   {
      private Size m_dimensions;
      public Size Dimensions
      {
         get
         {
            return m_dimensions;
         }
         set
         {
            m_dimensions = value;
            FirePropertyChanged("Dimensions");
         }
      }

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion

      private void FirePropertyChanged(string propertyName)
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }
}
