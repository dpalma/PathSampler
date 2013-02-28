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

      private Dictionary<GridCoordinate, double> m_blockedCells = new Dictionary<GridCoordinate, double>();

      public Dictionary<GridCoordinate, double> BlockedCells
      {
         get
         {
            return m_blockedCells;
         }
         set
         {
            m_blockedCells = value;
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
