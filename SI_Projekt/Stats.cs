using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Sylabizator
{
    public class DictionarySerializer<TKey, TValue>
    {
        protected string path = Stats.path;
        
        [XmlType(TypeName = "Item")]
        public class Item
        {
            [XmlAttribute("key")]
            public TKey Key;
            [XmlAttribute("value")]
            public TValue Value;
        }

        private XmlSerializer _serializer = new XmlSerializer(typeof(Item[]), new XmlRootAttribute("Dictionary"));

        public Dictionary<TKey, TValue> Deserialize(string filename)
        {
            if (!File.Exists(path + filename)) return new Dictionary<TKey, TValue>();
            using (FileStream stream = new FileStream(path+filename, FileMode.Open))
            using (XmlReader reader = XmlReader.Create(stream))
            {
                return ((Item[])_serializer.Deserialize(reader)).ToDictionary(p => p.Key, p => p.Value);
            }
        }

        public void Serialize(string filename, Dictionary<TKey, TValue> dictionary)
        {
            using (var writer = new StreamWriter(path+filename))
            {
                _serializer.Serialize(writer, dictionary.Select(p => new Item() { Key = p.Key, Value = p.Value }).ToArray());
            }
        }
    }

    public static class Stats
    {
        public static string path = "..\\..\\Serialization";
        public static string doublePath = "Double.xml";
        public static string triplePath = "Triple.xml";
        public static string firstPath = "First.xml";
        public static string adjacentPath = "Adjacent.xml";
        public static string lastPath = "Last.xml";
        public static string syllablesPath = "Syllables.xml";

        public static void SerializeListStringXml(string fileName, List<string> obj)
        {
            using (var fileStream = new System.IO.FileStream(path+fileName, System.IO.FileMode.Create))
            {
                var ser = new XmlSerializer(typeof(List<string>));
                ser.Serialize(fileStream, obj);
            }
        }

        public static List<string> DeserializeListStringFromXml(string xml)
        {
            if (!File.Exists(path + xml)) return new List<String>();
            List<string> result;
            var ser = new XmlSerializer(typeof(List<string>));
            using (XmlReader reader = XmlReader.Create(path+xml))
            {
                result = ((List<string>)ser.Deserialize(reader));
            }
            return result;
        }

    }


}
