//Copyright © 2019 Electric Power Research Institute, Inc. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 
//  Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  Neither the name of the EPRI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 

using System;
using System.Windows.Forms;

namespace PQds
{
    partial class PQdsMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PQdsMain));
            this.CSVExportButton = new System.Windows.Forms.Button();
            this.btn_FileOpen = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.ChannelMetaData = new System.Windows.Forms.TreeView();
            this.EvtList = new System.Windows.Forms.TreeView();
            this.ChannelTree = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.MetaDataTree = new System.Windows.Forms.TreeView();
            this.GlobalMetaDataContextMenue = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.DataChart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DataChart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataChart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataChart1)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // CSVExportButton
            // 
            this.CSVExportButton.Enabled = false;
            this.CSVExportButton.Location = new System.Drawing.Point(5, 62);
            this.CSVExportButton.Name = "CSVExportButton";
            this.CSVExportButton.Size = new System.Drawing.Size(120, 25);
            this.CSVExportButton.TabIndex = 4;
            this.CSVExportButton.Text = "Export to PQDS";
            this.CSVExportButton.UseVisualStyleBackColor = true;
            this.CSVExportButton.Click += new System.EventHandler(this.CSVExportButton_Click);
            // 
            // btn_FileOpen
            // 
            this.btn_FileOpen.Location = new System.Drawing.Point(5, 5);
            this.btn_FileOpen.Name = "btn_FileOpen";
            this.btn_FileOpen.Size = new System.Drawing.Size(120, 25);
            this.btn_FileOpen.TabIndex = 8;
            this.btn_FileOpen.Text = "Open File";
            this.btn_FileOpen.UseVisualStyleBackColor = true;
            this.btn_FileOpen.Click += new System.EventHandler(this.OPENFILEButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.ChannelMetaData);
            this.panel1.Controls.Add(this.EvtList);
            this.panel1.Controls.Add(this.ChannelTree);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.MetaDataTree);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(150, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 344);
            this.panel1.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(700, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Local Metadata";
            // 
            // ChannelMetaData
            // 
            this.ChannelMetaData.Location = new System.Drawing.Point(700, 170);
            this.ChannelMetaData.Name = "ChannelMetaData";
            this.ChannelMetaData.Size = new System.Drawing.Size(190, 163);
            this.ChannelMetaData.TabIndex = 8;
            this.ChannelMetaData.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ChannelMetaDataClick);
            // 
            // EvtList
            // 
            this.EvtList.HideSelection = false;
            this.EvtList.HotTracking = true;
            this.EvtList.Location = new System.Drawing.Point(700, 20);
            this.EvtList.Name = "EvtList";
            this.EvtList.Size = new System.Drawing.Size(190, 131);
            this.EvtList.TabIndex = 7;
            this.EvtList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.EvtList_ItemDrag);
            this.EvtList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.EvtList_AfterSelect);
            // 
            // ChannelTree
            // 
            this.ChannelTree.HideSelection = false;
            this.ChannelTree.Location = new System.Drawing.Point(403, 20);
            this.ChannelTree.Name = "ChannelTree";
            this.ChannelTree.Size = new System.Drawing.Size(295, 313);
            this.ChannelTree.TabIndex = 5;
            this.ChannelTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.ChannelTree_ItemDrag);
            this.ChannelTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ChannelTree_SelectedIndexChanged);
            this.ChannelTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.ChannelTree_DragDrop);
            this.ChannelTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.ChannelTree_DragEnter);
            this.ChannelTree.DragOver += new System.Windows.Forms.DragEventHandler(this.ChannelTree_DragOver);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(400, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Asset and Channels";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Global Metadata";
            // 
            // MetaDataTree
            // 
            this.MetaDataTree.ContextMenuStrip = this.GlobalMetaDataContextMenue;
            this.MetaDataTree.Location = new System.Drawing.Point(5, 20);
            this.MetaDataTree.Name = "MetaDataTree";
            this.MetaDataTree.Size = new System.Drawing.Size(390, 313);
            this.MetaDataTree.TabIndex = 2;
            // 
            // GlobalMetaDataContextMenue
            // 
            this.GlobalMetaDataContextMenue.Name = "contextMenuStrip3";
            this.GlobalMetaDataContextMenue.Size = new System.Drawing.Size(61, 4);
            this.GlobalMetaDataContextMenue.Opening += new System.ComponentModel.CancelEventHandler(this.PopulateGlobalMetaDataMenue);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(700, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Disturbance Events";
            // 
            // Panel2
            // 
            this.Panel2.Controls.Add(this.DataChart2);
            this.Panel2.Controls.Add(this.DataChart1);
            this.Panel2.Location = new System.Drawing.Point(150, 355);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(900, 200);
            this.Panel2.TabIndex = 2;
            // 
            // DataChart2
            // 
            this.DataChart2.AllowDrop = true;
            chartArea1.Name = "ChartArea1";
            this.DataChart2.ChartAreas.Add(chartArea1);
            this.DataChart2.ContextMenuStrip = this.contextMenuStrip2;
            legend1.Name = "Legend1";
            this.DataChart2.Legends.Add(legend1);
            this.DataChart2.Location = new System.Drawing.Point(5, 3);
            this.DataChart2.Name = "DataChart2";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.DataChart2.Series.Add(series1);
            this.DataChart2.Size = new System.Drawing.Size(440, 194);
            this.DataChart2.TabIndex = 1;
            this.DataChart2.Text = "chart1";
            this.DataChart2.DragDrop += new System.Windows.Forms.DragEventHandler(this.Chart2_DragDrop);
            this.DataChart2.DragOver += new System.Windows.Forms.DragEventHandler(this.Chart_DragOver);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
            // 
            // DataChart1
            // 
            this.DataChart1.AllowDrop = true;
            chartArea2.Name = "ChartArea1";
            this.DataChart1.ChartAreas.Add(chartArea2);
            this.DataChart1.ContextMenuStrip = this.contextMenuStrip1;
            legend2.Name = "Legend1";
            this.DataChart1.Legends.Add(legend2);
            this.DataChart1.Location = new System.Drawing.Point(450, 3);
            this.DataChart1.Name = "DataChart1";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.DataChart1.Series.Add(series2);
            this.DataChart1.Size = new System.Drawing.Size(440, 194);
            this.DataChart1.TabIndex = 0;
            this.DataChart1.Text = "chart1";
            this.DataChart1.DragDrop += new System.Windows.Forms.DragEventHandler(this.Chart1_DragDrop);
            this.DataChart1.DragOver += new System.Windows.Forms.DragEventHandler(this.Chart_DragOver);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button2);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.pictureBox1);
            this.panel3.Controls.Add(this.CSVExportButton);
            this.panel3.Controls.Add(this.btn_FileOpen);
            this.panel3.Location = new System.Drawing.Point(10, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(130, 550);
            this.panel3.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(5, 33);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 25);
            this.button2.TabIndex = 11;
            this.button2.Text = "Open Folder";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(5, 432);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 25);
            this.button1.TabIndex = 10;
            this.button1.Text = "About";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 463);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(122, 84);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // PQdsMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 561);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.Panel2);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1070, 600);
            this.Name = "PQdsMain";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "PQds";
            this.Load += new System.EventHandler(this.FileViewer_Load);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataChart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataChart1)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button CSVExportButton;
        private System.Windows.Forms.Button btn_FileOpen;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel Panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TreeView MetaDataTree;
        private System.Windows.Forms.DataVisualization.Charting.Chart DataChart1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TreeView ChannelTree;
        private TreeView EvtList;
        private Panel panel3;
        private PictureBox pictureBox1;
        private Label label4;
        private TreeView ChannelMetaData;
        private System.Windows.Forms.DataVisualization.Charting.Chart DataChart2;
        private ContextMenuStrip contextMenuStrip1;
        private ContextMenuStrip contextMenuStrip2;
        private Button button1;
        private Button button2;
        private ContextMenuStrip GlobalMetaDataContextMenue;
    }
}

