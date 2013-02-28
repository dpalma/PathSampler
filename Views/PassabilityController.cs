using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFind.Models;
using PathFind.ViewModels;

namespace PathFind.Views
{
   class PassabilityController
   {
      private MapView m_mapView;
      public MapView MapView
      {
         get { return m_mapView; }
      }

      public PassabilityController(MapView mapView)
      {
         m_mapView = mapView;

         MapView.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(MapView_MouseLeftButtonDown);
         MapView.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MapView_MouseLeftButtonUp);
         MapView.MouseMove += new System.Windows.Input.MouseEventHandler(MapView_MouseMove);
         MapView.LostMouseCapture += new System.Windows.Input.MouseEventHandler(MapView_LostMouseCapture);
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
               MapView.SelectedCells.Clear();
               MapView.InvalidateVisual();
            }
         }
      }

      void MapView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         if (MapView.CaptureMouse())
         {
            HitTestAndAddSelected(e);
            MouseDragging = true;
         }
      }

      void MapView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         if (MouseDragging)
         {
            var vm = MapView.DataContext as MapVM;
            if (vm != null)
            {
               vm.SetPassability(MapView.SelectedCells, 1);
            }

            MapView.ReleaseMouseCapture();
            MouseDragging = false;
         }
      }

      private void HitTestAndAddSelected(System.Windows.Input.MouseEventArgs mouseEventArgs)
      {
         GridCoordinate hitCell = MapView.GetHitCell(mouseEventArgs);

         if (MapView.SelectedCells.Add(hitCell))
         {
            MapView.InvalidateVisual();
         }
      }

      void MapView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
      {
         if (MouseDragging)
         {
            HitTestAndAddSelected(e);
         }
      }

      void MapView_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
      {
         m_mouseDragging = false;
      }
   }
}
