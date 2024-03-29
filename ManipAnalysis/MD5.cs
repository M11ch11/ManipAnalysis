﻿using System;
using System.IO;
using System.Security.Cryptography;

namespace ManipAnalysis_v2
{
    public static class Md5
    {
        public static string ComputeHash(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var md5 = new MD5CryptoServiceProvider();
            var buffer = md5.ComputeHash(fs);
            fs.Close();
            md5.Clear();

            var hash = Convert.ToBase64String(buffer);

            return hash;
        }
    }
}