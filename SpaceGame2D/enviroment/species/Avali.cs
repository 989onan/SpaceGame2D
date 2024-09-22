using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.graphics.texturemanager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.species
{
    public class Avali: ISpecies
    {

        public string standing_image => "species/avali/standing.png";

        public float jump_velocity => 7f;
        public float walk_speed => 20f;
        public Avali()
        {
            Console.WriteLine("create avali");


        }

        public TextureTile GetCurrentImage()
        {
            return Atlas.getTexture(standing_image);
        }

        public void LoadSpecies(Dictionary<string, ISpecies> speciesList)
        {
            speciesList.Add("SpaceGame2D:Avali", new Avali());
        }
    }
}
