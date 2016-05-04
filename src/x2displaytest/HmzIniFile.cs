/*************************************************
 * 版权(C)：Ceway Tech.Co.,Ltd
 * 作者：Jackie
 * 版本：v1.0
 * 时间：2013/10/19
 * 模块描述：实现C#版本的INI格式文件类型的操作
 * 
 * 函数列表：
 *         1. Create()          -- 创建INI文件
 *         2. WriteString()     -- 写入字符串值
 *         3. WriteDouble()     -- 写入浮点型数值
 *         4. WriteSection()    -- 写入整段的数据信息
 *         5. WriteSpaceLine()  -- 写入空行
 *         6. ReadString()      -- 读取字符串值
 *         7. ReadDouble()      -- 读取浮点型数值
 *         8. ReadInt()         -- 读取整数型数值
 *         9. ReadSection()     -- 读取整段的数据信息
 *         10. DeleteKey()      -- 删除指定的键值对
 *         11. DeleteSection()  -- 删除整段的键值对
 *         
 * 历史记录：
 *      作者      时间      版本      描述
 *     Jackie  2013/10/19   v1.0    建立该类
 * ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Hnwlxy.HmzSysPlatform
{
    /// <summary>
    /// C#版的INI文件操作类
    /// </summary>
    public class HmzIniFile
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">INI文件路径</param>
        public HmzIniFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("文件路径无效", "filePath");
            }
            
            this.path = filePath;

            if (0 == filePath.LastIndexOf(".ini"))
            {
                this.path += ".ini";
            }
        }

        private string path;

        /// <summary>
        /// 创建INI文件
        /// </summary>
        public void Create()
        {
            if (!File.Exists(this.path))
            {
                FileStream fs = File.Create(this.path);
                fs.Close();
            }
        }

        /// <summary>
        /// 写入字符串数据
        /// </summary>
        /// <param name="segName">写入数据的段名</param>
        /// <param name="keyName">写入数据的键名</param>
        /// <param name="value">写入数据的值</param>
        public void WriteString(string segName, string keyName, string value)
        {
            HmzWin32App.WritePrivateProfileString(segName, keyName, value, this.path);
        }

        /// <summary>
        /// 写入浮点型数据
        /// </summary>
        /// <param name="segName">写入数据的段名</param>
        /// <param name="keyName">写入数据的键名</param>
        /// <param name="value">写入数据的值</param>
        public void WriteDouble(string segName, string keyName, double value)
        {
            HmzWin32App.WritePrivateProfileString(segName, keyName, value.ToString(), this.path);
        }

        /// <summary>
        /// 写入指定段名的整段数据
        /// </summary>
        /// <param name="segName">写入数据的段名</param>
        /// <param name="keys">写入数据的键集合</param>
        /// <param name="values">写入数据的值集合</param>
        public void WriteSection(string segName, string[] keys, string[] values)
        {
            if (keys.Length != values.Length)
            {
                throw new Exception("WriteSection(): 缺失参数");
            }

            for (int i = 0; i < keys.Length; i++)
            {
                HmzWin32App.WritePrivateProfileString(segName, keys[i], values[i], this.path);
            }
        }

        /// <summary>
        /// 在INI文件末尾追加空行
        /// </summary>
        public void WriteSpaceLine()
        {            
            using (StreamWriter sw = new StreamWriter(this.path, true))
            {                
                sw.WriteLine();
                sw.Flush();
            }
        }

        /// <summary>
        /// 读取字符串数据
        /// </summary>
        /// <param name="segName">读取数据的段名</param>
        /// <param name="keyName">读取数据的键名</param>
        /// <returns>字符串数据</returns>
        public string ReadString(string segName, string keyName)
        {
            StringBuilder sbStr = new StringBuilder(512);
            HmzWin32App.GetPrivateProfileString(segName, keyName, "", sbStr, sbStr.Capacity, this.path);

            return sbStr.ToString();
        }

        /// <summary>
        /// 读取浮点型数据
        /// </summary>
        /// <param name="segName">读取数据的段名</param>
        /// <param name="keyName">读取数据的键名</param>
        /// <returns>浮点型数据</returns>
        public double ReadDouble(string segName, string keyName)
        {
            double value = 0;
            StringBuilder sbStr = new StringBuilder(128);
            HmzWin32App.GetPrivateProfileString(segName, keyName, "0", sbStr, sbStr.Capacity, this.path);

            if (!double.TryParse(sbStr.ToString(), out value))
            {
                value = 0;
            }

            return value;
        }

        /// <summary>
        /// 读取整数型数据
        /// </summary>
        /// <param name="segName">读取数据的段名</param>
        /// <param name="keyName">读取数据的键名</param>
        /// <returns>整型数据</returns>
        public int ReadInt(string segName, string keyName)
        {
            return HmzWin32App.GetPrivateProfileInt(segName, keyName, 0, this.path);
        }

        /// <summary>
        /// 读取指定段名的整段数据
        /// </summary>
        /// <param name="segName">读取数据的段名</param>
        /// <param name="keys">读取数据的键集合</param>
        /// <returns>文件中获得的字符串数据集合</returns>
        public string[] ReadSection(string segName, string[] keys)
        {
            List<string> values = new List<string>();

            foreach (string item in keys)
            {
                StringBuilder sbStr = new StringBuilder();
                HmzWin32App.GetPrivateProfileString(segName, item, "", sbStr, sbStr.Capacity, this.path);
                values.Add(sbStr.ToString());
            }

            return values.ToArray();
        }

        /// <summary>
        /// 删除指定段名及键名的数据信息
        /// </summary>
        /// <param name="segName">需要删除的数据的段名</param>
        /// <param name="keyName">需要删除的数据的键名</param>
        /// <returns>执行成功返回true</returns>
        public bool DeleteKey(string segName, string keyName)
        {
            return HmzWin32App.WritePrivateProfileString(segName, keyName, null, this.path);
        }

        /// <summary>
        /// 删除指定段名的整段数据信息
        /// </summary>
        /// <param name="segName">需要删除的数据的段名</param>
        /// <returns>执行成功返回true</returns>
        public bool DeleteSection(string segName)
        {
            return HmzWin32App.WritePrivateProfileString(segName, null, null, this.path);
        }
    }
}
