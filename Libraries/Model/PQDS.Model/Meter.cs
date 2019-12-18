//Copyright © 2019 Electric Power Research Institute, Inc. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 
//  Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  Neither the name of the EPRI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 


using System;
using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace PQds.Model
{
    public class Meter
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        [StringLength(50)]
        public string DeviceName { get; set; }

        [StringLength(50)]
        public string DeviceAlias { get; set; }

        [StringLength(50)]
        public string DeviceLocation { get; set; }

        [StringLength(50)]
        public string DeviceLocationAlias { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [StringLength(50)]
        public string AccountName { get; set; }

        [StringLength(50)]
        public string AccountAlias { get; set; }

        public double? DistanceToXFR { get; set; }

        [StringLength(50)]
        public string Owner { get; set; }

        public int? ConnectionType { get; set; }

    }
   
    public static partial class TableOperationsExtensions
    {
        public static Meter GetOrAdd(this TableOperations<Meter> eventTypeTable, string name, string alias)
        {
            Meter meter = eventTypeTable.QueryRecordWhere("DeviceName = {0}", name);

            if ((object)meter == null)
            {
                meter = new Meter();
                meter.DeviceName = name;
                meter.DeviceAlias = alias ?? name;

                try
                {
                    eventTypeTable.AddNewRecord(meter);
                }
                catch (Exception ex)
                {
                    return eventTypeTable.QueryRecordWhere("DeviceName = {0}", name);
                }

                meter.ID = eventTypeTable.Connection.ExecuteScalar<int>("SELECT @@IDENTITY");
            }

            return meter;
        }

    }

}
