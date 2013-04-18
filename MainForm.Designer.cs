namespace DJExplorer
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.fileTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // fileTree
            // 
            this.fileTree.AllowDrop = true;
            this.fileTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fileTree.Location = new System.Drawing.Point(0, 0);
            this.fileTree.Margin = new System.Windows.Forms.Padding(5);
            this.fileTree.Name = "fileTree";
            this.fileTree.Size = new System.Drawing.Size(192, 469);
            this.fileTree.TabIndex = 0;
            this.fileTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.fileTree_DragDrop);
            this.fileTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.fileTree_DragEnter);
            this.fileTree.DoubleClick += new System.EventHandler(this.fileTree_DoubleClick);
            this.fileTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fileTree_MouseDown);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(194, 472);
            this.Controls.Add(this.fileTree);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "DJExplorer";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView fileTree;


    }
}

