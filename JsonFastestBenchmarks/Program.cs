using Json.Fastest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonFastestBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //string s = SerializeCodeGenerator.Generator(typeof(NormalClass));
            //File.WriteAllText("code.txt", s);
            //Console.WriteLine(s);
            //ToJsonTests.Run();
            ToCsvTests.Run();

            Console.ReadLine();
            //JsonConvert.SerializeObject()
        }
    }
}
