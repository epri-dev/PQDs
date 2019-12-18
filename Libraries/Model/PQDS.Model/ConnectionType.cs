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


namespace PQds.Model
{
    
    public class ConnectionType
    {
        public static readonly int Unknown = 0;
        public static readonly int P3W4 = 1;
        public static readonly int P3W3 = 2;
        public static readonly int P1W2 = 3;
        public static readonly int P1W3 = 4;
        public static readonly int P2W3 = 5;

        public static int ToValue(string Selection)
        {
            try
            {
                return DisplayToValue.Find(item => item.Item1.ToLower() == Selection.ToLower()).Item2;
            }
            catch
            { return Unknown; }
        }

        public static string ToDisplay(int? Selection)
        {
            if (Selection == null)
            { return ""; }
            try
            {
                return DisplayToValue.Find(item => item.Item2 == Selection).Item1;
            }
            catch
            { return "Other"; }
        }

        private static List<Tuple<String, int>> DisplayToValue
        {
            get
            {
                return (new List<Tuple<string, int>>()
                    {
                        new Tuple<string, int>("Unknown",Unknown),
                        new Tuple<string, int>("3 Phase 4 Wires",P3W4),
                        new Tuple<string, int>("3 Phase 3 Wires",P3W3),
                        new Tuple<string, int>("Single Phase 2 Wire",P1W2),
                        new Tuple<string, int>("Single Phase 3 Wires",P1W3),
                        new Tuple<string, int>("2 Phase 3 Wires",P2W3),
                        new Tuple<string, int>("",-1)
                    });
            }
        }

        public static String[] DisplayOptions()
        {
            return DisplayToValue.Select(item => item.Item1).ToArray();
        }
    }

    public class DataSensitivityCode
    {
        public static readonly int Internal = 0;
        public static readonly int Business = 1;
        public static readonly int Reliability = 2;
        public static readonly int Public = 3;

        public static int ToValue(string Selection)
        {
            try
            {
                return DisplayToValue.Find(item => item.Item1.ToLower() == Selection.ToLower()).Item2;
            }
            catch
            { return Internal; }
        }

        public static string ToDisplay(int? Selection)
        {
            if (Selection == null)
            { return ""; }
            try
            {
                return DisplayToValue.Find(item => item.Item2 == Selection).Item1;
            }
            catch
            { return "Other"; }
        }

        private static List<Tuple<String, int>> DisplayToValue
        {
            get
            {
                return (new List<Tuple<string, int>>()
                    {
                        new Tuple<string, int>("Internal use only",Internal),
                        new Tuple<string, int>("Business sensitive",Business),
                        new Tuple<string, int>("Reliabilty sensitive",Reliability),
                        new Tuple<string, int>("Public",Public),
                        new Tuple<string, int>("",-1)
                    });
            }
        }

        public static String[] DisplayOptions()
        {
            return DisplayToValue.Select(item => item.Item1).ToArray();
        }
    }

    public class FaultType
    {
        public static readonly int Unknown = 0;
        public static readonly int Fault = 1;
        public static readonly int Sag = 2;
        public static readonly int Swell = 3;
        public static readonly int Interruption = 4;
        public static readonly int Harmonics = 5;
        public static readonly int Other = 6;

        public static int ToValue(string Selection)
        {
            try
            {
                return DisplayToValue.Find(item => item.Item1.ToLower() == Selection.ToLower()).Item2;
            }
            catch
            { return Other; }
        }

        public static string ToDisplay(int? Selection)
        {
            if (Selection == null)
            { return ""; }
            try
            {
                return DisplayToValue.Find(item => item.Item2 == Selection).Item1;
            }
            catch
            { return "Other"; }
        }

        private static List<Tuple<String, int>> DisplayToValue
        {
            get
            {
                return (new List<Tuple<string, int>>()
                    {
                        new Tuple<string, int>("Unknown",Unknown),
                        new Tuple<string, int>("Fault",Fault),
                        new Tuple<string, int>("Sag",Sag),
                        new Tuple<string, int>("Swell",Swell),
                        new Tuple<string, int>("Interruption",Interruption),
                        new Tuple<string, int>("Harmonics",Harmonics),
                        new Tuple<string, int>("Other",Other),
                        new Tuple<string, int>("",-1)
                    });
            }
        }

        public static String[] DisplayOptions()
        {
            return DisplayToValue.Select(item => item.Item1).ToArray();
        }
    }

    public class EventType
    {
        public static readonly int Unknown = 0;
        public static readonly int LG = 1;
        public static readonly int LL = 2;
        public static readonly int LLG = 3;
        public static readonly int LLL = 4;
        public static readonly int LLLG = 5;

        public static int ToValue(string Selection)
        {
            try
            {
                return DisplayToValue.Find(item => item.Item1.ToLower() == Selection.ToLower()).Item2;
            }
            catch
            { return -1; }
        }

        public static string ToDisplay(int? Selection)
        {
            if (Selection == null)
            { return "";  }
            try
            {
                return DisplayToValue.Find(item => item.Item2 == Selection).Item1;
            }
            catch
            { return "Other"; }
        }

