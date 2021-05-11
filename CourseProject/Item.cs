using System;
using System.Collections.Generic;

namespace CourseProject
{
    class Item
    {
        public static int cityNum;
        public static List<Item> items;
        public string NameTour { get; set; }
        public string NameCountry { get; set; }
        public string NameCity { get; set; }
        public string DateBegin { get; set; }
        public string DateEnd { get; set; }
        public int Price { get; set; }
        public int ActualCount {get; set; }
        public Byte[] Images { get; set; }
        public string DateBuy { get; set; }
        public static void InitList()
        {
            if (items == null) items = new List<Item>();
        }
    }
}
