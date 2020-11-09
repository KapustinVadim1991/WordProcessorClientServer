namespace WordProcessorServer.DataModel
{
    using System.Data.Entity;

    class DictionaryContext : DbContext
    {
        public DictionaryContext()
            : base("name = DictionaryContext")
        {
        }

        public DbSet<Word> Words { get; set; } 
    }
}