using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CourseProject
{
    public partial class PageTrips : Page
    {
        List<string> listParams;
        SqlManager sqlManager;
        public void UpdateListView()
        {
            //заполнение путевок по заданному фильтру
            LVTours.Items.Clear();
            LVTours = sqlManager.PushListView(LVTours, listParams, Item.cityNum, Sort.SelectedIndex, Search.Text);
            if (LVTours != null)
            {
                if (LVTours.Items.Count == 0)
                {
                    MessageBox.Show("Отсутствуют путевки по заданным фильтрам.");
                    Sort.SelectedIndex = -1;
                    Search.Text = "";
                }
            }
            else PagesManager.ShowConnectionError();
        }
        public PageTrips(List<string> list)
        {
            InitializeComponent();
            sqlManager = new SqlManager();
            int flag = 0;
            if (sqlManager.UpdateActualCountTripsInDB() != -2)
            {
                listParams = list;
                if (listParams.ElementAt(1) != "")
                {
                    Item.cityNum = sqlManager.ReturnNumber("SELECT Num_city FROM Cities WHERE Name_city = '" + listParams.ElementAt(1) + "'");
                    if (Item.cityNum == -2)
                    {
                        flag = -2;
                        PagesManager.ShowConnectionError();
                    }
                }
                if (flag != -2) UpdateListView();
            }
            else
            {
                PagesManager.ShowConnectionError();
            }
        }
        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateListView();
        private void Sort_DropDownOpened(object sender, EventArgs e) => Sort.SelectedIndex = -1;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Item item = button.DataContext as Item;
            if (Item.items == null) Item.InitList();
            if (Item.items.Find(x => x.NameTour == item.NameTour) == null)
            {
                if (Convert.ToDateTime(item.DateBegin).Date > DateTime.Today.Date)
                {
                    Item.items.Add(item);
                    MessageBox.Show("Путевка успешно добавлена в \"Отложенные\".");
                }
                else MessageBox.Show("Эта путевка уже неактуальна.");
            }
            else MessageBox.Show("Вы уже добавили эту путевку в \"Отложенные\".");
            System.GC.Collect();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateListView();
    }
}
