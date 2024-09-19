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

        public TextureTile standing_image => _standing_image;
        private static TextureTile _standing_image = new TextureTile("species/human/standing.png");
        public Human() {
            
        }

        public static Human instance = new Human();
    }
}
