//Copyright © 2019 Electric Power Research Institute, Inc. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 
//  Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  Neither the name of the EPRI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GSF.Data;
using PQds.Model;

namespace FileParser
{
    /// <summary>
    /// Class that parses a PQDS File for the PQDS software tool.
    /// This won't work for the openXDA
    /// </summary>
    public class PQDSParser
    {

        #region[properties]
        private string m_logilename;
        private int m_prevProgress;
        private IProgress<int> mProgress;
        #endregion[properties]

        #region[Constructor]

        /// <summary>
        /// creates a new Instance of a <see cref="PQDSParser"/>.
        /// </summary>
        /// <param name="logfile"> Name of the PQDS log file</param>
        public PQDSParser(string logfile)
        {
            this.m_logilename = logfile;
            this.m_prevProgress = 0;
        }

        /// <summary>
        /// creates a new Instance of a <see cref="PQDSParser"/> without logging.
        /// </summary>
        public PQDSParser()
        {
            this.m_logilename = null;
            this.m_prevProgress = 0;
        }

        #endregion[Constructor]

        #region[methods]

        /// <summary>
        /// Parses a single PQDS File.
        /// </summary>
        /// <param name="filename"> Name of the PQDS File</param>
        /// <param name="progress"> <see cref="IProgress{T}"/> provides interface to Progress</param>
        /// <returns>task that return true when successfull or false if failed </returns>
        public Task<bool> ParsePQDSFile(string filename, IProgress<int> progress)
        {
            this.mProgress = progress;
            Task<bool> result = Task.Run<bool>(() =>
            {
                try
                {
                    ParseFromPQDS(filename);
                    return true;
                }
                catch
                {
                    return false;
                }
            });

            return result;
               
        }

        /// <summary>
        /// Parses multiple PQDS File.
        /// </summary>
        /// <param name="filenames"> Name of the PQDS Files</param>
        /// <param name="progress"> <see cref="IProgress{T}"/> provides interface to Progress</param>
        /// <returns>task that returns number of successfully read files </returns>
        public Task<int> ParsePQDSFiles(string[] filenames, IProgress<int> progress)
        {
            
            this.mProgress = progress;

            Task<int> result = Task<int>.Run(() =>
            {
                int nSuccess = 0;
                for (int i = 0; i < filenames.Count(); i++)
                {
                    try
                    {
                        ParseFromPQDS(filenames[i]);
                        nSuccess++;
                    }
                    catch
                    { }
                }
                return nSuccess;

            });

            return result;
        }

