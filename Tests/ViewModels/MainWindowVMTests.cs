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
         Map map = MapUtilsForTesting.BuildMap(4);
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
         vm.Map = MapUtilsForTesting.BuildMap(4);
         bool canExecuteChanged = false;
         vm.StopPathingCommand.CanExecuteChanged += (object sender, EventArgs e) => { canExecuteChanged = true; };
         vm.StartPathingCommand.Execute(null);
         Assert.IsTrue(canExecuteChanged);
      }

      [Test, MaxTime(30000)]
      public void TestPathingCommandsUpdateWhenPathFindingCompletes()
      {
         MainWindowVM vm = new MainWindowVM();
         vm.Map = MapUtilsForTesting.BuildMap(4);
         vm.StartPathingCommand.Execute(null);
         Assert.IsFalse(vm.StartPathingCommand.CanExecute(null));
         vm.MapVM.ActivePathingTask.Wait();
         Assert.IsTrue(vm.StartPathingCommand.CanExecute(null));
      }

      [Test, MaxTime(30000)]
      public void TestStopPathingCanExecuteChangedFiresWhenPathFindingCompletes()
      {
         MainWindowVM vm = new MainWindowVM();
         vm.Map = MapUtilsForTesting.BuildMap(4);
         int canExecuteChangedCalls = 0;
         vm.StopPathingCommand.CanExecuteChanged += (object sender, EventArgs e) => { canExecuteChangedCalls++; };
         vm.StartPathingCommand.Execute(null);
         vm.MapVM.ActivePathingTask.Wait();
         Assert.AreEqual(2, canExecuteChangedCalls);
      }
   }
}
