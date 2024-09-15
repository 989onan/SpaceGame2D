using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.Entities.Species
{
    public class Human : ISpecies
    {
        public string name => "Human";
        public string description => "From the planet they call \"Earth\" they have an inherent nature to infight, but have managed themselves by making complex and often convoluted governments. They are very friendly towards other sentient species, even if they don't love Humans back.";
        public Vector2 stand_size => new Vector2(.4f,1.9f);
        public Vector2 crouchsize => new Vector2(.4f, 1.6f);

        public Vector2 crawlsize => new Vector2(.4f, 1.6f);


        public float weight => 3f;
        public Human() {
            StbImage.stbi_set_flip_vertically_on_load(1);
            this.standing = ImageResult.FromStream(File.OpenRead(Path.Join(BootStrapper.path, "graphics/textures/species/human/human.png")),ColorComponents.RedGreenBlueAlpha);
        }

        public float jump_height => 3f;



        public float walk_speed => 1f;
        public float run_speed => 3f;
        public float crouch_speed => .5f;

        public float crouch_run_speed => .6f;

        public float crawl_speed => .1f;

        public float crawl_run_speed => .13f;
        public int base_armor => 0;
        public int max_health => 100;
        public int base_mana => 100;

        public ImageResult standing { get; }
    }
}
