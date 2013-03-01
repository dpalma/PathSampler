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

            if (Goal == null)
            {
               Goal = new GridCoordinate() { Row = (int)m_dimensions.Height - 1, Column = (int)m_dimensions.Width - 1 };
            }

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

      private GridCoordinate m_start = new GridCoordinate() { Row = 0, Column = 0 };
      public GridCoordinate Start
      {
         get
         {
            return m_start;
         }
         set
         {
            if (BlockedCells.ContainsKey(value))
            {
               throw new ArgumentException(String.Format("Cell {0} is blocked and cannot be the start", value));
            }
            m_start = value;
            FirePropertyChanged("Start");
         }
      }

      private GridCoordinate m_goal;
      public GridCoordinate Goal
      {
         get
         {
            return m_goal;
         }
         set
         {
            if (BlockedCells.ContainsKey(value))
            {
               throw new ArgumentException(String.Format("Cell {0} is blocked and cannot be the goal", value));
            }
            m_goal = value;
            FirePropertyChanged("Goal");
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
