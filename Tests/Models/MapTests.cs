using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NUnit.Framework;
using PathSampler.Core;
using PathSampler.Models;

namespace PathSamplerTests.Models
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
      public void TestSettingNullGoalThrowsException()
      {
         Assert.Throws<ArgumentNullException>(() => map.Goal = null);
      }

      [Test]
      public void TestSettingNullStartThrowsException()
      {
         Assert.Throws<ArgumentNullException>(()=>map.Start = null);
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
      public void TestNeighborsAreSortedInOrderOfDistanceToGoal()
      {
         map.Goal = map.GetBottomLeft();
         GridCoordinate[] neighbors = map.GetNeighbors(map.GetCenter());
         var lastDist = Double.NegativeInfinity;
         foreach (var c in neighbors)
         {
            double dist = c.EuclideanDistance(map.Goal);
            Assert.IsTrue(dist >= lastDist);
            lastDist = dist;
         }
      }

      [Test]
      public void TestSettingNegativeRowCountThrowsException()
      {
         //ExpectedMessage = "RowCount must be greater than zero"
         Assert.Throws<ArgumentException>(() => map.RowCount = -1);
      }

      [Test]
      public void TestSettingNegativeColumnCountThrowsException()
      {
         // ExpectedMessage = "ColumnCount must be greater than zero"
         Assert.Throws<ArgumentException>(() => map.ColumnCount = -1);
      }

      [Test]
      public void TestSettingRowCountToZeroThrowsException()
      {
         // ExpectedMessage = "RowCount must be greater than zero"
         Assert.Throws<ArgumentException>(() => map.RowCount = 0);
      }

      [Test]
      public void TestSettingColumnCountToZeroThrowsException()
      {
         // ExpectedMessage = "ColumnCount must be greater than zero"
         Assert.Throws<ArgumentException>(() => map.ColumnCount = 0);
      }

      [Test]
      public void TestSettingAboveRowRangeGoalThrowsException()
      {
         // ExpectedMessage = "Cell is out of bounds"
         Assert.Throws<ArgumentException>(() => map.Goal = new GridCoordinate() { Column = 0, Row = map.RowCount + 1 });
      }

      [Test]
      public void TestSettingBelowRowRangeGoalThrowsException()
      {
         // ExpectedMessage = "Cell is out of bounds"
         Assert.Throws<ArgumentException>(() => map.Goal = new GridCoordinate() { Column = 0, Row = -1 });
      }

      [Test]
      public void TestSettingBelowColumnRangeGoalThrowsException()
      {
         // ExpectedMessage = "Cell is out of bounds"
         Assert.Throws<ArgumentException>(() => map.Goal = new GridCoordinate() { Column = -1, Row = 0 });
      }

      [Test]
      public void TestSettingAboveRowRangeStartThrowsException()
      {
         // ExpectedMessage = "Cell is out of bounds"
         Assert.Throws<ArgumentException>(() => map.Start = new GridCoordinate() { Column = 0, Row = map.RowCount + 1 });
      }

      [Test]
      public void TestSettingAboveColumnRangeStartThrowsException()
      {
         // ExpectedMessage = "Cell is out of bounds"
         Assert.Throws<ArgumentException>(() => map.Start = new GridCoordinate() { Column = map.ColumnCount + 1, Row = 0 });
      }

      [Test]
      public void TestSettingBelowRowRangeStartThrowsException()
      {
         // ExpectedMessage = "Cell is out of bounds"
         Assert.Throws<ArgumentException>(() => map.Start = new GridCoordinate() { Column = 0, Row = -1 });
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
            Assert.AreEqual(map.CellSizeScalar, loadedMap.CellSizeScalar);
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

         map.CellSizeScalar = Map.DefaultCellSizeScalar;
         map.RowCount = Map.DefaultRowColumnCount;
         map.ColumnCount = Map.DefaultRowColumnCount;
         map.Goal = map.GetBottomRight();
         map.Start = map.GetTopLeft();

         Assert.AreEqual(0, propertiesChanged.Count);
      }

      [Test]
      public void TestExceptionThrownWhenSettingCellSizeBelowMinimum()
      {
         Assert.Throws<ArgumentOutOfRangeException>(() => map.CellSizeScalar = Map.MinimumCellSizeScalar - 1);
      }

      [Test]
      public void TestExceptionThrownWhenSettingCellSizeAboveMaximum()
      {
         Assert.Throws<ArgumentOutOfRangeException>(() => map.CellSizeScalar = Map.MaximumCellSizeScalar + 1);
      }

      [Test]
      public void TestGoalAtRightEdgeMovesWhenColumnCountIncreases()
      {
         Assert.AreEqual(map.ColumnCount - 1, map.Goal.Column);
         map.ColumnCount = map.ColumnCount * 2;
         Assert.AreEqual(map.ColumnCount - 1, map.Goal.Column);
      }

      [Test]
      public void TestGoalAtBottomEdgeMovesWhenRowCountIncreases()
      {
         Assert.AreEqual(map.RowCount - 1, map.Goal.Row);
         map.RowCount = map.RowCount * 2;
         Assert.AreEqual(map.RowCount - 1, map.Goal.Row);
      }

      [Test]
      public void TestStartAtRightEdgeMovesWhenColumnCountIncreases()
      {
         map.Goal = map.GetTopLeft();
         map.Start = map.GetBottomRight();
         Assert.AreEqual(map.ColumnCount - 1, map.Start.Column);
         map.ColumnCount = map.ColumnCount * 2;
         Assert.AreEqual(map.ColumnCount - 1, map.Start.Column);
      }

      [Test]
      public void TestStartAtBottomEdgeMovesWhenRowCountIncreases()
      {
         map.Goal = map.GetTopLeft();
         map.Start = map.GetBottomRight();
         Assert.AreEqual(map.RowCount - 1, map.Start.Row);
         map.RowCount = map.RowCount * 2;
         Assert.AreEqual(map.RowCount - 1, map.Start.Row);
      }

      [Test]
      public void TestIsInBoundsReturnsTrueForCenter()
      {
         Assert.IsTrue(map.IsInBounds(map.GetCenter()));
      }

      [Test]
      public void TestIsInBoundsReturnsFalseForNegativeRows()
      {
         Assert.IsFalse(map.IsInBounds(new GridCoordinate() { Row = -10, Column = 1 }));
      }

      [Test]
      public void TestBlockedCellsAreRemovedWhenMapShrinksVertically()
      {
         map.Goal = map.GetTopRight();
         map.BlockRow(map.RowCount - 1);
         Assert.AreEqual(map.ColumnCount, map.BlockedCells.Count);
         map.RowCount -= 1;
         Assert.AreEqual(0, map.BlockedCells.Count);
      }

      [Test]
      public void TestBlockedCellsAreRemovedWhenMapShrinksHorizontally()
      {
         map.Goal = map.GetBottomLeft();
         map.BlockColumn(map.ColumnCount - 1);
         Assert.AreEqual(map.RowCount, map.BlockedCells.Count);
         map.ColumnCount -= 1;
         Assert.AreEqual(0, map.BlockedCells.Count);
      }
   }
}
