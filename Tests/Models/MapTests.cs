﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.Core;
using PathFind.Models;

namespace PathFindTests.Models
{
   class MapTests
   {
      [Test]
      public void TestDefaultSize()
      {
         Map map = new Map();
         Assert.AreEqual(Map.DefaultRowColumnCount, map.RowCount);
         Assert.AreEqual(Map.DefaultRowColumnCount, map.ColumnCount);
      }

      [Test]
      public void TestDefaultGoal()
      {
         Map map = new Map();
         Assert.IsNotNull(map.Goal);
      }

      [Test]
      public void TestTopLeftCellHasOnlyThreeNeighbors()
      {
         Map map = new Map();
         GridCoordinate[] neighbors = map.GetNeighbors(new GridCoordinate());
         Assert.AreEqual(3, neighbors.Length);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingNegativeRowCountThrowsException()
      {
         Map map = new Map();
         map.RowCount = -1;
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingNegativeColumnCountThrowsException()
      {
         Map map = new Map();
         map.ColumnCount = -1;
      }

      [Test]
      public void TestSettingAboveRangeGoalThrowsException()
      {
         Map map = new Map();
         //map.Dimensions = new Size(16, 16);
      }
   }
}
