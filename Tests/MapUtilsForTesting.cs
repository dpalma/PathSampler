using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFind.Core;
using PathFind.Models;

namespace PathFindTests
{
   public static class MapUtilsForTesting
   {
      public static Map BuildMap(int size)
      {
         Map map = new Map()
         {
            RowCount = size,
            ColumnCount = size,
            Goal = new GridCoordinate() { Row = size - 1, Column = size - 1 },
         };
         return map;
      }

      public static void BlockRow(this Map map, int row)
      {
         for (int i = 0; i < map.ColumnCount; ++i)
         {
            map.BlockedCells[new GridCoordinate() { Row = row, Column = i }] = 1;
         }
      }

      public static void Randomize(this Map map)
      {
         Random rand = new Random();

         for (int i = 0; i < map.RowCount; ++i)
         {
            for (int j = 0; j < map.ColumnCount; ++j)
            {
               GridCoordinate cell = new GridCoordinate() { Row = i, Column = j };
               if (map.Goal.Equals(cell) || map.Start.Equals(cell))
                  continue;

               if ((rand.Next() & 1) != 0)
                  map.BlockedCells[cell] = 1;
            }
         }
      }
   }
}
