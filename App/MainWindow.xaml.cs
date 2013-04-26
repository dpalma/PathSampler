using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;

namespace PathSampler
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      private void OnExit(object sender, ExecutedRoutedEventArgs e)
      {
         Application.Current.Shutdown();
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(mainGrid);
         adornerLayer.Add(new PathSampler.Views.ResizingAdorner(mapView1));
      }

   }
}
