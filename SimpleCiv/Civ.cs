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
using System.Windows;

namespace SimpleCiv
{
    public partial class Civ : Form
    {
        private class Game
        {
         // todo move properties here.
        }

        private nFMOD.FmodSystem fmod;
        private const nFMOD.SoundMode flags = nFMOD.SoundMode.NonBlocking | nFMOD.SoundMode.SoftwareProcessing;
        private Dictionary<UnitType, nFMOD.Sound> attackSounds;
        private Dictionary<UnitType, nFMOD.Sound> moveSounds;

        private World world;
        private bool setup = false;
        private bool loaded = false;
        private float zoomV = 0.0f;
        private float zoom = 2;

        private mat4 orthoMatrix;
        private mat4 projectionMatrix;
        private mat4 viewMatrix;
        private vec3 vel = new vec3(0, 0, 0);
        private vec3 pos = new vec3(5f, 2, 5.0f);
        private long lastFrameTicks = 0;
        private Thread loadThread;

        // tiles
        private Tile selectedTile = null;
        private Tile hoverTile = null;
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

        public List<Player> players;


        public async void loaderThreadFunc()
        {
            await tileGeometry.Load();
            await gridGeometry.Load();
            await borderGeometry.Load();

            int cur = 1;
            int total = Unit.unitTypes.Count();

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

            Parallel.ForEach(Unit.unitTypes, opts, (unit) =>
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
            viewMatrix = mat4.identity();
            projectionMatrix = mat4.identity();
            this.topMenu1.CapitalName.Text = "Hey";
            fmod = new nFMOD.FmodSystem();
            fmod.Init(32, nFMOD.InitFlags.SoftwareHRTF);
            //fmod.SetDspBufferSize(256, 2);
            attackSounds = new Dictionary<UnitType, nFMOD.Sound>();
            moveSounds = new Dictionary<UnitType, nFMOD.Sound>();

            bottomLeftMenu1.BuildCityButton.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                var curTile = selectedTile;
                if(curTile != null && curTile.city == null && curTile.currentUnit.currentType == UnitType.Settler) // todo check territory
                {
                    var curPlayer = players[0];

                    curTile.currentUnit = null; // Consume the unit.
                    curTile.owner = curPlayer;

                    var city = new City();
                    city.location = curTile;
                    city.name = "Test City";
                    city.player = curPlayer; // todo change to current player.
                    city.Expand();
                    city.Expand();
                    city.Expand();
                    city.Expand();
                    city.Expand();
                    city.Expand();
                    curTile.city = city;

                    world.UpdateBorders();
                }
            };

            bottomLeftMenu1.MoveButton.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                 throw new NotImplementedException();
            };
        }
    
        private void Civ_Load(object sender, EventArgs e)
        {
            var gl = overlayControl.OpenGL;

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

            world.UpdateBorders();
            tileTextureNames.Add(TileType.Grass, "grass.bmp");
            tileTextureNames.Add(TileType.Water, "water.bmp");
            tileTextureNames.Add(TileType.Desert, "desert.bmp");
            tileTextureNames.Add(TileType.DeepWater, "deepwater.bmp");
            tileTextureNames.Add(TileType.Lava, "lava.bmp");
            tileTextureNames.Add(TileType.Mountain, "mountain.bmp");

            var unitsConfig = FileLoader.LoadTextFile("assets/config/units.json");
            var unitTypeList = JsonConvert.DeserializeObject<UnitConfig>(unitsConfig);

            int unitLimit = 1;
            int unitIndex = 0;
            foreach(var unit in unitTypeList.units)
            {
                if(unitLimit != 0 && unitIndex > unitLimit && !(unit.name == UnitType.Settler || unit.name == UnitType.Worker || unit.name == UnitType.Explorer))
                {
                    continue;
                }
                Unit.unitTypes.Add(unit.name, unit);
                unitIndex++;
            }                            
         
            var curUnit = new Unit();
            curUnit.currentType = UnitType.Settler;
            curUnit.currentTile = world.tiles[5, 5];
            curUnit.currentTile.currentUnit = curUnit;
            var curUnit2 = new Unit();
            curUnit2.currentType = UnitType.Explorer;
            curUnit2.currentTile = world.tiles[6, 5];
            curUnit2.currentTile.currentUnit = curUnit2;


            loadThread = new Thread(new ThreadStart(loaderThreadFunc));
            loadThread.Start();

            tileProgram = new ShaderProgram();
            tileProgram.Create(gl, FileLoader.LoadTextFile("assets/shaders/tileShader.vert"), FileLoader.LoadTextFile("assets/shaders/tileShader.frag"), null);
            tileProgram.BindAttributeLocation(gl, 0, "in_Position");
            tileProgram.AssertValid(gl);


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
            this.MouseWheel += overlayControl_MouseWheel;
            overlayControl.MouseDown += OverlayControl_MouseDown;
            overlayControl.MouseMove += OverlayControl_MouseMove;
            overlayControl.Resized += overlayControl_Resized;
        }

