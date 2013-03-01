using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using PathFind.ViewModels;
using PathFind.Models;

namespace PathFind.Views
{
   abstract class MouseController
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
               MapViewModel.SelectedCells.Clear();
               View.InvalidateVisual();
            }
         }
      }

      void MapView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         if (View.CaptureMouse())
         {
            HitTestAndSelect(e);
            MouseDragging = true;
         }
      }

      void MapView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         if (MouseDragging)
         {
            Finish();

            View.ReleaseMouseCapture();
            MouseDragging = false;
         }
      }

      private void HitTestAndSelect(System.Windows.Input.MouseEventArgs mouseEventArgs)
      {
         GridCoordinate hitCell = MapViewModel.GetHitCell(mouseEventArgs);

         if (hitCell != null)
         {
            SelectCell(hitCell);
         }
      }

      protected abstract void Finish();

      protected abstract void SelectCell(GridCoordinate cell);

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
