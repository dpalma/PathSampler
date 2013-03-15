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
   class MainWindowVMTests
   {
      [Test]
      public void TestStopPathingCanExecuteChangedFiresWhenGoalChanges()
      {
         MainWindowVM vm = new MainWindowVM();
         Map map = new Map();
         vm.Map = map;
         bool canExecuteChanged = false;
         vm.StopPathingCommand.CanExecuteChanged += (object sender, EventArgs e) => { canExecuteChanged = true; };
         map.Goal = map.Start;
         Assert.IsTrue(canExecuteChanged);
      }

      [Test]
      public void TestStopPathingCanExecuteChangedFiresOnStartingPathing()
      {
         MainWindowVM vm = new MainWindowVM();
         vm.Map = new Map();
         bool canExecuteChanged = false;
         vm.StopPathingCommand.CanExecuteChanged += (object sender, EventArgs e) => { canExecuteChanged = true; };
         vm.StartPathingCommand.Execute(null);
         Assert.IsTrue(canExecuteChanged);
      }

      [Test]
      public void TestChangingGoalStopsPathing()
      {
         MainWindowVM vm = new MainWindowVM();
         Map map = new Map();
         vm.Map = map;
         vm.StartPathingCommand.Execute(null);
         Assert.IsTrue(vm.MapVM.IsPathing);
         GridCoordinate cell = new GridCoordinate() { Row = map.RowCount / 2, Column = map.ColumnCount / 2 };
         map.Goal = cell;
         Assert.IsFalse(vm.MapVM.IsPathing);
      }

      [Test]
      [Ignore]
      public void TestPathingCommandsUpdateWhenPathFindingCompletes()
      {
         MainWindowVM vm = new MainWindowVM();
         vm.Map = new Map() { RowCount = 4, ColumnCount = 4, Goal = new GridCoordinate() { Row = 3, Column = 3 } };
         vm.StartPathingCommand.Execute(null);
         Assert.IsFalse(vm.StartPathingCommand.CanExecute(null));
         // Not even sure if this will ever work. Maybe path-finding should
         // be encapsulated in a Task.
         while (vm.MapVM.CurrentPathFinder.Result == null)
            vm.MapVM.CurrentPathFinder.Step();
         Assert.IsTrue(vm.StartPathingCommand.CanExecute(null));
      }
   }
}
