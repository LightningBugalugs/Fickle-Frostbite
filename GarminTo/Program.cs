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
            Console.WriteLine("OnMesg: Received Mesg with global ID#{0}, its name is {1}", e.mesg.Num, e.mesg.Name);

            for (byte i = 0; i < e.mesg.GetNumFields(); i++)
            {
                for (int j = 0; j < e.mesg.fields[i].GetNumValues(); j++)
                {
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
            fitSource = new FileStream(@"D:\temp\fit-files\20150919-092010-1-1328-ANTFS-4-0.FIT", FileMode.Open);
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
