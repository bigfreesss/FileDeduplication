﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace 文件去重
{
    public partial class Form1 : Form
    {
        //DataSet data;
        public Form1()
        {
            try
            {
                InitializeComponent();
                textBox1.Text = AppDomain.CurrentDomain.BaseDirectory + "input";
                if (!Directory.Exists(textBox1.Text))
                {
                    Directory.CreateDirectory(textBox1.Text);
                }
                SQLiteHelper.ExecuteSql("vacuum");//数据库收缩指令
                //data = SQLiteHelper.Query("select path,size,md5,Hash,SHA256,LastWriteTime from file");
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }
        class Features
        {
            public string path { get; set; }
            public long size { get; set; }
            public string md5 { get; set; }
            public string Hash { get; set; }//sha1
            public string SHA256 { get; set; }
            public DateTime LastWriteTime { get; set; }
            //public FileInfo file { get; set; }
        }
        class HistoricalPath
        {
            public string oldPath { get; set; }
            public string newPath { get; set; }
        }
        List<HistoricalPath> historicalPaths = new List<HistoricalPath>();
        /// <summary>
        /// 文件去重本体
        /// </summary>
        void FileDuplicateRemoval()
        {
            string time = DateTime.Now.ToString("yyyyMMdd-HH-mm-ss");
            try
            {
                button1.Enabled = false;
                bool DirOnlyHere_time = DirOnlyHere;
                DirOnlyHere = false;
                log("加载中", time);

                button1.Text = "进行中";
                string FilePath = textBox1.Text;
                string Format = textBox2.Text;
                bool md5Open = checkBox_md5.Checked;
                bool HashOpen = checkBox_Hash.Checked;
                bool sha256Open = checkBox_sha256.Checked;
                string RepeatFilePath = AppDomain.CurrentDomain.BaseDirectory + @"RepeatFilePath\" + time;
                if (!Directory.Exists(RepeatFilePath))
                {
                    Directory.CreateDirectory(RepeatFilePath);
                }
                names.Clear();

                List<string> FilePathAllList = new List<string>();//防止输入重复路径
                if (Multifile.Checked)
                {
                    char[] Separator = { '+' };
                    string[] FilePathAll = FilePath.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                    if (FilePathAll.Length > 0)
                    {
                        int index = 0;
                        int OK = 0;
                        int error = 0;
                        FileGet.lst.Clear();
                        foreach (string str in FilePathAll)
                        {
                            progressBar1.Maximum = FilePathAll.Length;
                            label1.Text = $"{progressBar1.Value}/{progressBar1.Maximum} 扫描到文件：{FileGet.lst.Count} 正确路径：{OK} 过滤路径：{error}";
                            if (FilePathAllList.FindIndex(o => o.Equals(str) || (o.Length > str.Length && o.Contains(str) && o.Remove(0, str.Length).Contains(@"\")) || (str.Length > o.Length && str.Contains(o) && str.Remove(0, o.Length).Contains(@"\"))) == -1)
                            {
                                // 确保路径是完全限定的
                                if (Path.IsPathRooted(str))
                                {
                                    FilePathAllList.Add(str);
                                    if (DirOnlyHere_time)
                                        FileGet.getdirOnlyHere(str, Format);
                                    else
                                        FileGet.getdir(str, Format);
                                    OK++;
                                }
                                else
                                {
                                    error++;
                                    log("不是绝对路径：" + str, time);
                                }
                            }
                            else
                            {
                                error++;
                                log("是子路径：" + str, time);
                            }
                            progressBar1.Value = ++index;
                        }

                        FilePath = "";
                        foreach (string str in FilePathAllList)
                        {
                            FilePath += str + "+";
                        }
                        textBox1.Text = FilePath.Substring(0, FilePath.Length - 1);
                    }
                }
                else
                {
                    // 确保路径是完全限定的
                    if (Path.IsPathRooted(FilePath))
                    {
                        FileGet.getFile(FilePath, Format);
                    }
                    else
                    {
                        log("不是绝对路径：" + FilePath, time);
                    }
                }

                log("开始：" + textBox1.Text + " 格式：" + Format + " md5：" + md5Open + " Hash：" + HashOpen + " sha256：" + sha256Open, time);

                List<Features> features = new List<Features>();

                //进度条
                int index_progressBar1_Value = 0;
                progressBar1.Value = index_progressBar1_Value;
                label1.Text = progressBar1.Value + "/" + FileGet.lst.Count;
                log("共扫描到" + FileGet.lst.Count + "个文件", time);
                RepeatNum = 0;//重复文件个数
                int NoRepeatNum = 0;//不重复文件个数
                int ErrorNum = 0;
                string NewPath = "";

                foreach (FileInfo file in FileGet.lst)
                {
                    try
                    {
                        progressBar1.Maximum = FileGet.lst.Count;
                        string md5 = "";
                        string Hash = "";
                        string sha256 = "";
                        string path = file.FullName;
                        var index = features.FindAll(o => o.size == file.Length);//先比较文件大小
                        if (index.Count == 0)//文件大小不同
                        {
                            Features features1 = new Features();
                            DateTime start = DateTime.Now;
                            features1.size = file.Length;
                            features1.path = path;
                            features1.LastWriteTime = file.LastWriteTime;
                            //features1.file = file;
                            features.Add(features1);
                            NoRepeatNum++;
                        }
                        else//如果文件大小相同，再计算哈希
                        {
                            DateTime start = DateTime.Now;
                            if (checkBox_sql.Checked)
                            {
                                //查找数据库数据
                                var data = SQLiteHelper.Query(string.Format("select path,size,md5,Hash,SHA256,LastWriteTime from file where path = '{0}'", path));
                                //log($"查找数据库用时：{(DateTime.Now - start).TotalSeconds}s", time);
                                if (data.Tables[0].Rows.Count != 1)//数据库不存在数据或有重复数据
                                {
                                    if (data.Tables[0].Rows.Count > 1)//有重复数据
                                    {
                                        log($"数据库有重复数据：{path}",time);
                                        SQLiteHelper.ExecuteSql($"delete from file where path = '{path}'");//删除数据
                                    }
                                    log($"数据库无数据：{path}", time);
                                    start = DateTime.Now;
                                    //当前文件的md5和hash
                                    if (md5Open)
                                        md5 = GetMD5HashFromFile(file.Open(System.IO.FileMode.Open));
                                    if (HashOpen)
                                        Hash = GetHash(file.Open(System.IO.FileMode.Open));
                                    if (sha256Open)
                                        sha256 = general_sha256_code(file.Open(System.IO.FileMode.Open, System.IO.FileAccess.Read));
                                    //log($"计算哈希值用时：{(DateTime.Now - start).TotalSeconds}s", time);

                                    //string sql = $"insert into file(path,size,md5,Hash,SHA256,LastWriteTime) values('{path}','{file.Length}','{md5}','{Hash}','{sha256}','{file.LastWriteTime}')";
                                    ////log($"path={path}, size={file.Length}, md5={md5}, Hash={Hash}, SHA256={sha256}, LastWriteTime={file.LastWriteTime}", time);
                                    //SQLiteHelper.ExecuteSql(sql);//添加数据到数据库
                                    //log($"写入数据库用时：{(DateTime.Now - start).TotalSeconds}s", time);
                                    //更新数据
                                    ArrayList SQLStringList = new ArrayList();
                                    SQLStringList.Add($"delete from file where path = '{path}'");//删除数据
                                    SQLStringList.Add($"insert into file(path,size,md5,Hash,SHA256,LastWriteTime) values('{path}','{file.Length}','{md5}','{Hash}','{sha256}','{file.LastWriteTime}')");//添加数据到数据库
                                    SQLiteHelper.ExecuteSqlTran(SQLStringList);
                                }
                                else//数据库存在数据，从数据库中提取数据
                                {
                                    start = DateTime.Now;
                                    if (data.Tables[0].Rows[0].Field<string>("size") == file.Length.ToString() &&
                                        (data.Tables[0].Rows[0].Field<string>("LastWriteTime")) == file.LastWriteTime.ToString())//判断更新时间和大小是否匹配
                                    {
                                        log($"数据库存在数据：{path}", time); 
                                        md5 = data.Tables[0].Rows[0].Field<string>("md5");
                                        Hash = data.Tables[0].Rows[0].Field<string>("Hash");
                                        sha256 = data.Tables[0].Rows[0].Field<string>("SHA256");
                                    }
                                    else
                                    {
                                        log($"数据库更新数据：{path}", time);
                                        //当前文件的md5和hash
                                        if (md5Open)
                                            md5 = GetMD5HashFromFile(file.Open(System.IO.FileMode.Open));
                                        if (HashOpen)
                                            Hash = GetHash(file.Open(System.IO.FileMode.Open));
                                        if (sha256Open)
                                            sha256 = general_sha256_code(file.Open(System.IO.FileMode.Open, System.IO.FileAccess.Read));
                                        //更新数据
                                        //SQLiteHelper.ExecuteSql($"update file SET size = '{file.Length}', md5 = '{md5}' , Hash = '{Hash}' , SHA256 = '{sha256}', LastWriteTime ='{file.LastWriteTime}' WHERE path = '{path}'");
                                        ArrayList SQLStringList = new ArrayList();
                                        SQLStringList.Add($"delete from file where path = '{path}'");//删除数据
                                        SQLStringList.Add($"insert into file(path,size,md5,Hash,SHA256,LastWriteTime) values('{path}','{file.Length}','{md5}','{Hash}','{sha256}','{file.LastWriteTime}')");//添加数据到数据库
                                        SQLiteHelper.ExecuteSqlTran(SQLStringList);
                                    }
                                    //log($"从数据库读取用时：{(DateTime.Now - start).TotalSeconds}s", time);
                                }
                            }
                            else//不使用数据库模式
                            {
                                //当前文件的md5和hash
                                if (md5Open)
                                    md5 = GetMD5HashFromFile(file.Open(System.IO.FileMode.Open));
                                if (HashOpen)
                                    Hash = GetHash(file.Open(System.IO.FileMode.Open));
                                if (sha256Open)
                                    sha256 = general_sha256_code(file.Open(System.IO.FileMode.Open, System.IO.FileAccess.Read));
                            }
                            //log(path + " md5：" + md5);
                            //log(path + " Hash：" + Hash);
                            //log(path + " SHA256：" + SHA256);

                            bool BeRepeat = false;
                            for (int i = 0;i< index.Count;i++)
                            {
                                index[i] = sqlquery(index[i], time, md5Open, HashOpen, sha256Open);
                                //log($"数据库添加数据：{index[i].path}", time);
                                //if (index[i].md5 == null && md5Open)
                                //{
                                //    sqlBeData = false;
                                //    index[i].md5 = GetMD5HashFromFile(index[i].path);
                                //}
                                //if (index[i].Hash == null && HashOpen)
                                //{
                                //    sqlBeData = false;
                                //    index[i].Hash = GetHash(index[i].path);
                                //}
                                //if (index[i].SHA256 == null && sha256Open)
                                //{
                                //    sqlBeData = false;
                                //    index[i].SHA256 = general_sha256_code(index[i].path, Sha26ParseType.StreamType);
                                //}
                                //if(!sqlBeData)
                                //{
                                //    //更新数据到数据库
                                //    //SQLiteHelper.ExecuteSql($"update file SET size = '{index[i].size}'  md5 = '{index[i].md5}' , Hash = '{index[i].Hash}' , SHA256 = '{index[i].SHA256}',LastWriteTime ='{index[i].LastWriteTime}' WHERE path = '{index[i].path}'");
                                //    ArrayList SQLStringList = new ArrayList();
                                //    SQLStringList.Add($"delete from file where path = '{index[i].path}'");//删除数据
                                //    SQLStringList.Add($"insert into file(path,size,md5,Hash,SHA256,LastWriteTime) values('{index[i].path}','{index[i].size}','{index[i].md5}','{index[i].Hash}','{index[i].SHA256}','{index[i].LastWriteTime}')");//添加数据到数据库
                                //    SQLiteHelper.ExecuteSqlTran(SQLStringList);
                                //}
                                if (md5 == index[i].md5 && Hash == index[i].Hash)
                                {
                                    //确定为同一文件
                                    string oldpath = file.FullName;
                                    log(oldpath + " 与该文件相同： " + index[i].path, time);
                                    if (checkBox_Delete.Checked)
                                    {
                                        file.Delete();
                                        log(oldpath + " 已删除", time);
                                        NewPath = "";
                                    }
                                    else
                                    {
                                        if (!Directory.Exists(RepeatFilePath))
                                        {
                                            Directory.CreateDirectory(RepeatFilePath);
                                        }
                                        NewPath = RepeatRename(RepeatFilePath, file);
                                        file.MoveTo(NewPath);
                                        log(oldpath + " 已移动到： " + file.FullName, time);
                                        //设置还原点
                                        HistoricalPath historicalPath = new HistoricalPath();
                                        historicalPath.oldPath = oldpath;
                                        historicalPath.newPath = file.FullName;
                                        historicalPaths.Add(historicalPath);
                                    }

                                    RepeatNum++;
                                    BeRepeat = true;
                                    //更新数据到数据库
                                    ArrayList SQLStringList = new ArrayList();
                                    SQLStringList.Add($"delete from HistoricalPath where oldpath = '{oldpath}'");//删除数据
                                    SQLStringList.Add($"insert into HistoricalPath(oldpath,NewPath,repeatPath,time) values('{oldpath}','{NewPath}','{index[i].path}','{time}')");//添加数据到数据库
                                    SQLiteHelper.ExecuteSqlTran(SQLStringList);
                                    break;
                                }
                            }
                            if (!BeRepeat)//不同文件保存到列表
                            {
                                NoRepeatNum++;
                                Features features1 = new Features();
                                features1.size = file.Length;
                                features1.path = path;
                                features1.md5 = md5;
                                features1.Hash = Hash;
                                features1.SHA256 = sha256;
                                features1.LastWriteTime = file.LastWriteTime;
                                //features1.file = file;
                                features.Add(features1);
                            }
                        }
                        progressBar1.Value = index_progressBar1_Value++;
                        string percentage = string.Format("{0:F2}", (((double)progressBar1.Value / progressBar1.Maximum) * 100));
                        if (progressBar1.Value == progressBar1.Maximum)
                            percentage = "100";
                        label1.Text = progressBar1.Value + "/" + progressBar1.Maximum + " " + percentage + "% 重复文件：" + RepeatNum + " 不重复文件：" + NoRepeatNum + " 错误文件：" + ErrorNum;
                    }
                    catch (Exception ex)
                    {
                        progressBar1.Value = ++index_progressBar1_Value;
                        ErrorNum++;
                        log("错误：" + Environment.NewLine + "错误文件：" + file.FullName + Environment.NewLine + "目标路径：" + NewPath + Environment.NewLine + ex, time);
                    }
                }
                log("结束", time);
                button1.Text = "开始";
                DeleteNullFileRecursion(AppDomain.CurrentDomain.BaseDirectory, time);
                if (checkBox_DeleteNullFile.Checked)
                {
                    foreach (string str in FilePathAllList)
                    {
                        DeleteNullFileRecursion(str, time);
                    }
                }
                button1.Enabled = true;
            }
            catch (Exception ex)
            {
                log("button1错误：" + ex.Message, time);
                button1.Text = "开始";
                MessageBox.Show(ex.Message, "button1错误");
                button1.Enabled = true;
            }
        }

        int RepeatNum = 0;//重复文件个数
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => 
            FileDuplicateRemoval()
            );
        }

        List<string> names = new List<string>();//移动后文件名
        /// <summary>
        /// 重复文件重命名
        /// </summary>
        /// <param name="path">指定文件夹</param>
        /// <param name="name">初始文件名</param>
        /// <returns></returns>
        string RepeatRename(string path, FileInfo file)
        {
            //if (file.Name.Equals("items_CN.txt"))
            //{

            //}
            int i = 0;//重复文件重命名编号
            //当前文件夹下文件列表
            //DirectoryInfo fdir = new DirectoryInfo(path);
            //FileInfo[] CurrentFile = fdir.GetFiles();
            //foreach (FileInfo currentfile in CurrentFile)
            //{
            //    names.Add(currentfile.Name);
            //}

            string name = "";
            name += file.Name.Substring(0, file.Name.Length);
            string Extension = "";
            Extension += file.Extension.Substring(0, file.Extension.Length);
            //等待移动结束
            //int n = 0;
            //while (n < 500)
            //{
            //    if (CurrentFile.Length != RepeatNum)
            //    {
            //        Thread.Sleep(10);
            //    }
            //    else
            //    {
            //        if (n > 0)
            //            log(string.Format("等待{0}次", n));
            //        break;
            //    }
            //    n++;
            //}

            if (names.FindIndex(o => o.Equals(name)) != -1)
            {
                //已有重名文件
                int lenght = file.Name.Length - file.Extension.Length;

                //name.Substring(start, lenght) + "(" + i + ")" + file.Extension
                //"(" + i + ")" + name
                i = 0;
                while (names.FindIndex(o => o == name.Substring(0, lenght) + "(" + i + ")" + Extension) != -1)
                {
                    i++;
                }
                name = name.Substring(0, lenght) + "(" + i + ")" + Extension;
            }
            names.Add(name);
            return Path.Combine(path, name);
        }

        public static bool isValidFileContent(string filePath1, string filePath2)
        {
            //创建一个哈希算法对象
            using (HashAlgorithm hash = HashAlgorithm.Create())
            {
                using (FileStream file1 = new FileStream(filePath1, FileMode.Open), file2 = new FileStream(filePath2, FileMode.Open))
                {
                    byte[] hashByte1 = hash.ComputeHash(file1);//哈希算法根据文本得到哈希码的字节数组
                    byte[] hashByte2 = hash.ComputeHash(file2);
                    string str1 = BitConverter.ToString(hashByte1);//将字节数组装换为字符串
                    string str2 = BitConverter.ToString(hashByte2);
                    return (str1 == str2);//比较哈希码
                }
            }
        }
        public string GetHash(string filePath)
        {
            //创建一个哈希算法对象
            using (HashAlgorithm hash = HashAlgorithm.Create())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open))
                {
                    byte[] hashByte = hash.ComputeHash(file);//哈希算法根据文本得到哈希码的字节数组
                    string str = BitConverter.ToString(hashByte);//将字节数组装换为字符串
                    return str;//返回哈希码
                }
            }
        }
        public string GetHash(FileStream file)
        {
            //创建一个哈希算法对象
            using (HashAlgorithm hash = HashAlgorithm.Create())
            {
                using (file)
                {
                    byte[] hashByte = hash.ComputeHash(file);//哈希算法根据文本得到哈希码的字节数组
                    string str = BitConverter.ToString(hashByte);//将字节数组装换为字符串
                    return str;//返回哈希码
                }
            }
        }

        /// <summary>
        /// 获取md5值
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        public string GetMD5HashFromFile(FileStream file)
        {
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="name"></param>
        public static void log(string content, string time = "")
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "log";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (string.IsNullOrEmpty(time))
                path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            else
                path = path + "\\" + time + ".txt";
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }
            if (File.Exists(path))
            {
                StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.UTF8);
                sw.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff ") + content);
                sw.Close();
            }
        }
        public partial class FileGet
        {
            public static List<FileInfo> lst = new List<FileInfo>();
            public static List<string> PathList = new List<string>();
            /// <summary>
            /// 获得目录下所有文件或指定文件类型文件(包含所有子文件夹)
            /// </summary>
            /// <param name="path">文件夹路径</param>
            /// <param name="extName">扩展名可以多个 例如 .mp3.wma.rm</param>
            /// <returns>List<FileInfo></returns>
            public static void getFile(string path, string extName)
            {
                lst.Clear();
                getdir(path, extName);
            }
            /// <summary>
            /// 递归获取指定类型文件,包含子文件夹
            /// </summary>
            /// <param name="path">文件夹路径</param>
            /// <param name="extName">扩展名可以多个 例如 .mp3.wma.rm</param>
            public static void getdir(string path, string extName)
            {
                try
                {
                    string[] dir = Directory.GetDirectories(path); //文件夹列表
                    DirectoryInfo fdir = new DirectoryInfo(path);
                    FileInfo[] file = fdir.GetFiles();
                    //FileInfo[] file = Directory.GetFiles(path); //文件列表
                    if (file.Length != 0 || dir.Length != 0) //当前目录文件或文件夹不为空
                    {
                        foreach (FileInfo f in file) //显示当前目录所有文件
                        {
                            if (lst.FindIndex(o => o == f) != -1)//防止录入重复文件
                            {
                                continue;
                            }
                            if (string.IsNullOrEmpty(extName))
                            {
                                lst.Add(f);
                            }
                            else if (extName.ToLower().IndexOf(f.Extension.ToLower()) >= 0 && !string.IsNullOrEmpty(f.Extension))
                            {
                                lst.Add(f);
                            }
                        }
                        foreach (string d in dir)
                        {
                            getdir(d, extName);//递归
                        }
                    }
                }
                catch (Exception ex)
                {
                    log("getdir错误：" + ex.Message);
                    throw ex;
                }
            }

            public static void getpath(string path)
            {
                FileGet.PathList.Clear();
                getpath1(path);
            }


            public static void getpath1(string path)
            {
                try
                {
                    PathList.Add(path);//递归时把自己加入列表中
                    string[] dir = Directory.GetDirectories(path); //文件夹列表
                    DirectoryInfo fdir = new DirectoryInfo(path);
                    FileInfo[] file = fdir.GetFiles();
                    //FileInfo[] file = Directory.GetFiles(path); //文件列表
                    foreach (string d in dir)
                    {
                        PathList.Add(d);
                        getpath1(d);//递归
                    }
                }
                catch (Exception ex)
                {
                    log("getdir错误：" + ex.Message);
                    throw ex;
                }
            }


            /// <summary>
            /// 递归获取指定类型文件,包含子文件夹
            /// </summary>
            /// <param name="path">文件夹路径</param>
            /// <param name="extName">扩展名可以多个 例如 .mp3.wma.rm</param>
            public static void getdirOnlyHere(string path, string extName)
            {
                try
                {
                    string[] dir = Directory.GetDirectories(path); //文件夹列表
                    DirectoryInfo fdir = new DirectoryInfo(path);
                    FileInfo[] file = fdir.GetFiles();
                    //FileInfo[] file = Directory.GetFiles(path); //文件列表
                    if (file.Length != 0 || dir.Length != 0) //当前目录文件或文件夹不为空
                    {
                        foreach (FileInfo f in file) //显示当前目录所有文件
                        {
                            if (string.IsNullOrEmpty(extName))
                            {
                                lst.Add(f);
                            }
                            else if (extName.ToLower().IndexOf(f.Extension.ToLower()) >= 0 && !string.IsNullOrEmpty(f.Extension))
                            {
                                lst.Add(f);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log("getdir错误：" + ex.Message);
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 删除空文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteNullFile(string path, string time = "")
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path); //文件夹列表
                //为了先删除子目录再删除父目录，数组先排序再倒序，让子目录排在父目录前面
                Array.Sort(dirs);
                Array.Reverse(dirs);
                foreach (var dir in dirs)
                {
                    var info = new DirectoryInfo(dir);

                    //检查是否包含子文件夹及文件
                    if (info.GetFileSystemInfos().Length == 0)
                    {
                        //由于子文件夹或文件随时会增加，不强制删除子文件及子文件夹
                        info.Delete();
                        log("删除文件夹目录：" + dir, time);
                    }
                }
            }
            catch (Exception ex)
            {
                log("删除空文件夹错误：" + ex.Message, time);
                MessageBox.Show(ex.Message, "删除空文件夹错误");
                throw ex;
            }
        }
        /// <summary>
        /// 遍历删除空文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteNullFileRecursion(string path, string time = "")
        {
            FileGet.getpath(path);
            for (int i = FileGet.PathList.Count - 1; i >= 0; i--)//倒叙，从最底层子文件夹删起
            {
                DeleteNullFile(FileGet.PathList[i], time);
            }
        }

        /// <summary>
        /// 还原文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_recovery_Click(object sender, EventArgs e)
        {
            try
            {
                button_recovery.Text = "正在还原";
                button_recovery.Enabled = false;
                int index = 0;
                int NoFileExists = 0;
                int ErrorNum = 0;
                int SuccessNum = 0;
                Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < historicalPaths.Count; i++)
                    {
                        try
                        {
                            progressBar1.Maximum = historicalPaths.Count;
                            if (File.Exists(historicalPaths[i].newPath))
                            {
                                FileInfo file = new FileInfo(historicalPaths[i].newPath);
                                file.MoveTo(historicalPaths[i].oldPath);
                                log(historicalPaths[i].newPath + " 已还原到：" + historicalPaths[i].oldPath);
                                SuccessNum++;
                            }
                            else
                            {
                                log(historicalPaths[i].newPath + " 文件不存在！");
                                NoFileExists++;
                            }
                            progressBar1.Value = ++index;
                        }
                        catch (Exception ex)
                        {
                            progressBar1.Value = ++index;
                            ErrorNum++;
                            log("错误：" + ex);
                        }
                        string percentage = string.Format("{0:F2}", (((double)progressBar1.Value / progressBar1.Maximum) * 100));
                        if (progressBar1.Value == progressBar1.Maximum)
                            percentage = "100";
                        label1.Text = progressBar1.Value + "/" + progressBar1.Maximum + " " + percentage + "% 成功还原：" + SuccessNum + " 不存在文件：" + NoFileExists + " 错误文件：" + ErrorNum;
                    }
                    button_recovery.Enabled = true;
                    button_recovery.Text = "即时还原";
                    DeleteNullFileRecursion(AppDomain.CurrentDomain.BaseDirectory);
                });
            }
            catch (Exception ex)
            {
                log("即时还原错误：" + ex.Message);
                button_recovery.Text = "即时还原";
                MessageBox.Show(ex.Message, "即时还原错误");
                button_recovery.Enabled = true;
            }
        }

        /// <summary>
        /// 日志还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_recoveryLog_Click(object sender, EventArgs e)
        {
            try
            {
                button_recoveryLog.Enabled = false;
                OpenFileDialog openfile = new OpenFileDialog();
                //初始显示文件目录
                openfile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //过滤文件类型
                openfile.Filter = "文本文件|*.txt|可执行文件|*.exe|STOCK|STOCK.txt|所有文件类型|*.*";
                if (DialogResult.OK == openfile.ShowDialog())
                {
                    button_recoveryLog.Text = "正在还原";
                    int SuccessNum = 0;
                    int NoFileExists = 0;
                    int AllfileNum = 0;
                    List<string> Allfile = new List<string>();
                    StreamReader sr = new StreamReader(openfile.FileName, Encoding.Default);
                    string[] Separator = { " 已移动到： " };
                    string timestr = "21:03:53.683 ";
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            string content;
                            while ((content = sr.ReadLine()) != null)
                            {
                                if (content.Contains(Separator[0]))
                                {
                                    Allfile.Add(content);
                                    AllfileNum++;
                                }
                            }
                            sr.Close();

                            int index = 0;
                            progressBar1.Value = 0;

                            foreach (string str in Allfile)
                            {
                                progressBar1.Maximum = Allfile.Count;
                                string[] MovePath = str.Substring(timestr.Length, str.Length - timestr.Length).Split(Separator, StringSplitOptions.RemoveEmptyEntries);

                                if (MovePath.Length == 2)
                                {
                                    if (File.Exists(MovePath[1]))
                                    {
                                        FileInfo file = new FileInfo(MovePath[1]);
                                        file.MoveTo(MovePath[0]);
                                        SuccessNum++;
                                    }
                                    else
                                        NoFileExists++;
                                    //log("新路径: " + MovePath[1]);
                                    //log("旧路径: " + MovePath[0]);
                                }
                                progressBar1.Value = ++index;
                                string percentage = string.Format("{0:F2}", (((double)progressBar1.Value / progressBar1.Maximum) * 100));
                                if (progressBar1.Value == progressBar1.Maximum)
                                    percentage = "100";
                                label1.Text = index + "/" + progressBar1.Maximum +" "+ percentage + "% 成功还原：" + SuccessNum + " 不存在文件：" + NoFileExists;
                            }
                            button_recoveryLog.Text = "日志还原";
                            button_recoveryLog.Enabled = true;
                        }
                        catch (Exception ex)
                        {
                            log("日志还原循环错误：" + ex);
                        }
                        DeleteNullFileRecursion(AppDomain.CurrentDomain.BaseDirectory);
                    });
                }
                else
                {
                    button_recoveryLog.Enabled = true;
                    button_recoveryLog.Text = "日志还原";
                }
            }
            catch (Exception ex)
            {
                button_recoveryLog.Enabled = true;
                button_recoveryLog.Text = "日志还原";
                log("日志还原错误：" + ex.Message);
                MessageBox.Show(ex.Message, "日志还原错误");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
#if true
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择去重文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                else
                {
                    textBox1.Text = dialog.SelectedPath;
                    button1_Click(null, null);
                }
            }
