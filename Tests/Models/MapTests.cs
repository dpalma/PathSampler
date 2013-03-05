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
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingAboveRowRangeGoalThrowsException()
      {
         Map map = new Map();
         map.Goal = new GridCoordinate() { Column = 0, Row = map.RowCount + 1 };
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingBelowRowRangeGoalThrowsException()
      {
         Map map = new Map();
         map.Goal = new GridCoordinate() { Column = 0, Row = -1 };
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingAboveRowRangeStartThrowsException()
      {
         Map map = new Map();
         map.Start = new GridCoordinate() { Column = 0, Row = map.RowCount + 1 };
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void TestSettingBelowRowRangeStartThrowsException()
      {
         Map map = new Map();
         map.Start = new GridCoordinate() { Column = 0, Row = -1 };
      }

      [Test]
      public void TestSerialization()
      {
         Map map = new Map()
         {
            RowCount = 24,
            ColumnCount = 24,
            Start = new GridCoordinate() { Row = 5, Column = 18 },
            Goal = new GridCoordinate() { Row = 17, Column = 2 },
         };
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
   }
}
