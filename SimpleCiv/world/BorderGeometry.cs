using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{
    public class BorderGeometry : Renderable 
    {
        static float val = 0.1f;
 
        public BorderGeometry()
        {
        }

        public override async Task<bool> Load()
        {
            await Task.Delay(2);
            var y = 0.005f;
            //var topZ = 0.1f;
            //var bottomZ = 0.9f;
            //var leftX = 0.1f;
            //var rightX = 0.9f;
            this.vertexArray = new float[3 * 6];

            var baseVertex = 0;

            this.vertexArray[baseVertex + 0] = 1;   //
            this.vertexArray[baseVertex + 1] = y;
            this.vertexArray[baseVertex + 2] = 0;

            this.vertexArray[baseVertex + 3] = 0;
            this.vertexArray[baseVertex + 4] = y;
            this.vertexArray[baseVertex + 5] = 0;

            this.vertexArray[baseVertex + 6] = 0;
            this.vertexArray[baseVertex + 7] = y;
            this.vertexArray[baseVertex + 8] = 1;

            this.vertexArray[baseVertex + 9] = 1;      //
            this.vertexArray[baseVertex + 10] = y;
            this.vertexArray[baseVertex + 11] = 0;

            this.vertexArray[baseVertex + 12] = 0;
            this.vertexArray[baseVertex + 13] = y;
            this.vertexArray[baseVertex + 14] = 1;

            this.vertexArray[baseVertex + 15] = 1;
            this.vertexArray[baseVertex + 16] = y;
            this.vertexArray[baseVertex + 17] = 1;
            return true;
        }
        public void Draw(OpenGL gl)
        {
            this.vertexBufferArray.Bind(gl);
            gl.DrawArrays(OpenGL.GL_TRIANGLES,0, 6);
            this.vertexBufferArray.Unbind(gl);
        }
    }
}
