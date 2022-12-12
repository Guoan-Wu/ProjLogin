using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjLogin.Encrypt
{
    /// <summary>
    /// 用于帮助读取appsettings.json中的系统配置参数
    /// </summary>
    public class Appsettings
    {
        public static IConfiguration? Configuration { get; set; }

        public Appsettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 获取用appsettings.json某个字段下的值
        /// </summary>
        /// <param name="sections">获取值所在的字段（基于JSON层次结构，某个值会存在于多个层级的字段中）</param>
        /// <returns>JSON字段的值</returns>
        public static string? GetVal(params string[] sections)
        {
            if(Configuration == null)
            {
                return string.Empty;
            }
            try
            {
                if (sections.Any())
                {
                    string key = string.Join(":", sections);
                    return Configuration[key];
                }
            }
            catch (Exception) { 
            
            }

            return string.Empty;
        } // END GetVal()

        /// <summary>
        /// 获取用appsettings.json某个字段下值（值是一个组数）
        /// </summary>
        /// <param name="sections">获取值所在的字段（基于JSON层次结构，某个值会存在于多个层级的字段中）</param>
        /// <returns>JSON字段的多个值（集合）</returns>
        public static List<T> GetValues<T>(params string[] sections)
        {
            List<T> list = new ();
            
            Configuration?.Bind(string.Join(":", sections), list);
            
            return list;
        } // END GetValues()



    }
}