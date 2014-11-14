using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TankGame
{
    class GUI
    {
        SpriteBatch sb;
        SpriteFont font;
        ContentManager c;
        Texture2D pix;
        Vector2 origin = new Vector2(0, 0);

        Matrix miniS, miniR, miniT, miniM;


        public GUI(SpriteBatch spriteBatch, ContentManager conMan)
        {
            sb = spriteBatch;
            c = conMan;
            
            pix = LoadTexture(".\\Assets\\Texture\\GUI\\Pix");
            font = LoadFont(".\\Assets\\Font\\motorwerk");
            
        }

        private Texture2D LoadTexture(string item)
        {
            Texture2D tex = c.Load<Texture2D>(item);
            return tex;
        }

        private SpriteFont LoadFont(string item)
        {
            SpriteFont font = c.Load<SpriteFont>(item);
            return font;
        }

        public void DrawBar(Vector2 pos, int value, int maxvalue, Color? border, Color? fill)
        {
            int posx = (int)(pos.X - maxvalue / 2 - 2);
            int posy = (int)pos.Y;

            sb.Draw(pix, new Rectangle(posx, posy, maxvalue + 4, 2), Color.White);
            sb.Draw(pix, new Rectangle(posx, posy + 10, maxvalue + 4, 2), Color.White);
            sb.Draw(pix, new Rectangle(posx, posy + 1, 2, 10), Color.White);
            sb.Draw(pix, new Rectangle(posx + 2 + maxvalue, posy + 1, 2, 10), Color.White);
            sb.Draw(pix, new Rectangle(posx + 2, posy + 2, value, 8), Color.White);
        }

        public void DrawBarAdv(Vector2 pos, int value, int maxvalue, int[] status, Color?[] border, Color?[] fill)
        {
        }


        public void DrawString(float xPos, float yPos, string prefix, string value, Color? col)
        {
            sb.DrawString(font, prefix + value, new Vector2(xPos, yPos), col ?? Color.Black);
        }

        public void DrawFloat(float xPos, float yPos, string prefix, float? value, Color? col)
        {
            sb.DrawString(font, prefix + value, new Vector2(xPos, yPos), col ?? Color.Black);
        }

        public void DrawBool(float xPos, float yPos, string prefix, bool value, Color? col)
        {
            sb.DrawString(font, prefix + value, new Vector2(xPos, yPos), col ?? Color.Black);
        }

        public void DrawVector3(float xPos, float yPos, string prefix, Vector3 value, Color? col)
        {
            sb.DrawString(font, prefix + value, new Vector2(xPos, yPos), col ?? Color.Black);
        }

        public void DrawSphere(BoundingSphere sphere, Color col, int? rad)
        {
            sb.DrawCircle(new Vector2(sphere.Center.X, sphere.Center.Y), sphere.Radius, col, rad ?? 1, 32);
        }

        public void DrawSphere(BoundingSphere sphere, Color col, bool status)
        {
            if (!status)
            sb.DrawCircle(new Vector2(sphere.Center.X, sphere.Center.Y), sphere.Radius, col, 1, 32);

            if (status)
            sb.DrawCircle(new Vector2(sphere.Center.X, sphere.Center.Y), sphere.Radius, col, (int)sphere.Radius / 16, 32);
        }

        public void DrawMinimap(TileManager tileM, TankManager tankM, ProjManager projM, Matrix cameraM)
        {
            Color mcBlk = new Color (000, 000, 000, 128);
            Color mcGry = new Color (128, 128, 128, 128);
            Color mcGrn = new Color (000, 128, 000, 128);
            Color mcRed = new Color (128, 000, 000, 128);
            Color mcYlo = new Color (128, 128, 000, 128);
            Color mcPrp = new Color (128, 000, 128, 128);

            int mapWidth = tileM.TilesHori * tileM.TileWidth / 20;
            int mapHeight = tileM.TilesVert * tileM.TileHight / 20;
            int mapLeft = 1280 - mapWidth;
            int mapTop = 720 - mapHeight;

            int barHeight = 10;
           int healthPercent = (int)tankM.CurrentTank().Health + 1;

            int bullets = (int)tankM.CurrentTank().AmmoBullet;
            int mines = (int)tankM.CurrentTank().AmmoMine;
            int rockets = (int)tankM.CurrentTank().AmmoRocket;
            sb.Begin();
            sb.DrawString(font, "Remaining: " + tankM.Remaining(), new Vector2(mapLeft, mapTop - 80), mcGry);
            sb.DrawString(font, "Kills: " + tankM.PlayerKills, new Vector2(mapLeft, mapTop - 60), mcGry);
            sb.DrawString(font, "Score: " + tankM.PlayerScore, new Vector2(mapLeft, mapTop - 40), mcGry);
            //HEALTH
            sb.Draw(pix, new Rectangle(mapLeft, mapTop - barHeight, mapWidth, barHeight), mcBlk);//Bar Base
            sb.Draw(pix, new Rectangle(mapLeft, mapTop - barHeight, healthPercent  * (mapWidth / 100), barHeight), mcGrn);//Bar Fill

            //AMMO
            sb.Draw(pix, new Rectangle(mapLeft - barHeight * 3, mapTop + mapHeight / 2, barHeight * 3, mapHeight / 2), mcBlk);//Base bar
            sb.Draw(pix, new Rectangle(mapLeft - barHeight * 3, mapTop + mapHeight / 2, barHeight * 1, bullets * ((mapHeight / 2) / 25)), mcYlo);//Bullet bar
            sb.Draw(pix, new Rectangle(mapLeft - barHeight * 2, mapTop + mapHeight / 2, barHeight * 1, mines * ((mapHeight / 2) / 5)), mcPrp);//Mine bar
            sb.Draw(pix, new Rectangle(mapLeft - barHeight * 1, mapTop + mapHeight / 2, barHeight * 1, rockets * ((mapHeight / 2) / 10)), mcRed);//Rocket bar
            

            //MAP
            sb.Draw(pix, new Rectangle(mapLeft, mapTop, mapWidth, mapHeight), mcGry);//Draw tiles in map

            foreach (Tank t in tankM.TankList)//Draw tanks
            {
                int tankX = (int)t.Position.X / 20;
                int tankY = (int)t.Position.Y / 20;
                Color tankCol;
                int tanksize = 10;
                if (t.Player)
                {
                    tankCol = mcGrn;
                }

                else tankCol = mcRed;

                sb.Draw(pix, new Rectangle(mapLeft + tankX - (tanksize / 2), mapTop + tankY - (tanksize / 2), tanksize, tanksize), tankCol);
            }

            foreach (Projectile p in projM.GetProjList())//Draw projectiles
            {
                int projX = (int)p.Position.X / 20;
                int projY = (int)p.Position.Y / 20;
                Color projCol;
                int projsize = 5;

                switch (p.Type)
                {
                    case 'B':
                        projsize = 2;
                        break;
                    case 'M':
                        projsize = 6;
                        break;
                    case 'R':
                        projsize = 5;
                        break;
                }

                if (p.Player)
                {
                    projCol = mcGrn;
                }

                else projCol = mcRed;

                sb.Draw(pix, new Rectangle(mapLeft + projX - (projsize / 2), mapTop + projY - (projsize / 2), projsize, projsize), projCol);

                sb.Draw(pix, new Rectangle(mapLeft + projX - (projsize / 2), mapTop + projY - (projsize / 2), projsize, projsize), projCol);
            }
                
            sb.End();
        }

        ////////////////

        public void DrawMsg(Vector2 pos, string text, Color color)
        {
            sb.Begin();
            sb.Draw(pix, new Rectangle((int)pos.X - 64, (int)pos.Y - 32, 128, 64), Color.Black);
            sb.Draw(pix, new Rectangle((int)pos.X - 62, (int)pos.Y - 30, 124, 60), Color.White);
            sb.DrawString(font, text, new Vector2(pos.X - 40, pos.Y - 20), Color.Black);
            sb.End();
        }

        //public void DrawBool(float xPos, float yPos, bool value)
        //{
        //    sb.DrawString(font, "" + value, new Vector2(xPos, yPos), Color.Black);
        //}

        //public void DrawByte(int xPos, int yPos, byte value)
        //{
        //    sb.DrawString(font, "" + value, new Vector2(xPos, yPos), Color.Black);
        //}

        //public void DrawInt(int xPos, int yPos, int value)
        //{
        //    sb.DrawString(font, "" + value, new Vector2(xPos, yPos), Color.Black);
        //}

        //public void DrawInt(Vector2 position, int value)
        //{
        //    sb.DrawString(font, "" + value, position, Color.Black);
        //}

        //public void DrawString(int xPos, int yPos, string value)
        //{
        //    sb.DrawString(font, "" + value, new Vector2(xPos, yPos), Color.Black);
        //}

        //public void DrawString(Vector2 position, string value)
        //{
        //    sb.DrawString(font, "" + value, position, Color.Black);
        //}

        //public void DrawString(int xPos, int yPos, string value, Color color)
        //{
        //    sb.DrawString(font, "" + value, new Vector2(xPos, yPos), color);
        //}
        
        //public void DrawVector2(int xPos, int yPos, Vector2 value)
        //{
        //    sb.DrawString(font, "" + value, new Vector2(xPos, yPos), Color.Black);
        //}

        //public void DrawRectangle(int xPos, int yPos, Rectangle value)
        //{
        //    sb.DrawString(font, "" + value, new Vector2(xPos, yPos), Color.Black);
        //}
    }
}

