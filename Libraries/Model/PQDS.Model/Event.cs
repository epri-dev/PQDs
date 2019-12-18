//******************************************************************************************************
//  Event.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  08/27/2019 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************


using System;
using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;


namespace PQds.Model
{
    public class Event
    {
        #region[Properties]
        [PrimaryKey(true)]
        public int ID { get; set; }

        [StringLength(50)]
        public string GUID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public double? PeakCurrent { get; set; }

        public double? PeakVoltage { get; set; }

        public double? MaxVA { get; set; }

        public double? MaxVB { get; set; }

        public double? MaxVC { get; set; }

        public double? MinVA { get; set; }

        public double? MinVB { get; set; }

        public double? MinVC { get; set; }

        public double? MaxIA { get; set; }

        public double? MaxIB { get; set; }

        public double? MaxIC { get; set; }

        public double? FaultI2T { get; set; }

        public double? PreEventCurrent { get; set; }

        public double? PreEventVoltage { get; set; }

        public double? Duration { get; set; }

        [StringLength(50)]
        public string Notes { get; set; }

        public double? DistanceToFault { get; set; }

        public DateTime? EventTime { get; set; }

        public int? FaultType { get; set; }

        public int? FaultCause { get; set; }

        public int? EventType { get; set; }

        #endregion[Properties]

        #region[Methods]
        public override string ToString()
        {
            if(this.EventTime is null)
            {
                return String.Format("{0} - {1}", this.Name, "N/A");
            }
            return String.Format("{0} - {1}", this.Name, ((DateTime)this.EventTime).ToString("dd/MM/yyyy HH:mm"));
        }
        #endregion[Methods]
    }

} 