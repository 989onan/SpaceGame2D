using SpaceGame2D.enviroment.physics;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment
{
    public class Block: IStaticPhysicsObject, IRenderableObject
    {

        public BlockGrid grid;
        public Vector2 position { get => getPosition(); set => block_position = new Point((int)value.X, (int)value.Y); } //this allows us to render the block dynamically on screen from a position.

        private Vector2 getPosition()
        {
            Point position_grid = grid.getTileLocation(this);
            return new Vector2(position_grid.X + grid.RenderOffset.X, position_grid.Y + grid.RenderOffset.Y);
        }

        public Point block_position { get => grid.getTileLocation(this); set => grid.moveTileLocation(this, value); }
        public Vector2 size => new Vector2(1, 1);

        public AABB Collider { get => AABB.Size_To_AABB(position, size);}
        public bool HasCollision { get; set; }

        public TextureTile currentTexture { get => UpdateCurrentImage(); } //TODO: Load texture before needing it - @989onan



        public Block(BlockGrid grid)
        {
            this.grid = grid;
            this.HasCollision = true;


        }
    }
}
