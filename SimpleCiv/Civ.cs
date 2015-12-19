using SharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleCiv.world;
using SharpGL.SceneGraph.Assets;
using SharpGL.Shaders;
using GlmNet;
using SimpleCiv.helpers;
using System.Windows.Input;
using System.Diagnostics;
using SharpGL.Enumerations;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace SimpleCiv
{
    public partial class Civ : Form
    {

        private nFMOD.FmodSystem fmod;
        private const nFMOD.SoundMode flags = nFMOD.SoundMode.NonBlocking | nFMOD.SoundMode.SoftwareProcessing;
        private Dictionary<UnitType, nFMOD.Sound> attackSounds;
        private Dictionary<UnitType, nFMOD.Sound> moveSounds;


        //private const float MaxAngle = 0.523599f;
        private const float MaxAngle = 1.223599f;

        private World world;
       // private int tileTextureUnit;
        private bool setup = false;
        private bool loaded = false;
        private float zoomV = 0.0f;
        private float zoom = 2;

        private float rotateUnit = 0.0f;
        private mat4 projectionMatrix;
        private mat4 viewMatrix;
        private vec3 vel = new vec3(0, 0, 0);
        private vec3 pos = new vec3(5f, 2, 5.0f);
        private long lastFrameTicks = 0;
        private Thread loadThread;

        // tiles
        private Tile startTile = null;
        private ShaderProgram borderProgram;
        private ShaderProgram lineProgram;
        private ShaderProgram tileProgram;
        private TileGeometry tileGeometry;
        private GridGeometry gridGeometry;
        private BorderGeometry borderGeometry;
        public Dictionary<TileType, Texture> tileTextures = new Dictionary<TileType, Texture>();
        public Dictionary<TileType, string> tileTextureNames = new Dictionary<TileType, string>();

        // units
        private ShaderProgram unitProgram;
        public Dictionary<UnitType, UnitGeometry> unitGeometries = new Dictionary<UnitType, UnitGeometry>();
        //public Dictionary<UnitType, string> unitFileNames = new Dictionary<UnitType, string>();

        public Dictionary<UnitType, UnitDetail> unitTypes = new Dictionary<UnitType, UnitDetail>();

        public List<Player> players;


        public async void loaderThreadFunc()
        {
            await tileGeometry.Load();
            await gridGeometry.Load();
            await borderGeometry.Load();

            int cur = 1;
            int total = unitTypes.Count();

            var opts = new ParallelOptions
            {
                MaxDegreeOfParallelism = 6,
            };

            this.Invoke((MethodInvoker)delegate
            {
                elementHost2.Visible = false;
                elementHost1.Visible = false;
                mainMenu1.LoadProgress.Width = 0;
            });

            Parallel.ForEach(unitTypes, opts, (unit) =>
            {
                 var unitGeometry = new UnitGeometry(unit.Value);
                 unitGeometry.Load().Wait();
                 unitGeometries[unit.Key] = unitGeometry;
                 Debug.WriteLine("Loaded " + unit);

                 this.Invoke((MethodInvoker)delegate
                 {
                     var progress = (int)(300 * (cur / (total * 1.0f)));
                     mainMenu1.LoadProgress.Width = progress;
                     cur++;
                 });
             });
            this.Invoke((MethodInvoker)delegate
            {
                elementHost3.Visible = false;
                elementHost2.Visible = true;
                elementHost1.Visible = true;
            });         
        
            loaded = true;
        }

        public Civ()
        {
            lastFrameTicks = System.Environment.TickCount; // / TimeSpan.TicksPerMillisecond;
            InitializeComponent();
            this.topMenu1.CapitalName.Text = "Hey";
            fmod = new nFMOD.FmodSystem();
            fmod.Init(32, nFMOD.InitFlags.SoftwareHRTF);
            //fmod.SetDspBufferSize(256, 2);
            attackSounds = new Dictionary<UnitType, nFMOD.Sound>();
            moveSounds = new Dictionary<UnitType, nFMOD.Sound>();

        }

        private void Civ_Load(object sender, EventArgs e)
        {


            var gl = overlayControl.OpenGL;

            while (true)
            {

                var error = gl.GetError();
                if (error == OpenGL.GL_NO_ERROR)
                {
                    break;
                }
               // Debug.WriteLine("" + gl.GetErrorDescription(error));
            }

            int worldSize = 100;
            world = new World(worldSize, worldSize);
            tileGeometry = new TileGeometry();
            gridGeometry = new GridGeometry();
            borderGeometry = new BorderGeometry();

            players = new List<Player>();
            var player1 = new Player();
            player1.name = "Germany";
            player1.borderColor = System.Drawing.Color.Crimson;
            players.Add(player1);

            var player2 = new Player();
            player2.name = "England";
            player2.borderColor = System.Drawing.Color.DarkBlue;
            players.Add(player2);
            
            world.tiles[4, 3].owner = player2;
            world.tiles[4, 4].owner = player2;
            world.tiles[4, 5].owner = player2;
            world.tiles[4, 6].owner = player2;
            world.tiles[5, 3].owner = player2;
            world.tiles[5, 4].owner = player2;
            world.tiles[5, 5].owner = player2;
            world.tiles[5, 6].owner = player2;
            world.tiles[7, 5].owner = player1;
      
            world.UpdateBorders();
            tileTextureNames.Add(TileType.Grass, "grass.bmp");
            tileTextureNames.Add(TileType.Water, "water.bmp");
            tileTextureNames.Add(TileType.Desert, "desert.bmp");
            tileTextureNames.Add(TileType.DeepWater, "deepwater.bmp");
            tileTextureNames.Add(TileType.Lava, "lava.bmp");
            tileTextureNames.Add(TileType.Mountain, "mountain.bmp");

            var unitsConfig = FileLoader.LoadTextFile("assets/config/units.json");
            var unitTypeList = JsonConvert.DeserializeObject<List<UnitDetail>>(unitsConfig);

            int unitLimit = 0;
            int unitIndex = 0;
            foreach(var unit in unitTypeList)
            {
                if(unitLimit != 0 && unitIndex > unitLimit)
                {
                    break;
                }
                if (unit.moveSound != null)
                {
                    moveSounds.Add(unit.name, fmod.CreateSound("assets/sounds/" + unit.moveSound, flags));
                }
                if (unit.attackSound != null)
                {
                    attackSounds.Add(unit.name, fmod.CreateSound("assets/sounds/" + unit.attackSound, flags));
                }
                unitTypes.Add(unit.name, unit);
                unitIndex++;
            }                            
         
            var aa = 0;
            var yy = 0;
            foreach (var unit in unitTypes)
            {
                var curUnit = new Unit();
                curUnit.currentType = unit.Key;
                curUnit.currentTile = world.tiles[2+aa, yy + 2];
                curUnit.currentTile.currentUnit = curUnit;
                aa++;
                if(aa > 5)
                {
                    yy++;
                    aa = 0;
                }
            }

            loadThread = new Thread(new ThreadStart(loaderThreadFunc));
            loadThread.Start();

            tileProgram = new ShaderProgram();
            tileProgram.Create(gl, FileLoader.LoadTextFile("assets/shaders/tileShader.vert"), FileLoader.LoadTextFile("assets/shaders/tileShader.frag"), null);
            tileProgram.BindAttributeLocation(gl, 0, "in_Position");
            tileProgram.AssertValid(gl);
            //tileTextureUnit = gl.GetUniformLocation(tileProgram.ShaderProgramObject, "myTexture");


            lineProgram = new ShaderProgram();
            lineProgram.Create(gl, FileLoader.LoadTextFile("assets/shaders/lineShader.vert"), FileLoader.LoadTextFile("assets/shaders/lineShader.frag"), null);
            lineProgram.BindAttributeLocation(gl, 0, "in_Position");
            lineProgram.AssertValid(gl);

            unitProgram = new ShaderProgram();
            unitProgram.Create(gl, FileLoader.LoadTextFile("assets/shaders/unitShader.vert"), FileLoader.LoadTextFile("assets/shaders/unitShader.frag"), null);
            unitProgram.BindAttributeLocation(gl, 0, "in_Position");
            unitProgram.BindAttributeLocation(gl, 1, "in_Normal");            
            unitProgram.AssertValid(gl);


            borderProgram = new ShaderProgram();
            borderProgram.Create(gl, FileLoader.LoadTextFile("assets/shaders/borderShader.vert"), FileLoader.LoadTextFile("assets/shaders/borderShader.frag"), null);
            borderProgram.BindAttributeLocation(gl, 0, "in_Position");
            borderProgram.AssertValid(gl);

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(BlendingSourceFactor.SourceAlpha, BlendingDestinationFactor.OneMinusSourceAlpha);

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.DepthRange(0, 2);
            gl.LineWidth(4);

            overlayControl_Resized(null, null);
            overlayControl.OpenGLDraw += overlayControl_Draw;
            overlayControl.Resized += overlayControl_Resized;
            this.MouseWheel += overlayControl_MouseWheel;
            overlayControl.MouseDown += OverlayControl_MouseDown;

            while (true)
            {

                var error = gl.GetError();
                if (error == OpenGL.GL_NO_ERROR)
                {
                    break;
                }
                Debug.WriteLine("Error during opengl setup: " + gl.GetErrorDescription(error));
            }
        }

        private void OverlayControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var gl = overlayControl.OpenGL;
            if (overlayControl == null) { 
                return;
             }
          
            var clientMouse = PointToClient(MousePosition);

            var mouseX = clientMouse.X - overlayControl.Left;
            var mouseY = overlayControl.Height - clientMouse.Y + overlayControl.Top;

            Debug.WriteLine("mouseX" + mouseX);
            Debug.WriteLine("mouseY" + mouseY);

            var myBytes = new byte[4];

            gl.ReadPixels(mouseX, mouseY, 1, 1, OpenGL.GL_DEPTH_COMPONENT, OpenGL.GL_FLOAT, myBytes);
            var mouseZ = System.BitConverter.ToSingle(myBytes, 0);
            Debug.WriteLine("mouseZ" + mouseZ);

            //this.elementHost3.Location = new Point(mouseX, overlayControl.Height - mouseY);

            /*
            var result = glm.unProject(
                                new vec3(mouseX, mouseY, mouseZ),
                                viewMatrix, projectionMatrix,
                                new vec4(0, 0, overlayControl.Width, overlayControl.Height));
                                */
            double curX = 0;
            double curY = 0;
            double curZ = 0;

            gl.UnProject(
                mouseX,
                mouseY, 
                mouseZ,
                    Array.ConvertAll<float, double>(viewMatrix.to_array(), Convert.ToDouble),
                    Array.ConvertAll<float, double>(projectionMatrix.to_array(), Convert.ToDouble),
                    new int[] { 0, 0, (int)overlayControl.Width, (int)overlayControl.Height },
                    ref curX, 
                    ref curY,
                    ref curZ);
            //Debug.WriteLine("glmX" + result.x);
            Debug.WriteLine("-X " + curX);
            // Debug.WriteLine("glmY" + result.y);
            Debug.WriteLine("-Y" + curY);
            //  Debug.WriteLine("glmZ" + result.z);
            Debug.WriteLine("-Z" + curZ);

            int tileX;
            int tileZ;
            Tile.GetTileIndex((float)curX, (float)curZ, out tileX, out tileZ);

            Debug.WriteLine("tile Y " + tileX);
            Debug.WriteLine("tile Z " + tileZ);

            var tileTypeFound = world.GetTile(tileX, tileZ);
            if (tileTypeFound != null)
            {
                var clickedTile = world.tiles[tileX, tileZ];
                
                if (MouseButtons.Left == MouseButtons)
                {
                    clickedTile.owner = players[0];
                }
                if (MouseButtons.Right == MouseButtons)
                {
                    clickedTile.owner = null;
                }
                world.UpdateBorders();

                return;
                
                if (MouseButtons.Left == MouseButtons)
                {
                    /* foreach(var neighbors in clickedTile.neighbors)
                     {
                         neighbors.tile.currentType = TileType.Lava;
                     }*/


                    if (startTile == null)
                    {
                        if (clickedTile.currentUnit != null)
                         {
                        // topMenu1.SelectedUnit.Text = unitFileNames[clickedTile.currentUnit.currentType];
                        // clickedTile.currentType = TileType.Lava;
                          startTile = clickedTile;


                        }
                        //clickedTile.currentType = TileType.Lava;

                    }
                    else
                    {
                        /*
                        var impassable = new List<TileType>()
                        {
                            TileType.DeepWater,
                            TileType.Water
                        };

                        var pathway = world.FindShortestClearPath(startTile, clickedTile, impassable);
                        if (pathway != null)
                        {
                            foreach (var pathitem in pathway)
                            {
                                pathitem.currentType = TileType.Mountain;
                            }
                            clickedTile.currentType = TileType.Lava;
                        }*/


                        if (startTile.currentUnit != null)
                        {
                            if (startTile != clickedTile)
                            {
                                if (startTile.neighbors.Any(x => x.tile == clickedTile) && clickedTile.currentUnit != null)
                                {
                                    var attacker = startTile.currentUnit;
                                    var defender = clickedTile.currentUnit;

                                    if (attackSounds.ContainsKey(attacker.currentType)){
                                        fmod.PlaySound(attackSounds[attacker.currentType]);
                                    }

                                    Unit.EvaluateCombat(attacker, defender);

                                    if (defender.health <= 0)
                                    {
                                        clickedTile.currentUnit = null;
                                        // make function in unit to remove from players unit set and other information.
                                        if (attacker.health > 0)
                                        {
                                            clickedTile.currentUnit = startTile.currentUnit;
                                            clickedTile.currentUnit.currentTile = clickedTile;
                                            startTile.currentUnit = null;
                                        }
                                    }

                                    if (attacker.health <= 0)
                                    {
                                        startTile.currentUnit = null;
                                    }
                                }
                                else
                                {
                                    if (clickedTile.currentUnit == null)
                                    {
                                        clickedTile.currentUnit = startTile.currentUnit;
                                        clickedTile.currentUnit.currentTile = clickedTile;
                                        startTile.currentUnit = null;
                                        if (moveSounds.ContainsKey(clickedTile.currentUnit.currentType)){
                                            fmod.PlaySound(moveSounds[clickedTile.currentUnit.currentType]);
                                        }
                                    }
                                }
                            }
                        }

                        startTile = null;
                    }


            }
        }
        }

        void overlayControl_Resized(object sender, EventArgs e)
        {
            projectionMatrix = glm.perspective(0.785398f, (float)overlayControl.Width / (float)overlayControl.Height, 0, 50);
            var inversea = glm.inverse(projectionMatrix);
        }

        public void Setup(OpenGL gl)
        {
            foreach (var curTileType in tileTextureNames)
            {
                var curTexture = new Texture();
                curTexture.Create(gl, "assets/textures/" + curTileType.Value);
                tileTextures[curTileType.Key] = curTexture;
            }
            borderGeometry.Setup(gl);
            tileGeometry.Setup(gl);
            gridGeometry.Setup(gl);

            foreach(var unitGeometry in unitGeometries)
            {
                unitGeometry.Value.Setup(gl);
            }

            setup = true;

           
            /*
            DateTime.
            RenderTargetBitmap rtb = new RenderTargetBitmap( (int)topMenu1.RenderSize.Width, (int)topMenu1.RenderSize.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(topMenu1);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            using (Stream stm = File.Create("new.png"))
            {
                png.Save(stm);
            }*/
        }

        private void overlayControl_Draw(object sender, RenderEventArgs args)
        {
            var gl = overlayControl.OpenGL;

            if (!loaded)
                return;

            if (tileProgram == null || lineProgram == null || unitProgram == null)
                return;

            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            var theta = 0.785398f;
            var yOffset = (float)(zoom * Math.Sin(theta));
            var zOffset = (float)(zoom * Math.Cos(theta));

            viewMatrix = glm.lookAt(new vec3(pos.x, yOffset, pos.z + zOffset), new vec3(pos.x, 0, pos.z), new vec3(0, 1, 0));

            tileProgram.Bind(gl);
            tileProgram.SetUniformMatrix4(gl, "projectionMatrix", projectionMatrix.to_array());
            tileProgram.SetUniformMatrix4(gl, "viewMatrix", viewMatrix.to_array());

            if (!setup)
            {

                Setup(gl);
            }

            
           var delta = ProcessInput(gl);
            rotateUnit += delta / 700.0f;

            if(rotateUnit > Math.PI * 2)
            {
                rotateUnit =  (float)(rotateUnit % (Math.PI * 2));
            }


            foreach (var curTileType in tileTextures)
            {
             //  uint texUnit = 0;

               // gl.Uniform1(tileTextureUnit, texUnit);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, curTileType.Value.TextureName);
               // gl.ActiveTexture(texUnit);

                for (int i = 0; i < world.xSize; i++)
                {
                    for (int j = 0; j < world.zSize; j++)
                    {
                        var curTile = world.tiles[i, j];

                        if (Math.Abs(curTile.centerPos.z - pos.z) < 16 && Math.Abs(curTile.centerPos.x - pos.x) < 16)
                        {
                            if (curTile.currentType == curTileType.Key)
                            {
                                var model = glm.translate(mat4.identity(), new vec3(curTile.centerPos.x, 0,curTile.centerPos.z));
                                tileProgram.SetUniformMatrix4(gl, "modelMatrix", model.to_array());
                                tileGeometry.Draw(gl);
                            }
                        }
                        
                    }
                }
            }
            tileProgram.Unbind(gl);

            
            lineProgram.Bind(gl);
            lineProgram.SetUniformMatrix4(gl, "projectionMatrix", projectionMatrix.to_array());
            lineProgram.SetUniformMatrix4(gl, "viewMatrix", viewMatrix.to_array());
            lineProgram.SetUniform3(gl, "lineColor", 1.0f, 1.0f, 1.0f);
            lineProgram.SetUniform1(gl, "lineAlpha", 0.2f);

            for (int i = 0; i < world.xSize; i++)
            {
                for (int j = 0; j < world.zSize; j++)
                {
                    var curTile = world.tiles[i, j];
                    if (Math.Abs(curTile.centerPos.z - pos.z) < 16 && Math.Abs(curTile.centerPos.x - pos.x) < 16)
                    {
                        var model = glm.translate(mat4.identity(), new vec3(curTile.centerPos.x, 0.001f, curTile.centerPos.z));
                        lineProgram.SetUniformMatrix4(gl, "modelMatrix", model.to_array());
                        gridGeometry.Draw(gl);
                    }
                }
            }
          
            if(startTile != null && startTile.currentUnit != null)
            {
                lineProgram.SetUniform3(gl, "lineColor", 0.0f, 204.0f/255.0f, 1.0f);
                lineProgram.SetUniform1(gl, "lineAlpha", 1.0f);
                var activeUnitModel = glm.translate(mat4.identity(), new vec3(startTile.centerPos.x, 0.002f, startTile.centerPos.z));
                var scaled = glm.scale(activeUnitModel, new vec3(0.9f, 0.9f, 0.9f));
                lineProgram.SetUniformMatrix4(gl, "modelMatrix", activeUnitModel.to_array());
                gridGeometry.Draw(gl);
            }

            lineProgram.Unbind(gl);
            unitProgram.Bind(gl);
            unitProgram.SetUniformMatrix4(gl, "projectionMatrix", projectionMatrix.to_array());
            unitProgram.SetUniformMatrix4(gl, "viewMatrix", viewMatrix.to_array());

            for (int i = 0; i < world.xSize; i++)
            {
                if (Math.Abs(i - pos.x) > 16)
                {
                    continue;
                }

                for (int j = 0; j < world.zSize; j++)
                {
                    if (Math.Abs(j - pos.z) > 16)
                    {
                        continue;
                    }

                    var curTile = world.tiles[i, j];
                    if (curTile.currentUnit != null)
                    {
                        unitGeometries[curTile.currentUnit.currentType].Draw(gl, unitProgram, curTile.centerPos);

                        if (!unitTypes[curTile.currentUnit.currentType].singleUnit)
                        {
                            const float offset = 0.3f;
                            unitGeometries[curTile.currentUnit.currentType].Draw(gl, unitProgram, curTile.centerPos + new vec3(offset, 0, offset));
                            unitGeometries[curTile.currentUnit.currentType].Draw(gl, unitProgram, curTile.centerPos + new vec3(-offset, 0, offset));
                            unitGeometries[curTile.currentUnit.currentType].Draw(gl, unitProgram, curTile.centerPos + new vec3(-offset, 0, -offset));
                            unitGeometries[curTile.currentUnit.currentType].Draw(gl, unitProgram, curTile.centerPos + new vec3(offset, 0, -offset));
                        }                
                    }
                }
            }       
            unitProgram.Unbind(gl);

           
            borderProgram.Bind(gl);
            borderProgram.SetUniformMatrix4(gl, "projectionMatrix", projectionMatrix.to_array());
            borderProgram.SetUniformMatrix4(gl, "viewMatrix", viewMatrix.to_array());

            var bordersGrouped = world.borders.GroupBy(x => x.type);
            foreach (var borderGroup in bordersGrouped)
            {
                vec3 S0 = new vec3(0, 0, 0);
                vec3 S1 = new vec3(0, 0, 0);
              
                switch (borderGroup.Key)
                {
                    case BorderType.DownRight:
                        S0 = borderGeometry.v1;
                        S1 = borderGeometry.v2;
                        break;
                    case BorderType.DownLeft:
                        S0 = borderGeometry.v2;
                        S1 = borderGeometry.v3;
                        break;
                    case BorderType.Left:
                        S0 = borderGeometry.v3;
                        S1 = borderGeometry.v4;
                        break;
                    case BorderType.UpLeft:
                        S0 = borderGeometry.v4;
                        S1 = borderGeometry.v5;
                        break;
                    case BorderType.UpRight:
                        S0 = borderGeometry.v5;
                        S1 = borderGeometry.v6;
                        break;
                    case BorderType.Right:
                        S0 = borderGeometry.v6;
                        S1 = borderGeometry.v1;
                        break;
                    default:
                        continue;
                }
                borderProgram.SetUniform3(gl, "S0", S0.x, 0.003f, S0.z);
                borderProgram.SetUniform3(gl, "S1", S1.x, 0.003f, S1.z);

                foreach (var border in borderGroup)
                {
                    var borderModel = glm.translate(mat4.identity(), new vec3(border.tile.centerPos.x, 0.003f, border.tile.centerPos.z));
                    borderProgram.SetUniformMatrix4(gl, "modelMatrix", borderModel.to_array());
                    borderProgram.SetUniform3(gl, "borderColor", border.tile.owner.borderColor.R / 255.0f, border.tile.owner.borderColor.G / 255.0f, border.tile.owner.borderColor.B / 255.0f);
                    borderGeometry.Draw(gl,Convert.ToInt32( borderGroup.Key));                   
                }
            }
            borderProgram.Unbind(gl);

            double[] curX = new double[] { 0.0 };
            double[] curY = new double[] { 0.0 };
            double[] curZ = new double[] { 0.0 };

            /* gl.Project(5, 0, 5, Array.ConvertAll<float, double>(viewMatrix.to_array(), Convert.ToDouble),
                     Array.ConvertAll<float, double>(projectionMatrix.to_array(), Convert.ToDouble), new int[] { 0, 0, (int)overlayControl.Width, (int)overlayControl.Height },
                     curX,
                     curY,
                     curZ);*/

            //this.elementHost3.Location = new Point((int)curX[0], overlayControl.Height - (int)curY[0]);

            fmod.Update();

        }

        private float ProcessInput(OpenGL gl)
        {
            var curFrameTicks = System.Environment.TickCount;
            var delta = (curFrameTicks - lastFrameTicks);

            if (Form.ActiveForm == null)
                return delta;

            if (Keyboard.IsKeyDown(Key.Escape))
                Application.Exit();

            if (vel.x > 0)
                vel.x = Math.Max(0, vel.x - (delta / 100.0f));
            else
                vel.x = Math.Min(0, vel.x + (delta / 100.0f));

            if (vel.z > 0)
                vel.z = Math.Max(0, vel.z - (delta / 100.0f));
            else
                vel.z = Math.Min(0, vel.z + (delta / 100.0f));

            zoom += zoomV;
            zoom = Math.Min(9, zoom);
            zoom = Math.Max(2, zoom);

            if (zoomV > 0)
                zoomV = Math.Max(0, zoomV - (delta / 500.0f));
            else
                zoomV = Math.Min(0, zoomV + (delta / 500.0f));
                                         
            /*
            if (overlayControl.IsMouseOver)
            {
                var point = Mouse.GetPosition(overlayControl);
                var margin = 30;

                if (point.X < margin)
                    vel.x = -1;
                else if (point.X > (overlayControl.ActualWidth - margin))
                    vel.x = 1;

                if (point.Y < margin)
                    vel.z = -1;
                else if (point.Y > (overlayControl.ActualHeight - margin))
                    vel.z = 1;
            }*/

            if (Keyboard.IsKeyDown(Key.Left))
                vel.x = -1;
            if (Keyboard.IsKeyDown(Key.Right))
                vel.x = 1;
            if (Keyboard.IsKeyDown(Key.Up))
                vel.z = -1;
            if (Keyboard.IsKeyDown(Key.Down))
                vel.z = 1;

            pos.z += (vel.z / 200.0f) * delta;
            pos.x += (vel.x / 200.0f) * delta;

            lastFrameTicks = curFrameTicks;

            return delta;
        }

        void overlayControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            zoomV = -(e.Delta / 200.0f);
        }

        private void elementHost3_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }
    }
}
