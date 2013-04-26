using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace DJExplorer
{
    public partial class MainForm : Form
    {
        string nppPath = "";

        ContextMenuStrip folderContextMenu;
        ContextMenuStrip fileContextMenu;

        public MainForm()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            readConf();
            initTreeImages();
            initNpp();
            initContextMenu();
        }

        private void readConf()
        {
            string path = "conf";

            if (!File.Exists(path)) return;
            StreamReader confReader = new StreamReader(path);
            nppPath = confReader.ReadLine().Trim();
            if (!File.Exists(nppPath)) MessageBox.Show("搞毛线，指定的npp路径有问题啊~~！");

            string line;
            while ((line = confReader.ReadLine()) != null)
            {
                NodeHelper.add(fileTree, buildNode(line));
            }
            confReader.Close();
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
            // folder
            folderContextMenu = new ContextMenuStrip();

            ToolStripMenuItem folderUpdate = new ToolStripMenuItem("更新");
            folderUpdate.Click += new EventHandler(updateItem_Click);

            ToolStripMenuItem folderDelete = new ToolStripMenuItem("删除");
            folderDelete.Click += new EventHandler(deleteItem_Click);

            ToolStripMenuItem folderAdd = new ToolStripMenuItem("新建文件夹");
            folderAdd.Click += new EventHandler(folderAdd_Click);

            ToolStripMenuItem fileAdd = new ToolStripMenuItem("新建文件");
            fileAdd.Click += new EventHandler(fileAdd_Click);

            folderContextMenu.Items.Add(folderUpdate);
            folderContextMenu.Items.Add(folderDelete);
            folderContextMenu.Items.Add(folderAdd);
            folderContextMenu.Items.Add(fileAdd);

            // file
            fileContextMenu = new ContextMenuStrip();

            ToolStripMenuItem fileUpdate = new ToolStripMenuItem("更新");
            fileUpdate.Click += new EventHandler(updateItem_Click);

            ToolStripMenuItem fileDelete = new ToolStripMenuItem("移除");
            fileDelete.Click += new EventHandler(deleteItem_Click);

            ToolStripMenuItem fileOpen = new ToolStripMenuItem("打开");
            fileOpen.Click += new EventHandler(openItem_Click);


            fileContextMenu.Items.Add(fileUpdate);
            fileContextMenu.Items.Add(fileDelete);
            fileContextMenu.Items.Add(fileOpen);
        }

        void fileAdd_Click(object sender, EventArgs e)
        {
            ffAdd("新建文件", true);
        }

        void folderAdd_Click(object sender, EventArgs e)
        {
            ffAdd("新建文件夹", false);
        }

        private void ffAdd(string initText, bool isFile)
        {
            DJNode selectNode = fileTree.SelectedNode as DJNode;
            DJNode node = new DJNode(initText, selectNode.filePath, isFile);
            NodeHelper.add(selectNode, node);

            fileTree.SelectedNode = node;
            node.BeginEdit();
        }

        private void fileTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            DJNode node = fileTree.SelectedNode as DJNode;
            node.filePath = node.filePath + "/" + e.Label.Trim();
            if (node.isFile)
            {
                if (!File.Exists(node.filePath)) File.Create(node.filePath);
            }
            else
            {
                if (!Directory.Exists(node.filePath)) Directory.CreateDirectory(node.filePath);
            }
            node.EndEdit(true);
        }

        void openItem_Click(object sender, EventArgs e)
        {
            DJNode selectNode = fileTree.SelectedNode as DJNode;
            Process.Start(selectNode.filePath);
        }

        void updateItem_Click(object sender, EventArgs e)
        {
            DJNode selectNode = fileTree.SelectedNode as DJNode;
            if (selectNode == null) return;
            if (selectNode.isFile == true) return;

            int idx = selectNode.Index;
            DJNode newNode = buildNode(selectNode.filePath);

            if (selectNode.Parent == null) selectNode.TreeView.Nodes.Insert(idx, newNode);
            else selectNode.Parent.Nodes.Insert(idx, newNode);

            selectNode.Remove();

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
            if (!File.Exists(filePath))
            {
                MessageBox.Show("此文件不存在咯~~！");
                return;
            }
            Process.Start(nppPath, filePath);
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
            path = path.Trim();
            if (!File.Exists(path) && !Directory.Exists(path)) return null;
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
                    NodeHelper.add(DNode, buildNode(dirs[i].FullName));
                }
                FileInfo[] files = dir.GetFiles();
                for (int i = 0, len = files.Length; i < len; i++)
                {
                    NodeHelper.add(DNode, buildNode(files[i].FullName));
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
                NodeHelper.add(fileTree, buildNode(file));
            }
        }

        // 节点右键
        private void fileTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            Point clickPoint = new Point(e.X, e.Y);
            DJNode node = fileTree.GetNodeAt(clickPoint) as DJNode;
            if (node == null) return;

            if (node.isFile) node.ContextMenuStrip = fileContextMenu;
            else node.ContextMenuStrip = folderContextMenu;

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

    public static class NodeHelper
    {
        public static int add(TreeView tree, DJNode node)
        {
            if (tree == null || node == null) return -1;
            return tree.Nodes.Add(node);
        }

        public static int add(DJNode parent, DJNode child)
        {
            if (parent == null || child == null) return -1;
            return parent.Nodes.Add(child);
        }

    }
}
