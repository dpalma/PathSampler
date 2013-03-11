﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFind.Collections
{
   public static class HeapUtils
   {
      private static int HeapParent(int index)
      {
         return (int)Math.Floor((double)(index - 1) / 2);
      }

      private static int HeapLeft(int index)
      {
         return (2 * index) + 1;
      }

      private static int HeapRight(int index)
      {
         return (2 * index) + 2;
      }

      public static bool IsHeap<T>(this IList<T> list, int index = 0) where T : IComparable<T>
      {
         int left = HeapLeft(index);
         int right = HeapRight(index);

         if (left < list.Count)
         {
            if (list[left].CompareTo(list[index]) > 0)
            {
               return false;
            }
         }

         if (right < list.Count)
         {
            if (list[right].CompareTo(list[index]) > 0)
            {
               return false;
            }
         }

         return ((left >= list.Count) || list.IsHeap(left))
            && ((right >= list.Count) || list.IsHeap(right));
      }

      public static void Heapify<T>(this IList<T> list, int index) where T : IComparable<T>
      {
         int left = HeapLeft(index);
         int right = HeapRight(index);

         int largest = index;

         if (left < list.Count)
         {
            if (list[left].CompareTo(list[index]) > 0)
            {
               largest = left;
            }
         }

         if (right < list.Count)
         {
            if (list[right].CompareTo(list[largest]) > 0)
            {
               largest = right;
            }
         }

         if (largest != index)
         {
            T temp = list[largest];
            list[largest] = list[index];
            list[index] = temp;
            list.Heapify(largest);
         }
      }
   }
}
