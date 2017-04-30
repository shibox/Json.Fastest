using Json.Fastest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JShibo.Serialization.Csv
{
    public static class CsvSerializer
    {
        static Dictionary<Type, Action<CsvString, object>> msCache = null;

        static CsvSerializer()
        {
            msCache = new Dictionary<Type, Action<CsvString, object>>();
        }

        public static Action<CsvString, object> CreateMethodJsonString(Type type)
        {
            string source = SerializeCodeGenerator.GenerateCsvUsingCsvString(type);
            Assembly ass = SerializeCodeGenerator.Create(source);
            Type obj = ass.GetType(type.FullName.Replace('.', '_') + "_CsvExtensions");
            MethodInfo method = obj.GetMethod("ToCsv");
            return (Action<CsvString, object>)Delegate.CreateDelegate(typeof(Action<CsvString, object>), null, method);
        }

        public static Action<CsvString, object> GetMethodJsonString(Type type)
        {
            Action<CsvString, object> act = null;
            if (msCache.TryGetValue(type, out act) == false)
            {
                act = CreateMethodJsonString(type);
                msCache.Add(type, act);
            }
            return act;
        }

        public static string ToCsv(object value)
        {
            if (value.GetType().IsArray)
            {
                return null;
            }
            else
            {
                Type[] gtypes = value.GetType().GenericTypeArguments;
                if (gtypes.Length == 1)
                {
                    Type type = gtypes[0];
                    CsvStringContext info = ShiboCsvStringSerializer.GetLastContext(type);
                    Action<CsvString,object> act = GetMethodJsonString(type);
                    CsvString stream = null;
                    IList list = value as IList;
                    if (list != null)
                    {
                        int size = info.MinSize * list.Count + info.GetHeaderSize();
                        char[] buffer = CharsBufferManager.GetBuffer(size);
                        stream = new CsvString(buffer);
                        stream.WriteHeader(info.Names);
                        for (int i = 0; i < list.Count; i++)
                        {
                            act(stream, list[i]);
                            stream.WriteNewLine();
                        }
                        CharsBufferManager.SetBuffer(stream.GetBuffer());
                    }
                    return stream.ToString();
                }
            }

            return null;
        }
    }
}
