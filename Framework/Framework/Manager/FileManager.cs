using System;
using System.IO;
using Framework.Core;
using Framework.Core.Utility;

namespace Framework.Manager
{

    public interface IFileUtility
    {
        string Read(string path);
        byte[] ReadBytes(string path);
        void Write(string path, string content);
        void WriteBytes(string path, byte[] bytes);
    }

    /// <summary>
    /// 文件类
    /// </summary>
    public class FileManager : ManagerBase, IFileUtility
    {
        private LogUtility _log;
        protected override void OnInit()
        {
            _log = GetUtility<LogUtility>();
        }

        /// <summary>
        /// 读取文件 返回字符串
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Read(string path)
        {
            if (!CheckFile(path))
                return "";

            return File.ReadAllText(path);
        }

        /// <summary>
        /// 读取文件 返回字节数组
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] ReadBytes(string path)
        {
            if (!CheckFile(path))
                return null;

            return File.ReadAllBytes(path);
        }

        private bool CheckFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                _log.LogError("路径为空");
                return false;
            }

            if (!File.Exists(path))
            {
                _log.LogError(path + "文件不存在");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public void Write(string path, string content)
        {
            if (!CheckDirectory(path))
                return;
            try
            {
                File.WriteAllText(path, content);
            }
            catch (Exception e)
            {
                _log.LogError("WriteBytes error:" + path);
                _log.LogError(e.Message);
            }
        }

        /// <summary>
        /// 写入字节
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        public void WriteBytes(string path, byte[] bytes)
        {
            if (!CheckDirectory(path))
                return;
            try
            {
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception e)
            {
                _log.LogError("WriteBytes error:" + path);
                _log.LogError(e.Message);
            }
        }

        private bool CheckDirectory(string path)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(directory))
                {
                    _log.LogError("目录为空");
                    return false;
                }
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception e)
            {
                _log.LogError("CheckDirectory error:" + path);
                _log.LogError(e.Message);
                return false;
            }

            return Directory.Exists(path);
        }
    }
}

