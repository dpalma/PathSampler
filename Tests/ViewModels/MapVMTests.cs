using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using NUnit.Framework;
using PathFind.Core;
using PathFind.Models;
using PathFind.ViewModels;

namespace PathFindTests.ViewModels
{
   class MapVMTests
   {
      private static MapVM CreateDefaultMapVM(int mapSize)
      {
         return new MapVM(MapUtilsForTesting.BuildMap(mapSize));
      }

      private static MapVM CreateFastPathingMapVM(int mapSize)
      {
         MapVM vm = CreateDefaultMapVM(mapSize);
         vm.PathingStepDelay = TimeSpan.FromMilliseconds(1);
         vm.SelectedPathingAlgorithm = typeof(PathFind.PathFinders.AStar);
         return vm;
      }

      [Test]
      public void TestMapChangeTriggersRedrawRequest()
      {
         MapVM vm = CreateDefaultMapVM(8);
         var redrawRequests = new List<EventArgs>();
         vm.RedrawRequested += (sender, eventArgs) =>
            {
               redrawRequests.Add(eventArgs);
            };
         GridCoordinate cell = new GridCoordinate() { Row = 1, Column = 1 };
         vm.Map.BlockedCells[cell] = 1;
         Assert.AreEqual(1, redrawRequests.Count);
      }

      [Test]
      public void TestSelectSameCellTwiceOnlyAddsOnce()
      {
         MapVM vm = CreateDefaultMapVM(8);
         GridCoordinate cell = new GridCoordinate() { Row = vm.Map.RowCount / 2, Column = vm.Map.ColumnCount / 2 };
         vm.SelectedCells.Add(cell);
         vm.SelectedCells.Add(cell);
         Assert.AreEqual(1, vm.SelectedCells.Count);
      }

      //[Test]
      //public void TestStartPathingTwiceDoesntThrowException()
      //{
      //   mapVM.StartPathingCommand.Execute(null);
      //   mapVM.StartPathingCommand.Execute(null);

      [Test]
      public void TestPathingAlgorithmImplementationsAreCollected()
      {
         MapVM vm = CreateDefaultMapVM(5);
         Assert.IsTrue(vm.PathingAlgorithms.Count > 0);
      }

      [Test]
      public void TestThereIsADefaultPathingAlgorithmSelected()
      {
         MapVM vm = CreateDefaultMapVM(5);
         Assert.IsNotNull(vm.SelectedPathingAlgorithm);
      }

      [Test]
      public void TestDefaultPathingAlgorithmIsBFS()
      {
         MapVM vm = CreateDefaultMapVM(5);
         Assert.AreEqual("BreadthFirstSearch", vm.SelectedPathingAlgorithm.Name);
      }

      [Test]
      public void TestPathingUsesSelectedAlgorithm()
      {
         MapVM vm = CreateDefaultMapVM(5);
         var otherPathingAlgorithm = (from t in vm.PathingAlgorithms
                                      where !t.Equals(vm.SelectedPathingAlgorithm)
                                      select t).Single();
         Assert.AreNotEqual(otherPathingAlgorithm, vm.SelectedPathingAlgorithm);
         vm.SelectedPathingAlgorithm = otherPathingAlgorithm;
         vm.StartPathing();
         Assert.AreEqual(otherPathingAlgorithm, vm.CurrentPathFinder.GetType());
         vm.StopPathing();
      }

      private const int PathTaskTimeout = 10000;

      [Test, MaxTime(PathTaskTimeout)]
      public void TestPathingTaskCompletes()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         vm.StartPathing();
         Assert.IsNotNull(vm.ActivePathingTask);
         vm.ActivePathingTask.Wait();
      }

