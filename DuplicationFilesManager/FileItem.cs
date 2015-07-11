using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuplicationFilesManager
{
    class FileItem
    {
        public string FileName 
        {
            get
            {
               return fileSetting.Name;
            }
        }
        public string FileSize
        {
            get
            {
                if (fileSetting.Length > 1048576)
                {
                    return string.Format("{0:F2}MB", fileSetting.Length / 1048576);
                }
                else if (fileSetting.Length > 1024)
                {
                    return string.Format("{0:F2}KB", fileSetting.Length / 1024);
                }

                return fileSetting.Length.ToString();
            }
        }
        public string FilePath
        {
            get
            {
                return fileSetting.FullName;
            }
        }

        public string FileSHA1 { private set; get; }

        public bool IsExist
        {
            get
            {
                return fileSetting.Exists;
            }
        }
       
        private System.IO.FileInfo fileSetting;

        public FileItem(string filePath)
        {
            fileSetting = new System.IO.FileInfo(filePath);
            FileSHA1 = ComputeSHA1(fileSetting);
        }

        public void Delete()
        {
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(fileSetting.FullName, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }

        /// <summary>
        ///  计算指定文件的SHA1值
        /// </summary>
        /// <param name="fileName">指定文件的完全限定名称</param>
        /// <returns>返回值的字符串形式</returns>
        public String ComputeSHA1(System.IO.FileInfo fileInfo)
        {
            String hashSHA1 = String.Empty;
            //检查文件是否存在，如果文件存在则进行计算，否则返回空值
            if (fileInfo.Exists)
            {
                using (System.IO.FileStream fs = fileInfo.OpenRead())
                {
                    //计算文件的SHA1值
                    System.Security.Cryptography.SHA1 calculator = System.Security.Cryptography.SHA1.Create();
                    Byte[] buffer = calculator.ComputeHash(fs);
                    calculator.Clear();
                    //将字节数组转换成十六进制的字符串形式
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        stringBuilder.Append(buffer[i].ToString("x2"));
                    }
                    hashSHA1 = stringBuilder.ToString();
                }//关闭文件流
            }
            return hashSHA1;
        }
    }
}
