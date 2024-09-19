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

        public Avali()
        {
            Atlas.AddToQue(new Texture(standing_image));

        }

        public static Avali instance = new Avali();
    }
}
