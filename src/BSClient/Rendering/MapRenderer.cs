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

    internal class ChunkRenderer : IRenderable
    {
        public readonly IChunk Chunk;
        public ChunkRenderer(IChunk chunk)
        {
            Chunk = chunk;
            Chunk.BlockChanged += new EventHandler<BlockChangedEventArgs>(Chunk_BlockChanged);
        }

        void Chunk_BlockChanged(object sender, BlockChangedEventArgs e)
        {
            rebuild();
        }

        private void rebuild()
        {
            for (int x = 0; x < BSCoreConstants.CHUNK_SIZE_X; x++)
                for (int y = 0; y < BSCoreConstants.CHUNK_SIZE_Y; y++)
                    for (int z = 0; z < BSCoreConstants.CHUNK_SIZE_Z; z++)
                    {

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
        | | |  / / /           | | |/ / /     /
        | | | / / /            | | | / /     /
        | | |/ / /             | | |/ /     /
        | | | / /              | | ' /     /  +Z axis
        | | |/_/_______________| |  /     /
        | |____________________| | /     /
        |________Front___________|/    \/
     (0,0,0)
          ---------------------->
                +X axis
*/
        public static void _createCubeSide(ref IList<VertexPositionColor> vertexes, ref IList<short> indices, ref Vector3 frontBottomLeftOfCube, BlockSides sideType)
        {
            //Create vertexes
            float x = frontBottomLeftOfCube.X;
            float y = frontBottomLeftOfCube.Y;
            float z = frontBottomLeftOfCube.Z;
            if ((sideType & BlockSides.Front) == BlockSides.Front)
            {
                appendIndiciesForSideOfCube(ref vertexes, ref indices);
                vertexes.Add(new VertexPositionColor(x - 0.0f, y - 0.0f, z - 0.0f, Color.Red));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y - 0.0f, z - 0.0f, Color.Green));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y + 1.0f, z - 0.0f, Color.Blue));
                vertexes.Add(new VertexPositionColor(x - 0.0f, y + 1.0f, z - 0.0f, Color.Purple));
            }
            if ((sideType & BlockSides.Back) == BlockSides.Back)
            {
                appendIndiciesForSideOfCube(ref vertexes, ref indices);
                vertexes.Add(new VertexPositionColor(x - 0.0f, y + 1.0f, z + 1.0f, Color.Purple));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y + 1.0f, z + 1.0f, Color.Blue));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y - 0.0f, z + 1.0f, Color.Green));
                vertexes.Add(new VertexPositionColor(x - 0.0f, y - 0.0f, z + 1.0f, Color.Red));
            }
            if ((sideType & BlockSides.Left) == BlockSides.Left)
            {
                appendIndiciesForSideOfCube(ref vertexes, ref indices);
                vertexes.Add(new VertexPositionColor(x - 0.0f, y - 0.0f, z + 1.0f, Color.Red));
                vertexes.Add(new VertexPositionColor(x - 0.0f, y - 0.0f, z - 0.0f, Color.Green));
                vertexes.Add(new VertexPositionColor(x - 0.0f, y + 1.0f, z - 0.0f, Color.Blue));
                vertexes.Add(new VertexPositionColor(x - 0.0f, y + 1.0f, z + 1.0f, Color.Purple));
            }
            if ((sideType & BlockSides.Right) == BlockSides.Right)
            {
                appendIndiciesForSideOfCube(ref vertexes, ref indices);
                vertexes.Add(new VertexPositionColor(x + 1.0f, y + 1.0f, z + 1.0f, Color.Purple));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y + 1.0f, z - 0.0f, Color.Blue));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y - 0.0f, z - 0.0f, Color.Green));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y - 0.0f, z + 1.0f, Color.Red));
            }
            if ((sideType & BlockSides.Top) == BlockSides.Top)
            {
                appendIndiciesForSideOfCube(ref vertexes, ref indices);
                vertexes.Add(new VertexPositionColor(x - 0.0f, y + 1.0f, z - 0.0f, Color.Red));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y + 1.0f, z - 0.0f, Color.Green));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y + 1.0f, z + 1.0f, Color.Blue));
                vertexes.Add(new VertexPositionColor(x - 0.0f, y + 1.0f, z + 1.0f, Color.Purple));
            }
            if ((sideType & BlockSides.Bottom) == BlockSides.Bottom)
            {
                appendIndiciesForSideOfCube(ref vertexes, ref indices);
                vertexes.Add(new VertexPositionColor(x - 0.0f, y - 0.0f, z + 1.0f, Color.Purple));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y - 0.0f, z + 1.0f, Color.Blue));
                vertexes.Add(new VertexPositionColor(x + 1.0f, y - 0.0f, z - 0.0f, Color.Green));
                vertexes.Add(new VertexPositionColor(x - 0.0f, y - 0.0f, z - 0.0f, Color.Red));
            }
        }

        private static void appendIndiciesForSideOfCube(ref IList<VertexPositionColor> vertexes, ref IList<short> indices)
        {
            //Create indicies
            int vertexesStartIndex = vertexes.Count;
            indices.Add((short)(vertexesStartIndex + 0));
            indices.Add((short)(vertexesStartIndex + 1));
            indices.Add((short)(vertexesStartIndex + 2));
            indices.Add((short)(vertexesStartIndex + 2));
            indices.Add((short)(vertexesStartIndex + 3));
            indices.Add((short)(vertexesStartIndex + 0));
        }

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}
