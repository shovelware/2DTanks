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
    class TankAssets
    {
        Texture2D wheelsB, bannerB, turretB, barrelB,
                  arrowsB, crosshB;

        Random r = new Random();

        Texture2D[] wheelsA = new Texture2D[27];
        Texture2D[] turretA = new Texture2D[27];
        Texture2D[] barrelA = new Texture2D[27];
        Texture2D[] bannerA = new Texture2D[27];

        public Texture2D BaseWheels { get { return wheelsB; } }
        public Texture2D BaseTurret { get { return turretB; } }
        public Texture2D BaseBarrel { get { return barrelB; } }
        public Texture2D BaseBanner { get { return bannerB; } }
        public Texture2D BaseArrows { get { return arrowsB; } }

        public TankAssets(ContentManager c)
        {
            LoadAssets(c);
        }

        private void LoadAssets(ContentManager c)
        {

            wheelsB = c.Load<Texture2D>(".\\Assets\\Texture\\TankB\\W");
            turretB = c.Load<Texture2D>(".\\Assets\\Texture\\TankB\\T");
            barrelB = c.Load<Texture2D>(".\\Assets\\Texture\\TankB\\B");
            bannerB = c.Load<Texture2D>(".\\Assets\\Texture\\TankB\\V");
            arrowsB = c.Load<Texture2D>(".\\Assets\\Texture\\TankB\\A");

            wheelsA[0] = c.Load<Texture2D>(".\\Assets\\Texture\\TankA\\W0");
            turretA[0] = c.Load<Texture2D>(".\\Assets\\Texture\\TankA\\T0");
            barrelA[0] = c.Load<Texture2D>(".\\Assets\\Texture\\TankA\\B0");
            bannerA[0] = c.Load<Texture2D>(".\\Assets\\Texture\\TankA\\V0");

            for (int i = 1; i <=26; i++)
            {
                wheelsA[i] = c.Load<Texture2D>(".\\Assets\\Texture\\TankA\\W" + (char)(i + 64));
            }

            for (int i = 1; i <= 26; i++)
            {
                turretA[i] = c.Load<Texture2D>(".\\Assets\\Texture\\TankA\\T" + (char)(i + 64));
            }

            for (int i = 1; i <= 26; i++)
            {
                barrelA[i] = c.Load<Texture2D>(".\\Assets\\Texture\\TankA\\B" + (char)(i + 64));
            }

            for (int i = 1; i <= 26; i++)
            {
                //bannerA[y] = c.Load<Texture2D>(".\\Assets\\Texture\\TankA\\W" + (char)(y + 64));
            }
        }

        public Texture2D Wheels(char? inchar)
        {
            if (inchar == null)
            {
                return wheelsA[r.Next(1, 27)];
            }

            else if (inchar >= 'A' && inchar <= 'Z')
            {
                return wheelsA[(int)inchar - 64];
            }

            else return wheelsA[0];
        }

        public Texture2D Turret(char? inchar)
        {
            if (inchar == null)
            {
                return turretA[r.Next(1, 27)];
            }

            else if (inchar >= 'A' && inchar <= 'Z')
            {
                return turretA[(int)inchar - 64];
            }

            else return wheelsA[0];
        }

        public Texture2D Barrel(char? inchar)
        {
            if (inchar == null)
            {
                return barrelA[r.Next(1, 27)];
            }

            else if (inchar >= 'A' && inchar <= 'Z')
            {
                return barrelA[(int)inchar - 64];
            }

            else return wheelsA[0];
        }

        public Texture2D Banner(char? inchar)
        {
            //if (inchar == null)
            //{
            //    Random r = new Random();
            //    return bannerA[r.Next(1, 27)];
            //}

            //else if (inchar >= 'A' && inchar <= 'Z')
            //{
            //    return bannerA[(int)inchar - 64];
            //}

            //else 
            return wheelsA[0];
        }
    
    }

}
