using FileFormatWavefront;
using GlmNet;
using SharpGL;
using SharpGL.Shaders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{

    public class MaterialGroup
    {
        public int offset;
        public int count;
        public vec3 ambient;
        public vec3 diffuse;
        public vec3 specular;
        public float shinyness;
    }

    public class UnitGeometry : Renderable
    {
        private UnitDetail details;

        public UnitGeometry(UnitDetail unit)
        {
            details = unit;
        }

        private List<MaterialGroup> groups;

        public override async Task<bool> Load()
        {
            groups = new List<MaterialGroup>();
            await Task.Delay(2);

            bool loadTextureImages = true;
            var result = FileFormatObj.Load("assets/models/"+ details.objectFile, loadTextureImages);

            var facesCount = result.Model.Groups.Sum(x => x.Faces.Count());

            this.vertexArray = new float[facesCount * 3 * 3];  // 3 vertices per face,  xyz for each vertex
            this.normalArray = new float[facesCount * 3 * 3];  // 3 vertices per face,  xyz for each vertex

            int offset = 0;

            foreach (var grp in result.Model.Groups)
            {
                var grpName = grp.Names.FirstOrDefault(); 
                if(grpName == null)
                {
                    continue;
                }

                var matchingModel = result.Model.Materials.FirstOrDefault(x => grpName.EndsWith(x.Name));
                if(matchingModel == null)
                {
                    continue;
                }
                var renderGroup = new MaterialGroup();
                renderGroup.offset = offset;
                renderGroup.count = grp.Faces.Count();
                renderGroup.ambient = new vec3(matchingModel.Ambient.r, matchingModel.Ambient.g, matchingModel.Ambient.b);
                renderGroup.diffuse = new vec3(matchingModel.Diffuse.r, matchingModel.Diffuse.g, matchingModel.Diffuse.b);
                renderGroup.specular = new vec3(matchingModel.Specular.r, matchingModel.Specular.g, matchingModel.Specular.b);
                renderGroup.shinyness = matchingModel.Shininess;

                var faces = grp.Faces;

                for (int i = 0; i < faces.Count(); i++)
                {
                    var baseVertex = (offset +  i) * 3 * 3;
                    var v0i = faces[i].Indices[0].vertex;
                    var v1i = faces[i].Indices[1].vertex;
                    var v2i = faces[i].Indices[2].vertex;

                    var n0i = (int)faces[i].Indices[0].normal;
                    var n1i = (int)faces[i].Indices[1].normal;
                    var n2i = (int)faces[i].Indices[2].normal;

                    var v0 = result.Model.Vertices[v0i];
                    var v1 = result.Model.Vertices[v1i];
                    var v2 = result.Model.Vertices[v2i];

                    var n0 = result.Model.Normals[n0i];
                    var n1 = result.Model.Normals[n1i];
                    var n2 = result.Model.Normals[n2i];

                    this.vertexArray[baseVertex + 0] = v0.x;
                    this.vertexArray[baseVertex + 1] = v0.y;
                    this.vertexArray[baseVertex + 2] = v0.z;
                    this.vertexArray[baseVertex + 3] = v1.x;
                    this.vertexArray[baseVertex + 4] = v1.y;
                    this.vertexArray[baseVertex + 5] = v1.z;
                    this.vertexArray[baseVertex + 6] = v2.x;
                    this.vertexArray[baseVertex + 7] = v2.y;
                    this.vertexArray[baseVertex + 8] = v2.z;

                    this.normalArray[baseVertex + 0] = n0.x;
                    this.normalArray[baseVertex + 1] = n0.y;
                    this.normalArray[baseVertex + 2] = n0.z;
                    this.normalArray[baseVertex + 3] = n1.x;
                    this.normalArray[baseVertex + 4] = n1.y;
                    this.normalArray[baseVertex + 5] = n1.z;
                    this.normalArray[baseVertex + 6] = n2.x;
                    this.normalArray[baseVertex + 7] = n2.y;
                    this.normalArray[baseVertex + 8] = n2.z;
                }

                offset += renderGroup.count;
                groups.Add(renderGroup);
            }     

            float minX = 99999999;
            float maxX = -99999999;
            float minY = 99999999;
            float maxY = -99999999;
            float minZ = 99999999;
            float maxZ = -99999999;

            for(int i =0; i < facesCount * 3; i++)
            {
                var x = this.vertexArray[i * 3 + 0];
                var y = this.vertexArray[i * 3 + 1];
                var z = this.vertexArray[i * 3 + 2];
                if(x < minX)
                {
                    minX = x;
                }
                if (y < minY)
                {
                    minY = y;
                }
                if (z < minZ)
                {
                    minZ = z;
                }

                if (x > maxX)
                {
                    maxX = x;
                }
                if (y > maxY)
                {
                    maxY = y;
                }
                if (z > maxZ)
                {
                    maxZ = z;
                }
            }

            float sizeX = maxX - minX;
            float sizeZ = maxZ - minZ;
            float sizeY = maxY - minY;

            float translateY = -minY;
            float translateX = -minX - (sizeX / 2.0f);
            float translateZ = -minZ - (sizeZ / 2.0f);

            float scaleAmount = Math.Max(sizeY, Math.Max(sizeX, sizeZ)) / details.scaleSize;

            for (int i = 0; i < facesCount * 3; i++)
            {
                this.vertexArray[i * 3 + 0] = (this.vertexArray[i * 3 + 0] + translateX) / scaleAmount;
                this.vertexArray[i * 3 + 1] = (this.vertexArray[i * 3 + 1] + translateY) / scaleAmount;
                this.vertexArray[i * 3 + 2] = (this.vertexArray[i * 3 + 2] + translateZ) / scaleAmount;
            }
            return true;
        }
        public void Draw(OpenGL gl, ShaderProgram program, vec3 coordinate)
        {
            this.vertexBufferArray.Bind(gl);

            var translated = glm.translate(mat4.identity(), coordinate);
            program.SetUniformMatrix4(gl, "modelMatrix", translated.to_array());

            foreach (var renderGroup in groups)
            {
                program.SetUniform1(gl, "shininess", renderGroup.shinyness);
                program.SetUniform3(gl, "ambientColor", renderGroup.ambient.x, renderGroup.ambient.y, renderGroup.ambient.z);
                program.SetUniform3(gl, "diffuseColor", renderGroup.diffuse.x, renderGroup.diffuse.y, renderGroup.diffuse.z);
                program.SetUniform3(gl, "specColor", renderGroup.specular.x, renderGroup.specular.y, renderGroup.specular.z);
                gl.DrawArrays(OpenGL.GL_TRIANGLES, renderGroup.offset*3, renderGroup.count * 3);
            }


            this.vertexBufferArray.Unbind(gl);
        }
    }
}
