namespace ShuriOutlookAddIn
{
    partial class TimerForm
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timerCalRibbon = new System.Windows.Forms.Timer(this.components);
            this.timerSettings = new System.Windows.Forms.Timer(this.components);
            this.timerApptRibbon = new System.Windows.Forms.Timer(this.components);
            this.timerDelayWrite = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // TimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "TimerForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "TimerForm";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Timer timerCalRibbon;
        public System.Windows.Forms.Timer timerSettings;
        public System.Windows.Forms.Timer timerApptRibbon;
        public System.Windows.Forms.Timer timerDelayWrite;
    }
}