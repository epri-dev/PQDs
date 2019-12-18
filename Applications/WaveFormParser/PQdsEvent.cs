using GSF.Data;
using System;
using System.Windows.Forms;

namespace PQds
{
    public partial class PQdsEvent : Form
    {
        #region[properties]

        private PQds.Model.Event m_Event;
        
        #endregion[properties]

        public PQdsEvent(int id)
        {
            if (id == -1)
            {
                //This means the GUID needs to be generated

                m_Event = new PQds.Model.Event() { GUID = System.Guid.NewGuid().ToString() };
            }
            else
            {
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {

                    GSF.Data.Model.TableOperations<PQds.Model.Event> evtTable = new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection);
                    m_Event = evtTable.QueryRecordWhere("ID = {0}", id);
                }
            }


            InitializeComponent();
        }

        private void PQioEvent_Load(object sender, EventArgs e)
        {
            faultIDTxtBox.Text = m_Event.Name;
            guidTxtBox.Text = m_Event.GUID;

            if (m_Event.EventTime != null)
            {
                timeTxtBox.Value = (DateTime)m_Event.EventTime;
            }

            if (m_Event.PeakCurrent != null)
            {
                peakITxtBox.Text = Convert.ToString(m_Event.PeakCurrent);
            }
            if (m_Event.PeakVoltage != null)
            {
                peakVTxtBox.Text = Convert.ToString(m_Event.PeakVoltage);
            }
            if (m_Event.PreEventCurrent != null)
            {
                preITxtBox.Text = Convert.ToString(m_Event.PreEventCurrent);
            }
            if (m_Event.PreEventVoltage != null)
            {
                preVTxtBox.Text = Convert.ToString(m_Event.PreEventVoltage);
            }

            if (m_Event.FaultI2T != null)
            {
                I2tTxtBox.Text = Convert.ToString(m_Event.FaultI2T);
            }

            if (m_Event.MaxVA != null)
            {
                maxVaTxtBox.Text = Convert.ToString(m_Event.MaxVA);
            }
            if (m_Event.MaxVB != null)
            {
                maxVbTxtBox.Text = Convert.ToString(m_Event.MaxVB);
            }
            if (m_Event.MaxVC != null)
            {
                maxVcTxtBox.Text = Convert.ToString(m_Event.MaxVC);
            }

            
            if (m_Event.MinVA != null)
            {
                minVaTxtBox.Text = Convert.ToString(m_Event.MinVA);
            }
            if (m_Event.MinVB != null)
            {
                minVbTxtBox.Text = Convert.ToString(m_Event.MinVB);
            }
            if (m_Event.MinVC != null)
            {
                minVcTxtBox.Text = Convert.ToString(m_Event.MinVC);
            }

            if (m_Event.MaxIA != null)
            {
                maxIaTxtBox.Text = Convert.ToString(m_Event.MaxIA);
            }
            if (m_Event.MaxIB != null)
            {
                maxIbTxtBox.Text = Convert.ToString(m_Event.MaxIB);
            }
            if (m_Event.MaxIC != null)
            {
                maxIcTxtBox.Text = Convert.ToString(m_Event.MaxIC);
            }

            if (m_Event.Duration != null)
            {
                durationTxtBox.Text = Convert.ToString(m_Event.Duration);
            }
            if (m_Event.DistanceToFault != null)
            {
                distanceTxtBox.Text = Convert.ToString(m_Event.DistanceToFault);
            }

            FltTypeCombo.Items.AddRange(Model.FaultType.DisplayOptions());
            evtTypeCombo.Items.AddRange(Model.EventType.DisplayOptions());
            evtCauseCombo.Items.AddRange(Model.FaultCause.DisplayOptions());
                
            if (m_Event.FaultType != null)
            {
                FltTypeCombo.SelectedIndex = Array.FindIndex(Model.FaultType.DisplayOptions(),
                    item => item == Model.FaultType.ToDisplay((int)m_Event.FaultType));
            }
            else
            {
                FltTypeCombo.SelectedIndex = Array.FindIndex(Model.FaultType.DisplayOptions(),
                    item => item == Model.FaultType.ToDisplay(-1));
            }

            if (m_Event.EventType != null)
            {
                evtTypeCombo.SelectedIndex = Array.FindIndex(Model.EventType.DisplayOptions(),
                    item => item == Model.EventType.ToDisplay((int)m_Event.EventType));
            }
            else
            {
                evtTypeCombo.SelectedIndex = Array.FindIndex(Model.EventType.DisplayOptions(),
                    item => item == Model.EventType.ToDisplay(-1));
            }

            if (m_Event.FaultCause != null)
            {
                evtCauseCombo.SelectedIndex = Array.FindIndex(Model.FaultCause.DisplayOptions(),
                    item => item == Model.FaultCause.ToDisplay((int)m_Event.FaultCause));
            }
            else
            {
                evtCauseCombo.SelectedIndex = Array.FindIndex(Model.FaultCause.DisplayOptions(),
                    item => item == Model.FaultCause.ToDisplay(-1));
            }

            


        }

