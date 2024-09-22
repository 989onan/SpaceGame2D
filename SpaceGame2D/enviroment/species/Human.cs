using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.species
{
    public class Human: ISpecies
    {

        public string standing_image => "species/human/standing.png";

        public float walk_speed => 10f;
        public float jump_velocity => 30f;

        public Human()
        {
            Console.WriteLine("create human");


        }

        public TextureTile GetCurrentImage()
        {
            return Atlas.getTexture(standing_image);
        }

        public void LoadSpecies(Dictionary<string, ISpecies> speciesList)
        {
            speciesList.Add("SpaceGame2D:Human", new Human());
        }
    }
}
