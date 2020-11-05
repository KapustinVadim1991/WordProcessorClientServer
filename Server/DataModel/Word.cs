using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WordProcessor.DataModel
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
