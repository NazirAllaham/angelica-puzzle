using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace angelica_puzzle
{
    class Player
    {
        private string name;
        private string gender;

        public Player(string name, string gender)
        {
            this.name = name;
            this.gender = gender;
        }

        public string Name { get => name; set => name = value; }
        public string Gender { get => gender; set => gender = value; }
    }
}
