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
      private Queue<GridCoordinate> m_openList = new Queue<GridCoordinate>();

      private HashSet<GridCoordinate> m_closedList = new HashSet<GridCoordinate>();

      private Dictionary<GridCoordinate, GridCoordinate> m_predecessors = new Dictionary<GridCoordinate, GridCoordinate>();

      public BreadthFirstSearch(Map map, ICellColoring cellColoring)
         : base(map, cellColoring)
      {
         AddToOpenList(Map.Start);
      }

      private void AddToOpenList(GridCoordinate cell)
      {
         m_openList.Enqueue(cell);
         CellColoring.SetCellColor(cell, Brushes.Orange);
      }

      private void AddToClosedList(GridCoordinate cell)
      {
         m_closedList.Add(cell);
         CellColoring.SetCellColor(cell, Brushes.Gray);
      }

      override public void Step()
      {
         if (m_openList.Count == 0)
         {
            // done, no path found
            Result = PathFindResult.PathNotFound;
         }
         else
         {
            GridCoordinate cell = m_openList.Dequeue();

            AddToClosedList(cell);

            if (cell.Equals(Map.Goal))
            {
               // done, path found
               List<GridCoordinate> path = new List<GridCoordinate>();
               path.Add(cell);
               while (!cell.Equals(Map.Start))
               {
                  GridCoordinate predecessor = null;
                  if (!m_predecessors.TryGetValue(cell, out predecessor))
                  {
                     throw new InvalidOperationException("Predessor not found");
                  }
                  path.Add(predecessor);
                  cell = predecessor;
               }
               path.Reverse();
               Path = path.ToArray();
               Result = PathFindResult.PathFound;
            }
            else
            {
               GridCoordinate[] neighbors = Map.GetNeighbors(cell);

               foreach (var neighbor in neighbors)
               {
                  bool unseen = !m_openList.Contains(neighbor) && !m_closedList.Contains(neighbor);

                  if (unseen)
                  {
                     AddToOpenList(neighbor);
                     m_predecessors[neighbor] = cell;
                  }
               }
            }
         }
      }
   }
}
