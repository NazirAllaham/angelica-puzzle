using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace angelica_puzzle
{
    class Game
    {
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 1, -1, 0, 0 };

        private IGameWindow emitter;

        //connected player
        private Player player;

        //game objects
        private int rows;
        private int cols;
        private string pattern;

        private int stepsCount;
        private bool solved;

        private LinkedList<State> states;
        private State finalState;
        private State currentState;

        public Game(Player player, int rows, int columns, string pattern)
        {
            this.player = player;

            this.rows = rows;
            this.cols = columns;
            this.pattern = pattern;

            if (!this.pattern.Contains('.') || this.pattern.Length != this.rows * this.cols)
                throw new Exception("Game Objects Not Valid, Pattern Not Contain '.' Or Pattern Length Not Compatible With Rows and Cols");

            this.Initialize();
            //this.GameLoop();
        }

        public Game(Player player, int rows, int columns, string pattern, IGameWindow emitter) : this(player, rows, columns, pattern)
        {
            this.emitter = emitter;
        }

        private void Initialize()
        {
            this.states = new LinkedList<State>();

            this.currentState = new State(null, this.rows, this.cols, this.pattern, null, true);

            this.finalState = new State(null, this.rows, this.cols, this.pattern);

            this.stepsCount = 0;
            this.solved = false;
        }

        private void GameLoop()
        {
            this.emitter.Update(this.currentState);

            /*
            while (!this.solved)
            {
                
            }
            */
        }

        public void DoMove(int dx, int dy)
        {
            this.stepsCount++;
            this.currentState.Move(dx, dy);
            this.states.AddFirst(this.currentState);

            this.emitter.Update(this.currentState);
            Console.WriteLine(this.currentState.Finished());
            if(solved || this.currentState.Finished())
            {
                Finish();
            }
        }

        public string GetPattern()
        {
            return this.currentState.GetPattern();
        }

        public int GetStepsCount()
        {
            return this.stepsCount;
        }

        public void IncreaseStepsCount(int i)
        {
            this.stepsCount += i;
        }

        public State GetCurrent()
        {
            return this.currentState;
        }

        public void SetCurrent(State state)
        {
            this.currentState = state;
        }

        public State GetFinal()
        {
            return this.finalState;
        }

        private void Finish()
        {
            this.emitter.Finish();
        }
    }
}
