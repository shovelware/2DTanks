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
    class ProjAssets
    {

#region MVars
        
        Texture2D projBB, projMB, projRB,
                  projBA, projMA, projRA,
                  projBL, projML, projRL;

        public Texture2D BulletBase { get { return projBB; } }
        public Texture2D RocketBase { get { return projRB; } }
        public Texture2D MineBase { get { return projMB; } }

        public Texture2D BulletAccent { get { return projBA; } }
        public Texture2D RocketAccent { get { return projRA; } }
        public Texture2D MineAccent { get { return projMA; } }


        public Texture2D BulletLight { get { return projBL; } }
        public Texture2D RocketLight { get { return projRL; } }
        public Texture2D MineLight { get { return projML; } }


#endregion

#region Construct & Load

        public ProjAssets(ContentManager c)
        {
            LoadAssets(c);
        }

        private void LoadAssets(ContentManager c)
        {
            projBB = c.Load<Texture2D>(".\\Assets\\Texture\\Proj\\PBB");
            projBA = c.Load<Texture2D>(".\\Assets\\Texture\\Proj\\PBA");
            projBL = c.Load<Texture2D>(".\\Assets\\Texture\\Proj\\PBL");

            projMB = c.Load<Texture2D>(".\\Assets\\Texture\\Proj\\PMB");
            projMA = c.Load<Texture2D>(".\\Assets\\Texture\\Proj\\PMA");
            projML = c.Load<Texture2D>(".\\Assets\\Texture\\Proj\\PML");

            projRB = c.Load<Texture2D>(".\\Assets\\Texture\\Proj\\PRB");
            projRA = c.Load<Texture2D>(".\\Assets\\Texture\\Proj\\PRA");
            projRL = c.Load<Texture2D>(".\\Assets\\Texture\\Proj\\PRL");
        }

#endregion

    }

}
