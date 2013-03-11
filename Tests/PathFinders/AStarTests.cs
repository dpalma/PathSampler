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
      public void SetCellColor(GridCoordinate cell, System.Windows.Media.Brush brush)
      {
      }
   }

   class AStarTests
   {
      private static Map BuildMap(int size)
      {
         Map map = new Map()
         {
            RowCount = size,
            ColumnCount = size,
            Goal = new GridCoordinate() { Row = size - 1, Column = size - 1 },
         };
         return map;
      }

      private static void BlockRow(Map map, int row)
      {
         for (int i = 0; i < map.ColumnCount; ++i)
         {
            map.BlockedCells[new GridCoordinate() { Row = row, Column = i }] = 1;
         }
      }

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
         Map map = BuildMap(3);
         AStar astar = new AStar(map, new NullCellColoring());
         Assert.AreEqual(PathFindResult.PathFound, FindPath(astar));
      }

      [Test]
      public void TestNoPathFoundWhenBlocked()
      {
         Map map = BuildMap(3);
         BlockRow(map, 1);
         AStar astar = new AStar(map, new NullCellColoring());
         Assert.AreEqual(PathFindResult.PathNotFound, FindPath(astar));
      }
   }
}
