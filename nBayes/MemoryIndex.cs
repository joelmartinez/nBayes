using System.Linq;

namespace nBayes
{
    internal class MemoryIndex : Index
    {
        internal IndexTable<string, int> Table = new IndexTable<string, int>();

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
            return Table.ContainsKey(token) ? Table[token] : 0;
        }

        
    }
}
