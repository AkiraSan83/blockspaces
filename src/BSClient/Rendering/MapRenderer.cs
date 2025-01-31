﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Client.Rendering;
using JollyBit.BS.Core.World;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Drawing;
using JollyBit.BS.Core.Utility;
using Ninject;
using Ninject.Parameters;
using Ninject.Extensions.Logging;
using JollyBit.BS.Core;
//using JollyBit.BS.Client.Rendering;

namespace JollyBit.BS.Client.Rendering
{
    public class MapRenderer : IRenderable
    {
        public readonly IMap Map;
        private readonly ICollection<ChunkRenderer> _renderers = new List<ChunkRenderer>();
        private readonly ITextureAtlas _textureAtlas;
        private RenderConfig _config;

        public Camera camera;

		[Inject]
        public MapRenderer(IMap map, IKernel kernel, ITextureAtlasFactory textureAtlasFactory)
        {
            _config = Constants.Kernel.Get<RenderConfig>();
			
            Map = map;
            _textureAtlas = textureAtlasFactory.CreateTextureAtlas(new Size(_config.MaxTextureSize,_config.MaxTextureSize), new Size(_config.MaxTextureSize/16,_config.MaxTextureSize/16), 4);
            Map.ChunkChanged += new EventHandler<ChangedEventArgs<IChunk>>(Map_ChunkChanged);
            //Create renderers for all chunks that already exist
            foreach (IChunk chunk in Map.Chunks)
            {
                Map_ChunkChanged(this, new ChangedEventArgs<IChunk>(null, chunk));
            }
        }

        void Map_ChunkChanged(object sender, ChangedEventArgs<IChunk> e)
        {
            if (e.OldValue != null)
            {
                ChunkRenderer renderer = _renderers.FirstOrDefault(i => i.Chunk == e.OldValue);
                if (renderer != null) _renderers.Remove(renderer);
            }
            if (e.NewValue != null)
            {
                ChunkRenderer renderer = new ChunkRenderer(e.NewValue, _textureAtlas);
                _renderers.Add(renderer);
            }
        }

        public void Render()
        {
            _textureAtlas.Render();

//            Vector4[] planes = Frustum.CalculatePlanes(camera.ModelView, camera.Projection);
//            float radius = Math.Max( Math.Max(Constants.CHUNK_SIZE_X, Constants.CHUNK_SIZE_Y), Constants.CHUNK_SIZE_Z ) / 2;

//            ILogger logger = Constants.Kernel.Get<ILoggerFactory>().GetLogger(this.GetType());

            foreach (ChunkRenderer renderer in _renderers) {
//                Vector3 center = new Vector3();
//                center.X = renderer.Chunk.Location.X + Constants.CHUNK_SIZE_X/2;
//                center.Y = renderer.Chunk.Location.Y + Constants.CHUNK_SIZE_Y/2;
//                center.Z = renderer.Chunk.Location.Z + Constants.CHUNK_SIZE_Z/2;

    //            logger.Debug(string.Format("Chunk centered at {0} (r: {1}) returns: {2}",center,radius,Frustum.InsideFrustum(planes,center,radius)));

                //if(Frustum.InsideFrustum(planes,center,radius))
                renderer.Render();
            }

            //int a = 0;
        }
    }

    internal class ChunkRenderer : IRenderable
    {
        public readonly IChunk Chunk;
        private Vbo<VertexPositionColorTexture> _vbo;
        private ITextureAtlas _textureAtlas;
        public ChunkRenderer(IChunk chunk, ITextureAtlas textureAtlas)
        {
            Chunk = chunk;
            _textureAtlas = textureAtlas;
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
            ContentManager contentManager = Constants.Kernel.Get<ContentManager>();
            for (byte x = 0; x < Constants.CHUNK_SIZE_X; x++)
                for (byte y = 0; y < Constants.CHUNK_SIZE_Y; y++)
                    for (byte z = 0; z < Constants.CHUNK_SIZE_Z; z++)
                    {
                        IBlock block = Chunk[x, y, z];
                        if (block == null) continue;
                        if (z == 0 || Chunk[x, y, (byte)(z - 1)] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Front,
                                _textureAtlas.AddSubImage(contentManager.LoadBitmap(block.GetTextureForSide(BlockSides.Front))));
                        if (z == Constants.CHUNK_SIZE_Z - 1 || Chunk[x, y, (byte)(z + 1)] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Back,
                                _textureAtlas.AddSubImage(contentManager.LoadBitmap(block.GetTextureForSide(BlockSides.Back))));
                        if (x == 0 || Chunk[(byte)(x - 1), y, z] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Left,
                                _textureAtlas.AddSubImage(contentManager.LoadBitmap(block.GetTextureForSide(BlockSides.Left))));
                        if (x == Constants.CHUNK_SIZE_X - 1 || Chunk[(byte)(x + 1), y, z] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Right,
                                _textureAtlas.AddSubImage(contentManager.LoadBitmap(block.GetTextureForSide(BlockSides.Right))));
                        if (y == 0 || Chunk[x, (byte)(y - 1), z] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Bottom,
                                _textureAtlas.AddSubImage(contentManager.LoadBitmap(block.GetTextureForSide(BlockSides.Bottom))));
                        if (y == Constants.CHUNK_SIZE_Y - 1 || Chunk[x, (byte)(y + 1), z] == null)
                            createCubeSide(ref vertexes, ref indices, (Vector3)Chunk.Location + new Vector3(x, y, z), BlockSides.Top,
                                _textureAtlas.AddSubImage(contentManager.LoadBitmap(block.GetTextureForSide(BlockSides.Top))));
                    }
            if (_vbo != null) _vbo.Dispose();
            _vbo = new Vbo<VertexPositionColorTexture>(vertexes.ToArray(), indices.ToArray());
        }

        /*
                         _________________________ (1,1,0)
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
             (0,0,1)
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
            indices.Add((short)(vertexesStartIndex + 2));
            indices.Add((short)(vertexesStartIndex + 1));
            indices.Add((short)(vertexesStartIndex + 0));
            indices.Add((short)(vertexesStartIndex + 0));
            indices.Add((short)(vertexesStartIndex + 3));
            indices.Add((short)(vertexesStartIndex + 2));
        }

        public void Render()
        {
            _vbo.Render();
        }
    }
}
