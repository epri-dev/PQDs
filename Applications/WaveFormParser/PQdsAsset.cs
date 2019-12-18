using GSF.Data;
using System;
using System.Windows.Forms;

namespace PQds
{
    public partial class PQdsAsset : Form
    {
        #region[properties]
        private PQds.Model.Asset m_Asset;
        #endregion[properties]

        public PQdsAsset(int id)
        {
            if (id == -1)
            {
                m_Asset = new PQds.Model.Asset();
            }
            else
            {
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {

                    GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTable = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);
                    m_Asset = assetTable.QueryRecordWhere("ID = {0}", id);
                }
            }

            InitializeComponent();
        }

       

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LineForm_Load(object sender, EventArgs e)
        {
            
                AssetNameTxtBox.Text = m_Asset.AssetKey;
            if (!(m_Asset.NominalFrequency == 0))
            {
                NomfTxtBox.Text = Convert.ToString(m_Asset.NominalFrequency);
            }
            if (!(m_Asset.NominalVoltage == 0))
            {
                NomVTxtBox.Text = Convert.ToString(m_Asset.NominalVoltage);
            }
            if (!(m_Asset.Length == 0))
            {
                lenTxtBox.Text = Convert.ToString(m_Asset.Length);
            }
            if (!(m_Asset.UpstreamXFMR == 0))
            {
                XFTxtBox.Text = Convert.ToString(m_Asset.UpstreamXFMR);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_Asset.AssetKey = AssetNameTxtBox.Text;

            if (NomfTxtBox.Text.Trim() != "")
            {
                try
                {
                    if (!(Convert.ToDouble(NomfTxtBox.Text) == 0))
                    {
                        if (Convert.ToDouble(NomfTxtBox.Text) > 0)
                        {
                            m_Asset.NominalFrequency = Convert.ToDouble(NomfTxtBox.Text);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }
                catch { MessageBox.Show("Nominal Frequency has to be a positive number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            }

            if (NomVTxtBox.Text.Trim() != "")
            {
                try
                {
                    if (!(Convert.ToDouble(NomVTxtBox.Text) == 0))
                    {
                        m_Asset.NominalVoltage = Convert.ToDouble(NomVTxtBox.Text);
                    }
                }
                catch { MessageBox.Show("Nominal Voltage has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            }
            

            if (lenTxtBox.Text.Trim() != "")
            {
                try
                {
                    if (!(Convert.ToDouble(lenTxtBox.Text) == 0))
                    {
                        m_Asset.Length = Convert.ToDouble(lenTxtBox.Text);
                    }
                }
                catch { MessageBox.Show("Length has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            }
            
            if (XFTxtBox.Text.Trim() != "")
            {
                try
                {
                    if (!(Convert.ToDouble(XFTxtBox.Text) == 0))
                    {
                        m_Asset.UpstreamXFMR = Convert.ToDouble(XFTxtBox.Text);
                    }
                }
                catch { MessageBox.Show("Upstream XF Size has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            }
                       

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTable = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);
                assetTable.AddNewOrUpdateRecord(m_Asset);
            }
            this.Close();
        }

    }
}
