namespace LinguaSpace.Words.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu = new System.Windows.Forms.MainMenu();
            this.menuItemLeft = new System.Windows.Forms.MenuItem();
            this.menuItemRight = new System.Windows.Forms.MenuItem();
            this.menuItemSync = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.Add(this.menuItemLeft);
            this.mainMenu.MenuItems.Add(this.menuItemRight);
            // 
            // menuItemLeft
            // 
            this.menuItemLeft.Text = "Next";
            this.menuItemLeft.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItemRight
            // 
            this.menuItemRight.MenuItems.Add(this.menuItemSync);
            this.menuItemRight.Text = "Vocabularies";
            // 
            // menuItemSync
            // 
            this.menuItemSync.Enabled = false;
            this.menuItemSync.Text = "&Synchronize";
            this.menuItemSync.Click += new System.EventHandler(this.menuItemSync_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 266);
            this.KeyPreview = true;
            this.Menu = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "LinguaSpace Words 3.0.0";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItemLeft;
        private System.Windows.Forms.MenuItem menuItemRight;
        private System.Windows.Forms.MenuItem menuItemSync;
    }
}

