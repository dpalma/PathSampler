﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PathSampler.Core;
using PathSampler.Collections;

namespace PathSampler.Models
{
   [Serializable]
   public class Map : INotifyPropertyChanging, INotifyPropertyChanged
   {
      public const int DefaultRowColumnCount = 16;

      public const double DefaultCellSizeScalar = 16;
      public static double MinimumCellSizeScalar { get { return 4; } }
      public static double MaximumCellSizeScalar { get { return 50; } }

      public int RowCount
      {
         get
         {
            return m_rowCount;
         }
         set
         {
            if (value == RowCount)
            {
               return;
            }
            if (value <= 0)
            {
               throw new ArgumentException("RowCount must be greater than zero");
            }
            bool goalAtBottomEdge = (Goal.Row == RowCount - 1);
            bool startAtBottomEdge = (Start.Row == RowCount - 1);
            FirePropertyChanging("RowCount");
            m_rowCount = value;
            FirePropertyChanged("RowCount");
            if (goalAtBottomEdge)
            {
               var newGoal = new GridCoordinate() { Column = Goal.Column, Row = RowCount - 1 };
               BlockedCells.Remove(newGoal);
               Goal = newGoal;
            }
            if (startAtBottomEdge)
            {
               var newStart  = new GridCoordinate() { Column = Start.Column, Row = RowCount - 1 };
               BlockedCells.Remove(newStart);
               Start = newStart;
            }
            RemoveOutOfBoundsBlockedCells();
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
            if (value == ColumnCount)
            {
               return;
            }
            if (value <= 0)
            {
               throw new ArgumentException("ColumnCount must be greater than zero");
            }
            bool goalAtRightEdge = (Goal.Column == ColumnCount - 1);
            bool startAtRightEdge = (Start.Column == ColumnCount - 1);
            FirePropertyChanging("ColumnCount");
            m_columnCount = value;
            FirePropertyChanged("ColumnCount");
            if (goalAtRightEdge)
            {
               var newGoal = new GridCoordinate() { Column = ColumnCount - 1, Row = Goal.Row };
               BlockedCells.Remove(newGoal);
               Goal = newGoal;
            }
            if (startAtRightEdge)
            {
               var newStart = new GridCoordinate() { Column = ColumnCount - 1, Row = Start.Row };
               BlockedCells.Remove(newStart);
               Start = newStart;
            }
            RemoveOutOfBoundsBlockedCells();
         }
      }
      private int m_columnCount = DefaultRowColumnCount;

      private void RemoveOutOfBoundsBlockedCells()
      {
         var toRemove = (from k in BlockedCells.Keys
                         where !IsInBounds(k)
                         select k).ToList();
         foreach (var c in toRemove)
         {
            BlockedCells.Remove(c);
         }
      }

      public bool IsInBounds(GridCoordinate cell)
      {
         return cell.Row >= 0 && cell.Row < RowCount
            && cell.Column >= 0 && cell.Column < ColumnCount;
      }

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
            if (value == null)
            {
               throw new ArgumentNullException("Start");
            }
            if (value.Equals(Start))
            {
               return;
            }
            if (!IsInBounds(value))
            {
               throw new ArgumentException("Cell is out of bounds");
            }
            if (BlockedCells.ContainsKey(value))
            {
               throw new ArgumentException(String.Format("Cell {0} is blocked and cannot be the start", value));
            }
            FirePropertyChanging("Start");
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
            if (value == null)
            {
               throw new ArgumentNullException("Goal");
            }
            if (value.Equals(Goal))
            {
               return;
            }
            if (!IsInBounds(value))
            {
               throw new ArgumentException("Cell is out of bounds");
            }
            if (BlockedCells.ContainsKey(value))
            {
               throw new ArgumentException(String.Format("Cell {0} is blocked and cannot be the goal", value));
            }
            FirePropertyChanging("Goal");
            m_goal = value;
            FirePropertyChanged("Goal");
         }
      }
      private GridCoordinate m_goal = new GridCoordinate() { Row = DefaultRowColumnCount - 1, Column = DefaultRowColumnCount - 1 };

      public double CellSizeScalar
      {
         get
         {
            return m_cellSizeScalar;
         }
         set
         {
            if (CellSizeScalar == value)
            {
               return;
            }
            if (value < MinimumCellSizeScalar)
            {
               throw new ArgumentOutOfRangeException("CellSizeScalar", String.Format("Map.CellSizeScalar cannot be less than {0}", MinimumCellSizeScalar));
            }
            if (value > MaximumCellSizeScalar)
            {
               throw new ArgumentOutOfRangeException("CellSizeScalar", String.Format("Map.CellSizeScalar cannot be more than {0}", MaximumCellSizeScalar));
            }
            FirePropertyChanging("CellSizeScalar");
            m_cellSizeScalar = value;
            FirePropertyChanged("CellSizeScalar");
         }
      }
      private double m_cellSizeScalar = DefaultCellSizeScalar;

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

         neighbors.Sort((a, b) =>
            {
               return (int)(a.EuclideanDistanceSquared(Goal) - b.EuclideanDistanceSquared(Goal));
            });

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
         this.CellSizeScalar = other.CellSizeScalar;
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

      #region INotifyPropertyChanging Members

      [field: NonSerialized]
      public event PropertyChangingEventHandler PropertyChanging;

      #endregion

      private void FirePropertyChanging(string propertyName)
      {
         if (PropertyChanging != null)
         {
            PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
         }
      }
   }
}
