using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace angelica_puzzle
{
    public class State : IComparable
    {
        private int rows;
        private int cols;
        private char[] pattern;
        private string final_pattern;
            
        private int blackCellndex;
        private State parent;

        public static int ToIndex(int row, int col, int rows, int cols, int dr = 0, int dc = 0)
        {
            if (!isValid(row, col, rows, cols))
                return -1;
            return (col + dc) + ((row + dr) * cols);
        }

        public static int Offset(int index, int rows, int cols, int dr, int dc)
        {
            int off = index + dc + (dr * cols);
            if (off < 0 || off >= rows * cols)
                return -1;

            return off;
        }

        public static bool isValid(int row, int col, int rows, int cols)
        {
            return (row >= 0 && row < rows && col >= 0 && col < cols);
        }

        
        public State Parent()
        {
            return this.parent;
        }

        public State(State parent, int rows, int cols, string pattern, string final_pattern = null, bool shuffle = false, bool swap = true)
        {
            this.rows = rows;
            this.cols = cols;
            this.pattern = (pattern == null) ? null : pattern.ToCharArray();
            this.final_pattern = final_pattern == null ? pattern : final_pattern;
            
            if (shuffle)
                this.Shuffle();
       
            if(swap)
            {
                Swap(Array.IndexOf(this.pattern, '.'), this.pattern.Length - 1);
                this.blackCellndex = this.pattern.Length - 1;
            }

            if(parent != null)
                this.parent = parent;
        }

        private void Shuffle()
        {
            Random rand = new Random();
            int n = this.pattern.Length;

            while(n > 0)
            {
                int k = rand.Next(n);
                n--;

                Swap(n, k);
            }
        }

        private void Swap(int i, int j)
        {
            char temp = pattern[i];
            pattern[i] = pattern[j];
            pattern[j] = temp;
        }

        public void Print()
        {
            int length = pattern.Length;
            int i = 0, j = 0, k = 0;

            while (i < rows && k < length)
            {
                Console.Write(pattern[k]);

                k++;
                j++;
                Console.Write(" ");
                if (j == cols)
                {
                    j = 0;
                    i++;
                    Console.Write("\n");
                //    if (i == rows)
                //        break;
                }
            }
            Console.WriteLine("-------------");
        }

        public void PrintTrack()
        {
            Stack<State> stack = this.GetTrack();

            while(stack.Count > 0)
            {
                stack.Pop().Print();
            }
        }

        public Stack<State> GetTrack()
        {
            State state = this;
            Stack<State> stack = new Stack<State>();
            while (state != null)
            {
                stack.Push(state);
                state = state.Parent();
            }

            return stack;
        }

        public void Move(int dr, int dc)
        {
            int nBlackCellIndex = Offset(this.blackCellndex, this.rows, this.cols, dr, dc);
            if (nBlackCellIndex != -1)
            {
                Swap(this.blackCellndex, nBlackCellIndex);
                this.blackCellndex = nBlackCellIndex;
            }else
            {
                Console.WriteLine("Can not perform move operation; out of bounds ex\n");
            }
            Print();
        }

        public void Move(int i, int dr, int dc)
        {
            int nIndex = Offset(i, this.rows, this.cols, dr, dc);
            Swap(i, nIndex);
            blackCellndex = nIndex;
            Print();
        }

        public void JustMove(int i, int j)
        {
            Swap(i, j);
            blackCellndex = (blackCellndex == i) ? j : i;
            Print();
        }

        public State MakeMove(int offset)
        {
            if (this.blackCellndex + offset >= 0 && this.blackCellndex + offset < pattern.Length)
            {
                State state = new State(this, this.rows, this.cols, null, this.final_pattern, false, false);
                state.blackCellndex = blackCellndex + offset;

                char[] chars = new char[pattern.Length];
                pattern.CopyTo(chars, 0);

                char temp = chars[blackCellndex];
                chars[blackCellndex] = chars[blackCellndex + offset];
                chars[blackCellndex + offset] = temp;
                state.pattern = chars;

                return state;
            }else
            {
                return null;
            }

        }

        public bool Finished()
        {
            int i = 0, j = 0;
            while(i != pattern.Length)
            {
                if(pattern[i] == '.')
                {
                    i++;
                    continue;
                }else if(final_pattern[j] == '.')
                {
                    j++;
                    continue;
                }else
                {
                    if (pattern[i] != final_pattern[j])
                        break;
                    i++;
                    j++;
                }
            }
            return (i == pattern.Length);
        }

        public int GetColumnsCount()
        {
            return this.cols;
        }

        public int GetRowsCount()
        {
            return this.rows;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is State))
                return false;

            char[] obj_pattern = (obj as State).pattern;

            if (this.pattern.Length != obj_pattern.Length)
                return false;

            string s1 = "", s2 = "";

            for (int i = 0; i < pattern.Length; i++)
            {
                if(pattern[i] != '.')
                {
                    s1 += pattern[i];
                }

                if(obj_pattern[i] != '.')
                {
                    s2 += obj_pattern[i];
                }
            }

            return (s1.CompareTo(s2) == 0);
        }
        
        public override int GetHashCode()
        {
            var hashCode = -766287705;
            hashCode = hashCode * -1521134295 + rows.GetHashCode();
            hashCode = hashCode * -1521134295 + cols.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<char[]>.Default.GetHashCode(pattern);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(final_pattern);
            return hashCode;
        }

        public override string ToString()
        {        
            string tostring = "";
            tostring += "rows:" + this.rows.ToString();
            tostring += "cols:" + this.cols.ToString();
            tostring += "final_pattern:" + this.final_pattern;
            tostring += "pattern:" + new string(this.pattern);

            return tostring;
        }

        public int Weight()
        {
            int mWeight = 0;
            for(int i=0, j=0, k=0; i<pattern.Length; i++, j++)
            {
                if(pattern[i] == '.')
                {
                    j--;
                    continue;
                }

                if(final_pattern[j] == '.')
                {
                    i--;
                    continue;
                }

                if (final_pattern[j] > pattern[i])
                    mWeight += (10 + final_pattern[j] - pattern[i]) * (10 << k);
                else 
                    mWeight += (pattern[i] - final_pattern[j]) * (10 << k);
                k++;
            }

            return mWeight;
        }

        public string GetPattern()
        {
            return new string(pattern);
        }

        public int CompareTo(object obj)
        {
            return this.Weight();
        }
    }
}
