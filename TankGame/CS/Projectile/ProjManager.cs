using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TankGame
{
    class ProjManager
    {
#region MVars

        ProjAssets assets;
        List<Projectile> projList = new List<Projectile>();

        bool debug;

#endregion

#region Make

        public ProjManager(ProjAssets projAssets)
        {
            assets = projAssets;
        }

#endregion

#region Management

        public void AddProj(GUI gui, Vector3? pos, float? scale, float? rot, char? projType, Color? colB, Color? colA, bool playerOwned)
        {
            projList.Add(new Projectile(assets, gui, pos, scale, rot, projType, colB, colA, playerOwned));
        }

        public int TotaProjectiles()
        {
            return projList.Count;
        }

        public List<Projectile> GetProjList()
        {
            return projList;
        }

        private void Cleanup()
        {
            for (int p = 0; p < projList.Count; p++)
            {
                if (projList[p].Active == false)
                {
                    projList.RemoveAt(p);
                }
            }
        }

        public void KillAll()
        {
            projList.Clear();
        }

        //AddMultiple

        //Kill

        //KillAll

        //KillAllBut

        //ResetAll

#endregion

#region Ch-ch-ch-ch-changes

        public void ToggleDebug()
        {
            debug = !debug;
        }

#endregion

#region Collision

        public void CheckCollisions(TankManager tankM)
        {
            ResetCollisions();

            foreach (Projectile p in projList)//One proj
            {
                foreach (Projectile o in projList)//Check against others
                {
                    if (projList.IndexOf(p) != projList.IndexOf(o))//Make sure not same proj
                    {
                        if (p.CheckCollisonCoarse(o.GetSphereCoarse()))//Coarse collision
                        {
                            //Fine collision
                            if (p.CheckCollisionPrecis(o.GetSpherePrecis()))
                            {
                                p.Death();
                                //o.Hurt(t.Damage);
                                o.Death();//To be replaced by above once finished
                            }
                        }
                    }
                }
            }

            foreach (Projectile p in projList)//One Proj
            {
                foreach (Tank t in tankM.TankList)//Check against Tanks
                {
                    if (p.CheckCollisonCoarse(t.GetSphereCoarse()))//Coarse collision
                    {
                        foreach (BoundingSphere ob in t.GetSpherePrecis())//Fine collision
                        {
                            if (p.CheckCollisionPrecis(ob) || t.CheckCollisionPrecis(p.GetSpherePrecis()))
                            {
                                t.Hit(p.Damage, ob, p);
                                p.Death();
                            }
                        }
                    }
                }
            }
        }

        public void ResetCollisions()
        {
            foreach (Projectile p in projList)
            {
                p.ResetCollisions();
            }
        }

#endregion

#region U&D

        public void Update(GameTime gameTime, Matrix cameraM, bool pause, TankManager tankM)
        {
            foreach (Projectile p in projList)
            {
                p.Move();
                p.Update(gameTime, cameraM, pause);
            }
            CheckCollisions(tankM);
            Cleanup();

        }

        public void ForceUpdate(GameTime gameTime, Matrix cameraM, TankManager tankM)
        {
            CheckCollisions(tankM);
            foreach (Projectile p in projList)
            {
                p.Move();
                p.Update(gameTime, cameraM, false);
            }
            CheckCollisions(tankM);
            Cleanup();

        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            foreach (Projectile p in projList)
            {
                p.Draw(sb, debug);
            }
        }

#endregion
    }
}