        private void overlayControl_Resized(object sender, EventArgs e)
        {
            projectionMatrix = glm.perspective(0.785398f, (float)overlayControl.Width / (float)overlayControl.Height, 0, 50);
            orthoMatrix = glm.ortho(0, overlayControl.Width, overlayControl.Height, 0);
        }

        private void OverlayControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var tileAtMouse = GetTile(MousePosition);
            hoverTile = tileAtMouse;
        }

        private Tile GetTile(System.Drawing.Point p)
        {
            var gl = overlayControl.OpenGL;

            var clientMouse = PointToClient(MousePosition);
            var mouseX = clientMouse.X - overlayControl.Left;
            var mouseY = overlayControl.Height - clientMouse.Y + overlayControl.Top;
            var myBytes = new byte[4];
            gl.ReadPixels(mouseX, mouseY, 1, 1, OpenGL.GL_DEPTH_COMPONENT, OpenGL.GL_FLOAT, myBytes);
            var mouseZ = System.BitConverter.ToSingle(myBytes, 0);

            double curX = 0;
            double curY = 0;
            double curZ = 0;

            gl.UnProject(mouseX, mouseY, mouseZ,
                    Array.ConvertAll<float, double>(viewMatrix.to_array(), Convert.ToDouble),
                    Array.ConvertAll<float, double>(projectionMatrix.to_array(), Convert.ToDouble),
                    new int[] { 0, 0, (int)overlayControl.Width, (int)overlayControl.Height },
                    ref curX, ref curY, ref curZ);

            int tileX;
            int tileZ;
            Tile.GetTileIndex((float)curX, (float)curZ, out tileX, out tileZ);

            var tileTypeFound = world.GetTile(tileX, tileZ);
            return tileTypeFound;
        }

        private void OverlayControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (overlayControl == null) { 
                return;
            }

            var tileAtMouse = GetTile(MousePosition);
            if (tileAtMouse != null)
            {
                var clickedTile = tileAtMouse;

                if (MouseButtons.Left == MouseButtons)
                {
                    if(selectedTile == clickedTile)
                    {
                        selectedTile = null;
                    } else
                    {
                        selectedTile = clickedTile;
                    }

                }
                if (MouseButtons.Right == MouseButtons)
                {
                    if (selectedTile != null && selectedTile != clickedTile)
                    {
                        if (selectedTile.currentUnit != null)
                        {
                            if (selectedTile.neighbors.Any(x => x.tile == clickedTile) && clickedTile.currentUnit != null)
                            {
                                var attacker = selectedTile.currentUnit;
                                var defender = clickedTile.currentUnit;

                                if (attackSounds.ContainsKey(attacker.currentType))
                                {
                                    fmod.PlaySound(attackSounds[attacker.currentType]);
                                }

                                Unit.EvaluateCombat(attacker, defender);

                                if (defender.health <= 0)
                                {
                                    clickedTile.currentUnit = null;
                                    // make function in unit to remove from players unit set and other information.
                                    if (attacker.health > 0)
                                    {
                                        clickedTile.currentUnit = selectedTile.currentUnit;
                                        clickedTile.currentUnit.currentTile = clickedTile;
                                        selectedTile.currentUnit = null;
                                    }
                                }

                                if (attacker.health <= 0)
                                {
                                    selectedTile.currentUnit = null;
                                }
                            }
                            else
                            {
                                if (clickedTile.currentUnit == null)
                                {
                                    clickedTile.currentUnit = selectedTile.currentUnit;
                                    clickedTile.currentUnit.currentTile = clickedTile;
                                    selectedTile.currentUnit = null;
                                    if (moveSounds.ContainsKey(clickedTile.currentUnit.currentType))
                                    {
                                        fmod.PlaySound(moveSounds[clickedTile.currentUnit.currentType]);
                                    }
                                }
                            }
                        }
                    }

                    selectedTile = null;

                }
            }

            /*
           var pathway = world.FindShortestClearPath(startTile, clickedTile, impassable);
           if (pathway != null)
           {
               foreach (var pathitem in pathway)
               {
                   pathitem.currentType = TileType.Mountain;
               }
               clickedTile.currentType = TileType.Lava;
           }*/
            UpdateBottomLeftMenu();
        }

        private void UpdateBottomLeftMenu()
        {
            bottomLeftMenu1.BuildCityButtonContainer.Visibility = Visibility.Collapsed;
            bottomLeftMenu1.MoveButtonContainer.Visibility = Visibility.Collapsed;

            if (selectedTile != null && selectedTile.currentUnit != null)
            {
                var curTile = selectedTile;
                var curUnit = curTile.currentUnit;
                blMenuContainer.Visible = true;
                bottomLeftMenu1.UnitType.Text = curUnit.currentType.ToString();

                switch (curUnit.currentType)
                {
                    case UnitType.Settler:
                        if (curTile.city == null) // todo later: && curTile.owner == player)
                        {
                            bottomLeftMenu1.BuildCityButtonContainer.Visibility = Visibility.Visible;
                        }
                        break;
                    default:
                        break;
                }
                bottomLeftMenu1.MoveButtonContainer.Visibility = Visibility.Visible;
            }
            else
            {
                blMenuContainer.Visible = false;
            }
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

            var delta = ProcessInput(gl);

            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            var theta = 0.785398f;
            var yOffset = (float)(zoom * Math.Sin(theta));
            var zOffset = (float)(zoom * Math.Cos(theta));

            viewMatrix = glm.lookAt(new vec3(pos.x, yOffset, pos.z + zOffset), new vec3(pos.x, 0, pos.z), new vec3(0, 1, 0));

            if (!setup)
            {
                Setup(gl);
            }
            
            tileProgram.Bind(gl);
            tileProgram.SetUniformMatrix4(gl, "projectionMatrix", projectionMatrix.to_array());
            tileProgram.SetUniformMatrix4(gl, "viewMatrix", viewMatrix.to_array());

            tileGeometry.vertexBufferArray.Bind(gl);
            foreach (var curTileType in tileTextures)
            {
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, curTileType.Value.TextureName);
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
            tileGeometry.vertexBufferArray.Unbind(gl);
            tileProgram.Unbind(gl);

            // Hex grid lines
            lineProgram.Bind(gl);
            lineProgram.SetUniformMatrix4(gl, "projectionMatrix", projectionMatrix.to_array());
            lineProgram.SetUniformMatrix4(gl, "viewMatrix", viewMatrix.to_array());
            lineProgram.SetUniform3(gl, "lineColor", 1.0f, 1.0f, 1.0f);
            lineProgram.SetUniform1(gl, "lineAlpha", 0.2f);
            gridGeometry.vertexBufferArray.Bind(gl);

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
          
            if(selectedTile != null && selectedTile.currentUnit != null)
            {
                lineProgram.SetUniform3(gl, "lineColor", 0.0f, 204.0f/255.0f, 1.0f);
                lineProgram.SetUniform1(gl, "lineAlpha", 1.0f);
                var tileModel = glm.translate(mat4.identity(), new vec3(selectedTile.centerPos.x, 0.002f, selectedTile.centerPos.z));
                var scaled = glm.scale(tileModel, new vec3(0.9f, 0.9f, 0.9f));
                lineProgram.SetUniformMatrix4(gl, "modelMatrix", scaled.to_array());
                gridGeometry.Draw(gl);
            }

            if (selectedTile != null && hoverTile != null && hoverTile  != selectedTile)
            {
                lineProgram.SetUniform3(gl, "lineColor", 0.0f, 0.0f, 0.0f);
                lineProgram.SetUniform1(gl, "lineAlpha", 1.0f);
                var activeUnitModel = glm.translate(mat4.identity(), new vec3(hoverTile.centerPos.x, 0.002f, hoverTile.centerPos.z));
                var scaled = glm.scale(activeUnitModel, new vec3(0.9f, 0.9f, 0.9f));
                lineProgram.SetUniformMatrix4(gl, "modelMatrix", scaled.to_array());
                gridGeometry.Draw(gl);
            }

            gridGeometry.vertexBufferArray.Unbind(gl);
            lineProgram.Unbind(gl);


            unitProgram.Bind(gl);
            unitProgram.SetUniformMatrix4(gl, "projectionMatrix", projectionMatrix.to_array());
            unitProgram.SetUniformMatrix4(gl, "viewMatrix", viewMatrix.to_array());

            for (int i = 0; i < world.xSize; i++)
            {
                for (int j = 0; j < world.zSize; j++)
                {
                    var curTile = world.tiles[i, j];
                    if (Math.Abs(curTile.centerPos.z - pos.z) < 16 && Math.Abs(curTile.centerPos.x - pos.x) < 16)
                    {
                        if (curTile.currentUnit != null)
                        unitGeometries[curTile.currentUnit.currentType].Draw(gl, unitProgram, curTile.centerPos);

                        if (!Unit.unitTypes[curTile.currentUnit.currentType].singleUnit)
                        {
                            unitGeometries[curTile.currentUnit.currentType].Draw(gl, unitProgram, curTile.centerPos);

                            if (!Unit.unitTypes[curTile.currentUnit.currentType].singleUnit)
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
            }       
            unitProgram.Unbind(gl);

           
            borderProgram.Bind(gl);
            borderGeometry.vertexBufferArray.Bind(gl);
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
            borderGeometry.vertexBufferArray.Unbind(gl);
            borderProgram.Unbind(gl);

            // Text labels
            for (int i = 0; i < world.xSize; i++)
            {
                for (int j = 0; j < world.zSize; j++)
                {
                    var curTile = world.tiles[i, j];
                    if (Math.Abs(curTile.centerPos.z - pos.z) < 16 && Math.Abs(curTile.centerPos.x - pos.x) < 16)
                    {
                        double[] bottomX = new double[] { 0.0 };
                        double[] bottomY = new double[] { 0.0 };
                        double[] bottomZ = new double[] { 0.0 };

                        double[] topX = new double[] { 0.0 };
                        double[] topY = new double[] { 0.0 };
                        double[] topZ = new double[] { 0.0 };

                        gl.Project(curTile.centerPos.x, curTile.centerPos.y, curTile.centerPos.z - Tile.h, Array.ConvertAll<float, double>(viewMatrix.to_array(), Convert.ToDouble),
                                 Array.ConvertAll<float, double>(projectionMatrix.to_array(), Convert.ToDouble), new int[] { 0, 0, (int)overlayControl.Width, (int)overlayControl.Height },
                                 topX,
                                 topY,
                                 topZ);

                        gl.Project(curTile.centerPos.x, curTile.centerPos.y, curTile.centerPos.z + Tile.h, Array.ConvertAll<float, double>(viewMatrix.to_array(), Convert.ToDouble),
                                Array.ConvertAll<float, double>(projectionMatrix.to_array(), Convert.ToDouble), new int[] { 0, 0, (int)overlayControl.Width, (int)overlayControl.Height },
                                bottomX,
                                bottomY,
                                bottomZ);

                        if (curTile.city != null)
                        {
                            gl.DrawText((int)topX[0] , (int)topY[0], 1.0f, 1.0f, 1.0f, "Test", 18.0f, ""+12);
                            gl.DrawText((int)topX[0] - curTile.city.name.Length * 5, (int)topY[0]+20, 1.0f, 1.0f, 1.0f, "Test", 20.0f, curTile.city.name.ToString());
                        }

                        if (curTile.currentUnit != null)
                        {
                            gl.DrawText((int)bottomX[0], (int)bottomY[0], 1.0f, 1.0f, 1.0f, "Test", 12.0f, "" + (int)(curTile.currentUnit.health * 100));
                            gl.DrawText((int)bottomX[0], (int)bottomY[0]+15, 1.0f, 1.0f, 1.0f, "Test", 12.0f, curTile.currentUnit.currentType.ToString());
                        }
                    }
                }
            }    
  
            fmod.Update();

        }

        private float ProcessInput(OpenGL gl)
        {
            var curFrameTicks = System.Environment.TickCount;
            var delta = (curFrameTicks - lastFrameTicks);

            if (Form.ActiveForm == null)
                return delta;

            if (Keyboard.IsKeyDown(Key.Escape))
                System.Windows.Forms.Application.Exit();

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
    }
}
