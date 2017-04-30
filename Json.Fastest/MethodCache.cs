using JShibo.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JShibo.Serialization;

namespace Json.Fastest
{
    public static class MethodCache
    {
        static Dictionary<Type, Action<StringBuilder, object>> mCache = null;
        static Dictionary<Type, Action<JsonString, object>> msCache = null;
        static object locker = new object();

        static MethodCache()
        {
            mCache = new Dictionary<Type, Action<StringBuilder, object>>();
            msCache = new Dictionary<Type, Action<JsonString, object>>();
            RegisterAssemblyTypes();
        }

        internal static void RegisterAssemblyTypes()
        {
            try
            {
                AssemblyName[] assembly = Assembly.GetEntryAssembly().GetReferencedAssemblies();
                foreach (AssemblyName ass in assembly)
                {
                    AppDomain.CurrentDomain.Load(ass);
                }

                Assembly[] asmbs = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly a in asmbs)
                {
                    //if (Utils.IsTypeDecoratedByAttribute<TraceAssembly>(a.GetCustomAttributes(false)))
                    //{
                    //    foreach (Type t in a.GetTypes())
                    //    {
                    //        if (Utils.IsTypeDecoratedByAttribute<SerializableAttribute>(t.GetCustomAttributes(true)))
                    //        {
                    //            //GenerateSurrogateForEvent(t);
                    //            GenerateDataSerializeSurrogate(t, true);
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Action<StringBuilder, object> CreateMethod(Type type)
        {
            string source = SerializeCodeGenerator.GenerateJsonUsingStringBuilder(type);
            Assembly ass = SerializeCodeGenerator.Create(source);
            Type obj = ass.GetType(type.FullName.Replace('.', '_') + "_JsonExtensions");
            MethodInfo method = obj.GetMethod("ToJson");
            return (Action<StringBuilder, object>)Delegate.CreateDelegate(typeof(Action<StringBuilder, object>), null, method);
        }

        public static Action<JsonString, object> CreateMethodJsonString(Type type)
        {
            string source = SerializeCodeGenerator.GenerateJsonUsingJsonString(type);
            Assembly ass = SerializeCodeGenerator.Create(source);
            Type obj = ass.GetType(type.FullName.Replace('.', '_') + "_JsonExtensions");
            MethodInfo method = obj.GetMethod("ToJson");
            return (Action<JsonString, object>)Delegate.CreateDelegate(typeof(Action<JsonString, object>), null, method);
        }

        public static void CreateMethod(Type[] types)
        {
            string source = SerializeCodeGenerator.GenerateJsonUsingStringBuilder(types);
            Assembly ass = SerializeCodeGenerator.Create(source);
            Type obj = ass.GetType("JsonExtensions");
            MethodInfo method = obj.GetMethod("ToJson");
        }

        public static Action<StringBuilder, object> GetMethod(Type type)
        {
            Action<StringBuilder, object> act = null;
            if (mCache.TryGetValue(type, out act) == false)
            {
                act = CreateMethod(type);
                mCache.Add(type, act);
            }
            return act;
        }

        public static Action<JsonString, object> GetMethodJsonString(Type type)
        {
            Action<JsonString, object> act = null;
            if (msCache.TryGetValue(type, out act) == false)
            {
                act = CreateMethodJsonString(type);
                msCache.Add(type, act);
            }
            return act;
        }

    }



}
