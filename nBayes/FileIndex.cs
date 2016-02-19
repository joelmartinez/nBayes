using System;
using System.IO;
using System.Xml.Serialization;

namespace nBayes
{
    public class FileIndex : Index
    {
        private readonly MemoryIndex _index = new MemoryIndex();
        private readonly string _filePath;

        public FileIndex(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            _filePath = filePath;
        }

        public override int EntryCount
        {
            get { return _index.EntryCount; }
        }

        /// <exception cref="InvalidOperationException">Occurs when the serializer has trouble
        /// deserializing the file on disk. Can occur if the file is corrupted.</exception>
        public void Open()
        {
            if (File.Exists(_filePath))
            {
                using (Stream stream = File.OpenRead(_filePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IndexTable<string, int>));
                    _index.Table = serializer.Deserialize(stream) as IndexTable<string, int>;
                }
            }
        }

        public override void Add(Entry document)
        {
            _index.Add(document);
        }

        public void Save()
        {
            using (Stream stream = File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(IndexTable<string, int>));
                serializer.Serialize(stream, _index.Table);
            }
        }

        public override int GetTokenCount(string token)
        {
            return _index.GetTokenCount(token);
        }
    }
}
