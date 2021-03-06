﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Chen.CommonLibrary
{
    public class FileHelper
    {
        public void SaveFile(string file, string text)
        {
            if (file.Contains(@"/"))
            {
                CreateFolder(file.Remove(file.LastIndexOf(@"/")));
            }
            if (file.Contains(@"\"))
            {
                CreateFolder(file.Remove(file.LastIndexOf(@"\")));
            }
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(text);
            sw.Close();
        }
        public void SaveFile(string file, byte[] bytes)
        {
            if (file.Contains(@"/"))
            {
                CreateFolder(file.Remove(file.LastIndexOf(@"/")));
            }
            if (file.Contains(@"\"))
            {
                CreateFolder(file.Remove(file.LastIndexOf(@"\")));
            }
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            File.WriteAllBytes(file, bytes);
        }
        public void CreateFolder(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }
        public byte[] readFileByte(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            FileStream file = new FileStream(path, FileMode.Open);
            byte[] byData = new byte[Convert.ToInt32(file.Length)];
            file.Seek(0, SeekOrigin.Begin);
            file.Read(byData, 0, byData.Length);
            file.Close();
            return byData;
        }
        public string readFile(string path,Encoding encode=null)
        {
            if (!File.Exists(path))
            {
                return "";
            }
            FileStream file = new FileStream(path, FileMode.Open);
            byte[] byData = new byte[Convert.ToInt32(file.Length)];
            file.Seek(0, SeekOrigin.Begin);
            file.Read(byData, 0, byData.Length);
            if (encode == null)
            {
                encode = Encoding.UTF8;
            }
            string result = encode.GetString(byData);
            file.Close();
            return result;
        }
        public string[] readFileLine(string path)
        {
            List<string> Result = new List<string>();
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            var streamReader = new StreamReader(fileStream, Encoding.Default);

            fileStream.Seek(0, SeekOrigin.Begin);
            string content = streamReader.ReadLine();
            while (content != null)
            {
                if (!string.IsNullOrEmpty(content.Trim()))
                {
                    Result.Add(content);
                }
                content = streamReader.ReadLine();
            }
            return Result.ToArray();
        }
        /// <summary>
        /// 获得一个目录下的所有文件(包含子目录)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filterEx">过滤文件后缀名类型</param>
        /// <returns></returns>
        public List<FileInfo> GetAllFile(string path, string filterEx = null)
        {
            List<FileInfo> result = new List<FileInfo>();
            DirectoryInfo dir = new DirectoryInfo(path);
            var files = dir.GetFiles();
            if (!string.IsNullOrEmpty(filterEx))
            {
                files = files.Where(i => i.Extension == filterEx).ToArray();
            }

            result.AddRange(files);

            DirectoryInfo[] dii = dir.GetDirectories();
            foreach (var dinfo in dii)
            {
                result.AddRange(GetAllFile(dinfo.FullName, filterEx));
            }
            return result;
        }

        public bool CopyAll(string sourcePath, string savePath,Func<string,bool> filter=null)
        {
            if (filter == null)
            {
                filter = x => {
                    return true;
                };
            }
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            try
            {
                string[] labDirs = Directory.GetDirectories(sourcePath);//目录
                string[] labFiles = Directory.GetFiles(sourcePath);//文件
                if (labFiles.Length > 0)
                {
                    for (int i = 0; i < labFiles.Length; i++)
                    {
                        if (filter(labFiles[i]))//执行Filter
                        {
                            File.Copy(sourcePath + "\\" + Path.GetFileName(labFiles[i]), savePath + "\\" + Path.GetFileName(labFiles[i]), true);
                        }
                    }
                }
                if (labDirs.Length > 0)
                {
                    for (int j = 0; j < labDirs.Length; j++)
                    {
                        Directory.GetDirectories(sourcePath + "\\" + Path.GetFileName(labDirs[j]));

                        //递归调用
                        this.CopyAll(sourcePath + "\\" + Path.GetFileName(labDirs[j]), savePath + "\\" + Path.GetFileName(labDirs[j]), filter);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
