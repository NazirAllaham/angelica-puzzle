using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace angelica_puzzle
{
    class Dfs
    {
        public static int Calculate(State currentState, State finalState, IGameWindow emitter)
        {
            Console.WriteLine("Calculate: " + currentState.GetPattern().IndexOf('.'));
            return new Dfs(currentState, finalState, emitter).Start(currentState.GetPattern().IndexOf('.'), currentState);
        }

        private State final;
        private int[] dx;

        HashSet<string> visited = new HashSet<string>();
        Stack<Tuple<State, int>> states = new Stack<Tuple<State, int>>();
        private IGameWindow emitter;

        private Dfs(State current, State final, IGameWindow emitter = null)
        {
            this.final = final;
            this.dx = new int[] {-current.GetColumnsCount(), 1, current.GetColumnsCount(), -1};

            if (emitter != null)
                this.emitter = emitter;
        }


        private int Start(int index, State current)
        {
            ///current.Print();

            states.Push(new Tuple<State, int>(current, 0));

            while(states.Count > 0)
            {
                Tuple<State, int> top = states.Pop();

                if (top.Item1.Finished())
                {
                    Console.WriteLine("We have found a solution ");
                    if (this.emitter != null)
                    {
                        Stack<State> st = top.Item1.GetTrack();
                        Console.WriteLine(st.Count);
                        this.emitter.Update(st);
                    }
                    else
                    {
                        top.Item1.PrintTrack();
                    }
                    break;
                }

                if (visited.Contains(top.Item1.GetPattern()))
                {
                    continue;
                }

                visited.Add(top.Item1.GetPattern());

                for (int i=0; i<dx.Length; i++)
                {
                    State nState = top.Item1.MakeMove(dx[i]);

                    if(nState != null && !visited.Contains(nState.GetPattern()))
                    {
                        states.Push(new Tuple<State, int>(nState, top.Item2 + 1));
                    }
                }
            }

            return -1;
        }
    }
}
