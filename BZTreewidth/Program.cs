using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace BZTreewidth
{
    class Program
    {
        public static List<List<Vertex>> BagsList; // Global variabele that stores the bags used in the decomposition
        public static int TotalExpanded;
        public static Random random;

        static void Main(string[] args)
        {
            int rIndex = Array.IndexOf(args, "-s");
            try
            {
                random = new Random(int.Parse(args[rIndex + 1]));
            }
            catch
            {
                random = new Random(4321);
            }

            HandleCase();
        }

        static void HandleCase()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            //if (!file.EndsWith(".gr") && !file.EndsWith(".dgf") && !file.EndsWith(".dimacs") && !file.EndsWith(".col")) return; // Only process input (and not output) files

            // Parse the input graph
            Graph g = new Graph();
            for (string line = Console.ReadLine(); line != null; line = Console.ReadLine() )
            {
                if (line.StartsWith("c") || line.StartsWith("n")) continue; // Comment or dimacs node

                if (line.StartsWith("p")) // Initialization
                {
                    string[] cf = line.Split(' ');
                    
                    for (int i = 0; i < int.Parse(cf[2]); i++)
                        g.AddVertex(i);

                    continue;
                }

                // Dimacs-style edge
                if (line.StartsWith("e"))
                {
                    string[] vt = line.Split(' '); // Edge
                    g.AddVertex(int.Parse(vt[1]) - 1);
                    g.AddVertex(int.Parse(vt[2]) - 1);
                    g.AddEdge(g[int.Parse(vt[1]) - 1], g[int.Parse(vt[2]) - 1]);
                    continue;
                }

                // Something else, possibly an edge
                try
                {
                    string[] vt = line.Split(' '); // Edge
                    g.AddVertex(int.Parse(vt[0]) - 1);
                    g.AddVertex(int.Parse(vt[1]) - 1);
                    g.AddEdge(g[int.Parse(vt[0]) - 1], g[int.Parse(vt[1]) - 1]);
                }
                catch { }
            }

            if (g.vertices.Count == 0)
            {
                Console.WriteLine("s td 0 0 0");
                return;
            }

            // Run the algorithm
            BagsList = new List<List<Vertex>>();
            TotalExpanded = 0;

            Graph TD = g.Decompose();
            int treewidth = BagsList.Max((b) => b.Count);

            // Write the output decomposition
            Console.Write("s td {0} {1} {2}", TD.vertices.Count, treewidth, g.vertices.Count); //s-line

            // Output bag contents
            int bc = 1;
            foreach (Vertex v in TD.vertices.Keys)
            {
                Console.WriteLine();
                Console.Write("b " + bc);
                foreach (Vertex v2 in BagsList[v.Label])
                    Console.Write(" " + (v2.Label + 1));
                v.Label = bc++;
            }

            // Output edges
            foreach (Edge e in TD.edges)
            {
                Console.WriteLine();
                Console.Write((e.a.Label) + " " + (e.b.Label));
            }

            timer.Stop();
            //Console.Write("tw\t" + treewidth + "\texp\t" + TotalExpanded + "\ttime\t" + Math.Round(timer.ElapsedMilliseconds / 1000d, 2));
        }
    }

    static class Helpers
    {
        // http://www.dotnetperls.com/fisher-yates-shuffle
        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + Program.random.Next(0, n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        // http://stackoverflow.com/questions/273313/randomize-a-listt
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Program.random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
