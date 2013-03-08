using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.Core;
using PathFind.Models;
using PathFind.ViewModels;

namespace PathFindTests.ViewModels
{
   class MapVMTests
   {
      private Map map;
      private MapVM vm;

      [SetUp]
      public void SetUp()
      {
         map = new Map();
         vm = new MapVM();
         vm.Map = map;
      }

      [Test]
      public void TestMapChangeTriggersRedrawRequest()
      {
         var redrawRequests = new List<EventArgs>();
         vm.RedrawRequested += (sender, eventArgs) =>
            {
               redrawRequests.Add(eventArgs);
            };
         GridCoordinate cell = new GridCoordinate() { Row = 1, Column = 1 };
         map.BlockedCells[cell] = 1;
         Assert.AreEqual(1, redrawRequests.Count);
      }

      [Test]
      public void TestStopPathingCanExecuteChangedFiresWhenGoalChanges()
      {
         bool canExecuteChanged = false;
         vm.StopPathingCommand.CanExecuteChanged += (object sender, EventArgs e) => { canExecuteChanged = true; };
         map.Goal = map.Start;
         Assert.IsTrue(canExecuteChanged);
      }

      [Test]
      public void TestStopPathingCanExecuteChangedFiresOnStartingPathing()
      {
         bool canExecuteChanged = false;
         vm.StopPathingCommand.CanExecuteChanged += (object sender, EventArgs e) => { canExecuteChanged = true; };
         vm.StartPathingCommand.Execute(null);
         Assert.IsTrue(canExecuteChanged);
      }

      [Test]
      public void TestSelectSameCellTwiceOnlyAddsOnce()
      {
         GridCoordinate cell = new GridCoordinate() { Row = map.RowCount / 2, Column = map.ColumnCount / 2 };
         vm.SelectedCells.Add(cell);
         vm.SelectedCells.Add(cell);
         Assert.AreEqual(1, vm.SelectedCells.Count);
      }

      [Test]
      public void TestChangingGoalStopsPathing()
      {
         vm.StartPathingCommand.Execute(null);
         Assert.IsTrue(vm.IsPathing);
         GridCoordinate cell = new GridCoordinate() { Row = map.RowCount / 2, Column = map.ColumnCount / 2 };
         map.Goal = cell;
         Assert.IsFalse(vm.IsPathing);
      }

      //[Test]
      //public void TestStartPathingTwiceDoesntThrowException()
      //{
      //   vm.StartPathingCommand.Execute(null);
      //   vm.StartPathingCommand.Execute(null);
      //}
   }
}
