//******************************************************************************************************
//  PQioMain.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/05/2019 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data;
using GSF.Data;
using GSF.Configuration;
using GSF.IO;
using PQds.Model;

namespace PQds
{
    public partial class PQdsMain : Form
    {
        #region [ Members ]

        private string m_fileName;
        private Boolean m_ChartDragFlag;
        private Boolean m_hasData;

        #endregion [ Members ]

        #region [ Constructor ]

        /// <summary>
        /// Creates an instance of <see cref="PQdsMain"/> class.
        /// </summary>
        public PQdsMain()
        {
            ConfigurationFile configFile = ConfigurationFile.Current;
            this.m_hasData = false;
            InitializeComponent();
        }

        #endregion

        #region [ Methods ]

        private void Form_Resize(object sender, EventArgs e)
        {
            // Calculate Vertical Divisions

            this.panel1.Width = this.Size.Width - 170;
            this.Panel2.Width = this.Size.Width - 170;

            this.MetaDataTree.Width = (this.panel1.Width - 25) * 390 / 875;
            this.ChannelTree.Left = this.MetaDataTree.Width + 10;
            this.ChannelTree.Width = (this.panel1.Width - 25) * 295 / 875;
            this.EvtList.Left = this.ChannelTree.Right + 5;
            this.ChannelMetaData.Left = this.EvtList.Left;
            this.EvtList.Width = (this.panel1.Width - 25) * 190 / 875;
            this.ChannelMetaData.Width = (this.panel1.Width - 25) * 190 / 875;

            this.label3.Left = this.ChannelTree.Left;
            this.label1.Left = this.EvtList.Left;
            this.label4.Left = this.EvtList.Left;

            this.DataChart2.Width = (this.panel1.Width - 20) / 2;
            this.DataChart1.Left = (this.panel1.Width - 20) / 2 + 10;
            this.DataChart1.Width = (this.panel1.Width - 20) / 2;

            // Calculate Horizontal Divisions
            this.panel1.Height = (this.Size.Height - 56) * 344 / 544;
            this.Panel2.Top = this.panel1.Height + 11;
            this.Panel2.Height = (this.Size.Height - 56) * 200 / 544;

            this.DataChart1.Height = this.Panel2.Height - 5;
            this.DataChart2.Height = this.Panel2.Height - 5;

            this.MetaDataTree.Height = this.panel1.Height - 31;
            this.ChannelTree.Height = this.panel1.Height - 31;

            this.ChannelMetaData.Height = (this.panel1.Height - 50) * 163 / 294;
            this.EvtList.Height = (this.panel1.Height - 50) * 131 / 294;
            this.ChannelMetaData.Top = this.EvtList.Height + 39;
            this.label4.Top = this.EvtList.Height + 23;

            // Button Panel
            this.panel3.Height = this.Size.Height - 50;
            this.pictureBox1.Top = this.panel3.Height - 87;
            this.button1.Top = this.pictureBox1.Top - 31;
        }

