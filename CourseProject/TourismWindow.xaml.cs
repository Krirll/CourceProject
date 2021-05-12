using System;
using System.Windows;
using System.Windows.Controls;

namespace CourseProject
{
    public partial class TourismWindow : Window
    {
        public TourismWindow()
        {
            InitializeComponent();
            PagesManager.frame = this.frame;
            if (DataPerson.right == 2) History.Visibility = Visibility.Hidden;
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            DataPerson.id = -1;
            MainWindow mw = new MainWindow();
            mw.Show();
            PagesManager.CleanStates();
            DataPerson.Clear();
            if (Item.items != null) Item.items.Clear();
            Close();
            System.GC.Collect();
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (--PagesManager.states[PagesManager.currentState] != -1)
            {
                PagesManager.frame.Navigate(null);
                PagesManager.frame.GoBack();
                PagesManager.frame.RemoveBackEntry();
            }
            if (PagesManager.states[PagesManager.currentState] == -1)
            {
                ChangeWindow(false);
                PagesManager.frame.Navigate(null);
                PagesManager.currentState = -1;
            }
            System.GC.Collect();
        }
        private void ChangeWindow(bool flag)
        {
            if (flag == true) //if click position
            {
                Trips.Margin = new Thickness(25, 60, 0, 0);
                Selected.Margin = new Thickness(25, 150, 0, 0);
                Cabinet.Margin = new Thickness(25, 240, 0, 0);
                History.Margin = new Thickness(25, 330, 0, 0);
                Window.Width = 1250;
                Window.Height = 550;
                Window.MinHeight = 550;
                Window.MinWidth = 1200;
                Back.Visibility = Visibility.Visible;
            }
            else //start position
            {
                Window.MinHeight = 500;
                Window.MinWidth = 485;
                Trips.Margin = new Thickness(150, 60, 0, 0);
                Selected.Margin = new Thickness(150, 150, 0, 0);
                Cabinet.Margin = new Thickness(150, 240, 0, 0);
                History.Margin = new Thickness(150, 330, 0, 0);
                Window.Width = 485;
                Window.Height = 500;
                Back.Visibility = Visibility.Hidden;
            }
        }
        private void Trips_Click(object sender, RoutedEventArgs e)
        {
            if (PagesManager.currentState != 0)
            {
                //clear back state
                PagesManager.CleanFrame();
                PagesManager.ShowFilterTrips();
                Back.Visibility = Visibility.Visible;
                ChangeWindow(true);
                System.GC.Collect();
            }
        }
        private void Selected_Click(object sender, RoutedEventArgs e)
        {
            if (PagesManager.currentState != 1)
            {
                if (Item.items != null)
                {
                    if (Item.items.Count != 0)
                    {
                        PagesManager.CleanFrame();
                        PagesManager.ShowSelectedTrips();
                        Back.Visibility = Visibility.Visible;
                        ChangeWindow(true);
                        System.GC.Collect();
                    }
                    else MessageBox.Show("Вы не выбрали ни одной путевки.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else MessageBox.Show("Вы не выбрали ни одной путевки.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Cabinet_Click(object sender, RoutedEventArgs e)
        {
            if (PagesManager.currentState != 2)
            {
                PagesManager.CleanFrame();
                PagesManager.ShowCabinet();
                Back.Visibility = Visibility.Visible;
                ChangeWindow(true);
                System.GC.Collect();
            }
        }
        private void History_Click(object sender, RoutedEventArgs e)
        {
            //вывод в дата грид все путевки полученные клиентом
            if (PagesManager.currentState != 3)
            {
                PagesManager.CleanFrame();
                PagesManager.ShowHistory();
                Back.Visibility = Visibility.Visible;
                ChangeWindow(true);
                System.GC.Collect();
            }
        }
    }
}
