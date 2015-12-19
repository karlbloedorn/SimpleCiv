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
            this.elementHost2 = new System.Windows.Forms.Integration.ElementHost();
            this.bottomRightMenu1 = new SimpleCiv.BottomRightMenu();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.topMenu1 = new SimpleCiv.TopMenu();
            this.elementHost3 = new System.Windows.Forms.Integration.ElementHost();
            this.mainMenu1 = new SimpleCiv.views.MainMenu();
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
            // elementHost2
            // 
            this.elementHost2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost2.AutoSize = true;
            this.elementHost2.Location = new System.Drawing.Point(537, 269);
            this.elementHost2.Name = "elementHost2";
            this.elementHost2.Size = new System.Drawing.Size(300, 240);
            this.elementHost2.TabIndex = 2;
            this.elementHost2.TabStop = false;
            this.elementHost2.Text = "elementHost2";
            this.elementHost2.Child = this.bottomRightMenu1;
            this.elementHost2.Visible = false;
            // 
            // elementHost1
            // 
            this.elementHost1.AutoSize = true;
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Top;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(837, 28);
            this.elementHost1.TabIndex = 1;
            this.elementHost1.TabStop = false;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.topMenu1;
            this.elementHost1.Visible = false;
            // 
            // elementHost3
            // 
            this.elementHost3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost3.Location = new System.Drawing.Point(0, 28);
            this.elementHost3.Name = "elementHost3";
            this.elementHost3.Size = new System.Drawing.Size(837, 441);
            this.elementHost3.TabIndex = 3;
            this.elementHost3.Text = "elementHost3";
            this.elementHost3.Child = this.mainMenu1;
            // 
            // Civ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ClientSize = new System.Drawing.Size(837, 469);
            this.Controls.Add(this.elementHost3);
            this.Controls.Add(this.elementHost2);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.overlayControl);
            this.Name = "Civ";
            this.Text = "Simple Civ";
            this.Load += new System.EventHandler(this.Civ_Load);
            ((System.ComponentModel.ISupportInitialize)(this.overlayControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private TopMenu topMenu1;
        private SharpGL.OpenGLControl overlayControl;
        private System.Windows.Forms.Integration.ElementHost elementHost2;
        private BottomRightMenu bottomRightMenu1;
        private System.Windows.Forms.Integration.ElementHost elementHost3;
        private views.MainMenu mainMenu1;
    }
}

