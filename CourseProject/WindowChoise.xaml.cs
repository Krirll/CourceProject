using System;
using System.Windows;

namespace CourseProject
{
    public partial class WindowChoise : Window
    {
        public WindowChoise()
        {
            InitializeComponent();
        }
        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            int flag = 0;
            if (common.IsChecked != null && admin.IsChecked != null && worker.IsChecked != null)
            {
                SqlManager sqlManager;
                if (common.IsChecked == true)
                {
                    WindowRegistration wr = new WindowRegistration();
                    wr.Show();
                    DataPerson.right = 3;
                    Close();
                }
                else if (admin.IsChecked == true) //12345 пароль
                {
                    sqlManager = new SqlManager();
                    //проверка правильности ввода пароля для регистрации администратора
                    if (DataManager.CheckInputString(@"\d{5}", passAdmin.Password) != false)
                    {
                        flag = sqlManager.Select("SELECT pass_admin FROM Passwords " +
                        "WHERE pass_admin = '" + DataManager.MakeHash(passAdmin.Password) + "'", 1);
                        if (flag != -2)
                        {
                            if (flag != -1)
                            {
                                DataPerson.right = 1;
                                WindowRegistration wr = new WindowRegistration();
                                wr.Show();
                                Close();
                            }
                            else
                            {
                                MessageBox.Show("Пароль неверный", "", MessageBoxButton.OK, MessageBoxImage.Error);
                                passAdmin.Password = "";
                            }
                        }
                        else
                        {
                            ShowConnectionError();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пароль неверный", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        passAdmin.Password = "";
                    }
                }
                else if (worker.IsChecked == true) //54321 пароль
                {
                    //проверка правильности ввода пароля для регистрации работника
                    sqlManager = new SqlManager();
                    if (DataManager.CheckInputString(@"\d{5}", passWorker.Password) != false && flag != -1)
                    {
                        flag = sqlManager.Select("SELECT pass_worker FROM Passwords " +
                        "WHERE pass_worker = '" + DataManager.MakeHash(passAdmin.Password) + "'", 1);
                        if (flag != -2)
                        {
                            if (flag != -1)
                            {
                                DataPerson.right = 2;
                                WindowRegistration wr = new WindowRegistration();
                                wr.Show();
                                Close();
                            }
                            else
                            {
                                MessageBox.Show("Пароль неверный", "", MessageBoxButton.OK, MessageBoxImage.Error);
                                passAdmin.Password = "";
                            }
                        }
                        else
                        {
                            ShowConnectionError();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пароль неверный", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        passWorker.Password = "";
                    }
                }
            }
            else MessageBox.Show("Необходимо выбрать!\nДля отмены нажмите \"Назад\".", "", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            Close();
        }
        private void admin_Checked(object sender, RoutedEventArgs e)
        {
            label_admin.Visibility = Visibility.Visible;
            passAdmin.Visibility = Visibility.Visible;
            label_worker.Visibility = Visibility.Hidden;
            passWorker.Visibility = Visibility.Hidden;
            passWorker.Password = "";
        }
        private void common_Checked(object sender, RoutedEventArgs e)
        {
            label_admin.Visibility = Visibility.Hidden;
            label_worker.Visibility = Visibility.Hidden;
            passAdmin.Visibility = Visibility.Hidden;
            passWorker.Visibility = Visibility.Hidden;
            passAdmin.Password = "";
            passWorker.Password = "";
        }
        private void worker_Checked(object sender, RoutedEventArgs e)
        {
            label_worker.Visibility = Visibility.Visible;
            passWorker.Visibility = Visibility.Visible;
            label_admin.Visibility = Visibility.Hidden;
            passAdmin.Visibility = Visibility.Hidden;
            passAdmin.Password = "";
        }
        private void ShowConnectionError()
        {
            MessageBox.Show("Отсутствует соединение с сервером,\nповторите попытку позже.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            foreach (Window window in Application.Current.Windows)
            {
                if (window is WindowChoise)
                {
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    Close();
                }
            }
        }
    }
}
