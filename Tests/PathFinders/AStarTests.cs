using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.Core;
using PathFind.Models;
using PathFind.PathFinders;

namespace PathFindTests.PathFinders
{
   internal class NullCellColoring : ICellColoring
   {
      public void SetCellColor(GridCoordinate cell, CellColor color)
      {
      }
   }

   class AStarTests
   {
      private static PathFindResult FindPath(PathFinder pathFinder)
      {
         while (pathFinder.Result == null)
         {
            pathFinder.Step();
         }
         return pathFinder.Result.Value;
      }

      [Test]
      public void TestPathFoundOnTrivialMap()
      {
         Map map = MapUtilsForTesting.BuildMap(3);
         AStar astar = new AStar(map, new NullCellColoring());
         Assert.AreEqual(PathFindResult.PathFound, FindPath(astar));
      }

      [Test]
      public void TestNoPathFoundWhenBlocked()
      {
         Map map = MapUtilsForTesting.BuildMap(3);
         map.BlockRow(1);
         AStar astar = new AStar(map, new NullCellColoring());
         Assert.AreEqual(PathFindResult.PathNotFound, FindPath(astar));
      }
   }
}
