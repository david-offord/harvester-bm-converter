
namespace Harvester_image_ui
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ConvertSingleFileButton = new System.Windows.Forms.Button();
            this.ConvertFolderButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ConvertSingleFileButton
            // 
            this.ConvertSingleFileButton.Location = new System.Drawing.Point(12, 12);
            this.ConvertSingleFileButton.Name = "ConvertSingleFileButton";
            this.ConvertSingleFileButton.Size = new System.Drawing.Size(197, 45);
            this.ConvertSingleFileButton.TabIndex = 0;
            this.ConvertSingleFileButton.Text = "Convert Single Background";
            this.ConvertSingleFileButton.UseVisualStyleBackColor = true;
            this.ConvertSingleFileButton.Click += new System.EventHandler(this.ConvertSingleFileButton_Click);
            // 
            // ConvertFolderButton
            // 
            this.ConvertFolderButton.Location = new System.Drawing.Point(257, 12);
            this.ConvertFolderButton.Name = "ConvertFolderButton";
            this.ConvertFolderButton.Size = new System.Drawing.Size(197, 45);
            this.ConvertFolderButton.TabIndex = 1;
            this.ConvertFolderButton.Text = "Convert Folder of Backgrounds";
            this.ConvertFolderButton.UseVisualStyleBackColor = true;
            this.ConvertFolderButton.Click += new System.EventHandler(this.ConvertFolderButton_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(419, 111);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(35, 42);
            this.button1.TabIndex = 2;
            this.button1.Text = "?";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(12, 63);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(197, 42);
            this.button2.TabIndex = 3;
            this.button2.Text = "Convert Single Animation";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(258, 63);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(197, 42);
            this.button3.TabIndex = 4;
            this.button3.Text = "Convert Folder of Animations";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(467, 159);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ConvertFolderButton);
            this.Controls.Add(this.ConvertSingleFileButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Harvester Image Converter";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ConvertSingleFileButton;
        private System.Windows.Forms.Button ConvertFolderButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

