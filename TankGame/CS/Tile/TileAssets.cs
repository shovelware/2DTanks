using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace TankGame
{   
    class TileAssets
    {

#region MVars

        int tileWidth;
        int tileHeight;

        public int TileWidth { get { return tileWidth; } }
        public int TileHeight { get { return tileHeight; } }

        Texture2D tileB;
        Texture2D[] tileA = new Texture2D[20];

        public Texture2D TileBase { get { return tileB; } }
        public Texture2D TileAccent (int i)
        { 
            return tileA[i];
        }

#endregion

#region Construct & Load

        public TileAssets(ContentManager c)
        {
            LoadAssets(c);
            tileWidth = tileB.Width;
            tileHeight = tileB.Height;
        }

        private void LoadAssets(ContentManager c)
        {

            tileB = c.Load<Texture2D>(".\\Assets\\Texture\\Tile\\tileB");

            for (int i = 0; i <= 3; i++)
            {
                tileA[i] = c.Load<Texture2D>(".\\Assets\\Texture\\Tile\\tileA" + i);
            }
        }

#endregion

    }

}
