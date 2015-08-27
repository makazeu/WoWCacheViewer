using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoWCacheViewer
{
    public partial class Form1 : Form
    {
        ProgramInfo proinfo = new ProgramInfo();
        CacheItem[] Item;
        int ItemNum = 0;
        public Form1()
        {
            InitializeComponent();
            this.Text = proinfo.ProgramName;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(proinfo.ProgramName + " " + proinfo.ProgramVersion + "\n\n(C) 2015 "+ proinfo.ProgramAuthor,"About...");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            //MessageBox.Show();
            LoadFile(openFileDialog1.FileName);
        }

        private void LoadFile(string filename)
        {
            statusStrip1.Items[0].Text = "Ready.";
            //Open the cache file
            FileStream filestream;
            BinaryReader fin;
            try
            {
                filestream = new FileStream(filename, FileMode.Open);
                fin = new BinaryReader(filestream);
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message + "\nCannot open file.", "Error!");
                return;
            }

            char[] sbuff = new char[256];
            byte[] rbuff = new byte[256];
            string strread, locale, cachetype;
            int build;
            try
            {
                //Cache File Type
                sbuff = fin.ReadChars(4);
                Array.Reverse(sbuff);
                strread = new string(sbuff);
                if (strread != "WMOB" && strread != "WGOB")
                {
                    MessageBox.Show("This is not a WoW Creature or GameObject Cache file!","Error!");
                    filestream.Close();
                    return;
                }
                cachetype = strread == "WMOB" ? "Creature" : "GameObject";
                //WoW Build
                rbuff = fin.ReadBytes(4);
                NumberManipulator GetNum = new NumberManipulator();
                build = GetNum.CalcNum(ref rbuff);
                //WoW Locale
                sbuff = fin.ReadChars(4);
                Array.Reverse(sbuff);
                locale = new string(sbuff);
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message + "\nCannot read from file.", "Error!");
                filestream.Close();
                return;
            }
            filestream.Close();

            ReadFromFile CacheReader = new ReadFromFile(strread);
            Item = CacheReader.ReadCacheFile(filename);
            ItemNum = Item.Count();
            toolStripStatusLabel1.Text = ">>> " + ItemNum.ToString() + " " + cachetype + "s found.   Locale: " + locale + "  Build: " + build;

            //Display Grid
            int rowindex, typecol = 2, clscol = 3;
            dataGridView1.Rows.Clear();
            dataGridView1.Visible = true;
            toolStripProgressBar1.Visible = true;
            for (int i = 1; i <= ItemNum; i++)
            {
                rowindex = dataGridView1.Rows.Add();
                dataGridView1.Rows[rowindex].Cells[0].Value = Item[i - 1].itemID;
                dataGridView1.Rows[rowindex].Cells[1].Value = Item[i - 1].name;
                dataGridView1.Rows[rowindex].Cells[4].Value = Item[i - 1].descrition;
                switch(Item[i - 1].typeid)
                {
                    case 1: dataGridView1.Rows[rowindex].Cells[typecol].Value = "野兽"; break;
                    case 2: dataGridView1.Rows[rowindex].Cells[typecol].Value = "龙类"; break;
                    case 3: dataGridView1.Rows[rowindex].Cells[typecol].Value = "恶魔"; break;
                    case 4: dataGridView1.Rows[rowindex].Cells[typecol].Value = "元素生物"; break;
                    case 5: dataGridView1.Rows[rowindex].Cells[typecol].Value = "巨人"; break;
                    case 6: dataGridView1.Rows[rowindex].Cells[typecol].Value = "亡灵"; break;
                    case 7: dataGridView1.Rows[rowindex].Cells[typecol].Value = "人型生物"; break;
                    case 8: dataGridView1.Rows[rowindex].Cells[typecol].Value = "小动物"; break;
                    case 9: dataGridView1.Rows[rowindex].Cells[typecol].Value = "机械"; break;
                    case 10: dataGridView1.Rows[rowindex].Cells[typecol].Value = "未指定"; break;
                    case 11: dataGridView1.Rows[rowindex].Cells[typecol].Value = "图腾"; break;
                    case 12: dataGridView1.Rows[rowindex].Cells[typecol].Value = "非战斗宠物"; break;
                    case 13: dataGridView1.Rows[rowindex].Cells[typecol].Value = "气体云雾"; break;
                    case 14: dataGridView1.Rows[rowindex].Cells[typecol].Value = "野生宠物"; break;
                    case 15: dataGridView1.Rows[rowindex].Cells[typecol].Value = "畸变怪"; break;
                    default: dataGridView1.Rows[rowindex].Cells[typecol].Value = "——"; break;
                }
                switch (Item[i - 1].clsid)
                {
                    case 1: dataGridView1.Rows[rowindex].Cells[clscol].Value = "精英"; break;
                    case 2: dataGridView1.Rows[rowindex].Cells[clscol].Value = "稀有精英"; break;
                    case 3: dataGridView1.Rows[rowindex].Cells[clscol].Value = "首领"; break;
                    case 4: dataGridView1.Rows[rowindex].Cells[clscol].Value = "稀有"; break;
                    default: dataGridView1.Rows[rowindex].Cells[clscol].Value = "普通"; break;
                }
                toolStripProgressBar1.Value = i * 100 / ItemNum;
            }
            toolStripProgressBar1.Visible = false;
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                dataGridView1.RowHeadersWidth - 4,
                e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dataGridView1.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Visible = false;
            toolStripStatusLabel1.Text = "Ready.";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
