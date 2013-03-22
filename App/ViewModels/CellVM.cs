using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using PathFind.Core;

namespace PathFind.ViewModels
{
   public class CellVM : ViewModel
   {
      public CellVM(MapVM mapVM, GridCoordinate cell)
      {
         m_mapVM = mapVM;
         m_cell = cell;
         MapVM.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(MapVM_PropertyChanged);
      }

      void MapVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
         if (e.PropertyName.Equals("GridLineSize")
            || e.PropertyName.Equals("CellSize")
            || e.PropertyName.Equals("CellSizeScalar"))
         {
            FirePropertyChanged("CellPoint");
         }
      }

      private MapVM m_mapVM;
      public MapVM MapVM { get { return m_mapVM; } }

      private GridCoordinate m_cell;
      public GridCoordinate Cell { get { return m_cell; } }

      public bool IsBlocked
      {
         get
         {
            return MapVM.Map.BlockedCells.ContainsKey(Cell);
         }
      }

      public Point CellPoint
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
