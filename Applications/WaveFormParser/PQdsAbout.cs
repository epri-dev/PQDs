using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PQds
{
    public partial class PQdsAbout : Form
    {
        public PQdsAbout()
        {
            InitializeComponent();
        }

        private void PQioAbout_Load(object sender, EventArgs e)
        {
            using (Stream aboutStream = Assembly.GetEntryAssembly().GetManifestResourceStream("PQds.Documentation.About_PQDS.rtf"))
            using (TextReader aboutReader = new StreamReader(aboutStream))
            {
                this.richTextBox1.Rtf = aboutReader.ReadToEnd();
            }

        }
    }
}
