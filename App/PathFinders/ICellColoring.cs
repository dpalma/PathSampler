using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using PathSampler.Core;

namespace PathSampler.PathFinders
{
   public enum CellColor
   {
      Open,
      Closed,
   }

   public interface ICellColoring
   {
      void SetCellColor(GridCoordinate cell, CellColor color);
   }
}
