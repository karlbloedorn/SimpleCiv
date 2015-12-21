namespace SimpleCiv
{
    partial class Civ
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.overlayControl = new SharpGL.OpenGLControl();
            this.blMenuContainer = new System.Windows.Forms.Integration.ElementHost();
            this.bottomLeftMenu1 = new SimpleCiv.views.BottomLeftMenu();
            this.mainMenuContainer = new System.Windows.Forms.Integration.ElementHost();
            this.mainMenu = new SimpleCiv.views.MainMenu();
            this.bottomRightMenuContainer = new System.Windows.Forms.Integration.ElementHost();
            this.bottomRightMenu1 = new SimpleCiv.BottomRightMenu();
            this.topMenuContainer = new System.Windows.Forms.Integration.ElementHost();
            this.topMenu1 = new SimpleCiv.TopMenu();
            ((System.ComponentModel.ISupportInitialize)(this.overlayControl)).BeginInit();
            this.SuspendLayout();
            // 
            // overlayControl
            // 
            this.overlayControl.AutoSize = true;
            this.overlayControl.BackColor = System.Drawing.Color.Black;
            this.overlayControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.overlayControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overlayControl.DrawFPS = true;
            this.overlayControl.FrameRate = 60;
            this.overlayControl.Location = new System.Drawing.Point(0, 0);
            this.overlayControl.Name = "overlayControl";
            this.overlayControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL1_1;
            this.overlayControl.Padding = new System.Windows.Forms.Padding(100, 0, 0, 0);
            this.overlayControl.RenderContextType = SharpGL.RenderContextType.NativeWindow;
            this.overlayControl.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.overlayControl.Size = new System.Drawing.Size(837, 469);
            this.overlayControl.TabIndex = 0;
            // 
            // elementHost4
            // 
            this.blMenuContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.blMenuContainer.Location = new System.Drawing.Point(0, 269);
            this.blMenuContainer.Name = "elementHost4";
            this.blMenuContainer.Size = new System.Drawing.Size(200, 200);
            this.blMenuContainer.TabIndex = 4;
            this.blMenuContainer.Text = "elementHost4";
            this.blMenuContainer.Visible = false;
            this.blMenuContainer.Child = this.bottomLeftMenu1;
            // 
            // elementHost3
            // 
            this.mainMenuContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainMenuContainer.Location = new System.Drawing.Point(0, 28);
            this.mainMenuContainer.Name = "elementHost3";
            this.mainMenuContainer.Size = new System.Drawing.Size(837, 441);
            this.mainMenuContainer.TabIndex = 3;
            this.mainMenuContainer.Text = "elementHost3";
            this.mainMenuContainer.Child = this.mainMenu;
            // 
            // elementHost2
            // 
            this.bottomRightMenuContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomRightMenuContainer.AutoSize = true;
            this.bottomRightMenuContainer.Location = new System.Drawing.Point(537, 269);
            this.bottomRightMenuContainer.Name = "elementHost2";
            this.bottomRightMenuContainer.Size = new System.Drawing.Size(300, 240);
            this.bottomRightMenuContainer.TabIndex = 2;
            this.bottomRightMenuContainer.TabStop = false;
            this.bottomRightMenuContainer.Text = "elementHost2";
            this.bottomRightMenuContainer.Visible = false;
            this.bottomRightMenuContainer.Child = this.bottomRightMenu1;
            // 
            // elementHost1
            // 
            this.topMenuContainer.AutoSize = true;
            this.topMenuContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.topMenuContainer.Location = new System.Drawing.Point(0, 0);
            this.topMenuContainer.Name = "elementHost1";
            this.topMenuContainer.Size = new System.Drawing.Size(837, 28);
            this.topMenuContainer.TabIndex = 1;
            this.topMenuContainer.TabStop = false;
            this.topMenuContainer.Text = "elementHost1";
            this.topMenuContainer.Visible = false;
            this.topMenuContainer.Child = this.topMenu1;
            // 
            // Civ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ClientSize = new System.Drawing.Size(837, 469);
            this.Controls.Add(this.blMenuContainer);
            this.Controls.Add(this.mainMenuContainer);
            this.Controls.Add(this.bottomRightMenuContainer);
            this.Controls.Add(this.topMenuContainer);
            this.Controls.Add(this.overlayControl);
            this.Name = "Civ";
            this.Text = "Simple Civ";
            this.Load += new System.EventHandler(this.Civ_Load);
            ((System.ComponentModel.ISupportInitialize)(this.overlayControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost topMenuContainer;
        private TopMenu topMenu1;
        private SharpGL.OpenGLControl overlayControl;
        private System.Windows.Forms.Integration.ElementHost bottomRightMenuContainer;
        private BottomRightMenu bottomRightMenu1;
        private System.Windows.Forms.Integration.ElementHost mainMenuContainer;
        private views.MainMenu mainMenu;
        private System.Windows.Forms.Integration.ElementHost blMenuContainer;
        private views.BottomLeftMenu bottomLeftMenu1;
    }
}

