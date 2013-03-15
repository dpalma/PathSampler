using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
         map = new Map();
         vm = new MapVM();
         vm.Map = map;
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

      //[Test]
      //public void TestPathingUsesSelectedAlgorithm()
      //{
      //   Type defaultPathingAlgorithm = vm.SelectedPathingAlgorithm;
      //   vm.SelectedPathingAlgorithm = vm.PathingAlgorithms.Skip(1).First();
      //   Assert.AreNotEqual(defaultPathingAlgorithm, vm.SelectedPathingAlgorithm);
      //   vm.StartPathing();
      //   Assert.AreEqual(vm.SelectedPathingAlgorithm, vm.PathFinder.GetType());
      //}

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
