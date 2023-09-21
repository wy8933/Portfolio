namespace SwordRushLevelEditor
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
            buttonLoad = new Button();
            groupBoxCreateMap = new GroupBox();
            buttonCreate = new Button();
            groupBoxCreateMap.SuspendLayout();
            SuspendLayout();
            // 
            // buttonLoad
            // 
            buttonLoad.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            buttonLoad.Location = new Point(35, 10);
            buttonLoad.Margin = new Padding(2);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(395, 120);
            buttonLoad.TabIndex = 0;
            buttonLoad.Text = "Load Map";
            buttonLoad.UseVisualStyleBackColor = true;
            buttonLoad.Click += buttonLoad_Click;
            // 
            // groupBoxCreateMap
            // 
            groupBoxCreateMap.Controls.Add(buttonCreate);
            groupBoxCreateMap.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            groupBoxCreateMap.Location = new Point(35, 142);
            groupBoxCreateMap.Margin = new Padding(2);
            groupBoxCreateMap.Name = "groupBoxCreateMap";
            groupBoxCreateMap.Padding = new Padding(2);
            groupBoxCreateMap.Size = new Size(395, 166);
            groupBoxCreateMap.TabIndex = 1;
            groupBoxCreateMap.TabStop = false;
            groupBoxCreateMap.Text = "Create New Map";
            // 
            // buttonCreate
            // 
            buttonCreate.Location = new Point(21, 43);
            buttonCreate.Margin = new Padding(2);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(354, 109);
            buttonCreate.TabIndex = 4;
            buttonCreate.Text = "Create Map";
            buttonCreate.UseVisualStyleBackColor = true;
            buttonCreate.Click += buttonCreate_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(465, 323);
            Controls.Add(groupBoxCreateMap);
            Controls.Add(buttonLoad);
            Margin = new Padding(2);
            Name = "Form1";
            Text = "Form1";
            groupBoxCreateMap.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button buttonLoad;
        private GroupBox groupBoxCreateMap;
        private Button buttonCreate;
    }
}