using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace FickleFrostbite.TCX
{
    /// <summary>
    /// <para>.Net representation of a TCX (Training Center XML) File</para>
    /// </summary>
    [Serializable]
    [XmlRootAttribute(Namespace = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public class TrainingCenterDatabase
    {
        /// <summary>
        /// <para>List of Activities in the TCX File</para>
        /// </summary>
        public List<Activity> Activities { get; set; }

        /// <summary>
        /// <para>Serialize the TCX file to a on disk file</para>
        /// </summary>
        /// <param name="FileName">FileName of the file to be serialized to</param>
        public void SerializeToFile(string FileName)
        {
            /* create serializer for this object type */
            XmlSerializer objXmlSerializer = new XmlSerializer(typeof(TrainingCenterDatabase));
            XmlSerializerNamespaces objXmlSerializerNS = new XmlSerializerNamespaces();
            objXmlSerializerNS.Add("", "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2");

            /* create file to serialize object to */ 
            FileInfo SerializedFile = new FileInfo(FileName);
            if (!(Directory.Exists(SerializedFile.DirectoryName))) Directory.CreateDirectory(SerializedFile.DirectoryName);

            /* serialize object to file */
            StreamWriter objStreamWriter = new StreamWriter(SerializedFile.FullName);
            objXmlSerializer.Serialize(objStreamWriter, this, objXmlSerializerNS);
            objStreamWriter.Close();
        }

        /// <summary>
        /// <para>Read a TCX file from disk into TrainingCenterDatabase object</para>
        /// </summary>
        /// <param name="FileName">Filename (full filename) to read file from</param>
        /// <returns>
        /// <para>TrainingCenterDatabase object read in from fileName file</para>
        /// </returns>
        public static TrainingCenterDatabase ReadFromFile(string FileName)
        {
            /* deserialize object from file */
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TrainingCenterDatabase));
            StreamReader XMLFileStreamReader = new StreamReader(FileName);
            TrainingCenterDatabase trainingCenterDatabase = (TrainingCenterDatabase)xmlSerializer.Deserialize(XMLFileStreamReader);
            XMLFileStreamReader.Close();

            return trainingCenterDatabase;
        }
    }
}
