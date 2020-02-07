using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymnasiearbete.UI
{
    class Progres
    {
        private string Name { get; }
        private string Format { get; }
        private string LastPrint { get; set; }

        public Progres() : this(null) { }

        public Progres(string name) : this(name, "{0:0.00}") { }

        public Progres(string name, string format)
        {
            Name = name ?? "";
            Format = format;
        }

        /// <summary>
        /// Prints the progress to the console if it has changed enough to be displayed differently.
        /// </summary>
        /// <param name="progress">The progress as a double between 0 and 1.</param>
        public void Update(double progress)
        {
            string str = string.Format(Format, progress * 100);

            if (str != LastPrint)
            {
                string name = string.IsNullOrWhiteSpace(Name) ? "" : $"{Name}:";
                Console.WriteLine($"{Name}{str}%");
                LastPrint = str;
            }
        }
    }
}
