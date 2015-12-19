using SharpNoise.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{
    
    public class World
    {
        public Tile[,] tiles;
        public List<Border> borders;

        public int xSize;
        public int zSize;
        public World(int x, int z)
        {
            borders = new List<Border>();
            xSize = x;
            zSize = z;
            tiles = new Tile[xSize, zSize];

            for (int i = 0; i< x; i++)
            {
                for(int j = 0; j < z; j++)
                {
                    var tile = new Tile();
                    tile.currentType = TileType.Grass;
                    tile.xPos = i;
                    tile.zPos = j;
                    tile.neighbors = new List<Neighbor>();
                    tile.neighborsDict = new Dictionary<BorderType, Neighbor>();
                    tile.centerPos = Tile.GetTileCoord(i, j);
                    tiles[i, j] = tile;
                }
            }

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < z; j++)
                {
                    var curTile = tiles[i, j];

                    AddPossibleNeighbor(curTile, i - 1, j, BorderType.Left);  // LEFT
                    AddPossibleNeighbor(curTile, i + 1, j, BorderType.Right);  // RIGHT

                    if (j % 2 == 1)
                    {
                        // up one
                        AddPossibleNeighbor(curTile, i, j-1, BorderType.UpLeft);  // UP LEFT
                        AddPossibleNeighbor(curTile, i+1, j-1, BorderType.UpRight);  // UP RIGHT

                        // down one
                        AddPossibleNeighbor(curTile, i, j+1, BorderType.DownLeft); // DOWN LEFT
                        AddPossibleNeighbor(curTile, i + 1, j + 1, BorderType.DownRight);  // DOWN RIGHT
                    }
                    else
                    {
                        // up one
                        AddPossibleNeighbor(curTile, i-1, j - 1, BorderType.UpLeft);  // UP LEFT
                        AddPossibleNeighbor(curTile, i, j - 1, BorderType.UpRight); // UP RIGHT

                        // down one
                        AddPossibleNeighbor(curTile, i-1, j + 1, BorderType.DownLeft);  // DOWN LEFT
                        AddPossibleNeighbor(curTile, i, j + 1, BorderType.DownRight); // DOWN RIGHT
                    }
                }
            }
            GenerateLand();
        }

        public void AddPossibleNeighbor(Tile tile, int x, int z, BorderType borderType)
        {
           var possibleNeighborTile = GetTile(x, z);
            if(possibleNeighborTile != null)
            {
                var myNewNeighbor = new Neighbor
                {
                    borderType = borderType,
                    cost = 1,
                    tile = possibleNeighborTile
                };

                tile.neighbors.Add(myNewNeighbor);
                tile.neighborsDict[borderType] = myNewNeighbor;
            }
        }

        public Tile GetTile(int x, int z)
        {
            if (x >= 0 && x < xSize && z >= 0 && z < zSize)
            {
                return tiles[x, z];
            }
            else
            {
                return null;
            }
        }

        System.Random r = new System.Random();

        public List<Tile> FindShortestClearPath(Tile root, Tile b, List<TileType> impassable, bool ignoreUnits = false)
        {
            if(root == b)
            {
                return null;
            }
            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < zSize; j++)
                {
                    var curTile = tiles[i, j];
                    curTile.distance = -1;
                    curTile.parent = null;
                }
            }

            root.distance = 0;
            var q = new Queue<Tile>();
            q.Enqueue(root);

            while (q.Count() > 0){
                var current = q.Dequeue();
                foreach(var neighbor in current.neighbors)
                {

                    if( !impassable.Contains(neighbor.tile.currentType) &&  (neighbor.tile.currentUnit == null || ignoreUnits)){
                        if(neighbor.tile.distance == -1 || neighbor.tile.distance > (current.distance + neighbor.cost))
                        {
                            neighbor.tile.distance = current.distance + neighbor.cost;
                            neighbor.tile.parent = current;               
                            q.Enqueue(neighbor.tile);
                        }
                    }                   
                }
            }

            if(b.distance == -1)
            {
                return null;
            }

            var path = new List<Tile>();

            var curItem = b.parent;
            while (curItem != root)
            {
                if(curItem == null)
                {
                    Debug.WriteLine("Warning: Null path item found. Avoiding");
                    return null;
                }
                path.Add(curItem);
                curItem = curItem.parent;
            }
            return path;
        }
        public void UpdateBorders()
        {
          borders.Clear();

          for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < zSize; j++)
                {
                    var currentTile = tiles[i, j];
                    if(currentTile.owner == null)
                    {
                        continue;
                    }

                    foreach (var neighborItem in currentTile.neighbors)
                    {
                        var curNeighbor = neighborItem;
                        if (curNeighbor.tile.owner != currentTile.owner)
                        {
                            var b = new Border();
                            b.owner = currentTile.owner;
                            b.tile = currentTile;
                            b.type = curNeighbor.borderType;
                            borders.Add(b);
                        }
                    }
                }
            }
        }

        private void GenerateLand()
        {
            var noisemodule = new Perlin();
            noisemodule.Seed = (int)(99999 * r.NextDouble());

            var p = new SharpNoise.Models.Plane(noisemodule);

            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < zSize; j++)
                {
                    var height = 200 + p.GetValue(i / 15.5f, j / 15.5f) * 100;

                    if (height > 2998)
                    {
                        tiles[i, j].currentType = TileType.Lava;
                    }

                    if (height > 295)
                    {
                        tiles[i, j].currentType = TileType.Mountain;

                    }
                    else if (height > 208)
                    {
                        int earlyCutoff = (int)(zSize * 0.3);
                        int lateCutoff = (int)(zSize * 0.5);

                        if (j < earlyCutoff || j > lateCutoff)
                        {
                            tiles[i, j].currentType = TileType.Grass;
                        }
                        else
                        {
                            if (j == earlyCutoff || j == lateCutoff)
                            {
                                if (r.NextDouble() > 0.5)
                                {
                                    tiles[i, j].currentType = TileType.Grass;
                                }
                                else
                                {
                                    tiles[i, j].currentType = TileType.Desert;
                                }
                            }
                            else
                            {
                                tiles[i, j].currentType = TileType.Desert;
                            }
                        }
                    }
                    else
                    {
                        tiles[i, j].currentType = TileType.Water;
                    }

                    /*
                    
                    if (i % 2 == 0) {
                        if(j % 2 == 0){
                            tiles[i, j].currentType = TileType.Grass;
                        }
                        else {
                            tiles[i, j].currentType = TileType.Desert;
                        }
                    }
                    else
                    {
                        if (j % 2 == 0)
                        {
                            tiles[i, j].currentType = TileType.Water;
                        }
                        else
                        {
                            tiles[i, j].currentType = TileType.DeepWater;
                        }
                    }*/

                }
            }

            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < zSize; j++)
                {
                    var curTile = tiles[i, j];
                    if (curTile.currentType == TileType.Water)
                    {
                        var neighbors = curTile.neighbors;
                        if (neighbors.All(x => x.tile.currentType == TileType.Water || x.tile.currentType == TileType.DeepWater || x.tile.currentType == TileType.None))
                        {
                            tiles[i, j].currentType = TileType.DeepWater;
                        }
                    }
                }
            }
        }
    }
}
