using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using PathFind.Core;

namespace PathFind.ViewModels
{
   public class CellVM
   {
      public CellVM(GridCoordinate cell)
      {
         m_cell = cell;
      }

      private GridCoordinate m_cell;

      public GridCoordinate Cell { get { return m_cell; } }

      public Brush Brush;
   }
}
