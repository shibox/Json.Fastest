using JShibo.Serialization;
using JShibo.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.Fastest
{
    public static class ShiboSerializer
    {
        public static string ToJson(object value)
        {
            JsonStringContext ctx = ShiboJsonStringSerializer.GetContext(value.GetType());
            JsonString sb = new JsonString();
            sb.SetInfo(ctx);
            MethodCache.GetMethodJsonString(value.GetType())(sb, value);
            return sb.ToString();
        }

        public static string ToJsonStringBuilder(object value)
        {
            StringBuilder sb = new StringBuilder();
            MethodCache.GetMethod(value.GetType())(sb, value);
            return sb.ToString();
        }

    }
}
