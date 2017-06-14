using System;
using System.Linq;
using static System.Console;
using System.Collections.Generic;

namespace Bigs.Sudoky.Generator
{
    static class Program
    {
        static readonly Random _random = new Random();
        static void Main(string[] args)
        {
            var matrix = new int[9, 9];
            FillFirstSquare(matrix);

            matrix.Display();
            //for (var number = 1; number <= 9; number++)
            //    for (var square = 0; square < 9; square++)
            //    {
            //        var possiblePositions = GetPossiblePositions(number, square, matrix);
            //        //possiblePositions.Display();
            //        //WriteLine();
            //        var newRandom = random.Next(possiblePositions.Length - 1);
            //        var position = possiblePositions[newRandom];
            //        var column = (square % 3) * 3 + (position % 3);
            //        var row = ((int)Math.Floor((decimal)square / 3)) * 3 + (int)Math.Floor((decimal)position / 3);
            //        //WriteLine(position + " - " + (int)Math.Floor((decimal)position / 3) + "x" + (square % 3) * 3 + (position % 3));
            //        matrix[row, column] = number;
            //        DisplayMatrix(matrix);
            //        WriteLine();
            //    }

            //DisplayMatrix(matrix);

            ReadLine();
        }

        static int[] possible = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        static void FillFirstSquare(int[,] matrix)
        {
            var positions = new List<int>(possible);
            for (var number = 1; number <= 9; number++)
            {
                var newRandom = _random.Next(positions.Count - 1);
                var position = positions[newRandom];
                var row = (int)Math.Floor((decimal)position / 3);
                var column = position % 3;
                matrix[row, column] = number;
                positions.Remove(position);
            }
        }

        static void FillSecondSquare(int[,] matrix)
        {
            for (var number = 1; number <= 9; number++)
            {
                var possibleRows = GetAvailableRows(matrix, number);
                var positions = GetPositionsForRows(possibleRows);
                var newRandom = _random.Next(positions.Length - 1);
                var position = positions[newRandom];
                var row = (int)Math.Floor((decimal)position / 3);
                var column = position % 3;
                matrix[row, column] = number;
            }
        }

        static int[] GetRemainingPositions(int[,] matrix, int square)
        {
            var result = new List<int>();
            var rowOffset = (int)Math.Floor((decimal)square / 3) * 3;
            var columnOffset = square % 3;

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    if (matrix[rowOffset + x, columnOffset + y] == 0)
                        result.Add()
                }
            }

            return result.ToArray();
        }

        static int[] GetPositionsForRows(int[] rows)
        {
            var result = new List<int>();

            for (int row = 0; row < rows.Length; row++)
            {
                var start = (int)Math.Floor((decimal)row / 3);
                for (int i = 0; i < 3; i++)
                    result.Add(start + i);
            }

            return result.ToArray();
        }

        static int[] GetAvailableRows(int[,] matrix, int number)
        {
            var result = new List<int>();
            for (var x = 0; x < 3; x++)
            {
                var existsInRow = false;
                for (var y = 0; y < 3; y++)
                {
                    existsInRow |= matrix[x, y] == number;
                }
                if (!existsInRow)
                    result.Add(x);
            }
            return result.ToArray();
        }

        //static void DisplayMatrix(int[,] matrix)
        //{
        //    for (int x = 0; x < 9; x++)
        //    {
        //        Write("|");
        //        for (int y = 0; y < 9; y++)
        //            Write(matrix[x, y] + "|");
        //        WriteLine();
        //    }
        //}

        //static int[] GetPossiblePositions(int number, int square, int[,] matrix)
        //{
        //    var possible = PossiblePositions(square, matrix);
        //    var result = new List<int>();

        //    for (int position = 0; position < 9; position++)
        //    {
        //        var column = (square % 3) * 3 + (position % 3);
        //        var row = ((int)Math.Floor((decimal)square / 3)) * 3 + (int)Math.Floor((decimal)position / 3);
        //        if (ExistsInColumn(number, column, matrix)
        //                ||
        //                ExistsInRow(number, row, matrix))
        //            continue;
        //        else
        //            result.Add(position);
        //    }
        //    return result.Intersect(possible).ToArray();
        //}

        //static List<int> PossiblePositions(int square, int[,] matrix)
        //{
        //    var result = new List<int>();
        //    for (var position = 0; position < 9; position++)
        //    {
        //        var column = (square % 3) * 3 + (position % 3);
        //        var row = ((int)Math.Floor((decimal)square / 3)) * 3 + (int)Math.Floor((decimal)position / 3);
        //        if (matrix[row, column] == 0)
        //            result.Add(position);
        //    }

        //    return result;
        //}

        //static bool ExistsInColumn(int number, int column, int[,] matrix)
        //{
        //    for (int x = 0; x < 9; x++)
        //        if (matrix[x, column] == number)
        //            return true;
        //    return false;
        //}

        //static bool ExistsInRow(int number, int row, int[,] matrix)
        //{
        //    for (int y = 0; y < 9; y++)
        //        if (matrix[row, y] == number)
        //            return true;
        //    return false;
        //}

        //static int[] GetPossibleValues(int x, int y, int[,] matrix)
        //{
        //    var result = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        //    for (int xIndex = 0; xIndex < 9; xIndex++)
        //        if (result.Contains(matrix[xIndex, y]))
        //            result.Remove(matrix[xIndex, y]);

        //    for (int yIndex = 0; yIndex < 9; yIndex++)
        //        if (result.Contains(matrix[x, yIndex]))
        //            result.Remove(matrix[x, yIndex]);

        //    var xStart = GetRangeStart(x);
        //    var yStart = GetRangeStart(y);

        //    for (int xIndex = xStart; xIndex < xStart + 3; xIndex++)
        //        for (int yIndex = yStart; yIndex < yStart + 3; yIndex++)
        //            if (result.Contains(matrix[xIndex, yIndex]))
        //                result.Remove(matrix[xIndex, yIndex]);

        //    return result.ToArray();
        //}



        //static int GetRangeStart(int position)
        //{
        //    if (position > 5)
        //        return 6;

        //    if (position > 2)
        //        return 3;

        //    return 0;
        //}

        static void Display(this int[] array)
        {
            for (int i = 0; i < array.Length; i++)
                Write(array[i] + ",");
        }

        static void Display(this int[,] matrix)
        {
            for (int x = 0; x < 9; x++)
            {
                Write("|");
                for (int y = 0; y < 9; y++)
                    Write(matrix[x, y] + "|");
                WriteLine();
            }
        }
    }
}