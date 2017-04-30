using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Json.Fastest
{
    public class SerializeCodeGenerator
    {
        public static string Generator(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("using System;");
            sb.AppendLine("using JsonFastestBenchmarks;");
            sb.AppendLine("using System.Text;");
            
            sb.AppendLine("public class JsonExtensions{");
            sb.AppendLine(string.Format( "public static string ToJson({0} value)",type.Name));
            sb.AppendLine("{");
            sb.AppendLine("StringBuilder buffer = new StringBuilder();");
            foreach (FieldInfo field in fields)
            {
                if (field.IsPrivate)
                    sb.AppendFormat(string.Format( "buffer.Append(value.{0});", field.Name));
                //sb.AppendLine("buffer.AppendLine();");
            }
            foreach (PropertyInfo property in propertys)
            {
                if (property.PropertyType.IsPrimitive)
                    sb.AppendLine(string.Format( "buffer.Append(value.{0});", property.Name));
                //sb.AppendLine("buffer.AppendLine();");
            }
            sb.AppendLine("return buffer.ToString();");
            sb.AppendLine("}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        //public static string GenerateJson(Type type)
        //{
        //    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        //    PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    StringBuilder sb = new StringBuilder();
        //    //sb.AppendLine("using System;");
        //    sb.AppendLine("using JsonFastestBenchmarks;");
        //    sb.AppendLine("using System.Text;");

        //    sb.AppendLine("public class JsonExtensions{");
        //    sb.AppendLine(string.Format("public static string ToJson({0} value)", type.Name));
        //    sb.AppendLine("{");
        //    sb.AppendLine("StringBuilder buffer = new StringBuilder();");
        //    sb.AppendLine("buffer.Append('{');");
        //    foreach (FieldInfo field in fields)
        //    {
        //        sb.AppendFormat(string.Format("buffer.Append(\"\"{0}\"\");", field.Name));
        //        if (field.IsPrivate)
        //            sb.AppendFormat(string.Format("buffer.Append(value.{0});", field.Name));
        //        //sb.AppendLine("buffer.AppendLine();");
        //    }
        //    foreach (PropertyInfo property in propertys)
        //    {
        //        sb.AppendFormat(string.Format("buffer.Append(\"\\\"{0}\\\":\");", property.Name));
        //        if (property.PropertyType.IsPrimitive)
        //            sb.AppendLine(string.Format("buffer.Append(value.{0});", property.Name));
        //        //sb.AppendLine("buffer.AppendLine();");
        //    }
        //    sb.AppendLine("buffer.Append('}');");
        //    sb.AppendLine("return buffer.ToString();");
        //    sb.AppendLine("}");
        //    sb.AppendLine("}");
        //    return sb.ToString();
        //}

        public static string GenerateJsonBuffer(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("using System;");
            sb.AppendLine("using JsonFastestBenchmarks;");
            sb.AppendLine("using System.Text;");

            sb.AppendLine("public class JsonExtensions{");
            //sb.AppendLine(string.Format("public static void ToJson(StringBuilder buffer,{0} value)", type.Name));
            sb.AppendLine(string.Format("public static void ToJson(StringBuilder buffer,object value)", type.Name));
            sb.AppendLine("{");

            sb.AppendLine("buffer.Append(\"a\");");
            sb.AppendLine("}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string GenerateJson(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("using System;");
            sb.AppendLine("using JsonFastestBenchmarks;");
            sb.AppendLine("using System.Text;");

            sb.AppendLine("public class JsonExtensions{");
            sb.AppendLine(string.Format("public static string ToJson( {0} value)", type.Name));
            sb.AppendLine("{");

            sb.AppendLine("return \"abc\";");
            sb.AppendLine("}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string GenerateJsonUsingStringBuilder(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("using {0};", type.Namespace));
            sb.AppendLine("using System.Text;");
            sb.AppendLine();
            //sb.AppendLine(string.Format( "public class {0}_JsonExtensions{",type.FullName.Replace('.','_')));
            sb.AppendLine("public class "+ type.FullName.Replace('.', '_') + "_JsonExtensions{");
            sb.AppendLine();
            sb.AppendLine(string.Format("    public static void ToJson(StringBuilder buffer,object value)", type.Name));
            sb.AppendLine("    {");
            sb.AppendLine(string.Format("        {0} v = ({0})value;", type.Name));
            sb.AppendLine("        buffer.Append('{');");
            foreach (FieldInfo field in fields)
            {
                sb.AppendFormat(string.Format("        buffer.Append(\"\"{0}\"\");", field.Name));
                //if (field.IsPrivate)
                    sb.AppendFormat(string.Format("        buffer.Append(v.{0});", field.Name));
                //sb.AppendLine("buffer.AppendLine();");
            }
            foreach (PropertyInfo property in propertys)
            {
                sb.AppendFormat(string.Format("        buffer.Append(\"\\\"{0}\\\":\");", property.Name));
                //if (property.PropertyType.IsPrimitive)
                    sb.AppendLine(string.Format("        buffer.Append(v.{0});", property.Name));
                
            }
            sb.AppendLine("        buffer.Append('}');");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string GenerateJsonUsingJsonString(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("using {0};", type.Namespace));
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using JShibo.Serialization.Json;");
            sb.AppendLine();
            //sb.AppendLine(string.Format( "public class {0}_JsonExtensions{",type.FullName.Replace('.','_')));
            sb.AppendLine("public class " + type.FullName.Replace('.', '_') + "_JsonExtensions{");
            sb.AppendLine();
            sb.AppendLine(string.Format("    public static void ToJson(JsonString buffer,object value)", type.Name));
            sb.AppendLine("    {");
            sb.AppendLine(string.Format("        {0} v = ({0})value;", type.Name));
            //sb.AppendLine("        buffer.Append('{');");
            foreach (FieldInfo field in fields)
            {
                //sb.AppendFormat(string.Format("        buffer.Write(\"\"{0}\"\");", field.Name));
                //if (field.IsPrivate)
                sb.AppendFormat(string.Format("        buffer.Write(v.{0});", field.Name));
                //sb.AppendLine("buffer.AppendLine();");
            }
            foreach (PropertyInfo property in propertys)
            {
                //sb.AppendFormat(string.Format("        buffer.Write(\"\\\"{0}\\\":\");", property.Name));
                //if (property.PropertyType.IsPrimitive)
                sb.AppendLine(string.Format("        buffer.Write(v.{0});", property.Name));

            }
            //sb.AppendLine("        buffer.Append('}');");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string GenerateJsonUsingStringBuilder(Type[] types)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Type type in types)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                
                sb.AppendLine(string.Format("using {0};", type.Namespace));
                sb.AppendLine("using System.Text;");

                sb.AppendLine("public class JsonExtensions{");
                sb.AppendLine(string.Format("public static string ToJson({0} value)", type.Name));
                sb.AppendLine(string.Format("public static void ToJson(StringBuilder buffer,object value)", type.Name));
                sb.AppendLine("{");
                sb.AppendLine(string.Format("{0} v = ({0})value)", type.Name));
                sb.AppendLine("buffer.Append('{');");
                foreach (FieldInfo field in fields)
                {
                    sb.AppendFormat(string.Format("buffer.Append(\"\"{0}\"\");", field.Name));
                    if (field.IsPrivate)
                        sb.AppendFormat(string.Format("buffer.Append(v.{0});", field.Name));
                    //sb.AppendLine("buffer.AppendLine();");
                }
                foreach (PropertyInfo property in propertys)
                {
                    sb.AppendFormat(string.Format("buffer.Append(\"\\\"{0}\\\":\");", property.Name));
                    if (property.PropertyType.IsPrimitive)
                        sb.AppendLine(string.Format("buffer.Append(v.{0});", property.Name));
                    //sb.AppendLine("buffer.AppendLine();");
                }
                sb.AppendLine("buffer.Append('}');");
                sb.AppendLine("}");
                sb.AppendLine("}");
            }
            
            return sb.ToString();
        }

        public static string GenerateCsvUsingCsvString(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("using {0};", type.Namespace));
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using JShibo.Serialization.Csv;");
            sb.AppendLine();

            sb.AppendLine("public class " + type.FullName.Replace('.', '_') + "_CsvExtensions{");
            sb.AppendLine();
            sb.AppendLine(string.Format("    public static void ToCsv(CsvString buffer,object value)", type.Name));
            sb.AppendLine("    {");
            sb.AppendLine(string.Format("        {0} v = ({0})value;", type.Name));
            
            foreach (FieldInfo field in fields)
            {
               
                sb.AppendFormat(string.Format("        buffer.Write(v.{0});", field.Name));
                
            }
            foreach (PropertyInfo property in propertys)
            {
                
                sb.AppendLine(string.Format("        buffer.Write(v.{0});", property.Name));

            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        //public static string ToJson(object value)
        //{
        //    string source = Generator(value.GetType());
        //    Assembly ass = Create(source);
        //    //object obj = ass.CreateInstance("JsonExtensions");
        //    //MethodInfo method = obj.GetType().GetMethod("ToJson");
        //    Type obj = ass.GetType("JsonExtensions");
        //    MethodInfo method = obj.GetMethod("ToJson");
        //    object result = method.Invoke(obj, new object[] { value });
        //    Console.WriteLine(result);
        //    return result.ToString();
        //}

        public static Assembly CreateCodeDll(string source)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            string path = "MyCode.dll";
            parameters.OutputAssembly = path;
            //parameters.ReferencedAssemblies.Add("System");
            parameters.ReferencedAssemblies.Add("JsonFastestBenchmarks.exe");

            //    string source = @"
            //    using System;

            //    public class MyCode
            //    {
            //         public static int Test(int x, int y)
            //         {
            //             return x+y;
            //         }
            //     }
            //";
            CompilerResults cr = provider.CompileAssemblyFromSource(parameters, source);

            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building " + source + " into " + cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine(ce.ToString());
                }
            }
            else
            {
                Console.WriteLine("编译成功");
            }

            Assembly ass = cr.CompiledAssembly;
            return ass;

            //object obj = ass.CreateInstance("MyCode");
            //MethodInfo method = obj.GetType().GetMethod("Test");
            //object result = method.Invoke(obj, new object[] { 1, 5 });
            //Console.WriteLine(result);
        }

        public static Assembly Create(string source)
        {
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

            // 2.Sets the runtime compiling parameters by crating a new CompilerParameters instance  
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add("Json.Fastest.dll");
            objCompilerParameters.ReferencedAssemblies.Add("JsonFastestBenchmarks.exe");
            //objCompilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            objCompilerParameters.GenerateInMemory = false;
            string path = "MyCode.dll";
            objCompilerParameters.OutputAssembly = path;
            // 3.CompilerResults: Complile the code snippet by calling a method from the provider  
            CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromSource(objCompilerParameters, source);

            if (cr.Errors.HasErrors)
            {
                Console.WriteLine("Errors building " + source + " into " + cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine(ce.ToString());
                }
            }

            // 4. Invoke the method by using Reflection  
            Assembly objAssembly = cr.CompiledAssembly;
            return objAssembly;
        }

        public static void CreateCodeDll1()
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = false;
            string path = "MyCode.dll";
            parameters.OutputAssembly = path;
            //parameters.ReferencedAssemblies.Add("System");

            string sourceFile = @"
            

            public class MyCode
            {
                 public static int Test(int x, int y)
                 {
                     return x+y;
                 }
             }
        ";
            CompilerResults cr = provider.CompileAssemblyFromSource(parameters, sourceFile);

            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building " + sourceFile + " into " + cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine(ce.ToString());
                }
            }
            else
            {
                Console.WriteLine("编译成功");
            }

            Assembly ass = cr.CompiledAssembly;
            object obj = ass.CreateInstance("MyCode");
            MethodInfo method = obj.GetType().GetMethod("Test");
            object result = method.Invoke(obj, new object[] { 1, 5 });
            Console.WriteLine(result);
        }

    }



}
