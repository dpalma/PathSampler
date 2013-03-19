using System;
using System.Collections.Generic;
using System.ComponentModel;
using PathFind.Core;
using PathFind.Collections;

namespace PathFind.Models
{
   [Serializable]
   public class Map : INotifyPropertyChanged
   {
      public const int DefaultRowColumnCount = 16;

      public int RowCount
      {
         get
         {
            return m_rowCount;
         }
         set
         {
            if (value < 0)
            {
               throw new ArgumentException("RowCount must be non-negative");
            }
            m_rowCount = value;
            FirePropertyChanged("RowCount");
         }
      }
      private int m_rowCount = DefaultRowColumnCount;

      public int ColumnCount
      {
         get
         {
            return m_columnCount;
         }
         set
         {
            if (value < 0)
            {
               throw new ArgumentException("ColumnCount must be non-negative");
            }
            m_columnCount = value;
            FirePropertyChanged("ColumnCount");
         }
      }
      private int m_columnCount = DefaultRowColumnCount;

      public IObservableDictionary<GridCoordinate, double> BlockedCells
      {
         get
         {
            return m_blockedCells;
         }
      }
      private readonly ObservableDictionary<GridCoordinate, double> m_blockedCells = new ObservableDictionary<GridCoordinate, double>();

      public GridCoordinate Start
      {
         get
         {
            return m_start;
         }
         set
         {
            if (value.Row < 0 || value.Row >= RowCount)
            {
               throw new ArgumentException("Start's row is outside the extents of the map");
            }
            if (BlockedCells.ContainsKey(value))
            {
               throw new ArgumentException(String.Format("Cell {0} is blocked and cannot be the start", value));
            }
            m_start = value;
            FirePropertyChanged("Start");
         }
      }
      private GridCoordinate m_start = new GridCoordinate() { Row = 0, Column = 0 };

      public GridCoordinate Goal
      {
         get
         {
            return m_goal;
         }
         set
         {
            if (value.Row < 0 || value.Row >= RowCount)
            {
               throw new ArgumentException("Goal's row is outside the extents of the map");
            }
            if (BlockedCells.ContainsKey(value))
            {
               throw new ArgumentException(String.Format("Cell {0} is blocked and cannot be the goal", value));
            }
            m_goal = value;
            FirePropertyChanged("Goal");
         }
      }
      private GridCoordinate m_goal = new GridCoordinate() { Row = DefaultRowColumnCount - 1, Column = DefaultRowColumnCount - 1 };

      private bool IsAtLeftEdge(GridCoordinate cell)
      {
         return cell.Column == 0;
      }

      private bool IsAtRightEdge(GridCoordinate cell)
      {
         return cell.Column == (ColumnCount - 1);
      }

      private bool IsAtTopEdge(GridCoordinate cell)
      {
         return cell.Row == 0;
      }

      private bool IsAtBottomEdge(GridCoordinate cell)
      {
         return cell.Row == (RowCount - 1);
      }

      public GridCoordinate[] GetNeighbors(GridCoordinate cell, bool allowDiagonals = true)
      {
         List<GridCoordinate> neighbors = new List<GridCoordinate>();

         bool atLeftEdge = IsAtLeftEdge(cell);
         bool atRightEdge = IsAtRightEdge(cell);

         if (!IsAtTopEdge(cell))
         {
            // top
            neighbors.Add(new GridCoordinate() { Row = cell.Row - 1, Column = cell.Column });

            if (allowDiagonals)
            {
               // top left
               if (!atLeftEdge)
                  neighbors.Add(new GridCoordinate() { Row = cell.Row - 1, Column = cell.Column - 1 });

               // top right
               if (!atRightEdge)
                  neighbors.Add(new GridCoordinate() { Row = cell.Row - 1, Column = cell.Column + 1 });
            }
         }

         if (!IsAtBottomEdge(cell))
         {
            // bottom
            neighbors.Add(new GridCoordinate() { Row = cell.Row + 1, Column = cell.Column });

            if (allowDiagonals)
            {
               // bottom left
               if (!atLeftEdge)
                  neighbors.Add(new GridCoordinate() { Row = cell.Row + 1, Column = cell.Column - 1 });

               // bottom right
               if (!atRightEdge)
                  neighbors.Add(new GridCoordinate() { Row = cell.Row + 1, Column = cell.Column + 1 });
            }
         }

         // left
         if (!atLeftEdge)
            neighbors.Add(new GridCoordinate() { Row = cell.Row, Column = cell.Column - 1 });

         // right
         if (!atRightEdge)
            neighbors.Add(new GridCoordinate() { Row = cell.Row, Column = cell.Column + 1 });

         neighbors.RemoveAll(c => BlockedCells.ContainsKey(c));

         return neighbors.ToArray();
      }

      public void Assign(Map other)
      {
         this.RowCount = other.RowCount;
         this.ColumnCount = other.ColumnCount;
         this.BlockedCells.Clear();
         foreach (var item in other.BlockedCells)
         {
            this.BlockedCells.Add(item);
         }
         this.Goal = other.Goal;
         this.Start = other.Start;
      }

      #region INotifyPropertyChanged Members

      [field:NonSerialized]
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