        /// <summary>
        /// Create a single PQDS File.
        /// </summary>
        /// <param name="asset"> Asset used to get Asset Meta data. </param>
        /// <param name="evt"> Event used to get Event Meta data. </param>
        /// <param name="FileName"> File name of the PQDS file. </param>
        /// <param name="includeAssetMetaData"> Flag determines whether asset metadata is included. </param>
        /// <param name="includeCustomMetaData"> Flag that determines if custom meta data is included. </param>
        /// <param name="includeDeviceMetaData"> Flag that determines if device meta data is included. </param>
        /// <param name="includeTimingMetaData"> Flag that determines if timing meta data is included. </param>
        /// <param name="includeEventMetaData"> Flag that determines if event meta data is included. </param>
        /// <param name="includeWaveFormMetaData"> Flag that determines if waveform meta data is included. </param>
        /// <param name="includeWaveFormMetaData"> Flag that determines if waveform meta data is included. </param>
        /// <param name="startTime"> Overrides start time from event if selected. </param>
        /// <param name="includeAuthorMetaData"> Flag that determines if Author meta data is included. </param>
        /// <returns>task where .Result() return true when successfull or false if failed </returns>
        public Task<bool> WritePQDSFile(IProgress<int> progress, PQds.Model.Asset asset, PQds.Model.Event evt, string FileName, Boolean includeDeviceMetaData = false, Boolean includeAssetMetaData = false, Boolean includeTimingMetaData = false,
            Boolean includeEventMetaData = false, Boolean includeWaveFormMetaData = false, Boolean includeCustomMetaData = false, Boolean includeAuthorMetaData = false, Boolean includeGUID = false, DateTime? startTime = null)
        {

            this.mProgress = progress;
            Task<bool> result = Task<bool>.Run(() =>
            {
                try
                {
                    ParseToPQDS(asset, evt, includeDeviceMetaData, includeAssetMetaData, includeTimingMetaData, includeEventMetaData, includeWaveFormMetaData, includeCustomMetaData, includeAuthorMetaData, includeGUID,  startTime, FileName);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            });

            return result;
        }

        /// <summary>
        /// Create multiple PQDS File.
        /// </summary>
        /// <param name="asset"> Assets used to get Asset Meta data. </param>
        /// <param name="evt"> Events used to get Event Meta data. </param>
        /// <param name="FileName"> File names of the PQDS files. </param>
        /// <param name="includeAssetMetaData"> Flag determines whether asset metadata is included. </param>
        /// <param name="includeCustomMetaData"> Flag that determines if custom meta data is included. </param>
        /// <param name="includeDeviceMetaData"> Flag that determines if device meta data is included. </param>
        /// <param name="includeTimingMetaData"> Flag that determines if timing meta data is included. </param>
        /// <param name="includeEventMetaData"> Flag that determines if event meta data is included. </param>
        /// <param name="includeWaveFormMetaData"> Flag that determines if waveform meta data is included. </param>
        /// <param name="includeAuthorMetaData"> Flag that determines if Author meta data is included. </param>
        /// <param name="startTime"> Overrides start time from event if selected. </param>
        /// <param name="progress"> <see cref="IProgress{T}"/> provides interface to Progress</param>
        /// <returns>task where .Result() return true when successfull or false if failed </returns>
        public Task<int> WritePQDSFiles(IProgress<int> progress, List<PQds.Model.Asset> asset, List<PQds.Model.Event> evt, List<string> FileName, Boolean includeDeviceMetaData = false, Boolean includeAssetMetaData = false, Boolean includeTimingMetaData = false,
            Boolean includeEventMetaData = false, Boolean includeWaveFormMetaData = false, Boolean includeCustomMetaData = false, Boolean includeAuthorMetaData = false, Boolean includeGUID = false, DateTime? startTime = null)
        {

            this.mProgress = progress;
            Task<int> result = Task<int>.Run(() =>
            {
                int nSuccess = 0;

                for (int i=0; i < asset.Count(); i++)
                try
                {
                    ParseToPQDS(asset[i], evt[i], includeDeviceMetaData, includeAssetMetaData, includeTimingMetaData, includeEventMetaData, includeWaveFormMetaData, includeCustomMetaData, includeAuthorMetaData, includeGUID,  startTime, FileName[i]);
                    nSuccess++;
                }
                catch (Exception ex)
                { }

                return nSuccess;
            });

            return result;
        }




        private void UpdateIOProgress(double i)
        {
            // 50% is reading File
            this.mProgress.Report(this.m_prevProgress + (int)(i * 50.0D));
            //other 50% is spend in here based on testing
        }

        /// <summary>
        /// Parses the PQDS file into the Temporary SQLLite DataBase.
        /// </summary>
        /// <param name="filename"> Name of the PQDS File</param>
        private void ParseFromPQDS(string filename )
        {
            PQDSFile file = new PQDSFile();
            file.ReadFromFile(filename, new Progress<double>(UpdateIOProgress));
            this.m_prevProgress = this.m_prevProgress + 50;

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {


                GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTbl = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.Meter> deviceTbl = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.Event> evtTbl = new GSF.Data.Model.TableOperations<PQds.Model.Event>(connection);

                GSF.Data.Model.TableOperations<PQds.Model.Channel> channelTbl = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);
                GSF.Data.Model.TableOperations<PQds.Model.DataSeries> dataSeriesTbl = new GSF.Data.Model.TableOperations<PQds.Model.DataSeries>(connection);

                GSF.Data.Model.TableOperations<PQds.Model.CustomField> customFldTbl = new GSF.Data.Model.TableOperations<PQds.Model.CustomField>(connection);

                //Create Asset from Meta Data
                PQds.Model.Asset asset = MetadataToAsset(file.MetaData);

                // if Asset already exists rename it
                if (assetTbl.QueryRecordCountWhere("AssetKey = {0}", asset.AssetKey) > 0)
                {
                    PQds.Model.Asset original = assetTbl.QueryRecordWhere("AssetKey = {0}", asset.AssetKey);

                    //check to make sure they are the same
                    bool nominalV = (original.NominalVoltage == asset.NominalVoltage)||(original.NominalVoltage == null) || (asset.NominalVoltage == null);
                    bool nominalf = (original.NominalFrequency == asset.NominalFrequency) || (original.NominalFrequency == null) || (asset.NominalFrequency == null);
                    bool upstreamXFR = (original.UpstreamXFMR == asset.UpstreamXFMR) || (original.UpstreamXFMR == null) || (asset.UpstreamXFMR == null);
                    bool length = (original.Length == asset.Length) || (original.Length == null) || (asset.Length == null);

                    if (nominalV && nominalf && upstreamXFR && length)
                    {
                        asset = MergeAssets(original, asset);
                        assetTbl.UpdateRecord(asset);
                    }
                    else
                    {
                        int i = 1;
                        string assetkey = asset.AssetKey;

                        while(assetTbl.QueryRecordCountWhere("AssetKey = {0}", assetkey) > 0)
                        {
                            assetkey = String.Format("{0} {1}", asset.AssetKey, i);
                            i++;
                        }
                        asset.AssetKey = assetkey;
                        assetTbl.AddNewRecord(asset);
                        asset.ID = PQds.Model.ModelID.GetID<Asset>(connection);

                    }

                   
                }
                else
                {
                    assetTbl.AddNewRecord(asset);
                    asset.ID = PQds.Model.ModelID.GetID<Asset>(connection);
                }
                   

                //create Device from Meta Data
                PQds.Model.Meter device = MetadataToDevice(file.MetaData);
                if (CheckMeterDuplicates(device) != null)
                {
                    device = MergeDevices(CheckMeterDuplicates(device),device);
                    deviceTbl.UpdateRecord(device);
                }
                else
                {
                    deviceTbl.AddNewRecord(device);
                    device.ID = PQds.Model.ModelID.GetID<Meter>(connection);
                }

                //create Event from Meta Data
                PQds.Model.Event evt = MetadataToEvent(file.MetaData);
                evtTbl.AddNewRecord(evt);
                evt.ID = PQds.Model.ModelID.GetID<Event>(connection);

                //assume it is point on wave
                List<string> Tags = file.MetaData.Select(item => item.key.ToLower()).ToList();

                int signalType;
                if (Tags.Contains("waveformdatatype"))
                { signalType = ((MetaDataTag<int>)file.MetaData.Find(item => item.key.ToLower() == "waveformdatatype")).Value; }
                else
                { signalType = SignalType.PointOnWave; }

                int? datasensitivityid;
                string datasensitivityNote;
                if (Tags.Contains("waveformsensitivitycode"))
                { datasensitivityid = ((MetaDataTag<int>)file.MetaData.Find(item => item.key.ToLower() == "waveformsensitivitycode")).Value; }
                else
                { datasensitivityid = null; }

                if (Tags.Contains("waveformsensitivitynote"))
                { datasensitivityNote = ((MetaDataTag<string>)file.MetaData.Find(item => item.key.ToLower() == "waveformsensitivitynote")).Value; }
                else
                { datasensitivityNote = ""; }

                Dictionary<String, PQds.Model.DataSeries> data = file.Data;

                //Based on estimation we are at 75% of the work done
                this.m_prevProgress = this.m_prevProgress + 25;
                this.mProgress.Report(this.m_prevProgress);

                String[] possibleChannels = new string[] { "va", "vb", "vc", "ia", "ib", "ic", "f" };

                foreach(string key in data.Keys)
                {
                    if (possibleChannels.Contains(key.ToLower()))
                    {
                        PQds.Model.Channel channel = new Channel();
                        channel.MeasurementType = key.ToLower();
                        channel.MeterID = device.ID;
                        channel.Name = MeasurementType.ToDisplay(channel.MeasurementType);
                        channel.SignalType = signalType;
                        channel.AssetID = asset.ID;

                        //Merge Channels if channel already exists
                        int nDuplicates = channelTbl.QueryRecordCountWhere("MeasurementType = {0} AND AssetID = {1} AND SignalType = {2} AND MeterID = {3}", key.ToLower(), asset.ID, signalType, device.ID);
                        if (nDuplicates > 0 )
                        {
                            PQds.Model.Channel original = channelTbl.QueryRecordWhere("MeasurementType = {0} AND AssetID = {1} AND SignalType = {2} AND MeterID = {3}", key.ToLower(), asset.ID, signalType, device.ID);
                            channel = MergeChannel(original, channel);
                            channelTbl.UpdateRecord(channel);
                        }
                        else
                        {
                            channelTbl.AddNewRecord(channel);
                            channel.ID = PQds.Model.ModelID.GetID<Channel>(connection);
                        }

                        data[key].ChannelID = channel.ID;
                        data[key].EventID = evt.ID;

                        dataSeriesTbl.AddNewRecord(data[key]);
                    }
                }

                //Add DataSensitivity
                if (datasensitivityid != null)
                {
                    PQds.Model.DataSensitivity dataSensitivity = new DataSensitivity() { Event = evt.ID, Asset = asset.ID, DataSensitivityCode = datasensitivityid, Note = datasensitivityNote };
                    (new GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity>(connection)).AddNewRecord(dataSensitivity);
                }

                List<MetaDataTag> customTags = file.MetaData.FindAll(item => item.key.Contains(".")).ToList();

                foreach (MetaDataTag custom in customTags)
                {
                    CustomField fld = new CustomField();

                    switch(custom.Type())
                    {
                        case PQDSDataType.Numeric:
                            fld.Type = "N";
                            fld.Value = Convert.ToString(((MetaDataTag<double>)custom).Value);
                            break;
                        case PQDSDataType.Binary:
                            fld.Type = "B";
                            fld.Value = Convert.ToString(((MetaDataTag<Boolean>)custom).Value);
                            break;
                        case PQDSDataType.Enumeration:
                            fld.Type = "E";
                            fld.Value = Convert.ToString(((MetaDataTag<int>)custom).Value);
                            break;
                       default:
                            fld.Type = "T";
                            fld.Value = ((MetaDataTag<string>)custom).Value;
                            break;
                    }

                    int index = custom.key.IndexOf('.');
                    fld.domain = custom.key.Substring(0, index);
                    fld.key = custom.key.Substring(index + 1);

                    fld.AssetID = asset.ID;
                    fld.EventID = evt.ID;
                    customFldTbl.AddNewRecord(fld);
                }

                this.m_prevProgress = this.m_prevProgress + 25;
                this.mProgress.Report(this.m_prevProgress);
            }
        }

        /// <summary>
        /// Parses the data from the temporary SQLLite DataBase into a PQDS File.
        /// </summary>
        /// <param name="asset"> Asset used to get Asset Meta data. </param>
        /// <param name="evt"> Event used to get Event Meta data. </param>
        /// <param name="includeAssetMetaData"> Flag determines whether asset metadata is included. </param>
        /// <param name="includeCustomMetaData"> Flag that determines if custom meta data is included. </param>
        /// <param name="includeDeviceMetaData"> Flag that determines if device meta data is included. </param>
        /// <param name="includeTimingMetaData"> Flag that determines if timing meta data is included. </param>
        /// <param name="includeEventMetaData"> Flag that determines if event meta data is included. </param>
        /// <param name="includeWaveFormMetaData"> Flag that determines if waveform meta data is included. </param>
        /// <param name="startTime"> Overrides start time from event if selected. </param>
        /// <param name="filename"> File name of the PQDS file. </param>
        private void ParseToPQDS(PQds.Model.Asset asset, PQds.Model.Event evt, Boolean includeDeviceMetaData, Boolean includeAssetMetaData, Boolean includeTimingMetaData, 
            Boolean includeEventMetaData, Boolean includeWaveFormMetaData, Boolean includeCustomMetaData, Boolean includeAuthor, Boolean includeGUID, DateTime? startTime, string filename)
        {

            List<MetaDataTag> metaData = new List<MetaDataTag>();
            List<MetaDataTag> fullMetaData = new List<MetaDataTag>();

            PQds.Model.Meter device;
            List<PQds.Model.Channel> channels;
            List<PQds.Model.DataSeries> data;
            data = new List<DataSeries>();
            List<PQds.Model.CustomField> customMetaData = new List<CustomField>();
            int? lowestDataSensitivity;
            string dataSensitivityNote = null;

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {


                GSF.Data.Model.TableOperations<PQds.Model.Channel> channelTbl = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);
                channels = channelTbl.QueryRecordsWhere("(SELECT COUNT(DataSeries.ID) FROM DataSeries WHERE EventID = {0} AND ChannelID = Channel.ID) > 0  AND (AssetID = {1})", evt.ID, asset.ID).ToList();

                if (channels.Count() == 0)
                { throw new Exception("No channels Found"); }

                GSF.Data.Model.TableOperations<PQds.Model.Meter> deviceTbl = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);
                device = deviceTbl.QueryRecordWhere("ID = {0}", channels[0].MeterID);

