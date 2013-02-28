using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using PathFind.Models;

namespace PathFind.ViewModels
{
   public class MapVM : INotifyPropertyChanged
   {
      private Map m_map;
      public Map Map
      {
         get
         {
            return m_map;
         }
         set
         {
            m_map = value;
            FirePropertyChanged("Map");
         }
      }

      public Size Dimensions
      {
         get
         {
            return Map.Dimensions;
         }
         set
         {
            Map.Dimensions = value;
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
