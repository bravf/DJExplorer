using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace DJExplorer
{
    public partial class MainForm : Form
    {
        string targetDir = @"\\fe.baidu.com\zhangdongdong02\public_html";
        string nppPath = @"C:\Program Files\Notepad++\notepad++.exe";

        ContextMenuStrip treeContextMenu;

        public MainForm()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            initFormRect();
            initTreeImages();
            addNode(fileTree, buildNode(targetDir));
            initNpp();
            initContextMenu();
        }

        // 设置窗口大小
        private void initFormRect()
        {
            Rectangle win = SystemInformation.WorkingArea;
            this.Height = win.Height;
            this.Location = new Point(win.Width - this.Width, 0);
        }

        private void initTreeImages()
        {
            ImageList images = new ImageList();
            images.Images.Add("file" ,Image.FromFile("Images/file.jpg"));
            images.Images.Add("folder", Image.FromFile("Images/folder.jpg"));
            fileTree.ImageList = images;
        }

        // 右键菜单
        private void initContextMenu()
        {
            treeContextMenu = new ContextMenuStrip();

            ToolStripMenuItem updateItem = new ToolStripMenuItem("更新");
            updateItem.Click += new EventHandler(updateItem_Click);

            ToolStripMenuItem deleteItem = new ToolStripMenuItem("删除");
            deleteItem.Click += new EventHandler(deleteItem_Click);

            treeContextMenu.Items.Add(updateItem);
            treeContextMenu.Items.Add(deleteItem);
        }

        void updateItem_Click(object sender, EventArgs e)
        {
            DJNode selectNode = fileTree.SelectedNode as DJNode;
            if (selectNode == null) return;
            if (selectNode.isFile == true) return;

            int idx = selectNode.Index;
            DJNode parent = selectNode.Parent as DJNode;

            DJNode newNode = buildNode(selectNode.filePath);
            selectNode.Remove();
            parent.Nodes.Insert(idx, newNode);
            fileTree.SelectedNode = newNode;
        }

        void deleteItem_Click(object sender, EventArgs e)
        {
            DJNode selectNode = fileTree.SelectedNode as DJNode;
            if (selectNode == null) return;
            selectNode.Remove();
        }

        // 启动npp程序
        private void initNpp()
        {
            if (Process.GetProcessesByName("notepad++").Length == 0) Process.Start(nppPath);
        }

        private void openFile(string filePath)
        {
            Process.Start(nppPath, filePath);
        }

        private void addNode(DJNode parent, DJNode child)
        {
            if (parent != null && child != null) parent.Nodes.Add(child);
        }

        private void addNode(TreeView parent, DJNode child)
        {
            if (parent != null && child != null) parent.Nodes.Add(child);
        }

        // 文件是否具有隐藏属性
        private bool isHiddenFile(string filePath)
        {
            bool isHidden = false;
            FileAttributes attr = File.GetAttributes(filePath);
            if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden) isHidden = true;
            return isHidden;
        }

        // 创建节点
        private DJNode buildNode(string path)
        {
            if (isHiddenFile(path)) return null;
            if (File.Exists(path))
            {
                FileInfo file = new FileInfo(path);
                DJNode Fnode = new DJNode(file.Name, file.FullName, true);
                return Fnode;
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                DJNode DNode = new DJNode(dir.Name, dir.FullName, false);

                DirectoryInfo[] dirs = dir.GetDirectories();
                for (int i = 0, len = dirs.Length; i < len; i++)
                {
                    addNode(DNode, buildNode(dirs[i].FullName));
                }
                FileInfo[] files = dir.GetFiles();
                for (int i = 0, len = files.Length; i < len; i++)
                {
                    addNode(DNode, buildNode(files[i].FullName));
                }

                return DNode;
            }
        }

        // 双击节点如果是文件则打开
        private void fileTree_DoubleClick(object sender, EventArgs e)
        {
            TreeView tree = sender as TreeView;
            DJNode selectNode = tree.SelectedNode as DJNode;
            if (selectNode == null) return;
            if (selectNode.isFile == false) return;
            openFile(selectNode.filePath);
        }

        private void fileTree_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        // 拖拽增加节点
        private void fileTree_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int i = 0, len = files.Length; i < len; i++)
            {
                string file = files[i];
                addNode(fileTree, buildNode(file));
            }
        }

        // 节点右键
        private void fileTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            Point clickPoint = new Point(e.X, e.Y);
            TreeNode node = fileTree.GetNodeAt(clickPoint);
            if (node == null) return;

            node.ContextMenuStrip = treeContextMenu;
            fileTree.SelectedNode = node;
        }
    }

    public class DJNode:TreeNode
    {
        public string filePath;

        private bool _isFile = false;
        public bool isFile
        {
            get { return _isFile; }
            set
            {
                _isFile = value;
                string key = "file";
                if (_isFile == false) key = "folder";
                this.ImageKey = this.SelectedImageKey = key;
            }
        }

        public DJNode(string _text, string _filePath, bool _isFile)
        {
            this.Text = _text;
            this.isFile = _isFile;
            filePath = _filePath;
        }
    }
}
