using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CourseProject
{
    //отступ слева для текст бокса 240
    public partial class PagePerson : Page
    {
        SqlManager sqlManager;
        int flag;
        public void WriteDataPerson()
        {
            if (DataPerson.NamePerson == null)
            {
                if (DataPerson.NewDataPerson() == -1)
                {
                    PagesManager.ShowConnectionError();
                }
            }
            name.Content = DataPerson.NamePerson; 
            Lastname.Content = DataPerson.LastName;
            MiddleName.Content = DataPerson.MiddleName;
            SeriaPass.Content = DataPerson.SeriaPass;
            NumberPass.Content = DataPerson.NumberPass;
            Phone.Content = DataPerson.PhoneNumber;
        }
        public PagePerson()
        {
            InitializeComponent();
            sqlManager = new SqlManager();
            WriteDataPerson();
            if (DataPerson.right == 2) Delete.Visibility = Visibility.Hidden;
        }
        private void UpdateDataPerson(List<string> list)
        {
            flag = sqlManager.UpdatePerson(list);
            if (flag != -2)
            {
                ResetEdit();
                DataPerson.NewDataPerson();
                WriteDataPerson();
            }
            else if (flag == -2)
            {
                PagesManager.ShowConnectionError();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (NameEdit.Text != "" || LastNameEdit.Text != "" || MiddleNameEdit.Text != "" || PhoneEdit.Text != "")
            {
                int error = 0;
                List<string> list = new List<string>();
                if (NameEdit.Text != "")
                {
                    if (DataManager.CheckInputString(@"^(?:(?![ЫЫЙЪЬЁЦЩ])[А-Я])[а-я]{1,30}$", NameEdit.Text) == false)
                    {
                        ShowError("Имя введено неверно." + '\n' + "Имя должно содержать не более 30, не менее 2 букв русского алфавита" +
                        '\n' + "и начинаться с заглавной буквы русского алфавита.");
                        NameEdit.Text = "";
                        error++;
                    }
                    else
                    {
                        list.Add("Name = '" + NameEdit.Text + "', ");
                        if (error > 0) error--;
                    }
                }
                if (LastNameEdit.Text != "")
                {
                    if (DataManager.CheckInputString(@"^(?:(?!([ЬЪЫ]))[А-Я])[а-я]{0,30}$", LastNameEdit.Text) == false)
                    {
                        ShowError("Фамилия введена неверно." + '\n' + "Фамилия должна содержать не более 30, не менее 2 букв русского алфавита" +
                           '\n' + "и начинаться с заглавной буквы русского алфавита.");
                        LastNameEdit.Text = "";
                        if (error > 0) error++;
                    }
                    else
                    {
                        list.Add("Last_name = '" + LastNameEdit.Text + "', ");
                        if (error > 0) error--;
                    }
                }
                if (MiddleNameEdit.Text != "")
                {
                    if (DataManager.CheckInputString(@"^(?:(?![ЫЫЙЪЬЁЦЩ])[А-Я])[а-я]{1,30}$", MiddleNameEdit.Text) == false)
                    {
                        ShowError("Отчество должно содержать не более 40, не менее 4 букв русского алфавита" +
                            '\n' + "и начинаться с заглавной буквы русского алфавита.");
                        MiddleNameEdit.Text = "";
                        if (error > 0) error++;
                    }
                    else
                    {
                        list.Add("Middle_name = '" + MiddleNameEdit.Text + "', ");
                        if (error > 0) error--;
                    }
                }
                if (PhoneEdit.Text != "")
                {
                    if (DataManager.CheckInputString(@"^\+7\d{10}$", PhoneEdit.Text) == false)
                    {
                        ShowError("Номер телефона введен неверно." + '\n' + "Номер телефона должен начинаться с +7"
                                + '\n' + "и после этого должны быть 10 цифр.");
                        PhoneEdit.Text = "";
                        if (error > 0) error++;
                    }
                    else
                    {
                        list.Add("Phone_number = '" + PhoneEdit.Text + "', ");
                        if (error > 0) error--;
                    }
                }
                if (list.Count > 0 && error == 0) UpdateDataPerson(list);
            }
            else
            {
                ShowError("Пустые поля не допускаются.\nДолжно быть изменено хотя бы одно поле.\nДля выхода из редактора нажмите \"Отмена\".");
            }
            System.GC.Collect();
        }
        private void ResetEdit()
        {
            if (DataPerson.right != 2) Delete.Visibility = Visibility.Visible;
            Edit.Visibility = Visibility.Visible;
            Current.Visibility = Visibility.Hidden;
            NameEdit.Visibility = Visibility.Hidden;
            LastNameEdit.Visibility = Visibility.Hidden;
            MiddleNameEdit.Visibility = Visibility.Hidden;
            PhoneEdit.Visibility = Visibility.Hidden;
            Save.Visibility = Visibility.Hidden;
            Reset.Visibility = Visibility.Hidden;
            name.Margin = new Thickness(354, 14, 0, 0);
            Lastname.Margin = new Thickness(354, 14, 0, 0);
            MiddleName.Margin = new Thickness(354, 14, 0, 0);
            Phone.Margin = new Thickness(354, 14, 0, 0);
            NameEdit.Text = "";
            LastNameEdit.Text = "";
            MiddleNameEdit.Text = "";
            PhoneEdit.Text = "";
        }
        private void Reset_Click(object sender, RoutedEventArgs e) => ResetEdit();
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Edit.Visibility = Visibility.Hidden;
            if (DataPerson.right != 2) Delete.Visibility = Visibility.Hidden;
            Current.Visibility = Visibility.Visible;
            NameEdit.Visibility = Visibility.Visible;
            LastNameEdit.Visibility = Visibility.Visible;
            MiddleNameEdit.Visibility = Visibility.Visible;
            PhoneEdit.Visibility = Visibility.Visible;
            Save.Visibility = Visibility.Visible;
            Reset.Visibility = Visibility.Visible;
            name.Margin = new Thickness(560, 14, 0, 0);
            Lastname.Margin = new Thickness(560, 14, 0, 0);
            MiddleName.Margin = new Thickness(560, 14, 0, 0);
            Phone.Margin = new Thickness(560, 14, 0, 0);
        }
        private void ShowError(string error) => MessageBox.Show(error + '\n' + "Попробуйте снова.", "", MessageBoxButton.OK, MessageBoxImage.Error);
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Вы уверены, что хотите удалить свой аккаунт?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                int flag = sqlManager.Delete("DELETE FROM Tourists WHERE Num_tourist = " + DataPerson.id);
                if (flag != -2)
                {
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
                else if (flag == -2)
                {
                    PagesManager.ShowConnectionError();
                }
            }
        }
    }
}
