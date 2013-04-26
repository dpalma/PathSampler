using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathSampler.Core;
using PathSampler.ViewModels;

namespace PathSamplerTests
{
   public static class MapVMUtilsForTesting
   {
      public static CellVM GetCell(this MapVM mapVM, GridCoordinate cell)
      {
         return mapVM.Cells.Where(x => x.Cell.Equals(cell)).SingleOrDefault();
      }

      public static bool HasCell(this MapVM mapVM, GridCoordinate cell)
      {
         return mapVM.GetCell(cell) != null;
      }
   }
}
