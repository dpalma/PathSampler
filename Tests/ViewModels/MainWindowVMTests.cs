﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathSampler.Core;
using PathSampler.Models;
using PathSampler.ViewModels;

namespace PathSamplerTests.ViewModels
{
   class MainWindowVMTests
   {
      private MainWindowVM vm;

      [SetUp]
      public void SetUp()
      {
         vm = new MainWindowVM();
         vm.Map = MapUtilsForTesting.BuildMap(4);
         vm.MapVM.PathingStepDelay = TimeSpan.FromMilliseconds(1);
      }

      [Test]
      public void TestStopPathingCanExecuteChangedFiresWhenGoalChanges()
      {
         bool canExecuteChanged = false;
         vm.StopPathingCommand.CanExecuteChanged += (object sender, EventArgs e) => { canExecuteChanged = true; };
         vm.Map.Goal = vm.Map.Start;
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

      private const int PathingTimeout = 10000;

      [Test, MaxTime(PathingTimeout)]
      public void TestPathingCommandsUpdateWhenPathFindingCompletes()
      {
         vm.StartPathingCommand.Execute(null);
         Assert.IsFalse(vm.StartPathingCommand.CanExecute(null));
         vm.MapVM.ActivePathingTask.Wait();
         Assert.IsTrue(vm.StartPathingCommand.CanExecute(null));
      }

      [Test, MaxTime(PathingTimeout)]
      public void TestStopPathingCanExecuteChangedFiresWhenPathFindingCompletes()
      {
         int canExecuteChangedCalls = 0;
         vm.StopPathingCommand.CanExecuteChanged += (object sender, EventArgs e) => { canExecuteChangedCalls++; };
         vm.StartPathingCommand.Execute(null);
         vm.MapVM.ActivePathingTask.Wait();
         Assert.AreEqual(2, canExecuteChangedCalls);
      }

      [Test]
      public void TestFileThatSavesSuccessfullyUpdatesCurrentFileName()
      {
         var fileName = System.IO.Path.GetTempFileName();
         try
         {
            Assert.AreNotEqual(fileName, vm.CurrentMapFileName);
            vm.SaveMap(fileName);
            Assert.AreEqual(fileName, vm.CurrentMapFileName);
         }
         finally
         {
            System.IO.File.Delete(fileName);
         }
      }

      [Test]
      public void TestNewMapClearsCurrentFileName()
      {
         var fileName = System.IO.Path.GetTempFileName();
         try
         {
            vm.SaveMap(fileName);
            Assert.AreEqual(fileName, vm.CurrentMapFileName);
            vm.NewMap();
            Assert.AreEqual(String.Empty, vm.CurrentMapFileName);
         }
         finally
         {
            System.IO.File.Delete(fileName);
         }
      }

      [Test]
      public void TestFileThatFailsToLoadDoesNotChangeCurrentFileName()
      {
         Assert.IsFalse(vm.OpenMap("FileDoesNotExist"));
         Assert.AreEqual(String.Empty, vm.CurrentMapFileName);
      }
   }
}
