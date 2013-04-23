﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.Models;
using PathFind.ViewModels;
using System.ComponentModel;

namespace PathFindTests.ViewModels
{
   class CellVMTests
   {
      private MapVM mapVM;

      [SetUp]
      public void SetUp()
      {
         mapVM = new MapVM(MapUtilsForTesting.BuildMap(3));
      }

      [Test]
      public void TestIsBlockedPropertyIsTrueWhenCellIsBlocked()
      {
         CellVM cellVM = new CellVM(mapVM, mapVM.Map.GetCenter());
         Assert.IsFalse(cellVM.IsBlocked);
         mapVM.Map.BlockedCells[mapVM.Map.GetCenter()] = 1;
         Assert.IsTrue(cellVM.IsBlocked);
      }

      [Test]
      public void TestIsInOpenListIsTrueWhenCellColorIsOpen()
      {
         mapVM.SetCellColor(mapVM.Map.GetCenter(), PathFind.PathFinders.CellColor.Open);
         CellVM cellVM = mapVM.GetCell(mapVM.Map.GetCenter());
         Assert.IsNotNull(cellVM);
         Assert.IsTrue(cellVM.IsInOpenList);
      }

      [Test]
      public void TestIsInClosedListIsTrueWhenCellColorIsClosed()
      {
         mapVM.SetCellColor(mapVM.Map.GetCenter(), PathFind.PathFinders.CellColor.Closed);
         CellVM cellVM = mapVM.GetCell(mapVM.Map.GetCenter());
         Assert.IsNotNull(cellVM);
         Assert.IsTrue(cellVM.IsInClosedList);
      }

      [Test]
      public void TestPropertyChangeNotificationHappensWhenColorChanges()
      {
         mapVM.SetCellColor(mapVM.Map.GetCenter(), PathFind.PathFinders.CellColor.Open);
         CellVM cellVM = mapVM.GetCell(mapVM.Map.GetCenter());
         var propertiesChanged = new List<string>();
         cellVM.PropertyChanged += (object sender, PropertyChangedEventArgs eventArgs) =>
         {
            propertiesChanged.Add(eventArgs.PropertyName);
         };
         mapVM.SetCellColor(mapVM.Map.GetCenter(), PathFind.PathFinders.CellColor.Closed);
         Assert.IsTrue(propertiesChanged.Contains("IsInOpenList"));
         Assert.IsTrue(propertiesChanged.Contains("IsInClosedList"));
      }
   }
}