#else

            //FileGet.getdir(textBox1.Text, textBox2.Text);
            //textBox1.Text = @"C:\Users\Administrator\Desktop\新建文本文档.txt";
            //string md5 = GetMD5HashFromFile(textBox1.Text);
            //string Hash = GetHash(textBox1.Text);
            //string sha256 = general_sha256_code(textBox1.Text, Sha26ParseType.StreamType); 

            List<string> FilePathAllList = new List<string>();//防止输入重复路径
            string FilePath = textBox1.Text;
            string Format = textBox2.Text;
            if (Multifile.Checked)
            {
                char[] Separator = { '+' };
                string[] FilePathAll = FilePath.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                if (FilePathAll.Length > 0)
                {
                    FileGet.lst.Clear();
                    foreach (string str in FilePathAll)
                    {
                        if (FilePathAllList.FindIndex(o => (IsSubPathOf(o, str) || IsSubPathOf(str, o)))==-1)
                        {
                            FilePathAllList.Add(str);
                            if (DirOnlyHere)
                                FileGet.getdirOnlyHere(str, Format);
                            else
                                FileGet.getdir(str, Format);
                        }
                    }

                    FilePath = "";
                    foreach (string str in FilePathAllList)
                    {
                        FilePath += str + "+";
                    }
                    textBox1.Text = FilePath.Substring(0, FilePath.Length - 1);
                }
            }
