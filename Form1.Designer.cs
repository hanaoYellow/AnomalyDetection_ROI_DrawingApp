namespace DrawingROIApp
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
            tableLayoutPanel_Out = new TableLayoutPanel();
            tableLayoutPanel_Upper = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            ledViewButton = new Button();
            resetButton = new Button();
            saveButton = new Button();
            fIleSelectButton = new Button();
            tableLayoutPanel_Lower = new TableLayoutPanel();
            pictureBox1 = new PictureBox();
            dataGridView1 = new DataGridView();
            listBox1 = new ListBox();
            tableLayoutPanel_Out.SuspendLayout();
            tableLayoutPanel_Upper.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            tableLayoutPanel_Lower.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel_Out
            // 
            tableLayoutPanel_Out.BackColor = SystemColors.Control;
            tableLayoutPanel_Out.ColumnCount = 1;
            tableLayoutPanel_Out.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel_Out.Controls.Add(tableLayoutPanel_Upper, 0, 0);
            tableLayoutPanel_Out.Controls.Add(tableLayoutPanel_Lower, 0, 1);
            tableLayoutPanel_Out.Dock = DockStyle.Fill;
            tableLayoutPanel_Out.Location = new Point(0, 0);
            tableLayoutPanel_Out.Name = "tableLayoutPanel_Out";
            tableLayoutPanel_Out.RowCount = 2;
            tableLayoutPanel_Out.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel_Out.RowStyles.Add(new RowStyle(SizeType.Percent, 83.3333359F));
            tableLayoutPanel_Out.Size = new Size(1117, 637);
            tableLayoutPanel_Out.TabIndex = 1;
            // 
            // tableLayoutPanel_Upper
            // 
            tableLayoutPanel_Upper.BackColor = SystemColors.ActiveBorder;
            tableLayoutPanel_Upper.ColumnCount = 2;
            tableLayoutPanel_Upper.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel_Upper.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel_Upper.Controls.Add(flowLayoutPanel1, 1, 0);
            tableLayoutPanel_Upper.Controls.Add(fIleSelectButton, 0, 0);
            tableLayoutPanel_Upper.Dock = DockStyle.Fill;
            tableLayoutPanel_Upper.Location = new Point(3, 3);
            tableLayoutPanel_Upper.Name = "tableLayoutPanel_Upper";
            tableLayoutPanel_Upper.RowCount = 1;
            tableLayoutPanel_Upper.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel_Upper.Size = new Size(1111, 100);
            tableLayoutPanel_Upper.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.None;
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(ledViewButton);
            flowLayoutPanel1.Controls.Add(resetButton);
            flowLayoutPanel1.Controls.Add(saveButton);
            flowLayoutPanel1.Location = new Point(642, 12);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(382, 76);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // ledViewButton
            // 
            ledViewButton.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            ledViewButton.Location = new Point(3, 3);
            ledViewButton.Name = "ledViewButton";
            ledViewButton.Size = new Size(140, 70);
            ledViewButton.TabIndex = 0;
            ledViewButton.Text = "LEDの表示";
            ledViewButton.UseVisualStyleBackColor = true;
            ledViewButton.Click += ledViewButton_Click;
            // 
            // resetButton
            // 
            resetButton.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            resetButton.Location = new Point(149, 3);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(112, 70);
            resetButton.TabIndex = 1;
            resetButton.Text = "リセット";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += resetButton_Click;
            // 
            // saveButton
            // 
            saveButton.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            saveButton.Location = new Point(267, 3);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(112, 70);
            saveButton.TabIndex = 2;
            saveButton.Text = "保存";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // fIleSelectButton
            // 
            fIleSelectButton.Anchor = AnchorStyles.Left;
            fIleSelectButton.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            fIleSelectButton.Location = new Point(3, 15);
            fIleSelectButton.Name = "fIleSelectButton";
            fIleSelectButton.Size = new Size(146, 70);
            fIleSelectButton.TabIndex = 1;
            fIleSelectButton.Text = "ファイル選択";
            fIleSelectButton.UseVisualStyleBackColor = true;
            fIleSelectButton.Click += fIleSelectButton_Click;
            // 
            // tableLayoutPanel_Lower
            // 
            tableLayoutPanel_Lower.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel_Lower.ColumnCount = 3;
            tableLayoutPanel_Lower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10.2197294F));
            tableLayoutPanel_Lower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54.0112419F));
            tableLayoutPanel_Lower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35.76903F));
            tableLayoutPanel_Lower.Controls.Add(pictureBox1, 1, 0);
            tableLayoutPanel_Lower.Controls.Add(dataGridView1, 2, 0);
            tableLayoutPanel_Lower.Controls.Add(listBox1, 0, 0);
            tableLayoutPanel_Lower.Location = new Point(3, 109);
            tableLayoutPanel_Lower.Name = "tableLayoutPanel_Lower";
            tableLayoutPanel_Lower.RowCount = 1;
            tableLayoutPanel_Lower.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel_Lower.Size = new Size(1111, 525);
            tableLayoutPanel_Lower.TabIndex = 1;
            tableLayoutPanel_Lower.Paint += tableLayoutPanel1_Paint;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Black;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(116, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(594, 519);
            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = true;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.MouseEnter += pictureBox1_MouseEnter;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(716, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(392, 519);
            dataGridView1.TabIndex = 1;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // listBox1
            // 
            listBox1.Dock = DockStyle.Fill;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(3, 3);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(107, 519);
            listBox1.TabIndex = 2;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1117, 637);
            Controls.Add(tableLayoutPanel_Out);
            Name = "Form1";
            Text = "Form1";
            tableLayoutPanel_Out.ResumeLayout(false);
            tableLayoutPanel_Upper.ResumeLayout(false);
            tableLayoutPanel_Upper.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel_Lower.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel_Out;
        private TableLayoutPanel tableLayoutPanel_Upper;
        private TableLayoutPanel tableLayoutPanel_Lower;
        private FlowLayoutPanel flowLayoutPanel1;
        private PictureBox pictureBox1;
        private DataGridView dataGridView1;
        private ListBox listBox1;
        private Button fIleSelectButton;
        private Button ledViewButton;
        private Button resetButton;
        private Button saveButton;
    }
}
