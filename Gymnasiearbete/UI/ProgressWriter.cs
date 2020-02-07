using System;

namespace Gymnasiearbete.UI
{
    class ProgressWriter
    {
        private DateTime StartTime { get; set; }
        private string LastProgressStr { get; set; }

        /// <summary>
        /// Prints the progress to the console if it has changed enough to be displayed differently.
        /// </summary>
        /// <param name="progress">The progress as a double-precision floating-point number between 0 and 1.</param>
        public void Update(double progress)
        {
            // Set start time if not set
            if (StartTime == DateTime.MinValue)
                StartTime = DateTime.UtcNow;

            // progress to write
            string progressStr = string.Format("{0:000.00}", progress * 100);

            // Write progress if it has changed enough
            if (progressStr != LastProgressStr)
            {
                // elapsed time
                var timeDiff = DateTime.UtcNow - StartTime;

                // time left
                double secondsPerProgress = timeDiff.TotalSeconds / progress;
                var timeLeft = TimeSpan.FromSeconds((1 - progress) * secondsPerProgress);

                // write
                Console.WriteLine($"Progress: {progressStr}%   Elapsed time: {timeDiff.ToString("hh':'mm':'ss")}   Estimated remaining time: {timeLeft.ToString("hh':'mm':'ss")}");
                LastProgressStr = progressStr;
            }
        }
    }
}
