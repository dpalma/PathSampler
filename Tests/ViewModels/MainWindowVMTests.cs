using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.ViewModels;

namespace PathFindTests.ViewModels
{
   class MainWindowVMTests
   {
      [Test]
      public void TestPathingAlgorithmImplementationsAreCollected()
      {
         MainWindowVM vm = new MainWindowVM();
         Assert.IsTrue(vm.PathingAlgorithms.Count > 0);
      }

      [Test]
      public void TestThereIsADefaultPathingAlgorithmSelected()
      {
         MainWindowVM vm = new MainWindowVM();
         Assert.IsNotNull(vm.SelectedPathingAlgorithm);
      }
   }
}
