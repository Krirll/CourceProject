using System;
using System.Windows;

namespace CourseProject
{
    public partial class WindowRegistration : Window
    {
        int flag;
        public WindowRegistration()
        {
            InitializeComponent();
        }
        private void Registr_Click(object sender, RoutedEventArgs e)
        {
            //проверка правильности ввода данных
            if (name.Text.Length > 0 && LastName.Text.Length > 0
                && SeriaPass.Text.Length > 0 && NumberPass.Text.Length > 0 
                && Phone.Text.Length > 0 && Login.Text.Length > 0 
                && Password1.Password.Length > 0  && Password2.Password.Length > 0 && DateBirth.Text != "")
            {
                //проверка имени
                if (DataManager.CheckInputString(@"^(?:(?![ЫЫЙЪЬЁЦЩ])[А-Я])[а-я]{1,30}$", name.Text))
                {
                    //проверка фамилии
                    if (DataManager.CheckInputString(@"^(?:(?!([ЬЪЫ]))[А-Я])[а-я]{0,30}$", LastName.Text))
                    {
                        if (MiddleName.Text.Length > 0)
                        {
                            //проверка отчества
                            if (DataManager.CheckInputString(@"^(?:(?![ЫЫЙЪЬЁЦЩ])[А-Я])[а-я]{1,30}$", MiddleName.Text) == false) 
                            {
                                ShowError("Отчество должно содержать не более 40, не менее 4 букв русского алфавита" +
                                    '\n' + "и начинаться с заглавной буквы русского алфавита.\n Другие символы запрещены!");
                                MiddleName.Text = "";
                            }
                        }
                        DateTime dt = DateTime.Today;
                        if (DateBirth.DisplayDate.Year < dt.Year && 
                            (dt.Year - DateBirth.DisplayDate.Year >= 18) && (dt.Year - DateBirth.DisplayDate.Year <= 120))
                        {
                            //проверка серии паспорта
                            if (DataManager.CheckInputString(@"^(?=.{4}$)\d*$", SeriaPass.Text))
                            {
                                //проверка номера паспорта
                                if (DataManager.CheckInputString(@"^(?=.{6}$)\d*$", NumberPass.Text))
                                {
                                    //проверка номера телефона
                                    if (DataManager.CheckInputString(@"^\+7\d{10}$", Phone.Text))
                                    {
                                        //проверка логина
                                        if (DataManager.CheckInputString(@"^(?=.{3,10}$)(?=[^A-Z]*[A-Z])(?=[^0-9]*[0-9])(?=[^a-z]*[a-z])[A-Za-z0-9]*$", Login.Text))
                                        {
                                            //проверка пароля
                                            if (DataManager.CheckInputString(@"^(?=.{5,20}$)(?=[^0-9]*[0-9])(?=[^A-Z]*[A-Z])(?=[^!#$%^&*]*[!#$%^&*])[a-zA-Z0-9!#$%^&*]*$", Password1.Password))
                                            {
                                                if (Password1.Password == Password2.Password) //проверка на повторное введение пароля
                                                {
                                                    SqlManager connection = new SqlManager();
                                                    flag = connection.Select("SELECT Passport_number,Passport_series FROM Tourists WHERE Passport_number = " + NumberPass.Text +
                                                                                " AND Passport_series = " + SeriaPass.Text, 2);
                                                    if (flag == -1) //возвращать строку для провверки введенного логина и существующих
                                                    {
                                                        flag = connection.Select("SELECT login FROM Accounts WHERE login = '" + DataManager.MakeHash(Login.Text) + "'", 1);
                                                        if (flag == -1)
                                                        {
                                                            //добавление пользовательских данных в БД
                                                            if (DataPerson.right == 2)
                                                            {
                                                                InsertNewUserWithRight("Workers", "Num_worker", 2, connection);
                                                            }
                                                            else if (DataPerson.right == 3)
                                                            {
                                                                InsertNewUserWithRight("Tourists", "Num_tourist", 3, connection);
                                                            }
                                                            else if (DataPerson.right == 1)
                                                            {
                                                                InsertNewUserWithRight("Admins", "Num_admin", 1, connection);   
                                                            }
                                                            if (flag != -2)
                                                            {
                                                                MessageBox.Show("Регистрация прошла успешно.\nПройдите авторизацию для дальнейшего использования.");
                                                                MainWindow mw = new MainWindow();
                                                                mw.Show();
                                                                Close();
                                                                System.GC.Collect();
                                                            }
                                                        }
                                                        else if (flag == -2)
                                                        {
                                                            ShowConnectionError();
                                                        }
                                                        else
                                                        {
                                                            ShowError("Такой пользователь уже существует.");
                                                            Login.Text = "";
                                                            Password1.Password = "";
                                                            Password2.Password = "";
                                                        }
                                                    }
                                                    else if (flag == -2)
                                                    {
                                                        ShowConnectionError();
                                                    }
                                                    else
                                                    {
                                                        ShowError("Такой пользователь уже существует.");
                                                        SeriaPass.Text = "";
                                                        NumberPass.Text = "";
                                                    }
                                                }
                                                else
                                                {
                                                    ShowError("Вы неправильно повторили пароль.");
                                                    Password1.Password = "";
                                                    Password2.Password = "";
                                                }
                                            }
                                            else
                                            {
                                                ShowError("Пароль введен неверно." + '\n' + "Пароль должен как минимум состоять из 1 заглавной, 2 строчных английских букв," + '\n' +
                                                     "1 символа из #, $, %, ^, &, *, ! " +
                                                     "и 1 цифры.\nДлина пароля не может привышать 5 и не может быть больше 20 символов.\n Другие символы запрещены!");
                                                Password1.Password = "";
                                                Password2.Password = "";
                                            }
                                        }
                                        else
                                        {
                                            ShowError("Логин введен неверно." + '\n' + "Логин должен состоять только из английских букв, цифр,"
                                                        + '\n' + "быть не более 10, не менее 4 символов\n и должно содержать хотя бы 1 заглавную, 1 строчную букву и 1 цифру.\n Другие символы запрещены!");
                                            Login.Text = "";
                                            Password1.Password = "";
                                            Password2.Password = "";
                                        }
                                    }
                                    else
                                    {
                                        ShowError("Номер телефона введен неверно." + '\n' + "Номер телефона должен начинаться с +7"
                                            + '\n' + "и после этого должны быть 10 цифр.\n Другие символы запрещены!");
                                        Phone.Text = "";
                                    }
                                }
                                else
                                {
                                    ShowError("Номер паспорта введен неверно." + '\n' + "Номер паспорта должен состоять из 6 цифр.\n Другие символы запрещены!");
                                    NumberPass.Text = "";
                                }
                            }
                            else
                            {
                                ShowError("Серия паспорта введена неверно." + '\n' + "Серия паспорта должна состоять из 4 цифр.\n Другие символы запрещены!");
                                SeriaPass.Text = "";
                            }
                        }
                        else
                        {
                            ShowError("Приложение предназначено для совершеннолетних пользователей.\nДата рождения не может быть больше текущего года и вам не может быть больше 120 лет.");
                            DateBirth.Text = "";
                        }
                    }
                    else
                    {
                        ShowError("Фамилия введена неверно." + '\n' + "Фамилия должна содержать не более 30, не менее 2 букв русского алфавита" +
                            '\n' + "и начинаться с заглавной буквы русского алфавита.\n Другие символы запрещены!");
                        LastName.Text = "";
                    }
                }
                else
                {
                    ShowError("Имя введено неверно." + '\n' + "Имя должно содержать не более 30, не менее 2 букв русского алфавита" +
                        '\n' + "и начинаться с заглавной буквы русского алфавита.\n Другие символы запрещены!");
                    name.Text = "";
                }
                                    
            }
            else ShowError("Пустые поля не допускаются (кроме Отчества).");
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            Close();
            System.GC.Collect();
        }
        private void ShowError(string error) => MessageBox.Show(error + '\n' + "Попробуйте снова.");
        private void ShowConnectionError()
        {
            MessageBox.Show("Отсутствует соединение с сервером,\nповторите попытку позже.");
            foreach (Window window in Application.Current.Windows)
            {
                if (window is WindowRegistration)
                {
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    Close();
                }
            }
        }
        private void InsertNewUserWithRight(string table, string num, int right, SqlManager connection)
        {
            flag = connection.Insert($"INSERT INTO {table}" +
                                     "(Name, Last_name, Middle_name, Passport_series, Passport_number, Phone_number, DateBirth)" +
                                     "VALUES ('" + name.Text + "','" + LastName.Text + "','" + MiddleName.Text + "'," + SeriaPass.Text +
                                     "," + NumberPass.Text + ",'" + Phone.Text + "','" + DateBirth.Text + "')");
            if (flag != -2)
            {
                flag = connection.ReturnNumber($"SELECT {num} FROM {table} WHERE Passport_series = " + SeriaPass.Text + "AND Passport_number = " + NumberPass.Text);
                if (flag != -2)
                {
                    int buf = flag;
                    flag = connection.Insert($"INSERT INTO Accounts(login,password,access_right, {num})" +
                                        " VALUES ('" + DataManager.MakeHash(Login.Text) + "','" + DataManager.MakeHash(Password1.Password) + $"', {right}," + buf + ")");
                    if (flag == -2)
                    {
                        ShowConnectionError();
                    }
                }
                else if (flag == -2)
                {
                    ShowConnectionError();
                }
            }
            else if (flag == -2)
            {
                ShowConnectionError();
            }
        }
    }
}
