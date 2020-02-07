using System;

namespace Gymnasiearbete.UI
{
    class ProgressWriter
    {
        private string Name { get; }
        private string Format { get; }
        private string LastPrint { get; set; }

        public ProgressWriter() : this(null) { }

        /// <param name="name">Progress name.</param>
        public ProgressWriter(string name) : this(name, "{0:0.00}") { }

        /// <param name="name">Progress name.</param>
        /// <param name="format">Format for displaying progress percentage.</param>
        public ProgressWriter(string name, string format)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "" : $"{name}: ";
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
                Console.WriteLine($"{Name}{str}%");
                LastPrint = str;
            }
        }
    }
}
