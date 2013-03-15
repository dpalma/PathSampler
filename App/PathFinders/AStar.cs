using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using PathFind.Collections;
using PathFind.Core;
using PathFind.Models;

namespace PathFind.PathFinders
{
   internal sealed class AStarNode : IComparable<AStarNode>
   {
      public AStarNode(GridCoordinate cell)
      {
         m_cell = cell;
      }

      public GridCoordinate Cell { get { return m_cell; } }
      private GridCoordinate m_cell;

      public double CostFromStart
      {
         get { return m_costFromStart; }
         internal set { m_costFromStart = value; }
      }
      private double m_costFromStart = 0;

      public double CostToGoal
      {
         get { return m_costToGoal; }
         internal set { m_costToGoal = value; }
      }
      private double m_costToGoal = Double.PositiveInfinity;

      #region IComparable<AStarNode> Members

      public int CompareTo(AStarNode other)
      {
         // TODO: compare by priority
         return Cell.Equals(other) ? 0 : -1;
      }

      #endregion
   }

   [DisplayName("A*")]
   public class AStar : PathFinder
   {
      public AStar(Map map, ICellColoring cellColoring)
         : base(map, cellColoring)
      {
         OpenList.Enqueue(new AStarNode(Map.Start) { CostToGoal = Map.Start.ManhattanDistance(Map.Goal) });
      }

      private PriorityQueue<AStarNode> OpenList { get { return m_openList; } }
      private PriorityQueue<AStarNode> m_openList = new PriorityQueue<AStarNode>();

      private void AddToOpenList(GridCoordinate cell)
      {
         OpenList.Enqueue(new AStarNode(cell));
         CellColoring.SetCellColor(cell, Brushes.Orange);
      }

      private void AddToClosedList(GridCoordinate cell)
      {
         ClosedList.Add(cell);
         CellColoring.SetCellColor(cell, Brushes.Gray);
      }

      private bool IsOnOpenList(GridCoordinate cell)
      {
         return OpenList.Where(n => n.Cell.Equals(cell)).Count() > 0;
      }

      protected bool IsUnseen(GridCoordinate cell)
      {
         return !IsOnOpenList(cell) && !ClosedList.Contains(cell);
      }

      public override void Step()
      {
         if (OpenList.Count == 0)
         {
            // done, no path found
            Result = PathFindResult.PathNotFound;
         }
         else
         {
            AStarNode node = OpenList.Dequeue();
            AddToClosedList(node.Cell);

            if (node.Cell.Equals(Map.Goal))
            {
               // done, path found
               if (BuildPath())
               {
                  Result = PathFindResult.PathFound;
               }
            }
            else
            {
               GridCoordinate[] neighbors = Map.GetNeighbors(node.Cell);

               foreach (var neighbor in neighbors)
               {
                  //double newCost = node.CostFromStart + node.Cell.ManhattanDistance(neighbor);

                  if (IsUnseen(neighbor))
                  {
                     AddToOpenList(neighbor);
                     Predecessors[neighbor] = node.Cell;
                  }
               }
            }
         }
      }
   }
}
