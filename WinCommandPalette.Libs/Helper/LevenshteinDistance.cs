using System;

namespace WinCommandPalette.Libs.Helper
{
    // https://www.dotnetperls.com/levenshtein - 02.07.2017
    public static class LevenshteinDistance
    {
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string search, string target)
        {
            var searchLength = search.Length;
            var targetLength = target.Length;
            var difArray = new int[searchLength + 1, targetLength + 1];

            // Step 1
            if (searchLength == 0)
            {
                return targetLength;
            }

            if (targetLength == 0)
            {
                return searchLength;
            }

            // Step 2
            for (var i = 0; i <= searchLength; difArray[i, 0] = i++)
            {
            }

            for (var j = 0; j <= targetLength; difArray[0, j] = j++)
            {
            }

            // Step 3
            for (var i = 1; i <= searchLength; i++)
            {
                //Step 4
                for (var j = 1; j <= targetLength; j++)
                {
                    // Step 5
                    var cost = (target[j - 1] == search[i - 1]) ? 0 : 1;

                    // Step 6
                    difArray[i, j] = Math.Min(
                        Math.Min(difArray[i - 1, j] + 1, difArray[i, j - 1] + 1),
                        difArray[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return difArray[searchLength, targetLength];
        }
    }
}
