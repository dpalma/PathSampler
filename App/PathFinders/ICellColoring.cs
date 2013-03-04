﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using PathFind.Core;

namespace PathFind.PathFinders
{
   public interface ICellColoring
   {
      void SetCellColor(GridCoordinate cell, Brush brush);
   }
}
