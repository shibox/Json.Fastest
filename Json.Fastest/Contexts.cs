using JShibo.Serialization.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JShibo.Serialization
{
    public class HCsvStringContext
    {
        internal SerializerSettings Seting = SerializerSettings.Default;
        internal Action<CsvString, object> Serializer;
        internal List<string> Names;
        internal List<CheckAttribute> ChecksList;
        //internal List<int> NameCountsList;

        internal int HeaderSize;
        internal int MinSize;

        internal HCsvStringContext()
        {
            Names = new List<string>();
            ChecksList = new List<CheckAttribute>();
            //NameCountsList = new List<int>();

            HeaderSize = 0;
            MinSize = 32;
        }

        public int GetHeaderSize()
        {
            return Names.Sum(item => item.Length * 2 + 2);
        }
    }
}
