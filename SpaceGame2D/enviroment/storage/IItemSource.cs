using SpaceGame2D.graphics.renderables;
using SpaceGame2D.graphics.texturemanager;

namespace SpaceGame2D.enviroment.storage
{
    public interface IItemSource
    {

        string Name { get; }

        string UniqueIdentifier { get; }
        TextureTileFrame UpdateCurrentImage(float animation_time);

        IRenderableWorldGraphic graphic { get; }
    }
}