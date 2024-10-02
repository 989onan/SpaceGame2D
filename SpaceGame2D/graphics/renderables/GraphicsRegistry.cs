using OpenTK.Graphics.OpenGL;
using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.renderables
{
    public class GraphicsRegistry
    {

        public static int VertexArrayObject_Quad;
        public static int ElementBufferObject_Quad;
        public static int VertexBufferObject_Quad;
        public static byte[] indices_Quad = {0,1,2, // first triangle (bottom left - top left - top right)
                     0,2,3};

        private static List<IRenderableWorldGraphic> _worldGraphicObjects = new List<IRenderableWorldGraphic>();
        private static List<IRenderable> _renderableObjects = new List<IRenderable>();

        public GraphicsRegistry() { }


        //Overlays/GUIS

        public static bool registerRenderGraphic(IRenderable renderGraphic)
        {
            if (_renderableObjects.Contains(renderGraphic))
            {
                return false;
            }
            _renderableObjects.Add(renderGraphic);
            return true;
        }

        public static bool deregisterRenderGraphic(IRenderable renderGraphic)
        {
            if (!_renderableObjects.Contains(renderGraphic))
            {
                return false;
            }
            //renderGraphic.DisposeGraphic();
            _renderableObjects.Remove(renderGraphic);
            return true;
        }

        public static IReadOnlyList<IRenderable> getAllRenderGraphics()
        {
            List<IRenderable> result = new List<IRenderable>();
            result.AddRange(_renderableObjects);
            //result;
            return result.AsReadOnly();
        }


        //world
        public static bool registerWorldRenderGraphic(IRenderableWorldGraphic renderGraphic)
        {
            if (_worldGraphicObjects.Contains(renderGraphic))
            {
                return false;
            }
            _worldGraphicObjects.Add(renderGraphic);
            return true;
        }

        public static bool deregisterWorldRenderGraphic(IRenderableWorldGraphic renderGraphic)
        {
            if (!_worldGraphicObjects.Contains(renderGraphic))
            {
                return false;
            }
            //renderGraphic.DisposeGraphic();
            _worldGraphicObjects.Remove(renderGraphic);
            return true;
        }

        public static IReadOnlyList<IRenderableWorldGraphic> getAllWorldGraphics()
        {
            List<IRenderableWorldGraphic> result = new List<IRenderableWorldGraphic>();
            result.AddRange(_worldGraphicObjects);
            //result;
            return result.AsReadOnly();
        }


        //buffers
        public static void LoadBuffers()
        {
            GraphicsRegistry.ElementBufferObject_Quad = GL.GenBuffer();
            GraphicsRegistry.VertexArrayObject_Quad = GL.GenVertexArray();
            GraphicsRegistry.VertexBufferObject_Quad = GL.GenBuffer();
        }

        public static void DisposeBuffers()
        {
            //Console.WriteLine("disposing graphic");
            GL.DeleteBuffer(GraphicsRegistry.VertexBufferObject_Quad);
            GL.DeleteVertexArray(GraphicsRegistry.VertexArrayObject_Quad);
            GL.DeleteVertexArray(GraphicsRegistry.ElementBufferObject_Quad);
        }

        
    }
}
