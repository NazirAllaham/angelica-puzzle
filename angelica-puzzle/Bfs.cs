using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace angelica_puzzle
{
    class Bfs
    {
        HashSet<string> visited;
        Dictionary<string, int> distance;
        Queue<State> q;
        IGameWindow emitter;

        public static void Calculate(State node, IGameWindow emitter)
        {
            new Bfs() { emitter = emitter}.Start(node);
        }

        public Bfs()
        {
            this.visited = new HashSet<string>();
            this.distance = new Dictionary<string, int>();
            this.q = new Queue<State>();
        }

        public void Start(State node)
        {
            q.Enqueue(node);
            visited.Add(node.GetPattern());
            distance[node.GetPattern()] = 0;

            int[] dx = new int[] { -node.GetColumnsCount(), 1, node.GetColumnsCount(), -1 };

            while (q.Count > 0)
            {
                State top = q.Dequeue();

                if(top.Finished())
                {
                    Console.WriteLine("We have found a solution ");
                    if (this.emitter != null)
                    {
                        Stack<State> st = top.GetTrack();
                        Console.WriteLine(st.Count);
                        this.emitter.Update(st);
                    }
                    else
                    {
                        top.PrintTrack();
                    }
                    break;
                }

                for (int i = 0; i < 4; i++)
                {
                    int j = top.GetPattern().IndexOf('.');
                    if(j + dx[i] >= 0 && j + dx[i] < top.GetPattern().Length)
                    {
                        State xx = top.MakeMove(dx[i]);
                        if(!visited.Contains(xx.GetPattern()))
                        {
                            visited.Add(xx.GetPattern());
                            distance[xx.GetPattern()] = distance[top.GetPattern()] + 1;
                            q.Enqueue(xx);
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
