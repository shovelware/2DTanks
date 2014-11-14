using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankGame
{
    class TileManager
    {
#region MVArs
        TileAssets assets;
        Random rng = new Random();

        int tileWidth;
        int tileHight;
        int tilesHori = 32;
        int tilesVert = 32;
        List<Tile[,]> tilesetList = new List<Tile[,]>();
        int currentTileset;

        public int TileWidth { get { return tileWidth; } }
        public int TileHight { get { return tileHight; } }
        public int TilesHori { get { return tilesHori; } }
        public int TilesVert { get { return tilesVert; } }
        public Tile[,] GetCurrentSet { get { return tilesetList[currentTileset]; } }

#endregion

#region Make, Break and Change

        public TileManager(TileAssets tileAssets)
        {
            assets = tileAssets;
            tileWidth = tileAssets.TileWidth;
            tileHight = tileAssets.TileHeight;
            
            DebugInit();
        }

        public void DebugInit()
        {          
        }

        public void AddSet(Color primary, Color secondary, int accent, bool makeCurrent, bool jiggle, bool darken)
        {
            Tile[,] setTemp = new Tile[32, 32];
            for (int i = 0; i < tilesVert; i++)
            {
                for (int j = 0; j < tilesHori; j++)
                {
                    {
                        if (j == 0 && i == 0)
                        {
                            setTemp[j, i] = new Tile(assets, accent, primary , secondary, tileWidth, tileHight, new Vector2(j, i), rng, jiggle, darken);
                        }

                        else
                        {
                            setTemp[j, i] = new Tile(assets, accent, primary, secondary, tileWidth, tileHight, new Vector2(j, i), rng, jiggle, darken);
                        }
                    }

                }
            }
            tilesetList.Add(setTemp);

            if (makeCurrent)
            {
                currentTileset = tilesetList.Count - 1;
            }
        }

        public void RefreshCurrent(Color primary, Color secondary, int accent)
        {
            Tile[,] setTemp = new Tile[32, 32];
            for (int i = 0; i < tilesVert; i++)
            {
                for (int j = 0; j < tilesHori; j++)
                {
                    {
                        if (j == 0 && i == 0)
                        {
                            setTemp[j, i] = new Tile(assets, accent, primary , secondary, tileWidth, tileHight, new Vector2(j, i), rng, false, false);
                        }

                        else
                        {
                            setTemp[j, i] = new Tile(assets, accent, primary, secondary, tileWidth, tileHight, new Vector2(j, i), rng, true, true);
                        }
                    }

                }
            }
            tilesetList[currentTileset] = setTemp;
        }

        public void CrementCurrentTileset(int direction)
        {
            int dir = direction / Math.Abs(direction);

            if (currentTileset + dir >= 0 && currentTileset + dir <= tilesetList.Count - 1)
            {
                currentTileset += dir;
            }
        }

#endregion

#region U&D

        public void Update(GameTime gameTime, Matrix cameraMFinal, bool pause)
        {
            foreach (Tile t in tilesetList[currentTileset])
            {
                t.Update(gameTime, cameraMFinal, pause);
            }
        }

        public void Draw(SpriteBatch sprBat)
        {
            foreach (Tile t in tilesetList[currentTileset])
            {
                t.Draw(sprBat);
            }
        }

#endregion
    }
}
