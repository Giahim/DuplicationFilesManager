using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DuplicationFilesManager
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            listBox1.DataSource = FileManager.Instance.ScanDirList;
            dataGridView1.DataSource = FileManager.Instance.DuplicationFiles;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "请选择添加目录";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var path = dialog.SelectedPath;
                if (!FileManager.Instance.ScanDirList.Contains(path))
                    FileManager.Instance.ScanDirList.Add(path);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count < 1)
                return;

            var del = new List<string>();
            foreach (string item in listBox1.SelectedItems)
            {
                del.Add(item);
            }
            foreach (string item in del)
            {
                FileManager.Instance.ScanDirList.Remove(item);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (FileManager.Instance.IsScaning)
            {
                FileManager.Instance.StopScan();
            }
            else
            {
                FileManager.Instance.StartScan();
            }

            SetButtonText();
        }

        public void SetStatus(string text)
        {
            if (this.InvokeRequired) 
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    SetStatus(text);
                }));
                return;
            }
            this.Text = string.Format("重复文件扫描 - {0}", text);
        }

        public void SetButtonText()
        {
            if (this.InvokeRequired) 
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    SetButtonText();
                }));
                return;
            }
            if (FileManager.Instance.IsScaning)
            {
                button3.Text = "停止扫描";
            }
            else
            {
                button3.Text = "开始扫描";
            }
        }

        private void clearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileManager.Instance.IsScaning)
                return;

            FileManager.Instance.DuplicationFiles.Clear();
        }

        private void deleteSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count < 1)
                return;

            var rows = dataGridView1.SelectedRows;
            foreach (DataGridViewRow item in rows)
            {
                var file = item.DataBoundItem as FileItem;
                if (file == null)
                    continue;
                file.Delete();

            }
            foreach (DataGridViewRow item in rows)
            {
                dataGridView1.Rows.Remove(item);
            }
        }


    }
}
