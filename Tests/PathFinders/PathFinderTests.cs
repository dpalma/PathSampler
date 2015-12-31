using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathSampler.Core;
using PathSampler.Models;
using PathSampler.PathFinders;

namespace PathSamplerTests.PathFinders
{
   internal class PathFinderForTesting : PathFinder
   {
      internal PathFinderForTesting(Map map)
         : base(map, null)
      {
      }

      public override void Step()
      {
      }

      public void AddPredecessor(int fromColumn, int fromRow, int toColumn, int toRow)
      {
         var to = new GridCoordinate() { Column = toColumn, Row = toRow };
         var from = new GridCoordinate() { Column = fromColumn, Row = fromRow };
         Predecessors[to] = from;
      }
   }

   class PathFinderTests
   {
      [Test]
      public void TestBuildPathReturnsFalseWhenPredecessorChainIsBroken()
      {
         Map map = new Map() { RowCount = 4, ColumnCount = 4 };
         map.Goal = new GridCoordinate() { Row = 3, Column = 3 };
         var pathFinder = new PathFinderForTesting(map);
         pathFinder.AddPredecessor(2, 2, 3, 3);
         pathFinder.AddPredecessor(1, 1, 2, 2);
         Assert.IsFalse(pathFinder.BuildPath());
      }

      [Test]
      public void TestBuildPathThrowsExceptionWhenPredecessorChainContainsCycle()
      {
         Map map = new Map();
         var pathFinder = new PathFinderForTesting(map);
         pathFinder.AddPredecessor(8, 8, map.Goal.Column, map.Goal.Row);
         pathFinder.AddPredecessor(map.Goal.Column, map.Goal.Row, 8, 8);
         // ExpectedMessage = "The chain of predessors contains a cycle"
         Assert.Throws<InvalidOperationException>(()=>pathFinder.BuildPath());
      }
   }
}
