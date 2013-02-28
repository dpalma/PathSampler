using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace PathFind.ViewModels
{
   public class MapVM : INotifyPropertyChanged
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

      public event PropertyChangedEventHandler PropertyChanged;

      private void FirePropertyChanged(string propertyName)
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }
}
