using SpaceGame2D.graphics.texturemanager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.species
{
    public interface ISpecies
    {
        TextureTile standing_image {  get; }
    }
}