                if (device is null)
                { throw new Exception("No device Found"); ; }

                GSF.Data.Model.TableOperations<PQds.Model.CustomField> customFieldTbl = new GSF.Data.Model.TableOperations<PQds.Model.CustomField>(connection);
                customMetaData = customFieldTbl.QueryRecordsWhere("AssetID = {0} AND EventID = {1}", asset.ID, evt.ID).ToList();

                GSF.Data.Model.TableOperations<PQds.Model.Setting> settingTbl = new GSF.Data.Model.TableOperations<PQds.Model.Setting>(connection);

                if (includeAuthor)
                {
                    metaData.Add(new MetaDataTag<string>("Utility", settingTbl.QueryRecordWhere("Name = {0}", "contact.utility").value));
                    metaData.Add(new MetaDataTag<string>("ContactEmail", settingTbl.QueryRecordWhere("Name = {0}", "contact.email").value));
                }

                fullMetaData.Add(new MetaDataTag<string>("Utility", settingTbl.QueryRecordWhere("Name = {0}", "contact.utility").value));
                fullMetaData.Add(new MetaDataTag<string>("ContactEmail", settingTbl.QueryRecordWhere("Name = {0}", "contact.email").value));

                GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity> dataSensitivityTbl = new GSF.Data.Model.TableOperations<PQds.Model.DataSensitivity>(connection);

                lowestDataSensitivity = dataSensitivityTbl.QueryRecordsWhere("Asset = {0} AND Event = {1}",new object[2] { asset.ID, evt.ID }).Select(item => item.DataSensitivityCode).Min();

                List<string> notes = dataSensitivityTbl.QueryRecordsWhere("Asset = {0} AND Event = {1} AND Note NOT NULL AND Note <> ''", new object[2] { asset.ID, evt.ID }).Select(item => item.Note.ToLower()).ToList();
                if (notes.Distinct().Count() == 1)
                {
                    dataSensitivityNote = notes[0];
                }
                else if (notes.Count() > 1)
                {
                    //Attempt to string all Data Sensitivity notes together
                    dataSensitivityNote = String.Join(";", notes);
                    if (dataSensitivityNote.Length > 250)
                    {
                        dataSensitivityNote = dataSensitivityNote.Substring(0, 247) + "...";
                    }
                }

            }

            data = channels.Select(item => GetData(item, evt)).ToList();

            // Figure out inital start Time
            DateTime initalTimeStamp;

            if (evt.EventTime is null)
            {
                initalTimeStamp = data.Select(item => item.Series[0].Time).Min();
            }
            else
            {
                initalTimeStamp = (DateTime)evt.EventTime;
            }

            if (!(startTime is null))
            {
                evt.EventTime = startTime;
            }

            //Include all MetaData in Log File
            fullMetaData.AddRange(DeviceMetaData(device));
            fullMetaData.AddRange(AssetMetaData(asset));
            fullMetaData.AddRange(EventMetaData(evt));
            fullMetaData.AddRange(TimingMetaData(evt));


           
            if (lowestDataSensitivity != null)
            {
                fullMetaData.Add(new MetaDataTag<int>("WaveFormSensitivityCode", (int)lowestDataSensitivity));
            }
            if (dataSensitivityNote != null)
            {
                fullMetaData.Add(new MetaDataTag<string>("WaveFormSensitivityNote", dataSensitivityNote));
            }

            fullMetaData.AddRange(customMetaData.Select(item => ChannelMetaData(item)).ToList());

            //Based on estimation we are at 25% of the work done
            this.m_prevProgress = this.m_prevProgress + 25;
            this.mProgress.Report(this.m_prevProgress);



            //Deal with MetaData
            if (includeDeviceMetaData)
            {
                //Try to get Meta Data From Meter
                metaData.AddRange(DeviceMetaData(device));
            }
            if (includeAssetMetaData)
            {
                // Get MetaData from Asset
                metaData.AddRange(AssetMetaData(asset));
            }
            if (includeEventMetaData)
            {
                // Get MetaData from event
                metaData.AddRange(EventMetaData(evt));
            }
            if (includeTimingMetaData)
            {
                // Get MetaData from Timing
                metaData.AddRange(TimingMetaData(evt));
            }
            if (includeWaveFormMetaData)
            {
                if (lowestDataSensitivity != null)
                {
                    metaData.Add(new MetaDataTag<int>("WaveFormSensitivityCode", (int)lowestDataSensitivity));
                }

                if ((dataSensitivityNote != null) && (dataSensitivityNote != ""))
                {
                    metaData.Add(new MetaDataTag<string>("WaveFormSensitivityNote", dataSensitivityNote));
                }
            }
            if (includeCustomMetaData)
            {
                metaData.AddRange(customMetaData.Select(item => ChannelMetaData(item)).ToList());
            }
            if (!includeEventMetaData && includeGUID)
            {
                if (evt.GUID != null)
                {
                    metaData.Add(new MetaDataTag<string>("EventGUID", evt.GUID));
                }
            }

            // Get Meta Data from channel

            //Based on estimation we are at 50% of the work done
            this.m_prevProgress = this.m_prevProgress + 25;
            this.mProgress.Report(this.m_prevProgress);


            //Create PQDSModel
            PQDSFile file = new PQDSFile(metaData, PQDSData(data), initalTimeStamp);
            PQDSLogFile logfile = new PQDSLogFile(fullMetaData, evt.GUID, PQDSData(data).Keys.ToList());

            //Write PQDSModel to File
            file.WriteToFile(filename, new Progress<double>(UpdateIOProgress));
            this.m_prevProgress = this.m_prevProgress + 50;

