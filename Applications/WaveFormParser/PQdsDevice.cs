using GSF.Data;
using System;
using System.Windows.Forms;

namespace PQds
{
    public partial class PQdsDevice : Form
    {

        #region[Properties]

        private PQds.Model.Meter m_device;

        #endregion[Properties]

        public PQdsDevice(int id)
        {
            if (id == -1)
            {
                m_device = new PQds.Model.Meter();
            }
            else
            {
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {

                    GSF.Data.Model.TableOperations<PQds.Model.Meter> deviceTable = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);
                    m_device = deviceTable.QueryRecordWhere("ID = {0}", id);
                    this.Text = string.Format("PQds {0} MetaData", m_device.DeviceAlias);
                }
            }

            InitializeComponent();
        }

        private void PQioDevice_Load(object sender, EventArgs e)
        {

            NameTxtBox.Text = m_device.DeviceName;
            AliasTxtBox.Text = m_device.DeviceAlias;

            LocationTxtBox.Text = m_device.DeviceLocation;
            LocationAliasTxtBox.Text = m_device.DeviceLocationAlias;

            if (!(m_device.Latitude == 0 ))
            {
                LatTxtBox.Text = Convert.ToString(m_device.Latitude);
            }
            if (!(m_device.Longitude == 0))
            {
                LongTxtBox.Text = Convert.ToString(m_device.Longitude);
            }

            ActAliasTxtBox.Text = m_device.AccountAlias;
            ActTxtBox.Text = m_device.AccountName;

            OwnerTxtBox.Text = m_device.Owner;

            if (!(m_device.DistanceToXFR == 0))
            {
                XFRTxtBox.Text = Convert.ToString(m_device.DistanceToXFR);
            }

            connBox.Items.AddRange(Model.ConnectionType.DisplayOptions());

            if (m_device.ConnectionType != null)
            {
                connBox.SelectedIndex = Array.FindIndex(Model.ConnectionType.DisplayOptions(),
                    item => item == Model.ConnectionType.ToDisplay((int)m_device.ConnectionType));
            }
            else
            {
                connBox.SelectedIndex = Array.FindIndex(Model.ConnectionType.DisplayOptions(),
                    item => item == Model.ConnectionType.ToDisplay(-1));
            }

        }

        private void CxnBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Meter> deviceTable = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);
                PQds.Model.Meter device = deviceTable.QueryRecordWhere("ID = {0}", m_device.ID);
                if (device is null)
                {
                    device = new PQds.Model.Meter();
                }

                device.DeviceName = NameTxtBox.Text;

                device.DeviceAlias = AliasTxtBox.Text;


                device.DeviceLocation = LocationTxtBox.Text;
                device.DeviceLocationAlias = LocationAliasTxtBox.Text;


                if (LatTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(LatTxtBox.Text) == 0))
                        {
                            device.Latitude = Convert.ToDouble(LatTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Latitude has to be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }

                if (LongTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(LongTxtBox.Text) == 0))
                        {
                            device.Longitude = Convert.ToDouble(LongTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Longitude has to be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }

                device.AccountAlias = ActAliasTxtBox.Text;
                device.AccountName = ActTxtBox.Text;

                device.Owner = OwnerTxtBox.Text;

                if (XFRTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(XFRTxtBox.Text) == 0))
                        {
                            device.DistanceToXFR = Convert.ToDouble(XFRTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Distance to XFMR has to be a number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }


                if ((string)connBox.SelectedItem == "")
                {
                    device.ConnectionType = null;
                }
                else
                {
                    device.ConnectionType = Model.ConnectionType.ToValue((string)connBox.SelectedItem);
                }

                deviceTable.AddNewOrUpdateRecord(device);

            }
            this.Close();

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}
