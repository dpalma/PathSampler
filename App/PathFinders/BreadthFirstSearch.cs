using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using PathFind.Core;
using PathFind.Models;

namespace PathFind.PathFinders
{
   public class BreadthFirstSearch : PathFinder
   {
      public BreadthFirstSearch(Map map, ICellColoring cellColoring)
         : base(map, cellColoring)
      {
         AddToOpenList(Map.Start);
      }

      protected Queue<GridCoordinate> OpenList { get { return m_openList; } }
      private Queue<GridCoordinate> m_openList = new Queue<GridCoordinate>();

      private void AddToOpenList(GridCoordinate cell)
      {
         OpenList.Enqueue(cell);
         CellColoring.SetCellColor(cell, Brushes.Orange);
      }

      private void AddToClosedList(GridCoordinate cell)
      {
         ClosedList.Add(cell);
         CellColoring.SetCellColor(cell, Brushes.Gray);
      }

      protected bool IsUnseen(GridCoordinate cell)
      {
         return !OpenList.Contains(cell) && !ClosedList.Contains(cell);
      }

      override public void Step()
      {
         if (OpenList.Count == 0)
         {
            // done, no path found
            Result = PathFindResult.PathNotFound;
         }
         else
         {
            GridCoordinate cell = OpenList.Dequeue();
            AddToClosedList(cell);

            if (cell.Equals(Map.Goal))
            {
               // done, path found
               if (BuildPath())
               {
                  Result = PathFindResult.PathFound;
               }
            }
            else
            {
               GridCoordinate[] neighbors = Map.GetNeighbors(cell);

               foreach (var neighbor in neighbors)
               {
                  if (IsUnseen(neighbor))
                  {
                     AddToOpenList(neighbor);
                     Predecessors[neighbor] = cell;
                  }
               }
            }
         }
      }
   }
}
