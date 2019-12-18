//Copyright © 2019 Electric Power Research Institute, Inc. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 
//  Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  Neither the name of the EPRI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 

using GSF.Data;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PQds.Model;
using GSF.Diagnostics;

namespace SetupTool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    GSF.Data.Model.TableOperations<PQds.Model.Setting> tableOperations = new GSF.Data.Model.TableOperations<PQds.Model.Setting>(connection);

                    Setting company = tableOperations.QueryRecordWhere("Name = {0}", "contact.utility");
                    Setting email = tableOperations.QueryRecordWhere("Name = {0}", "contact.email");

                    int n = args.Count();

                    company.value = String.Join(" ",args.Take(n-1));
                    email.value = args[n-1];

                    tableOperations.UpdateRecord(company);
                    tableOperations.UpdateRecord(email);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
            }
        }
    }
}
