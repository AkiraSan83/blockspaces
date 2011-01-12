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
            	 _________________________
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
       
          ---------------------->
                +X axis
         */
        public static void _createCubeSide(out VertexPositionColor[] vertexes, out short[] indicies, int startIndex, ref Vector3 centeOfSide, CubeSideTypes sideType)
        {
            
        }

        public enum CubeSideTypes
        {
            Front,
            Back,
            Left,
            Right,
            Top,
            Bottom
        }

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}
