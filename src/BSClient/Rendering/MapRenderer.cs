using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Rendering;
using JollyBit.BS.World;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Drawing;
using JollyBit.BS.Utility;
using Ninject;
using Ninject.Parameters;

namespace JollyBit.BS.Rendering
{
    public interface IMapRenderer : IRenderable
    {
        IMap Map { get; }
    }

    public class MapRenderer : IRenderable
    {
        public readonly IMap Map;
        private readonly ICollection<ChunkRenderer> _renderers = new List<ChunkRenderer>();
        [Inject]
        public MapRenderer(IMap map, IKernel kernel)
        {
            Map = map;
            Map.ChunkChanged += new EventHandler<ItemChangedEventArgs<IChunk>>(Map_ChunkChanged);
            //Create renderers for all chunks that already exist
            foreach (IChunk chunk in Map.Chunks)
            {
                Map_ChunkChanged(this, new ItemChangedEventArgs<IChunk>(null, chunk)); 
            }
        }

        void Map_ChunkChanged(object sender, ItemChangedEventArgs<IChunk> e)
        {
            if (e.OldValue != null)
            {
                ChunkRenderer renderer = _renderers.FirstOrDefault(i => i.Chunk == e.OldValue);
                if (renderer != null) _renderers.Remove(renderer);
            }
            if (e.NewValue != null)
            {
                ChunkRenderer renderer = new ChunkRenderer(e.NewValue);
                _renderers.Add(renderer);
            }
        }

        public void Render()
        {
            foreach (ChunkRenderer renderer in _renderers)
            {
                renderer.Render();
            }
        }
    }

    public class ChunkRenderer : IRenderable
    {
        public readonly IChunk Chunk;
        public ChunkRenderer(IChunk chunk)
        {
            Chunk = chunk;
            Chunk.BlockChanged += new EventHandler<BlockChangedEventArgs>(Chunk_BlockChanged);
        }

        void Chunk_BlockChanged(object sender, BlockChangedEventArgs e)
        {
            _rebuild();
        }

