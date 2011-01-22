using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace JollyBit.BS.Client.Rendering
{
    public class Vbo<TVertex> : IDisposable, IRenderable where TVertex : struct
    {
        public BeginMode PrimitiveMode = BeginMode.Triangles;

        private int _vboId, _eboId, _numElements, _stride;

        public int VboId
        {
            get { return _vboId; }
        }
        public int EboId
        {
            get { return _eboId; }
        }
        public int NumElements
        {
            get { return _numElements; }
        }

        public Vbo(TVertex[] vertices, short[] elements)
        {
            // To create a VBO:
            // 1) Generate the buffer handles for the vertex and element buffers.
            // 2) Bind the vertex buffer handle and upload your vertex data. Check that the buffer was uploaded correctly.
            // 3) Bind the element buffer handle and upload your element data. Check that the buffer was uploaded correctly.
            
            //Create VBO buffer
            _stride = BlittableValueType.StrideOf(vertices);
            GL.GenBuffers(1, out _vboId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * _stride), vertices,
                          BufferUsageHint.StaticDraw);
            #if DEBUG
            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (vertices.Length * BlittableValueType.StrideOf(vertices) != size)
                throw new ApplicationException("Vertex data not uploaded correctly");
            #endif

            //Create EBO buffer
            GL.GenBuffers(1, out _eboId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(elements.Length * sizeof(short)), elements,
                          BufferUsageHint.StaticDraw);
            #if DEBUG
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (elements.Length * sizeof(short) != size)
                throw new ApplicationException("Element data not uploaded correctly");
            #endif

            _numElements = elements.Length;            
        }

        public void Render()
        {
            // To draw a VBO:
            // 1) Ensure that the VertexArray client state is enabled.
            // 2) Bind the vertex and element buffer handles.
            // 3) Set up the data pointers (vertex, normal, color) according to your vertex format.
            // 4) Call DrawElements. (Note: the last parameter is an offset into the element buffer
            //    and will usually be IntPtr.Zero).

            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboId);

            GL.VertexPointer(3, VertexPointerType.Float, _stride, new IntPtr(0));
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, _stride, new IntPtr(12));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, _stride, new IntPtr(16));


            GL.DrawElements(PrimitiveMode, _numElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            if (GraphicsContext.CurrentContext != null)
            {
                GL.DeleteBuffers(2, ref _vboId);
                GL.DeleteBuffers(2, ref _eboId);
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~Vbo()
        {
            Dispose(false);
        }

    }
}
