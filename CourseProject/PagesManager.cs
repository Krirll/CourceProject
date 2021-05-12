using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace CourseProject
{
    class PagesManager
    {
        public static int[] states = new int[4] { -1, -1, -1, -1};
        public static int currentState = -1;
        public static Frame frame;
        public static void CleanStates()
        {
            for (int i = 0; i < 4; i++) states[i] = -1;
            frame = null;
            currentState = -1;
            System.GC.Collect();
        }
        public static void CleanFrame()
        {
            if (currentState != -1)
            {
                while (--states[currentState] != -1)
                {
                    frame.Navigate(null);
                    frame.GoBack();
                    frame.RemoveBackEntry();
                }
                System.GC.Collect();
            }
        }
        public static void ShowFilterTrips()
        {
            currentState = 0;
            frame.Navigate(new PageFilterTrips());
            states[0] = 0;
        }
        public static void ShowTrips(List<string> list)
        {
            frame.Navigate(new PageTrips(list));
            states[0]++;
        }
        public static void ShowSelectedTrips()
        {
            currentState = 1;
            frame.Navigate(new PageSelectedTickets());
            states[1] = 0;
        }
        public static void ShowCabinet()
        {
            currentState = 2;
            frame.Navigate(new PagePerson());
            states[2] = 0;
        }
        public static void ShowHistory()
        {
            currentState = 3;
            frame.Navigate(new PageHistory());
            states[3] = 0;
        }
        public static void ShowConnectionError()
        {
            MessageBox.Show("Отсутствует соединение с сервером,\nповторите попытку позже.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            foreach (Window window in Application.Current.Windows)
            {
                if (window is TourismWindow)
                {
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    window.Close();
                }
            }
        }
    }
}
