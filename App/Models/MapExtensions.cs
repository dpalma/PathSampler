﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathSampler.Core;

namespace PathSampler.Models
{
   public static class MapExtensions
   {
      public static GridCoordinate GetCenter(this Map map)
      {
         return new GridCoordinate() { Row = map.RowCount / 2, Column = map.ColumnCount / 2 };
      }
   }
}
