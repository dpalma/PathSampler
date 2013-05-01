using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PathSampler.Core;
using PathSampler.Models;
using System.ComponentModel;

namespace PathSampler.PathFinders
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

      public static List<Type> GatherPathingAlgorithms()
      {
         var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                 where t.IsClass && t.BaseType.Equals(typeof(PathFinder))
                 select t;

         var l = q.ToList();

         l.Sort((a, b) =>
            {
               return GetDisplayName(a).CompareTo(GetDisplayName(b));
            });

         return l;
      }

      public static string GetDisplayName(Type type)
      {
         if (!type.IsSubclassOf(typeof(PathFinder)))
         {
            throw new ArgumentException("Type does not inherit from PathFinder");
         }

         object[] attributes = type.GetCustomAttributes(typeof(DisplayNameAttribute), false);

         if (attributes != null && attributes.Length > 0)
         {
            DisplayNameAttribute displayName = attributes.Single() as DisplayNameAttribute;

            if (displayName != null)
            {
               return displayName.DisplayName;
            }
         }

         return type.Name;
      }

      abstract public void Step();

      protected HashSet<GridCoordinate> ClosedList { get { return m_closedList; } }
      private HashSet<GridCoordinate> m_closedList = new HashSet<GridCoordinate>();

      protected IDictionary<GridCoordinate, GridCoordinate> Predecessors { get { return m_predecessors; } }
      private Dictionary<GridCoordinate, GridCoordinate> m_predecessors = new Dictionary<GridCoordinate, GridCoordinate>();

      public bool BuildPath()
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
            if (path.Contains(predecessor))
            {
               throw new InvalidOperationException("The chain of predessors contains a cycle");
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
