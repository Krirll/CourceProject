using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Controls;
using System.Linq;

namespace CourseProject
{
    class SqlManager
    {
        public SqlConnection connection;
        public SqlManager()
        {
            try
            {
                string connectionString = @"Data Source=LAPTOP-KF4LLVT0\SQLEXPRESS;
                                        Initial Catalog=Tourism;
                                        Integrated Security=True";
                connection = new SqlConnection(connectionString);
            }
            catch (SqlException ex) 
            { 
                Console.WriteLine(ex.Message);
            }
        }
        public string[] ReturnDataPerson()
        {
            //получение данных о пользователе
            string[] str = null;
            DataRow dr = null;
            DataTable dt;
            if (DataPerson.right == 3)
            {
                if ((dt = ReturnTable("SELECT Name, Last_name, Middle_name, Passport_series, Passport_number, Phone_number " +
                                         "FROM Tourists " +
                                         "WHERE Num_tourist = " + DataPerson.id)) != null)
                {
                    dr = dt.Rows[0];
                }
            }
            else if (DataPerson.right == 2)
            {
                if ((dt = ReturnTable("SELECT Name, Last_name, Middle_name, Passport_series, Passport_number, Phone_number " +
                                         "FROM Workers " +
                                         "WHERE Num_worker = " + DataPerson.id)) != null)
                {
                    dr = dt.Rows[0];
                }
            }
            if (dr != null)
            {
                str = new string[6]
                {
                    dr.ItemArray[0].ToString(),
                    dr.ItemArray[1].ToString(),
                   (dr.ItemArray[2].ToString() == "") ? "Отсутствует" : dr.ItemArray[2].ToString(),
                    dr.ItemArray[3].ToString(),
                    dr.ItemArray[4].ToString(),
                    dr.ItemArray[5].ToString()
                };
            }
            else str = null;
            System.GC.Collect();
            return str;
        }
        public int UpdatePerson(List<string> list)
        {
            //обновление данных пользователя с внедренными изменениями
            int flag = -2;
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Tourists SET ", connection);
                string str = "";
                foreach (string data in list) str += data;
                str = str.Remove(str.Length - 2, 2);
                cmd.CommandText += str + " WHERE Num_tourist = " + DataPerson.id;
                flag = cmd.ExecuteNonQuery();
                connection.Close();
                System.GC.Collect();
            }
            catch (SqlException ex) 
            {
                Console.WriteLine(ex.Message);
            }
            return flag;
        }
        public DataTable ReturnTable(string sql)
        {
            //получение таблицы по запросу из БД
            DataTable dt = null;
            try
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                dt = new DataTable();
                adapter.Fill(dt);
                connection.Close();
                System.GC.Collect();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dt;
        }
        public int UpdateActualCountTripsInDB()
        {
            //обновление актуального количества билетов на каждую путевку
            int flag = 0;
            SqlCommand cmd;
            DataTable dt = ReturnTable("SELECT Tours.Num_tour, count(Trips.Num_tour) " +
                                               "FROM trips " +
                                               "RIGHT OUTER JOIN tours on tours.Num_tour = trips.Num_tour " +
                                               "GROUP BY trips.Num_tour, tours.Num_tour ");
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cmd = new SqlCommand("UPDATE Tours SET ActualCount = CountTrips - " + dr.ItemArray[1] +
                                            "WHERE Num_tour = " + dr.ItemArray[0], connection);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            else flag = -2;
            System.GC.Collect();
            return flag;
        }
        public SqlDataAdapter TypeSort(string sql, ref SqlDataAdapter da, int SelectedSort)
        {
            //присваивание сортировки в зависимости от переданной сортировки
            if (SelectedSort == 0) sql += " ORDER BY Price";
            else if (SelectedSort == 1) sql += " ORDER BY Price DESC";
            else if (SelectedSort == 2) sql += " ORDER BY Date_begin";
            else if (SelectedSort == 3) sql += " ORDER BY Date_begin DESC";
            else if (SelectedSort == 4) sql += " ORDER BY Name_country";
            else if (SelectedSort == 5) sql += " ORDER BY Name_country DESC";
            else if (SelectedSort == 6) sql += " ORDER BY Name_city";
            else if (SelectedSort == 7) sql += " ORDER BY Name_city DESC";
            da = new SqlDataAdapter(sql, connection);
            return da;
        }
        public string AddConditions(ref string x, List<string> listParams)
        {
            //добавление условий к запросу на заполнение ListView
            string[] array = new string[5] { "Name_country", "Name_city", "Price", "Date_begin", "Date_end" };
            for (int i = 0; i <= listParams.Count - 1; i++)
            {
                if (listParams.ElementAt(i) != "" && listParams.ElementAt(i) != "1000")
                {
                    if (i == 2) x += array[i] + " >= " + Math.Round(Convert.ToDouble(listParams.ElementAt(i)), 0) + " AND ";
                    else if (i == 3) x += array[i] + " >= " + "'" + listParams.ElementAt(i) + "' AND ";
                    else if (i == 4) x += array[i] + " <= " + "'" + listParams.ElementAt(i) + "' AND ";
                    else x += array[i] + " = " + "'" + listParams.ElementAt(i) + "' AND ";
                }
            }
            x = x.Remove(x.Length - 4, 4);
            return x;
        }
        public int AddItems(ref ListView List, string sql, int SelectedSort)
        {
            //добавление элементов в ListView
            int flag = 0;
            try
            {
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter("", connection);
                TypeSort(sql, ref da, SelectedSort);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    DataManager.AddNewListViewItem(List, row);
                    System.GC.Collect();
                }
                connection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                flag = -2;
            }
            return flag;
        }
        public ListView PushListView(ListView List, List<string> listParams, int cityNum, int SelectedSort, string search)
        {
            string sql = "SELECT * FROM PushListView WHERE ";
            if (search != "")
            {
                if (DataManager.CheckInputString(@"[А-Яа-я]{1,30}$", search) == true)
                {
                    sql += " Name_city LIKE '%" + search + "%' AND ";
                }
            }
            AddConditions(ref sql, listParams);
            if (AddItems(ref List, sql, SelectedSort) == -2) List = null;
            System.GC.Collect();
            return List;
        }
        public int ExistsAccount(string login, string password)
        {
            int flag = -2;
            SqlCommand cmd = new SqlCommand("ifExists", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@login", SqlDbType.NVarChar).Value = DataManager.MakeHash(login).ToString();
            cmd.Parameters.Add("@pass", SqlDbType.NVarChar).Value = DataManager.MakeHash(password).ToString();
            try
            {
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                flag = (reader.Read()) ? 0 : -1;
                connection.Close();
                System.GC.Collect();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return flag;
        }
        public int ReturnNumber(string sql)
        {
            //возвращение числа по запросу
            int id = -2;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                id = -1;
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    id = (dr.Read()) ? (dr.GetValue(0) != null) ? dr.GetInt32(0) : -1 : -1;
                    connection.Close();
                    System.GC.Collect();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return id;
        }
        public int Insert(string sql)
        {
            //добавление строки в БД
            int result = -2;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                result = cmd.ExecuteNonQuery();
                connection.Close();
                System.GC.Collect();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result; 
        }
        public int Select(string sql, int countColumns)
        {
            //получение строк из БД
            int flag = -2;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader dt = cmd.ExecuteReader();
                if (dt.Read())
                {
                    for (int i = 0; i <= countColumns - 1 && flag != -1; i++)
                        flag = (dt.GetValue(i) != null) ? 0 : -1;
                }
                else flag = -1;
                connection.Close();
                System.GC.Collect();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return flag;
        }
        public int Delete(string sql)
        {
            //удаление из БД
            int res = -2;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                res = cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) res = 0;
                Console.WriteLine(ex.Message);
            }
            connection.Close();
            return res;
        }
        public DataTable ReturnFormatTable(DataTable dataTable, string sql, string tableName)
        {
            //возвращает форматированную таблицу для DatGrid
            dataTable = null;
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(sql, connection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sql, connection);
                DataSet ds = new DataSet();
                sqlDataAdapter.FillSchema(ds, SchemaType.Source, tableName);
                dataTable = ds.Tables[0];
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    dataTable.Rows.Clear();
                    List<string> headers = new List<string>();
                    for (int cur = 0; cur < dataTable.Columns.Count; cur++) headers.Add(dataTable.Columns[cur].DataType.ToString());
                    int i;
                    while ((i = headers.FindIndex(x => x.Contains("Date"))) != -1)
                    {
                        dataTable.Columns[i].DataType = typeof(string);
                        headers[i] = "";
                    }
                    while (reader.Read())
                    {
                        DataRow newRow;
                        newRow = dataTable.NewRow();
                        for (int j = 0; j < reader.FieldCount; j++)
                        {
                            if (reader.GetDataTypeName(j) == "date")
                            {
                                newRow[dataTable.Columns.IndexOf(dataTable.Columns[j])] = reader.GetDateTime(j).ToString("dd.MM.yyyy");
                            }
                            else
                            {
                                newRow[dataTable.Columns.IndexOf(dataTable.Columns[j])] = reader.GetValue(j);
                            }
                        }
                        dataTable.Rows.Add(newRow);
                    }
                }
                connection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dataTable;
        }
        public int CheckKey(string table, string column, int num)
        {
            //проверка зависимостей определенного поля таблицы
            int flag = -2;
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("CheckForeignKey", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@table", table);
                cmd.Parameters.AddWithValue("@column", column);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        string primaryColumn;
                        if ((primaryColumn = reader.GetString(0)) != null)
                        {
                            reader.Close();
                            primaryColumn = primaryColumn.Remove(0, 3);
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "SELECT * FROM " + primaryColumn + " WHERE " + column + " = " + num;
                            reader = cmd.ExecuteReader();
                            flag = (reader.HasRows) ? 1 : 2;
                        }
                    }
                }
                else flag = 0;
                connection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return flag;
        }
        public int UpdateRow(string tableName, List<object> values, DataColumnCollection headers)
        {
            //обновление строки в БД
            int res = -2;
            try
            {
                connection.Open();
                string str = "UPDATE " + tableName + " SET ",
                    endOfStr = $" WHERE {headers[0].ColumnName} = {values[0]}";
                SqlCommand cmd = new SqlCommand();
                values.RemoveAt(0);
                int i = 0;
                foreach (DataColumn dt in headers)
                {
                    if (i != 0)
                    {
                        str += dt.ColumnName + " = " + $"@{dt.ColumnName}, ";
                        if (values.First().ToString() == "")
                        {
                            if (dt.DataType.Name == "Byte[]")
                            {
                                cmd.Parameters.Add($"@{dt.ColumnName}", SqlDbType.Image).Value = DBNull.Value;
                            }
                            else cmd.Parameters.AddWithValue($"@{dt.ColumnName}", values.First());
                        }
                        else cmd.Parameters.AddWithValue($"@{dt.ColumnName}", values.First());
                        values.RemoveAt(0);
                    }
                    i++;
                }
                str = str.Remove(str.Length - 2, 2);
                cmd.CommandText = str + endOfStr;
                cmd.Connection = connection;
                res = cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }
        public int AddRow(string tableName, List<object> values, DataColumnCollection headers)
        {
            //добавление строк в БД
            int res = -2;
            try
            {
                connection.Open();
                string str = "INSERT INTO " + tableName + "(";
                SqlCommand cmd = new SqlCommand();
                for (int i = 1; i < headers.Count; i++)
                {
                    if (values[i].ToString() != "") str += headers[i].ColumnName + ", ";
                }
                str = str.Remove(str.Length - 2, 2) + ") VALUES(";
                for (int i = 1; i < headers.Count; i++) //add values
                {
                    if (values[i].ToString() != "")
                    {
                        str += $"@{headers[i].ColumnName}, ";
                        cmd.Parameters.AddWithValue($"@{headers[i].ColumnName}", values[i]);
                    }
                }
                str = str.Remove(str.Length - 2, 2) + ")";
                cmd.CommandText = str;
                cmd.Connection = connection;
                res = cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }
    }
}
