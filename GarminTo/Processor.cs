using FickleFrostbite.GoogleMap;
using FickleFrostbite.TCX;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GarminTo
{
    /// <summary>
    /// <para>Processes inputFiles into appropriate output format</para>
    /// </summary>
    public class Processor
    {
        #region Garmin to Google Maps
        /// <summary>
        /// <para>Processes inputFiles into Google Maps</para>
        /// </summary>
        /// <param name="inputFiles">List of inputFiles to process</param>
        /// <param name="googleMapsMatch">Export parameters to process</param>
        public static void ToGoogleMaps(List<FileInfo> inputFiles, Match googleMapsMatch)
        {
            System.Console.WriteLine("Starting GarminToGoogleMaps Process ...");

            var exportDirectoryPath = googleMapsMatch.Groups["googleMapsExportDirectory"].Value;
            var exportDirectory = new DirectoryInfo(exportDirectoryPath);
            if (!exportDirectory.Exists)
            {
                Console.WriteLine("Failed to Export to Google Maps: Unable to find Directory \"" + exportDirectoryPath + "\"");
                Console.WriteLine();
                GarminTo.Program.ShowUsageMessage();
            }
            else
            {
                Console.WriteLine("Exporting to Google Maps ... ");

                var fileIndex = 0;
                foreach (var inputFile in inputFiles)
                {
                    fileIndex++;
                    Console.WriteLine("Processing " + fileIndex + " of " + inputFiles.Count + " file(s)");

                    /* read in training centre database from file provide as arg 0 */
                    var trainingCenterDatabase = TrainingCenterDatabase.ReadFromFile(inputFile.FullName);

                    var activityIndex = 0;
                    foreach (var activity in trainingCenterDatabase.Activities)
                    {
                        activityIndex++;
                        Console.WriteLine("   Processing " + activityIndex + " of " + trainingCenterDatabase.Activities.Count + " activities");

                        /* create map points (google) for creation of google maps */
                        var mapPoints = activity.ToMapPoints();

                        /* create google map file */
                        if (mapPoints.Count == 0)
                        {
                            Console.WriteLine("             " + activityIndex + " contains no GPS data, no Google map created.");
                        }
                        else
                        {
                            var outputFilePath = exportDirectory.FullName + "\\" + activity.Id.Replace(":", "-") + ".htm";
                            var outputFile = new MapFile(outputFilePath);
                            outputFile.GenerateMap(mapPoints);
                        }
                    }
                }

                Console.WriteLine("Completed Exporting to Google Maps.");
            }

            Console.WriteLine();
        }
        #endregion

        #region Garmin to Sql
        /// <summary>
        /// <para>Processes inputFiles into Sql Tables</para>
        /// </summary>
        /// <param name="inputFiles">List of inputFiles to process</param>
        /// <param name="sqlMatch">Export parameters to process</param>
        public static void ToSql(List<FileInfo> inputFiles, Match sqlMatch)
        {
            /* read in parameters from export options */
            var serverName = sqlMatch.Groups["serverName"].Value;
            var databaseName = sqlMatch.Groups["databaseName"].Value;
            var username = sqlMatch.Groups["username"].Value;
            var password = sqlMatch.Groups["password"].Value;

            System.Console.WriteLine("Starting GarminToSQL Process ...");

            /* confirm access level to sql server (creation of database and table) */
            var hasPermissions = Processor.GarminToSqlConfirmPermissions(serverName, username, password, Properties.Resources.GarminToSQL_ConfirmPermissions);
            if (!hasPermissions)
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Username " + username + " does not have sufficent permissions on SQL Server " + serverName);
            }
            else
            {
                /* do database creation */
                var sqlDatabaseCreationConnectionString = "Server=" + serverName + ";UID=" + username + ";PWD=" + password + ";";
                Processor.GarminToSqlRunSqlNonQuery(sqlDatabaseCreationConnectionString, databaseName, Properties.Resources.GarminToSQL_CreateDatabase);

                /* do table creation */
                var sqlTableCreationConnectionString = "Server=" + serverName + ";Database=" + databaseName + ";UID=" + username + ";PWD=" + password + ";";
                Processor.GarminToSqlRunSqlNonQuery(sqlTableCreationConnectionString, databaseName, Properties.Resources.GarminToSQL_CreateTables);

                var fileIndex = 0;
                foreach (var inputFile in inputFiles)
                {
                    fileIndex++;
                    Console.WriteLine("Processing " + fileIndex + " of " + inputFiles.Count + " file(s)");

                    /* read in training centre database from file provide as arg 0 */
                    var trainingCenterDatabase = FickleFrostbite.TCX.TrainingCenterDatabase.ReadFromFile(inputFile.FullName);

                    /* transfer data from trainingCenterDatabase to sql database */
                    var garminToSqlConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["GarminToSqlEntities"].ConnectionString.Replace("{ServerName}", serverName).Replace("{DatabaseName}", databaseName).Replace("{Username}", username).Replace("{Password}", password);
                    var activityIndex = 0;
                    foreach (var activity in trainingCenterDatabase.Activities)
                    {
                        activityIndex++;
                        Console.WriteLine("   Processing " + activity + " of " + trainingCenterDatabase.Activities.Count + " activities");
                        Processor.GarminToSqlProcessActivity(activity, new FickleFrostbite.SQL.GarminToSqlEntities(garminToSqlConnectionString));   
                    }                    
                }

                System.Console.WriteLine();
                System.Console.WriteLine("GarminToSQL Process Completed.");
            }
        }

        /// <summary>
        /// <para>Process the Activity items into Sql Tables</para>
        /// </summary>
        /// <param name="processActivity">The activity to be processed</param>
        /// <param name="garminToSqlEntities">The entity framework to process activities into</param>
        protected static void GarminToSqlProcessActivity(FickleFrostbite.TCX.Activity processActivity, FickleFrostbite.SQL.GarminToSqlEntities garminToSqlEntities)
        {
            /* check if there is a duplicate to be processed */
            var isDuplicate = (from a in garminToSqlEntities.Activities
                               where a.TcxActivityId == processActivity.Id
                               select a).Any();
            if (isDuplicate)
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Activity " + processActivity.Id + " is a duplicate entry.");
            }
            else
            {
                /* process activity into SQL */
                System.Console.Write("   Processing " + processActivity.RowCount + " trackpoints: ");

                var processedItemCount = 0;
                var writeActivity = new FickleFrostbite.SQL.Activity();
                writeActivity.Sport = processActivity.Sport;
                writeActivity.TcxActivityId = processActivity.Id;
                garminToSqlEntities.Set<FickleFrostbite.SQL.Activity>().Add(writeActivity);
                foreach (var processLap in processActivity.Laps)
                {
                    var writeLap = new FickleFrostbite.SQL.Lap();
                    if (processLap.AverageHeartRateBpm != null) { writeLap.AverageHeartRateBpm = processLap.AverageHeartRateBpm.Value; }
                    writeLap.Calories = processLap.Calories;
                    writeLap.DistanceMeters = processLap.DistanceMeters;
                    if (processLap.MaximumHeartRateBpm != null) { writeLap.MaximumHeartRateBpm = processLap.MaximumHeartRateBpm.Value; }
                    writeLap.StartTime = processLap.StartTime;
                    writeLap.TotalTimeSeconds = processLap.TotalTimeSeconds;
                    writeLap.Activity = writeActivity;
                    garminToSqlEntities.Set<FickleFrostbite.SQL.Lap>().Add(writeLap);
                    foreach (var processTrackpoint in processLap.Track)
                    {
                        var writeTrackpoint = new FickleFrostbite.SQL.Trackpoint();
                        writeTrackpoint.AltitudeMeters = processTrackpoint.AltitudeMeters;
                        writeTrackpoint.DistanceMeters = processTrackpoint.DistanceMeters;
                        if (processTrackpoint.HeartRateBpm != null) { writeTrackpoint.HeartRateBpm = processTrackpoint.HeartRateBpm.Value; }
                        if (processTrackpoint.Position != null) { writeTrackpoint.LatitudeDegrees = processTrackpoint.Position.LatitudeDegrees; }
                        if (processTrackpoint.Position != null) { writeTrackpoint.LongitudeDegrees = processTrackpoint.Position.LongitudeDegrees; }
                        writeTrackpoint.Time = processTrackpoint.Time;
                        writeTrackpoint.Lap = writeLap;
                        garminToSqlEntities.Set<FickleFrostbite.SQL.Trackpoint>().Add(writeTrackpoint);
                        processedItemCount++;
                        if (processedItemCount % 500 == 0) { System.Console.Write(processedItemCount + ".."); }
                    }
                }
                garminToSqlEntities.SaveChanges();
                System.Console.WriteLine();
            }
        }

        /// <summary>
        /// <para>Confirms provided details have high enough permission level to process into SQL</para>
        /// </summary>
        /// <param name="serverName">The name of the server</param>
        /// <param name="username">The username to connect with</param>
        /// <param name="password">The password to connect with</param>
        /// <param name="sqlText">The sql to run to confirm</param>
        /// <returns>
        /// <para>True - credentials have required permissions</para>
        /// <para>False - credentails invalid or do not have enough permissions</para>
        /// </returns>
        protected static bool GarminToSqlConfirmPermissions(string serverName, string username, string password, string sqlText)
        {
            var sqlConnectionString = "Server=" + serverName + ";UID=" + username + ";PWD=" + password + ";";
            var sqlConnection = new SqlConnection(sqlConnectionString);
            sqlText = sqlText.Replace("{username}", username);
            var sqlCommand = new SqlCommand(sqlText, sqlConnection);
            sqlConnection.Open();
            var count = (int)sqlCommand.ExecuteScalar();
            sqlConnection.Close();

            var hasPermissions = count > 0;
            return hasPermissions;
        }

        /// <summary>
        /// <para>Run Sql query to process Garmin files</para>
        /// </summary>
        /// <param name="connectionString">Connection string for the query</param>
        /// <param name="databaseName">Database name to run the query against</param>
        /// <param name="sqlText">Query to run against the database</param>
        protected static void GarminToSqlRunSqlNonQuery(string connectionString, string databaseName, string sqlText)
        {
            var sqlConnection = new SqlConnection(connectionString);
            var sqlCommandFullText = sqlText.Replace("{DatabaseName}", databaseName);
            var sqlCommandTextSplit = Regex.Split(sqlCommandFullText, "GO", RegexOptions.Multiline);
            var sqlCommand = new SqlCommand();
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();
            foreach (var sqlCommandText in sqlCommandTextSplit)
            {
                sqlCommand.CommandText = sqlCommandText;
                sqlCommand.ExecuteNonQuery();
            }
            sqlConnection.Close();
        }
        #endregion

        #region Garmin to Excel 2013
        /// <summary>
        /// <para>Process inputFiles into Excel 2013</para>
        /// </summary>
        /// <param name="inputFiles">List of inputFiles to process</param>
        /// <param name="excel2013Match">Export parameters to process</param>
        public static void ToExcel2013(List<FileInfo> inputFiles, Match excel2013Match)
        {
            System.Console.WriteLine("Starting GarminToExcel2013 Process ...");

            /* validate export values */            
            var exportDirectoryPath = excel2013Match.Groups["excel2013ExportDirectory"].Value;
            var exportDirectory = new DirectoryInfo(exportDirectoryPath);
            if (!exportDirectory.Exists)
            {
                Console.WriteLine("Failed to Export to Excel 2013: Unable to find Directory \"" + exportDirectoryPath + "\"");
                Console.WriteLine();
                GarminTo.Program.ShowUsageMessage();
            }
            else
            {
                Console.WriteLine("Exporting to Excel 2013 ... ");

                var fileIndex = 0;
                foreach (var inputFile in inputFiles)
                {
                    fileIndex++;
                    Console.WriteLine("Processing " + fileIndex + " of " + inputFiles.Count + " file(s)");

                    /* read in training centre database from file provide as arg 0 */
                    var trainingCenterDatabase = TrainingCenterDatabase.ReadFromFile(inputFile.FullName);
                                        
                    var rowIndex = 0;
                    var activityIndex = 0;
                    foreach (var activity in trainingCenterDatabase.Activities)
                    {
                        activityIndex++;
                        Console.WriteLine("   Processing " + activity + " of " + trainingCenterDatabase.Activities.Count + " activities");
                        var excelWorkbokFilePath = exportDirectory.FullName + "\\" + activity.Id.Replace(":","-") + ".xlsx";
                        // start Excel and get Application object.
                        var excelApplication = new Microsoft.Office.Interop.Excel.Application { Visible = false, UserControl = false };
                        Microsoft.Office.Interop.Excel.Workbook excelWorkbook = excelApplication.Workbooks.Add();

                        try
                        {
                            /* get the relevant Workbook Sheet */
                            Microsoft.Office.Interop.Excel.Worksheet excelSheet = excelWorkbook.Worksheets.Add();

                            /* create data to be written to excel
                             *      http://www.clear-lines.com/blog/post/Write-data-to-an-Excel-worksheet-with-C-fast.aspx */
                            var data = new string[activity.RowCount + 1, 8];
                            data[rowIndex, 0] = "ID";
                            data[rowIndex, 1] = "Sport";
                            data[rowIndex, 2] = "Time";
                            data[rowIndex, 3] = "LatitudeDegrees";
                            data[rowIndex, 4] = "LongitudeDegrees";
                            data[rowIndex, 5] = "AltitudeMeters";
                            data[rowIndex, 6] = "DistanceMeters";
                            data[rowIndex, 7] = "HeartRateBpm";
                            rowIndex++;

                            /* for each lap read all the trackpoints from that lap and create a data row */
                            System.Console.Write("   Processing " + activity.RowCount + " trackpoints: ");
                            foreach (var lap in activity.Laps)
	                        {
                                foreach (var trackpoint in lap.Track)
                                {
                                    data[rowIndex, 0] = rowIndex.ToString();
                                    data[rowIndex, 1] = activity.Sport;
                                    data[rowIndex, 2] = trackpoint.Time != null ? trackpoint.Time.ToString() : "";
                                    data[rowIndex, 3] = trackpoint.Position != null ? trackpoint.Position.LatitudeDegrees.ToString() : "";
                                    data[rowIndex, 4] = trackpoint.Position != null ? trackpoint.Position.LongitudeDegrees.ToString() : "";
                                    data[rowIndex, 5] = trackpoint.AltitudeMeters != null ? trackpoint.AltitudeMeters.ToString() : "";
                                    data[rowIndex, 6] = trackpoint.AltitudeMeters != null ? trackpoint.AltitudeMeters.ToString() : "";
                                    data[rowIndex, 7] = trackpoint.HeartRateBpm != null ? trackpoint.HeartRateBpm.Value.ToString() : "";
                                    rowIndex++;
                                    if (rowIndex % 500 == 0) { System.Console.Write(rowIndex + ".."); }
                                }                                
	                        }

                            /* write the data set as a range to the workbook */
                            var startCell = (Microsoft.Office.Interop.Excel.Range)excelSheet.Cells[1, 1];
                            var endCell = (Microsoft.Office.Interop.Excel.Range)excelSheet.Cells[rowIndex, 8];
                            var writeRange = excelSheet.Range[startCell, endCell];
                            writeRange.Value2 = data;                           

                            /* save the excel work book */
                            excelWorkbook.SaveAs(excelWorkbokFilePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                            System.Console.WriteLine();
                        }
                        catch (Exception exception)
                        {
                            throw exception;
                        }
                        finally
                        {
                            excelWorkbook.Close();
                            excelApplication.Quit();
                        }
                    }
                }

                Console.WriteLine("Completed Exporting to Excel 2013.");
            }

            Console.WriteLine();
        }
        #endregion
    }
}
