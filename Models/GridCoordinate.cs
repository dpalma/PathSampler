using System;

namespace PathFind.Models
{
   [Serializable]
   public class GridCoordinate
   {
      public int Row;
      public int Column;

      public override bool Equals(object obj)
      {
         GridCoordinate other = obj as GridCoordinate;
         if (other == null)
            return false;

         return other.Row == this.Row && other.Column == this.Column;
      }

      public override int GetHashCode()
      {
         return (Row * 31) ^ Column;
      }

      public override string ToString()
      {
         return String.Format("GridCoordinate({0}, {1})", Row, Column);
      }
   }
}
