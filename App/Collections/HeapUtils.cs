using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathSampler.Collections
{
   public static class HeapUtils
   {
      private static int HeapParent(int index)
      {
         return ((index - 1) / 2);
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

      public static void MakeHeap<T>(this IList<T> list) where T : IComparable<T>
      {
         for (int i = list.Count / 2; i >= 0; --i)
         {
            list.Heapify(i);
         }
      }

      public static void HeapAdd<T>(this IList<T> heap, T value) where T : IComparable<T>
      {
         heap.Add(value);
         int index = heap.Count - 1;
         while ((index > 0) && (heap[HeapParent(index)].CompareTo(value) < 0))
         {
            heap[index] = heap[HeapParent(index)];
            index = HeapParent(index);
         }
         heap[index] = value;
      }

      public static T HeapRemoveAt<T>(this IList<T> heap, int index) where T : IComparable<T>
      {
         T max = heap[index];
         heap[index] = heap[heap.Count - 1];
         heap.RemoveAt(heap.Count - 1);
         heap.Heapify(index);
         return max;
      }

      public static T HeapRemove<T>(this IList<T> heap) where T : IComparable<T>
      {
         return heap.HeapRemoveAt(0);
      }
   }
}