      [Test, MaxTime(PathTaskTimeout)]
      public void TestIsPathingIsFalseWhenPathingTaskCompletes()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         vm.StartPathing();
         Assert.IsTrue(vm.IsPathing);
         vm.ActivePathingTask.Wait();
         Assert.IsFalse(vm.IsPathing);
      }

      [Test]
      public void TestChangingGoalStopsPathing()
      {
         MapVM vm = CreateFastPathingMapVM(5);
         vm.StartPathing();
         Assert.IsTrue(vm.IsPathing);
         vm.Map.Goal = vm.Map.GetCenter();
         Assert.IsFalse(vm.IsPathing);
      }

      [Test, MaxTime(PathTaskTimeout)]
      public void TestPathingTaskSetsCurentPathProperty()
      {
         MapVM vm = CreateDefaultMapVM(4);
         vm.PathingStepDelay = TimeSpan.FromMilliseconds(1);
         vm.StartPathing();
         vm.ActivePathingTask.Wait();
         int diagonal = (int)vm.Map.Start.EuclideanDistance(vm.Map.Goal);
         Assert.AreEqual(diagonal, vm.CurrentPath.Count);
      }

      [Test, MaxTime(PathTaskTimeout)]
      public void TestCurrentPathIsEmptyListWhenBlocked()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         vm.Map.BlockRow(vm.Map.GetCenter().Row);
         vm.StartPathing();
         vm.ActivePathingTask.Wait();
         Assert.AreEqual(0, vm.CurrentPath.Count);
      }

      [Test, MaxTime(PathTaskTimeout)]
      public void TestCurrentPathIncludesStartAndGoal()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         vm.StartPathing();
         vm.ActivePathingTask.Wait();
         Assert.IsTrue(vm.CurrentPath.Contains(vm.Map.Start));
         Assert.IsTrue(vm.CurrentPath.Contains(vm.Map.Goal));
      }

      [Test, MaxTime(PathTaskTimeout)]
      public void TestCurrentPathIsRepresentedInCellsCollection()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         vm.StartPathing();
         vm.ActivePathingTask.Wait();
         Assert.AreEqual(vm.CurrentPath.Count, vm.Cells.Count);
         foreach (var c in vm.CurrentPath)
         {
            Assert.IsTrue(vm.HasCell(c));
         }
      }

      [Test, MaxTime(PathTaskTimeout)]
      public void TestOldPathCellVMsAreClearedByNewPath()
      {
         MapVM vm = CreateFastPathingMapVM(4);

         vm.Map.Start = vm.Map.GetTopLeft();
         vm.Map.Goal = vm.Map.GetBottomLeft();
         vm.StartPathing();
         vm.ActivePathingTask.Wait();
         List<GridCoordinate> oldPath = vm.CurrentPath;

         vm.Map.Start = vm.Map.GetTopRight();
         vm.Map.Goal = vm.Map.GetBottomRight();
         vm.StartPathing();
         vm.ActivePathingTask.Wait();

         Assert.AreEqual(vm.CurrentPath.Count, vm.Cells.Count);
         foreach (var c in oldPath)
         {
            Assert.IsFalse(vm.HasCell(c));
         }
         foreach (var c in vm.CurrentPath)
         {
            Assert.IsTrue(vm.HasCell(c));
         }
      }

      [Test]
      public void TestSettingCurrentPathNullRemovesCellVMs()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         vm.StartPathing();
         vm.ActivePathingTask.Wait();
         Assert.IsTrue(vm.CurrentPath.Count > 0);
         Assert.AreEqual(vm.CurrentPath.Count, vm.Cells.Count);
         vm.CurrentPath = null;
         Assert.AreEqual(0, vm.Cells.Count);
      }

      [Test]
      public void TestMapWidthAndHeightChangeWhenCellSizeChanges()
      {
         MapVM vm = CreateDefaultMapVM(5);
         var propertiesChanged = new List<string>();
         vm.PropertyChanged += (object sender, PropertyChangedEventArgs eventArgs) =>
         {
            propertiesChanged.Add(eventArgs.PropertyName);
         };
         vm.Map.CellSizeScalar = 40;
         Assert.IsTrue(propertiesChanged.Contains("CellSize"), "No change notification for CellSize property");
         Assert.IsTrue(propertiesChanged.Contains("MapWidth"), "No change notification for MapWidth property");
         Assert.IsTrue(propertiesChanged.Contains("MapHeight"), "No change notification for MapHeight property");
      }

      [Test]
      public void TestMapWidthAndHeightChangeWhenGridLineSizeChanges()
      {
         MapVM vm = CreateDefaultMapVM(5);
         var propertiesChanged = new List<string>();
         vm.PropertyChanged += (object sender, PropertyChangedEventArgs eventArgs) =>
         {
            propertiesChanged.Add(eventArgs.PropertyName);
         };
         vm.GridLineSize = 8;
         Assert.IsTrue(propertiesChanged.Contains("GridLineSize"));
         Assert.IsTrue(propertiesChanged.Contains("MapWidth"));
         Assert.IsTrue(propertiesChanged.Contains("MapHeight"));
      }

      [Test]
      public void TestOnlyMapHeightChangesWhenRowCountChanges()
      {
         MapVM vm = CreateDefaultMapVM(5);
         var propertiesChanged = new List<string>();
         vm.PropertyChanged += (object sender, PropertyChangedEventArgs eventArgs) =>
         {
            propertiesChanged.Add(eventArgs.PropertyName);
         };
         vm.Map.RowCount = 128;
         Assert.IsFalse(propertiesChanged.Contains("MapWidth"));
         Assert.IsTrue(propertiesChanged.Contains("MapHeight"));
      }

      [Test]
      public void TestOnlyMapWidthChangesWhenColumnCountChanges()
      {
         MapVM vm = CreateDefaultMapVM(5);
         var propertiesChanged = new List<string>();
         vm.PropertyChanged += (object sender, PropertyChangedEventArgs eventArgs) =>
         {
            propertiesChanged.Add(eventArgs.PropertyName);
         };
         vm.Map.ColumnCount = 128;
         Assert.IsTrue(propertiesChanged.Contains("MapWidth"));
         Assert.IsFalse(propertiesChanged.Contains("MapHeight"));
      }

      [Test]
      public void TestAddingBlockedCellToMapAddsACellViewModel()
      {
         MapVM vm = CreateDefaultMapVM(5);
         Assert.AreEqual(0, vm.Cells.Count);
         vm.Map.BlockedCells.Add(vm.Map.GetCenter(), 1);
         Assert.AreEqual(1, vm.Cells.Count);
      }

      [Test]
      public void TestRemovingBlockedCellFromMapRemovesCorrespondingCellViewModel()
      {
         MapVM vm = CreateDefaultMapVM(5);
         vm.Map.BlockedCells.Add(vm.Map.GetCenter(), 1);
         Assert.AreEqual(1, vm.Cells.Count);
         Assert.IsTrue(vm.Map.BlockedCells.Remove(vm.Map.GetCenter()));
         Assert.AreEqual(0, vm.Cells.Count);
      }

      [Test]
      public void TestResettingMapClearsCellViewModels()
      {
         MapVM vm = CreateDefaultMapVM(5);
         vm.Map.Randomize();
         Assert.AreEqual(vm.Map.BlockedCells.Count, vm.Cells.Count);
         vm.Map.Assign(new Map());
         Assert.AreEqual(0, vm.Cells.Count);
      }

      [Test]
      [ExpectedException(typeof(InvalidOperationException))]
      public void TestSetGoalCommandThrowsExceptionWhenMoreThanOneCellIsSelected()
      {
         MapVM vm = CreateDefaultMapVM(5);
         vm.SelectedCells.Add(new GridCoordinate() { Row = 0, Column = 1 });
         vm.SelectedCells.Add(new GridCoordinate() { Row = 0, Column = 2 });
         vm.SetGoalCommand.Execute(null);
      }

      [Test]
      [ExpectedException(typeof(InvalidOperationException))]
      public void TestSetStartCommandThrowsExceptionWhenMoreThanOneCellIsSelected()
      {
         MapVM vm = CreateDefaultMapVM(5);
         vm.SelectedCells.Add(new GridCoordinate() { Row = 1, Column = 1 });
         vm.SelectedCells.Add(new GridCoordinate() { Row = 1, Column = 2 });
         vm.SetStartCommand.Execute(null);
      }

      [Test, MaxTime(PathTaskTimeout)]
      public void TestChangingCellSizeDoesNotStopPathing_Issue8()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         vm.StartPathing();
         vm.Map.CellSizeScalar = 30;
         Assert.IsFalse(vm.ActivePathingTask.IsCanceled);
      }

      [Test]
      public void TestSelectedCellsBinding()
      {
         MapVM vm = CreateDefaultMapVM(5);
         Target target = new Target();
         BindingOperations.SetBinding(target, Target.SelectedCellsProperty,
            new Binding()
               {
                  Source = vm,
                  Path = new PropertyPath("SelectedCells", null),
               });
         Assert.AreEqual(0, target.SelectedCells.Count);
         vm.SelectedCells.Add(vm.Map.GetCenter());
         Assert.AreEqual(1, target.SelectedCells.Count);
      }
   }

   sealed class Target : DependencyObject
   {
      public static readonly DependencyProperty SelectedCellsProperty = DependencyProperty.Register("SelectedCells", typeof(ICollection<GridCoordinate>), typeof(Target));
      public ICollection<GridCoordinate> SelectedCells
      {
         get { return (ICollection<GridCoordinate>)GetValue(SelectedCellsProperty); }
         set { SetValue(SelectedCellsProperty, value); }
      }
   }
}
