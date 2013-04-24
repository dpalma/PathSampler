using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using PathFind.Core;
using PathFind.PathFinders;

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
            || e.PropertyName.Equals("CellSize"))
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

      public bool IsGoal
      {
         get
         {
            return Cell.Equals(MapVM.Map.Goal);
         }
      }

      public bool IsStart
      {
         get
         {
            return Cell.Equals(MapVM.Map.Start);
         }
      }

      public bool IsOnPath
      {
         get
         {
            return (MapVM.CurrentPath != null) && MapVM.CurrentPath.Contains(Cell);
         }
      }

      private CellColor? m_cellColor = null;

      public CellColor? CellColor
      {
         get
         {
            return m_cellColor;
         }
         set
         {
            m_cellColor = value;
            OnColorChanged();
            FirePropertyChanged("CellColor");
         }
      }

      public bool IsInOpenList
      {
         get
         {
            CellColor? color = CellColor;
            return color.HasValue && color.Value == PathFinders.CellColor.Open;
         }
      }

      public bool IsInClosedList
      {
         get
         {
            CellColor? color = CellColor;
            return color.HasValue && color.Value == PathFinders.CellColor.Closed;
         }
      }

      private void OnColorChanged()
      {
         FirePropertyChanged("IsInOpenList");
         FirePropertyChanged("IsInClosedList");
      }

      public Point CellPoint
      {
         get
         {
            return new Point(
               (MapVM.CellSize.Width * Cell.Column) + (MapVM.GridLineSize * (Cell.Column + 1)),
               (MapVM.CellSize.Height * Cell.Row) + (MapVM.GridLineSize * (Cell.Row + 1)));
         }
      }

      public Rect CellRect
      {
         get
         {
            return new Rect(CellPoint, MapVM.CellSize);
         }
      }
   }
}
