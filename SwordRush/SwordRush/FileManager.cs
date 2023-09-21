using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SwordRush
{
    internal class FileManager
    {
        public static string LoadFile(string path)
        {
            return File.ReadAllText(path);
        }

        public static void SaveFile(string path)
        {
            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine(path);
            writer.Close();
        }

        public static void SaveStats(string path, string info)
        {
            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine(info);
            writer.Close();
        }

        public string[] LoadStats(string path)
        {
            StreamReader reader = new StreamReader(path);
            string[] info = reader.ReadLine().Split(',');
            reader.Close();
            return info;
        }

        /// <summary>
        /// creates a new grid based on the data read from a file
        /// </summary>
        /// <param name="path">the file path of the file being used to load the data</param>
        /// <returns>a 2D int array that contains the grid data</returns>
        public int[,] LoadGrid(string path)
        {
            StreamReader reader = new StreamReader(path);
            int[,] grid = new int[20,12];

            for (int i = 0; i < 12; i++)
            {
                string[] row = reader.ReadLine().Split(',');
                for (int j = 0; j < 20; j++)
                {
                    grid[j,i] = int.Parse(row[j]);
                }
            }

            reader.Close();
            return grid;
        }


        public int[,] LoadWallTiles(string path)
        {
            StreamReader reader = new StreamReader(path);
            int[,] grid = new int[20, 12];

            for (int i = 0; i < 13; i++)
            {
                reader.ReadLine();
            }

            for (int i = 0; i < 12; i++)
            {
                string[] row = reader.ReadLine().Split(',');
                for (int j = 0; j < 20; j++)
                {
                    grid[j, i] = int.Parse(row[j]);
                }
            }
            
            reader.Close();
            return grid;

        }
    }
}
