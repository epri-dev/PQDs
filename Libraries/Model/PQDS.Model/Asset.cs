//Copyright © 2019 Electric Power Research Institute, Inc. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 
//  Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  Neither the name of the EPRI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 

using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using Newtonsoft.Json;


namespace PQds.Model
{
    public class Asset
    {
        #region[Properties]

        [PrimaryKey(true)]
        public int ID { get; set; }

        public double? NominalVoltage { get; set; }

        public double? NominalFrequency { get; set; }

        public double? UpstreamXFMR { get; set; }

        public double? Length { get; set; }

        [Required]
        [StringLength(50)]
        public string AssetKey { get; set; }

        #endregion[Properties]

        #region[Methods]

        public override string ToString()
        {
            return this.AssetKey;
        }

        #endregion[Methods]
    }

    public class AssetToEvent
    {
        public int EventID { get; set; }
        public int AssetID { get; set; }
        public int DataSeriesID { get; set; }
    }
}

    //public static partial class TableOperationsExtensions
    //{
    //    public static Asset GetOrAdd(this TableOperations<Asset> AssetTable, string name, string description = null)
    //    {
    //        Asset phase = AssetTable.QueryRecordWhere("Name = {0}", name);
    //
    //        if ((object)phase == null)
    /*        {
                phase = new Phase();
                phase.Name = name;
                phase.Description = description ?? name;

                try
                {
                    phaseTable.AddNewRecord(phase);
                }
                catch (Exception ex)
                {
                    // Ignore errors regarding unique key constraints
                    // which can occur as a result of a race condition
                    bool isUniqueViolation = ExceptionHandler.IsUniqueViolation(ex);

                    if (!isUniqueViolation)
                        throw;

                    return phaseTable.QueryRecordWhere("Name = {0}", name);
                }

                phase.ID = phaseTable.Connection.ExecuteScalar<int>("SELECT @@IDENTITY");
            }

            return phase;
        } */

