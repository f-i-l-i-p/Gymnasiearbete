using System;
using System.Collections.Generic;
using System.Linq;

namespace Gymnasiearbete.Helpers
{
    static class RandomHelper
    {
        private static Random random = new Random();

        /// <summary>
        /// Returns a list of unique random numbers, greater than or equal to minValue and less than maxValue, in a random order.
        /// </summary>
        /// <param name="count">Amount of randoms numbers to return. count must be smaller than or equal to the amount off possible unique numbers.</param>
        /// <param name="minValue">The inclusive lower bound of the random numbers returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random numbers returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>List of unique 32-bit signed integers greater than or equal to minValue and less than maxValue.</returns>
        public static IList<int> UniqueRandoms(int count, int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException("minValue is greater than maxValue.", "maxValue");

            if (maxValue - minValue + 1 < count)
                throw new ArgumentOutOfRangeException($"{count} unique numbers does not exist from {minValue} to {maxValue}.", "count");

            var unused = Enumerable.Range(minValue, maxValue - minValue).ToList();
            var randoms = new int[count];

            for (int i = 0; i < count; i++)
            {
                int index = random.Next(unused.Count);

                randoms[i] = unused[index];
                unused.RemoveAt(index);
            }

            return randoms;
        }
    }
}
