using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
      public MapVM(Map map)
      {
         Map = map;

         ColoredCells.CollectionChanged += new NotifyCollectionChangedEventHandler(ColoredCells_CollectionChanged);

         var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                 where t.IsClass && t.BaseType.Equals(typeof(PathFinder))
                 select t;
         q.ToList().ForEach(t => PathingAlgorithms.Add(t));

         SelectedPathingAlgorithm = (from t in PathingAlgorithms
                                     where t.Equals(typeof(BreadthFirstSearch))
                                     select t).Single();
      }

      void ColoredCells_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         FireRedrawRequested();
      }

      public Map Map
      {
         get
         {
            return m_map;
         }
         private set
         {
            if (value == null)
            {
               throw new ArgumentNullException("Map");
            }

            DisconnectMapEventHandlers();

            m_map = value;

            ConnectMapEventHandlers();
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
         FireRedrawRequested(); // TODO: this might not actually be needed

         if (e.Action == NotifyCollectionChangedAction.Add)
         {
            foreach (GridCoordinate cell in e.NewItems)
            {
               Cells.Add(new CellVM(this, cell) { Brush = Brushes.Black });
            }
         }
         else if (e.Action == NotifyCollectionChangedAction.Remove)
         {
            var toRemove = (from c in Cells
                            where e.OldItems.Contains(c.Cell)
                            select c).ToList();

            foreach (var cellVM in toRemove)
            {
               Cells.Remove(cellVM);
            }
         }
         else if (e.Action == NotifyCollectionChangedAction.Reset)
         {
            Cells.Clear();
         }
      }

      private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName == "BlockedCells")
         {
            DisconnectMapEventHandlers();
            ConnectMapEventHandlers();
         }
         else if (e.PropertyName == "RowCount")
         {
            FirePropertyChanged("MapHeight"); // MapHeight depends upon RowCount
         }
         else if (e.PropertyName == "ColumnCount")
         {
            FirePropertyChanged("MapWidth"); // MapWidth depends upon ColumnCount
         }

         StopPathing();

         FirePropertyChanged(e.PropertyName);
         FireRedrawRequested();
      }

      private void FireRedrawRequestedWhenOnProperThread()
      {
         if (RedrawRequested != null)
         {
            RedrawRequested(this, EventArgs.Empty);
         }
      }

      delegate void MethodInvoker();

      private void FireRedrawRequested()
      {
         if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
            FireRedrawRequestedWhenOnProperThread();
         else
            Application.Current.Dispatcher.BeginInvoke(new MethodInvoker(FireRedrawRequestedWhenOnProperThread));
      }

      public event EventHandler RedrawRequested;

      public event EventHandler PathingStarted;
      public event EventHandler PathingFinished;

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
            FirePropertyChanged("MapWidth"); // MapWidth depends upon GridLineSize
            FirePropertyChanged("MapHeight"); // MapHeight depends upon GridLineSize
         }
      }
      private int m_gridLineSize = 1;

      public double CellSizeScalar
      {
         get
         {
            return m_cellSizeScalar;
         }
         set
         {
            m_cellSizeScalar = value;
            CellSize = new Size(CellSizeScalar, CellSizeScalar);
            FirePropertyChanged("CellSizeScalar");
         }
      }
      private double m_cellSizeScalar = 16;

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
            FirePropertyChanged("MapWidth"); // MapWidth depends upon CellSize
            FirePropertyChanged("MapHeight"); // MapHeight depends upon CellSize
         }
      }
      private Size m_cellSize = new Size(16, 16);

      public double MapWidth
      {
         get
         {
            return (CellSize.Width + GridLineSize) * Map.ColumnCount;
         }
      }

      public double MapHeight
      {
         get
         {
            return (CellSize.Height + GridLineSize) * Map.RowCount;
         }
      }

      public ObservableCollection<CellVM> Cells
      {
         get { return m_cells; }
      }
      private ObservableCollection<CellVM> m_cells = new ObservableCollection<CellVM>();

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
                        t => { Map.Start = SelectedCells.Single(); },
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
                        t => { Map.Goal = SelectedCells.Single(); },
                        t => { return SelectedCells.Count == 1; });
            }
            return m_setGoalCommand;
         }
      }

      public ObservableCollection<Type> PathingAlgorithms
      {
         get
         {
            return m_pathingAlgorithms;
         }
      }
      ObservableCollection<Type> m_pathingAlgorithms = new ObservableCollection<Type>();

      public Type SelectedPathingAlgorithm
      {
         get
         {
            return m_selectedPathingAlgorithm;
         }
         set
         {
            m_selectedPathingAlgorithm = value;
            FirePropertyChanged("SelectedPathingAlgorithm");
         }
      }
      private Type m_selectedPathingAlgorithm;

      public PathFinder CurrentPathFinder { get; private set; }

      public bool IsPathing
      {
         get
         {
            if (ActivePathingTask == null)
            {
               return false;
            }
            else
            {
               return !ActivePathingTask.IsCanceled
                  && !ActivePathingTask.IsCompleted
                  && !ActivePathingTask.IsFaulted;
            }
         }
      }

      public Task ActivePathingTask
      {
         get { return (ActivePathingTaskCompletionSource != null) ? ActivePathingTaskCompletionSource.Task : null; }
      }
      internal TaskCompletionSource<object> ActivePathingTaskCompletionSource
      {
         get;
         private set;
      }

      public void StartPathing()
      {
         if (IsPathing)
         {
            throw new InvalidOperationException("Pathing already running");
         }

         ActivePathingTaskCompletionSource = StartPathingTask();

         if (PathingStarted != null)
         {
            PathingStarted(this, EventArgs.Empty);
         }
      }

      public bool CanStartPathing
      {
         get
         {
            return !IsPathing && Map.Goal != null && Map.Start != null;
         }
      }

      public TimeSpan PathingStepDelay
      {
         get
         {
            return m_pathingStepDelay;
         }
         set
         {
            if (value == TimeSpan.Zero)
            {
               throw new ArgumentException("Do not set pathing step delay to zero");
            }

            m_pathingStepDelay = value;
            FirePropertyChanged("PathingStepDelay");
         }
      }
      private TimeSpan m_pathingStepDelay = TimeSpan.FromMilliseconds(150);

      private TaskCompletionSource<object> StartPathingTask()
      {
         var constructor = SelectedPathingAlgorithm.GetConstructor(new Type[] { typeof(Map), typeof(ICellColoring) });

         CurrentPathFinder = (PathFinder)constructor.Invoke(new object[] { Map, (ICellColoring)this });

         TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

         var timer = new System.Threading.Timer(x =>
            {
               lock (CurrentPathFinder)
               {
                  CurrentPathFinder.Step();

                  if (CurrentPathFinder.Result != null)
                  {
                     ClearCellColors();

                     if (CurrentPathFinder.Result == PathFindResult.PathFound)
                     {
                        foreach (var cell in CurrentPathFinder.Path)
                        {
                           SetCellColor(cell, Brushes.MediumSeaGreen);
                        }
                     }
                     else
                     {
                        System.Diagnostics.Debug.WriteLine("No path");
                     }

                     if (PathingFinished != null)
                     {
                        PathingFinished(this, EventArgs.Empty);
                     }

                     tcs.SetResult(new object());
                  }
               }

            }, null, TimeSpan.Zero, PathingStepDelay);

         tcs.Task.ContinueWith(x =>
            {
               timer.Dispose();
            });

         return tcs;
      }

      internal void StopPathing()
      {
         if (ActivePathingTaskCompletionSource != null)
         {
            ActivePathingTaskCompletionSource.SetCanceled();
            ActivePathingTaskCompletionSource = null;
         }

         ClearCellColors();
      }

      internal bool CanStopPathing
      {
         get
         {
            return IsPathing;
         }
      }

      #region ICellColoring Implementation

      private ObservableDictionary<GridCoordinate, Brush> m_coloredCells = new ObservableDictionary<GridCoordinate, Brush>();

      public IObservableDictionary<GridCoordinate, Brush> ColoredCells
      {
         get { return m_coloredCells; }
      }

      public void SetCellColor(GridCoordinate cell, Brush brush)
      {
         if (Map.BlockedCells.ContainsKey(cell))
         {
            throw new InvalidOperationException("Attempting to color a blocked cell");
         }

         ColoredCells[cell] = brush;
      }

      private void ClearCellColors()
      {
         ColoredCells.Clear();
      }

      #endregion
   }
}
