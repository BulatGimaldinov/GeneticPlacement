using System;
using System.IO;
using System.Windows;

namespace GeneticPlacement
{
    public class FileUtils
    {
        public static int[][] ReadFiles(string inputPath)
        {
            try
            {
                var sr = new StreamReader(inputPath, System.Text.Encoding.Default);
                var n = 0; // кол-во строк
                var m = 0; // кол-во элементов в строке
                while (sr.ReadLine() != null)
                {
                    n++;
                    if (n == 1) m = sr.ReadLine()!.Split(' ').Length;
                }
                n += 1;
                var matrixRead = new int[n][];
                for (int i = 0; i < n; i++)
                {
                    matrixRead[i] = new int[m];
                }
                var arrayFile = File.ReadAllLines(inputPath, System.Text.Encoding.Default);
                for (var i = 0; i < n; i++)
                {
                    for (var j = 0; j < m; j++)
                    {
                        var line = arrayFile[i].Split(' ');
                        var symbol = int.Parse(line[j]);
                        matrixRead[i][j] = symbol;
                    }
                }
                return matrixRead;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex);
            }
            return Array.Empty<int[]>();
        }
    }
}