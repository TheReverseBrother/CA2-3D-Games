/*
Function: 		This child class for drawing primitives where the vertex data is buffered on the GFX card in VRAM
                and the number of vertices required is reduced through the use of an index buffer.
                Note: 
                - The class is generic and can be used to draw VertexPositionColor, VertexPositionColorTexture, VertexPositionColorNormal types etc.
                - For each draw the GFX card refers to vertex and index data that has already been buffered to VRAM 
                - This is a more efficient approach than either using the VertexData, BufferedVertexData, or DynamicBufferedVertexData classes if
                  you wish to draw a large number of primitives on screen.

Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
See:            http://rbwhitaker.wikidot.com/index-and-vertex-buffers
*/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class BufferedIndexedVertexData<T> : VertexData<T> where T : struct, IVertexType
    {
        #region Variables
        private VertexBuffer vertexBuffer;
        private GraphicsDevice graphicsDevice;
        private IndexBuffer indexBuffer;
        private short[] indices;
        #endregion

        #region Properties
        public IndexBuffer IndexBuffer
        {
            get
            {
                return indexBuffer;
            }
            set
            {
                indexBuffer = value;

            }
        }
        public VertexBuffer VertexBuffer
        {
            get
            {
                return vertexBuffer;
            }
            set
            {
                vertexBuffer = value;

            }
        }
        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return this.graphicsDevice;
            }
        }
        #endregion

        //allows developer to pass in vertices AND buffer - more efficient since buffer is defined ONCE outside of the object instead of a new VertexBuffer for EACH instance of the class
        public BufferedIndexedVertexData(GraphicsDevice graphicsDevice, T[] vertices, 
            VertexBuffer vertexBuffer, short[] indices, PrimitiveType primitiveType, int primitiveCount)
            : base(vertices, primitiveType, primitiveCount)
        {
            this.graphicsDevice = graphicsDevice;
            this.VertexBuffer = vertexBuffer;
 
            //set vertex and index buffer data on the reserved space
            this.vertexBuffer.SetData<T>(this.Vertices);
            this.indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            this.indexBuffer.SetData(indices);
        }

        //buffer is created INSIDE the class so each class has a buffer - not efficient
        public BufferedIndexedVertexData(GraphicsDevice graphicsDevice, T[] vertices, short[] indices, PrimitiveType primitiveType, int primitiveCount)
            : base(vertices, primitiveType, primitiveCount)
        {
            this.graphicsDevice = graphicsDevice;
            this.VertexBuffer = new VertexBuffer(graphicsDevice, typeof(T), vertices.Length, BufferUsage.None);

            //set vertex and index buffer data on the reserved space
            this.vertexBuffer.SetData<T>(this.Vertices);
            this.indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            this.indexBuffer.SetData(indices);
        }


        public void SetData(T[] vertices, short[] indices)
        {
            this.Vertices = vertices;
            if(this.vertexBuffer == null)
                this.VertexBuffer = new VertexBuffer(graphicsDevice, typeof(T), vertices.Length, BufferUsage.None);

            this.vertexBuffer.SetData<T>(this.Vertices);

            if(this.indexBuffer == null)
                this.indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);

            this.indexBuffer.SetData(indices);
        }

        public override void Draw(GameTime gameTime, Effect effect)
        {
            //set vertices
            effect.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);
            //set indices
            effect.GraphicsDevice.Indices = this.indexBuffer;

            //draw!
            effect.GraphicsDevice.DrawIndexedPrimitives(this.PrimitiveType, 0, 0, this.Vertices.Length, 0, this.PrimitiveCount);
        }

        public new object Clone()
        {
            return new BufferedIndexedVertexData<T>(this.graphicsDevice,  //shallow - reference
                this.Vertices, //shallow - reference
                this.indices,
                this.PrimitiveType, //struct - deep
                this.PrimitiveCount); //deep - primitive
        }
    }
}
