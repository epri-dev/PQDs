using GSF.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PQds
{
    public partial class PQdsChannel : Form
    {
        #region[Properties]
        PQds.Model.Event m_evt;
        PQds.Model.Channel m_channel;

        //This flag is for the Custom MetaData field
        bool alreadySavedFlag;
        #endregion[Properties]

        #region[Constructors]
        public PQdsChannel(int eventId, int channelId)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                this.m_channel = (new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection)).QueryRecordWhere("ID = {0}", channelId);
                this.m_evt = (new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection)).QueryRecordWhere("ID = {0}", eventId);
            }
            this.alreadySavedFlag = false;

            InitializeComponent();
        }

        #endregion[Constructors]
        #region[Methods]
        private void PQioChannel_Load(object sender, EventArgs e)
        {
            // Populate Combo Boxes
            comboBox1.Items.AddRange(Model.SignalType.DisplayOptions());
            comboBox2.Items.AddRange(Model.MeasurementType.DisplayOptions());
            comboBox3.Items.AddRange(Model.DataSensitivityCode.DisplayOptions());


            if (m_channel.SignalType != null)
            {
                comboBox1.SelectedIndex = Array.FindIndex(Model.SignalType.DisplayOptions(),
                    item => item == Model.SignalType.ToDisplay((int)m_channel.SignalType));
            }
            else
            {
                comboBox1.SelectedIndex = Array.FindIndex(Model.SignalType.DisplayOptions(),
                    item => item == Model.SignalType.ToDisplay(-1));
            }

            if (m_channel.MeasurementType != null)
            {
                comboBox2.SelectedIndex = Array.FindIndex(Model.MeasurementType.DisplayOptions(),
                    item => item == Model.MeasurementType.ToDisplay(m_channel.MeasurementType));
            }
            else
            {
                comboBox2.SelectedIndex = Array.FindIndex(Model.MeasurementType.DisplayOptions(),
                    item => item == Model.MeasurementType.ToDisplay(""));
            }

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity> dataSensitivityTbl = new GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.Meter> deviceTbl = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);


                if (this.m_channel.AssetID != null)
                {
                    // Make sure we check if there is any Data Sensitivity associated with this event

                    if (dataSensitivityTbl.QueryRecordCountWhere("Event = {0} AND Asset = {1}", this.m_evt.ID, this.m_channel.AssetID) > 0)
                    {
                        PQds.Model.DataSensitivity dataSensitivity = dataSensitivityTbl.QueryRecordsWhere("Event = {0} AND Asset = {1}", this.m_evt.ID, this.m_channel.AssetID).First();


                        comboBox3.SelectedIndex = Array.FindIndex(Model.DataSensitivityCode.DisplayOptions(),
                        item => item == Model.DataSensitivityCode.ToDisplay((int)dataSensitivity.DataSensitivityCode));

                        DataSensitivityNoteText.Text = dataSensitivity.Note;
                    }
                    else
                    {
                        comboBox3.SelectedIndex = Array.FindIndex(Model.DataSensitivityCode.DisplayOptions(),
                        item => item == Model.DataSensitivityCode.ToDisplay(-1));
                        DataSensitivityNoteText.Text = "";
                    }
                }
                else
                {
                    comboBox3.Enabled = false;
                    comboBox3.SelectedIndex = Array.FindIndex(Model.DataSensitivityCode.DisplayOptions(),
                        item => item == Model.DataSensitivityCode.ToDisplay(-1));
                }

                comboDevice.DisplayMember = "Text";
                comboDevice.ValueMember = "Value";

                PQds.Model.Meter[] meters = deviceTbl.QueryRecords("").ToArray();

                comboDevice.Items.AddRange(meters.Select(item => new { Text = item.DeviceName, Value = item.ID }).ToArray());
                comboDevice.SelectedIndex = Array.FindIndex(meters,item => item.ID == this.m_channel.MeterID);

            }
            if (this.m_evt != null)
                UpdateCustomFields();
            else
                this.tabControl1.Enabled = false;

            this.MouseDown += new MouseEventHandler(OnClick);

        }

        private void UpdateCustomFields()
        {

            tabControl1.TabPages.Clear();

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.CustomField> customFldTbl = new GSF.Data.Model.TableOperations<PQds.Model.CustomField>(connection);
                List<string> HeaderFlds;


                HeaderFlds = customFldTbl.QueryRecordsWhere("AssetID = {0} AND EventID = {1} ", m_channel.AssetID, m_evt.ID).Select(item => item.domain).Distinct().ToList();


                foreach (string domainName in HeaderFlds)
                {
                    TabPage myTabPage = new TabPage(domainName);


                    ListView data = new ListView();
                    data.HideSelection = false;
                    data.Location = new System.Drawing.Point(9, 4);
                    data.Name = "listView1-" + domainName;
                    data.Size = new System.Drawing.Size(623, 223);
                    data.TabIndex = 0;
                    data.UseCompatibleStateImageBehavior = false;
                    data.FullRowSelect = true;

                    data.View = View.Details;
                    // Add a column with width 20 and left alignment.
                    data.Columns.Add("key", "Key", 100, HorizontalAlignment.Left, 0);
                    data.Columns.Add("value", "Value", 75, HorizontalAlignment.Left, 0);
                    data.Columns.Add("type", "Type", 75, HorizontalAlignment.Left, 0);

                    data.Items.AddRange(customFldTbl.QueryRecordsWhere("domain = {0}", domainName).Select(item =>
                    {
                        ListViewItem listItem = new ListViewItem(new string[] { item.key, item.Value, TypeToText(item.Type) });
                        listItem.Tag = item.ID;
                        return listItem;
                    }).ToArray());

                    ListViewItem newItem = new ListViewItem(new string[] { "*", "*", "*" });
                    newItem.Tag = -1;

                    data.Items.Add(newItem);
                    data.SelectedIndexChanged += new EventHandler(this.SelectedIndexChanged);
                    myTabPage.Controls.Add(data);

                    //Add textbox and ComboBox to create new Custom Fields and edit them
                    TextBox txtKey = new TextBox();
                    txtKey.Visible = false;
                    txtKey.Name = "editkey";
                    txtKey.KeyUp += new KeyEventHandler(this.TxtEdit_KeyUp);
                    txtKey.Leave += new EventHandler(this.TxtEdit_Leave);

                    TextBox txtValue = new TextBox();
                    txtValue.Visible = false;
                    txtValue.Name = "editvalue";
                    txtValue.KeyUp += new KeyEventHandler(this.TxtEdit_KeyUp);
                    txtValue.Leave += new EventHandler(this.TxtEdit_Leave);

                    ComboBox txtType = new ComboBox();
                    txtType.Visible = false;

                    txtType.Items.Add("Text");
                    txtType.Items.Add("Numeric");

                    txtType.Name = "editType";
                    txtType.KeyUp += new KeyEventHandler(this.TxtEdit_KeyUp);
                    txtType.Leave += new EventHandler(this.TxtEdit_Leave);
                    txtType.DropDownStyle = ComboBoxStyle.DropDownList;

                    myTabPage.Controls.Add(txtKey);
                    myTabPage.Controls.Add(txtValue);
                    myTabPage.Controls.Add(txtType);
                    tabControl1.TabPages.Add(myTabPage);
                }


                TabPage newTabbPage = new TabPage("Add New");
                tabControl1.TabPages.Add(newTabbPage);
                tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.CreateNewTab);

                if (HeaderFlds.Count == 0)
                    this.tabControl1.Enabled = false;
                else
                    this.tabControl1.Enabled = true;
            }

            this.alreadySavedFlag = false;
        }

        private string TypeToText(string type)
        {
            switch (type)
            {
                case ("T"):
                    return "Text";
                case ("N"):
                    return "Numeric";
                default:
                    return "Other";
            }
        }

        private string TextToType(string type)
        {
            switch (type)
            {
                case ("Text"):
                    return "T";
                case ("Numeric"):
                    return "N";
                default:
                    return "T";
            }
        }

        private void CreateNewTab(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == null)
                return;

            string activeTab = tabControl1.SelectedTab.Text;

            if (activeTab != "Add New")
                return;

            PQdsAddCustom subForm = new PQdsAddCustom((int)this.m_channel.AssetID,this.m_evt.ID);
            subForm.ShowDialog();

            this.UpdateCustomFields();
        }

        #region[listBoxEdit]


        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            string activeTab = tabControl1.SelectedTab.Text;
            Control[] ctrls = tabControl1.SelectedTab.Controls.Find("listView1-" + activeTab, false);
            ListView list = (ListView)ctrls[0];

            ctrls = tabControl1.SelectedTab.Controls.Find("editkey", false);
            TextBox txtkey = (TextBox)ctrls[0];

            ctrls = tabControl1.SelectedTab.Controls.Find("editvalue", false);
            TextBox txtvalue = (TextBox)ctrls[0];

            ctrls = tabControl1.SelectedTab.Controls.Find("editType", false);
            ComboBox txtType = (ComboBox)ctrls[0];

            if (list.SelectedItems.Count == 0)
            {
                //HideTextEditor();
                return;
            }

            int index = (int)list.SelectedItems[0].Tag;

            int border = 0;
            switch (list.BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    border = 1;
                    break;
                case BorderStyle.Fixed3D:
                    border = 2;
                    break;
            }

            int CellWidth = list.SelectedItems[0].SubItems[0].Bounds.Width;
            int CellHeight = list.SelectedItems[0].SubItems[0].Bounds.Height;
            int CellLeft = border + list.Left + list.SelectedItems[0].SubItems[0].Bounds.Left;
            int CellTop = list.Top + list.SelectedItems[0].SubItems[0].Bounds.Top;

            txtkey.Location = new Point(CellLeft, CellTop);
            txtkey.Size = new Size(CellWidth, CellHeight);
            txtkey.Visible = true;
            txtkey.BringToFront();
            txtkey.Text = list.SelectedItems[0].SubItems[0].Text;

            CellWidth = list.SelectedItems[0].SubItems[1].Bounds.Width;
            CellHeight = list.SelectedItems[0].SubItems[1].Bounds.Height;
            CellLeft = border + list.Left + list.SelectedItems[0].SubItems[1].Bounds.Left;
            CellTop = list.Top + list.SelectedItems[0].SubItems[1].Bounds.Top;

            txtvalue.Location = new Point(CellLeft, CellTop);
            txtvalue.Size = new Size(CellWidth, CellHeight);
            txtvalue.Visible = true;
            txtvalue.BringToFront();
            txtvalue.Text = list.SelectedItems[0].SubItems[1].Text;

            CellWidth = list.SelectedItems[0].SubItems[2].Bounds.Width;
            CellHeight = list.SelectedItems[0].SubItems[2].Bounds.Height;
            CellLeft = border + list.Left + list.SelectedItems[0].SubItems[2].Bounds.Left;
            CellTop = list.Top + list.SelectedItems[0].SubItems[2].Bounds.Top;

            txtType.Location = new Point(CellLeft, CellTop);
            txtType.Size = new Size(CellWidth, CellHeight);
            txtType.Visible = true;
            txtType.BringToFront();
            txtType.Text = list.SelectedItems[0].SubItems[2].Text;

        }

        private void TxtEdit_Leave(object sender, EventArgs e)
        {
            SaveField();
        }

        private void SaveField()
        {
            if (this.alreadySavedFlag)
                return;

            this.alreadySavedFlag = true;
            string activeTab = tabControl1.SelectedTab.Text;
            Control[] ctrls = tabControl1.SelectedTab.Controls.Find("listView1-" + activeTab, false);
            ListView list = (ListView)ctrls[0];

            ctrls = tabControl1.SelectedTab.Controls.Find("editkey", false);
            TextBox txtkey = (TextBox)ctrls[0];

            ctrls = tabControl1.SelectedTab.Controls.Find("editvalue", false);
            TextBox txtvalue = (TextBox)ctrls[0];

            ctrls = tabControl1.SelectedTab.Controls.Find("editType", false);
            ComboBox txtType = (ComboBox)ctrls[0];

            int index = (int)list.SelectedItems[0].Tag;

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.CustomField> customFldTbl = new GSF.Data.Model.TableOperations<PQds.Model.CustomField>(connection);
                PQds.Model.CustomField fld;

                if (index == -1)
                {
                    fld = new Model.CustomField();
                    fld.domain = activeTab;
                    fld.AssetID = (int)this.m_channel.AssetID;
                    fld.EventID = (int)this.m_evt.ID;
                }
                else
                {
                    fld = customFldTbl.QueryRecordWhere("ID = {0}", index);
                }


                if (txtvalue.Text == "" && index != -1)
                {
                    DialogResult confirm = System.Windows.Forms.MessageBox.Show("This will delete the MetaDataTag " + fld.key, "Warning",
                        System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Warning);
                    if (confirm == DialogResult.OK)
                    {
                        customFldTbl.DeleteRecord(fld);
                        HideTextEditor();
                        this.UpdateCustomFields();
                        return;
                    }
                }

                fld.key = txtkey.Text;
                fld.Value = txtvalue.Text;
                fld.Type = TextToType(txtType.Text);

                customFldTbl.AddNewOrUpdateRecord(fld);
                HideTextEditor();
                this.UpdateCustomFields();
            }
        }

        private void TxtEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                SaveField();
            }
        }

        private void HideTextEditor()
        {

            string activeTab = tabControl1.SelectedTab.Text;
            Control[] ctrls = tabControl1.SelectedTab.Controls.Find("listView1-" + activeTab, false);
            ListView list = (ListView)ctrls[0];

            ctrls = tabControl1.SelectedTab.Controls.Find("editkey", false);
            TextBox txtkey = (TextBox)ctrls[0];

            ctrls = tabControl1.SelectedTab.Controls.Find("editvalue", false);
            TextBox txtvalue = (TextBox)ctrls[0];

            ctrls = tabControl1.SelectedTab.Controls.Find("editType", false);
            ComboBox txtType = (ComboBox)ctrls[0];

            txtType.Visible = false;
            txtkey.Visible = false;
            txtvalue.Visible = false;
        }

        #endregion[listBoxEdit]

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (Model.SignalType.ToValue((string)comboBox1.SelectedItem) != -1)
                this.m_channel.SignalType = Model.SignalType.ToValue((string)comboBox1.SelectedItem);
            else
                this.m_channel.SignalType = null;

            if (Model.MeasurementType.ToValue((string)comboBox2.SelectedItem) != "")
                this.m_channel.MeasurementType = Model.MeasurementType.ToValue((string)comboBox2.SelectedItem);
            else
                this.m_channel.MeasurementType = null;

            this.m_channel.MeterID = (comboDevice.SelectedItem as dynamic).Value;

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                (new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection)).UpdateRecord(this.m_channel);

                GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity> dataSensitivityTbl = new GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity>(connection);

                PQds.Model.DataSensitivity dataSensitivity;

                if (dataSensitivityTbl.QueryRecordCountWhere("Event = {0} AND Asset = {1}", this.m_evt.ID, this.m_channel.AssetID) > 0)
                {
                   dataSensitivity = dataSensitivityTbl.QueryRecordsWhere("Event = {0} AND Asset = {1}", this.m_evt.ID, this.m_channel.AssetID).First();
                }

                dataSensitivity = new Model.DataSensitivity() { Event = this.m_evt.ID, Asset = (int)this.m_channel.AssetID };
                dataSensitivity.DataSensitivityCode = Model.DataSensitivityCode.ToValue((string)comboBox3.SelectedItem);
                dataSensitivity.Note = DataSensitivityNoteText.Text;
                dataSensitivityTbl.UpdateRecord(dataSensitivity);
            }



            this.Close();

        }

        private void OnClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !this.tabControl1.Enabled)
            {
                if (e.X > tabControl1.Location.X && e.X < tabControl1.Location.X + tabControl1.Width
                    && e.Y > tabControl1.Location.Y && e.Y < tabControl1.Location.Y + tabControl1.Height)
                {
                    PQdsAddCustom subForm = new PQdsAddCustom((int)this.m_channel.AssetID, this.m_evt.ID);
                    subForm.ShowDialog();

                    this.UpdateCustomFields();
                    return;
                }

            }
        }

    }

    #endregion[Methods]

}
