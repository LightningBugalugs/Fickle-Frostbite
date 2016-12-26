using FickleFrostbite.FIT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GarminTo
{
    /// <summary>
    /// <para>Main Program loop for GarminTo application (console application)</para>
    /// </summary>
    public class Program
    {
        /* regular expressions for processing export option args */
        protected static Regex googleMapsRegex = new Regex(@"/GoogleMaps\(DIR:(?<googleMapsExportDirectory>.*)\)"); //example: /GoogleMaps(DIR:D:\temp\googleMaps)
        protected static Regex sqlRegex = new Regex(@"/SQL\(SERVER:(?<serverName>.*);DB:(?<databaseName>.*);UID:(?<username>.*);PWD:(?<password>.*)\)"); //example: /SQL(SERVER:.;DB:GarminToSql;UID:FickleFrostbite;PWD:FickleFrostbite)
        protected static Regex excel2013Regex = new Regex(@"/Excel2013\(DIR:(?<excel2013ExportDirectory>.*)\)"); //example: /Excel2013(DIR:D:\temp\garmin)

        static Dictionary<ushort, int> mesgCounts = new Dictionary<ushort, int>();
        static FileStream fitSource;

        #region Message Handlers
        // Client implements their handlers of interest and subscribes to MesgBroadcaster events
        static void OnMesgDefn(object sender, MesgDefinitionEventArgs e)
        {
            Console.WriteLine("OnMesgDef: Received Defn for local message #{0}, global num {1}", e.mesgDef.LocalMesgNum, e.mesgDef.GlobalMesgNum);
            Console.WriteLine("\tIt has {0} fields and is {1} bytes long", e.mesgDef.NumFields, e.mesgDef.GetMesgSize());
        }

        static void OnMesg(object sender, MesgEventArgs e)
        {
            // important items
            //      

            /* *
                OnMesg: Received Mesg with global ID#101, its name is Length
	                Field0 Index0 ("Timestamp" Field#253) Value: 4294967295 (raw value 4294967295)
	                Field1 Index0 ("StartTime" Field#2) Value: 820537806 (raw value 820537806)
	                Field2 Index0 ("TotalElapsedTime" Field#3) Value: 70.375 (raw value 70375)
	                Field3 Index0 ("TotalTimerTime" Field#4) Value: 70.375 (raw value 70375)
	                Field4 Index0 ("MessageIndex" Field#254) Value: 3 (raw value 3)
	                Field5 Index0 ("TotalStrokes" Field#5) Value: 28 (raw value 28)
	                Field6 Index0 ("AvgSpeed" Field#6) Value: 0.71 (raw value 710)
	                Field7 Index0 ("TotalCalories" Field#11) Value: 15 (raw value 15)
	                Field8 Index0 ("Event" Field#0) Value: 28 (raw value 28)
	                Field9 Index0 ("EventType" Field#1) Value: 1 (raw value 1)
	                Field10 Index0 ("SwimStroke" Field#7) Value: 0 (raw value 0)
	                Field11 Index0 ("AvgSwimmingCadence" Field#9) Value: 24 (raw value 24)
	                Field12 Index0 ("EventGroup" Field#10) Value: 255 (raw value 255)
	                Field13 Index0 ("LengthType" Field#12) Value: 1 (raw value 1)
             * */
            /* *
                OnMesg: Received Mesg with global ID#18, its name is Session
	                Field0 Index0 ("Timestamp" Field#253) Value: 820540407 (raw value 820540407)
	                Field1 Index0 ("StartTime" Field#2) Value: 820537603 (raw value 820537603)
	                Field2 Index0 ("StartPositionLat" Field#3) Value: 2147483647 (raw value 2147483647)
	                Field3 Index0 ("StartPositionLong" Field#4) Value: 2147483647 (raw value 2147483647)
	                Field4 Index0 ("TotalElapsedTime" Field#7) Value: 2793.562 (raw value 2793562)
	                Field5 Index0 ("TotalTimerTime" Field#8) Value: 2793.562 (raw value 2793562)
	                Field6 Index0 ("TotalDistance" Field#9) Value: 2000 (raw value 200000)
	                Field7 Index0 ("TotalCycles" Field#10) Value: 1151 (raw value 1151)
	                Field8 Index0 ("NecLat" Field#29) Value: 2147483647 (raw value 2147483647)
	                Field9 Index0 ("NecLong" Field#30) Value: 2147483647 (raw value 2147483647)
	                Field10 Index0 ("SwcLat" Field#31) Value: 2147483647 (raw value 2147483647)
	                Field11 Index0 ("SwcLong" Field#32) Value: 2147483647 (raw value 2147483647)
	                Field12 Index0 ("unknown" Field#38) Value: 2147483647 (raw value 2147483647)
	                Field13 Index0 ("unknown" Field#39) Value: 2147483647 (raw value 2147483647)
	                Field14 Index0 ("AvgStrokeCount" Field#41) Value: 28.8 (raw value 288)
	                Field15 Index0 ("TotalWork" Field#48) Value: 4294967295 (raw value 4294967295)
	                Field16 Index0 ("TimeInPowerZone" Field#68) Value: 4294968 (raw value 4294967295)
	                Field17 Index0 ("MessageIndex" Field#254) Value: 0 (raw value 0)
	                Field18 Index0 ("TotalCalories" Field#11) Value: 606 (raw value 606)
	                Field19 Index0 ("TotalFatCalories" Field#13) Value: 0 (raw value 0)
	                Field20 Index0 ("AvgSpeed" Field#14) Value: 0.718 (raw value 718)
	                Field21 Index0 ("MaxSpeed" Field#15) Value: 0.83 (raw value 830)
	                Field22 Index0 ("AvgPower" Field#20) Value: 65535 (raw value 65535)
	                Field23 Index0 ("MaxPower" Field#21) Value: 65535 (raw value 65535)
	                Field24 Index0 ("TotalAscent" Field#22) Value: 65535 (raw value 65535)
	                Field25 Index0 ("TotalDescent" Field#23) Value: 65535 (raw value 65535)
	                Field26 Index0 ("FirstLapIndex" Field#25) Value: 0 (raw value 0)
	                Field27 Index0 ("NumLaps" Field#26) Value: 1 (raw value 1)
	                Field28 Index0 ("unknown" Field#33) Value: 40 (raw value 40)
	                Field29 Index0 ("NormalizedPower" Field#34) Value: 65535 (raw value 65535)
	                Field30 Index0 ("TrainingStressScore" Field#35) Value: 6553.5 (raw value 65535)
	                Field31 Index0 ("IntensityFactor" Field#36) Value: 65.535 (raw value 65535)
	                Field32 Index0 ("LeftRightBalance" Field#37) Value: 65535 (raw value 65535)
	                Field33 Index0 ("AvgStrokeDistance" Field#42) Value: 1.74 (raw value 174)
	                Field34 Index0 ("PoolLength" Field#44) Value: 50 (raw value 5000)
	                Field35 Index0 ("ThresholdPower" Field#45) Value: 65535 (raw value 65535)
	                Field36 Index0 ("NumActiveLengths" Field#47) Value: 40 (raw value 40)
	                Field37 Index0 ("Event" Field#0) Value: 9 (raw value 9)
	                Field38 Index0 ("EventType" Field#1) Value: 1 (raw value 1)
	                Field39 Index0 ("Sport" Field#5) Value: 5 (raw value 5)
	                Field40 Index0 ("SubSport" Field#6) Value: 17 (raw value 17)
	                Field41 Index0 ("AvgHeartRate" Field#16) Value: 255 (raw value 255)
	                Field42 Index0 ("MaxHeartRate" Field#17) Value: 255 (raw value 255)
	                Field43 Index0 ("AvgCadence" Field#18) Value: 25 (raw value 25)
	                Field44 Index0 ("MaxCadence" Field#19) Value: 26 (raw value 26)
	                Field45 Index0 ("TotalTrainingEffect" Field#24) Value: 25.5 (raw value 255)
	                Field46 Index0 ("EventGroup" Field#27) Value: 255 (raw value 255)
	                Field47 Index0 ("Trigger" Field#28) Value: 0 (raw value 0)
	                Field48 Index0 ("SwimStroke" Field#43) Value: 0 (raw value 0)
	                Field49 Index0 ("PoolLengthUnit" Field#46) Value: 0 (raw value 0)
	                Field50 Index0 ("EnhancedAvgSpeed" Field#124) Value: 0.718 (raw value 718)
	                Field51 Index0 ("EnhancedMaxSpeed" Field#125) Value: 0.83 (raw value 830)
             * */

            var filename = @"D:\temp\fit-files\message.txt";
            System.IO.File.AppendAllText(filename, System.Environment.NewLine);
            System.IO.File.AppendAllText(filename, String.Format("OnMesg: Received Mesg with global ID#{0}, its name is {1}", e.mesg.Num, e.mesg.Name));
            Console.WriteLine("OnMesg: Received Mesg with global ID#{0}, its name is {1}", e.mesg.Num, e.mesg.Name);

            for (byte i = 0; i < e.mesg.GetNumFields(); i++)
            {
                for (int j = 0; j < e.mesg.fields[i].GetNumValues(); j++)
                {
                    System.IO.File.AppendAllText(filename, System.Environment.NewLine);
                    System.IO.File.AppendAllText(filename, String.Format("\tField{0} Index{1} (\"{2}\" Field#{4}) Value: {3} (raw value {5})", i, j, e.mesg.fields[i].GetName(), e.mesg.fields[i].GetValue(j), e.mesg.fields[i].Num, e.mesg.fields[i].GetRawValue(j)));
                    Console.WriteLine("\tField{0} Index{1} (\"{2}\" Field#{4}) Value: {3} (raw value {5})", i, j, e.mesg.fields[i].GetName(), e.mesg.fields[i].GetValue(j), e.mesg.fields[i].Num, e.mesg.fields[i].GetRawValue(j));
                }
            }

            if (mesgCounts.ContainsKey(e.mesg.Num) == true)
            {
                mesgCounts[e.mesg.Num]++;
            }
            else
            {
                mesgCounts.Add(e.mesg.Num, 1);
            }

            System.IO.File.AppendAllText(filename, System.Environment.NewLine);
        }

        static void OnFileIDMesg(object sender, MesgEventArgs e)
        {
            Console.WriteLine("FileIdHandler: Received {1} Mesg with global ID#{0}", e.mesg.Num, e.mesg.Name);
            FileIdMesg myFileId = (FileIdMesg)e.mesg;
            try
            {
                Console.WriteLine("\tType: {0}", myFileId.GetType());
                Console.WriteLine("\tManufacturer: {0}", myFileId.GetManufacturer());
                Console.WriteLine("\tProduct: {0}", myFileId.GetProduct());
                Console.WriteLine("\tSerialNumber {0}", myFileId.GetSerialNumber());
                Console.WriteLine("\tNumber {0}", myFileId.GetNumber());
                Console.WriteLine("\tTimeCreated {0}", myFileId.GetTimeCreated());
                FickleFrostbite.FIT.DateTime dtTime = new FickleFrostbite.FIT.DateTime(myFileId.GetTimeCreated().GetTimeStamp());

            }
            catch (FitException exception)
            {
                Console.WriteLine("\tOnFileIDMesg Error {0}", exception.Message);
                Console.WriteLine("\t{0}", exception.InnerException);
            }
        }
        static void OnUserProfileMesg(object sender, MesgEventArgs e)
        {
            Console.WriteLine("UserProfileHandler: Received {1} Mesg, it has global ID#{0}", e.mesg.Num, e.mesg.Name);
            UserProfileMesg myUserProfile = (UserProfileMesg)e.mesg;
            try
            {
                Console.WriteLine("\tFriendlyName \"{0}\"", Encoding.UTF8.GetString(myUserProfile.GetFriendlyName()));
                Console.WriteLine("\tGender {0}", myUserProfile.GetGender().ToString());
                Console.WriteLine("\tAge {0}", myUserProfile.GetAge());
                Console.WriteLine("\tWeight  {0}", myUserProfile.GetWeight());
            }
            catch (FitException exception)
            {
                Console.WriteLine("\tOnUserProfileMesg Error {0}", exception.Message);
                Console.WriteLine("\t{0}", exception.InnerException);
            }
        }

        static void OnDeviceInfoMessage(object sender, MesgEventArgs e)
        {
            Console.WriteLine("DeviceInfoHandler: Received {1} Mesg, it has global ID#{0}", e.mesg.Num, e.mesg.Name);
            DeviceInfoMesg myDeviceInfoMessage = (DeviceInfoMesg)e.mesg;
            try
            {
                Console.WriteLine("\tTimestamp  {0}", myDeviceInfoMessage.GetTimestamp());
                Console.WriteLine("\tBattery Status{0}", myDeviceInfoMessage.GetBatteryStatus());
            }
            catch (FitException exception)
            {
                Console.WriteLine("\tOnDeviceInfoMesg Error {0}", exception.Message);
                Console.WriteLine("\t{0}", exception.InnerException);
            }
        }

        static void OnMonitoringMessage(object sender, MesgEventArgs e)
        {
            Console.WriteLine("MonitoringHandler: Received {1} Mesg, it has global ID#{0}", e.mesg.Num, e.mesg.Name);
            MonitoringMesg myMonitoringMessage = (MonitoringMesg)e.mesg;
            try
            {
                Console.WriteLine("\tTimestamp  {0}", myMonitoringMessage.GetTimestamp());
                Console.WriteLine("\tActivityType {0}", myMonitoringMessage.GetActivityType());
                switch (myMonitoringMessage.GetActivityType()) // Cycles is a dynamic field
                {
                    case ActivityType.Walking:
                    case ActivityType.Running:
                        Console.WriteLine("\tSteps {0}", myMonitoringMessage.GetSteps());
                        break;
                    case ActivityType.Cycling:
                    case ActivityType.Swimming:
                        Console.WriteLine("\tStrokes {0}", myMonitoringMessage.GetStrokes());
                        break;
                    default:
                        Console.WriteLine("\tCycles {0}", myMonitoringMessage.GetCycles());
                        break;
                }
            }
            catch (FitException exception)
            {
                Console.WriteLine("\tOnDeviceInfoMesg Error {0}", exception.Message);
                Console.WriteLine("\t{0}", exception.InnerException);
            }
        }
        #endregion

        /// <summary>
        /// <para>Main Program call for GarminTo application</para>
        /// </summary>
        /// <param name="args">Args for the GarminTo application</param>
        public static void Main(string[] args)
        {
            // read FIT file test 
            fitSource = new FileStream(@"D:\temp\fit-files\20160101-090645-1-1328-ANTFS-4-0.FIT", FileMode.Open);
            var fitDecode = new FickleFrostbite.FIT.Decode();
            var mesgBroadcaster = new FickleFrostbite.FIT.MesgBroadcaster();
            fitDecode.MesgEvent += mesgBroadcaster.OnMesg;
            fitDecode.MesgDefinitionEvent += mesgBroadcaster.OnMesgDefinition;
            mesgBroadcaster.MesgEvent += new MesgEventHandler(OnMesg);
            mesgBroadcaster.MesgDefinitionEvent += new MesgDefinitionEventHandler(OnMesgDefn);
            mesgBroadcaster.FileIdMesgEvent += new MesgEventHandler(OnFileIDMesg);
            mesgBroadcaster.UserProfileMesgEvent += new MesgEventHandler(OnUserProfileMesg);
            bool status = fitDecode.IsFIT(fitSource);
            status &= fitDecode.CheckIntegrity(fitSource);
            // Process the file
            if (status == true)
            {
                Console.WriteLine("Decoding...");
                fitDecode.Read(fitSource);
                Console.WriteLine("Decoded FIT file.");
            }
            else
            {
                try
                {
                    Console.WriteLine("Integrity Check Failed {0}", args[0]);
                    Console.WriteLine("Attempting to decode...");
                    fitDecode.Read(fitSource);
                }
                catch (FitException ex)
                {
                    Console.WriteLine("DecodeDemo caught FitException: " + ex.Message);
                }
            }
            fitSource.Close();
            Console.WriteLine("");
            Console.WriteLine("Summary:");
            int totalMesgs = 0;
            foreach (KeyValuePair<ushort, int> pair in mesgCounts)
            {
                Console.WriteLine("MesgID {0,3} Count {1}", pair.Key, pair.Value);
                totalMesgs += pair.Value;
            }


            if (args.Length < 2)
            {
                Program.ShowUsageMessage();
            }
            else
            {
                /* process input file (either single file or directory) into list of FileInfo */
                var inputFiles = Program.GetInputFiles(args[0]);

                /* process export options */
                var exportOptions = args.ToList();
                exportOptions.RemoveAt(0);

                /* for each export option determine what type of option 
                 *              and then run the process over that option */
                foreach (var exportOption in exportOptions)
                {
                    var googleMapsMatch = googleMapsRegex.Match(exportOption);
                    var sqlMatch = sqlRegex.Match(exportOption);
                    var excel2013Match = excel2013Regex.Match(exportOption);

                    if (googleMapsMatch.Success) { Processor.ToGoogleMaps(inputFiles, googleMapsMatch); }

                    if (sqlMatch.Success) { Processor.ToSql(inputFiles, sqlMatch); }

                    if (excel2013Match.Success) { Processor.ToExcel2013(inputFiles, excel2013Match); }
                }
            }

            System.Console.WriteLine("Press any key to close ... ");
            System.Console.ReadKey();
        }

        /// <summary>
        /// <para>Display usage pattern to users</para>
        /// </summary>
        public static void ShowUsageMessage()
        {
            Console.WriteLine("GarminTo: Translates Garmin Files (TCX) to other formats.");
            Console.WriteLine();
            Console.WriteLine("GarminTo input [/SQL(SERVER:<ServerName>;DB:<DatabaseName>;UID:<SqlUser>;PWD:<SqlUserPassword>)] [/GoogleMaps(DIR:<directory>)] [/Excel2013(DIR:<directory>)]");
            Console.WriteLine();
            Console.WriteLine("input\tSpecifies the file or directory of files to be translated");
            Console.WriteLine("/SQL\tOutput to SQL with specified parameters");
            Console.WriteLine("/GoogleMaps\tOutput to Google Maps with specified parameters");
            Console.WriteLine("/Excel2013\tOutput to Excel 2013 with specified parameters");
        }

        /// <summary>
        /// <para>Process input parameter into input files</para>
        /// </summary>
        /// <param name="input">Input parameter (either a single file or a directory of multiple files)</param>
        /// <returns>
        /// <para>Returns a list of fileInfo objects for all the .tcx files in the input parameter</para>
        /// </returns>
        protected static List<FileInfo> GetInputFiles(string input)
        {
            var inputList = new List<FileInfo>();
            var singleInputFileRegex = new Regex(".tcx$", RegexOptions.IgnoreCase);

            if (singleInputFileRegex.Match(input).Success)
            {
                var singleFileInfo = new FileInfo(input);
                if (singleFileInfo.Exists)
                {
                    inputList.Add(singleFileInfo);
                }
                else
                {
                    Console.WriteLine("Unable to find file \"" + input + "\"");
                    Console.WriteLine();
                    Program.ShowUsageMessage();
                }
            }
            else
            {
                var directoryInfo = new DirectoryInfo(input);
                if (directoryInfo.Exists)
                {
                    inputList = directoryInfo.GetFiles("*.tcx", SearchOption.TopDirectoryOnly).ToList();
                }
                else
                {
                    Console.WriteLine("Unable to find directory \"" + input + "\"");
                    Console.WriteLine();
                    Program.ShowUsageMessage();
                }
            }

            return inputList;
        }
    }
}
