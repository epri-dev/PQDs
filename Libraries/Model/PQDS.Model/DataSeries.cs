//Copyright © 2019 Electric Power Research Institute, Inc. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 
//  Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  Neither the name of the EPRI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 


using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using GSF.Data.Model;

namespace PQds.Model
{
    public class DataSeries
    {
        #region[Properties]

        [PrimaryKey(true)]
        public int ID { get; set; }

        public int ChannelID { get; set; }

        public int EventID { get; set; }

        public byte[] Data { get; set; }


        [NonRecordField]
        public List<DataPoint> Series
        {
            get { return ReadFrom((byte[])Data).ToList(); }
            set { this.Data = ToData(value); }
        }

        #endregion[Properties]

        #region[methods]

        private static IEnumerable<DataPoint> ReadFrom(byte[] data)
        {
            // If the blob contains the GZip header,
            // use the legacy deserialization algorithm
            if (data[0] == 0x1F && data[1] == 0x8B)
                return ReadFrom_Legacy(data);

            return ReadFrom_Custom(data);
        }

        private static IEnumerable<DataPoint> ReadFrom_Custom(byte[] data)
        {
            // Restore the GZip header before uncompressing
            data[0] = 0x1F;
            data[1] = 0x8B;

            byte[] uncompressedData = Inflate(data);

            // Restore the header to original to avoid reference issues
            data[0] = 0x44;
            data[1] = 0x33;

            int offset = 0;

            int samples = BitConverter.ToInt32(uncompressedData, offset);
            offset += sizeof(int);

            List<DateTime> times = new List<DateTime>();

            while (times.Count < samples)
            {
                int timeValues = BitConverter.ToInt32(uncompressedData, offset);
                offset += sizeof(int);

                long currentValue = BitConverter.ToInt64(uncompressedData, offset);
                offset += sizeof(long);
                times.Add(new DateTime(currentValue));

                for (int i = 1; i < timeValues; i++)
                {
                    currentValue += BitConverter.ToUInt16(uncompressedData, offset);
                    offset += sizeof(ushort);
                    times.Add(new DateTime(currentValue));
                }
            }

            int seriesIndex = 0;

            while (offset < uncompressedData.Length)
            {
                int seriesID = BitConverter.ToInt32(uncompressedData, offset);
                offset += sizeof(int);

                const ushort NaNValue = ushort.MaxValue;
                double decompressionOffset = BitConverter.ToDouble(uncompressedData, offset);
                double decompressionScale = BitConverter.ToDouble(uncompressedData, offset + sizeof(double));
                offset += 2 * sizeof(double);

                for (int i = 0; i < samples; i++)
                {
                    ushort compressedValue = BitConverter.ToUInt16(uncompressedData, offset);
                    offset += sizeof(ushort);

                    double decompressedValue = decompressionScale * compressedValue + decompressionOffset;

                    if (compressedValue == NaNValue)
                        decompressedValue = double.NaN;

                    yield return new DataPoint()
                    {
                        Time = times[i],
                        Value = decompressedValue
                    };
                }

                seriesIndex++;
            }
        }

        private static IEnumerable<DataPoint> ReadFrom_Legacy(byte[] data)
        {
            byte[] uncompressedData = Inflate(data);

            int offset = 0;
            int samples = BitConverter.ToInt32(uncompressedData, offset);
            DateTime[] times = new DateTime[samples];

            offset += sizeof(int);

            for (int i = 0; i < samples; i++)
            {
                times[i] = new DateTime(BitConverter.ToInt64(uncompressedData, offset));
                offset += sizeof(long);
            }

            int seriesIndex = 0;

            while (offset < uncompressedData.Length)
            {
                int seriesID = BitConverter.ToInt32(uncompressedData, offset);

                offset += sizeof(int);

                for (int i = 0; i < samples; i++)
                {
                    yield return new DataPoint()
                    {
                        Time = times[i],
                        Value = BitConverter.ToDouble(uncompressedData, offset)
                    };

                    offset += sizeof(double);
                }

                seriesIndex++;
            }
        }

