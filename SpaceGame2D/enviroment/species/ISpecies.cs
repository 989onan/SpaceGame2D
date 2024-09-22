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
        string standing_image {  get; }

        float walk_speed { get; }

        float jump_velocity { get; }

        void LoadSpecies(Dictionary<string, ISpecies> speciesList); //this can be called more than once.
    }
}