        private async void OPENFILEButton_Click(object sender, EventArgs e)
        {
            string directory;
            string rootFileName;
            string fileExtension;

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "PQDIF Files|*.pqd|PQDS Files|*.csv|All Files|*.*";
                dialog.Title = "Browse Files";
                if (dialog.ShowDialog() == DialogResult.Cancel)
                    return;

                if (!File.Exists(dialog.FileName))
                    return;

                directory = Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
                rootFileName = FilePath.GetFileNameWithoutExtension(dialog.FileName);
                fileExtension = Path.GetExtension(dialog.FileName);

                PQdsProgress progressWindow = new PQdsProgress(100);

                if (fileExtension == ".dat" || fileExtension == ".d00") { }
                else if (fileExtension == ".pqd")
                {
                    progressWindow.updateText("Loading PQDIF file...");
                    progressWindow.Show();

                    FileParser.PQDIFFparser parser = new FileParser.PQDIFFparser();
                    bool success = await parser.ParsePQDIFFile(dialog.FileName, new Progress<int>(progressWindow.updateProgress));
                    if (!success)
                    {
                        progressWindow.Close();
                        MessageBox.Show(String.Format("File {0} could not be loaded.",dialog.SafeFileName), "Loading PQDIF", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    else
                    {
                        progressWindow.Close();
                        m_fileName = dialog.SafeFileName;
                        Text = string.Format("PQds PQDIF - [{0}]", dialog.SafeFileName);
                        CSVExportButton.Enabled = true;
                        this.m_hasData = true;
                    }
                }
                else if (fileExtension == ".csv")
                {
                    
                    progressWindow.updateText("Loading PQDS file...");
                    progressWindow.Show();

                    FileParser.PQDSParser parser = new FileParser.PQDSParser("");
                    bool success = await parser.ParsePQDSFile(dialog.FileName, new Progress<int>(progressWindow.updateProgress));
                    if (!success)
                    {
                        progressWindow.Close();
                        MessageBox.Show(String.Format("File {0} could not be loaded.", dialog.SafeFileName), "Loading PQDS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        progressWindow.Close();
                        m_fileName = dialog.SafeFileName;
                        this.Text = string.Format("PQds PQDS - [{0}]", dialog.SafeFileName);
                        CSVExportButton.Enabled = true;
                        this.m_hasData = true;
                    }
                }

                //Update MetaData and ChannelList
                UpdateMetaDataSection();
                UpdateChannelList();

            }

        }

        private void RecalculateAxes(Chart chart)
        {
            double xMin, xMax;
            double yMin, yMax;
            double xScale, yScale;

            // Get the absolute smallest and largest x-values and y-values of the data points on the chart
            xMin = chart.Series.Select(series => series.Points.Select(point => point.XValue).Min()).Min();
            xMax = chart.Series.Select(series => series.Points.Select(point => point.XValue).Max()).Max();
            yMin = chart.Series.Select(series => series.Points.Select(point => point.YValues.Min()).Min()).Min();
            yMax = chart.Series.Select(series => series.Points.Select(point => point.YValues.Max()).Max()).Max();

            // Determine scale factor
            xScale = GetChartScale(xMax - xMin);
            yScale = GetChartScale(yMax - yMin);

            // Apply scale to make axis labels more readable
            xMin = xScale * Math.Floor(xMin / xScale);
            xMax = xScale * Math.Ceiling(xMax / xScale);
            yMin = yScale * Math.Floor(yMin / yScale);
            yMax = yScale * Math.Ceiling(yMax / yScale);

            // If the difference between the min an max values is
            // zero, add some space so we do not encounter an error
            if (xMax - xMin == 0.0D)
            {
                xMin -= 0.5D;
                xMax += 0.5D;
            }

            if (yMax - yMin == 0.0D)
            {
                yMin -= 0.5D;
                yMax += 0.5D;
            }

            // Set min, max, and interval of each axis
            chart.ChartAreas[0].AxisX.Minimum = xMin;
            chart.ChartAreas[0].AxisX.Maximum = xMax;
            chart.ChartAreas[0].AxisY.Minimum = yMin;
            chart.ChartAreas[0].AxisY.Maximum = yMax;
            chart.ChartAreas[0].AxisX.Interval = (xMax - xMin) / 10.0D;
            chart.ChartAreas[0].AxisY.Interval = (yMax - yMin) / 10.0D;
        }

        private double GetChartScale(double diff)
        {
            double abs = Math.Abs(diff);
            double log = Math.Log10(abs);
            return (diff == 0.0D) ? 1.0D : Math.Pow(10.0D, Math.Floor(log) - 1.0D);
        }

        #region[MetaData]

        private void UpdateMetaDataSection()
        {
            //Clear MetaDataTree
            MetaDataTree.BeginUpdate();
            MetaDataTree.Nodes.Clear();

            //Create Nodes
            TreeNode generalNode = new TreeNode("Data Sensitivity");
            generalNode.Tag = "general";

            generalNode.Nodes.AddRange(GetWaveFormMetaData().ToArray());

            TreeNode assetNode = new TreeNode("Assets");
            assetNode.Tag = "asset";

            assetNode.Nodes.AddRange(GetAssetMetaData().ToArray());

            TreeNode deviceNode = new TreeNode("Devices");
            deviceNode.Tag = "device";

            deviceNode.Nodes.AddRange(GetDeviceMetaData().ToArray());

            TreeNode eventNode = new TreeNode("Events");
            eventNode.Tag = "event";

            eventNode.Nodes.AddRange(GetEventMetaData().ToArray());

            //Add Nodes to Tree
            MetaDataTree.Nodes.Add(generalNode);
            MetaDataTree.Nodes.Add(assetNode);
            MetaDataTree.Nodes.Add(deviceNode);
            MetaDataTree.Nodes.Add(eventNode);


            MetaDataTree.EndUpdate();


        }

        private List<TreeNode> GetWaveFormMetaData()
        {
            List<TreeNode> nodes = new List<TreeNode>(3);

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                //DataType
                GSF.Data.Model.TableOperations<PQds.Model.Channel> channelTable = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity> dataSensitivityTbl = new GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity>(connection);

                if (channelTable.QueryRecords().Select(record => record.SignalType).Distinct().Count() == 1)
                {
                    GSF.Data.Model.TableOperations<PQds.Model.SignalType> signalTypeTable = new GSF.Data.Model.TableOperations<PQds.Model.SignalType>(connection);
                    int? signalType = channelTable.QueryRecords().First().SignalType;

                    if (signalType != null)
                    {
                        TreeNode datatype = new TreeNode(string.Format("Data Type: {0} ({1})", Model.SignalType.ToDisplay((int)signalType), signalType));
                        datatype.Tag = "DataType";
                        nodes.Add(datatype);
                    }
                }

                //Data Sensitivity
                
                if (DataSensitivity.CodeisGlobal())
                {
                    int? sesitivity = dataSensitivityTbl.QueryRecordsWhere("DataSensitivityCode NOT NULL").First().DataSensitivityCode;
                    TreeNode datasensitivity = new TreeNode(string.Format("Data Sensitivity: {0} ({1})", Model.DataSensitivityCode.ToDisplay((int)sesitivity), sesitivity));
                    datasensitivity.Tag = "DataSensitivity";
                    nodes.Add(datasensitivity);

                }

               
                if (DataSensitivity.NoteisGlobal())
                {
                    string note = dataSensitivityTbl.QueryRecordsWhere("Note NOT NULL AND Note <> ''").First().Note;
                    TreeNode datasensitivitynote = new TreeNode(string.Format("Data Sensitivity Note: {0}", shorten(note,10)));
                    datasensitivitynote.Tag = "DataSensitivityNote";
                    nodes.Add(datasensitivitynote);

                }

            }

            return nodes;

        }

        private List<TreeNode> GetAssetMetaData()
        {
            List<TreeNode> nodes = new List<TreeNode>();
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {

                GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTable = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);
                nodes = assetTable.QueryRecords().Select(AssetToNode).ToList();
            }
            return nodes;
        }

        private List<TreeNode> GetDeviceMetaData()
        {
            List<TreeNode> nodes = new List<TreeNode>();
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {

                GSF.Data.Model.TableOperations<PQds.Model.Meter> deviceTable = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);
                nodes = deviceTable.QueryRecords().Select(DeviceToNode).ToList();
            }
            return nodes;
        }

