using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace angelica_puzzle
{
    class Dijkstra
    {
        HashSet<string> visited;
        Dictionary<string, int> distance;
        SortedSet<Tuple<Int64, State>> pq;
        IGameWindow emitter;
    
        public static void Calculate(State node, IGameWindow emitter)
        {
            new Dijkstra(emitter).Start(node);
        }

        public Dijkstra(IGameWindow emitter = null)
        {
            this.visited = new HashSet<string>();
            this.distance = new Dictionary<string, int>();
            this.pq = new SortedSet<Tuple<long, State>>();

            if (emitter != null)
                this.emitter = emitter;
        }

        public void Start(State node)
        {
            pq.Add(new Tuple<long, State>(0, node));
            visited.Add(node.GetPattern());
            distance[node.GetPattern()] = 0;

            int[] dx = new int[] { -node.GetColumnsCount(), 1, node.GetColumnsCount(), -1 };

            while (pq.Count > 0)
            {
                Tuple<long, State> top = pq.Max;
                pq.Remove(top);

                if (top.Item2.Finished())
                {
                    Console.WriteLine("We have found a solution ");
                    if (this.emitter != null)
                    {
                        Stack<State> st = top.Item2.GetTrack();
                        Console.WriteLine(st.Count);
                        this.emitter.Update(st);
                    }
                    else
                    {
                        top.Item2.PrintTrack();
                    }
                    break;
                }

                for (int i = 0; i < 4; i++)
                {
                    int j = top.Item2.GetPattern().IndexOf('.');
                    if (j + dx[i] >= 0 && j + dx[i] < top.Item2.GetPattern().Length)
                    {
                        Tuple<long, State> xx = new Tuple<long, State>(top.Item1 + top.Item2.Weight(), top.Item2.MakeMove(dx[i]));
                        if (!visited.Contains(xx.Item2.GetPattern()))
                        {
                            visited.Add(xx.Item2.GetPattern());
                            distance[xx.Item2.GetPattern()] = distance[top.Item2.GetPattern()] + 1;
                            pq.Add(xx);
                        }
                    }
                }
            }
        }

        string replace(string source, int x, int y)
        {
            y = x + y;
            char[] vs = source.ToCharArray();
            char tmp = vs[x];
            vs[x] = vs[y];
            vs[y] = tmp;
            return new string(vs);
        }
    }
}
