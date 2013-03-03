using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFind.Models;
using System.Windows.Media;

namespace PathFind.PathFinders
{
   public interface ICellColoring
   {
      void SetCellColor(GridCoordinate cell, Brush brush);
   }
}
