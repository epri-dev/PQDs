﻿//Copyright © 2019 Electric Power Research Institute, Inc. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 
//  Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  Neither the name of the EPRI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 



using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;


namespace PQds.Model
{
    public class Setting
    {
        #region[Properties]
        [PrimaryKey(true)]
        public int ID { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [StringLength(50)]
        public string value { get; set; }

        #endregion[Properties]

        #region[Methods]
        
        #endregion[Methods]
    }

} 