        private static List<Tuple<String, int>> DisplayToValue
        {
            get
            {
                return (new List<Tuple<string, int>>()
                    {
                        new Tuple<string, int>("Unknown",Unknown),
                        new Tuple<string, int>("2 phase",LL),
                        new Tuple<string, int>("single phase to GND",LG),
                        new Tuple<string, int>("3 phase",LLL),
                        new Tuple<string, int>("2 phase to GND",LLG),
                        new Tuple<string, int>("3 phase to GND",LLLG),
                        new Tuple<string, int>("",-1)
                    });
            }
        }

        public static String[] DisplayOptions()
        {
            return DisplayToValue.Select(item => item.Item1).ToArray();
        }
    }

    public class FaultCause
    {
        public static readonly int Unknown = 0;
        public static readonly int Lightning = 1;
        public static readonly int Animal = 2;
        public static readonly int Vegitation = 3;
        public static readonly int Contamination = 4;
        public static readonly int Smoke = 5;
        public static readonly int Ice = 6;
        public static readonly int Wind = 7;
        public static readonly int Utility = 8;
        public static readonly int Customer = 9;
        public static readonly int Vandalism = 10;
        public static readonly int Human = 11;
        public static readonly int Other = 12;


        public static int? ToValue(string Selection)
        {
            try
            {
                return DisplayToValue.Find(item => item.Item1.ToLower() == Selection.ToLower()).Item2;
            }
            catch
            { return null; }
        }

        public static string ToDisplay(int? Selection)
        {
            if (Selection == null)
            { return ""; }
            try
            {
                return DisplayToValue.Find(item => item.Item2 == Selection).Item1;
            }
            catch
            { return "Other"; }
        }

        private static List<Tuple<String, int>> DisplayToValue
        {
            get
            {
                return (new List<Tuple<string, int>>()
                    {
                        new Tuple<string, int>("Unknown",Unknown),
                        new Tuple<string, int>("Lightning",Lightning),
                        new Tuple<string, int>("Animal",Animal),
                        new Tuple<string, int>("Vegitation",Vegitation),
                        new Tuple<string, int>("Contamination",Contamination),
                        new Tuple<string, int>("Smoke",Smoke),
                        new Tuple<string, int>("Ice or Snow",Ice),
                        new Tuple<string, int>("Wind",Wind),
                        new Tuple<string, int>("Utility equipment",Utility),
                        new Tuple<string, int>("Customer Equipment",Customer),
                        new Tuple<string, int>("Vandalism",Vandalism),
                        new Tuple<string, int>("Human Error",Human),
                        new Tuple<string, int>("Other",Other),
                        new Tuple<string, int>("",-1)
                });
            }
        }

        public static String[] DisplayOptions()
        {
            return DisplayToValue.Select(item => item.Item1).ToArray();
        }
    }

    public class SignalType
    {
        public static readonly int PointOnWave = 0;
        public static readonly int RMS = 1;
        public static readonly int other = 2;

        public static int ToValue(string Selection)
        {
            try
            {
                return DisplayToValue.Find(item => item.Item1.ToLower() == Selection.ToLower()).Item2;
            }
            catch
            { return other; }
        }

        public static string ToDisplay(int Selection)
        {
            try
            {
                return DisplayToValue.Find(item => item.Item2 == Selection).Item1;
            }
            catch
            { return "Other"; }
        }

        private static List<Tuple<String, int>> DisplayToValue
        {
            get
            {
                return (new List<Tuple<string, int>>()
                    {
                        new Tuple<string, int>("Point on Wave",PointOnWave),
                        new Tuple<string, int>("RMS",RMS),
                        new Tuple<string, int>("Other",other)
                    });
            }
        }

        public static String[] DisplayOptions()
        {
            return DisplayToValue.Select(item => item.Item1).ToArray();
        }
    }
    
    public static class MeasurementType
        {
            public static readonly string VoltageA = "va";
            public static readonly string VoltageB = "vb";
            public static readonly string VoltageC = "vc";

            public static readonly string CurrentA = "ia";
            public static readonly string CurrentB = "ib";
            public static readonly string CurrentC = "ic";

            public static readonly string Frequency = "f";

            public static readonly string other = "";

            public static string ToValue(string Selection)
            {
                try
                {
                    return DisplayToValue.Find(item => item.Item1.ToLower() == Selection.ToLower()).Item2;
                }
                catch
                { return other; }
            }

            public static string ToDisplay(string Selection)
            {
                try
                {
                    return DisplayToValue.Find(item => item.Item2.ToLower() == Selection.ToLower()).Item1;
                }
                catch
                { return "Other"; }
            }

            private static List<Tuple<String, String>> DisplayToValue
            {
                get
                {
                    return (new List<Tuple<string, string>>()
                    {
                        new Tuple<string, string>("Voltage Phase A",VoltageA),
                        new Tuple<string, string>("Voltage Phase B",VoltageB),
                        new Tuple<string, string>("Voltage Phase C",VoltageC),
                        new Tuple<string, string>("Current Phase A",CurrentA),
                        new Tuple<string, string>("Current Phase B",CurrentB),
                        new Tuple<string, string>("Current Phase C",CurrentC),
                        new Tuple<string, string>("Frequency",Frequency),
                        new Tuple<string, string>("Other",other)

                    });
                }
            }

            public static String[] DisplayOptions()
            {
                return DisplayToValue.Select(item => item.Item1).ToArray();
            }
    }


}
