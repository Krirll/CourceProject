using System;
using System.Windows.Controls;
using System.Data;
using System.Windows;

namespace CourseProject
{
    public partial class PageHistory : Page
    {
        public PageHistory()
        {
            //получение истории пользователя
            InitializeComponent();
            SqlManager sqlManager = new SqlManager();
            DataTable dt = sqlManager.ReturnTable("SELECT Tours.Name_tour, Price, DateOfBuy " +
                                                  "FROM Trips JOIN Tours ON Tours.Num_tour = Trips.Num_tour " +
                                                  "WHERE Num_tourist = " + DataPerson.id);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    HistoryGrid.Items.Add(new Item
                    {
                        NameTour = dr.ItemArray[0].ToString(),
                        Price = Convert.ToInt32(dr.ItemArray[1]),
                        DateBuy = dr.ItemArray[2].ToString().Remove(dr.ItemArray[2].ToString().Length - 8, 8)
                    });
                    System.GC.Collect();
                }
            }
            else
            {
                PagesManager.ShowConnectionError();
            }
        }
    }
}
