namespace nBayes
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Index
    {
        public Index()
        {
        }

        public abstract int EntryCount { get; }


        public virtual void Add(params Entry[] documents)
        {
            foreach (Entry t in documents)
            {
                this.Add(t);
            }
        }

        public abstract void Add(Entry document);
        public abstract int GetTokenCount(string token);

        public static Index CreateMemoryIndex()
        {
            return new MemoryIndex();
        }
    }
}
