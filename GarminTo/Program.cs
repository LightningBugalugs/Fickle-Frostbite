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

        /// <summary>
        /// <para>Main Program call for GarminTo application</para>
        /// </summary>
        /// <param name="args">Args for the GarminTo application</param>
        public static void Main(string[] args)
        {
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
