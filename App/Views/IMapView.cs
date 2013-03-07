﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PathFind.Views
{
   public interface IMapView
   {
      Size CellSize { get; set; }

      int GridLineSize { get; set; }
   }
}