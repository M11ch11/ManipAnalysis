using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ManipAnalysis
{
    static public class MD5
    {
        static public string computeHash(string path)
        {
            MD5CryptoServiceProvider md5;
            FileStream fs;
            byte[] buffer;

            fs = new FileStream(path,  FileMode.Open, FileAccess.Read);
            md5 = new MD5CryptoServiceProvider();
            buffer = md5.ComputeHash(fs);
            fs.Close();
            md5.Clear();
            string hash = Convert.ToBase64String(buffer);

            return hash;
        }
    }
}
