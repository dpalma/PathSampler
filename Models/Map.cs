using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace PathFind.Models
{
   [Serializable]
   public class Map : INotifyPropertyChanged
   {
      public const int DefaultDimension = 16;

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

      private bool IsAtLeftEdge(GridCoordinate cell)
      {
         return cell.Column == 0;
      }

      private bool IsAtRightEdge(GridCoordinate cell)
      {
         return cell.Column == (Dimensions.Width - 1);
      }

      private bool IsAtTopEdge(GridCoordinate cell)
      {
         return cell.Row == 0;
      }

      private bool IsAtBottomEdge(GridCoordinate cell)
      {
         return cell.Row == (Dimensions.Height - 1);
      }

      public GridCoordinate[] GetNeighbors(GridCoordinate cell)
      {
         List<GridCoordinate> neighbors = new List<GridCoordinate>();

         bool atLeftEdge = IsAtLeftEdge(cell);
         bool atRightEdge = IsAtRightEdge(cell);

         if (!IsAtTopEdge(cell))
         {
            // top
            neighbors.Add(new GridCoordinate() { Row = cell.Row - 1, Column = cell.Column });

            // top left
            if (!atLeftEdge)
               neighbors.Add(new GridCoordinate() { Row = cell.Row - 1, Column = cell.Column - 1 });

            // top right
            if (!atRightEdge)
               neighbors.Add(new GridCoordinate() { Row = cell.Row - 1, Column = cell.Column + 1 });
         }

         if (!IsAtBottomEdge(cell))
         {
            // bottom
            neighbors.Add(new GridCoordinate() { Row = cell.Row + 1, Column = cell.Column });

            // bottom left
            if (!atLeftEdge)
               neighbors.Add(new GridCoordinate() { Row = cell.Row + 1, Column = cell.Column - 1 });

            // bottom right
            if (!atRightEdge)
               neighbors.Add(new GridCoordinate() { Row = cell.Row + 1, Column = cell.Column + 1 });
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
         this.Dimensions = other.Dimensions;
         this.BlockedCells = other.BlockedCells;
         this.Goal = other.Goal;
         this.Start = other.Start;
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
