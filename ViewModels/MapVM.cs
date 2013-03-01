using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using PathFind.Commands;
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

      private int m_gridLineSize = 1;
      public int GridLineSize
      {
          get
          {
             return m_gridLineSize;
          }
          set
          {
             m_gridLineSize = value;
             FirePropertyChanged("GridLineSize");
          }
      }

      private Size m_cellSize = new Size(16, 16);
      public Size CellSize
      {
          get
          {
             return m_cellSize;
          }
          set
          {
             m_cellSize = value;
             FirePropertyChanged("CellSize");
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

      public double ViewWidth
      {
         get
         {
            return (CellSize.Width + GridLineSize) * Dimensions.Width;
         }
      }

      public double ViewHeight
      {
         get
         {
            return (CellSize.Height + GridLineSize) * Dimensions.Height;
         }
      }

      private HashSet<GridCoordinate> m_selectedCells = new HashSet<GridCoordinate>();
      public HashSet<GridCoordinate> SelectedCells
      {
         get
         {
            return m_selectedCells;
         }
         set
         {
            m_selectedCells = value;
         }
      }

      public void SetPassability(ICollection<GridCoordinate> cells, double passability)
      {
         foreach (var cell in cells)
         {
            Map.BlockedCells[cell] = passability;
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