        private static byte[] Inflate(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (GZipStream inflater = new GZipStream(stream, CompressionMode.Decompress))
            using (MemoryStream result = new MemoryStream())
            {
                inflater.CopyTo(result);
                return result.ToArray();
            }
        }

        private byte[] ToData(List<DataPoint> dataPoints)
        {
            var timeSeries = dataPoints
                .Select(dataPoint => new { Time = dataPoint.Time.Ticks, Compressed = false })
                .ToList();

            for (int i = 1; i < timeSeries.Count; i++)
            {
                long previousTimestamp = dataPoints[i - 1].Time.Ticks;
                long timestamp = timeSeries[i].Time;
                long diff = timestamp - previousTimestamp;

                if (diff >= 0 && diff <= ushort.MaxValue)
                    timeSeries[i] = new { Time = diff, Compressed = true };
            }

            int timeSeriesByteLength = timeSeries.Sum(obj => obj.Compressed ? sizeof(ushort) : sizeof(int) + sizeof(long));
            int dataSeriesByteLength = sizeof(int) + (2 * sizeof(double)) + (dataPoints.Count * sizeof(ushort));
            int totalByteLength = sizeof(int) + timeSeriesByteLength + (dataSeriesByteLength);

            byte[] data = new byte[totalByteLength];
            int offset = 0;

            offset += GSF.LittleEndian.CopyBytes(dataPoints.Count, data, offset);

            List<int> uncompressedIndexes = timeSeries
                .Select((obj, Index) => new { obj.Compressed, Index })
                .Where(obj => !obj.Compressed)
                .Select(obj => obj.Index)
                .ToList();

            for (int i = 0; i < uncompressedIndexes.Count; i++)
            {
                int index = uncompressedIndexes[i];
                int nextIndex = (i + 1 < uncompressedIndexes.Count) ? uncompressedIndexes[i + 1] : timeSeries.Count;

                offset += GSF.LittleEndian.CopyBytes(nextIndex - index, data, offset);
                offset += GSF.LittleEndian.CopyBytes(timeSeries[index].Time, data, offset);

                for (int j = index + 1; j < nextIndex; j++)
                    offset += GSF.LittleEndian.CopyBytes((ushort)timeSeries[j].Time, data, offset);
            }
            
            const ushort NaNValue = ushort.MaxValue;
            const ushort MaxCompressedValue = ushort.MaxValue - 1;
            int seriesID = this.ID;
            double dataSeriesMaximum = dataPoints.Select(dataPoint => dataPoint.Value).ToList<double>().Max();
            double dataSeriesMinimum = dataPoints.Select(dataPoint => dataPoint.Value).ToList<double>().Min();

            double range = dataSeriesMaximum - dataSeriesMinimum;
            double decompressionOffset = dataSeriesMinimum;
            double decompressionScale = range / MaxCompressedValue;
            double compressionScale = (decompressionScale != 0.0D) ? 1.0D / decompressionScale : 0.0D;

            offset += GSF.LittleEndian.CopyBytes(seriesID, data, offset);
            offset += GSF.LittleEndian.CopyBytes(decompressionOffset, data, offset);
            offset += GSF.LittleEndian.CopyBytes(decompressionScale, data, offset);

            foreach (DataPoint dataPoint in dataPoints)
            {
                ushort compressedValue = (ushort)Math.Round((dataPoint.Value - decompressionOffset) * compressionScale);

                if (compressedValue == NaNValue)
                    compressedValue--;

                if (double.IsNaN(dataPoint.Value))
                    compressedValue = NaNValue;

                offset += GSF.LittleEndian.CopyBytes(compressedValue, data, offset);
            }
            

            byte[] returnArray = Ionic.Zlib.GZipStream.CompressBuffer(data);
            returnArray[0] = 0x44;
            returnArray[1] = 0x33;

            return returnArray;
        }


        #endregion[methods]
    }

    public class DataPoint
    {
        public DateTime Time;
        public double Value;
    }

}
