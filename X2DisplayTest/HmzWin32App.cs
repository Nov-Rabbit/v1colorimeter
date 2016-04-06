/*************************************************
 * 版权(C)：Ceway Tech.Co.,Ltd
 * 作者：Jackie
 * 版本：v1.1
 * 时间：2013/2/14
 * 模块描述：对系统的API函数的导出
 * 
 * 函数列表：
 *         1. ReleaseCaptrue()              -- 释放被当前线程中某个窗口捕获的光标
 *         2. SendMessage()                 -- 向窗体发送Windows消息
 *         3. WritePrivateProfileString()   -- 系统API对INI文件写方法
 *         4. GetPrivateProfileString()     -- 系统API对INI文件字符串的读方法
 *         5. GetPrivateProfileInt()        -- 系统API对INI文件整型数据的读方法
 *         
 * 历史记录：
 *      作者      时间      版本              描述
 *     Jackie  2013/02/14   v1.0            建立该类
 *     Jackie  2013/10/19   v1.1    添加了GetPrivateProfileInt()函数
 * ***********************************************/

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Hnwlxy.HmzSysPlatform
{
    /// <summary>
    /// 导出系统API函数
    /// </summary>
    internal static class HmzWin32App
    {
        #region 消息

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        #endregion

        /// <summary>
        /// 用来释放被当前线程中某个窗口捕获的光标
        /// </summary>
        /// <returns>如果函数执行成功，则返回非0值</returns>
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        public static extern bool ReleaseCaptrue();

        /// <summary>
        /// 向指定的窗体发送Windows消息
        /// </summary>
        /// <param name="hWnd">窗体句柄</param>
        /// <param name="msg">发送的消息</param>
        /// <param name="wParam">消息的附加信息</param>
        /// <param name="lParam">消息的附加信息</param>
        /// <returns>消息处理的结果</returns>
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern bool SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        /// <summary>
        /// Windows API 对INI文件写方法
        /// </summary>
        /// <param name="lpAppName">要在其中写入新字串的段名。这个字串不区分大小写</param>
        /// <param name="lpKeyName">要设置的项名或条目名。这个字串不区分大小写。用null可删除这个小节的所有设置项</param>
        /// <param name="lpString">指定为这个项写入的字串值。用null表示删除这个项现有的字串</param>
        /// <param name="lpFileName">初始化文件的名字。如果没有指定完整路径名，则windows会在windows目录查找文件。如果文件没有找到，则函数会创建它</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString", CharSet = CharSet.Unicode)]
        public static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName,
            string lpString, string lpFileName);

        /// <summary>
        /// Windows API 对INI文件字符串的读方法
        /// </summary>
        /// <param name="lpAppName">要在其中读取字符串的段名。这个字串不区分大小写</param>
        /// <param name="lpKeyName">要读取数据的关键字名称。这个字串不区分大小写。</param>
        /// <param name="lpDefault">当不存在指定关键字名称时，默认返回的字符串</param>
        /// <param name="lpReturnedString">返回的字符串</param>
        /// <param name="nSize">lpReturnedString参数的空间字节大小</param>
        /// <param name="lpFileName">初始化文件的名字。如果没有指定完整路径名，则windows会在windows目录查找文件。如果文件没有找到，则函数会创建它</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName,
            string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        /// <summary>
        /// Windows API 对INI文件整数的读方法
        /// </summary>
        /// <param name="lpAppName">要在其中读取字符串的段名。这个字串不区分大小写</param>
        /// <param name="lpKeyName">要读取数据的关键字名称。这个字串不区分大小写。用null可删除这个小节的所有设置项</param>
        /// <param name="nDefault">当未能找到指定的关键字名称时，则返回的默认值</param>
        /// <param name="lpFileName">初始化文件的名字。如果没有指定完整路径名，则windows会在windows目录查找文件。如果文件没有找到，则函数会创建它</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileInt", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileInt(
            string lpAppName,
            string lpKeyName,
            int nDefault, 
            string lpFileName);
    }
}
