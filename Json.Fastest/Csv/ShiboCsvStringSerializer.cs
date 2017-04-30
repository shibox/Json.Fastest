using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Reflection;
using System.Text;
using JShibo.Serialization.Common;
using JShibo.Serialization;

namespace JShibo.Serialization.Csv
{
    internal class ShiboCsvStringSerializer : SerializerBase<CsvString, CsvUstring, CsvStringContext, CsvStringSize>
    {

        static ShiboCsvStringSerializer Instance;

        #region 构造函数

        static ShiboCsvStringSerializer()
        {
            Instance = new ShiboCsvStringSerializer();
            Instance.builder = new CsvILBuilder();
            Instance.RegisterAssemblyTypes();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 必须是数组或集合类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        internal static void CreateContext(Type type, CsvStringContext info)
        {

            FieldInfo[] fields = type.GetFields(info.Seting.Flags);
            foreach (FieldInfo field in fields)
            {
                if (Utils.IsIgnoreAttribute(field) == false)
                {
                    string name = Utils.GetAttributeName(field);
                    info.NamesList.Add(name);
                    //info.MinSize += name.Length + 3;
                    if (Utils.IsDeep(field.FieldType))
                    {
                        info.IsAllFixedSize = false;
                    }
                    else
                        info.MinSize += Instance.builder.GetSize(field.FieldType);
                }
            }
            PropertyInfo[] propertys = type.GetProperties(info.Seting.Flags);
            foreach (PropertyInfo property in propertys)
            {
                if (Utils.IsIgnoreAttribute(property) == false)
                {
                    string name = Utils.GetAttributeName(property);
                    info.NamesList.Add(name);
                    //info.MinSize += name.Length + 3;
                    if (Utils.IsDeep(property.PropertyType))
                    {
                        info.IsAllFixedSize = false;
                    }
                    else
                        info.MinSize += Instance.builder.GetSize(property.PropertyType);
                }
            }
        }

        internal static CsvStringContext GetContext(Type type)
        {
            CsvStringContext info = null;
            if (types.TryGetValue(type, out info) == false)
            {
                info = new CsvStringContext();
                CreateContext(type, info);
                info.Serializer = Instance.GenerateDataSerializeSurrogate(type);
                info.SizeSerializer = Instance.GenerateSizeSerializeSurrogate(type);
                //info.Deserializer = Instance.GetDeserializeSurrogate(type);
                types.Add(type, info);
                if (info != null)
                    info.ToArray();
            }
            return info; 
        }

        internal static CsvStringContext GetSizeInfos(Type type)
        {
            CsvStringContext info = null;
            if (types.TryGetValue(type, out info) == false)
            {
                info = new CsvStringContext();
                CreateContext(type, info);
                info.SizeSerializer = Instance.GenerateSizeSerializeSurrogate(type);
                //info.SizeSerialize = GenerateSizeSerializeSurrogate(type);
                //info.Deserialize = GetJsonDeserializeSurrogateFromType(type);
                types.Add(type, info);
                if (info != null)
                    info.ToArray();
            }
            return info;
        }

        #endregion

        #region 公共的

        internal static CsvStringContext GetLastContext(Type type)
        {
            CsvStringContext info = null;
            if (type == Instance.lastSerType)
            {
                info = Instance.lastSerTypeInfo;
            }
            else
            {
                info = GetContext(type);
                Instance.lastSerType = type;
                Instance.lastSerTypeInfo = info;
            }
            return info;
        }

        internal static string Serialize(object graph)
        {
            if (graph.GetType().IsArray)
            {
                Array ar = (Array)graph;
                if (ar.Length == 0)
                    return null;
                bool isFirst = true;
                CsvStringContext info = null;
                CsvString stream = null;
                foreach (object item in ar)
                {
                    if (isFirst)
                    {
                        info = GetLastContext(item.GetType());
                        int size = info.MinSize * ar.Length + info.GetHeaderSize();
                        char[] buffer = CharsBufferManager.GetBuffer(size);
                        stream = new CsvString(buffer);
                        stream.WriteHeader(info.Names);
                        isFirst = false;
                    }
                    info.Serializer(stream, item);
                    stream.WriteNewLine();
                }
                CharsBufferManager.SetBuffer(stream.GetBuffer());
                return stream.ToString();
            }
            else
            {
                Type[] gtypes = graph.GetType().GenericTypeArguments;
                if (gtypes.Length == 1)
                {
                    Type type = gtypes[0];
                    CsvStringContext info = GetLastContext(type);
                    CsvString stream = null;
                    IList list = graph as IList;
                    if (list != null)
                    {
                        int size = info.MinSize * list.Count + info.GetHeaderSize();
                        char[] buffer = CharsBufferManager.GetBuffer(size);
                        stream = new CsvString(buffer);
                        stream.WriteHeader(info.Names);
                        for (int i = 0; i < list.Count; i++)
                        {
                            info.Serializer(stream, list[i]);
                            stream.WriteNewLine();
                        }
                        CharsBufferManager.SetBuffer(stream.GetBuffer());
                    }
                    return stream.ToString();
                }
                //IEnumerable enumerable =  graph as IEnumerable;
                //foreach (object item in enumerable)
                //{
                //    //stream.Write(item);
                //    //Serialize(stream, graph, info);
                //}
            }

            return null;
        }

        //internal static void Serialize(CsvString stream, object graph)
        //{
        //    Type type = graph.GetType();
        //    CsvStringContext info = GetLastContext(type);
        //    Serialize(stream, graph, info);
        //}

        internal static void Serialize(CsvString stream, object graph, CsvStringContext info)
        {
            if (info.IsBaseType == false)
            {
                stream.SetInfo(info);
                stream.WriteHeader(info.Names);
                info.Serializer(stream, graph);
                //if (stream._buffer[stream.position - 1] == ',')
                //    stream.position--;
                //if (info.IsBaseType == false)
                //{
                //    stream._buffer[stream.position++] = '\r';
                //    stream._buffer[stream.position++] = '\n';
                //}
            }
            else
            {
                //stream.isJsonBaseType = true;
                info.Serializer(stream, graph);
                //stream.position -= 2;
            }
        }

        //internal static void Serialize(CsvString stream, object graph, Serialize<CsvString> ser)
        //{
        //    stream._buffer[stream.position++] = '{';
        //    ser(stream, graph);
        //    if (stream._buffer[stream.position - 1] == ',')
        //        stream.position--;
        //    stream._buffer[stream.position++] = '}';
        //}

        internal static CsvString SerializeToBuffer(object graph)
        {
            Type type = graph.GetType();
            CsvStringContext info = GetLastContext(type);
            CsvStringSize size = new CsvStringSize();
            info.SizeSerializer(size, graph);
            int totalSize = info.MinSize + size.Size;
            CsvString result = null;
            char[] buffer = null;
            if (totalSize > 400)
            {
                buffer = CharsBufferManager.GetBuffer(totalSize);
                result = new CsvString(buffer);
                Serialize(result, graph, info);
                CharsBufferManager.SetBuffer(result.GetBuffer());
            }
            else
            {
                buffer = new char[totalSize];
                result = new CsvString(buffer);
                Serialize(result, graph, info);
            }
            return result;
        }

        internal static T Deserialize<T>(CsvString stream, CsvStringContext info)
        {
            //if (info.IsJsonBaseType == false)
            //    stream.position++;

            //stream.desers = info.DeserializeStreams;
            //stream.types = info.Types;
            //stream.typeCounts = info.TypeCounts;
            //stream.nameCounts = info.NameCounts;
            //stream.names = info.Names;

            //object value = info.Deserialize(stream);
            //return (T)value;

            return default(T);
        }

        internal static T Deserialize<T>(CsvString stream)
        {
            Type type = typeof(T);
            CsvStringContext info = null;
            if (type == Instance.lastSerType)
            {
                info = Instance.lastSerTypeInfo;
            }
            else
            {
                info = GetContext(type);
                Instance.lastSerType = type;
                Instance.lastSerTypeInfo = info;
            }
            return Deserialize<T>(stream, info);

            //return default(T);
            //throw new Exception("Not supported,it will implementation in next versions");
        }

        #endregion
    }
}
