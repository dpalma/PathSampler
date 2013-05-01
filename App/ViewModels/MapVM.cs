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
using PathSampler.Collections;
using PathSampler.Commands;
using PathSampler.Core;
using PathSampler.Models;
using PathSampler.PathFinders;

namespace PathSampler.ViewModels
{
   public class MapVM : ViewModel, ICellColoring
   {
      public MapVM(Map map)
      {
         Map = map;

         PathFinder.GatherPathingAlgorithms().ForEach(t => PathingAlgorithms.Add(t));

         SelectedPathingAlgorithm = (from t in PathingAlgorithms
                                     where t.Equals(typeof(BreadthFirstSearch))
                                     select t).Single();
      }

      public Map Map
      {
         get
         {
            return m_map;
         }
         internal set
         {
            if (value == null)
            {
               throw new ArgumentNullException("Map");
            }

            DisconnectMapEventHandlers();

            m_map = value;

            InitializeCells();

            FirePropertyChanged("Map");

            ConnectMapEventHandlers();
         }
      }

      private Map m_map;

      private void ConnectMapEventHandlers()
      {
         if (Map != null)
         {
            Map.PropertyChanged += new PropertyChangedEventHandler(Map_PropertyChanged);
            Map.PropertyChanging += new PropertyChangingEventHandler(Map_PropertyChanging);
            Map.BlockedCells.CollectionChanged += new NotifyCollectionChangedEventHandler(BlockedCells_CollectionChanged);
         }
      }

      private void DisconnectMapEventHandlers()
      {
         if (Map != null)
         {
            Map.PropertyChanged -= new PropertyChangedEventHandler(Map_PropertyChanged);
            Map.PropertyChanging -= new PropertyChangingEventHandler(Map_PropertyChanging);
            Map.BlockedCells.CollectionChanged -= new NotifyCollectionChangedEventHandler(BlockedCells_CollectionChanged);
         }
      }

      private void InitializeCells()
      {
         Cells.Clear();
         Cells.Add(new CellVM(this, Map.Goal));
         Cells.Add(new CellVM(this, Map.Start));
      }

      void BlockedCells_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         UpdateCells(e);
      }

      private void UpdateCells(NotifyCollectionChangedEventArgs e)
      {
         if (e.Action == NotifyCollectionChangedAction.Add)
         {
            AddCellVMs(e.NewItems);
         }
         else if (e.Action == NotifyCollectionChangedAction.Remove)
         {
            RemoveCellVMs(e.OldItems);
         }
         else if (e.Action == NotifyCollectionChangedAction.Reset)
         {
            PruneCellVMs();
         }
      }

      private void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         Map map = sender as Map;

         bool stopPathingTask = true;

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
         else if (e.PropertyName == "CellSizeScalar")
         {
            stopPathingTask = false;
            FirePropertyChanged("CellSize");
            FirePropertyChanged("MapWidth"); // MapWidth depends upon CellSize
            FirePropertyChanged("MapHeight"); // MapHeight depends upon CellSize
         }
         else if (e.PropertyName == "Start")
         {
            AddCellVM(new CellVM(this, map.Start));
         }
         else if (e.PropertyName == "Goal")
         {
            AddCellVM(new CellVM(this, map.Goal));
         }

         if (stopPathingTask)
         {
            StopPathing();
         }

