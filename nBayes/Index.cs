namespace nBayes
{
    public abstract class Index
    {
        public abstract int EntryCount { get; }


        public virtual void Add(params Entry[] documents)
        {
            foreach (Entry t in documents)
            {
                Add(t);
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
