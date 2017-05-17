using System;
using System.Collections.Generic;

namespace Bigs.Sudoky.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            var matrix = new int[9, 9];

            for (int x = 0; x < 9; x++)
                for (int y = 0; y < 9; y++)
                {
                    var possibleValues = GetPossibleValues(x, y, matrix);
                    var newRandom = random.Next(possibleValues.Length - 1);
                    matrix[x, y] = possibleValues[newRandom];
                }


            Console.WriteLine("Hello World!");
        }

        static int[] GetPossibleValues(int x, int y, int[,] matrix)
        {
            var result = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int xIndex = 0; xIndex < 9; xIndex++)
                if (result.Contains(matrix[xIndex, y]))
                    result.Remove(matrix[xIndex, y]);

            for (int yIndex = 0; yIndex < 9; yIndex++)
                if (result.Contains(matrix[x, yIndex]))
                    result.Remove(matrix[x, yIndex]);

            var xStart = GetRangeStart(x);
            var yStart = GetRangeStart(y);

            for (int xIndex = xStart; xIndex <= xStart + 3; xIndex++)
                for (int yIndex = yStart; yIndex <= yStart + 3; yIndex++)
                    if (result.Contains(matrix[xIndex, yIndex]))
                        result.Remove(matrix[xIndex, yIndex]);

            return result.ToArray();
        }

        static int GetRangeStart(int position)
        {
            if (position > 5)
                return 6;

            if (position > 2)
                return 3;

            return 0;
        }
    }
}