using SpaceGame2D.enviroment.Entities;
using SpaceGame2D.enviroment.Entities.Species;
using SpaceGame2D.enviroment.materials;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.utilities.math;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.blockTypes
{
    public class Air : IBlock
    {

        private AABB private_bounding_box;
        public AABB bounding_box { get => private_bounding_box; set => setPosition(value); }

        public TileGrid source_grid { get; }

        public Point size => new Point(1, 1);

        public Air(Point position, TileGrid source_grid)
        {
            this.source_grid = source_grid;
            this.private_bounding_box = AABB.Size_To_AABB(new Vector2(position.X,position.Y), new Vector2(size.X, size.Y));
            
            //this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default", "blocks/air.png");

            this._physlock = new ReaderWriterLockSlim();
        }

        public Air() {}

        public IRenderableTile graphic { get; set; }

        public IMaterial surfaceProperties => new AirMaterial();
        public ReaderWriterLockSlim _physlock { get; }

        public IWorld World => this.source_grid.world;

        public void setPosition(AABB box)
        {
            Point center = new Point((int)this.bounding_box.Center.X, (int)this.bounding_box.Center.Y);
            source_grid.setTile(center, new Air(center, this.source_grid));
            this.private_bounding_box = box;
            source_grid.setTile(new Point((int)box.Center.X, (int)box.Center.Y), this);
            
        }
    }
}
