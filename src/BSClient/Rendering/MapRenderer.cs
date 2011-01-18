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
        private readonly ITextureManager _textureManager;
        [Inject]
        public MapRenderer(IMap map, IKernel kernel, ITextureManager textureManager)
        {
            Map = map;
            _textureManager = textureManager;
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
                ChunkRenderer renderer = new ChunkRenderer(e.NewValue, _textureManager);
                _renderers.Add(renderer);
            }
        }

        public void Render()
        {
            _textureManager.RenderCubeTexture();
            foreach (ChunkRenderer renderer in _renderers)
            {
                renderer.Render();
            }
        }
    }

    internal class ChunkRenderer : IRenderable
    {
        public readonly IChunk Chunk;
        private Vbo<VertexPositionColorTexture> _vbo;
        private readonly ITextureManager _textureManger;
        public ChunkRenderer(IChunk chunk, ITextureManager textureManger)
        {
            Chunk = chunk;
            _textureManger = textureManger;
            Chunk.BlockChanged += new EventHandler<BlockChangedEventArgs>(Chunk_BlockChanged);
            rebuild();
        }

        void Chunk_BlockChanged(object sender, BlockChangedEventArgs e)
        {
            rebuild();
        }

        private void rebuild()
        {
            IList<VertexPositionColorTexture> vertexes = new List<VertexPositionColorTexture>();
            IList<short> indices = new List<short>();
            for (byte x = 0; x < BSCoreConstants.CHUNK_SIZE_X; x++)
                for (byte y = 0; y < BSCoreConstants.CHUNK_SIZE_Y; y++)
                    for (byte z = 0; z < BSCoreConstants.CHUNK_SIZE_Z; z++)
                    {
                        IBlock block = Chunk[x, y, z];
                        if (block == null) continue;
                        if (z == 0 || Chunk[x, y, (byte)(z - 1)] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Front,
                                _textureManger.AddCubeTexture(block.GetTextureForSide(BlockSides.Front)));
                        if (z == BSCoreConstants.CHUNK_SIZE_Z - 1 || Chunk[x, y, (byte)(z + 1)] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Back,
                                _textureManger.AddCubeTexture(block.GetTextureForSide(BlockSides.Back)));
                        if (x == 0 || Chunk[(byte)(x - 1), y, z] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Left,
                                _textureManger.AddCubeTexture(block.GetTextureForSide(BlockSides.Left)));
                        if (x == BSCoreConstants.CHUNK_SIZE_X - 1 || Chunk[(byte)(x + 1), y, z] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Right,
                                _textureManger.AddCubeTexture(block.GetTextureForSide(BlockSides.Right)));
                        if (y == 0 || Chunk[x, (byte)(y - 1), z] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Bottom,
                                _textureManger.AddCubeTexture(block.GetTextureForSide(BlockSides.Bottom)));
                        if (y == BSCoreConstants.CHUNK_SIZE_Y - 1 || Chunk[x, (byte)(y + 1), z] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Top,
                                _textureManger.AddCubeTexture(block.GetTextureForSide(BlockSides.Top)));
                    }
            if (_vbo != null) _vbo.Dispose();
            _vbo = new Vbo<VertexPositionColorTexture>(vertexes.ToArray(), indices.ToArray());
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
        public static void createCubeSide(ref IList<VertexPositionColorTexture> vertexes, ref IList<short> indices, Vector3 frontBottomLeftOfCube, BlockSides sideType, ITextureReference texture)
        {
            //Create vertexes
            float x = frontBottomLeftOfCube.X;
            float y = frontBottomLeftOfCube.Y;
            float z = frontBottomLeftOfCube.Z;
            RectangleF textLoc = texture.TextureLocation;
            switch (sideType)
            {
                case BlockSides.Front:
                    appendIndiciesForSideOfCube(ref vertexes, ref indices);
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y - 0.0f, z - 0.0f, Color.Gray, textLoc.X, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y - 0.0f, z - 0.0f, Color.Gray, textLoc.X + textLoc.Width, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y + 1.0f, z - 0.0f, Color.White, textLoc.X + textLoc.Width, textLoc.Y + textLoc.Height));
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y + 1.0f, z - 0.0f, Color.White, textLoc.X, textLoc.Y + textLoc.Height));
                    break;
                case BlockSides.Back:
                    appendIndiciesForSideOfCube(ref vertexes, ref indices);
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y + 1.0f, z + 1.0f, Color.White, textLoc.X, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y + 1.0f, z + 1.0f, Color.White, textLoc.X + textLoc.Width, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y - 0.0f, z + 1.0f, Color.Gray, textLoc.X + textLoc.Width, textLoc.Y + textLoc.Height));
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y - 0.0f, z + 1.0f, Color.Gray, textLoc.X, textLoc.Y + textLoc.Height));
                    break;
                case BlockSides.Left:
                    appendIndiciesForSideOfCube(ref vertexes, ref indices);
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y - 0.0f, z + 1.0f, Color.Gray, textLoc.X, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y - 0.0f, z - 0.0f, Color.Gray, textLoc.X + textLoc.Width, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y + 1.0f, z - 0.0f, Color.White, textLoc.X + textLoc.Width, textLoc.Y + textLoc.Height));
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y + 1.0f, z + 1.0f, Color.White, textLoc.X, textLoc.Y + textLoc.Height));
                    break;
                case BlockSides.Right:
                    appendIndiciesForSideOfCube(ref vertexes, ref indices);
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y + 1.0f, z + 1.0f, Color.White, textLoc.X, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y + 1.0f, z - 0.0f, Color.White, textLoc.X + textLoc.Width, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y - 0.0f, z - 0.0f, Color.Gray, textLoc.X + textLoc.Width, textLoc.Y + textLoc.Height));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y - 0.0f, z + 1.0f, Color.Gray, textLoc.X, textLoc.Y + textLoc.Height));
                    break;
                case BlockSides.Top:
                    appendIndiciesForSideOfCube(ref vertexes, ref indices);
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y + 1.0f, z - 0.0f, Color.White, textLoc.X, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y + 1.0f, z - 0.0f, Color.White, textLoc.X + textLoc.Width, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y + 1.0f, z + 1.0f, Color.White, textLoc.X + textLoc.Width, textLoc.Y + textLoc.Height));
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y + 1.0f, z + 1.0f, Color.White, textLoc.X, textLoc.Y + textLoc.Height));
                    break;
                case BlockSides.Bottom:
                    appendIndiciesForSideOfCube(ref vertexes, ref indices);
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y - 0.0f, z + 1.0f, Color.Gray, textLoc.X, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y - 0.0f, z + 1.0f, Color.Gray, textLoc.X + textLoc.Width, textLoc.Y));
                    vertexes.Add(new VertexPositionColorTexture(x + 1.0f, y - 0.0f, z - 0.0f, Color.Gray, textLoc.X + textLoc.Width, textLoc.Y + textLoc.Height));
                    vertexes.Add(new VertexPositionColorTexture(x - 0.0f, y - 0.0f, z - 0.0f, Color.Gray, textLoc.X, textLoc.Y + textLoc.Height));
                    break;
            }
        }

        private static void appendIndiciesForSideOfCube(ref IList<VertexPositionColorTexture> vertexes, ref IList<short> indices)
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
