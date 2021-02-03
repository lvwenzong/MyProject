using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace WebApplication1.Utils
{
    public class JsonObject
    {
        /// <summary>
        /// 解析JSON字串
        /// </summary>
        public static JsonObject Parse(string json)
        {
            var js = new JavaScriptSerializer();

            object obj = js.DeserializeObject(json);

            return new JsonObject()
            {
                Value = obj
            };
        }

        /// <summary>
        /// 取对象的属性
        /// </summary>
        public JsonObject this[string key]
        {
            get
            {
                var dict = this.Value as Dictionary<string, object>;
                if (dict != null && dict.ContainsKey(key))
                {
                    return new JsonObject { Value = dict[key] };
                }

                return new JsonObject();
            }
        }

        /// <summary>
        /// 取数组
        /// </summary>
        public JsonObject this[int index]
        {
            get
            {
                var array = this.Value as object[];
                if (array != null && array.Length > index)
                {
                    return new JsonObject { Value = array[index] };
                }
                return new JsonObject();
            }
        }

        /// <summary>
        /// 将值以希望类型取出
        /// </summary>
        public T GetValue<T>()
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }

        /// <summary>
        /// 取出字串类型的值
        /// </summary>
        public string Text()
        {
            return Convert.ToString(Value);
        }

        /// <summary>
        /// 取出数值
        /// </summary>
        public double Number()
        {
            return Convert.ToDouble(Value);
        }

        /// <summary>
        /// 取出整型
        /// </summary>
        public int Integer()
        {
            return Convert.ToInt32(Value);
        }

        /// <summary>
        /// 取出布尔型
        /// </summary>
        public bool Boolean()
        {
            return Convert.ToBoolean(Value);
        }

        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// 如果是数组返回数组长度
        /// </summary>
        public int Length
        {
            get
            {
                var array = this.Value as object[];
                if (array != null)
                {
                    return array.Length;
                }
                return 0;
            }
        }

        public void SetValues(string key, string value)
        {
            var dict = this.Value as Dictionary<string, object>;
            dict[key] = value;
            object obj = dict as object;
            Value = obj;
        }

        /// <summary> 
        /// 过滤特殊字符 
        /// </summary> 
        /// <param name="str">输入的字符串</param> 
        /// <returns>过滤后的特殊字符</returns> 
        public static string String2Json(string str)
        {
            string sourceStr = str;
            sourceStr = sourceStr.Replace("\\", "\\\\");
            sourceStr = sourceStr.Replace("\b", "\\b");
            sourceStr = sourceStr.Replace("\t", "\\t");
            sourceStr = sourceStr.Replace("/", "\\/");
            sourceStr = sourceStr.Replace("\n", "\\n");
            sourceStr = sourceStr.Replace("\f", "\\f");
            sourceStr = sourceStr.Replace("\r", "\\r");
            return sourceStr.Replace("\"", "\\\"");
        }

        /// <summary>
        /// json字符串 转换成数组、集合、相应对象
        /// </summary>
        /// <typeparam name="T">反射类型</typeparam>
        /// <param name="jsonText">json类型</param>
        /// <returns>返回反射类型</returns>
        public static T JSONToSource<T>(string jsonText)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                return jss.Deserialize<T>(jsonText);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 使用Newtonsoft.Json.dll 解析json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 使用Newtonsoft.Json.dll 序列化对象 json串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SerializeObject(object str)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(str);
        }
    }
}
