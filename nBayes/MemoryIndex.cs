namespace nBayes
{
    using System.Linq;

    internal class MemoryIndex : Index
    {
        internal IndexTable<string, int> Table = new IndexTable<string, int>();

        public MemoryIndex()
        {
        }

        public override int EntryCount
        {
            get
            {
                return Table.Values.Sum();
            }
        }

        public override void Add(Entry document)
        {
            foreach (string token in document)
            {
                if (Table.ContainsKey(token))
                {
                    Table[token]++;
                }
                else
                {
                    Table.Add(token, 1);
                }
            }
        }

        public override int GetTokenCount(string token)
        {
            return this.Table.ContainsKey(token) ? this.Table[token] : 0;
        }
    }
}
