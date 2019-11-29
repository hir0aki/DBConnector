namespace DBConnector
{
    partial class frmDebug
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
            this.txtQuery = new System.Windows.Forms.TextBox();
            this.dataGridViewParameters = new System.Windows.Forms.DataGridView();
            this.ParameterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParameterValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // txtQuery
            // 
            this.txtQuery.Location = new System.Drawing.Point(13, 13);
            this.txtQuery.Multiline = true;
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(518, 49);
            this.txtQuery.TabIndex = 0;
            // 
            // dataGridViewParameters
            // 
            this.dataGridViewParameters.AllowUserToAddRows = false;
            this.dataGridViewParameters.AllowUserToDeleteRows = false;
            this.dataGridViewParameters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewParameters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParameterName,
            this.ParameterValue});
            this.dataGridViewParameters.Location = new System.Drawing.Point(13, 68);
            this.dataGridViewParameters.Name = "dataGridViewParameters";
            this.dataGridViewParameters.ReadOnly = true;
            this.dataGridViewParameters.RowHeadersVisible = false;
            this.dataGridViewParameters.Size = new System.Drawing.Size(518, 175);
            this.dataGridViewParameters.TabIndex = 1;
            // 
            // ParameterName
            // 
            this.ParameterName.HeaderText = "Parameter Name";
            this.ParameterName.Name = "ParameterName";
            this.ParameterName.ReadOnly = true;
            // 
            // ParameterValue
            // 
            this.ParameterValue.HeaderText = "Parameter Value";
            this.ParameterValue.Name = "ParameterValue";
            this.ParameterValue.ReadOnly = true;
            // 
            // frmDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 255);
            this.Controls.Add(this.dataGridViewParameters);
            this.Controls.Add(this.txtQuery);
            this.Name = "frmDebug";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Debug";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewParameters)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtQuery;
        private System.Windows.Forms.DataGridView dataGridViewParameters;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterValue;
    }
}