        private void cxnBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Event> evtTable = new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection);
                PQds.Model.Event evt = evtTable.QueryRecordWhere("ID =  {0}", m_Event.ID);

                evt.Name = faultIDTxtBox.Text;
                evt.EventTime = timeTxtBox.Value;

                if (peakITxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(peakITxtBox.Text) == 0))
                        {
                            evt.PeakCurrent = Convert.ToDouble(peakITxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Peak Current has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }

                if (peakVTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(peakVTxtBox.Text) == 0))
                        {
                            evt.PeakVoltage = Convert.ToDouble(peakVTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Peak Voltage has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }

                if(preITxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(preITxtBox.Text) == 0))
                        {
                            evt.PreEventCurrent = Convert.ToDouble(preITxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Pre-Event Current has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }

                if(preVTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(preVTxtBox.Text) == 0))
                        {
                            evt.PreEventVoltage = Convert.ToDouble(preVTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Pre-Event Voltage has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }

                if (I2tTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(I2tTxtBox.Text) == 0))
                        {
                            evt.FaultI2T = Convert.ToDouble(I2tTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Fault I2(t) has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }
              

                if (maxVaTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(maxVaTxtBox.Text) == 0))
                        {
                            evt.MaxVA = Convert.ToDouble(maxVaTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Phase A Voltage Maximum has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }
                if (maxVbTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(maxVbTxtBox.Text) == 0))
                        {
                            evt.MaxVB = Convert.ToDouble(maxVbTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Phase B Voltage Maximum has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }
                if (maxVcTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(maxVcTxtBox.Text) == 0))
                        {
                            evt.MaxVC = Convert.ToDouble(maxVcTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Phase C Voltage Maximum has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }

                if (minVaTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(minVaTxtBox.Text) == 0))
                        {
                            evt.MinVA = Convert.ToDouble(minVaTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Phase A Voltage Minimum has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                }
                if (minVbTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(minVbTxtBox.Text) == 0))
                        {
                            evt.MinVB = Convert.ToDouble(minVbTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Phase B Voltage Minimum has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                }
                if (minVcTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(minVcTxtBox.Text) == 0))
                        {
                            evt.MinVC = Convert.ToDouble(minVcTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Phase C Voltage Minimum has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                }
                
                if (maxIaTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(maxIaTxtBox.Text) == 0))
                        {
                            evt.MaxIA = Convert.ToDouble(maxIaTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Phase A Current Maximum has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                }
                if (maxIbTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(maxIbTxtBox.Text) == 0))
                        {
                            evt.MaxIB = Convert.ToDouble(maxIbTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Phase B Current Maximum has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                }
                if (maxIcTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(maxIcTxtBox.Text) == 0))
                        {
                            evt.MaxIC = Convert.ToDouble(maxIcTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Phase C Current Maximum has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                }
                  

                if (durationTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(durationTxtBox.Text) == 0))
                        {
                            evt.Duration = Convert.ToDouble(durationTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Event duration has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                }
                
                

                if (distanceTxtBox.Text != "")
                {
                    try
                    {
                        if (!(Convert.ToDouble(distanceTxtBox.Text) == 0))
                        {
                            evt.DistanceToFault = Convert.ToDouble(distanceTxtBox.Text);
                        }
                    }
                    catch { MessageBox.Show("Distance to Fault has to be a Number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                }

               

                // Update Data from Comboboxes
                if ((string)FltTypeCombo.SelectedItem == "")
                {
                    evt.FaultType = null;
                }
                else
                {
                    evt.FaultType = Model.FaultType.ToValue((string)FltTypeCombo.SelectedItem);
                }

                if ((string)evtTypeCombo.SelectedItem == "")
                {
                    evt.EventType = null;
                }
                else
                {
                    evt.EventType = Model.EventType.ToValue((string)evtTypeCombo.SelectedItem);
                }


                if ((string)evtCauseCombo.SelectedItem == "")
                {
                    evt.FaultCause = null;
                }
                else
                {
                    evt.FaultCause = Model.FaultCause.ToValue((string)evtCauseCombo.SelectedItem);
                }

                evtTable.AddNewOrUpdateRecord(evt);

            }
            this.Close();
        }

        private void timeTxtBox_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.m_Event.GUID = Guid.NewGuid().ToString();
            this.guidTxtBox.Text = this.m_Event.GUID;
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
