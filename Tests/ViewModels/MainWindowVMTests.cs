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
      public void TestPathingAlgorithmImplementationsAreCollected()
      {
         MainWindowVM vm = new MainWindowVM();
         Assert.IsTrue(vm.PathingAlgorithms.Count > 0);
      }

      [Test]
      public void TestThereIsADefaultPathingAlgorithmSelected()
      {
         MainWindowVM vm = new MainWindowVM();
         Assert.IsNotNull(vm.SelectedPathingAlgorithm);
      }

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
   }
}
