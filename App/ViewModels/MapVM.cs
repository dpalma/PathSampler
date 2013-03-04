using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PathFind.Commands;
using PathFind.Core;
using PathFind.Models;
using PathFind.PathFinders;

namespace PathFind.ViewModels
{
   public class MapVM : INotifyPropertyChanged, ICellColoring
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
            if (value == null)
            {
               throw new ArgumentNullException("Map");
            }

            if (Map != null)
            {
               Map.PropertyChanged -= new PropertyChangedEventHandler(Map_PropertyChanged);
            }

            m_map = value;

            Map.PropertyChanged += new PropertyChangedEventHandler(Map_PropertyChanged);

            FirePropertyChanged("Map");
         }
      }

      private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         FirePropertyChanged(e.PropertyName);
         FireRedrawRequested();
      }

      private void FireRedrawRequested()
      {
         if (RedrawRequested != null)
         {
            RedrawRequested(this, EventArgs.Empty);
         }
      }

      public event EventHandler RedrawRequested;

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
            return new Size(Map.ColumnCount, Map.RowCount);
         }
         set
         {
            Map.RowCount = (int)value.Height;
            Map.ColumnCount = (int)value.Width;
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

      public GridCoordinate GetHitCell(System.Windows.Input.MouseEventArgs mouseEventArgs)
      {
         FrameworkElement view = mouseEventArgs.Source as FrameworkElement;
         if (view == null)
         {
            return null;
         }

         Point mouse = mouseEventArgs.GetPosition(view);

         if (mouse.X < 0 || mouse.X > view.Width || mouse.Y < 0 || mouse.Y > view.Height)
         {
            return null;
         }

         double hitX = mouse.X / (CellSize.Width + GridLineSize);
         double hitY = mouse.Y / (CellSize.Height + GridLineSize);

         return new GridCoordinate() { Column = (int)hitX, Row = (int)hitY };
      }

      private HashSet<GridCoordinate> m_selectedCells = new HashSet<GridCoordinate>();
      internal HashSet<GridCoordinate> SelectedCells
      {
         get
         {
            return m_selectedCells;
         }
         private set
         {
            m_selectedCells = value;
            FirePropertyChanged("SelectedCells");
         }
      }

      public void AddSelectedCell(GridCoordinate cell)
      {
         if (SelectedCells.Add(cell))
         {
            if (RedrawRequested != null)
            {
               RedrawRequested(this, EventArgs.Empty);
            }
         }
      }

      private double m_passability = 1;
      public double Passability
      {
         get
         {
            return m_passability;
         }
         set
         {
            if (value < 0 || value > 1)
            {
               throw new ArgumentOutOfRangeException("Passability", "Passability values must be between 0 and 1");
            }
            m_passability = value;
            FirePropertyChanged("Passability");
         }
      }

      private void SetPassability()
      {
         foreach (var cell in SelectedCells)
         {
            Map.BlockedCells[cell] = Passability;
         }
      }

      private ICommand m_setPassabilityCommand;
      public ICommand SetPassabilityCommand
      {
         get
         {
            if (m_setPassabilityCommand == null)
            {
               m_setPassabilityCommand = new DelegateCommand(
                        t => { SetPassability(); },
                        t => { return SelectedCells.Count > 0; });
            }
            return m_setPassabilityCommand;
         }
      }

      private ICommand m_clearPassabilityCommand;
      public ICommand ClearPassabilityCommand
      {
         get
         {
            if (m_clearPassabilityCommand == null)
            {
               m_clearPassabilityCommand = new DelegateCommand(
                        t =>
                        {
                           foreach (var cell in SelectedCells)
                           {
                              Map.BlockedCells.Remove(cell);
                           }
                        },
                        t => { return SelectedCells.Count > 0; });
            }
            return m_clearPassabilityCommand;
         }
      }

      private ICommand m_setStartCommand;
      public ICommand SetStartCommand
      {
         get
         {
            if (m_setStartCommand == null)
            {
               m_setStartCommand = new DelegateCommand(
                        t => { Map.Start = SelectedCells.First(); },
                        t => { return SelectedCells.Count == 1; });
            }
            return m_setStartCommand;
         }
      }

      private ICommand m_setGoalCommand;
      public ICommand SetGoalCommand
      {
         get
         {
            if (m_setGoalCommand == null)
            {
               m_setGoalCommand = new DelegateCommand(
                        t => { Map.Goal = SelectedCells.First(); },
                        t => { return SelectedCells.Count == 1; });
            }
            return m_setGoalCommand;
         }
      }

      private DispatcherTimer m_timer;
      private PathFinder m_pathFinder;

      internal bool IsPathing
      {
         get { return m_timer != null; }
      }

      private void StartPathing()
      {
         if (m_timer != null)
         {
            throw new InvalidOperationException("Pathing already running");
         }

         m_timer = new DispatcherTimer();
         m_timer.Tick += new EventHandler(timer_Tick);
         m_timer.Interval = new TimeSpan(0, 0, 0, 0, 600);
         m_timer.Start();

         m_pathFinder = new BreadthFirstSearch(Map, this);
      }

      void timer_Tick(object sender, EventArgs e)
      {
         m_pathFinder.Step();

         if (m_pathFinder.Result != null)
         {
            System.Diagnostics.Debug.WriteLine("Pathfinding complete");
         }
      }

      private void StopPathing()
      {
         if (m_timer != null)
         {
            m_timer.Stop();
            m_timer = null;
         }

         ColoredCells.Clear();
      }

      private ICommand m_startPathingCommand;
      public ICommand StartPathingCommand
      {
         get
         {
            if (m_startPathingCommand == null)
            {
               m_startPathingCommand = new DelegateCommand(
                        t =>
                        {
                           StartPathing();
                        },
                        t =>
                        {
                           return Map.Goal != null && Map.Start != null;
                        });
            }
            return m_startPathingCommand;
         }
      }

      private ICommand m_stopPathingCommand;
      public ICommand StopPathingCommand
      {
         get
         {
            if (m_stopPathingCommand == null)
            {
               m_stopPathingCommand = new DelegateCommand(
                        t =>
                        {
                           StopPathing();
                        },
                        t =>
                        {
                           return IsPathing;
                        });
            }
            return m_stopPathingCommand;
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

      #region ICellColoring Implementation

      private Dictionary<GridCoordinate, Brush> m_coloredCells = new Dictionary<GridCoordinate,Brush>();

      public IDictionary<GridCoordinate, Brush> ColoredCells
      {
         get { return m_coloredCells; }
      }

      public void SetCellColor(GridCoordinate cell, Brush brush)
      {
         if (Map.BlockedCells.ContainsKey(cell))
         {
            throw new InvalidOperationException("Attempting to color a blocked cell");
         }

         m_coloredCells[cell] = brush;

         FireRedrawRequested();
      }

      #endregion
   }
}
