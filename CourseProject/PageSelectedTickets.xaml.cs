using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;
using System.Data;
using System.IO;

namespace CourseProject
{
    public partial class PageSelectedTickets : Page
    {
        public PageSelectedTickets()
        {
            InitializeComponent();
            PushDataGrid();
        }
        public void PushDataGrid()
        {
            //заполнение таблицы истории путевок текущего пользователя
            foreach (Item item in Item.items)
            {
                SelectedTrips.Items.Add(new Item
                {
                    NameTour = item.NameTour,
                    Price = item.Price,
                    ActualCount = item.ActualCount
                });
                System.GC.Collect();
            }
        }
        private void MakeTicket(string[] param)
        {
            //создание билета
            SqlManager sqlManager;
            MessageBoxResult res = MessageBox.Show("Вы хотите купить именно этот билет?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                Word.Document doc = null;
                try
                {
                    Item item;
                    sqlManager = new SqlManager();
                    if (sqlManager.UpdateActualCountTripsInDB() != -2)
                    {
                        DataTable dt = sqlManager.ReturnTable("SELECT ActualCount " +
                                                   "FROM Tours " +
                                                   "WHERE Name_tour = '" + (SelectedTrips.SelectedItem as Item).NameTour + "'");
                        if (dt != null)
                        {
                            if (Convert.ToInt32(dt.Rows[0].ItemArray[0]) != 0)
                            {
                                //создаем новый документ
                                Word.Application app = new Word.Application();
                                var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                                doc = app.Documents.Open(Path.GetDirectoryName(location) + @"\Шаблон.docx");
                                doc.Activate();
                                //открываем шаблон и получаем список закладок
                                Word.Bookmarks wBookmarks = doc.Bookmarks;
                                Word.Range wRange; //положение закладки в документе
                                List<Word.Bookmark> bookmarks = new List<Word.Bookmark>();
                                foreach (Word.Bookmark mark in wBookmarks) bookmarks.Add(mark);
                                string[] mass = (DataPerson.right == 3) ? sqlManager.ReturnDataPerson() : param;
                                if (mass != null)
                                {
                                    string[] data = new string[13];
                                    for (int i = 0; i < 6; i++) data[i] = mass[i];
                                    item = Item.items.Find(x => x.NameTour == (SelectedTrips.SelectedItem as Item).NameTour);
                                    data[6] = item.NameTour;
                                    data[7] = item.NameCountry;
                                    data[8] = item.NameCity;
                                    data[9] = item.DateBegin;
                                    data[10] = item.DateEnd;
                                    data[11] = item.Price.ToString();
                                    data[12] = DateTime.Today.ToShortDateString();
                                    for (int i = 0; i < 13; i++) //заполнение данных на месте закладок
                                    {
                                        wRange = bookmarks.ElementAt(i).Range;
                                        wRange.Text = data[i];
                                    }
                                    Item.items.Remove(item);
                                    SelectedTrips.Items.Remove(SelectedTrips.SelectedItem);
                                    int flag = sqlManager.ReturnNumber("SELECT Num_Tour FROM Tours WHERE Name_tour = '" + data[6] + "'");
                                    if (flag != -2)
                                    {
                                        int buf = flag;
                                        if (sqlManager.Insert("INSERT INTO Trips(Num_tour, Num_tourist, DateOfBuy) VALUES(" + buf +
                                                              "," + DataPerson.id + ", '" + DateTime.Today.Date + "')") != -2)
                                        {
                                            if ((flag = DataPerson.CountTickets()) != -2)
                                            {
                                                doc.SaveAs(FileName: @"..\Desktop\Ticket" + DataPerson.id + "_" + flag +".docx");
                                                doc.Close();
                                                doc = null;
                                                MessageBox.Show("Ваш билет готов и находится на рабочем столе", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                choiseDataGrid.ItemsSource = null;
                                                choiseDataGrid.Visibility = Visibility.Hidden;
                                                label.Visibility = Visibility.Hidden;
                                                SelectedTrips.Visibility = Visibility.Visible;
                                            }
                                            else
                                            {
                                                PagesManager.ShowConnectionError();
                                            }
                                        }
                                        else
                                        {
                                            PagesManager.ShowConnectionError();
                                        }
                                    }
                                    else
                                    {
                                        PagesManager.ShowConnectionError();
                                    }
                                }
                                else
                                {
                                    PagesManager.ShowConnectionError();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Билеты закончились!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                item = Item.items.Find(x => x.NameTour == (SelectedTrips.SelectedItem as Item).NameTour);
                                Item.items.Remove(item);
                                SelectedTrips.Items.Remove(SelectedTrips.SelectedItem);
                                choiseDataGrid.ItemsSource = null;
                                choiseDataGrid.Visibility = Visibility.Hidden;
                                label.Visibility = Visibility.Hidden;
                                SelectedTrips.Visibility = Visibility.Visible;
                            }
                            System.GC.Collect();
                        }
                        else
                        {
                            PagesManager.ShowConnectionError();
                        }
                    }
                    else
                    {
                        PagesManager.ShowConnectionError();
                    }
                }
                catch (Exception ex)
                {
                    doc.Close();
                    doc = null;
                    Console.WriteLine("Во время выполнения произошла ошибка! " + ex.Message);
                }
            }
        }
        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            SqlManager sqlManager;
            if (DataPerson.right == 3) //создание билета для обычного пользователя
            {
                MakeTicket(null);
            }
            else if (DataPerson.right == 2) //для сотрудника выбор пользователя из списка
            {
                SelectedTrips.Visibility = Visibility.Hidden;
                choiseDataGrid.Visibility = Visibility.Visible;
                label.Visibility = Visibility.Visible;
                sqlManager = new SqlManager();
                DataTable dt = sqlManager.ReturnTable("SELECT * FROM Tourists");
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        choiseDataGrid.Items.Add(new ManyPersons
                        {
                            NamePerson = dr.ItemArray[2].ToString(),
                            LastName = dr.ItemArray[1].ToString(),
                            MiddleName = (dr.ItemArray[3].ToString() == "") ? "Отсутствует" : dr.ItemArray[3].ToString(),
                            SeriaPass = Convert.ToInt32(dr.ItemArray[5]),
                            NumberPass = Convert.ToInt32(dr.ItemArray[6]),
                            PhoneNumber = dr.ItemArray[7].ToString()
                        });
                    }
                }
                else
                {
                    PagesManager.ShowConnectionError();
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //человек для которого будет сформирован билет
            Button person = sender as Button;
            string[] param = new string[]
            {
                (person.DataContext as ManyPersons).NamePerson,
                (person.DataContext as ManyPersons).LastName,
                (person.DataContext as ManyPersons).MiddleName,
                (person.DataContext as ManyPersons).SeriaPass.ToString(),
                (person.DataContext as ManyPersons).NumberPass.ToString(),
                (person.DataContext as ManyPersons).PhoneNumber
            };
            MakeTicket(param);
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Item.items.Remove(Item.items.Find(x => x.NameTour == (SelectedTrips.SelectedItem as Item).NameTour));
            SelectedTrips.Items.Remove(SelectedTrips.SelectedItem);
            System.GC.Collect();
        }
    }
}
