using PersianNews.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Interaction logic for DataViewer.xaml
    /// </summary>
    public partial class DataViewer : Page
    {
        public List<NewsItem> Items { get; }

        private int CurrentIndex { get; set; } = 0;

        public DataViewer(List<NewsItem> data)
        {
            Items = new List<NewsItem>(data);

            InitializeComponent();

            UpdateDataContext();
        }

        public void UpdateDataContext()
        {
            DataContext = Items[CurrentIndex];
            indicator.Content = $"{CurrentIndex + 1} از {Items.Count}";
        }

        public void GoNext() => CurrentIndex = (CurrentIndex + 1 >= Items.Count) ? 0 : CurrentIndex + 1;

        public void GoPrevious() => CurrentIndex = (CurrentIndex - 1 < 0) ? Items.Count - 1 : CurrentIndex - 1;

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            GoNext(); UpdateDataContext();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            GoPrevious();
            UpdateDataContext();
        }
    }
}
