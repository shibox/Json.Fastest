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
            NormalClass n = new NormalClass();
            n.X = 12;
            n.Y = "78";
            n.Z = 4.6;
            n.V = 'c';

            List<NormalClass> list = new List<NormalClass>();
            list.Add(n);
            list.Add(n);
            list.Add(n);
            list.Add(n);
            

            string s = CsvSerializer.ToCsv(list);
            Console.WriteLine(s);
            
        }
    }
}
