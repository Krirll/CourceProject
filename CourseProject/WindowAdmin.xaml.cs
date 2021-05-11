using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;

namespace CourseProject
{
    public partial class WindowAdmin : Window
    {
        SqlManager sqlManager;
        DataTable dataTable;
        bool flag;
        string sqlTable, sqlRow;
        int numColumn;
        Dictionary<string, string> listRegexs;
        public WindowAdmin()
        {
            InitializeComponent();
            sqlManager = new SqlManager();
            listRegexs = new Dictionary<string, string> 
            {
                { "String", @"[А-яа-я0-9\ ]{2,50}" },
                { "Int32", @"\d{1,10}"},
                { "Date", @"^(?:(?:31(.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$" },
                { "Decimal", @"^\d*(?(?=\d.{1}\d)\d*.{1}\d*)$"}
            };
            if ((tables = DataManager.PushComboBox(tables, "name", "SELECT name FROM sys.tables " +
                                                  "WHERE name != 'sysdiagrams' AND name != 'Trips' AND name != 'Accounts' AND name != 'Admins' AND name != 'Passwords'")) == null)
            {
                ShowConnectionError();
            } 
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            Close();
        }
        private void tables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sqlTable = "SELECT * FROM " + (tables.SelectedItem as DataRowView).Row.ItemArray[0].ToString();
            ShowTable();
            AddNew.Visibility = Visibility.Visible;
            if (flag == true || flag == false)
            {
                Update.Visibility = Visibility.Hidden;
                Reset.Visibility = Visibility.Hidden;
                Delete.Visibility = Visibility.Hidden;
                Save.Visibility = Visibility.Hidden;
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int flag = sqlManager.Delete("DELETE FROM " + (tables.SelectedItem as DataRowView).Row.ItemArray[0].ToString() +
                                         " WHERE " + dataTable.Columns[0] + " = " + (datagrid.SelectedItem as DataRowView).Row.ItemArray[0]);
            if (flag == 0) MessageBox.Show("Запись невозможно удалить из-за ограничения!");
            else if (flag == -2) ShowConnectionError();
            else
            {
                dataTable.Rows.Remove((datagrid.SelectedItem as DataRowView).Row);
                ShowTable();
                MessageBox.Show("Запись успешно удалена.");
            }
            AddNew.Visibility = Visibility.Visible;
            Update.Visibility = Visibility.Hidden;
            Delete.Visibility = Visibility.Hidden;
            Save.Visibility = Visibility.Hidden;
            Reset.Visibility = Visibility.Hidden;
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            flag = false;
            sqlRow = "SELECT * FROM " + (tables.SelectedItem as DataRowView).Row.ItemArray[0].ToString() +
                  " WHERE " + (datagrid.SelectedItem as DataRowView).DataView.Table.Columns[0] +
                  " = '" + (datagrid.SelectedItem as DataRowView).Row.ItemArray[0] + "'";
            bool error = ShowRow();
            datagrid.IsReadOnly = false;
            datagrid.Columns[1].IsReadOnly = true; //первичные ключи
            datagrid.CanUserAddRows = false;
            Update.Visibility = Visibility.Hidden;
            Delete.Visibility = Visibility.Hidden;
            Save.Visibility = Visibility.Visible;
            if (error != true) MessageBox.Show("Измените поля и нажмите \"Сохранить\".");
        }
        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            flag = true;
            dataTable.Rows.Clear();
            datagrid.CanUserAddRows = true;
            datagrid.IsReadOnly = false;
            datagrid.Columns[1].IsReadOnly = true;
            Reset.Visibility = Visibility.Visible;
            Save.Visibility = Visibility.Visible;
            AddNew.Visibility = Visibility.Hidden;
            MessageBox.Show("Заполните пустые поля и нажмите \"Сохранить\".\nВвод данных осуществляется только в первой строке!");
        }
        private void datagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGridCellInfo cellInfo = datagrid.CurrentCell;
            if (datagrid.CurrentCell.Column != null)
            {
                numColumn = datagrid.CurrentCell.Column.DisplayIndex - 1;
                if ((cellInfo.Column.GetCellContent(cellInfo.Item) as TextBox) != null)
                {
                    if ((cellInfo.Column.GetCellContent(cellInfo.Item) as TextBox).Text != "")
                    {
                        DataTable dt = sqlManager.ReturnTable("SELECT * FROM " + dataTable.TableName + " WHERE 1 = 2");
                        if (dt != null) 
                        {
                            if (dt.Columns[numColumn].DataType.Name == "DateTime")
                            {
                                if (DataManager.CheckInputString(listRegexs.First(x => x.Key == "Date").Value, (cellInfo.Column.GetCellContent(cellInfo.Item) as TextBox).Text) == true)
                                {
                                    e.Cancel = false;
                                }
                                else
                                {
                                    MessageBox.Show("Изменения не зафиксированы, некорректный ввод!\nДата вводится по формату dd.mm.yyyy");
                                    e.Cancel = true;
                                }
                            }
                            else if (DataManager.CheckInputString(listRegexs.First(x => x.Key == dataTable.Columns[numColumn].DataType.Name).Value, (cellInfo.Column.GetCellContent(cellInfo.Item) as TextBox).Text) == true)
                            {
                                if (dataTable.Columns[numColumn].DataType.Name == "Int32")
                                {
                                    int res = sqlManager.CheckKey(dataTable.TableName, cellInfo.Column.Header.ToString(), Convert.ToInt32((cellInfo.Column.GetCellContent(cellInfo.Item) as TextBox).Text));
                                    if (res == 0 || res == 1)
                                    {
                                        e.Cancel = false;
                                    }
                                    else if (res == -2)
                                    {
                                        ShowConnectionError();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Строка с таким номером не найдена!");
                                        e.Cancel = true;
                                    }

                                }
                                else
                                {
                                    e.Cancel = false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Изменения не зафиксированы, некорректный ввод!");
                                e.Cancel = true;
                            }
                        }
                        else
                        {
                            ShowConnectionError();
                        }
                    }
                    else if (dataTable.Columns[numColumn].AllowDBNull == true && (cellInfo.Column.GetCellContent(cellInfo.Item) as TextBox).Text == "")
                    {
                        if (dataTable.Columns[numColumn].DataType.Name == "Int32") (cellInfo.Column.GetCellContent(cellInfo.Item) as TextBox).Text = "0";
                        e.Cancel = false;
                    }
                    else
                    {
                        MessageBox.Show("Изменения не зафиксированы,\nв этом столбце запрещены значения NULL!");
                        e.Cancel = true;
                    }
                }
            }
        }
        private void datagrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
                numColumn = datagrid.CurrentCell.Column.DisplayIndex - 1;
            if (dataTable.Columns[numColumn].DataType.Name == "Byte[]")
            {
                if (dataTable.Rows.Count != 0)
                {
                    MessageBoxResult result = MessageBox.Show("Да - удалить фотографию, Нет - изменить/добавить", "", MessageBoxButton.YesNoCancel);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            dataTable.Rows[0][numColumn] = null;
                            datagrid.ItemsSource = dataTable.AsDataView();
                            break;
                        case MessageBoxResult.No:
                            var dialog = new OpenFileDialog();
                            if (dialog.ShowDialog() == true)
                            {
                                if (dialog.FileName.Contains(".jpg") || dialog.FileName.Contains(".png"))
                                {
                                    dataTable.Rows[0][numColumn] = File.ReadAllBytes(dialog.FileName);
                                    datagrid.ItemsSource = dataTable.AsDataView();
                                }
                                else
                                {
                                    MessageBox.Show("Неверный формат");
                                }
                            }
                            break;
                        case MessageBoxResult.Cancel:
                            e.Cancel = true;
                            break;
                    }
                }
                else if (dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Работа с изображениями доступна\nтолько в режиме редактирования.");
                    datagrid.ItemsSource = dataTable.AsDataView();
                }
            }   
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool checkError = false;
            if (flag == true)
            {
                //for add
                List<DataGridCellInfo> cells = new List<DataGridCellInfo>();
                datagrid.SelectAllCells();
                int i = 1;
                for (int j = 1; j < datagrid.Columns.Count; j++) cells.Add(datagrid.SelectedCells[j]);
                foreach (DataGridCellInfo cell in cells)
                {
                    if ((cell.Column.GetCellContent(cell.Item) as TextBlock).Text == "" && dataTable.Columns[i-1].AllowDBNull == false)
                    {
                        checkError = true;
                        MessageBox.Show($"Ячейка в столбце №{i} не может быть пустой!");
                    }
                    i++;
                }
                if (checkError != true)
                {
                    int flag = sqlManager.AddRow(dataTable.TableName, (datagrid.Items[0] as DataRowView).Row.ItemArray.ToList(), dataTable.Columns);
                    if (flag == -1)
                    {
                        checkError = true;
                        MessageBox.Show("Не удалось сохранить изменения!\nПроверьте правильность введенных данных.");
                    }
                    else if (flag == -2)
                    {
                        ShowConnectionError();
                    }
                    else
                    {
                        datagrid.CanUserAddRows = false;
                        bool error = ShowTable();
                        AddNew.Visibility = Visibility.Visible;
                        Update.Visibility = Visibility.Hidden;
                        Delete.Visibility = Visibility.Hidden;
                        Save.Visibility = Visibility.Hidden;
                        Reset.Visibility = Visibility.Hidden;
                        datagrid.IsReadOnly = true;
                        if (error != true) MessageBox.Show("Запись успешно добавлена!");
                    }
                }
            }
            else
            {
                //for edit
                int flag = sqlManager.UpdateRow(dataTable.TableName, dataTable.Rows[0].ItemArray.ToList(), dataTable.Columns);
                if (flag == -1)
                {
                    MessageBox.Show("Не удалось добавить строку!\nПроверьте правильность введенных данных.");
                }
                else if (flag == -2)
                {
                    ShowConnectionError();
                }
                else
                {
                    bool error = ShowTable();
                    AddNew.Visibility = Visibility.Visible;
                    Update.Visibility = Visibility.Hidden;
                    Delete.Visibility = Visibility.Hidden;
                    Save.Visibility = Visibility.Hidden;
                    Reset.Visibility = Visibility.Hidden;
                    datagrid.IsReadOnly = true;
                    if (error != true) MessageBox.Show("Запись успешно изменена!");
                }
            }
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            AddNew.Visibility = Visibility.Visible;
            Update.Visibility = Visibility.Hidden;
            Delete.Visibility = Visibility.Hidden;
            Save.Visibility = Visibility.Hidden;
            Reset.Visibility = Visibility.Hidden;
            datagrid.ItemsSource = null;
            datagrid.IsReadOnly = true;
            if ((dataTable = sqlManager.ReturnFormatTable(dataTable, sqlTable, (tables.SelectedItem as DataRowView).Row.ItemArray[0].ToString())) != null)
            {
                datagrid.ItemsSource = dataTable.AsDataView();
            }
            else
            {
                ShowConnectionError();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddNew.Visibility = Visibility.Hidden;
            Update.Visibility = Visibility.Visible;
            Delete.Visibility = Visibility.Visible;
            Reset.Visibility = Visibility.Visible;
        }
        private bool ShowTable()
        {
            bool flag = false;
            if ((dataTable = sqlManager.ReturnTable(sqlTable + " WHERE 1 = 2")) != null)
            {
                if ((dataTable = sqlManager.ReturnFormatTable(dataTable, sqlTable, (tables.SelectedItem as DataRowView).Row.ItemArray[0].ToString())) != null)
                {
                    datagrid.ItemsSource = dataTable.AsDataView();
                }
                else
                {
                    flag = true;
                    ShowConnectionError();
                }
            }
            else
            {
                flag = true;
                ShowConnectionError();
            }
            return flag;
        }
        private bool ShowRow()
        {
            bool flag = false;
            if ((dataTable = sqlManager.ReturnFormatTable(dataTable, sqlRow, (tables.SelectedItem as DataRowView).Row.ItemArray[0].ToString())) != null)
            {
                datagrid.ItemsSource = dataTable.AsDataView();
            }
            else
            {
                flag = true;
                ShowConnectionError();
            }
            return flag;
        }
        private void ShowConnectionError()
        {
            MessageBox.Show("Отсутствует соединение с сервером,\nповторите попытку позже.");
            foreach (Window window in Application.Current.Windows)
            {
                if (window is WindowAdmin)
                {
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    window.Close();
                }
            }
        }
    }
}
