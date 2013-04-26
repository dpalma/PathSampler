using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using PathSampler.Collections;
using PathSampler.Core;
using PathSampler.Models;

namespace PathSampler.PathFinders
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

      public double Cost
      {
         get { return CostFromStart + CostToGoal; }
      }

      #region IComparable<AStarNode> Members

      public int CompareTo(AStarNode other)
      {
         return (int)(other.Cost - this.Cost);
      }

      #endregion
   }

   [DisplayName("A*")]
   public class AStar : PathFinder
   {
      public AStar(Map map, ICellColoring cellColoring)
         : base(map, cellColoring)
      {
         OpenList.Enqueue(new AStarNode(Map.Start) { CostToGoal = PathCostEstimate(Map.Start, Map.Goal) });
      }

      private PriorityQueue<AStarNode> OpenList { get { return m_openList; } }
      private PriorityQueue<AStarNode> m_openList = new PriorityQueue<AStarNode>();

      private void AddToOpenList(GridCoordinate cell)
      {
         AddToOpenList(new AStarNode(cell));
      }

      private void AddToOpenList(AStarNode node)
      {
         OpenList.Enqueue(node);
         CellColoring.SetCellColor(node.Cell, CellColor.Open);
      }

      private void AddToClosedList(GridCoordinate cell)
      {
         ClosedList.Add(cell);
         CellColoring.SetCellColor(cell, CellColor.Closed);
      }

      private bool IsOnOpenList(GridCoordinate cell)
      {
         return OpenList.Where(n => n.Cell.Equals(cell)).Count() > 0;
      }

      protected bool IsUnseen(GridCoordinate cell)
      {
         return !IsOnOpenList(cell) && !ClosedList.Contains(cell);
      }

      private double PathCostActual(GridCoordinate from, GridCoordinate to)
      {
         if (Math.Abs(from.Row - to.Row) > 1 || Math.Abs(from.Column - to.Column) > 1)
            throw new InvalidOperationException("Cells are more than one space away");

         if (from.Row != to.Row && from.Column != to.Column)
            return 1.4;
         else
            return 1.0;
      }

      private double PathCostEstimate(GridCoordinate from, GridCoordinate to)
      {
         return from.ManhattanDistance(to);
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
                  double newCost = node.CostFromStart + PathCostActual(node.Cell, neighbor);

                  AStarNode neighborNode = OpenList.Where(n => n.Cell.Equals(neighbor)).FirstOrDefault();

                  if (ClosedList.Contains(neighbor)
                     || (neighborNode != null && neighborNode.CostFromStart <= newCost))
                  {
                     continue;
                  }
                  else
                  {
                     if (neighborNode == null)
                     {
                        neighborNode = new AStarNode(neighbor);
                     }
                     else
                     {
                        OpenList.Remove(neighborNode);
                     }

                     neighborNode.CostFromStart = newCost;
                     neighborNode.CostToGoal = PathCostEstimate(neighbor, Map.Goal);
                     AddToOpenList(neighborNode);
                     Predecessors[neighbor] = node.Cell;
                  }
               }

               AddToClosedList(node.Cell);
            }
         }
      }
   }
}
