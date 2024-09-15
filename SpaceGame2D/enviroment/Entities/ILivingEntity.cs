using SpaceGame2D.enviroment.Entities.Species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.Entities
{
    internal interface ILivingEntity: IDamageableEntity
    {
        ISpecies species { get; }

        bool is_crouching { get; }
        bool is_running { get; }

    }
}
