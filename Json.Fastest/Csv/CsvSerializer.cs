using JShibo.Serialization.Common;
using Json.Fastest;
using Json.Fastest.Csv;
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
        #region 字段

        static readonly string Extensions = "_CsvExtensions";
        static readonly string Method = "ToCsv";
        static Dictionary<Type, Action<CsvString, object>> msCache = null;
        static Dictionary<Type, HCsvStringContext> mCache = null;
        static IIBuilder builder = new CsvBuilder();

        #endregion

        #region 方法

        static CsvSerializer()
        {
            msCache = new Dictionary<Type, Action<CsvString, object>>();
            mCache = new Dictionary<Type, HCsvStringContext>();
        }

        public static Action<CsvString, object> CreateMethod(Type type)
        {
            string source = SerializeCodeGenerator.GenerateCsvUsingCsvString(type);
            Assembly ass = SerializeCodeGenerator.Create(source);
            Type obj = ass.GetType(type.FullName.Replace('.', '_') + Extensions);
            MethodInfo method = obj.GetMethod(Method);
            return (Action<CsvString, object>)Delegate.CreateDelegate(typeof(Action<CsvString, object>), null, method);
        }

        public static Action<CsvString, object> GetMethod(Type type)
        {
            Action<CsvString, object> act = null;
            if (msCache.TryGetValue(type, out act) == false)
            {
                act = CreateMethod(type);
                msCache.Add(type, act);
            }
            return act;
        }

        public static HCsvStringContext CreateContext(Type type)
        {
            HCsvStringContext info = new HCsvStringContext();
            info.Serializer = CreateMethod(type);
            FieldInfo[] fields = type.GetFields(info.Seting.Flags);
            foreach (FieldInfo field in fields)
            {
                if (Utils.IsIgnoreAttribute(field) == false)
                {
                    string name = Utils.GetAttributeName(field);
                    info.Names.Add(name);
                    info.MinSize += name.Length + 3;
                    info.ChecksList.Add(Utils.GetCheckAttribute(field));

                    //if (Utils.IsDeep(field.FieldType))
                    //{
                    //    info.SerializeList.Add(Instance.GenerateDataSerializeSurrogate(field.FieldType));
                    //    info.SizeSerializeList.Add(Instance.GenerateSizeSerializeSurrogate(field.FieldType));
                    //    info.DeserializeList.Add(Instance.GetDeserializeSurrogate(field.FieldType));
                    //    info.TypeCountsList.Add(Instance.GetDeserializeTypes(field.FieldType).Length);
                    //    info.TypesList.Add(field.FieldType);
                    //    info.NameCountsList.Add(Instance.GetSerializeNames(field.FieldType).Length);
                    //    CreateContext(field.FieldType, info);
                    //}
                    //else
                        info.MinSize += builder.GetSize(field.FieldType);
                }
            }
            PropertyInfo[] propertys = type.GetProperties(info.Seting.Flags);
            foreach (PropertyInfo property in propertys)
            {
                if (Utils.IsIgnoreAttribute(property) == false)
                {
                    string name = Utils.GetAttributeName(property);
                    info.Names.Add(name);
                    info.MinSize += name.Length + 3;
                    info.ChecksList.Add(Utils.GetCheckAttribute(property));

                    //if (Utils.IsDeep(property.PropertyType))
                    //{
                    //    info.SerializeList.Add(Instance.GenerateDataSerializeSurrogate(property.PropertyType));
                    //    info.SizeSerializeList.Add(Instance.GenerateSizeSerializeSurrogate(property.PropertyType));
                    //    info.DeserializeList.Add(Instance.GetDeserializeSurrogate(property.PropertyType));
                    //    info.TypeCountsList.Add(Instance.GetDeserializeTypes(property.PropertyType).Length);
                    //    info.TypesList.Add(property.PropertyType);
                    //    info.NameCountsList.Add(Instance.GetSerializeNames(property.PropertyType).Length);
                    //    CreateContext(property.PropertyType, info);
                    //}
                    //else
                        info.MinSize += builder.GetSize(property.PropertyType);
                }
            }
            return info;
        }

        public static HCsvStringContext GetContext(Type type)
        {
            HCsvStringContext ctx = null;
            if (mCache.TryGetValue(type, out ctx) == false)
            {
                ctx = CreateContext(type);
                mCache.Add(type, ctx);
            }
            return ctx;
        }

        private static CsvString Wrap(CsvString writer,int count, HCsvStringContext info)
        {
            int size = info.MinSize * count + info.GetHeaderSize();
            char[] buffer = CharsBufferManager.GetBuffer(size);
            writer = new CsvString(buffer);
            writer.WriteHeader(info.Names);
            return writer;
        }

        #endregion



        public static string ToCsv(object value)
        {
            if (value == null)
                return string.Empty;
            Type type = value.GetType();
            CsvString writer = null;
            HCsvStringContext info = null;
            if (type.IsArray)
            {
                Array ar = (Array)value;
                if (ar.Length == 0)
                    return null;
                Type cType = null;
                for (int i = 0; i < ar.Length; i++)
                {
                    if (ar.GetValue(i) != null)
                    {
                        cType = ar.GetValue(i).GetType();
                        info = GetContext(cType);
                        break;
                    }
                }
                writer = Wrap(writer, ar.Length, info);
                if (type.FullName == "System.Object[]")
                {
                    for (int i = 0; i < ar.Length; i++)
                    {
                        object item = ar.GetValue(i);
                        if (item != null)
                        {
                            if (cType == item.GetType())
                                info.Serializer(writer, item);
                            else
                                writer.WriteNullLine(info.Names.Count);
                        }
                        else
                            writer.WriteNullLine(info.Names.Count);
                        writer.WriteNewLine();
                    }
                }
                else
                {
                    for (int i = 0; i < ar.Length; i++)
                    {
                        object item = ar.GetValue(i);
                        if (item != null)
                            info.Serializer(writer, item);
                        else
                            writer.WriteNullLine(info.Names.Count);
                        writer.WriteNewLine();
                    }
                }
            }
            else if (type.IsGenericType)
            {
                Type[] types = type.GenericTypeArguments;
                if (types.Length == 1)
                {
                    info = GetContext(types[0]);
                    IList list = value as IList;
                    if (list != null)
                    {
                        writer = Wrap(writer, list.Count, info);
                        for (int i = 0; i < list.Count; i++)
                        {
                            object item = list[i];
                            if (item != null)
                                info.Serializer(writer, item);
                            else
                                writer.WriteNullLine(info.Names.Count);
                            writer.WriteNewLine();
                        }
                    }
                }
                else
                    throw new NotSupportedException("not support multi generic type!");
            }
            else
            {
                //直接处理枚举模式
                //bool isFirst = true;
                //foreach (object item in ar)
                //{
                //    if (isFirst)
                //    {
                //        info = GetContext(item.GetType());
                //        int size = info.MinSize * ar.Length + info.GetHeaderSize();
                //        char[] buffer = CharsBufferManager.GetBuffer(size);
                //        writer = new CsvString(buffer);
                //        writer.WriteHeader(info.Names);
                //        isFirst = false;
                //    }
                //    info.Serializer(writer, item);
                //    writer.WriteNewLine();
                //}
            }
            CharsBufferManager.SetBuffer(writer.GetBuffer());
            return writer.ToString();
        }
    }
}
