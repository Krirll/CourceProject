using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Data;

namespace CourseProject
{
    class DataManager
    {
        public static void AddNewListViewItem(ListView List, DataRow row)
        {
            //добавление новой путеввки в ListView
            if (Convert.ToInt32(row.ItemArray[2]) != 0)
            {
                List.Items.Add(new Item
                {
                    NameTour = row.ItemArray[0].ToString(),
                    NameCountry = row.ItemArray[7].ToString(),
                    NameCity = row.ItemArray[6].ToString(),
                    DateBegin = row.ItemArray[4].ToString().Remove(row.ItemArray[4].ToString().Length - 8, 8),
                    DateEnd = row.ItemArray[5].ToString().Remove(row.ItemArray[4].ToString().Length - 8, 8),
                    Price = Convert.ToInt32(row.ItemArray[1]),
                    ActualCount = Convert.ToInt32(row.ItemArray[2]),
                    Images = (row.ItemArray[3].ToString() == "") ? null : (byte[])row.ItemArray[3]
                });
            }
        }
        public static ComboBox PushComboBox(ComboBox x, string column, string sql)
        {
            SqlManager sqlManager = new SqlManager();
            DataTable dt;
            if ((dt = sqlManager.ReturnTable(sql)) != null)
            {
                x.ItemsSource = dt.AsDataView();
                x.DisplayMemberPath = column;
            }
            else x = null;
            return x;
        }
        public static bool CheckInputString(string regex, string inputStr)
        {
            //проверка строки регулярным выражением
            Regex regularEx = new Regex(regex);
            MatchCollection matches = regularEx.Matches(inputStr);
            return (matches.Count == 1) ? true : false;
        }
        public static StringBuilder MakeHash(string str)
        {
            //создание хэша для строки
            byte[] tmpSource = ASCIIEncoding.ASCII.GetBytes(str);
            byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            StringBuilder outputStr = new StringBuilder(tmpHash.Length);
            for (int i = 0; i < tmpHash.Length - 1; i++) outputStr.Append(tmpHash[i].ToString("X2"));
            return outputStr;
        }
    }
}
