using SharpGL;
using SharpGL.VertexBuffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{
    public abstract class Renderable
    {
        public float[] vertexArray;
        public float[] colorArray;
        public float[] normalArray;

        public VertexBufferArray vertexBufferArray;
        public bool isSetup = false;
        public bool didFail = false;

        public void Setup(OpenGL gl)
        {
            this.vertexBufferArray = new VertexBufferArray();
            this.vertexBufferArray.Create(gl);
            this.vertexBufferArray.Bind(gl);

            uint bufferIndex = 0;

            var vertexDataBuffer = new VertexBuffer();
            vertexDataBuffer.Create(gl);
            vertexDataBuffer.Bind(gl);
            vertexDataBuffer.SetData(gl, bufferIndex, this.vertexArray, false, 3);
            bufferIndex++;

            if (this.colorArray != null)
            {
                var vertexDataBuffer2 = new VertexBuffer();
                vertexDataBuffer2.Create(gl);
                vertexDataBuffer2.Bind(gl);
                vertexDataBuffer2.SetData(gl, bufferIndex, this.colorArray, false, 4);
                bufferIndex++;
            }

            if (this.normalArray != null)
            {
                var vertexDataBuffer3 = new VertexBuffer();
                vertexDataBuffer3.Create(gl);
                vertexDataBuffer3.Bind(gl);
                vertexDataBuffer3.SetData(gl, bufferIndex, this.normalArray, false, 3);
                bufferIndex++;
            }

            this.vertexBufferArray.Unbind(gl);
            //  Debug.WriteLine("used " + (this.vertexArray.Length * 3 + this.texArray.Length * 4) / 1000000 + " MB");
            this.vertexArray = null;
            this.colorArray = null;
            this.isSetup = true;
        }

        public void Teardown(OpenGL gl)
        {
            if (this.didFail)
            {
                return;
            }
            if (this.isSetup)
            {
                this.vertexBufferArray.Delete(gl);
                this.isSetup = false;
            }
        }

        public virtual async Task<bool> Load()
        {
            return true;
        }
    }
}
