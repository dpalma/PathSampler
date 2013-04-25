using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PathFind.Core
{
   public delegate void PlotCoordinate(GridCoordinate coordinate);

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

      [DllImport("gdi32.dll")]
      static extern bool LineDDA(int xStart, int yStart, int xEnd, int yEnd,
         LineDDADelegate pLineFunc, IntPtr pData);

      delegate void LineDDADelegate(int x, int y, IntPtr pData);

      public static void LineTo(this GridCoordinate from, GridCoordinate to, PlotCoordinate plotCoordinate)
      {
         LineDDA(from.Column, from.Row, to.Column, to.Row, new LineDDADelegate(
            (x, y, data) =>
            {
               plotCoordinate(new GridCoordinate() { Column = x, Row = y });
            }),
            IntPtr.Zero);
      }
   }
}
