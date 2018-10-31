using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace angelica_puzzle
{
    interface IGameWindow
    {
        void Update(State currentState);
        void Update(Stack<State> track);
        void Finish();
    }
}
