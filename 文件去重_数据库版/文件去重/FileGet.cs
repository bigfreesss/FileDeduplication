using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static 文件去重.Form1;

namespace 文件去重
{
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
}
