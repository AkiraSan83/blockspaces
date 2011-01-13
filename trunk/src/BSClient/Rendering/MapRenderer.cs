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
        private Vbo<VertexPositionColor> _vbo;
        public ChunkRenderer(IChunk chunk)
        {
            Chunk = chunk;
            Chunk.BlockChanged += new EventHandler<BlockChangedEventArgs>(Chunk_BlockChanged);
            rebuild();
        }

        void Chunk_BlockChanged(object sender, BlockChangedEventArgs e)
        {
            rebuild();
        }

        private void rebuild()
        {
            IList<VertexPositionColor> vertexes = new List<VertexPositionColor>();
            IList<short> indices = new List<short>();
            for (byte x = 0; x < BSCoreConstants.CHUNK_SIZE_X; x++)
                for (byte y = 0; y < BSCoreConstants.CHUNK_SIZE_Y; y++)
                    for (byte z = 0; z < BSCoreConstants.CHUNK_SIZE_Z; z++)
                    {
                        if (Chunk[x, y, z] == null) continue;
                        BlockSides sides = BlockSides.None;
                        if (z == 0 || Chunk[x, y, (byte)(z - 1)] == null)
                            sides |= BlockSides.Front;
                        if (z == BSCoreConstants.CHUNK_SIZE_Z - 1 || Chunk[x, y, (byte)(z + 1)] == null)
                            sides |= BlockSides.Back;
                        if (x == 0 || Chunk[(byte)(x - 1), y, z] == null)
                            sides |= BlockSides.Left;
                        if (x == BSCoreConstants.CHUNK_SIZE_X - 1 || Chunk[(byte)(x + 1), y, z] == null)
                            sides |= BlockSides.Right;
                        if (y == 0 || Chunk[x, (byte)(y - 1), z] == null)
                            sides |= BlockSides.Bottom;
                        if (y == BSCoreConstants.CHUNK_SIZE_Y - 1 || Chunk[x, (byte)(y + 1), z] == null)
                            sides |= BlockSides.Top;
                        createCubeSide(ref vertexes, ref indices, new Vector3(x, y, z), sides);
                    }
            if (_vbo != null) _vbo.Dispose();
            _vbo = new Vbo<VertexPositionColor>(vertexes.ToArray(), indices.ToArray());
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
        public static void createCubeSide(ref IList<VertexPositionColor> vertexes, ref IList<short> indices, Vector3 frontBottomLeftOfCube, BlockSides sideType)
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
            _vbo.Render();
        }
    }
}
