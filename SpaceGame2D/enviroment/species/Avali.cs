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
        public TextureTile standing_image => _standing_image;
        private static TextureTile _standing_image = new TextureTile("species/avali/standing.png");

        public Avali()
        {

        }

        public static Avali instance = new Avali();
    }
}
