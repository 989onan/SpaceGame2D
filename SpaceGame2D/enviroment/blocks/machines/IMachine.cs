using SpaceGame2D.threads.Factory_Threads;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.blocks.machines
{
    public interface IMachine : IBlock
    {
        FactorySubThread source_factory {get;}
        public List<IMachine> Links();

        public bool links_changed { get; set;  }
        void Iterate();
    }
}
