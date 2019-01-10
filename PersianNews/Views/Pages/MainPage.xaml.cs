using PersianNews.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PersianNews.Views.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void HazmButton_Click(object sender, RoutedEventArgs e) => App.MainFrame.Navigate(new HazmPlayground());

        private void ExitButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void DataViewerButton_Click(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Data/out.jsonl"
                );
            var text = File.ReadAllText(path);
            var jsonItems = JsonLinesParser.ParseLines<JsonItem>(text);
            var newsItems = DataCleanser.CleanseJsonItems(jsonItems);

            App.MainFrame.Navigate(new DataViewer(newsItems.ToList()));
        }
    }
}