#endif
        }

        #region 获取sha256  
        public enum Sha26ParseType
        {
            StringType,
            StreamType
        }
        /// <summary>
        /// 获取sha256
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string general_sha256_code(string str, Sha26ParseType type)
        {
            string result = string.Empty;
            byte[] by = null;
            //求字节流的SHA256
            if (type.Equals(Sha26ParseType.StreamType))
            {
                if (!System.IO.File.Exists(str))
                    return result;

                System.IO.FileStream stream = new System.IO.FileStream(str, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.Security.Cryptography.SHA256Managed Sha256 = new System.Security.Cryptography.SHA256Managed();
                by = Sha256.ComputeHash(stream);
                stream.Close();
            }
            //求字符串的SHA256
            else
            {
                byte[] SHA256Data = Encoding.UTF8.GetBytes(str);

                System.Security.Cryptography.SHA256Managed Sha256 = new System.Security.Cryptography.SHA256Managed();
                by = Sha256.ComputeHash(SHA256Data);
            }

            result = BitConverter.ToString(by).Replace("-", "").ToLower(); //64
            //return Convert.ToBase64String(by);                         //44

            return result;
        }
        public static string general_sha256_code(FileStream stream)
        {
            byte[] by = null;
            System.Security.Cryptography.SHA256Managed Sha256 = new System.Security.Cryptography.SHA256Managed();
            by = Sha256.ComputeHash(stream);
            stream.Close();
            return BitConverter.ToString(by).Replace("-", "").ToLower();
        }
        #endregion
        /// <summary>
        /// 判断一个路径是否是另一个路径的子目录
        /// </summary>
        /// <param name="potentialChildPath"></param>
        /// <param name="potentialParentPath"></param>
        /// <returns></returns>
        public static bool IsSubPathOf(string potentialChildPath, string potentialParentPath)
        {
            // 确保路径是完全限定的
            if (!Path.IsPathRooted(potentialChildPath) || !Path.IsPathRooted(potentialParentPath))
            {
                throw new ArgumentException("Both paths must be fully qualified.");
            }

            // 转换路径为DirectoryInfo对象
            DirectoryInfo childDir = new DirectoryInfo(potentialChildPath);
            DirectoryInfo parentDir = new DirectoryInfo(potentialParentPath);

            // 递归向上查找，直到找到根目录或与潜在父目录匹配
            while (childDir != null)
            {
                if (childDir.FullName == parentDir.FullName)
                {
                    return true;
                }
                childDir = childDir.Parent;
            }

            return false;
        }
        bool DirOnlyHere = false;
        private void button3_Click(object sender, EventArgs e)
        {
            DirOnlyHere = true;
            button1_Click(null, null);
        }

        private void button_SQLempty_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("是否清空数据库？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    button_SQLempty.Enabled = false;
                    SQLiteHelper.ExecuteSql("delete from file");//清空数据库
                    SQLiteHelper.ExecuteSql("vacuum");//数据库收缩指令
                    button_SQLempty.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                log(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private void button_SQLAuditFile_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("是否去除无效文件？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        button_SQLAuditFile.Enabled = false;
                        var data = SQLiteHelper.Query("select path,size,md5,Hash,SHA256,LastWriteTime from file");
                        int index = 0;
                        int nobe = 0;
                        int timediff = 0;
                        int OK = 0;

                        ArrayList SQLStringList = new ArrayList();
                        SQLiteHelper.ExecuteSqlTran(SQLStringList);

                        for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                        {
                            progressBar1.Maximum = data.Tables[0].Rows.Count;
                            string path = data.Tables[0].Rows[i].Field<string>("path");
                            string LastWriteTime = data.Tables[0].Rows[i].Field<string>("LastWriteTime");
                            //log(path);
                            //log(LastWriteTime);
                            if (File.Exists(path))
                            {
                                FileInfo file = new FileInfo(path);
                                if (!(file.LastWriteTime.ToString() == LastWriteTime))
                                {
                                    //时间不匹配
                                    SQLStringList.Add($"delete from file where path = '{path}'");//删除数据
                                    timediff++;
                                }
                                else
                                {
                                    OK++;
                                }
                            }
                            else
                            {
                                //文件不存在
                                SQLStringList.Add($"delete from file where path = '{path}'");//删除数据
                                nobe++;
                            }
                            progressBar1.Value = ++index;
                            string percentage = string.Format("{0:F2}", (((double)progressBar1.Value / progressBar1.Maximum) * 100));
                            if (progressBar1.Value == progressBar1.Maximum)
                                percentage = "100";
                            label1.Text = $"{progressBar1.Value}/{progressBar1.Maximum} {percentage}% 文件正常：{OK} 时间不匹配：{timediff} 文件不存在：{nobe}";
                        }
                        SQLiteHelper.ExecuteSqlTran(SQLStringList);
                        button_SQLAuditFile.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        log(ex.ToString());
                        MessageBox.Show(ex.Message);
                    }
                }
            );
            }
        }
        Features sqlquery(Features features,string time,bool md5Open, bool HashOpen, bool sha256Open)
        {
            //查找数据库数据
            var data = SQLiteHelper.Query(string.Format("select path,size,md5,Hash,SHA256,LastWriteTime from file where path = '{0}'", features.path));
            //log($"查找数据库用时：{(DateTime.Now - start).TotalSeconds}s", time);
            if (data.Tables[0].Rows.Count != 1)//数据库不存在数据或有重复数据
            {
                if (data.Tables[0].Rows.Count > 1)//有重复数据
                {
                    log($"数据库有重复数据：{features.path}", time);
                    SQLiteHelper.ExecuteSql($"delete from file where path = '{features.path}'");//删除数据
                }
                log($"数据库无数据：{features.path}", time);
                var start = DateTime.Now;
                //当前文件的md5和hash
                if (md5Open)
                    features.md5 = GetMD5HashFromFile(features.path);
                if (HashOpen)
                    features.Hash = GetHash(features.path);
                if (sha256Open)
                {
                    features.SHA256 = general_sha256_code(features.path, Sha26ParseType.StreamType);
                }
                //log($"计算哈希值用时：{(DateTime.Now - start).TotalSeconds}s", time);

                string sql = $"insert into file(path,size,md5,Hash,SHA256,LastWriteTime) values('{features.path}','{features.size}','{features.md5}','{features.Hash}','{features.SHA256}','{features.LastWriteTime}')";
                //log($"path={path}, size={file.Length}, md5={md5}, Hash={Hash}, SHA256={sha256}, LastWriteTime={file.LastWriteTime}", time);
                SQLiteHelper.ExecuteSql(sql);//添加数据到数据库
                                             //log($"写入数据库用时：{(DateTime.Now - start).TotalSeconds}s", time);
            }
            else//数据库存在数据，从数据库中提取数据
            {
                var start = DateTime.Now;
                if (data.Tables[0].Rows[0].Field<string>("size") == features.size.ToString() &&
                    (data.Tables[0].Rows[0].Field<string>("LastWriteTime")) == features.LastWriteTime.ToString())//判断更新时间和大小是否匹配
                {
                    log($"数据库存在数据：{features.path}", time);
                    features.md5 = data.Tables[0].Rows[0].Field<string>("md5");
                    features.Hash = data.Tables[0].Rows[0].Field<string>("Hash");
                    features.SHA256 = data.Tables[0].Rows[0].Field<string>("SHA256");
                }
                else
                {
                    log($"数据库更新数据：{features.path}", time);
                    //当前文件的md5和hash
                    if (md5Open)
                        features.md5 = GetMD5HashFromFile(features.path);
                    if (HashOpen)
                        features.Hash = GetHash(features.path);
                    if (sha256Open)
                        features.SHA256 = general_sha256_code(features.path, Sha26ParseType.StreamType);
                    //更新数据
                    //SQLiteHelper.ExecuteSql($"update file SET size = '{file.Length}', md5 = '{md5}' , Hash = '{Hash}' , SHA256 = '{sha256}', LastWriteTime ='{file.LastWriteTime}' WHERE path = '{path}'");
                    ArrayList SQLStringList = new ArrayList();
                    SQLStringList.Add($"delete from file where path = '{features.path}'");//删除数据
                    SQLStringList.Add($"insert into file(path,size,md5,Hash,SHA256,LastWriteTime) values('{features.path}','{features.size}','{features.md5}','{features.Hash}','{features.SHA256}','{features.LastWriteTime}')");//添加数据到数据库
                    SQLiteHelper.ExecuteSqlTran(SQLStringList);
                }
                //log($"从数据库读取用时：{(DateTime.Now - start).TotalSeconds}s", time);
            }
            return features;
        }
    }
}
