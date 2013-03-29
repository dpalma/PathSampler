﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFind.Core;
using PathFind.ViewModels;

namespace PathFindTests
{
   public static class MapVMUtilsForTesting
   {
      public static bool HasCell(this MapVM mapVM, GridCoordinate cell)
      {
         return mapVM.Cells.Where(x => x.Cell.Equals(cell)).SingleOrDefault() != null;
      }
   }
}
