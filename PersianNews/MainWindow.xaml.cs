using com.sun.org.apache.bcel.@internal.generic;
using com.sun.org.apache.xalan.@internal.xslt;
using ikvm.extensions;
using NHazm;
using PersianNews.Views.Animations;
using PersianNews.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PersianNews.Engine;

namespace PersianNews
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Frame MainFrame => _mainFrame;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            MainFrame.MarginFadeInAnimation(
                from: new Thickness(20, 10, 0, 0),
                to: new Thickness(0, 10, 0, 0),
                TimeSpan.FromMilliseconds(500));
        }

        private void BackToHomeButton_Click(object sender, RoutedEventArgs e)
        {
            App.MainFrame.Navigate(new MainPage());
        }
    }
}
