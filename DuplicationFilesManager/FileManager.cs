using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuplicationFilesManager
{
    class FileManager
    {
        private static FileManager singleton;
        public static FileManager Instance
        {
            get
            {
                lock (typeof(FileManager))
                {
                    if (singleton == null)
                        singleton = new FileManager();
                }
                return singleton;
            }
        }

        private readonly System.ComponentModel.BindingList<string> scanDirList;

        public System.ComponentModel.BindingList<string> ScanDirList
        {
            get { return scanDirList; }
        }

        private readonly BindingCollection<FileItem> duplicationFiles;

        internal BindingCollection<FileItem> DuplicationFiles
        {
            get { return duplicationFiles; }
        } 

        private readonly List<FileItem> scanFile;
        public FormMain MainForm { private set; get; }

        public bool IsScaning { private set; get; }
        private FileManager()
        {
            MainForm = new FormMain();
            scanDirList = new System.ComponentModel.BindingList<string>();
            duplicationFiles = new BindingCollection<FileItem>();
            scanFile = new List<FileItem>();
            IsScaning = false;
        }

        public void StartScan()
        {
            if (ScanDirList.Count < 1)
                return;

            IsScaning = true;
            duplicationFiles.Clear();
            System.Threading.ThreadPool.QueueUserWorkItem((O) =>
            {
                scanFile.Clear();
                foreach (var path in ScanDirList)
                {
                    GetPathFile(path);
                    if (!IsScaning)
                        break;
                }
                IsScaning = false;
                MainForm.SetButtonText();
            });
            
        }

        public void StopScan()
        {
            IsScaning = false;
            MainForm.SetStatus("停止扫描");
        }

        private void GetPathFile(string path)
        {
            MainForm.SetStatus(string.Format("正在扫描{0}", path));
            foreach (var item in System.IO.Directory.GetFiles(path))
            {
                if (!IsScaning)
                    return;

                var file = new FileItem(item);
                scanFile.Add(file);

                var existFiles = scanFile.Where(q => q.FileSHA1 == file.FileSHA1).ToList();
                if (existFiles.Count > 1)
                {
                    foreach (var existfile in existFiles)
                    {
                        if (duplicationFiles.Contains(existfile))
                            continue;

                        if (MainForm.InvokeRequired)
                        {
                            MainForm.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                duplicationFiles.Add(existfile);
                            }));
                        }
                        else
                        {
                            duplicationFiles.Add(existfile);
                        }
                    }
                }
                
            }

            foreach (var item in System.IO.Directory.GetDirectories(path))
            {
                GetPathFile(item);
            }
        }
    }
}
