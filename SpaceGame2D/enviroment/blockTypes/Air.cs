using SpaceGame2D.enviroment.Entities;
using SpaceGame2D.enviroment.materials;
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
        private Point _position;
        public Point position { get => getPosition(); set => setPosition(value); }

        public TileGrid source_grid { get; }

        public Point size => new Point(1, 1);

        public Air(Point position, TileGrid source_grid)
        {
            this.source_grid = source_grid;
            this._position = position;
            this._physlock = new ReaderWriterLockSlim();
        }

        public Air() {}

        public IMaterial surfaceProperties => new AirMaterial();

        Vector2 IStaticPhysicsObject.position { get { return new Vector2(this.position.X, this.position.Y); } set => throw new NotImplementedException(); }

        Vector2 IStaticPhysicsObject.size => new Vector2(this.size.X, this.size.Y);
        public ReaderWriterLockSlim _physlock { get; }

        public IWorld World => this.source_grid.world;

        public ImageResult currentimage {  get; set; }

        public Point getPosition() {
            return this._position;
        }
        public void setPosition(Point position)
        {
            source_grid.setTile(this._position, new Air(this._position, this.source_grid));
            this._position = position;
            source_grid.setTile(this._position, this);
        }
    }
}