        private void _rebuild()
        {
            Point3L loc = Chunk.Location;
            List<VertexPositionColor> vertexes = new List<VertexPositionColor>();
            List<short> indices = new List<short>();
            short currentIndx = 0;
            for (byte x = 0; x < BSCoreConstants.CHUNK_SIZE_X; x++)
                for (byte y = 0; x < BSCoreConstants.CHUNK_SIZE_Y; y++)
                    for (byte z = 0; z < BSCoreConstants.CHUNK_SIZE_Z; z++)
                    {
                        if (Chunk[x,y,z] != null)
                        {

                            break;
                        }
                    }
        }

/*
            	 _________________________ (1,1,1)
                / _________Top_________  /|
               / / ___________________/ / |   
              / / /| |               / /  |  /|\
             / / / | |              / / . |   |
            / / /| | |             / / /| |   |
           / / / | | |            / / / | |   |
          / / /  | | |           / / /| | |   | +Y axis
         / /_/__________________/ / / | | |   |
        /________________________/ /  | | |   |
Left--> | ______________________ | |  | | |   | 
        | | |    | | |_________| | |__| | |   |
        | | |    | |___________| | |____| |   |
        | | |   / / ___________| | |_  / /    
        | | |  / / /           | | |/ / /     /\
        | | | / / /            | | | / /     /
        | | |/ / /             | | |/ /     /
        | | | / /              | | ' /     /  +Z axis
        | | |/_/_______________| |  /     /
        | |____________________| | /     /
        |________Front___________|/     /
     (0,0,0)
          ---------------------->
                +X axis
*/
        public static void _createCubeSide(ref VertexPositionColor[] vertexes, int vertexesStartIndex, ref short[] indices, int indicesStartIndex, ref Vector3 centerOfCube, CubeSideTypes sideType)
        {
            //Create indicies
            indices[indicesStartIndex + 0] = (short)(vertexesStartIndex + 0);
            indices[indicesStartIndex + 1] = (short)(vertexesStartIndex + 1);
            indices[indicesStartIndex + 2] = (short)(vertexesStartIndex + 2);
            indices[indicesStartIndex + 3] = (short)(vertexesStartIndex + 2);
            indices[indicesStartIndex + 4] = (short)(vertexesStartIndex + 3);
            indices[indicesStartIndex + 5] = (short)(vertexesStartIndex + 0);

            //Create vertexes
            float x = centerOfCube.X;
            float y = centerOfCube.Y;
            float z = centerOfCube.Z;
            switch (sideType)
            {
                case CubeSideTypes.Front:
                    vertexes[vertexesStartIndex + 0] = new VertexPositionColor(x - 0.5f, y - 0.5f, z - 0.5f, Color.Red);
                    vertexes[vertexesStartIndex + 1] = new VertexPositionColor(x + 0.5f, y - 0.5f, z - 0.5f, Color.Green);
                    vertexes[vertexesStartIndex + 2] = new VertexPositionColor(x + 0.5f, y + 0.5f, z - 0.5f, Color.Blue);
                    vertexes[vertexesStartIndex + 3] = new VertexPositionColor(x - 0.5f, y + 0.5f, z - 0.5f, Color.Purple);
                    break;
                case CubeSideTypes.Back:
                    vertexes[vertexesStartIndex + 3] = new VertexPositionColor(x - 0.5f, y - 0.5f, z + 0.5f, Color.Red);
                    vertexes[vertexesStartIndex + 2] = new VertexPositionColor(x + 0.5f, y - 0.5f, z + 0.5f, Color.Green);
                    vertexes[vertexesStartIndex + 1] = new VertexPositionColor(x + 0.5f, y + 0.5f, z + 0.5f, Color.Blue);
                    vertexes[vertexesStartIndex + 0] = new VertexPositionColor(x - 0.5f, y + 0.5f, z + 0.5f, Color.Purple);
                    break;
                case CubeSideTypes.Left:
                    vertexes[vertexesStartIndex + 0] = new VertexPositionColor(x - 0.5f, y - 0.5f, z + 0.5f, Color.Red);
                    vertexes[vertexesStartIndex + 1] = new VertexPositionColor(x - 0.5f, y - 0.5f, z - 0.5f, Color.Green);
                    vertexes[vertexesStartIndex + 2] = new VertexPositionColor(x - 0.5f, y + 0.5f, z - 0.5f, Color.Blue);
                    vertexes[vertexesStartIndex + 3] = new VertexPositionColor(x - 0.5f, y + 0.5f, z + 0.5f, Color.Purple);
                    break;
                case CubeSideTypes.Right:
                    vertexes[vertexesStartIndex + 3] = new VertexPositionColor(x + 0.5f, y - 0.5f, z + 0.5f, Color.Red);
                    vertexes[vertexesStartIndex + 2] = new VertexPositionColor(x + 0.5f, y - 0.5f, z - 0.5f, Color.Green);
                    vertexes[vertexesStartIndex + 1] = new VertexPositionColor(x + 0.5f, y + 0.5f, z - 0.5f, Color.Blue);
                    vertexes[vertexesStartIndex + 0] = new VertexPositionColor(x + 0.5f, y + 0.5f, z + 0.5f, Color.Purple);
                    break;
                case CubeSideTypes.Top:
                    vertexes[vertexesStartIndex + 0] = new VertexPositionColor(x - 0.5f, y + 0.5f, z - 0.5f, Color.Red);
                    vertexes[vertexesStartIndex + 1] = new VertexPositionColor(x + 0.5f, y + 0.5f, z - 0.5f, Color.Green);
                    vertexes[vertexesStartIndex + 2] = new VertexPositionColor(x + 0.5f, y + 0.5f, z + 0.5f, Color.Blue);
                    vertexes[vertexesStartIndex + 3] = new VertexPositionColor(x - 0.5f, y + 0.5f, z + 0.5f, Color.Purple);
                    break;
                case CubeSideTypes.Bottom:
                    vertexes[vertexesStartIndex + 3] = new VertexPositionColor(x - 0.5f, y - 0.5f, z - 0.5f, Color.Red);
                    vertexes[vertexesStartIndex + 2] = new VertexPositionColor(x + 0.5f, y - 0.5f, z - 0.5f, Color.Green);
                    vertexes[vertexesStartIndex + 1] = new VertexPositionColor(x + 0.5f, y - 0.5f, z + 0.5f, Color.Blue);
                    vertexes[vertexesStartIndex + 0] = new VertexPositionColor(x - 0.5f, y - 0.5f, z + 0.5f, Color.Purple);
                    break;
            }
        }

        public enum CubeSideTypes
        {
            Front = 0x0,
            Back = 0x1,
            Left = 0x2,
            Right = 0x4,
            Top = 0x8,
            Bottom = 0x10
        }

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}
