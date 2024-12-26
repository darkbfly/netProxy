namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            comboBox1 = new ComboBox();
            button2 = new Button();
            richTextBox1 = new RichTextBox();
            button3 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 134);
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(692, 65);
            button1.TabIndex = 0;
            button1.Text = "启动";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // comboBox1
            // 
            comboBox1.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold);
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(12, 83);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(692, 39);
            comboBox1.TabIndex = 1;
            // 
            // button2
            // 
            button2.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 134);
            button2.Location = new Point(12, 128);
            button2.Name = "button2";
            button2.Size = new Size(692, 65);
            button2.TabIndex = 2;
            button2.Text = "重新加载fiddler配置";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold);
            richTextBox1.Location = new Point(12, 199);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(692, 402);
            richTextBox1.TabIndex = 3;
            richTextBox1.Text = "";
            // 
            // button3
            // 
            button3.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 134);
            button3.Location = new Point(12, 607);
            button3.Name = "button3";
            button3.Size = new Size(692, 65);
            button3.TabIndex = 4;
            button3.Text = "清屏";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(716, 677);
            Controls.Add(button3);
            Controls.Add(richTextBox1);
            Controls.Add(button2);
            Controls.Add(comboBox1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private ComboBox comboBox1;
        private Button button2;
        private RichTextBox richTextBox1;
        private Button button3;
    }
}
