using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NUnit.Framework;
using PathFind.Core;
using PathFind.Models;

namespace PathFindTests.Models
{
   class MapTests
   {
      private Map map;

      [SetUp]
      public void SetUp()
      {
         map = new Map();
      }

      [Test]
      public void TestDefaultSize()
      {
         Assert.AreEqual(Map.DefaultRowColumnCount, map.RowCount);
         Assert.AreEqual(Map.DefaultRowColumnCount, map.ColumnCount);
      }

      [Test]
      public void TestDefaultGoal()
      {
         Assert.IsNotNull(map.Goal);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void TestSettingNullGoalThrowsException()
      {
         map.Goal = null;
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void TestSettingNullStartThrowsException()
      {
         map.Start = null;
      }

      [Test]
      public void TestTopLeftCellHasOnlyThreeNeighbors()
      {
         GridCoordinate[] neighbors = map.GetNeighbors(new GridCoordinate());
         Assert.AreEqual(3, neighbors.Length);
      }

      [Test]
      public void TestGetNeighborsReturnsAllEightResults()
      {
         GridCoordinate[] neighbors = map.GetNeighbors(map.GetCenter());
         Assert.AreEqual(8, neighbors.Length);
      }

      [Test]
      public void TestGetNeighborsWithNoDiagonalsReturnsOnlyFourResults()
      {
         GridCoordinate[] neighbors = map.GetNeighbors(map.GetCenter(), false);
         Assert.AreEqual(4, neighbors.Length);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingNegativeRowCountThrowsException()
      {
         map.RowCount = -1;
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingNegativeColumnCountThrowsException()
      {
         map.ColumnCount = -1;
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingAboveRowRangeGoalThrowsException()
      {
         map.Goal = new GridCoordinate() { Column = 0, Row = map.RowCount + 1 };
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingBelowRowRangeGoalThrowsException()
      {
         map.Goal = new GridCoordinate() { Column = 0, Row = -1 };
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingAboveRowRangeStartThrowsException()
      {
         map.Start = new GridCoordinate() { Column = 0, Row = map.RowCount + 1 };
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingBelowRowRangeStartThrowsException()
      {
         map.Start = new GridCoordinate() { Column = 0, Row = -1 };
      }

      [Test]
      public void TestSerialization()
      {
         map.RowCount = 24;
         map.ColumnCount = 24;
         map.Start = new GridCoordinate() { Row = 5, Column = 18 };
         map.Goal = new GridCoordinate() { Row = 17, Column = 2 };
         using (var stream = new MemoryStream())
         {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, map);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            Map loadedMap = (Map)formatter.Deserialize(stream);
            Assert.AreEqual(map.RowCount, loadedMap.RowCount);
            Assert.AreEqual(map.ColumnCount, loadedMap.ColumnCount);
            Assert.AreEqual(map.Start, loadedMap.Start);
            Assert.AreEqual(map.Goal, loadedMap.Goal);
         }
      }

      [Test]
      public void TestAssign()
      {
         map.CellSizeScalar = 38;
         map.Randomize();
         Assert.IsTrue(map.BlockedCells.Count > 0);
         Map newMap = new Map();
         newMap.Assign(map);
         Assert.AreEqual(map.BlockedCells.Count, newMap.BlockedCells.Count);
         Assert.AreEqual(map.BlockedCells, newMap.BlockedCells);
         Assert.AreEqual(map.CellSizeScalar, newMap.CellSizeScalar);
      }

      [Test]
      public void TestPropertyChangingHappensBeforePropertyChanged()
      {
         var propertiesChanging = new List<string>();
         var propertiesChanged = new List<string>();

         map.PropertyChanging += (s, e) =>
         {
            propertiesChanging.Add(e.PropertyName);
            Assert.AreEqual(0, propertiesChanged.Count, "PropertyChanged shouldn't have happened yet");
         };

         map.PropertyChanged += (s, e) =>
            {
               propertiesChanged.Add(e.PropertyName);
               Assert.AreEqual(e.PropertyName, propertiesChanging.FirstOrDefault());
               Assert.AreEqual(1, propertiesChanging.Count, "PropertyChanging should have happend already");
            };

         map.Goal = map.GetTopRight();
      }

      [Test]
      public void TestPropertyNotificationsAreSkippedWhenNoNetChange()
      {
         var propertiesChanged = new List<string>();
         map.PropertyChanged += (s, e) =>
         {
            propertiesChanged.Add(e.PropertyName);
         };

         map.RowCount = Map.DefaultRowColumnCount;
         map.ColumnCount = Map.DefaultRowColumnCount;
         map.Goal = map.GetBottomRight();
         map.Start = map.GetTopLeft();

         Assert.AreEqual(0, propertiesChanged.Count);
      }
   }
}
