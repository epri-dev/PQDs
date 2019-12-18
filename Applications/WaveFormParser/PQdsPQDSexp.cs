using GSF.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PQds
{
    public partial class PQdsPQDSexp : Form
    {
        public PQdsPQDSexp()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PQioPQDSexp_Load(object sender, EventArgs e)
        {
            //Load All Assets
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTable = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);

                List<PQds.Model.Asset> assets = assetTable.QueryRecordsWhere("(SELECT COUNT(DataSeries.ID) FROM DataSeries LEFT OUTER JOIN Channel ON Channel.ID = DataSeries.ChannelID WHERE Channel.AssetID = Asset.ID) > 0  ").ToList();
                this.chLstBoxAsset.Items.Clear();
                this.chLstBoxAsset.Items.AddRange(assets.ToArray());
                
            }

            UpdateEvents();
        }

        private void UpdateEvents()
        {
            this.chLstBoxEvt.Items.Clear();
            List<int> selectedAssetIDs = this.chLstBoxAsset.CheckedItems.OfType<Model.Asset>().Select(item => item.ID).ToList();

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Event> evtTable = new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection);
                string sqlQuerry = "((SELECT COUNT(DataSeries.ID) FROM DataSeries WHERE DataSeries.EventID = Event.ID) > 0) " +
                    "AND ({0} IN " +
                    "(SELECT Channel.AssetID FROM Channel LEFT JOIN DataSeries ON Channel.ID = DataSeries.ChannelID " +
                    "WHERE DataSeries.EventID = Event.ID )) ";

                foreach (int i in selectedAssetIDs)
                {
                    this.chLstBoxEvt.Items.AddRange(evtTable.QueryRecordsWhere(sqlQuerry,i).ToArray());
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Create a List of Tuples we will have to deal with....
            List<Tuple<PQds.Model.Event, PQds.Model.Asset>> files = new List<Tuple<Model.Event, Model.Asset>>();

            List<int> selectedEvtIDs = this.chLstBoxEvt.CheckedItems.OfType<Model.Event>().Select(item => item.ID).ToList();
            List<int> selectedAssetIDs = this.chLstBoxAsset.CheckedItems.OfType<Model.Asset>().Select(item => item.ID).ToList();

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.AssetToEvent> assetToEventTable = new GSF.Data.Model.TableOperations<PQds.Model.AssetToEvent>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTable = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.Event> eventTable = new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection);

                foreach (int assetID in selectedAssetIDs)
                {
                    List<int> associatedEvents = assetToEventTable.QueryRecordsWhere("AssetID = {0}", assetID).Select(item => item.EventID).ToList();
                    foreach(int eventID in selectedEvtIDs)
                    {
                        if(associatedEvents.Contains(eventID))
                        {
                            files.Add(new Tuple<Model.Event, Model.Asset>(eventTable.QueryRecordWhere("ID = {0}", eventID), assetTable.QueryRecordWhere("ID = {0}", assetID)));
                        }
                    }
                    
                }
            }

            
            if (files.Count() == 0)
            {
                MessageBox.Show("The selected assets and events do not contain any data. To export every asset from an event select all assets!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (files.Count() == 1 )
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.Filter = "PQDS file (*.csv)|*.csv";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    WritePQDSFile(files, new List<string>() { saveFileDialog1.FileName });
                }
            }
            else
            {

                DialogResult msg = MessageBox.Show("The selected assets and events will result in multiple PQDS files", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (msg == DialogResult.OK)
                {
                    FolderBrowserDialog saveFileDialog1 = new FolderBrowserDialog();
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        List<string> fileName = files.Select((item,index) => saveFileDialog1.SelectedPath + String.Format("/PQDS_{0}.csv", index)).ToList();
                        WritePQDSFile(files, fileName);
                    }
                }
            }

            this.Close();
        }

        private async void WritePQDSFile(List<Tuple<PQds.Model.Event, PQds.Model.Asset>> data, List<string> fileName)
        {
            string logfileName;
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Setting> settingTbl = new GSF.Data.Model.TableOperations<PQds.Model.Setting>(connection);
                logfileName = settingTbl.QueryRecordWhere("Name = {0}", "logfile.name").value;
            }

            FileParser.PQDSParser parser = new FileParser.PQDSParser(logfileName);

            PQdsProgress progressWindow = new PQdsProgress(fileName.Count() * 100);
            progressWindow.updateText(String.Format("Exporting file {0} out of {1}...", new object[2] { "{0}", fileName.Count() }));
            progressWindow.Show();

            if (fileName.Count() == 1)
            {
                if (radioButton1.Checked)
                {
                    await parser.WritePQDSFile(new Progress<int>(progressWindow.updateProgress), data[0].Item2, data[0].Item1, fileName[0], includeAuthorMetaData: chkAuthor.Checked, includeGUID: chkGUID.Checked);
                    progressWindow.Close();
                    return;
                }

                if (checkBox2.Checked)
                {
                    await parser.WritePQDSFile(new Progress<int>(progressWindow.updateProgress), data[0].Item2, data[0].Item1, fileName[0], 
                        chkDeviceMD.Checked, chkAssetMD.Checked, chkTimeMD.Checked, chkEvtMD.Checked, chkWaveForm.Checked, CustomMetaData.Checked, chkAuthor.Checked, chkGUID.Checked, dateTimePicker1.Value);
                }
                else
                {
                    await parser.WritePQDSFile(new Progress<int>(progressWindow.updateProgress), data[0].Item2, data[0].Item1, fileName[0],
                        chkDeviceMD.Checked, chkAssetMD.Checked, chkTimeMD.Checked, chkEvtMD.Checked, chkWaveForm.Checked, CustomMetaData.Checked, chkAuthor.Checked, chkGUID.Checked);
                }
            }
            else
            {
                if (radioButton1.Checked)
                {
                    await parser.WritePQDSFiles(new Progress<int>(progressWindow.updateProgress), data.Select(item => item.Item2).ToList(), data.Select(item => item.Item1).ToList(), fileName, includeAuthorMetaData: chkAuthor.Checked, includeGUID: chkGUID.Checked);
                    progressWindow.Close();
                    return;
                }

                if (checkBox2.Checked)
                {
                    await parser.WritePQDSFiles(new Progress<int>(progressWindow.updateProgress), data.Select(item => item.Item2).ToList(), data.Select(item => item.Item1).ToList(), fileName,
                        chkDeviceMD.Checked, chkAssetMD.Checked, chkTimeMD.Checked, chkEvtMD.Checked, chkWaveForm.Checked, CustomMetaData.Checked, chkAuthor.Checked, chkGUID.Checked, dateTimePicker1.Value);
                }
                else
                {
                    await parser.WritePQDSFiles(new Progress<int>(progressWindow.updateProgress), data.Select(item => item.Item2).ToList(), data.Select(item => item.Item1).ToList(), fileName,
                        chkDeviceMD.Checked, chkAssetMD.Checked, chkTimeMD.Checked, chkEvtMD.Checked, chkWaveForm.Checked, CustomMetaData.Checked, chkAuthor.Checked, chkGUID.Checked);
                }
            }
            progressWindow.Close();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            
            if (this.checkBox2.Checked)
            {
                this.chkTimeMD.Checked = true;
                DialogResult msg = MessageBox.Show("This will override the inclussion of Timing MetaData and set the EventDate and EventTime fields to the seleted date.", "Override Timing MetaData", 
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (msg == DialogResult.OK)
                {
                    this.dateTimePicker1.Enabled = this.checkBox2.Checked;
                    this.chkTimeMD.Enabled = !this.checkBox2.Checked;
                }
                else
                {
                    checkBox2.CheckedChanged -= checkBox2_CheckedChanged;
                    checkBox2.Checked = false;
                    checkBox2.CheckedChanged += checkBox2_CheckedChanged;
                }
            }
        }

        private void chLstBoxAsset_SelectedIndexChanged(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox clb = (CheckedListBox)sender;
            // Switch off event handler
            clb.ItemCheck -= chLstBoxAsset_SelectedIndexChanged;
            clb.SetItemCheckState(e.Index, e.NewValue);
            // Switch on event handler
            clb.ItemCheck += chLstBoxAsset_SelectedIndexChanged;

           
            UpdateEvents();

            if (chLstBoxEvt.CheckedIndices.Count > 0)
                button2.Enabled = true;
            else
                button2.Enabled = false;

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                chkAssetMD.Enabled = false;
                chkDeviceMD.Enabled = false;
                chkEvtMD.Enabled = false;
                chkTimeMD.Enabled = false;
                checkBox2.Enabled = false;
                dateTimePicker1.Enabled = false;
                chkWaveForm.Enabled = false;
                CustomMetaData.Enabled = false;
                //chkAuthor.Enabled = false;

            }
            else
            {
                chkAssetMD.Enabled = true;
                chkDeviceMD.Enabled = true;
                chkEvtMD.Enabled = true;
                chkTimeMD.Enabled = true;
                checkBox2.Enabled = true;
                chkWaveForm.Enabled = true;
                CustomMetaData.Enabled = true;
                //chkAuthor.Enabled = true;
                if (checkBox2.Checked)
                {
                    dateTimePicker1.Enabled = true;
                    chkTimeMD.Enabled = false;
                }
            }
        }

        private void chLstBoxEvt_SelectedIndexChanged(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox clb = (CheckedListBox)sender;
            // Switch off event handler
            clb.ItemCheck -= chLstBoxEvt_SelectedIndexChanged;
            clb.SetItemCheckState(e.Index, e.NewValue);
            // Switch on event handler
            clb.ItemCheck += chLstBoxEvt_SelectedIndexChanged;

            if (chLstBoxEvt.CheckedIndices.Count > 0)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }

        private void chkEvtMD_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEvtMD.Checked)
            {
                this.chkGUID.Checked = true;
                this.chkGUID.Enabled = false;
            }
            else
            {
                this.chkGUID.Enabled = true;
            }
        }
    }
}
