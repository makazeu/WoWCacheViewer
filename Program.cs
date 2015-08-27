using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

struct CacheItem
{
    public int itemID;
    public string name;
    public string descrition;
    public int typeid;
    public int clsid;
}

namespace WoWCacheViewer
{
    public class ProgramInfo
    {
        public string ProgramName = "WoWCache Viewer";
        public string ProgramAuthor = "Makazeu";
        public string ProgramVersion = "1.0";
    }

    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
