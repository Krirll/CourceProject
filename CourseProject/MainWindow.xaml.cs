using System;
using System.Windows;

namespace CourseProject
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();
        private void Exit_Click(object sender, RoutedEventArgs e) => Close();
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (Login.Text.Length > 0 && Password.Password.Length > 0)
            {
                if (DataManager.CheckInputString(@"^(?=.{3,10}$)[A-Za-z0-9]*$", Login.Text)) //предварительная проверка логина
                {
                    if (DataManager.CheckInputString(@"^(?=.{5,20}$)[A-Za-z0-9!#$%^&*]*$", Password.Password)) //предварительная проверка пароля
                    {
                        SqlManager connection = new SqlManager();
                        int flag = connection.ExistsAccount(Login.Text, Password.Password); //проверка на существование аккаунта
                        if (flag == 0)
                        {
                            //получение прав доступа
                            DataPerson.right = connection.ReturnNumber("SELECT access_right FROM Accounts " +
                                                                        "WHERE login = '" + DataManager.MakeHash(Login.Text).ToString() + "'");
                            if (DataPerson.right != -2)
                            {
                                //получение номера аккаунта в зависимости от прав
                                if (DataPerson.right == 3)
                                {
                                    DataPerson.id = connection.ReturnNumber("SELECT Num_tourist FROM Accounts " +
                                                                             "WHERE login = '" + DataManager.MakeHash(Login.Text).ToString() + "'");
                                    if (DataPerson.id == -2) flag = -2;
                                }
                                else if (DataPerson.right == 2)
                                {
                                    DataPerson.id = connection.ReturnNumber("SELECT Num_worker FROM Accounts " +
                                                                             "WHERE login = '" + DataManager.MakeHash(Login.Text).ToString() + "'");
                                    if (DataPerson.id == -2) flag = -2;
                                }
                                if ((DataPerson.right == 2 || DataPerson.right == 3) && flag != -2)
                                {
                                    TourismWindow tw = new TourismWindow();
                                    tw.Show();
                                    Close();
                                }
                                else if (DataPerson.right == 1 && flag != -2)
                                {
                                    WindowAdmin wa = new WindowAdmin();
                                    wa.Show();
                                    Close();
                                }
                                else ShowConnectionError();
                                System.GC.Collect();
                            }
                            else ShowConnectionError();
                        }
                        else if (flag == -2) ShowConnectionError();
                        else ShowError();
                    }
                    else ShowError();
                }
                else ShowError();
            }
            else MessageBox.Show("Необходимо ввести пароль и логин.");
        }
        private void Registr_Click(object sender, RoutedEventArgs e)
        {
            WindowChoise wr = new WindowChoise();
            wr.Show();
            Close();
            System.GC.Collect();
        }
        private void ShowError()
        {
            MessageBox.Show("Неверно введен логин или пароль." + '\n' + "Попробуйте снова.");
            Login.Text = "";
            Password.Password = "";
        }
        private void ShowConnectionError()
        {
            MessageBox.Show("Отсутствует соединение с сервером,\nповторите попытку позже.");
            Login.Text = "";
            Password.Password = "";
        }
    }
}