            logfile.Write(this.m_logilename);
        }

        #region[MetaData]
        private List<MetaDataTag> DeviceMetaData(PQds.Model.Meter device)
        {
            List<MetaDataTag> results = new List<MetaDataTag>();

            if (device.DeviceName != null)
            {
                results.Add(new MetaDataTag<string>("DeviceName", device.DeviceName));
            }
            if (device.DeviceAlias != null)
            {
                results.Add(new MetaDataTag<string>("DeviceAlias", device.DeviceAlias));
            }
            if (device.DeviceLocation != null)
            {
                results.Add(new MetaDataTag<string>("DeviceLocation", device.DeviceLocation));
            }
            if (device.DeviceLocationAlias != null)
            {
                results.Add(new MetaDataTag<string>("DeviceLocationAlias", device.DeviceLocationAlias));
            }
            if (device.Latitude != null)
            {
                results.Add(new MetaDataTag<string>("Latitude", Convert.ToString(device.Latitude)));
            }
            if (device.Longitude != null)
            {
                results.Add(new MetaDataTag<string>("Longitude", Convert.ToString(device.Longitude)));
            }
            if (device.AccountName != null)
            {
                results.Add(new MetaDataTag<string>("AccountName", device.AccountName));
            }
            if (device.AccountAlias != null)
            {
                results.Add(new MetaDataTag<string>("AccountNameAlias", device.AccountAlias));
            }

            if (device.DistanceToXFR != null)
            {
                results.Add(new MetaDataTag<double>("DeviceDistanceToXFMR", (double)device.DistanceToXFR));
            }
            if (device.ConnectionType != null)
            {
                    results.Add(new MetaDataTag<int>("DeviceConnectionTypeCode", (int)device.ConnectionType));
            }
        
            if (device.Owner != null)
            {
                results.Add(new MetaDataTag<string>("DeviceOwner", device.Owner));
            }


            return results;
        }

        private List<MetaDataTag> AssetMetaData(PQds.Model.Asset asset)
        {
            List<MetaDataTag> results = new List<MetaDataTag>();

            if (asset.NominalVoltage != null)
            {
                results.Add(new MetaDataTag<double>("NominalVoltage-LG", (double)asset.NominalVoltage));
            }
            if (asset.NominalFrequency != null)
            {
                results.Add(new MetaDataTag<double>("NominalFrequency", (double)asset.NominalFrequency));
            }
            if (asset.UpstreamXFMR != null)
            {
                results.Add(new MetaDataTag<double>("UpstreamXFMR-kVA", (double)asset.UpstreamXFMR));
            }
            if (asset.Length != null)
            {
                results.Add(new MetaDataTag<double>("LineLength", (double)asset.Length));
            }
            if (asset.AssetKey != null)
            {
                results.Add(new MetaDataTag<string>("AssetName", asset.AssetKey));
            }

            return results;
        }

        private List<MetaDataTag> EventMetaData(PQds.Model.Event evt)
        {
            List<MetaDataTag> results = new List<MetaDataTag>();

            if (evt.GUID != null)
            {
                results.Add(new MetaDataTag<string>("EventGUID", evt.GUID));
            }
            if (evt.Name != null)
            {
                results.Add(new MetaDataTag<string>("EventID", evt.Name));
            }

            if (evt.EventType != null)
            {
                results.Add(new MetaDataTag<int>("EventTypeCode", (int)evt.EventType));
            }
            if (evt.FaultType != null)
            {
                results.Add(new MetaDataTag<int>("EventFaultTypeCode", (int)evt.FaultType));
            }

            if (evt.PeakCurrent != null)
            {
                results.Add(new MetaDataTag<double>("EventPeakCurrent", (double)evt.PeakCurrent));
            }
            if (evt.PeakVoltage != null)
            {
                results.Add(new MetaDataTag<double>("EventPeakVoltage", (double)evt.PeakVoltage));
            }

            if (evt.MaxVA != null)
            {
                results.Add(new MetaDataTag<double>("EventMaxVA", (double)evt.MaxVA));
            }
            if (evt.MaxVB != null)
            {
                results.Add(new MetaDataTag<double>("EventMaxVB", (double)evt.MaxVB));
            }
            if (evt.MaxVC != null)
            {
                results.Add(new MetaDataTag<double>("EventMaxVC", (double)evt.MaxVC));
            }

            if (evt.MinVA != null)
            {
                results.Add(new MetaDataTag<double>("EventMinVA", (double)evt.MinVA));
            }
            if (evt.MinVB != null)
            {
                results.Add(new MetaDataTag<double>("EventMinVB", (double)evt.MinVB));
            }
            if (evt.MinVC != null)
            {
                results.Add(new MetaDataTag<double>("EventMinVC", (double)evt.MinVC));
            }

            if (evt.MaxIA != null)
            {
                results.Add(new MetaDataTag<double>("EventMaxIA", (double)evt.MaxIA));
            }
            if (evt.MaxIB != null)
            {
                results.Add(new MetaDataTag<double>("EventMaxIB", (double)evt.MaxIB));
            }
            if (evt.MaxIC != null)
            {
                results.Add(new MetaDataTag<double>("EventMaxIC", (double)evt.MaxIC));
            }

            if (evt.PreEventCurrent != null)
            {
                results.Add(new MetaDataTag<double>("EventPreEventCurrent", (double)evt.PreEventCurrent));
            }
            if (evt.PreEventVoltage != null)
            {
                results.Add(new MetaDataTag<double>("EventPreEventVoltage", (double)evt.PreEventVoltage));
            }
            if (evt.Duration != null)
            {
                results.Add(new MetaDataTag<double>("EventDuration", (double)evt.Duration));
            }

            if (evt.FaultI2T != null)
            {
                results.Add(new MetaDataTag<double>("EventFaultI2T", (double)evt.FaultI2T));
            }
            if (evt.DistanceToFault != null)
            {
                results.Add(new MetaDataTag<double>("DistanceToFault", (double)evt.DistanceToFault));
            }

            if (evt.FaultCause != null)
            {
                results.Add(new MetaDataTag<int>("EventCauseCode", (int)evt.FaultCause));
            }

            return results;
        }

        private List<MetaDataTag> TimingMetaData(PQds.Model.Event evt)
        {
            List<MetaDataTag> results = new List<MetaDataTag>();

            if (!(evt.EventTime is null))
            {
                results.Add(new MetaDataTag<int>("EventYear", ((DateTime)evt.EventTime).Year));

                results.Add(new MetaDataTag<int>("EventMonth", ((DateTime)evt.EventTime).Month));
                results.Add(new MetaDataTag<int>("EventDay", ((DateTime)evt.EventTime).Day));
                results.Add(new MetaDataTag<int>("EventHour", ((DateTime)evt.EventTime).Hour));
                results.Add(new MetaDataTag<int>("EventMinute", ((DateTime)evt.EventTime).Minute));
                results.Add(new MetaDataTag<int>("EventSecond", ((DateTime)evt.EventTime).Second));
                results.Add(new MetaDataTag<int>("EventNanoSecond", Get_nanoseconds((DateTime)evt.EventTime)));

                String date = String.Format("{0:D2}/{1:D2}/{2:D4}", ((DateTime)evt.EventTime).Month, ((DateTime)evt.EventTime).Day, ((DateTime)evt.EventTime).Year);
                String time = String.Format("{0:D2}:{1:D2}:{2:D2}", ((DateTime)evt.EventTime).Hour, ((DateTime)evt.EventTime).Minute, ((DateTime)evt.EventTime).Second);
                results.Add(new MetaDataTag<string>("EventDate", date));
                results.Add(new MetaDataTag<string>("EventTime", time));
            }

            return results;
        }

        private MetaDataTag ChannelMetaData(PQds.Model.CustomField tag)
        {
            string key = tag.domain + "." +  tag.key;

            switch(tag.Type)
            {
                case ("T"):
                    return new MetaDataTag<string>(key, tag.Value);
                case ("N"):
                    return new MetaDataTag<double>(key, Convert.ToDouble(tag.Value));
                default:
                    return new MetaDataTag<string>(key, tag.Value);
            }
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

        private DataSeries GetData(PQds.Model.Channel channel, PQds.Model.Event evt)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.DataSeries> dataSeriesTbl = new GSF.Data.Model.TableOperations<PQds.Model.DataSeries>(connection);
                return dataSeriesTbl.QueryRecordWhere("ChannelID = {0} AND EventID = {1}", channel.ID, evt.ID);
            }
        }

        private Dictionary<string, PQds.Model.DataSeries> PQDSData(List<DataSeries> dataSeries)
        {
            Dictionary<string, PQds.Model.DataSeries> result = new Dictionary<string, DataSeries>();

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Channel> channelTbl = new GSF.Data.Model.TableOperations<PQds.Model.Channel>(connection);

                foreach (PQds.Model.DataSeries ds in dataSeries)
                {
                    PQds.Model.Channel channel = channelTbl.QueryRecordWhere("ID = {0}", ds.ChannelID);
                    ;

                    if (result.Keys.Contains(channel.MeasurementType.ToLower()))
                    {
                        // This is an issue since 2 measurements of the same type should not exist
                        continue;
                    }
                    else
                    {

                        result.Add(channel.MeasurementType.ToLower(), ds);
                    }
                }
            }

            return result;
        }

        private PQds.Model.Meter CheckMeterDuplicates(PQds.Model.Meter device)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<PQds.Model.Meter> deviceTbl = new GSF.Data.Model.TableOperations<PQds.Model.Meter>(connection);

                foreach (PQds.Model.Meter duplicate in deviceTbl.QueryRecordsWhere("Devicename = {0}", device.DeviceName))
                {
                    bool devicealias = (device.DeviceAlias == duplicate.DeviceAlias) || (device.DeviceAlias == null) || (duplicate.DeviceAlias == null);
                    bool devicelocation = (device.DeviceLocation == duplicate.DeviceLocation) || (device.DeviceLocation == null) || (duplicate.DeviceLocation == null);
                    bool devicelocationalias = (device.DeviceLocationAlias == duplicate.DeviceLocationAlias) || (device.DeviceLocationAlias == null) || (duplicate.DeviceLocationAlias == null);
                    bool latitude = (device.Latitude == duplicate.Latitude) || (device.Latitude == null) || (duplicate.Latitude == null);
                    bool longitude = (device.Longitude == duplicate.Longitude) || (device.Longitude == null) || (duplicate.Longitude == null);
                    bool accountname = (device.AccountName == duplicate.AccountName) || (device.AccountName == null) || (duplicate.AccountName == null);
                    bool accountnamealias = (device.AccountAlias == duplicate.AccountAlias) || (device.AccountAlias == null) || (duplicate.AccountAlias == null);
                    bool devicedistancetoxfmr = (device.DistanceToXFR == duplicate.DistanceToXFR) || (device.DistanceToXFR == null) || (duplicate.DistanceToXFR == null);
                    bool deviceowner = (device.Owner == duplicate.Owner) || (device.Owner == null) || (duplicate.Owner == null);

                    if (devicealias && devicelocation && devicelocationalias && latitude && longitude && accountname && accountnamealias && devicedistancetoxfmr && deviceowner)
                    {
                        return duplicate;
                    }
                }
            }


            return null;
        }

        private PQds.Model.Meter MetadataToDevice(List<MetaDataTag> metaData)
        {
            PQds.Model.Meter device= new Meter();
            List<string> Tags = metaData.Select(item => item.key.ToLower()).ToList();

            if (Tags.Contains("devicename"))
            {
                device.DeviceName = ((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "devicename")).Value;
            }
            else
            {
                device.DeviceName = "PQDS File";
            }

            if (Tags.Contains("devicealias"))
            {
                device.DeviceAlias = ((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "devicealias")).Value;
            }

            if (Tags.Contains("devicelocation"))
            {
                device.DeviceLocation = ((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "devicelocation")).Value;
            }
            if (Tags.Contains("devicelocationalias"))
            {
                device.DeviceLocationAlias = ((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "devicelocationalias")).Value;
            }

            if (Tags.Contains("latitude"))
            {
                try
                {
                    device.Latitude = Convert.ToInt32(((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "latitude")).Value);
                }
                catch { }
                }
            if (Tags.Contains("longitude"))
            {
                try
                {
                    device.Longitude = Convert.ToInt32(((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "longitude")).Value);

                }
                catch { }
            }

            if (Tags.Contains("accountname"))
            {
                device.AccountName = ((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "accountname")).Value;
            }
            if (Tags.Contains("accountnamealias"))
            {
                device.AccountAlias = ((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "accountnamealias")).Value;
            }

            if (Tags.Contains("devicedistancetoxfmr"))
            {
                device.DistanceToXFR = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "devicedistancetoxfmr")).Value;
            }

            if (Tags.Contains("deviceowner"))
            {
                device.Owner = ((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "deviceowner")).Value;
            }
            //skip Connection Type for now

             return device;
        }

        private PQds.Model.Asset MetadataToAsset(List<MetaDataTag> metaData)
        {
            PQds.Model.Asset asset = new Asset();
            List<string> Tags = metaData.Select(item => item.key.ToLower()).ToList();

            if (Tags.Contains("assetname"))
            {
                asset.AssetKey = ((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "assetname")).Value;
            }
            else
            {

                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    asset.AssetKey = "PQDS File";
                    int i = 0;

                    GSF.Data.Model.TableOperations<PQds.Model.Asset> assetTbl = new GSF.Data.Model.TableOperations<PQds.Model.Asset>(connection);
                    while (assetTbl.QueryRecordCountWhere("AssetKey = {0}", asset.AssetKey) > 0)
                    {
                        i++;
                        asset.AssetKey = String.Format("PQDS File {0}",i);
                    }
                    
                }
            }

            if (Tags.Contains("nominalvoltage-lg"))
            {
                asset.NominalVoltage = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "nominalvoltage-lg")).Value;
            }
            if (Tags.Contains("nominalfrequency"))
            {
                asset.NominalFrequency = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "nominalfrequency")).Value;
            }
            if (Tags.Contains("linelength"))
            {
                asset.Length = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "linelength")).Value;
            }
            if (Tags.Contains("upstreamxfmr-kva"))
            {
                asset.UpstreamXFMR = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "upstreamxfmr-kva")).Value;
            }

            return asset;
        }

        private PQds.Model.Event MetadataToEvent(List<MetaDataTag> metaData)
        {
            PQds.Model.Event evt = new Event();
            List<string> Tags = metaData.Select(item => item.key.ToLower()).ToList();


            if (Tags.Contains("eventguid"))
            {
                evt.GUID = ((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "eventguid")).Value;
            }
            else
            {
                evt.GUID = (Guid.NewGuid()).ToString();
            }
            if (Tags.Contains("eventid"))
            {
                evt.Name = ((MetaDataTag<String>)metaData.Find(item => item.key.ToLower() == "eventid")).Value;
            }
            else
            {
                evt.Name = "PQDS File";
            }
            if (Tags.Contains("eventtypecode"))
            {
                evt.EventType = ((MetaDataTag<int>)metaData.Find(item => item.key.ToLower() == "eventtypecode")).Value;
            }
            if (Tags.Contains("eventfaulttypecode"))
            {
                evt.FaultType = ((MetaDataTag<int>)metaData.Find(item => item.key.ToLower() == "eventfaulttypecode")).Value;
            }

            if (Tags.Contains("eventpeakcurrent"))
            {
                evt.PeakCurrent = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventpeakcurrent")).Value;
            }
            if (Tags.Contains("eventpeakvoltage"))
            {
                evt.PeakVoltage = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "EventPeakVoltage")).Value;
            }

            if (Tags.Contains("eventmaxva"))
            {
                evt.MaxVA = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventmaxva")).Value;
            }
            if (Tags.Contains("eventmaxvb"))
            {
                evt.MaxVB = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventmaxvb")).Value;
            }
            if (Tags.Contains("eventmaxvc"))
            {
                evt.MaxVC = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventmaxvc")).Value;
            }

            if (Tags.Contains("eventminva"))
            {
                evt.MinVA = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventminva")).Value;
            }
            if (Tags.Contains("eventminvb"))
            {
                evt.MinVB = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventminvb")).Value;
            }
            if (Tags.Contains("eventminvc"))
            {
                evt.MinVC = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventminvc")).Value;
            }

            if (Tags.Contains("eventmaxia"))
            {
                evt.MaxIA = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventmaxia")).Value;
            }
            if (Tags.Contains("eventmaxib"))
            {
                evt.MaxIB = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventmaxib")).Value;
            }
            if (Tags.Contains("eventmaxic"))
            {
                evt.MaxIC = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventmaxic")).Value;
            }



            if (Tags.Contains("eventpreeventcurrent"))
            {
                evt.PreEventCurrent = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventpreeventcurrent")).Value;
            }
            if (Tags.Contains("eventpreeventvoltage"))
            {
                evt.PreEventVoltage = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventpreeventvoltage")).Value;
            }
            if (Tags.Contains("eventduration"))
            {
                evt.Duration = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventduration")).Value;
            }


            if (Tags.Contains("eventfaulti2t"))
            {
                evt.FaultI2T = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "eventfaulti2t")).Value;
            }
            if (Tags.Contains("distancetofault"))
            {
                evt.DistanceToFault = ((MetaDataTag<double>)metaData.Find(item => item.key.ToLower() == "distancetofault")).Value;
            }

            if (Tags.Contains("faultcause"))
            {
                evt.FaultCause = ((MetaDataTag<int>)metaData.Find(item => item.key.ToLower() == "faultcause")).Value;
            }
            
            return evt;
        }

        private PQds.Model.Asset MergeAssets(PQds.Model.Asset asset1, PQds.Model.Asset asset2)
        {
            PQds.Model.Asset result = asset1;

            if ((result.Length == null) && (asset2.Length != null))
                result.Length = asset2.Length;
            if ((result.UpstreamXFMR == null) && (asset2.UpstreamXFMR != null))
                result.UpstreamXFMR = asset2.UpstreamXFMR;
            if ((result.NominalFrequency == null) && (asset2.NominalFrequency != null))
                result.NominalFrequency = asset2.NominalFrequency;
            if ((result.NominalVoltage == null) && (asset2.NominalVoltage != null))
                result.NominalVoltage = asset2.NominalVoltage;
            return result;
        }

        private PQds.Model.Channel MergeChannel(PQds.Model.Channel channel1, PQds.Model.Channel channel2)
        {
            PQds.Model.Channel result = channel1;
            
            return result;
        }

        private PQds.Model.Meter MergeDevices(PQds.Model.Meter device1, PQds.Model.Meter device2)
        {
            PQds.Model.Meter result = device1;

            if (device2.DeviceAlias != null)
            {
                result.DeviceAlias = device2.DeviceAlias;
            }

            if (device2.DeviceLocation != null)
            {
                result.DeviceLocation = device2.DeviceLocation;
            }
            if (device2.DeviceLocationAlias != null)
            {
                result.DeviceLocationAlias = device2.DeviceLocationAlias;
            }
            if (device2.Latitude != null)
            {
                result.Latitude = device2.Latitude;
            }
            if (device2.Longitude != null)
            {
                result.Longitude = device2.Longitude;
            }
            if (device2.AccountName != null)
            {
                result.AccountName = device2.AccountName;
            }
            if (device2.AccountAlias != null)
            {
                result.AccountAlias = device2.AccountAlias;
            }
            if (device2.DistanceToXFR != null)
            {
                result.DistanceToXFR = device2.DistanceToXFR;
            }
            if (device2.Owner != null)
            {
                result.Owner = device2.Owner;
            }

            return result;
        }
        #endregion[MetaData]

        #endregion[methods]

    }

    /// <summary>
    /// Abstract Class of MetaData Tags for a <see cref="PQDSFile"/>.
    /// </summary>
    public abstract class MetaDataTag
    {
        #region[Properties]
        protected string m_key;
        protected string m_unit;
        protected PQDSDataType m_expectedDataType;
        protected string m_note;
        #endregion[Properties]

        #region[Methods]

        /// <summary>
        /// the Metadata Tag key.
        /// </summary>
        public String key { get { return (this.m_key); } }

        /// <summary>
        /// Converst the Metadata tag into a line of a PQDS file
        /// </summary>
        /// <returns>The metadataTag as a String</returns>
        public abstract String Write();

        /// <summary>
        /// Returns the PQDS datatype <see cref="PQDSDataType"/>
        /// </summary>
        /// <returns>The PQDS Datatype </returns>
        public abstract PQDSDataType Type();

        #endregion[Methods]
    }

    /// <summary>
    /// Class of MetaData Tags for a <see cref="PQDSFile"/>.
    /// </summary>
    public class MetaDataTag<DataType> : MetaDataTag 
    {
        #region[Properties]
        private DataType m_value;

        /// <summary>
        /// Value of the MetadataTag.
        /// </summary>
        public DataType Value { get { return m_value; } }

        #endregion[Properties]

        #region[Constructor]

        /// <summary>
        /// Creates a <see cref="MetaDataTag"/>.
        /// </summary>
        /// <param name="key"> key of the MetadataTag</param>
        /// <param name="value"> Value of the MetadataTag</param>
        public MetaDataTag (String key, DataType value)
        {
            this.m_value = value;

            this.m_key = key;
            if (!keyToDataTypeLookup.TryGetValue(key, out this.m_expectedDataType))
                this.m_expectedDataType = PQDSDataType.Text;

            if (!keyToUnitLookup.TryGetValue(key, out this.m_unit))
                this.m_unit = null;

            if (!keyToNoteLookup.TryGetValue(key, out this.m_note))
                this.m_note = null;

            //Check to ensure a string does not end up being a number etc...
            if (this.m_expectedDataType == PQDSDataType.AlphaNumeric)
            {
                if (!((value is string) | (value is Guid)))
                { throw new InvalidCastException("Can not cast object to Alphanumeric Type"); }
            }
            else if (this.m_expectedDataType == PQDSDataType.Numeric)
            {
                if (!((value is int) | (value is double)))
                { throw new InvalidCastException("Can not cast object to Numeric Type"); }
            }
            else if (this.m_expectedDataType == PQDSDataType.Enumeration)
            {
                if (!((value is int)))
                { throw new InvalidCastException("Can not cast object to Numeric Type"); }
            }
            else if (this.m_expectedDataType == PQDSDataType.Binary)
            {
                if (!((value is int) | (value is Boolean)))
                { throw new InvalidCastException("Can not cast object to Numeric Type"); }
            }

        }

        /// <summary>
        /// Creates a custom <see cref="MetaDataTag"/>.
        /// </summary>
        /// <param name="key"> key of the MetadataTag</param>
        /// <param name="value"> Value of the MetadataTag</param>
        /// <param name="valueType"> The <see cref="PQDSDataType"/> of the metadata tag</param>
        /// <param name="unit"> The unit of the metadata tag </param>
        /// <param name="description"> a describtion of the metadata tag</param>
        public MetaDataTag(String key, DataType value, PQDSDataType valueType, String unit, String description)
        {
            this.m_value = value;

            this.m_key = key;
            this.m_expectedDataType = valueType;

            if (unit.Trim('"') == "") { this.m_unit = null; }
            else { this.m_unit = unit.Trim('"'); }

            if (description.Trim('"') == "") { this.m_note = null; }
            else { this.m_note = description.Trim('"'); }

        }

        #endregion[Constructor]

        #region[Methods]

        /// <summary>
        /// Converst the Metadata tag into a line of a PQDS file
        /// </summary>
        /// <returns>The metadataTag as a String</returns>
        public override string Write()
        {
            string result = String.Format("{0},\"{1}\",{2},{3},\"{4}\"", 
                this.m_key, this.m_value, this.m_unit, DataTypeToCSV(this.m_expectedDataType), this.m_note);

            return result;
        }

        /// <summary>
        /// Returns the PQDS datatype <see cref="PQDSDataType"/>
        /// </summary>
        /// <returns>The PQDS Datatype </returns>
        public override PQDSDataType Type()
        {
            return this.m_expectedDataType;
        }

        #endregion[Methods]

        #region[Statics]

        private static readonly Dictionary<string, PQDSDataType> keyToDataTypeLookup = new Dictionary<string, PQDSDataType>()
            {
                {"DeviceName", PQDSDataType.Text },
                {"DeviceAlias", PQDSDataType.Text },
                {"DeviceLocation", PQDSDataType.Text },
                {"DeviceLocationAlias", PQDSDataType.Text },
                {"DeviceLatitude", PQDSDataType.Text },
                {"DeviceLongitude", PQDSDataType.Text },
                {"Accountname", PQDSDataType.Text },
                {"AccountNameAlias", PQDSDataType.Text },
                {"DeviceDistanceToXFMR", PQDSDataType.Numeric },
                {"DeviceConnectionTypeCode", PQDSDataType.Enumeration },
                {"DeviceOwner", PQDSDataType.Text },
                {"NominalVoltage-LG", PQDSDataType.Numeric },
                {"NominalFrequency", PQDSDataType.Numeric },
                {"UpstreamXFMR-kVA", PQDSDataType.Numeric },
                {"LineLength", PQDSDataType.Numeric },
                {"AssetName", PQDSDataType.Text },
                {"EventGUID", PQDSDataType.AlphaNumeric },
                {"EventID", PQDSDataType.Text },
                {"EventYear", PQDSDataType.Enumeration },
                {"EventMonth", PQDSDataType.Enumeration },
                {"EventDay", PQDSDataType.Enumeration },
                {"EventHour", PQDSDataType.Enumeration },
                {"EventMinute", PQDSDataType.Enumeration },
                {"EventSecond", PQDSDataType.Enumeration },
                {"EventNanoSecond", PQDSDataType.Numeric },
                {"EventDate", PQDSDataType.Text },
                {"EventTime", PQDSDataType.Text },
                {"EventTypeCode", PQDSDataType.Enumeration },
                {"EventFaultTypeCode", PQDSDataType.Enumeration },
                {"EventPeakCurrent", PQDSDataType.Numeric },
                {"EventPeakVoltage", PQDSDataType.Numeric },
                {"EventMaxVA", PQDSDataType.Numeric },
                {"EventMaxVB", PQDSDataType.Numeric },
                {"EventMaxVC", PQDSDataType.Numeric },
                {"EventMinVA", PQDSDataType.Numeric },
                {"EventMinVB", PQDSDataType.Numeric },
                {"EventMinVC", PQDSDataType.Numeric },
                {"EventMaxIA", PQDSDataType.Numeric },
                {"EventMaxIB", PQDSDataType.Numeric },
                {"EventMaxIC", PQDSDataType.Numeric },
                {"EventPreEventCurrent", PQDSDataType.Numeric },
                {"EventPreEventVoltage", PQDSDataType.Numeric },
                {"EventDuration", PQDSDataType.Numeric },
                {"EventFaultI2T", PQDSDataType.Numeric },
                {"DistanceToFault", PQDSDataType.Numeric },
                {"EventCauseCode", PQDSDataType.Enumeration },
                {"WaveformDataType", PQDSDataType.Enumeration },
                {"WaveFormSensitivityCode", PQDSDataType.Enumeration },
                {"WaveFormSensitivityNote", PQDSDataType.Text },
                {"Utility",  PQDSDataType.Text },
                {"ContactEmail",  PQDSDataType.Text }
            };

        private static readonly Dictionary<string, string> keyToUnitLookup = new Dictionary<string, string>()
            {
                {"DeviceName", null },
                {"DeviceAlias", null },
                {"DeviceLocation", null },
                {"DeviceLocationAlias", null },
                {"DeviceLatitude", null },
                {"DeviceLongitude", null },
                {"Accountname", null },
                {"AccountNameAlias", null },
                {"DeviceDistanceToXFMR", "feet" },
                {"DeviceConnectionTypeCode", null },
                {"DeviceOwner", null },
                {"NominalVoltage-LG", "Volts" },
                {"NominalFrequency", "Hz" },
                {"UpstreamXFMR-kVA", "kVA" },
                {"LineLength", "miles" },
                {"AssetName", null },
                {"EventGUID", null },
                {"EventID", null },
                {"EventYear", null },
                {"EventMonth", null },
                {"EventDay", null },
                {"EventHour", null },
                {"EventMinute", null },
                {"EventSecond", null },
                {"EventNanoSecond", null },
                {"EventDate", null },
                {"EventTime", null },
                {"EventTypeCode", null },
                {"EventFaultTypeCode", null },
                {"EventPeakCurrent", "Amps" },
                {"EventPeakVoltage", "Volts" },
                {"EventMaxVA", "Volts" },
                {"EventMaxVB", "Volts" },
                {"EventMaxVC", "Volts" },
                {"EventMinVA", "Volts" },
                {"EventMinVB", "Volts" },
                {"EventMinVC", "Volts" },
                {"EventMaxIA", "Amps" },
                {"EventMaxIB", "Amps" },
                {"EventMaxIC", "Amps" },
                {"EventPreEventCurrent", "Amps" },
                {"EventPreEventVoltage", "Volts" },
                {"EventDuration", "ms" },
                {"EventFaultI2T", "A2s" },
                {"DistanceToFault", "miles" },
                {"EventCauseCode", null },
                {"WaveformDataType", null },
                {"WaveFormSensitivityCode", null },
                {"WaveFormSensitivityNote", null },
                {"Utility", null },
                {"ContactEmail", null }
            };

        private static readonly Dictionary<string, string> keyToNoteLookup = new Dictionary<string, string>()
            {
                {"DeviceName", "Meter or measurement device name" },
                {"DeviceAlias", "Alternate meter or measurement device name" },
                {"DeviceLocation", "Meter or measurment device location name" },
                {"DeviceLocationAlias", "Alternate meter or device location name" },
                {"DeviceLatitude", "Latitude" },
                {"DeviceLongitude", "Longtitude" },
                {"Accountname", "Name of customer or account" },
                {"AccountNameAlias", "Alternate name of customer or account" },
                {"DeviceDistanceToXFMR", "Distance to the upstream transformer" },
                {"DeviceConnectionTypeCode", "PQDS code for meter connection type" },
                {"DeviceOwner", "Utility name" },
                {"NominalVoltage-LG", "Nominal Line to Ground Voltage" },
                {"NominalFrequency", "Nominal System frequency" },
                {"UpstreamXFMR-kVA", "Upstream Transformer size" },
                {"LineLength", "Length of the Line" },
                {"AssetName", "Asset name" },
                {"EventGUID", "Globally Unique Event Identifier" },
                {"EventID", "A user defined Event Name" },
                {"EventYear", "Year" },
                {"EventMonth", "Month" },
                {"EventDay", "Day" },
                {"EventHour", "Hour" },
                {"EventMinute", "Minute" },
                {"EventSecond", "Second" },
                {"EventNanoSecond", "Nanosconds" },
                {"EventDate", "Event Date" },
                {"EventTime", "Event Time" },
                {"EventTypeCode", "PQDS Event Type Code" },
                {"EventFaultTypeCode", "PQDS Fault Type Code" },
                {"EventPeakCurrent", "Peak Current"},
                {"EventPeakVoltage", "Peak Voltage" },
                {"EventMaxVA", "RMS Maximum A Phase Voltage" },
                {"EventMaxVB", "RMS Maximum B Phase Voltage" },
                {"EventMaxVC", "RMS Maximum C Phase Voltage" },
                {"EventMinVA", "RMS Minimum A Phase Voltage" },
                {"EventMinVB", "RMS Minimum B Phase Voltage" },
                {"EventMinVC", "RMS Minimum C Phase Voltage" },
                {"EventMaxIA", "RMS Maximum A Phase Current" },
                {"EventMaxIB", "RMS Maximum B Phase Current" },
                {"EventMaxIC", "RMS Maximum C Phase Current" },
                {"EventPreEventCurrent", "Pre Event Current" },
                {"EventPreEventVoltage", "pre Event Voltage" },
                {"EventDuration", "Event Duration" },
                {"EventFaultI2T", "I2(t) during Fault duration" },
                {"DistanceToFault", "Distance to Fault" },
                {"EventCauseCode", "PQDS Event Cause Code" },
                { "WaveformDataType", "PQDS Data Type Code"},
                {"WaveFormSensitivityCode", "PQDS Data Sensitivity Code" },
                {"WaveFormSensitivityNote", "Notes on the PQDS Data Sensitivity Code" },
                {"Utility", "Utility that Generated this Dataset" },
                {"ContactEmail", "Contact for Utility that Created this Dataset" }
            };

        private static string DataTypeToCSV(PQDSDataType dataType)
        {
            switch (dataType)
            {
                case (PQDSDataType.Text):
                    return "T";
                case (PQDSDataType.Numeric):
                    return "N";
                case (PQDSDataType.Enumeration):
                    return "E";
                case (PQDSDataType.AlphaNumeric):
                    return "A";
                case (PQDSDataType.Binary):
                    return "B";
                default:
                    return "T";
            }
        }


        #endregion[Statics]


    }

    /// <summary>
    /// PQDS metadata tag Datatypes according to PQDS spec.
    /// </summary>
    public enum PQDSDataType
    {
        Enumeration = 0,
        Numeric = 1,
        AlphaNumeric = 2,
        Text = 3,
        Binary = 4

    }


    /// <summary>
    /// Class sthat represents a PQDS file.
    /// </summary>
    public class PQDSFile
    {
        #region[Properties]

        private List<MetaDataTag> m_metaData;
        private Dictionary<string, PQds.Model.DataSeries> m_Data;
        private DateTime m_initialTS;
        #endregion[Properties]

        #region[Constructors]
        /// <summary>
        /// Creates a new PQDS file.
        /// </summary>
        /// <param name="dataSeries"> Measurment data to be included </param>
        /// <param name="initialTimeStamp"> Timestamp used as the beginning of the PQDS file </param>
        /// <param name="metaData"> List of MetaData to be included in the PQDS file </param>
        public PQDSFile(List<MetaDataTag> metaData, Dictionary<string, PQds.Model.DataSeries> dataSeries, DateTime initialTimeStamp)
        {
            if (metaData is null) { this.m_metaData = new List<MetaDataTag>(); }
            else { this.m_metaData = metaData; }

            this.m_initialTS = initialTimeStamp;
            this.m_Data = dataSeries;
        }

        /// <summary>
        /// Creates a new PQDS file.
        /// </summary>
        public PQDSFile()
        {
            this.m_metaData = new List<MetaDataTag>();
            this.m_Data = new Dictionary<string, DataSeries>();
        }

        #endregion[Constructors]

        #region[Methods]

        private void GetStartTime()
        {
            DateTime result;
            int? day = null;
            int? month = null;
            int? year = null;

            if (this.m_metaData.Select(item => item.key).Contains("eventdate"))
            {
                string val = ((MetaDataTag<String>)this.m_metaData.Find(item => item.key == "eventdate")).Value;
                if (DateTime.TryParseExact(val, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    day = result.Day;
                    month = result.Month;
                    year = result.Year;
                }
            }
            if (day is null)
            {
                if (this.m_metaData.Select(item => item.key).Contains("eventday"))
                {
                    day = ((MetaDataTag<int>)this.m_metaData.Find(item => item.key == "eventday")).Value;
                }
                else
                {
                    day = DateTime.Now.Day;
                }
            }
            if (month is null)
            {
                if (this.m_metaData.Select(item => item.key).Contains("eventmonth"))
                {
                    month = ((MetaDataTag<int>)this.m_metaData.Find(item => item.key == "eventmonth")).Value;
                }
                else
                {
                    month = DateTime.Now.Month;
                }
            }
            if (year is null)
            {
                if (this.m_metaData.Select(item => item.key).Contains("eventyear"))
                {
                    year = ((MetaDataTag<int>)this.m_metaData.Find(item => item.key == "eventyear")).Value;
                }
                else
                {
                    year = DateTime.Now.Year;
                }
            }

            int? hour = null;
            int? minute = null;
            int? second = null;

            if (this.m_metaData.Select(item => item.key).Contains("eventtime"))
            {
                string val = ((MetaDataTag<String>)this.m_metaData.Find(item => item.key == "eventtime")).Value;
                if (DateTime.TryParseExact(val, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    hour = result.Hour;
                    minute = result.Minute;
                    second = result.Second;
                }
            }
            if (hour is null)
            {
                if (this.m_metaData.Select(item => item.key).Contains("eventhour"))
                {
                    hour = ((MetaDataTag<int>)this.m_metaData.Find(item => item.key == "eventhour")).Value;
                }
                else
                {
                    hour = DateTime.Now.Hour;
                }
            }
            if (minute is null)
            {
                if (this.m_metaData.Select(item => item.key).Contains("eventminute"))
                {
                    minute = ((MetaDataTag<int>)this.m_metaData.Find(item => item.key == "eventminute")).Value;
                }
                else
                {
                    minute = DateTime.Now.Minute;
                }
            }
            if (second is null)
            {
                if (this.m_metaData.Select(item => item.key).Contains("eventsecond"))
                {
                    second = ((MetaDataTag<int>)this.m_metaData.Find(item => item.key == "eventsecond")).Value;
                }
                else
                {
                    second = DateTime.Now.Second;
                }
            }


            result = new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, (int)second);

            this.m_initialTS = result;
        }

        private MetaDataTag CreateMetaData(string[] flds)
        {

            string dataTypeString = flds[3].Trim().ToUpper();
            PQDSDataType dataType;

            switch (dataTypeString)
            {
                case "N":
                {
                        dataType = PQDSDataType.Numeric;
                        break;
                }
                case "E":
                    {
                        dataType = PQDSDataType.Enumeration;
                        break;
                    }
                case "B":
                    {
                        dataType = PQDSDataType.Binary;
                        break;
                    }
                case "A":
                    {
                        dataType = PQDSDataType.AlphaNumeric;
                        break;
                    }
                default:
                {
                        dataType = PQDSDataType.Text;
                        break;
                }
            }

            string key = flds[0].Trim().ToLower();
            string note = flds[4].Trim('"');
            string unit = flds[2].Trim('"');

            switch (dataType)
            {
                case (PQDSDataType.AlphaNumeric):
                    {
                        string value = flds[1].Trim('"');
                        return new MetaDataTag<String>(key, value, dataType, unit, note);
                    }
                case (PQDSDataType.Text):
                    {
                        string value = flds[1].Trim('"');
                        return new MetaDataTag<String>(key, value, dataType, unit, note);
                    }
                case (PQDSDataType.Enumeration):
                    {
                        int value = Convert.ToInt32(flds[1].Trim('"'));
                        return new MetaDataTag<int>(key, value, dataType, unit, note);
                    }
                case (PQDSDataType.Numeric):
                    {
                        double value = Convert.ToDouble(flds[1].Trim('"'));
                        return new MetaDataTag<double>(key, value, dataType, unit, note);
                    }
                case (PQDSDataType.Binary):
                    {
                        Boolean value = Convert.ToBoolean(flds[1].Trim('"'));
                        return new MetaDataTag<Boolean>(key, value, dataType, unit, note);
                    }
                default:
                    {
                        string value = flds[1].Trim('"');
                        return new MetaDataTag<String>(key, value, dataType, unit, note);
                    }
            }


        }

        private Boolean IsDataHeader(string line)
        {
            if (!line.Contains(","))
                return false;
            String[] flds = line.Split(',');

            if (flds[0].ToLower().Trim() == "waveform-data")
                return true;

            return false;
        }

        /// <summary>
        /// List of included Metadata tags.
        /// </summary>
        public List<MetaDataTag> MetaData
        {
            get { return this.m_metaData; }
        }

        /// <summary>
        /// Dictionary of data included in PQDS file.
        /// </summary>
        public Dictionary<String, PQds.Model.DataSeries> Data
        {
            get { return this.m_Data; }
        }

        /// <summary>
        /// Writes the content to a .csv file.
        /// </summary>
        /// <param name="file"> file name </param>
        /// <param name="progress"> <see cref="IProgress"/> Progress Token</param>
        public void WriteToFile(string file, IProgress<double> progress)
        {
            int n_data = this.Data.Select((item) => item.Value.Series.Count()).Max();
            int n_total = n_data + n_data + this.m_metaData.Count() + 1;

            //create the metadata header
            List<string> lines = new List<string>();
            lines = this.m_metaData.Select(item => item.Write()).ToList();

            lines.AddRange(DataLines(progress,n_total));


            // Open the file and write in each line
            using (StreamWriter fileWriter = new StreamWriter(File.OpenWrite(file)))
            {
                for (int i = 0; i<lines.Count(); i++)
                { 
                    fileWriter.WriteLine(lines[i]);
                    progress.Report((double)(n_data + i) / n_total);
                }
            }


        }

        /// <summary>
        /// Reads the content from a .csv file.
        /// </summary>
        /// <param name="filename"> file name</param>
        /// <param name="progress"> <see cref="IProgress"/> Progress Token</param>
        public void ReadFromFile(string filename, IProgress<double> progress)
        {
            List<string> lines = new List<string>();
            // Open the file and read each line
            using (StreamReader fileReader = new StreamReader(File.OpenRead(filename)))
            {
                while (!fileReader.EndOfStream)
                {
                    lines.Add(fileReader.ReadLine().Trim());
                }
            }

            int index = 0;
            String[] flds;
            // Parse MetaData Section
            this.m_metaData = new List<MetaDataTag>();

            while (!(IsDataHeader(lines[index])))
            {
                if (!lines[index].Contains(","))
                {
                    index++;
                    continue;
                }

                flds = lines[index].Split(',');

                if (flds.Count() < 5)
                {
                    index++;
                    continue;
                }
                this.m_metaData.Add(CreateMetaData(flds));
                index++;

                if (index == lines.Count())
                { throw new InvalidDataException("PQDS File not valid"); }
                progress.Report((double)index / (double)lines.Count());
            }

            //Parse Data  Header
            flds = lines[index].Split(',');

            if (flds.Count() < 2)
            {
                throw new InvalidDataException("PQDS File has invalid data section or no data");
            }

            this.m_Data = new Dictionary<string, DataSeries>();
            List<string> signals = new List<string>();
            List<List<DataPoint>> data = new List<List<DataPoint>>();


            for (int i = 1; i < flds.Count(); i++)
            {
                if (this.m_Data.Keys.Contains(flds[i].Trim().ToLower()))
                {
                    continue;
                }
                this.m_Data.Add(flds[i].Trim().ToLower(), new DataSeries());
                signals.Add(flds[i].Trim().ToLower());
                data.Add(new List<DataPoint>());
            }

            index++;
            //Parse Data
            GetStartTime();

            while (index < lines.Count())
            {
                if (!lines[index].Contains(","))
                {
                    index++;
                    continue;
                }

                flds = lines[index].Split(',');

                if (flds.Count() != (this.m_Data.Count() + 1))
                {
                    index++;
                    continue;
                }
                DateTime TS;
                try
                {
                    double ticks = Convert.ToDouble(flds[0].Trim());
                    TS = this.m_initialTS + new TimeSpan((Int64)(ticks * 100));
                }
                catch
                {
                    index++;
                    continue;
                }

                for (int i = 0; i < signals.Count(); i++)
                {
                    try
                    {
                        double value = Convert.ToDouble(flds[i + 1].Trim());
                        data[i].Add(new DataPoint() { Time = TS, Value = value });
                    }
                    catch
                    {
                        continue;
                    }
                }

                progress.Report((double)index / (double)lines.Count());
                index++;
            }

            for (int i = 0; i < signals.Count(); i++)
            {
                this.m_Data[signals[i]].Series = data[i];
            }
        }

        private List<String> DataLines(IProgress<double> progress, int n_total)
        {
            List<string> result = new List<string>();

            //ensure they all start at the same Time
            List<string> measurements = this.Data.Keys.ToList();
            DateTime initalStart = measurements.Select(item => this.Data[item].Series[0].Time).Min();
            List<TimeSpan> startTime = this.Data.Select(item => item.Value.Series[0].Time - initalStart).ToList();

            //1 ms difference is ok
            if (startTime.Max().TotalMilliseconds > 1)
            {
                throw new Exception("The measurements start at different times");
            }

            //write the header
            result.Add("waveform-data," + String.Join(",", measurements));

            //write the Data
            int n_data = measurements.Select(item => this.Data[item].Series.Count()).Max();

            for (int i = 0; i < n_data; i++)
            {
                TimeSpan dT = this.Data[measurements[0]].Series[i].Time - this.m_initialTS;
                result.Add(Convert.ToString(dT.TotalMilliseconds) + "," +
                    String.Join(",", measurements.Select(item => String.Format("{0:F12}",this.Data[item].Series[i].Value)).ToList()));
                progress.Report((double)i / (double)n_total);
            }

            return result;

        }

        #endregion[Methods]

    }

    public class PQDSLogFile
    {
        #region[Properties]
        private List<MetaDataTag> m_metaData;
        private DateTime m_created;
        private string m_FileGuid;
        private List<string> m_data;
        #endregion[Properties]

        #region[Methods]
        public PQDSLogFile(List<MetaDataTag> metadata, string FileGUID, List<string> measurements)
        {
            this.m_metaData = metadata;
            this.m_created = DateTime.Now;
            this.m_data = measurements;

        }

        public void Write(string filename)
        {
            bool includeSeperator = true;

            if (!File.Exists(filename))
            {
                using (File.Create(filename))
                includeSeperator = false;
            }

            using (StreamWriter sw = File.AppendText(filename))
            {
                if (includeSeperator)
                {
                    sw.WriteLine(new String('-', 45));
                }

                //Write LogFile Header (FileID, Date Created)
                sw.WriteLine(Header());

                List<string> lines = this.m_metaData.Select(item => item.Write()).ToList();

                foreach (string line in lines)
                    sw.WriteLine(line);

                sw.WriteLine("waveform-data," + String.Join(",", this.m_data));
            }

        }

        private string Header()
        {
            return String.Format("\"{0:d/M/yyyy HH:mm:ss}\",{1}",this.m_created,this.m_FileGuid);
        }

        #endregion[Methods]
    }
}