using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFind.Core;
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

      protected Queue<GridCoordinate> OpenList { get { return m_openList; } }
      private Queue<GridCoordinate> m_openList = new Queue<GridCoordinate>();

      protected HashSet<GridCoordinate> ClosedList { get { return m_closedList; } }
      private HashSet<GridCoordinate> m_closedList = new HashSet<GridCoordinate>();

      protected bool IsUnseen(GridCoordinate cell)
      {
         return !OpenList.Contains(cell) && !ClosedList.Contains(cell);
      }

      protected IDictionary<GridCoordinate, GridCoordinate> Predecessors { get { return m_predecessors; } }
      private Dictionary<GridCoordinate, GridCoordinate> m_predecessors = new Dictionary<GridCoordinate, GridCoordinate>();

      protected bool BuildPath()
      {
         List<GridCoordinate> path = new List<GridCoordinate>();
         GridCoordinate cell = Map.Goal;
         path.Add(cell);
         while (!cell.Equals(Map.Start))
         {
            GridCoordinate predecessor = null;
            if (!Predecessors.TryGetValue(cell, out predecessor))
            {
               return false;
            }
            path.Add(predecessor);
            cell = predecessor;
         }
         path.Reverse();
         Path = path.ToArray();
         return true;
      }
   }
}
