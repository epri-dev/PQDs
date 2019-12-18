//Copyright © 2019 Electric Power Research Institute, Inc. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 
//  Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  Neither the name of the EPRI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 


using System;
using GSF.Data;


namespace PQds.Model
{
    public static class ModelID
    {
        public static int GetID<T>(AdoDataConnection cxn)
        {
            string Name = typeof(T).Name;
            string m_identitySQL;
            switch (cxn.DatabaseType)
            {
                case DatabaseType.SQLServer:
                    m_identitySQL = "SELECT IDENT_CURRENT('" + Name + "')";
                    break;

                case DatabaseType.Oracle:
                    m_identitySQL = "SELECT SEQ_" + Name + ".CURRVAL from dual";
                    break;

                case DatabaseType.SQLite:
                    m_identitySQL = "SELECT last_insert_rowid()";
                    break;

                //case DatabaseType.PostgreSQL:
                //    if ((object)AutoIncField != null)
                //        m_identitySQL = "SELECT currval(pg_get_serial_sequence('" + Name.ToLower() + "', '" + AutoIncField.Name.ToLower() + "'))";
                //    else
                //        m_identitySQL = "SELECT lastval()";
                //   break;

                default:
                    m_identitySQL = "SELECT @@IDENTITY";
                    break;
            }

            return Convert.ToInt32(cxn.ExecuteScalar(m_identitySQL));
        }

    }
    

}
