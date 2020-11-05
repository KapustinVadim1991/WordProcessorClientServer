namespace WordProcessor.DataModel
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Configuration;

    class DictionaryContext : DbContext
    {
        public DictionaryContext()
            : base("name = DictionaryContext")
        {
        }

        public DbSet<Word> Words { get; set; } 
    }
}