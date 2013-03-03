using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFind.Models;

namespace PathFind.PathFinders
{
   public enum PathFindResult
   {
      PathNotFound,
      PathFound,
   }

   abstract public class PathFinder
   {
      internal Map Map { get { return m_map; } }
      private Map m_map;

      internal ICellColoring CellColoring { get { return m_cellColoring; } }
      private ICellColoring m_cellColoring;

      Nullable<PathFindResult> m_result;
      public PathFindResult? Result
      {
         get { return m_result; }
         protected set { m_result = value; }
      }

      private GridCoordinate[] m_path;
      public GridCoordinate[] Path
      {
         get { return m_path; }
         protected set { m_path = value; }
      }

      public PathFinder(Map map, ICellColoring cellColoring)
      {
         m_map = map;
         m_cellColoring = cellColoring;
      }

      abstract public void Step();
   }
}