        private List<TreeNode> GetEventMetaData()
        {
            List<TreeNode> nodes = new List<TreeNode>();
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {

                GSF.Data.Model.TableOperations<PQds.Model.Event> eventTable = new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection);
                nodes = eventTable.QueryRecords().Select(EventToNode).ToList();
            }
            return nodes;
        }

        private TreeNode AssetToNode(PQds.Model.Asset asset)
        {
            TreeNode node = new TreeNode(string.Format("Asset {0}", shorten(asset.AssetKey,10)));

            if (asset.AssetKey != null)
            {
                node.Nodes.Add(string.Format("Name: {0}", shorten(asset.AssetKey,15)));
            }
            if (asset.NominalVoltage != null)
            {
                node.Nodes.Add(string.Format("Nom. Voltage (L-G): {0}", asset.NominalVoltage));
            }
            if (asset.NominalFrequency != null)
            {
                node.Nodes.Add(string.Format("Nom. Freq: {0}", asset.NominalFrequency));
            }
            if (asset.UpstreamXFMR != null)
            {
                node.Nodes.Add(string.Format("Upstream XFR (kVA): {0}", asset.UpstreamXFMR));
            }
            if (asset.Length != null)
            {
                node.Nodes.Add(string.Format("Length (miles): {0}", asset.Length));
            }
            node.Tag = asset.ID;

            return node;
        }

        private TreeNode DeviceToNode(PQds.Model.Meter device)
        {
            TreeNode node;
            if (device.DeviceAlias != null)
                node = new TreeNode(string.Format("Meter {0}", shorten(device.DeviceAlias,10)));
            else
                node = new TreeNode(string.Format("Meter {0}", shorten(device.DeviceName, 10)));

            node.Nodes.Add(string.Format("Name: {0}", shorten(device.DeviceName,20)));

            if (device.DeviceAlias != null)
            {
                node.Nodes.Add(string.Format("Alias: {0}", shorten(device.DeviceAlias,15)));
            }

            if (!(device.DeviceLocation is null))
            {
                node.Nodes.Add(string.Format("Location: {0}", shorten(device.DeviceLocation,20)));
            }

            if (device.DeviceLocationAlias != null)
            {
                node.Nodes.Add(string.Format("Location Alias: {0}", shorten(device.DeviceLocationAlias,20)));
            }

            if (device.Latitude != null)
            {
                node.Nodes.Add(string.Format("Latitude: {0}", device.Latitude));
            }
            if (device.Longitude != null)
            {
                node.Nodes.Add(string.Format("Longitude: {0}", device.Longitude));
            }

            if (device.AccountName != null)
            {
                node.Nodes.Add(string.Format("Account: {0}", shorten(device.AccountName,20)));
            }
            if (device.AccountAlias != null)
            {
                node.Nodes.Add(string.Format("Account Alias: {0}", shorten(device.AccountAlias,15)));
            }

            if (device.DistanceToXFR != null)
            {
                node.Nodes.Add(string.Format("Distance to XFMR: {0}", device.DistanceToXFR));
            }

            if (device.ConnectionType != null)
            {
                node.Nodes.Add(string.Format("Event Type: {0} ({1})", Model.ConnectionType.ToDisplay(device.ConnectionType), device.ConnectionType));
            }

            if (!(device.Owner is null))
            {
                node.Nodes.Add(string.Format("Owner: {0}", shorten(device.Owner,20)));
            }

            node.Tag = device.ID;

            return node;
        }

        private TreeNode EventToNode(PQds.Model.Event evt)
        {
            TreeNode node = new TreeNode(string.Format("Event {0}", shorten(evt.Name,15)));
            node.Nodes.Add(string.Format("GUID: {0}", evt.GUID));
            node.Nodes.Add(string.Format("Name: {0}", shorten(evt.Name,20)));

            if (evt.EventTime != null)
            {
                node.Nodes.Add(string.Format("Year: {0}", ((DateTime)evt.EventTime).Date.Year));
                node.Nodes.Add(string.Format("Month: {0}", ((DateTime)evt.EventTime).Date.Month));
                node.Nodes.Add(string.Format("Day of Month: {0}", ((DateTime)evt.EventTime).Date.Day));

                node.Nodes.Add(string.Format("Hour: {0}", ((DateTime)evt.EventTime).Hour));
                node.Nodes.Add(string.Format("Minute: {0}", ((DateTime)evt.EventTime).Minute));
                node.Nodes.Add(string.Format("Second: {0}", ((DateTime)evt.EventTime).Second));
                node.Nodes.Add(string.Format("Nano Seconds: {0}", Get_nanoseconds(((DateTime)evt.EventTime))));

                node.Nodes.Add(string.Format("Date: {0}", ((DateTime)evt.EventTime).ToString("MM/dd/yyyy")));
                node.Nodes.Add(string.Format("Time: {0}", ((DateTime)evt.EventTime).TimeOfDay));
            }

            if (evt.EventType != null)
            {
                node.Nodes.Add(string.Format("Event Type: {0} ({1})", Model.EventType.ToDisplay(evt.EventType), evt.EventType));
            }
            if (evt.FaultType != null)
            {
                node.Nodes.Add(string.Format("Fault Type: {0} ({1})", Model.FaultType.ToDisplay(evt.FaultType), evt.FaultType));
            }


            if (evt.PeakCurrent != null)
            {
                node.Nodes.Add(string.Format("Peak Current (A): {0}", evt.PeakCurrent));
            }

            if (evt.PeakVoltage != null)
            {
                node.Nodes.Add(string.Format("Peak Voltage (V): {0}", evt.PeakVoltage));
            }


            if (evt.MaxVA != null)
            {
                node.Nodes.Add(string.Format("Max Voltage A: {0}", evt.MaxVA));
            }
            if (evt.MaxVB != null)
            {
                node.Nodes.Add(string.Format("Max Voltage B: {0}", evt.MaxVB));
            }
            if (evt.MaxVC != null)
            {
                node.Nodes.Add(string.Format("Max Voltage C: {0}", evt.MaxVC));
            }

            if (evt.MinVA != null)
            {
                node.Nodes.Add(string.Format("Min Voltage A: {0}", evt.MinVA));
            }
            if (evt.MinVB != null)
            {
                node.Nodes.Add(string.Format("Min Voltage B: {0}", evt.MinVB));
            }
            if (evt.MinVC != null)
            {
                node.Nodes.Add(string.Format("Min Voltage C: {0}", evt.MinVC));
            }




            if (evt.MaxIA != null)
            {
                node.Nodes.Add(string.Format("Max Current A: {0}", evt.MaxIA));
            }
            if (evt.MaxIB != null)
            {
                node.Nodes.Add(string.Format("Max Current B: {0}", evt.MaxIB));
            }
            if (evt.MaxIC != null)
            {
                node.Nodes.Add(string.Format("Max Current C: {0}", evt.MaxIC));
            }

            if (evt.FaultI2T != null)
            {
                node.Nodes.Add(string.Format("Fault I2(t): {0}", evt.FaultI2T));
            }
            if (evt.PreEventCurrent != null)
            {
                node.Nodes.Add(string.Format("Pre Event Current: {0}", evt.PreEventCurrent));
            }
            if (evt.PreEventVoltage != null)
            {
                node.Nodes.Add(string.Format("Pre Event Voltage: {0}", evt.PreEventVoltage));
            }

            if (evt.Duration != null)
            {
                node.Nodes.Add(string.Format("Fault Duration (ms): {0}", evt.Duration));
            }
            if (evt.DistanceToFault != null)
            {
                node.Nodes.Add(string.Format("Distance to Fault: {0}", evt.DistanceToFault));
            }

            if (evt.FaultCause != null)
            {
                node.Nodes.Add(string.Format("Fault Cause: {0} ({1})", Model.FaultCause.ToDisplay(evt.FaultCause), evt.FaultCause));
            }


            node.Tag = evt.ID;

            return node;
        }

        private int Get_nanoseconds(DateTime date)
        {
            TimeSpan day = date.TimeOfDay;
            long result = day.Ticks;
            result = result - (long)day.Hours * (60L * 60L * 10000000L);
            result = result - (long)day.Minutes * (60L * 10000000L);
            result = result - (long)day.Seconds * 10000000L;


            return ((int)result * 100);
        }

        private string shorten(string input, int length)
        {
            if (input.Length > length)
            {
                input = input.Substring(0, length - 3) + "...";
            }
            
            return input;
        }

        #endregion[MetaData]

        #region[ChannelInformation]

        private void UpdateChannelList()
        {
            // Clear the list box and data chart
            ChannelTree.BeginUpdate();
            ChannelTree.Nodes.Clear();
            ChannelTree.AllowDrop = true;

            //Create Nodes
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTable = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.Channel> channelViewTable = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);

                ChannelTree.Nodes.AddRange(assetTable.QueryRecords().Select(ChannelList).ToArray());

                TreeNode unassignedNode = new TreeNode("Not Assigned");
                unassignedNode.Tag = -1;

                unassignedNode.Nodes.AddRange(channelViewTable.QueryRecordsWhere("AssetID IS NULL").Select(ChannelNode).ToArray());
                ChannelTree.Nodes.Add(unassignedNode);
            }

            ChannelTree.EndUpdate();
            DataChart1.Series.Clear();


        }

        private TreeNode ChannelList(PQds.Model.Asset asset)
        {
            TreeNode assetNode = new TreeNode(string.Format("Asset: {0}", asset.AssetKey));
            assetNode.Tag = asset.ID;

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Channel> channeTable = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);
                assetNode.Nodes.AddRange(channeTable.QueryRecordsWhere("AssetID = {0}", asset.ID).Select(ChannelNode).ToArray());
            }

            return assetNode;
        }

        private TreeNode ChannelNode(PQds.Model.Channel channel)
        {
            TreeNode channelNode = new TreeNode();

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Meter> meterTable = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);


                PQds.Model.Meter meter = meterTable.QueryRecordWhere("ID = {0}", channel.MeterID);

                if (channel.MeasurementType == MeasurementType.other)
                {
                    channelNode = new TreeNode(string.Format("({0} - NA) {1} ", meter.DeviceName, channel.Name));
                }
                else
                {
                    channelNode = new TreeNode(string.Format("({0} - {1}) {2} ", meter.DeviceName, channel.MeasurementType, channel.Name));
                }

                channelNode.Tag = channel.ID;
            }


            return channelNode;
        }

        private Boolean IsChannel(TreeNode node)
        {
            if (node is null) { return false; }
            if (node.Parent is null) { return false; }
            return true;
        }

        private void ChannelTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.
            TreeNode node = (TreeNode)e.Item;
            if (node is null) { return; }

            if ((e.Button == MouseButtons.Left) && (IsChannel((TreeNode)e.Item)))
            {
                this.m_ChartDragFlag = false;
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        private void ChannelTree_DragEnter(object sender, DragEventArgs e)
        {
            if (this.m_ChartDragFlag == false)
            {
                e.Effect = e.AllowedEffect;
            }
        }

        private void ChannelTree_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.
            System.Drawing.Point targetPoint = ChannelTree.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Select the line this is being dragged to
            TreeNode node = ChannelTree.GetNodeAt(targetPoint);
            if (IsChannel(node))
            {
                ChannelTree.SelectedNode = node.Parent;
            }
            else
            {
                ChannelTree.SelectedNode = node;
            }
        }

        private void ChannelTree_DragDrop(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            if (this.m_ChartDragFlag == false)
            {
                System.Drawing.Point targetPoint = ChannelTree.PointToClient(new System.Drawing.Point(e.X, e.Y));
                TreeNode targetNode = ChannelTree.GetNodeAt(targetPoint);

                if (IsChannel(targetNode))
                {
                    targetNode = targetNode.Parent;
                }

                // Retrieve the node that was dragged.
                TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    GSF.Data.Model.TableOperations<PQds.Model.Channel> channelTbl = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);
                    PQds.Model.Channel channel = channelTbl.QueryRecordWhere("ID = {0}", (int)draggedNode.Tag);

                    if (channel is null)
                    {
                        return;
                    }

                    //Dragged none to none
                    if ((channel.AssetID == null) && ((targetNode is null)|| ((int)targetNode.Tag < 0)))
                    {
                        return;
                    }
                    //Dragged none to something
                    else if (channel.AssetID == null)
                    {
                        channel.AssetID = (int)targetNode.Tag;
                    }
                    //Dragged something to none
                    else if ((targetNode is null) || ((int)targetNode.Tag < 0))
                    {
                        channel.AssetID = null;
                    }
                    else
                    {
                        channel.AssetID = (int)targetNode.Tag;
                    }
                    channelTbl.UpdateRecord(channel);

                }

                UpdateChannelList();
            }

        }

        #endregion[ChannelInformation]

        #region [EventInformation]

        private void UpdateEventList()
        {
            // Clear the list
            EvtList.Nodes.Clear();

            if (ChannelTree.SelectedNode is null) { return; }
            if (!IsChannel(ChannelTree.SelectedNode)) { return; }

            //Create Nodes
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.DataSeries> dataSeriesTable = new GSF.Data.Model.TableOperations<PQds.Model.DataSeries>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.Channel> channelTbl = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);


                EvtList.Nodes.AddRange(dataSeriesTable.QueryRecordsWhere("ChannelID = {0}", (int)ChannelTree.SelectedNode.Tag).Select(EvtNode).ToArray());


                UpdateChannelMetaData(channelTbl.QueryRecordWhere("ID = {0}", (int)ChannelTree.SelectedNode.Tag));
            }



        }

        private TreeNode EvtNode(PQds.Model.DataSeries dataSeries)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Event> evtTable = new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection);

                PQds.Model.Event evt = evtTable.QueryRecordWhere("ID = {0}", dataSeries.EventID);

                TreeNode node = new TreeNode(string.Format("({0}) {1}", evt.Name, evt.EventTime));
                node.Tag = dataSeries.ID;


                return node;
            }

        }

        private void UpdateChannelMetaData(PQds.Model.Channel channel)
        {

            //Clear MetaDataTree
            ChannelMetaData.BeginUpdate();
            ChannelMetaData.Nodes.Clear();

            //Create Nodes
            if (!(channel.Name is null))
            {
                ChannelMetaData.Nodes.Add(string.Format("Name: {0}", channel.Name));
            }

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.CustomField> customFldTbl = new GSF.Data.Model.TableOperations<PQds.Model.CustomField>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.DataSeries> dataSeriesTable = new GSF.Data.Model.TableOperations<PQds.Model.DataSeries>(connection);

                GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTbl = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.Meter> deviceTbl = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);
                
                Asset asset = assetTbl.QueryRecordWhere("ID = {0}", channel.AssetID);
                Meter device = deviceTbl.QueryRecordWhere("ID = {0}", channel.MeterID);
                

                if (asset != null)
                {
                    ChannelMetaData.Nodes.Add(string.Format("Asset: {0}", asset.AssetKey));
                }
                if (device != null)
                {
                    ChannelMetaData.Nodes.Add(string.Format("Device: {0}", device.DeviceName));
                }
                if (channel.SignalType != null)
                {
                    TreeNode datatype = new TreeNode(string.Format("Data Type: {0} ({1})", Model.SignalType.ToDisplay((int)channel.SignalType), channel.SignalType));
                }
                if (channel.MeasurementType != MeasurementType.other)
                {
                    ChannelMetaData.Nodes.Add(string.Format("Measurement: {0} ({1})", MeasurementType.ToDisplay(channel.MeasurementType), channel.MeasurementType));
                }
                //if (channel.DataSensitivity != null)
                //{
                //    ChannelMetaData.Nodes.Add(string.Format("Data Sensitivity Code: {0} ({1})", Model.DataSensitivityCode.ToDisplay((int)channel.DataSensitivity), channel.DataSensitivity));
                //}

                if ((asset != null)&&(this.EvtList.SelectedNode != null))
                {

                    DataSeries dataSeries = dataSeriesTable.QueryRecordWhere("ID = {0}", (int)this.EvtList.SelectedNode.Tag);

                    if (dataSeries != null)
                    {
                        int nCustom = customFldTbl.QueryRecordCountWhere("AssetID = {0} AND EventID = {1} ", asset.ID, dataSeries.EventID);

                        if (nCustom > 0)
                        {
                            TreeNode custom = new TreeNode("Custom Metadata");
                            
                            List<string> HeaderFlds = customFldTbl.QueryRecordsWhere("AssetID = {0} AND EventID = {1} ", asset.ID, dataSeries.EventID).Select(item => item.domain).Distinct().ToList();

                            foreach (string domainName in HeaderFlds)
                            {
                                TreeNode domain = new TreeNode("Domain: " + domainName);
                                domain.Nodes.AddRange(customFldTbl.QueryRecordsWhere("AssetID = {0} AND EventID = {1} AND Domain = {2}", asset.ID, dataSeries.EventID, domainName).Select(item =>
                                    new TreeNode(item.key + ": " + item.Value)
                                    ).ToArray());
                                custom.Nodes.Add(domain);
                            }
                            this.ChannelMetaData.Nodes.Add(custom);
                        }
                    }
                }

            }

            ChannelMetaData.EndUpdate();
        }

        #endregion [EventInformation]             

        private void FileViewer_Load(object sender, EventArgs e)
        {
            DialogResult license = new DialogResult();

            using (PQdsSplashScreen subForm = new PQdsSplashScreen())
            {
                license = subForm.ShowDialog();
            }

            if (license != DialogResult.OK)
            { this.Close(); return; }

            // Clean DataBase
            CleanDB();

            UpdateMetaDataSection();
            UpdateChannelList();

            this.DataChart1.Series.Clear();
            this.DataChart2.Series.Clear();

            this.contextMenuStrip1.Items.Add("Clear", null, new EventHandler(ClearChart1));
            this.contextMenuStrip2.Items.Add("Clear", null, new EventHandler(ClearChart2));



        }

        private void CleanDB()
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                connection.ExecuteScalar("DELETE FROM CustomField");
                connection.ExecuteScalar("DELETE FROM DataSeries");
                connection.ExecuteScalar("DELETE FROM Channel");

                connection.ExecuteScalar("DELETE FROM Meter");
                connection.ExecuteScalar("DELETE FROM Asset");
                connection.ExecuteScalar("DELETE FROM Event");
                connection.ExecuteScalar("DELETE FROM DataSensitivity");

            }
        }

        private void PopulateGlobalMetaDataMenue(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.GlobalMetaDataContextMenue.Items.Clear();
            // Retrieve the client coordinates of the mouse position.
            System.Drawing.Point targetPoint = MetaDataTree.PointToClient(this.GlobalMetaDataContextMenue.Location);

            // Select the node this is being clicked on
            TreeNode node = MetaDataTree.GetNodeAt(targetPoint);

            if (this.m_hasData)
            {
                MetaDataMenueStandard();

                if (node != null)
                {
                    if (isNodeAsset(node) && NodeToID(node)!= -1)
                    {
                        
                        ToolStripMenuItem editAsset = new ToolStripMenuItem("edit this asset");
                        editAsset.Click += new EventHandler((object sender1, EventArgs e1) =>
                        {
                            using (PQdsAsset subForm = new PQdsAsset(NodeToID(node)))
                            {
                                subForm.FormClosed += delegate
                                {
                                    this.UpdateMetaDataSection();
                                    this.UpdateChannelList();
                                };
                                subForm.ShowDialog();
                            }
                        });

                        ToolStripMenuItem deleteAsset = new ToolStripMenuItem("delete this asset");
                        deleteAsset.Click += new EventHandler((object sender1, EventArgs e1) =>
                        {
                            RemoveAsset(NodeToID(node));                           
                        });

                        GlobalMetaDataContextMenue.Items.Add(editAsset);
                        GlobalMetaDataContextMenue.Items.Add(deleteAsset);
                    }


                    if (isNodeDevice(node) && NodeToID(node) != -1)
                    {

                        ToolStripMenuItem editDevice = new ToolStripMenuItem("edit this device");
                        editDevice.Click += new EventHandler((object sender1, EventArgs e1) =>
                        {
                            using (PQdsDevice subForm = new PQdsDevice(NodeToID(node)))
                            {
                                subForm.FormClosed += delegate
                                {
                                    this.UpdateMetaDataSection();
                                    this.UpdateChannelList();
                                };
                                subForm.ShowDialog();
                            }
                        });

                        ToolStripMenuItem deleteDevice = new ToolStripMenuItem("delete this device");
                        deleteDevice.Click += new EventHandler((object sender1, EventArgs e1) =>
                        {
                            RemoveDevice(NodeToID(node));
                        });

                        GlobalMetaDataContextMenue.Items.Add(editDevice);
                        GlobalMetaDataContextMenue.Items.Add(deleteDevice);
                    }

                    if (isNodeEvent(node) && NodeToID(node) != -1)
                    {

                        ToolStripMenuItem editEvent = new ToolStripMenuItem("edit this event");
                        editEvent.Click += new EventHandler((object sender1, EventArgs e1) =>
                        {
                            using (PQdsEvent subForm = new PQdsEvent(NodeToID(node)))
                            {
                                subForm.FormClosed += delegate
                                {
                                    this.UpdateMetaDataSection();
                                    this.UpdateChannelList();
                                };
                                subForm.ShowDialog();
                            }
                        });

                        ToolStripMenuItem deleteEvent = new ToolStripMenuItem("delete this event");
                        deleteEvent.Click += new EventHandler((object sender1, EventArgs e1) =>
                        {
                            RemoveEvent(NodeToID(node));
                        });

                        GlobalMetaDataContextMenue.Items.Add(editEvent);
                        GlobalMetaDataContextMenue.Items.Add(deleteEvent);
                    }


                }

            }
        }

        private void MetaDataMenueStandard()
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {

                ToolStripMenuItem dataSensitivityItem = new ToolStripMenuItem("Data Sensitivity");
                dataSensitivityItem.Click += new EventHandler(OpenDataSensitivity);

                //For each Asset
                ToolStripMenuItem assetMenueItem = new ToolStripMenuItem("Assets");
                GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTable = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);
                List<Asset> assetList = assetTable.QueryRecords("").ToList();
                foreach (Asset asset in assetList)
                {
                    ToolStripMenuItem assetItem = new ToolStripMenuItem(String.Format("edit {0}", shorten(asset.AssetKey, 10)));
                    assetItem.Click += new EventHandler((object sender, EventArgs e) =>
                    {
                        using (PQdsAsset subForm = new PQdsAsset(asset.ID))
                        {
                            subForm.FormClosed += delegate
                            {
                                this.UpdateMetaDataSection();
                                this.UpdateChannelList();
                            };
                            subForm.ShowDialog();
                        }
                    });

                    assetMenueItem.DropDownItems.Add(assetItem);
                }


                ToolStripMenuItem newAssetItem = new ToolStripMenuItem("add new Asset");
                newAssetItem.Click += new EventHandler((object sender, EventArgs e) =>
                {
                    using (PQdsAsset subForm = new PQdsAsset(-1))
                    {
                        subForm.FormClosed += delegate
                        {
                            this.UpdateMetaDataSection();
                            this.UpdateChannelList();
                        };
                        subForm.ShowDialog();
                    }
                });
                assetMenueItem.DropDownItems.Add(newAssetItem);

                // For each Device
                ToolStripMenuItem deviceMenueItem = new ToolStripMenuItem("Devices");
                GSF.Data.Model.TableOperations<PQds.Model.Meter> deviceTable = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);
                List<Meter> deviceList = deviceTable.QueryRecords("").ToList();
                foreach (Meter device in deviceList)
                {
                    ToolStripMenuItem deviceItem;
                    if (device.DeviceAlias != null)
                        deviceItem  = new ToolStripMenuItem(String.Format("edit {0}", shorten(device.DeviceAlias, 10)));
                    else
                        deviceItem = new ToolStripMenuItem(String.Format("edit {0}", shorten(device.DeviceName, 10)));

                    deviceItem.Click += new EventHandler((object sender, EventArgs e) =>
                    {
                        using (PQdsDevice subForm = new PQdsDevice(device.ID))
                        {
                            subForm.FormClosed += delegate
                            {
                                this.UpdateMetaDataSection();
                                this.UpdateChannelList();
                            };
                            subForm.ShowDialog();
                        }
                    });

                    deviceMenueItem.DropDownItems.Add(deviceItem);
                }

                ToolStripMenuItem newDeviceItem = new ToolStripMenuItem("add new Device");
                newDeviceItem.Click += new EventHandler((object sender, EventArgs e) =>
                {
                    using (PQdsDevice subForm = new PQdsDevice(-1))
                    {
                        subForm.FormClosed += delegate
                        {
                            this.UpdateMetaDataSection();
                            this.UpdateChannelList();
                        };
                        subForm.ShowDialog();
                    }
                });
                deviceMenueItem.DropDownItems.Add(newDeviceItem);

                //For each Event
                ToolStripMenuItem eventMenuItem = new ToolStripMenuItem("Events");
                GSF.Data.Model.TableOperations<PQds.Model.Event> eventTable = new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection);
                List<Event> eventList = eventTable.QueryRecords("").ToList();
                foreach (Event evt in eventList)
                {
                    ToolStripMenuItem eventItem = new ToolStripMenuItem(String.Format("edit {0}", shorten(evt.Name, 15)));
                    eventItem.Click += new EventHandler((object sender, EventArgs e) =>
                    {
                        using (PQdsEvent subForm = new PQdsEvent(evt.ID))
                        {
                            subForm.FormClosed += delegate
                            {
                                this.UpdateMetaDataSection();
                                this.UpdateChannelList();
                            };
                            subForm.ShowDialog();
                        }
                    });

                    eventMenuItem.DropDownItems.Add(eventItem);
                }

                ToolStripMenuItem neweventItem = new ToolStripMenuItem("add new Event");
                neweventItem.Click += new EventHandler((object sender, EventArgs e) =>
                {
                    using (PQdsEvent subForm = new PQdsEvent(-1))
                    {
                        subForm.FormClosed += delegate
                        {
                            this.UpdateMetaDataSection();
                            this.UpdateChannelList();
                        };
                        subForm.ShowDialog();
                    }
                });
                eventMenuItem.DropDownItems.Add(neweventItem);

                this.GlobalMetaDataContextMenue.Items.Add(dataSensitivityItem);
                this.GlobalMetaDataContextMenue.Items.Add(assetMenueItem);
                this.GlobalMetaDataContextMenue.Items.Add(deviceMenueItem);
                this.GlobalMetaDataContextMenue.Items.Add(eventMenuItem);
            }
        }

        private void OpenDataSensitivity(object sender,EventArgs e)
        {
            using (PQdsDataSensitivity subForm = new PQdsDataSensitivity())
            {
                subForm.FormClosed += delegate
                {
                    this.UpdateMetaDataSection();
                };
                subForm.ShowDialog();
            }
            this.GlobalMetaDataContextMenue.Close();
        }

        private void RemoveAsset(int id)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTable = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);

                DialogResult msg =  MessageBox.Show(String.Format("This will delete {0}. Are you sure you want to Continue?",assetTable.QueryRecordWhere("ID={0}",id).AssetKey), "Delete Asset", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (msg == DialogResult.Yes)
                {
                    assetTable.DeleteRecordWhere("ID = {0}", id);
                    connection.ExecuteScalar("UPDATE Channel SET AssetID = NULL WHERE AssetID = {0}", id);

                    UpdateMetaDataSection();
                    UpdateChannelList();

                    // Still need to do something with the data sensitivity
                }
            }
            
        }

        private void RemoveDevice(int id)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Meter> deviceTable = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.Channel> channelTable = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.DataSeries> seriesTable = new GSF.Data.Model.TableOperations<PQds.Model.DataSeries>(connection);

                DialogResult msg = MessageBox.Show(String.Format("This will delete {0} and all Channels associated with it. Are you sure you want to Continue?", deviceTable.QueryRecordWhere("ID={0}", id).DeviceName), "Delete Device", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (msg == DialogResult.Yes)
                {
                    List<Channel> channels = channelTable.QueryRecordsWhere("MeterID = {0}", id).ToList();

                    foreach (Channel c in channels)
                    {
                        seriesTable.DeleteRecordWhere("ChannelID = {0}", c.ID);
                        channelTable.DeleteRecordWhere("ID = {0}", c.ID);
                    }

                    deviceTable.DeleteRecordWhere("ID = {0}", id);

                    UpdateMetaDataSection();
                    UpdateChannelList();
                }
            }

        }

        private void RemoveEvent(int id)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Event> eventTable = new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.DataSeries> seriesTable = new GSF.Data.Model.TableOperations<PQds.Model.DataSeries>(connection);

                DialogResult msg = MessageBox.Show(String.Format("This will delete {0} and all datasets associated with it. Are you sure you want to Continue?", eventTable.QueryRecordWhere("ID={0}", id).Name), "Delete Event", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (msg == DialogResult.Yes)
                {
                    seriesTable.DeleteRecordWhere("EventID = {0}", id);
                    eventTable.DeleteRecordWhere("ID = {0}", id);
                                      
                    UpdateMetaDataSection();
                    UpdateChannelList();
                }
            }

        }


        private bool isNodeAsset(TreeNode node)
        {
            string root;
            try
            {
                root = (string)node.Tag;
                if (root == null) { throw new System.ArgumentException("Parameter cannot be null", "original"); }
            }
            catch
            {
                TreeNode rootNode = node.Parent;
                TreeNode childNode = node;

                while (rootNode.Parent != null)
                {
                    childNode = rootNode;
                    rootNode = rootNode.Parent;
                }

                root = (string)rootNode.Tag;
            }

            if (root == "asset")
            {
                return true;
            }

            return false;
        }

        private bool isNodeDevice(TreeNode node)
        {
            string root;
            try
            {
                root = (string)node.Tag;
                if (root == null) { throw new System.ArgumentException("Parameter cannot be null", "original"); }
            }
            catch
            {
                TreeNode rootNode = node.Parent;
                TreeNode childNode = node;

                while (rootNode.Parent != null)
                {
                    childNode = rootNode;
                    rootNode = rootNode.Parent;
                }

                root = (string)rootNode.Tag;
            }

            if (root == "device")
            {
                return true;
            }

            return false;
        }

        private bool isNodeEvent(TreeNode node)
        {
            string root;
            try
            {
                root = (string)node.Tag;
                if (root == null) { throw new System.ArgumentException("Parameter cannot be null", "original"); }
            }
            catch
            {
                TreeNode rootNode = node.Parent;
                TreeNode childNode = node;

                while (rootNode.Parent != null)
                {
                    childNode = rootNode;
                    rootNode = rootNode.Parent;
                }

                root = (string)rootNode.Tag;
            }

            if (root == "event")
            {
                return true;
            }

            return false;
        }

        private int NodeToID(TreeNode node)
        {
            int result = -1;
            try
            {
                result = (int)node.Tag;
            }
            catch
            {
                if (node.Parent == null)
                    return -1;
                    
                TreeNode rootNode = node.Parent;
                TreeNode childNode = node;

                while (rootNode.Parent != null)
                {
                    childNode = rootNode;
                    rootNode = rootNode.Parent;
                }

                result = (int)childNode.Tag;
            }

            return result;
        }

        private void ChannelTree_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEventList();
        }

        private void AddToChart(PQds.Model.DataSeries dataseries, Chart chart)
        {
            Series chartSeries;
            DateTime startTime = dataseries.Series[0].Time;

            List<object> xValues = dataseries.Series
                .Select(pt => pt.Time - startTime)
                .Select(timeSpan => timeSpan.TotalSeconds)
                .Cast<object>()
                .ToList();

            List<object> yValues = dataseries.Series.Select(pt => pt.Value).Cast<object>().ToList();

            // Add a series to the chart for this channel
            string seriesName = "";
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Event> evtTbl = new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.Channel> channelTbl = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);

                seriesName = String.Format("{0} - {1}", evtTbl.QueryRecordWhere("ID = {0}", dataseries.EventID).Name,
                    channelTbl.QueryRecordWhere("ID = {0}", dataseries.ChannelID).Name);
            }

            try
            {
                chartSeries = chart.Series.Add(seriesName);
            }
            catch (System.ArgumentException ex)
            {
                int i = 1;

                while (true)
                {
                    try
                    {
                        chartSeries = chart.Series.Add(seriesName + " " + Convert.ToString(i));
                        break;
                    }

                    catch (System.ArgumentException exw)
                    {
                        i = i + 1;
                    }
                }
            }

            chartSeries.ChartType = SeriesChartType.Line;

            // Go through all the x and y values and
            // add them as points to the chart series
            for (int i = 0; i < xValues.Count && i < yValues.Count; i++)
                chartSeries.Points.AddXY(xValues[i], yValues[i]);

            RecalculateAxes(chart);
        }

        private void CSVExportButton_Click(object sender, EventArgs e)
        {
            PQdsPQDSexp subForm = new PQdsPQDSexp();
            subForm.ShowDialog();
        }


       
        private void EvtList_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.
            TreeNode node = (TreeNode)e.Item;
            if (node is null) { return; }

            if (e.Button == MouseButtons.Left)
            {
                this.m_ChartDragFlag = true;
                DoDragDrop(e.Item, DragDropEffects.Copy);
            }
        }

        private void Chart_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void Chart1_DragDrop(object sender, DragEventArgs e)
        {
            if (this.m_ChartDragFlag)
            {
                TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    GSF.Data.Model.TableOperations<PQds.Model.DataSeries> dataSeriesTable = new GSF.Data.Model.TableOperations<PQds.Model.DataSeries>(connection);

                    AddToChart(dataSeriesTable.QueryRecordWhere("ID = {0}", (int)draggedNode.Tag),this.DataChart1);
                }
            }
        }

        private void Chart2_DragDrop(object sender, DragEventArgs e)
        {
            if (this.m_ChartDragFlag)
            {
                TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    GSF.Data.Model.TableOperations<PQds.Model.DataSeries> dataSeriesTable = new GSF.Data.Model.TableOperations<PQds.Model.DataSeries>(connection);

                    AddToChart(dataSeriesTable.QueryRecordWhere("ID = {0}", (int)draggedNode.Tag), this.DataChart2);
                }
            }
        }

        private void ClearChart1(object sender, EventArgs e)
        {
            this.DataChart1.Series.Clear();
        }
        private void ClearChart2(object sender, EventArgs e)
        {
            this.DataChart2.Series.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PQdsAbout subForm = new PQdsAbout();
            subForm.ShowDialog();
        }


        private void EvtList_AfterSelect(object sender, TreeViewEventArgs e)
        {

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Channel> channelTbl = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);
                
                UpdateChannelMetaData(channelTbl.QueryRecordWhere("ID = {0}", (int)ChannelTree.SelectedNode.Tag));
            }
        }

        private void ChannelMetaDataClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            int evtID = -1;
            if (ChannelTree.SelectedNode is null) { return; }
            if (EvtList.SelectedNode is null) { return; }
            else
            {
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    GSF.Data.Model.TableOperations<PQds.Model.DataSeries> dataSeriesTable = new GSF.Data.Model.TableOperations<PQds.Model.DataSeries>(connection);
                    DataSeries dataSeries = dataSeriesTable.QueryRecordWhere("ID = {0}", (int)this.EvtList.SelectedNode.Tag);
                    evtID = dataSeries.EventID;
                }
            }

            int channelID = (int)ChannelTree.SelectedNode.Tag;

            using (PQdsChannel subForm = new PQdsChannel(evtID,channelID))
            {
                subForm.ShowDialog();
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    GSF.Data.Model.TableOperations<PQds.Model.Channel> channelTable = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);
                    UpdateChannelMetaData(channelTable.QueryRecordWhere("ID={0}",channelID));
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string directory;

            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {

                dialog.Description =  "Select the directory.";

                // Do not allow the user to create new files via the FolderBrowserDialog.
                dialog.ShowNewFolderButton = false;

                if (dialog.ShowDialog() == DialogResult.Cancel)
                    return;

                if (!Directory.Exists(dialog.SelectedPath))
                    return;

                directory = Path.GetDirectoryName(dialog.SelectedPath) ?? string.Empty;

                string[] pqdsFiles = Directory.GetFiles(dialog.SelectedPath, "*.csv",SearchOption.AllDirectories);

                string[] pqdiffFiles = Directory.GetFiles(dialog.SelectedPath, "*.pqd", SearchOption.AllDirectories);

                int nPQDS = pqdsFiles.Count();
                int nPQDiff = pqdiffFiles.Count();
                int nFail = 0;

                PQdsProgress progressWindow = new PQdsProgress(nPQDS*100+100*nPQDiff);
                progressWindow.updateText(String.Format("Loading file {0} out of {1}...", new object[2] { "{0}", nPQDiff + nPQDS }));
                progressWindow.Show();

                if (nPQDS > 0)
                {
                    FileParser.PQDSParser parser = new FileParser.PQDSParser("");
                    int nSuccess = parser.ParsePQDSFiles(pqdsFiles, new Progress<int>(progressWindow.updateProgress)).Result;
                    nFail = nPQDS - nSuccess;
                }

                if (nPQDiff > 0)
                {
                    FileParser.PQDIFFparser parser = new FileParser.PQDIFFparser();
                    int nSuccess = parser.ParsePQDIFFiles(pqdiffFiles, new Progress<int>(progressWindow.updateProgress)).Result;
                    nFail = nFail + nPQDiff - nSuccess;
                }

                progressWindow.Close();
                if ((nPQDS + nPQDiff) == 0)
                {
                    MessageBox.Show("No files were found.", "Warning", MessageBoxButtons.OK);
                    return;
                }
                if (nFail > 0)
                    MessageBox.Show(String.Format("Failed to load {0} files out of {1} PQDS files and {2} PQDIF files.",nFail,nPQDS,nPQDiff), "Warning", MessageBoxButtons.OK);
                else
                    MessageBox.Show(String.Format("Successfully loaded {0} PQDS files and {1} PQDIF Files.",nPQDS,nPQDiff), "Success", MessageBoxButtons.OK);

                if (nFail < (nPQDS + nPQDiff))
                {
                    CSVExportButton.Enabled = true;
                    this.m_hasData = true;
                }

                UpdateMetaDataSection();
                UpdateChannelList();

            }
        }

        
        #endregion [ Methods ]

        
    }
}
