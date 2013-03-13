﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PathFind.Collections;
using PathFind.Commands;
using PathFind.Core;
using PathFind.Models;
using PathFind.PathFinders;

namespace PathFind.ViewModels
{
   public class MapVM : ViewModel, ICellColoring
   {
      public MapVM()
      {
      }

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

            DisconnectMapEventHandlers();

            m_map = value;

            ConnectMapEventHandlers();

            FirePropertyChanged("Map");
         }
      }
      private Map m_map;

      private void ConnectMapEventHandlers()
      {
         if (Map != null)
         {
            Map.PropertyChanged += new PropertyChangedEventHandler(Map_PropertyChanged);
            Map.BlockedCells.CollectionChanged += new NotifyCollectionChangedEventHandler(BlockedCells_CollectionChanged);
         }
      }

      private void DisconnectMapEventHandlers()
      {
         if (Map != null)
         {
            Map.PropertyChanged -= new PropertyChangedEventHandler(Map_PropertyChanged);
            Map.BlockedCells.CollectionChanged -= new NotifyCollectionChangedEventHandler(BlockedCells_CollectionChanged);
         }
      }

      void BlockedCells_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         FireRedrawRequested();
      }

      private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName == "BlockedCells")
         {
            DisconnectMapEventHandlers();
            ConnectMapEventHandlers();
         }

         StopPathing();

         FirePropertyChanged(e.PropertyName);
         FireRedrawRequested();
      }

      private void UpdateCommands()
      {
         if (m_stopPathingCommand != null)
         {
            m_stopPathingCommand.RaiseCanExecuteChanged();
         }
         if (m_startPathingCommand != null)
         {
            m_startPathingCommand.RaiseCanExecuteChanged();
         }
      }

      private void FireRedrawRequested()
      {
         if (RedrawRequested != null)
         {
            RedrawRequested(this, EventArgs.Empty);
         }
      }

      public event EventHandler RedrawRequested;

      public Size Dimensions
      {
         get
         {
            return new Size(Map.ColumnCount, Map.RowCount);
         }
      }

      private readonly ObservableSet<GridCoordinate> m_selectedCells = new ObservableSet<GridCoordinate>();
      public IObservableSet<GridCoordinate> SelectedCells
      {
         get
         {
            return m_selectedCells;
         }
      }

      private Brush m_selectedCellBrush = Brushes.Tan;
      public Brush SelectedCellBrush
      {
         get
         {
            return m_selectedCellBrush;
         }
         set
         {
            m_selectedCellBrush = value;
            FirePropertyChanged("SelectedCellBrush");
         }
      }

      private Brush m_startCellBrush = Brushes.Green;
      public Brush StartCellBrush
      {
         get
         {
            return m_startCellBrush;
         }
         set
         {
            m_startCellBrush = value;
            FirePropertyChanged("StartCellBrush");
         }
      }

      private Brush m_goalCellBrush = Brushes.Red;
      public Brush GoalCellBrush
      {
         get
         {
            return m_goalCellBrush;
         }
         set
         {
            m_goalCellBrush = value;
            FirePropertyChanged("GoalCellBrush");
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

      private DelegateCommand m_setPassabilityCommand;
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

      private DelegateCommand m_clearPassabilityCommand;
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

      private DelegateCommand m_setStartCommand;
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

      private DelegateCommand m_setGoalCommand;
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

      public bool IsPathing
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
         m_timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
         m_timer.Start();

         m_pathFinder = new BreadthFirstSearch(Map, this);

         UpdateCommands();
      }

      public bool CanStartPathing
      {
         get
         {
            return !IsPathing && Map.Goal != null && Map.Start != null;
         }
      }

      void timer_Tick(object sender, EventArgs e)
      {
         m_pathFinder.Step();

         if (m_pathFinder.Result != null)
         {
            StopPathing();
            if (m_pathFinder.Result == PathFindResult.PathFound)
            {
               foreach (var cell in m_pathFinder.Path)
               {
                  this.ColoredCells[cell] = Brushes.MediumSeaGreen;
               }
            }
            else
            {
               System.Diagnostics.Debug.WriteLine("No path");
            }
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

         UpdateCommands();

         FireRedrawRequested();
      }

      private DelegateCommand m_startPathingCommand;
      public ICommand StartPathingCommand
      {
         get
         {
            if (m_startPathingCommand == null)
            {
               m_startPathingCommand = new DelegateCommand(
                        t => { StartPathing(); },
                        t => { return CanStartPathing; });
            }
            return m_startPathingCommand;
         }
      }

      private DelegateCommand m_stopPathingCommand;
      public ICommand StopPathingCommand
      {
         get
         {
            if (m_stopPathingCommand == null)
            {
               m_stopPathingCommand = new DelegateCommand(
                        t => { StopPathing(); },
                        t => { return IsPathing; });
            }
            return m_stopPathingCommand;
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
