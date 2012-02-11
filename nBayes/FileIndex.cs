namespace nBayes
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    public class FileIndex : Index
    {
        private MemoryIndex index = new MemoryIndex();
        private string filePath;

        public FileIndex(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            this.filePath = filePath;
        }

        public override int EntryCount
        {
            get { return this.index.EntryCount; }
        }

        /// <exception cref="InvalidOperationException">Occurs when the serializer has trouble
        /// deserializing the file on disk. Can occur if the file is corrupted.</exception>
        public void Open()
        {
            if (File.Exists(this.filePath))
            {
                using (Stream stream = File.OpenRead(this.filePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IndexTable<string, int>));
                    index.table = serializer.Deserialize(stream) as IndexTable<string, int>;
                }
            }
        }

        public override void Add(Entry document)
        {
            this.index.Add(document);
        }

        public void Save()
        {
            using (Stream stream = File.Open(this.filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(IndexTable<string, int>));
                serializer.Serialize(stream, index.table);
            }
        }

        public override int GetTokenCount(string token)
        {
            return this.index.GetTokenCount(token);
        }
    }
}
