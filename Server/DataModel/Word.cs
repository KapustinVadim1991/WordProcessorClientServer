using System.ComponentModel.DataAnnotations;

namespace WordProcessorServer.DataModel
{
    public class Word
    {
        public Word() { }

        public Word(string text, int count)
        {
            Text = text;
            Count = count;
        }

        [Key, Required]
        [MaxLength(15), MinLength(3)]
        public string Text { get; set; }

        [Required]
        public int Count { get; set; }
    }
}
