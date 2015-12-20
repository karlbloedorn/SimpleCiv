using GlmNet;
using SharpGL;
using SimpleCiv.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{
    public class GridGeometry : Renderable
    {
        const int numLines = 6;
        const int verticesPerLine = 2;

        private vec3 v0, v1, v2, v3, v4, v5, v6;

        private vec3 getVertex(int index)
        {
            var angle_deg = 60 * index + 30.0f;
            var angle_rad = (Math.PI / 180.0) * angle_deg;
            return new vec3((float)Math.Cos(angle_rad), 0.0f, (float)Math.Sin(angle_rad));
        }
        private void SetLine(int lineIndex, vec3 va, vec3 vb)
        {
            var baseVertex = lineIndex * 6;
            this.vertexArray[baseVertex + 0] = va.x;
            this.vertexArray[baseVertex + 1] = va.y;
            this.vertexArray[baseVertex + 2] = va.z;
            this.vertexArray[baseVertex + 3] = vb.x;
            this.vertexArray[baseVertex + 4] = vb.y;
            this.vertexArray[baseVertex + 5] = vb.z;
        }
        public override async Task<bool> Load()
        {
            await Task.Delay(2);

            this.vertexArray = new float[numLines * verticesPerLine * 3];

            v0 = new vec3(0, 0, 0);
            v1 = getVertex(0);
            v2 = getVertex(1);
            v3 = getVertex(2);
            v4 = getVertex(3);
            v5 = getVertex(4);
            v6 = getVertex(5);

            SetLine(0, v1, v2);
            SetLine(1, v2, v3);
            SetLine(2, v3, v4);
            SetLine(3, v4, v5);
            SetLine(4, v5, v6);
            SetLine(5, v6, v1);
            return true;
        }

        public void Draw(OpenGL gl)
        {
            gl.DrawArrays(OpenGL.GL_LINES, 0, numLines * verticesPerLine);
        }
    }
}
