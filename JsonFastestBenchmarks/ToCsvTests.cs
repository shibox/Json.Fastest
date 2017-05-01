using JShibo.Serialization;
using JShibo.Serialization.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonFastestBenchmarks
{
    public class ToCsvTests
    {
        public static void Run()
        {
            Array();
            //List();
            
        }

        public static void Array()
        {
            NormalClass item = new NormalClass();
            item.X = 12;
            item.Y = "78";
            item.Z = 4.6;
            item.V = 'c';
            object[] value = new object[6];
            for (int i = 0; i < value.Length; i++)
                value[i] = item;

            string s = CsvSerializer.ToCsv(value);
            Console.WriteLine(s);
        }

        public static void List()
        {
            NormalClass item = new NormalClass();
            item.X = 12;
            item.Y = "78";
            item.Z = 4.6;
            item.V = 'c';

            List<NormalClass> value = new List<NormalClass>();
            value.Add(item);
            value.Add(item);
            value.Add(item);
            value.Add(item);

            string s = CsvSerializer.ToCsv(value);
            Console.WriteLine(s);
        }

    }
}
