using System;
using System.Windows.Forms;

namespace PQds
{
    public partial class PQdsProgress : Form
    {
        string m_textTemplate;

        public PQdsProgress(int maxProgress)
        {
            InitializeComponent();
            progressBar1.Maximum = maxProgress;
            this.m_textTemplate = "";
        }

        private void PQIOProgress_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            
        }

        public void updateText(string text)
        {
            this.m_textTemplate = text;
            updateText();
        }

        public void updateText()
        {
            if (this.m_textTemplate.Contains("{0}"))
                this.label1.Text = String.Format(this.m_textTemplate, (this.progressBar1.Value / 100) + 1);
            else
                this.label1.Text = this.m_textTemplate;

            // Recalculate Position to center with new text
            this.label1.Left = this.Width / 2 - this.label1.Width / 2;
        }

        public void updateProgress(int i)
        {
            if (i > progressBar1.Maximum)
                i = progressBar1.Maximum;

            progressBar1.Value = i;
            updateText();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
