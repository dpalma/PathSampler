﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using NUnit.Framework;
using PathSampler.Core;
using PathSampler.Models;
using PathSampler.ViewModels;
using PathSampler.Views;

namespace PathSamplerTests.ViewModels
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
         vm.SelectedPathingAlgorithm = typeof(PathSampler.PathFinders.AStar);
         return vm;
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

      [Test]
      public void TestStartPathingTwiceThrowsException()
      {
         MapVM vm = CreateDefaultMapVM(10);
         vm.StartPathing();
         // ExpectedMessage="Pathing already running"
         Assert.Throws<InvalidOperationException>(() => vm.StartPathing());
      }

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
      public void TestPathingAlgorithmsAreSortedAlphabetically()
      {
         var converter = new DisplayNameConverter();
         MapVM vm = CreateDefaultMapVM(5);

         var names = new List<string>();
         foreach (var x in vm.PathingAlgorithms)
         {
            string name = converter.Convert(x, typeof(string), null, null) as string;
            names.Add(name);
         }

         string last = String.Empty;
         foreach (var x in names)
         {
            Assert.IsTrue(x.CompareTo(last) > 0);
            last = x;
         }
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
                                      select t).First();
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
         MapVM vm = CreateFastPathingMapVM(4);
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
         Assert.AreEqual(2, vm.Cells.Count);
      }

      [Test]
      public void TestClearingCurrentPathDoesNotRemoveStartAndGoalCellVMs()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         vm.StartPathing();
         vm.ActivePathingTask.Wait();
         vm.CurrentPath = null;
         Assert.IsTrue(vm.HasCell(vm.Map.Goal));
         Assert.IsTrue(vm.HasCell(vm.Map.Start));
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
         Assert.AreEqual(0 + 2, vm.Cells.Count);
         vm.Map.BlockedCells.Add(vm.Map.GetCenter(), 1);
         Assert.AreEqual(1 + 2, vm.Cells.Count);
      }

      [Test]
      public void TestRemovingBlockedCellFromMapRemovesCorrespondingCellViewModel()
      {
         MapVM vm = CreateDefaultMapVM(5);
         vm.Map.BlockedCells.Add(vm.Map.GetCenter(), 1);
         Assert.AreEqual(1 + 2, vm.Cells.Count);
         Assert.IsTrue(vm.Map.BlockedCells.Remove(vm.Map.GetCenter()));
         Assert.AreEqual(0 + 2, vm.Cells.Count);
      }

      [Test]
      public void TestBlockingTheGoalCellDoesNotCreateRedundantCellVM()
      {
         MapVM vm = CreateDefaultMapVM(5);
         vm.Map.BlockedCells.Add(vm.Map.Goal, 1);
         Assert.AreEqual(2, vm.Cells.Count);
         Assert.IsTrue(vm.HasCell(vm.Map.Goal));
      }

      [Test]
      public void TestSettingCellColorAddsACellViewModel()
      {
         MapVM vm = CreateDefaultMapVM(15);
         int preCellCount = vm.Cells.Count;
         vm.SetCellColor(vm.Map.GetCenter(), PathSampler.PathFinders.CellColor.Open);
         Assert.IsTrue(vm.HasCell(vm.Map.GetCenter()));
         Assert.AreEqual(preCellCount + 1, vm.Cells.Count);
      }

      [Test]
      public void TestClearingCellColorsRemovesColoredCellViewModels()
      {
         MapVM vm = CreateDefaultMapVM(15);
         int preCellCount = vm.Cells.Count;
         var coloredCells = new List<GridCoordinate>() { vm.Map.GetCenter(), vm.Map.GetTopRight() }.AsReadOnly();
         foreach (var c in coloredCells)
         {
            vm.SetCellColor(c, PathSampler.PathFinders.CellColor.Open);
            Assert.IsTrue(vm.HasCell(c));
         }
         Assert.AreEqual(preCellCount + coloredCells.Count, vm.Cells.Count);
         vm.ClearCellColors();
         foreach (var c in coloredCells)
         {
            Assert.IsFalse(vm.HasCell(c));
         }
      }

      [Test]
      public void TestNewMapHasViewModelsForStartAndGoalCells()
      {
         MapVM vm = CreateDefaultMapVM(10);
         Assert.IsTrue(vm.HasCell(vm.Map.Goal));
         Assert.IsTrue(vm.HasCell(vm.Map.Start));
      }

      [Test]
      public void TestStartCellViewModelChangesWhenMapStartPropertyChanges()
      {
         MapVM vm = CreateDefaultMapVM(12);
         Assert.IsTrue(vm.HasCell(vm.Map.Start));
         GridCoordinate oldStart = vm.Map.Start;
         vm.Map.Start = vm.Map.GetBottomLeft();
         Assert.IsFalse(vm.HasCell(oldStart));
         Assert.IsTrue(vm.HasCell(vm.Map.Start));
      }

      [Test]
      public void TestGoalCellViewModelChangesWhenMapGoalPropertyChanges()
      {
         MapVM vm = CreateDefaultMapVM(12);
         Assert.IsTrue(vm.HasCell(vm.Map.Goal));
         GridCoordinate oldGoal = vm.Map.Goal;
         vm.Map.Goal = vm.Map.GetBottomLeft();
         Assert.IsFalse(vm.HasCell(oldGoal));
         Assert.IsTrue(vm.HasCell(vm.Map.Goal));
      }

      [Test]
      public void TestResettingMapClearsCellViewModels()
      {
         MapVM vm = CreateDefaultMapVM(5);
         vm.Map.Randomize();
         Assert.AreEqual(vm.Map.BlockedCells.Count + 2, vm.Cells.Count);
         vm.Map.Assign(new Map());
         Assert.AreEqual(0 + 2, vm.Cells.Count);
      }

      [Test]
      public void TestSetGoalCommandThrowsExceptionWhenMoreThanOneCellIsSelected()
      {
         MapVM vm = CreateDefaultMapVM(5);
         vm.SelectedCells.Add(new GridCoordinate() { Row = 0, Column = 1 });
         vm.SelectedCells.Add(new GridCoordinate() { Row = 0, Column = 2 });
         Assert.Throws<InvalidOperationException>(() => vm.SetGoalCommand.Execute(null));
      }

      [Test]
      public void TestSetStartCommandThrowsExceptionWhenMoreThanOneCellIsSelected()
      {
         MapVM vm = CreateDefaultMapVM(5);
         vm.SelectedCells.Add(new GridCoordinate() { Row = 1, Column = 1 });
         vm.SelectedCells.Add(new GridCoordinate() { Row = 1, Column = 2 });
         Assert.Throws<InvalidOperationException>(() => vm.SetStartCommand.Execute(null));
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
      public void TestIsPathingPropertyChangesWhenPathingStarts()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         Assert.IsFalse(vm.IsPathing);
         var propertiesChanged = new List<string>();
         vm.PropertyChanged += (object sender, PropertyChangedEventArgs eventArgs) =>
         {
            propertiesChanged.Add(eventArgs.PropertyName);
         };
         vm.StartPathing();
         Assert.IsTrue(vm.IsPathing);
         Assert.IsTrue(propertiesChanged.Contains("IsPathing"));
      }

      [Test]
      public void TestCanStartPathingPropertyChangesWhenPathingStarts()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         var propertiesChanged = new List<string>();
         vm.PropertyChanged += (object sender, PropertyChangedEventArgs eventArgs) =>
         {
            propertiesChanged.Add(eventArgs.PropertyName);
         };
         vm.StartPathing();
         Assert.IsTrue(propertiesChanged.Contains("CanStartPathing"));
      }

      [Test]
      public void TestIsPathingPropertyChangesWhenPathingStops()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         var propertiesChanged = new List<string>();
         vm.PropertyChanged += (object sender, PropertyChangedEventArgs eventArgs) =>
         {
            propertiesChanged.Add(eventArgs.PropertyName);
         };
         vm.StartPathing();
         vm.StopPathing();
         Assert.AreEqual(2, propertiesChanged.Count(x => x == "IsPathing"));
      }

      [Test]
      public void TestCanStartPathingPropertyChangesWhenPathingStops()
      {
         MapVM vm = CreateFastPathingMapVM(4);
         var propertiesChanged = new List<string>();
         vm.PropertyChanged += (object sender, PropertyChangedEventArgs eventArgs) =>
         {
            propertiesChanged.Add(eventArgs.PropertyName);
         };
         vm.StartPathing();
         vm.StopPathing();
         Assert.AreEqual(2, propertiesChanged.Count(x => x == "CanStartPathing"));
      }

      [Test]
      public void TestSettingPathingStepDelayTooLowThrowsException()
      {
         MapVM vm = CreateDefaultMapVM(5);
         Assert.Throws<ArgumentOutOfRangeException>(() => vm.PathingStepDelay = MapVM.PathingStepDelayMinimum.Subtract(TimeSpan.FromMinutes(1)));
      }

      [Test]
      public void TestSettingPathingStepDelayTooHighThrowsException()
      {
         MapVM vm = CreateDefaultMapVM(5);
         Assert.Throws<ArgumentOutOfRangeException>(() => vm.PathingStepDelay = MapVM.PathingStepDelayMaximum.Add(TimeSpan.FromMinutes(1)));
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

      [Test]
      public void TestAddingBlockCellClearsCurrentPath()
      {
         MapVM vm = CreateFastPathingMapVM(6);
         vm.StartPathing();
         vm.ActivePathingTask.Wait();
         Assert.IsTrue(vm.CurrentPath.Count > 0);
         vm.Map.BlockedCells.Add(vm.Map.GetCenter(), 1);
         Assert.IsNull(vm.CurrentPath);
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
