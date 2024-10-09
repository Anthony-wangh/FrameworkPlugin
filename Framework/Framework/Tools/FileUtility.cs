using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace Framework.Tools
{
    /// <summary>
    /// 文件工具类
    /// </summary>
    public class FileUtility
    {
        /// <summary>
        /// 打开文件目录
        /// </summary>
        /// <param name="path"></param>
        public static void OpenDirectory(string path)
        {
            // 新开线程防止锁死
            path = Path.GetDirectoryName(path);
            path = $"\"{path}\"";
            Thread thread = new Thread(() =>
            {
                CmdOpenDirectory(path);
            });
            thread.Start();
            thread.Abort();
        }

        private static void CmdOpenDirectory(object obj)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c start \"\" " + obj.ToString();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.WaitForExit();
            p.Close();
        }



        /// <summary>
        /// 打开目录并选中文件(容易被杀毒软件监听成不信任)
        /// </summary>
        /// <param name="path"></param>
        public static void OpenPathAndSelect(string path)
        {
            try
            {
                path = Path.GetDirectoryName(path);
                if (path != null)
                {
                    path = path.Replace("/", "\\");
                    Process.Start("explorer.exe", path);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }

        /// <summary>
        /// 获取文件夹下面的最新文件
        /// </summary>
        public static string GetNewFile(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return "";
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            if (fileInfos.Length > 0)
            {
                return fileInfos[0].FullName;
            }

            return "";
        }

        /// <summary>
        /// 获取图片大小
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static double GetTextureSize(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            double length = Convert.ToDouble(fileInfo.Length);
            double size = length / 1024 / 1024;
            return size;
        }

        /// <summary>
        ///直接删除指定目录下的所有文件
        /// </summary>
        public static void DeleteDir(string file)
        {
            if (!Directory.Exists(file))
            {
                return;
            }
            //去除文件夹的只读属性
            DirectoryInfo fileInfo = new DirectoryInfo(file);
            fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
            //去除文件的只读属性
            File.SetAttributes(file, FileAttributes.Normal);
            //判断文件夹是否还存在
            if (Directory.Exists(file))
            {
                foreach (string f in Directory.GetFileSystemEntries(file))
                {
                    if (File.Exists(f))
                    {
                        //如果有子文件删除文件
                        File.Delete(f);
                    }
                    else
                    {
                        //循环递归删除子文件
                        DeleteDir(f);
                    }
                }
            }
        }
        /// <summary>
        /// 打开图片文件
        /// </summary>
        /// <param name="callback"></param>
        public static void OpenTextureFile(Action<string> callback)
        {
            OpenFile("图片文件(*.jpg *.png)\0 *.jpg; *.png", callback);
        }

        /// <summary>
        /// 打开所有文件
        /// </summary>
        /// <param name="callback"></param>
        public static void OpenFile(Action<string> callback)
        {
            OpenFile("All files (*.*)|*.*", callback);
        }
        /// <summary>
        /// 打开FBX文件
        /// </summary>
        /// <param name="callback"></param>
        public static void OpenModelFile(Action<string> callback)
        {
            OpenFile("FBX File(*.fbx)\0 *.fbx", callback);
        }

        public static void OpenFile(string filter, Action<string> callback)
        {
            try
            {
                OpenFileDlg openFileName = new OpenFileDlg();
                openFileName.structSize = Marshal.SizeOf(openFileName);
                openFileName.filter = filter;// "All files (*.*)|*.*";
                openFileName.file = new string(new char[256]);
                openFileName.maxFile = openFileName.file.Length;
                openFileName.fileTitle = new string(new char[64]);
                openFileName.maxFileTitle = openFileName.fileTitle.Length;
                openFileName.initialDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                openFileName.title = "打开文件";
                openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
                if (OpenFileDialog.GetOpenFileName(openFileName))
                {
                    UnityEngine.Debug.Log(openFileName.file);
                    callback(openFileName.file);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }

        /// <summary>
        /// 保存文件项目
        /// </summary>
        public static void SaveProject(string initialDir, string filter, string defExt, Action<string> callback)
        {
            try
            {
                SaveFileDlg pth = new SaveFileDlg();
                pth.structSize = Marshal.SizeOf(pth);
                pth.filter = filter;
                pth.file = new string(new char[256]);
                pth.maxFile = pth.file.Length;
                pth.fileTitle = new string(new char[64]);
                pth.maxFileTitle = pth.fileTitle.Length;
                pth.initialDir = initialDir; //默认路径

                pth.title = "保存项目";
                pth.defExt = defExt;
                pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
                if (SaveFileDialog.GetSaveFileName(pth))
                {
                    string filepath = pth.file; //选择的文件路径;  
                    callback?.Invoke(filepath);
                    UnityEngine.Debug.Log(filepath);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }

        /// <summary>
        /// 保存文件项目
        /// </summary>
        public static void SaveTextProject(Action<string> callback)
        {
            SaveProject(Application.dataPath, "*.txt*", "txt", callback);
        }
        /// <summary>
        /// 保存Fbx格式文件
        /// </summary>
        /// <param name="callback"></param>
        public static void SaveFbxProject(Action<string> callback)
        {
            SaveProject(Application.dataPath, "*.fbx*", "fbx", callback);
        }

    }
}
