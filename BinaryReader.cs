using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;

namespace WoWCacheViewer
{
    class NumberManipulator
    {
        public int CalcNum(ref byte[] buff)
        {
            return (buff[0] + 256) % 256 + (buff[1] + 256) % 256 * 256 +
                (buff[2] + 256) % 256 * 256 * 256 + (buff[3] + 256) % 256 * 256 * 256 * 256;
        }
        public int strlen(ref byte[] buff)
        {
            for (int i = 0; ; i++)
            {
                if (buff[i] == 0)
                    return i;
            }
        }
    }
    class ReadFromFile
    {
        private string cachetype;
        public ReadFromFile(string cache)
        {
            cachetype = cache;
        }

        public CacheItem[] ReadCacheFile(string filename)
        {
            if (cachetype == "WMOB")
                return GetMobs(filename);
            else return null;
        }

        private CacheItem[] GetMobs(string filename)
        {
            //Initialize
            CacheItem item;
            item.typeid = 0;
            item.clsid = 0;
            ArrayList itemList = new ArrayList();
            char[] sbuff = new char[256];
            byte[] rbuff = new byte[256];
            byte[] tmpbyte = new byte[256];
            byte[] byteinfo;
            String strread, npctitle;

            //Open the cache file
            BinaryReader fin;
            try
            {
                fin = new BinaryReader(new FileStream(filename, FileMode.Open));
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message + "\nCannot open file.", "Error!");
                return null;
            }

            //Read the cache file
            try
            {
                sbuff = fin.ReadChars(4);
                strread = new string(sbuff);
                rbuff = fin.ReadBytes(4);
                sbuff = fin.ReadChars(4);
                NumberManipulator GetNum = new NumberManipulator();

                //Read NPC Info
                rbuff = fin.ReadBytes(12);
                rbuff = fin.ReadBytes(4);
                int npcid, length, tmplen, ptr;
                npcid = GetNum.CalcNum(ref rbuff);
                while (npcid > 0)
                {
                    //Read the NPC Name
                    rbuff = fin.ReadBytes(4);
                    length = GetNum.CalcNum(ref rbuff);
                    rbuff = fin.ReadBytes(8);
                    rbuff = fin.ReadBytes(8);
                    Array.Clear(tmpbyte, 0, tmpbyte.Length);
                    tmpbyte[0] = rbuff[7];
                    length -= 16;


                    rbuff = fin.ReadBytes(length);
                    tmplen = 1; ptr = 0;
                    while (rbuff[ptr] > 0) 
                        tmpbyte[tmplen++] = rbuff[ptr++];
                    if (rbuff[ptr + 1] == tmpbyte[0] && rbuff[ptr + 2] == tmpbyte[1])
                        ptr += tmplen + 1;
                        
                    byteinfo = new byte[tmplen];
                    Array.Copy(tmpbyte, byteinfo, tmplen);
                    strread = Encoding.UTF8.GetString(byteinfo);
                    item.typeid = (int)rbuff[ptr + 9];
                    item.clsid = (int)rbuff[ptr + 17];

                    //Title & Description
                    Array.Clear(tmpbyte, 0, tmpbyte.Length);
                    tmplen = 0;
                    int deslen = length - 2;
                    bool istitled = false;
                    if (rbuff[deslen] != 1)
                    {
                        while (rbuff[deslen] > 0)
                        {
                            tmpbyte[tmplen++] = rbuff[deslen--];
                            istitled = true;
                        }
                    }
                    if(istitled)
                    {
                        deslen--;
                        if (rbuff[deslen] > 0)
                        {
                            Array.Clear(tmpbyte, 0, tmpbyte.Length);
                            tmplen = 0;
                            while (rbuff[deslen] > 0)
                                tmpbyte[tmplen++] = rbuff[deslen--];
                        }
                    }

                    byteinfo = new byte[tmplen];
                    Array.Copy(tmpbyte, byteinfo, tmplen);
                    Array.Reverse(byteinfo);
                    npctitle = Encoding.UTF8.GetString(byteinfo);
                    //if (npctitle.Length > 0)
                    //    strread = strread + " <" + npctitle + ">";

                    /******Add to the Array List******/
                    item.itemID = npcid;
                    item.name = strread;
                    item.descrition = npctitle;
                    itemList.Add(item);
                    /****************************************/

                    rbuff = fin.ReadBytes(4);
                    npcid = GetNum.CalcNum(ref rbuff);
                }
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message + "\nCannot read from file.", "Error!");
                return null;
            }
            //End
            fin.Close();
            return (CacheItem[])itemList.ToArray(typeof(CacheItem));
        }
    }
}