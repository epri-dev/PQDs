using GSF.Data;
using System;
using System.Windows.Forms;

namespace PQds
{
    public partial class PQdsAddCustom : Form
    {
        int m_eventid;
        int m_assetid;

        public PQdsAddCustom(int AssetID, int EventID)
        {
            InitializeComponent();
            this.m_assetid = AssetID;
            this.m_eventid = EventID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PQioAddCustom_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.CustomField> customFldTbl = new GSF.Data.Model.TableOperations<PQds.Model.CustomField>(connection);

                if (this.textBox1.Text.Length < 1)
                {
                    MessageBox.Show("A Domain name has to be selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (customFldTbl.QueryRecordCountWhere("domain = {0}", this.textBox1.Text) > 0)
                {
                    MessageBox.Show("This domain already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    PQds.Model.CustomField fld = new Model.CustomField() { EventID = m_eventid, AssetID = m_assetid, key = "Key", Value = "Value", Type = "T", domain = this.textBox1.Text };
                    customFldTbl.AddNewRecord(fld);
                }


            }

            this.Close();
        }
    }
}
