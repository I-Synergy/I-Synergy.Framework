using ISynergy.Framework.Mathematics.Base;
using System;
using System.Collections.Generic;
using System.IO;

namespace ISynergy.Framework.Mathematics.Helpers
{
    /// <summary>
    /// Class IOHelper.
    /// </summary>
    public class IOHelper
    {
        /// <summary>
        /// Method for read fole
        /// </summary>
        /// <param name="source">sourse for read</param>
        /// <param name="matrix">out param readed matrix</param>
        /// <param name="vectorB">out param readed vector B</param>
        public static void ReadFile(string source, out IBaseMatrix matrix,
            out double[] vectorB)
        {
            var count = 0;

            var allElement = new List<string>();

            //Counting values in alternating rows
            using (var fs = new StreamReader(source))
            {
                string line;
                while ((line = fs.ReadLine()) != null)
                {
                    allElement.Add(line);
                    count++;
                }
            }

            //Creation of the initial matrix
            matrix = new UMatrix(count);
            vectorB = new double[count];

            //Entering elements from a file
            for (var i = 0; i < allElement.Count; i++)
            {
                var part = allElement[i].Split(' ');
                for (var j = 0; j < part.Length - 1; j++)
                {
                    matrix[i, j] = Double.Parse(part[j]);
                }
                vectorB[i] = Convert.ToDouble(part[part.Length - 1]);
            }
        }

        /// <summary>
        /// Method for read value from console
        /// </summary>
        /// <param name="matrix">out param readed matrix</param>
        /// <param name="vectorB">out param readed vector B</param>
        public static void ReadConsole(out IBaseMatrix matrix, out double[] vectorB)
        {
            var count = 0;
            var fl = false;

            while (!fl)
            {
                Console.Clear();
                Console.WriteLine("Input rows and colns of matrix");
                count = int.Parse(Console.ReadLine());
                if (count > 0 && count <= 50000)
                {
                    fl = true;
                }
                else
                {
                    Console.WriteLine("Please write correct value. More than 0 and less than 50000");
                    Console.WriteLine("Press any key to retry");
                    Console.ReadKey();
                }
            }

            matrix = new UMatrix(count);
            vectorB = new double[count];

            var i = 0;

            while (i < count)
            {
                Console.WriteLine("Input " + (i + 1).ToString() + " rows");

                try
                {
                    var byfer = Console.ReadLine();
                    var parts = byfer.Split(' ');
                    for (var j = 0; j < parts.Length; j++)
                    {
                        matrix[i, j] = Double.Parse(parts[j]);
                    }
                    i++;
                }
                catch
                {
                    Console.WriteLine("Please reinput string.");
                }
            }

            Console.WriteLine("Input vector B");

            var byfers = Console.ReadLine();
            var part = byfers.Split(' ');

            for (var j = 0; j < part.Length; j++)
            {
                vectorB[j] = Convert.ToDouble(part[j]);
            }
        }
    }
}
