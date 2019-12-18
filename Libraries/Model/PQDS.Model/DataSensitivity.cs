//Copyright © 2019 Electric Power Research Institute, Inc. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 
//  Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  Neither the name of the EPRI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 


using System.ComponentModel.DataAnnotations;
using System.Linq;
using GSF.Data;
using GSF.Data.Model;

namespace PQds.Model
{

    public class DataSensitivity
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        [StringLength(250)]
        public string Note { get; set; }

        public int Event { get; set; }

        public int Asset { get; set; }

        public int? DataSensitivityCode { get; set; }

        public static bool NoteisGlobal()
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<DataSensitivity> tableOperations = new GSF.Data.Model.TableOperations<DataSensitivity>(connection);

                int uniqueDataSensitivityNotes = tableOperations.QueryRecordsWhere("Note NOT NULL AND Note <> ''").Select(record => record.Note.ToLower()).Distinct().Count();

                if (uniqueDataSensitivityNotes == 1)
                    return true;
            }
            return false;
        }

        public static bool CodeisGlobal()
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<DataSensitivity> tableOperations = new GSF.Data.Model.TableOperations<DataSensitivity>(connection);

                int uniqueDataSensitivity = tableOperations.QueryRecordsWhere("DataSensitivityCode NOT NULL")
                    .Select(record => record.DataSensitivityCode).Distinct().Count();

                if (uniqueDataSensitivity == 1)
                    return true;
            }
            return false;
        }
    }
}
