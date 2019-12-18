using GSF.Reflection;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace PQds
{
    public partial class PQdsSplashScreen : Form
    {
        public PQdsSplashScreen()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
            //Load License
            ReadLicense();

        }

        private void ReadLicense()
        {
            using (Stream aboutStream = Assembly.GetEntryAssembly().GetManifestResourceStream("PQds.Documentation.Splash_PQDS.rtf"))
            using (TextReader aboutReader = new StreamReader(aboutStream))
            {
                this.richTextBox1.Rtf = aboutReader.ReadToEnd();
            }
        }

        private void PQdsSpalshScreen_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
