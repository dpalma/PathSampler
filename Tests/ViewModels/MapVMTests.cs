using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using NUnit.Framework;
using PathFind.Core;
using PathFind.Models;
using PathFind.ViewModels;

namespace PathFindTests.ViewModels
{
   class MapVMTests
   {
      private Map map;
      private MapVM vm;

      [SetUp]
      public void SetUp()
      {
         map = MapUtilsForTesting.BuildMap(5);
         vm = new MapVM(map);
      }

      [Test]
      public void TestMapChangeTriggersRedrawRequest()
      {
         var redrawRequests = new List<EventArgs>();
         vm.RedrawRequested += (sender, eventArgs) =>
            {
               redrawRequests.Add(eventArgs);
            };
         GridCoordinate cell = new GridCoordinate() { Row = 1, Column = 1 };
         map.BlockedCells[cell] = 1;
         Assert.AreEqual(1, redrawRequests.Count);
      }

      [Test]
      public void TestSelectSameCellTwiceOnlyAddsOnce()
      {
         GridCoordinate cell = new GridCoordinate() { Row = map.RowCount / 2, Column = map.ColumnCount / 2 };
         vm.SelectedCells.Add(cell);
         vm.SelectedCells.Add(cell);
         Assert.AreEqual(1, vm.SelectedCells.Count);
      }

      //[Test]
      //public void TestStartPathingTwiceDoesntThrowException()
      //{
      //   vm.StartPathingCommand.Execute(null);
      //   vm.StartPathingCommand.Execute(null);

      [Test]
      public void TestPathingAlgorithmImplementationsAreCollected()
      {
         Assert.IsTrue(vm.PathingAlgorithms.Count > 0);
      }

      [Test]
      public void TestThereIsADefaultPathingAlgorithmSelected()
      {
         Assert.IsNotNull(vm.SelectedPathingAlgorithm);
      }

      [Test]
      public void TestDefaultPathingAlgorithmIsBFS()
      {
         Assert.AreEqual("BreadthFirstSearch", vm.SelectedPathingAlgorithm.Name);
      }

      [Test]
      public void TestPathingUsesSelectedAlgorithm()
      {
         var otherPathingAlgorithm = (from t in vm.PathingAlgorithms
                                      where !t.Equals(vm.SelectedPathingAlgorithm)
                                      select t).Single();
         Assert.AreNotEqual(otherPathingAlgorithm, vm.SelectedPathingAlgorithm);
         vm.SelectedPathingAlgorithm = otherPathingAlgorithm;
         vm.StartPathing();
         Assert.AreEqual(otherPathingAlgorithm, vm.CurrentPathFinder.GetType());
      }

      [Test, MaxTime(10000)]
      public void TestPathingTaskCompletes()
      {
         vm.PathingStepDelay = TimeSpan.FromMilliseconds(1);
         vm.StartPathing();
         Assert.IsNotNull(vm.ActivePathingTask);
         vm.ActivePathingTask.Wait();
      }

      [Test, MaxTime(30000)]
      public void TestIsPathingIsFalseWhenPathingTaskCompletes()
      {
         vm.PathingStepDelay = TimeSpan.FromMilliseconds(1);
         vm.StartPathing();
         while (!vm.ActivePathingTask.IsCompleted)
         {
            Assert.IsTrue(vm.IsPathing);
         }
         Assert.IsFalse(vm.IsPathing);
      }

      [Test]
      public void TestChangingGoalStopsPathing()
      {
         vm.StartPathing();
         Assert.IsTrue(vm.IsPathing);
         map.Goal = map.GetCenter();
         Assert.IsFalse(vm.IsPathing);
      }

      [Test]
      public void TestSelectedCellsBinding()
      {
         Target target = new Target();
         BindingOperations.SetBinding(target, Target.SelectedCellsProperty,
            new Binding()
               {
                  Source = vm,
                  Path = new PropertyPath("SelectedCells", null),
               });
         Assert.AreEqual(0, target.SelectedCells.Count);
         vm.SelectedCells.Add(map.GetCenter());
         Assert.AreEqual(1, target.SelectedCells.Count);
      }
   }

   sealed class Target : DependencyObject
   {
      public static readonly DependencyProperty SelectedCellsProperty = DependencyProperty.Register("SelectedCells", typeof(ICollection<GridCoordinate>), typeof(Target));
      public ICollection<GridCoordinate> SelectedCells
      {
         get { return (ICollection<GridCoordinate>)GetValue(SelectedCellsProperty); }
         set { SetValue(SelectedCellsProperty, value); }
      }
   }
}
