using System;

namespace CourseProject
{
    class DataPerson
    {
        //хравнение всех данных текущего пользователя
        public static int right { get; set; }
        public static int id { get; set; }
        public static string NamePerson { get; set; }
        public static string LastName { get; set; }
        public static string MiddleName { get; set; }
        public static int SeriaPass { get; set; }
        public static int NumberPass { get; set; }
        public static  string PhoneNumber { get; set; }
        public static void Clear()
        {
            id = -1;
            NamePerson = null;
            LastName = null;
            MiddleName = null;
            SeriaPass = 0;
            NumberPass = 0;
            PhoneNumber = null;
            right = -1;
            System.GC.Collect();
        }
        public static int NewDataPerson()
        {
            int flag = 0;
            SqlManager sqlManager = new SqlManager();
            string[] data = sqlManager.ReturnDataPerson();
            if (data != null)
            {
                NamePerson = data[0];
                LastName = data[1];
                MiddleName = data[2];
                SeriaPass = Convert.ToInt32(data[3]);
                NumberPass = Convert.ToInt32(data[4]);
                PhoneNumber = data[5];
                System.GC.Collect();
            }
            else flag = -1;
            return flag;
        }
        public static int CountTickets()
        {
            //получение количества купленных пользователем путевок
            SqlManager sqlManager = new SqlManager();
            int flag = sqlManager.ReturnNumber("SELECT COUNT(Num_trip) " +
                                               "FROM Trips WHERE Num_tourist = " + id);
            return flag;
        }
    }
}
