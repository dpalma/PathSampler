using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PathFind.Core;
using PathFind.Models;
using PathFind.ViewModels;

namespace PathFind.Views
{
   class MouseController
   {
      private FrameworkElement m_view;
      public FrameworkElement View
      {
         get { return m_view; }
      }

      private MapVM m_mapViewModel;
      public MapVM MapViewModel
      {
         get { return m_mapViewModel; }
      }

      private ICommand m_command;
      public ICommand Command
      {
         get { return m_command; }
         set { m_command = value; }
      }

      public MouseController(FrameworkElement view, MapVM mapViewModel)
      {
         m_view = view;
         m_mapViewModel = mapViewModel;

         View.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(MapView_MouseLeftButtonDown);
         View.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MapView_MouseLeftButtonUp);
         View.MouseMove += new System.Windows.Input.MouseEventHandler(MapView_MouseMove);
         View.LostMouseCapture += new System.Windows.Input.MouseEventHandler(MapView_LostMouseCapture);
      }

      private bool m_mouseDragging = false;
      public bool MouseDragging
      {
         get { return m_mouseDragging; }
         private set
         {
            m_mouseDragging = value;
            if (!m_mouseDragging)
            {
               if (m_oldSelectedCellBrush != null)
               {
                  MapViewModel.SelectedCellBrush = m_oldSelectedCellBrush;
                  m_oldSelectedCellBrush = null;
               }
               MapViewModel.SelectedCells.Clear();
               LastHitCell = null;
            }
         }
      }

      delegate void CellSelector(GridCoordinate hitCell);

      private Brush m_oldSelectedCellBrush;

      void MapView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         if (View.CaptureMouse())
         {
            GridCoordinate hitCell = GetHitCell(e);
            if (hitCell != null)
            {
               m_oldSelectedCellBrush = MapViewModel.SelectedCellBrush;
               if (hitCell.Equals(MapViewModel.Map.Goal))
               {
                  Command = MapViewModel.SetGoalCommand;
                  //MapViewModel.SelectedCellBrush = MapViewModel.GoalCellBrush;
                  CellSelectionBehavior = CellSelectSingle;
               }
               else if (hitCell.Equals(MapViewModel.Map.Start))
               {
                  Command = MapViewModel.SetStartCommand;
                  //MapViewModel.SelectedCellBrush = MapViewModel.StartCellBrush;
                  CellSelectionBehavior = CellSelectSingle;
               }
               else if (MapViewModel.Map.BlockedCells.ContainsKey(hitCell))
               {
                  Command = MapViewModel.ClearPassabilityCommand;
                  CellSelectionBehavior = CellSelectMultiple;
               }
               else
               {
                  Command = MapViewModel.SetPassabilityCommand;
                  CellSelectionBehavior = CellSelectMultiple;
               }
            }

            HitTestAndSelect(e);
            MouseDragging = true;
         }
      }

      private CellSelector CellSelectionBehavior;

      void CellSelectSingle(GridCoordinate cell)
      {
         MapViewModel.SelectedCells.Clear();
         MapViewModel.SelectedCells.Add(cell);
      }

      void CellSelectMultiple(GridCoordinate cell)
      {
         MapViewModel.SelectedCells.Add(cell);
      }

      public GridCoordinate GetHitCell(System.Windows.Input.MouseEventArgs mouseEventArgs)
      {
         FrameworkElement view = mouseEventArgs.Source as FrameworkElement;
         if (view == null)
         {
            return null;
         }

         Point mouse = mouseEventArgs.GetPosition(view);

         if (mouse.X < 0 || mouse.X > view.Width || mouse.Y < 0 || mouse.Y > view.Height)
         {
            return null;
         }

         double hitX = mouse.X / (MapViewModel.CellSize.Width + MapViewModel.GridLineSize);
         double hitY = mouse.Y / (MapViewModel.CellSize.Height + MapViewModel.GridLineSize);

         return new GridCoordinate() { Column = (int)hitX, Row = (int)hitY };
      }

      void MapView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         if (MouseDragging)
         {
            if (Command != null)
               Command.Execute(null);

            View.ReleaseMouseCapture();
            MouseDragging = false;
         }
      }

      private GridCoordinate LastHitCell;

      private void HitTestAndSelect(System.Windows.Input.MouseEventArgs mouseEventArgs)
      {
         GridCoordinate hitCell = GetHitCell(mouseEventArgs);

         if (hitCell != null)
         {
            if (hitCell.Equals(LastHitCell))
            {
               return;
            }

            if (LastHitCell != null)
            {
               LastHitCell.LineTo(hitCell, new PlotCoordinate(CellSelectionBehavior));
            }
            else
            {
               CellSelectionBehavior(hitCell);
            }

            LastHitCell = hitCell;
         }
      }

      void MapView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
      {
         if (MouseDragging)
         {
            HitTestAndSelect(e);
         }
      }

      void MapView_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
      {
         MouseDragging = false;
      }
   }
}
