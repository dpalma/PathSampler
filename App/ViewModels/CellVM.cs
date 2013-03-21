using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using PathFind.Core;

namespace PathFind.ViewModels
{
   public class CellVM
   {
      public CellVM(MapVM mapVM, GridCoordinate cell)
      {
         m_mapVM = mapVM;
         m_cell = cell;
      }

      private MapVM m_mapVM;
      internal MapVM MapVM { get { return m_mapVM; } }

      private GridCoordinate m_cell;
      public GridCoordinate Cell { get { return m_cell; } }

      internal Point CellPoint
      {
         get
         {
            return new Point(
               Cell.Column * (MapVM.CellSize.Width + MapVM.GridLineSize) + MapVM.GridLineSize,
               Cell.Row * (MapVM.CellSize.Height + MapVM.GridLineSize) + MapVM.GridLineSize);
         }
      }

      public Rect CellRect
      {
         get
         {
            return new Rect(CellPoint, MapVM.CellSize);
         }
      }

      public Brush Brush;
   }
}
