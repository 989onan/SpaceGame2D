using SpaceGame2D.threads.Factory_Threads;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.blocks.machines
{

    //when it comes to modding this, make sure your IMachine always returns the same input and output types.
    //if you have a solar panel that varies based on time of day, use IVariableMachine, and use IVariableMachine somewhat sparingly!
    public interface IMachine : IBlock
    {
        FactorySubThread source_factory { get; set; }

        List<ModifiableTuple2<Dictionary<string, int>>> ProcessingTypes { get; }

        int selected_recipe { get; set; }
    }
}