         FirePropertyChanged(e.PropertyName);
         FireRedrawRequested();
      }

      private void Map_PropertyChanging(object sender, PropertyChangingEventArgs e)
      {
         Map map = sender as Map;

         if (e.PropertyName == "Start")
         {
            RemoveCellVMs(new List<GridCoordinate>() { map.Start });
         }
         else if (e.PropertyName == "Goal")
         {
            RemoveCellVMs(new List<GridCoordinate>() { map.Goal });
         }
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

      public GridCoordinate Goal
      {
         get
         {
            return Map.Goal;
         }
      }

      public GridCoordinate Start
      {
         get
         {
            return Map.Start;
         }
      }

      public int RowCount
      {
         get
         {
            return Map.RowCount;
         }
      }

      public int ColumnCount
      {
         get
         {
            return Map.ColumnCount;
         }
      }

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

      public Size CellSize
      {
         get
         {
            return new Size(Map.CellSizeScalar, Map.CellSizeScalar);
         }
      }

      public double MapWidth
      {
         get
         {
            return (CellSize.Width * Map.ColumnCount) + (GridLineSize * (Map.ColumnCount - 1));
         }
      }

      public double MapHeight
      {
         get
         {
            return (CellSize.Height * Map.RowCount) + (GridLineSize * (Map.RowCount - 1));
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

      private void PruneCellVMs()
      {
         if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
         {
            lock (Cells)
            {
               var toRemove = (from c in Cells
                               where !c.IsBlocked && !c.IsStart && !c.IsGoal && !c.IsOnPath
                               select c).ToList();
               foreach (var cellVM in toRemove)
               {
                  Cells.Remove(cellVM);
               }
            }
         }
         else
         {
            Application.Current.Dispatcher.BeginInvoke(new MethodInvoker(PruneCellVMs));
         }
      }

      private delegate void RemoveCellVMsDelegate(System.Collections.IList cells);

      private void RemoveCellVMs(System.Collections.IList cells)
      {
         if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
         {
            lock (Cells)
            {
               var toRemove = (from c in Cells
                               where cells.Contains(c.Cell)
                               select c).ToList();
               foreach (var cellVM in toRemove)
               {
                  Cells.Remove(cellVM);
               }
            }
         }
         else
         {
            Application.Current.Dispatcher.BeginInvoke(new RemoveCellVMsDelegate(RemoveCellVMs), cells);
         }
      }

      private delegate void AddCellVMsDelegate(System.Collections.IList cells);

      private void AddCellVMs(System.Collections.IList cells)
      {
         if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
         {
            lock (Cells)
            {
               var existing = (from c in Cells
                               where cells.Contains(c.Cell)
                               select c.Cell).ToList();
               foreach (var cell in cells)
               {
                  if (existing.Contains(cell))
                  {
                     continue;
                  }
                  Cells.Add(new CellVM(this, cell as GridCoordinate));
               }
            }
         }
         else
         {
            Application.Current.Dispatcher.BeginInvoke(new AddCellVMsDelegate(AddCellVMs), cells);
         }
      }

      private delegate void AddCellVMDelegate(CellVM cellVM);

      private void AddCellVM(CellVM cellVM)
      {
         if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
         {
            lock (Cells)
            {
               var existing = (from c in Cells
                               where c.Cell.Equals(cellVM.Cell)
                               select c.Cell).SingleOrDefault();
               if (existing == null)
               {
                  Cells.Add(cellVM);
               }
            }
         }
         else
         {
            Application.Current.Dispatcher.BeginInvoke(new AddCellVMDelegate(AddCellVM), cellVM);
         }
      }

      private static readonly List<GridCoordinate> EmptyPath = new List<GridCoordinate>();

      public List<GridCoordinate> CurrentPath
      {
         get
         {
            return m_currentPath;
         }
         set
         {
            if (CurrentPath != null)
            {
               var toRemove = new List<GridCoordinate>();
               foreach (var c in CurrentPath)
               {
                  if (c.Equals(Map.Goal) || c.Equals(Map.Start))
                  {
                     continue;
                  }
                  toRemove.Add(c);
               }
               RemoveCellVMs(toRemove);
            }

            m_currentPath = value;

            if (CurrentPath != null && CurrentPath.Count > 0)
            {
               var toAdd = new List<GridCoordinate>();
               foreach (var c in CurrentPath)
               {
                  if (c.Equals(Map.Goal) || c.Equals(Map.Start))
                  {
                     continue;
                  }
                  toAdd.Add(c);
               }
               AddCellVMs(toAdd);
            }

            FirePropertyChanged("CurrentPath");
         }
      }
      private List<GridCoordinate> m_currentPath = EmptyPath;

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
      internal System.Threading.Timer ActivePathingTaskTimer
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

         CurrentPath = EmptyPath;

         StartPathingTask();

         FirePropertyChanged("IsPathing");
         FirePropertyChanged("CanStartPathing");
         FirePropertyChanged("CanStopPathing");

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

      private static readonly TimeSpan m_pathingStepDelayMinimum = TimeSpan.FromMilliseconds(1);

      public static TimeSpan PathingStepDelayMinimum
      {
         get
         {
            return m_pathingStepDelayMinimum;
         }
      }

      private static readonly TimeSpan m_pathingStepDelayMaximum = TimeSpan.FromMilliseconds(401);

      public static TimeSpan PathingStepDelayMaximum
      {
         get
         {
            return m_pathingStepDelayMaximum;
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
            if (value.CompareTo(PathingStepDelayMinimum) < 0)
            {
               throw new ArgumentOutOfRangeException("PathingStepDelay", "The given value is too low");
            }

            if (value.CompareTo(PathingStepDelayMaximum) > 0)
            {
               throw new ArgumentOutOfRangeException("PathingStepDelay", "The given value is too high");
            }

            m_pathingStepDelay = value;

            if (ActivePathingTaskTimer != null)
            {
               ActivePathingTaskTimer.Change(PathingStepDelay, PathingStepDelay);
            }

            FirePropertyChanged("PathingStepDelay");
         }
      }
      private TimeSpan m_pathingStepDelay = TimeSpan.FromMilliseconds(150);

      private void StartPathingTask()
      {
         var constructor = SelectedPathingAlgorithm.GetConstructor(new Type[] { typeof(Map), typeof(ICellColoring) });

         CurrentPathFinder = (PathFinder)constructor.Invoke(new object[] { Map, (ICellColoring)this });

         TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

         ActivePathingTaskTimer = new System.Threading.Timer(x =>
            {
               lock (CurrentPathFinder)
               {
                  CurrentPathFinder.Step();

                  if (CurrentPathFinder.Result != null)
                  {
                     CurrentPath = (CurrentPathFinder.Path != null) ? CurrentPathFinder.Path.ToList() : EmptyPath;

                     OnPathingDone();

                     if (PathingFinished != null)
                     {
                        PathingFinished(this, EventArgs.Empty);
                     }

                     tcs.TrySetResult(null);
                  }
               }

            }, null, TimeSpan.Zero, PathingStepDelay);

         tcs.Task.ContinueWith(x =>
            {
               ActivePathingTaskTimer.Dispose();
               ActivePathingTaskTimer = null;
            });

         ActivePathingTaskCompletionSource = tcs;
      }

      public void StopPathing()
      {
         if (ActivePathingTaskCompletionSource != null)
         {
            ActivePathingTaskCompletionSource.TrySetCanceled();
         }

         OnPathingDone(true);
      }

      private void OnPathingDone(bool stopped = false)
      {
         ClearCellColors();

         FirePropertyChanged("IsPathing");
         FirePropertyChanged("CanStartPathing");
         FirePropertyChanged("CanStopPathing");
      }

      internal bool CanStopPathing
      {
         get
         {
            return IsPathing;
         }
      }

      private static readonly TimeSpan PathingStepDelayIncrement = TimeSpan.FromMilliseconds((PathingStepDelayMaximum.Milliseconds - PathingStepDelayMinimum.Milliseconds) / 8);

      internal void PathSlower()
      {
         PathingStepDelay = PathingStepDelay.Add(PathingStepDelayIncrement);
         if (PathingStepDelay.CompareTo(PathingStepDelayMaximum) > 0)
         {
            PathingStepDelay = PathingStepDelayMaximum;
         }
      }

      internal bool CanPathSlower
      {
         get
         {
            return true;
         }
      }

      internal void PathFaster()
      {
         PathingStepDelay = PathingStepDelay.Subtract(PathingStepDelayIncrement);
         if (PathingStepDelay.CompareTo(PathingStepDelayMinimum) < 0)
         {
            PathingStepDelay = PathingStepDelayMinimum;
         }
      }

      internal bool CanPathFaster
      {
         get
         {
            return true;
         }
      }

      #region ICellColoring Implementation

      public void SetCellColor(GridCoordinate cell, CellColor color)
      {
         if (Map.BlockedCells.ContainsKey(cell))
         {
            throw new InvalidOperationException("Attempting to color a blocked cell");
         }

         CellVM cellVM = null;

         lock (Cells)
         {
            cellVM = (from cvm in Cells
                      where cvm.Cell.Equals(cell)
                      select cvm).SingleOrDefault();
         }

         if (cellVM != null)
         {
            cellVM.CellColor = color;
         }
         else
         {
            AddCellVM(new CellVM(this, cell) { CellColor = color });
         }
      }

      public void ClearCellColors()
      {
         var toRemove = (from cvm in Cells
                         where cvm.CellColor.HasValue && !cvm.IsGoal && !cvm.IsStart && !cvm.IsOnPath
                         select cvm.Cell).ToList();
         RemoveCellVMs(toRemove);
      }

      #endregion
   }
}
