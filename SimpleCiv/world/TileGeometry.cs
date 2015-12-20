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
    public class TileGeometry : Renderable
    {
        const int numTriangles = 6;

        private vec3 v0, v1, v2, v3, v4, v5, v6;

        private vec3 getVertex(int index)
        {
            var angle_deg = 60 * index + 30.0f;
            var angle_rad = (Math.PI / 180.0) * angle_deg;
            return new vec3((float)Math.Cos(angle_rad), 0, (float)Math.Sin(angle_rad));
        }

        private void SetTriangle(int faceIndex, vec3 a, vec3 b, vec3 c)
        {
            var baseVertex = faceIndex * 9;
            this.vertexArray[baseVertex + 0] = a.x;
            this.vertexArray[baseVertex + 1] = a.y;
            this.vertexArray[baseVertex + 2] = a.z;
            this.vertexArray[baseVertex + 3] = b.x;
            this.vertexArray[baseVertex + 4] = b.y;
            this.vertexArray[baseVertex + 5] = b.z;
            this.vertexArray[baseVertex + 6] = c.x;
            this.vertexArray[baseVertex + 7] = c.y;
            this.vertexArray[baseVertex + 8] = c.z;
        }

        public override async Task<bool> Load()
        {
            await Task.Delay(2);
            this.vertexArray = new float[9 * numTriangles];

            v0 = new vec3(0, 0.0f, 0);
            v1 = getVertex(0);
            v2 = getVertex(1);
            v3 = getVertex(2);
            v4 = getVertex(3);
            v5 = getVertex(4);
            v6 = getVertex(5);

            SetTriangle(0, v0, v1, v2);
            SetTriangle(1, v0, v2, v3);
            SetTriangle(2, v0, v3, v4);
            SetTriangle(3, v0, v4, v5);
            SetTriangle(4, v0, v5, v6);
            SetTriangle(5, v0, v6, v1);

            return true;
        }

        public void Draw(OpenGL gl)
        {
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3 * numTriangles);
        }
    }
}

