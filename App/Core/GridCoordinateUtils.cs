using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFind.Core
{
   public static class GridCoordinateUtils
   {
      public static double EuclideanDistance(this GridCoordinate from, GridCoordinate to)
      {
         return Math.Sqrt(
            ((to.Column - from.Column) * (to.Column - from.Column)) +
            ((to.Row - from.Row) * (to.Row - from.Row))
            );
      }

      public static double ManhattanDistance(this GridCoordinate from, GridCoordinate to)
      {
         return Math.Abs(to.Column - from.Column) + Math.Abs(to.Row - from.Row);
      }
   }
}
