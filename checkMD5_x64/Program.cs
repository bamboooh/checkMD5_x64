using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace checkMD5_x64
{
    class Program
    {
        static readonly HashAlgorithm hashProvider = new MD5CryptoServiceProvider();
        static void Main(string[] args)
        {
            int i = 0;
            StringBuilder allmd5 = new StringBuilder();
            String allmd5_str;
            ulong allsize = 0;
            Encoding enc = Encoding.GetEncoding("UTF-8");
            string LogFileName = "./time_checkMD5_result.txt";
            DateTime dt = DateTime.Now;
            string time = dt.ToString("yyyMMdd_HHmmss");
            LogFileName = LogFileName.Replace("time", time);


            //フォルダ以下にあるファイルをすべて取得する
            //"*.jpg"の部分を"*"にするとすべての拡張子を取得できる
            //SearchOption.AllDirectoriesにすると下の階層のフォルダ内まで探索する
            //string[] filesFullPath = System.IO.Directory.GetFiles(@".\", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            string[] filesFullPath = System.IO.Directory.GetFiles(@".\", "*", System.IO.SearchOption.AllDirectories);

            Console.WriteLine("処理スタート");

            using (StreamWriter writer = new StreamWriter(LogFileName, false, enc))
            {
                Console.WriteLine("MD5ファイル確認結果");
                writer.WriteLine("MD5ファイル確認結果");
                var myhash = ComputeFileHash(@".\checkMD5_x64.exe");
                Console.WriteLine("checkMD5_x64.exeのMd5値：{0}", myhash);
                writer.WriteLine("checkMD5_x64.exeの値：{0}", myhash);
                Console.WriteLine("実行日時：{0}", dt);
                writer.WriteLine("実行日時：{0}", dt);
                Console.WriteLine("");
                writer.WriteLine("");
            }

            //filesFullPath配列から一つずつ画像のフルパスを取得する→fileFullPathへ格納
            foreach (string fileFullPath in filesFullPath)
            {
                //GetFileNameメソッドをつかって、ファイル名を取得する(using System.IO;が必要)
                string fileName = Path.GetFileName(fileFullPath);
                if (true == fileName.Contains("checkMD5"))
                {
                    continue;
                }
                i++;
                using (StreamWriter writer = new StreamWriter(LogFileName, true, enc))
                {
                    Console.WriteLine("ファイル{0}：", i);
                    writer.WriteLine("ファイル{0}：", i);
                    //ファイル名のみを出力
                    Console.WriteLine("Name   = {0}", fileName);
                    writer.WriteLine("Name   = {0}", fileName);
                    //フルパスを出力
                    Console.WriteLine("Path   = {0}", fileFullPath);
                    writer.WriteLine("Path   = {0}", fileFullPath);
                    //ファイルをハッシュ化
                    var hash = ComputeFileHash(fileFullPath);
                    Console.WriteLine("Md5    = {0}", hash);
                    writer.WriteLine("Md5    = {0}", hash);
                    //ファイル情報取得
                    FileInfo file = new FileInfo(fileFullPath);
                    //ファイル更新日時の取得
                    DateTime Update = file.LastWriteTime;
                    Console.WriteLine("Update = {0}", Update);
                    writer.WriteLine("Update = {0}", Update);
                    //ファイルのサイズ取得
                    long size = file.Length;
                    Console.WriteLine("Size   = {0:N0}[byte]", size);
                    writer.WriteLine("Size   = {0:N0}[byte]", size);
                    allsize += (ulong)size;
                    Console.WriteLine("");
                    writer.WriteLine("");

                    //ハッシュ値の先頭4文字を保持
                    allmd5.Append(hash.Substring(0, 4));
                    //ファイルサイズの合計値を保持

                }
            }

            allmd5_str = allmd5.ToString();
            Console.WriteLine("すべてのハッシュ値の先頭4を連結した値：{0}", allmd5_str);
            var allmd5_hash = ComputeStringHash(allmd5.ToString());

            using (StreamWriter writer = new StreamWriter(LogFileName, true, enc))
            {
                Console.WriteLine("----------------------------------------------");
                writer.WriteLine("----------------------------------------------");
                //すべてのファイルの先頭4文字を再ハッシュ化
                Console.WriteLine("AllFile_Md5 = {0}", allmd5_hash);
                writer.WriteLine("AllFile_Md5 = {0}", allmd5_hash);
                //合計ファイルサイズ計算
                Console.WriteLine("AllFile_Size = {0:N0}[byte]", allsize);
                writer.WriteLine("AllFile_Size = {0:N0}[byte]", allsize);

            }

            Console.WriteLine("処理終了");

            //コンソールが勝手に消えないように、入力待ちにする
            //Console.ReadKey();
        }

        /// <summary>
        /// Returns the hash string for the file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ComputeFileHash(string filePath)
        {
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var bs = hashProvider.ComputeHash(fs);
            return BitConverter.ToString(bs).ToLower().Replace("-", "");
        }

        public static string ComputeStringHash(string str)
        {
            Encoding enc = Encoding.GetEncoding("UTF-8");
            var bs = hashProvider.ComputeHash(enc.GetBytes(str));
            return BitConverter.ToString(bs).ToLower().Replace("-", "");
        }
    }
}
