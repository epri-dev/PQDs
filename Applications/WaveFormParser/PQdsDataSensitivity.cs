//******************************************************************************************************
//  PQioDataSensitivity.cs - Gbtc
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

using GSF.Data;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PQds
{
    public partial class PQdsDataSensitivity : Form
    {
        #region [Properties]

        #endregion [Properties]

        #region [Methods]


        public PQdsDataSensitivity()
        {
            InitializeComponent();
        }

        private void Cncl_Btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MetaData_Load(object sender, EventArgs e)
        {
            DataSensitivityCombo.Items.AddRange(Model.DataSensitivityCode.DisplayOptions());

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity> dataSensitivityTable = new GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity>(connection);
                
                if (PQds.Model.DataSensitivity.CodeisGlobal())
                {
                    int? dataSensitivity = null;
                    dataSensitivity = dataSensitivityTable.QueryRecords().First().DataSensitivityCode;

                    if (dataSensitivity != null)
                    {
                        DataSensitivityCombo.SelectedIndex = Array.FindIndex(Model.DataSensitivityCode.DisplayOptions(),
                            item => item == Model.DataSensitivityCode.ToDisplay((int)dataSensitivity));
                    }
                    else
                    {
                        DataSensitivityCombo.SelectedIndex = Array.FindIndex(Model.DataSensitivityCode.DisplayOptions(),
                            item => item == Model.DataSensitivityCode.ToDisplay(-1));
                    }
                }
                else
                {
                    DataSensitivityCombo.Enabled = false;
                }

                if (PQds.Model.DataSensitivity.NoteisGlobal())
                {
                    string dataSensitivityNote = null;
                    dataSensitivityNote = dataSensitivityTable.QueryRecordsWhere("Note <> ''").First().Note;

                    DataSensitivityNoteText.Text = dataSensitivityNote;
                }
                else
                {
                    DataSensitivityNoteText.Enabled = false;
                }
            }
            this.MouseDown += new MouseEventHandler(OnClick);
        }

        private void WarnCodeOverride()
        {
            DialogResult result = MessageBox.Show("This will overwrite the Data Sensitivity Code on all Channels. Are you sure you want to continue?", "Data Sensitivity",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DataSensitivityCombo.Enabled = true;
                return;
            } 
        }

        private void WarnNoteOverride()
        {
            DialogResult result = MessageBox.Show("This will overwrite the Data Sensitivity Note on all Channels. Are you sure you want to continue?", "Data Sensitivity",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DataSensitivityNoteText.Enabled = true;
                return;
            }
        }

        private void OnClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !this.DataSensitivityCombo.Enabled)
            {
                if (e.X > DataSensitivityCombo.Location.X && e.X < DataSensitivityCombo.Location.X + DataSensitivityCombo.Width
                    && e.Y > DataSensitivityCombo.Location.Y && e.Y < DataSensitivityCombo.Location.Y + DataSensitivityCombo.Height)
                {
                    this.WarnCodeOverride();
                    return;
                }

            }
            else if (e.Button == MouseButtons.Left && !this.DataSensitivityNoteText.Enabled)
            {
                if (e.X > DataSensitivityNoteText.Location.X && e.X < DataSensitivityNoteText.Location.X + DataSensitivityNoteText.Width
                   && e.Y > DataSensitivityNoteText.Location.Y && e.Y < DataSensitivityNoteText.Location.Y + DataSensitivityNoteText.Height)
                {
                    this.WarnNoteOverride();
                    return;
                }
            }
        }
             
        private void Save_BTN_Click(object sender, EventArgs e)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity> dataSensitivityTable = new GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity>(connection);

                if (DataSensitivityCombo.Enabled)
                {
                    int? dataSensitivity = null;

                    if (PQds.Model.DataSensitivityCode.ToValue((string)DataSensitivityCombo.SelectedItem) != -1)
                    {
                        dataSensitivity = PQds.Model.DataSensitivityCode.ToValue((string)DataSensitivityCombo.SelectedItem);
                    }

                    connection.ExecuteScalar("UPDATE DataSensitivity SET DataSensitivityCode = {0}", dataSensitivity);
                }

                if (DataSensitivityNoteText.Enabled)
                    connection.ExecuteScalar("UPDATE DataSensitivity SET Note = {0}", new object[1] { DataSensitivityNoteText.Text.Trim() });
                
                this.Close();
            }
        }

        #endregion[Methods]

    }

}
