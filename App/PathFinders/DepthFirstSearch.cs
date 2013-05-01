using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PathSampler.Models;
using PathSampler.Core;

namespace PathSampler.PathFinders
{
   [DisplayName("Depth-First Search")]
   public class DepthFirstSearch : PathFinder
   {
      public DepthFirstSearch(Map map, ICellColoring cellColoring)
         : base(map, cellColoring)
      {
         Push(Map.Start);
         MarkVisited(Map.Start);
      }

      private HashSet<GridCoordinate> Visited = new HashSet<GridCoordinate>();

      private void MarkVisited(GridCoordinate c)
      {
         Visited.Add(c);
      }

      private void MarkClosed(GridCoordinate c)
      {
         ClosedList.Add(c);
         CellColoring.SetCellColor(c, CellColor.Closed);
      }

      private bool IsVisited(GridCoordinate c)
      {
         return Visited.Contains(c);
      }

      private Stack<GridCoordinate> S = new Stack<GridCoordinate>();

      private void Push(GridCoordinate c)
      {
         S.Push(c);
         CellColoring.SetCellColor(c, CellColor.Open);
      }

      public override void Step()
      {
         if (S.Count > 0)
         {
            var w = S.Pop();
            MarkClosed(w);

            if (w.Equals(Map.Goal))
            {
               if (BuildPath())
               {
                  Result = PathFindResult.PathFound;
               }
               return;
            }

            var wNeighbors = Map.GetNeighbors(w);
            foreach (var u in wNeighbors)
            {
               if (IsVisited(u))
               {
                  continue;
               }
               MarkVisited(u);
               Push(u);
               Predecessors[u] = w;
            }
         }
         else
         {
            Result = PathFindResult.PathNotFound;
         }
      }
   }
}
