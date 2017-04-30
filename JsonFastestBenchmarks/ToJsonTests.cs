using Json.Fastest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonFastestBenchmarks
{
    public class ToJsonTests
    {
        public static void Run()
        {
            NormalClass n = new NormalClass();
            n.X = 12;
            n.Y = "78";
            n.Z = 4.6;
            n.V = 'c';
            //string s = SerializeCodeGenerator.ToJson(n);
            //string s = ToJson(n);
            //string s = ToJsonFunc(n);
            string s = ShiboSerializer.ToJson(n);
            Console.WriteLine(s);
            //SerializeCodeGenerator.CreateCodeDll1();
        }

        public static string ToJson(object value)
        {
            //string source = SerializeCodeGenerator.Generator(value.GetType());
            string source = SerializeCodeGenerator.GenerateJson(value.GetType());
            Assembly ass = SerializeCodeGenerator.Create(source);
            //object obj = ass.CreateInstance("JsonExtensions");
            //MethodInfo method = obj.GetType().GetMethod("ToJson");
            Type obj = ass.GetType("JsonExtensions");
            MethodInfo method = obj.GetMethod("ToJson");

            object result = method.Invoke(obj, new object[] { value });
            Stopwatch w = Stopwatch.StartNew();
            for(int i = 0;i < 1000000;i++)
                result= method.Invoke(obj, new object[] { value });
            w.Stop();
            Console.WriteLine("cost:"+w.ElapsedMilliseconds);
            //Func<object, string> setter = (Func<object, string>)Delegate.CreateDelegate(typeof(Func<object, string>), value, method);
            //string result = setter(value);

            //Console.WriteLine(result);
            return result.ToString();
        }

        public static string ToJsonBuffer(object value)
        {
            string source = SerializeCodeGenerator.GenerateJsonBuffer(value.GetType());
            Assembly ass = SerializeCodeGenerator.Create(source);
            Type obj = ass.GetType("JsonExtensions");
            MethodInfo method = obj.GetMethod("ToJson");
            StringBuilder sb = new StringBuilder();
            string s = "";
            method.Invoke(obj, new object[] { sb, value });
            Stopwatch w = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
                //method.Invoke(obj, new object[] { sb, value });
                s = int.MaxValue.ToString();
            w.Stop();
            Console.WriteLine("cost:" + w.ElapsedMilliseconds);
            return s;
            return sb.ToString();


            //string source = SerializeCodeGenerator.GenerateJsonBuffer(value.GetType());
            //Assembly ass = SerializeCodeGenerator.Create(source);
            //Type obj = ass.GetType("JsonExtensions");
            //MethodInfo method = obj.GetMethod("ToJson");
            //StringBuilder sb = new StringBuilder();
            //SetValueDelegate ss = DynamicMethodFactory.CreateCallJson(method, value.GetType());
            //ss(sb, value);
            //Stopwatch w = Stopwatch.StartNew();
            //for (int i = 0; i < 1000000; i++)
            //    ss(sb, value);
            //w.Stop();
            //Console.WriteLine("cost:" + w.ElapsedMilliseconds);

            //return sb.ToString();

        }

        public static string ToJsonFunc(object value)
        {
            string source = SerializeCodeGenerator.GenerateJsonBuffer(value.GetType());
            Assembly ass = SerializeCodeGenerator.Create(source);
            Type obj = ass.GetType("JsonExtensions");
            MethodInfo method = obj.GetMethod("ToJson");
            //MethodInfo method = typeof(JsonExtensions).GetMethod("ToJson");
            StringBuilder sb = new StringBuilder();
            //object result = method.Invoke(obj, new object[] { value });
            //Action<StringBuilder, NormalClass> setter = (Action<StringBuilder, NormalClass>)Delegate.CreateDelegate(typeof(Action<StringBuilder, NormalClass>), null, method);
            //setter(sb, (NormalClass)value);
            //Action<StringBuilder, object> setter = (Action<StringBuilder, object>)Delegate.CreateDelegate(typeof(Action<StringBuilder, object>), null, method);
            //setter(sb, value);
            Action<StringBuilder, object> setter = (Action<StringBuilder, object>)Delegate.CreateDelegate(typeof(Action<StringBuilder, object>), null, method);
            setter(sb, value);

            Stopwatch w = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                sb.Length = 0;
                setter(sb, value);
            }
            w.Stop();
            Console.WriteLine("cost:" + w.ElapsedMilliseconds);
            //Action<StringBuilder, object> setter = (Action<StringBuilder, object>)Delegate.CreateDelegate(typeof(Action<StringBuilder, object>), null, method);
            //setter(sb, value);


            //Console.WriteLine(result);
            return sb.ToString();
        }

        //public static void In()
        //{
        //    OrderInfo testObj = new OrderInfo();
        //    PropertyInfo propInfo = typeof(OrderInfo).GetProperty("OrderID");
        //    SetValueDelegate setter2 = DynamicMethodFactory.CreatePropertySetter(propInfo);
        //    Console.Write("EmitSet花费时间：        ");
        //    Stopwatch watch2 = Stopwatch.StartNew();

        //    for (int i = 0; i < count; i++)
        //        setter2(testObj, 123);

        //    watch2.Stop();
        //    Console.WriteLine(watch2.Elapsed.ToString());
        //}

    }

    //public class JsonExtensions
    //{
    //    public static void ToJson(StringBuilder buffer, NormalClass value)
    //    {
    //        buffer.Append("abc");
    //    }
    //}

}
