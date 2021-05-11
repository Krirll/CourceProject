using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CourseProject
{
    public partial class PageFilterTrips : Page
    {
        public PageFilterTrips()
        {
            InitializeComponent();
            CountryBox = DataManager.PushComboBox(CountryBox, "Name_country", "SELECT Name_country FROM Countries");
            if (CountryBox == null)
            {
                PagesManager.ShowConnectionError();
            }
        }
        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            DateBegin.Text = (DateBegin.Text == "") ? DateTime.Today.ToShortDateString() : DateBegin.Text;
            List<string> list = new List<string> { CountryBox.Text, CityBox.Text, SelectedPrice.Value.ToString(), DateBegin.Text, DateEnd.Text }; //передаваемые параметры
            PagesManager.ShowTrips(list);
            System.GC.Collect();
        }
        private void DefaultValues_Click(object sender, RoutedEventArgs e)
        {
            CountryBox.SelectedIndex = -1;
            CityBox.SelectedIndex = -1;
            SelectedPrice.Value = 1000;
            DateBegin.Text = "";
            DateEnd.Text = "";
        }
        private void CountryBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => CityBox.SelectedIndex = -1;
        private void CountryBox_DropDownOpened(object sender, EventArgs e) => CountryBox.SelectedIndex = -1;
        private void CityBox_DropDownOpened(object sender, EventArgs e)
        {
            CityBox.SelectedIndex = -1;
            if (CountryBox.Text != "")
            {
                SqlManager sqlManager = new SqlManager();
                int num = sqlManager.ReturnNumber("SELECT Num_country FROM Countries WHERE Name_country = '" + CountryBox.Text + "'");
                if (num != -2)
                {
                    CityBox.ItemsSource = null;
                    CityBox = DataManager.PushComboBox(CityBox, "Name_city", "SELECT Name_city FROM Cities WHERE Num_country = " + num);
                    if (CityBox == null)
                    {
                        PagesManager.ShowConnectionError();
                    }
                }
                else if (num == -2)
                {
                    PagesManager.ShowConnectionError();
                }
            }
            else
            {
                CityBox = DataManager.PushComboBox(CityBox, "Name_city", "SELECT Name_city FROM Cities");
                if (CityBox == null)
                {
                    PagesManager.ShowConnectionError();
                }
            }
            System.GC.Collect();
        }
        private void SelectedPrice_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ShowPrice == null) { } 
            else ShowPrice.Content = "от " + System.Math.Round(SelectedPrice.Value,0) + " рублей";
        }
        private void DateBegin_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateEnd.Text != "")
            {
                if (DateBegin.SelectedDate >= DateEnd.SelectedDate)
                {
                    MessageBox.Show("Дата начала не может быть\nбольше даты окончания путевки.");
                    DateBegin.Text = "";
                }
                else if (DateBegin.SelectedDate > DateTime.Today.AddYears(1))
                {
                    MessageBox.Show("Нет путевок на год вперед.");
                    DateBegin.Text = "";
                }
                else if (DateBegin.SelectedDate < DateTime.Today.AddDays(1))
                {
                    MessageBox.Show("Можно смотреть путевки от завтрашнего дня и позднее.");
                }
            }
        }
        private void DateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateBegin.Text != "")
            {
                if (DateEnd.SelectedDate <= DateBegin.SelectedDate)
                {
                    MessageBox.Show("Дата окончания не может быть\nменьше даты начала путевки.");
                    DateEnd.Text = "";
                }
                else if (DateEnd.SelectedDate > DateTime.Today.AddYears(1))
                {
                    MessageBox.Show("Нет путевок на год вперед.");
                    DateEnd.Text = "";
                }
            }
        }
    }
}
