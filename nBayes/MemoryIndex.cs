namespace nBayes
{
    using System.Linq;

    internal class MemoryIndex : Index
    {
        internal IndexTable<string, int> table = new IndexTable<string, int>();

        public MemoryIndex()
        {
        }

        public override int EntryCount
        {
            get
            {
                return table.Values.Sum();
            }
        }

        public override void Add(Entry document)
        {
            foreach (string token in document)
            {
                if (table.ContainsKey(token))
                {
                    table[token]++;
                }
                else
                {
                    table.Add(token, 1);
                }
            }
        }

        public override int GetTokenCount(string token)
        {
            return this.table.ContainsKey(token) ? this.table[token] : 0;
        }
    }
}
