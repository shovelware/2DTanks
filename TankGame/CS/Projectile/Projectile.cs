using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TankGame
{
    class Projectile
    {
#region MVArs

        //Status
        bool alive;
        bool active;
        char type;
        bool player;

        public bool Alive { get { return alive; } }
        public bool Active { get { return active; } }
        public char Type { get { return type; } }
        public bool Player { get { return player; } }

        //Combat
        float damage;
        float health;

        public float Damage { get { return damage; } }
        public float Health { get { return health; } }

        const int SPEEDB = 64, SPEEDM = 0, SPEEDR = 16, SPEEDX = 32,
                  DAMAGEB = 5, DAMAGEM = 50, DAMAGER = 25, DAMAGEX = 500,
                  HEALTHB = 1, HEALTHM = 15, HEALTHR = 10, HEALTHX = 500;

        


        //Assets
        ProjAssets a;
        GUI g;

        Texture2D projBTex, projATex, projLTex;

        Color projBCol, projACol, projLCol;

        Vector2 projVOrigin;

        //SRT Matrices
        Matrix projMScale,
               projMRotat,
               projMTrans,
               projMFinal,
               debugsMFinal;

        //Float values for Matrix Transformation
        float //Scale
              projFScale,
            //Rotation
              projFRotat,
            //Movement
              projFMovSp,
              projFMovAc,
              projFMovDe,
              projFMovCh,
              projFMovMx;

        public float ProjectileRotation { get { return (float)(projFRotat * 180.0 / Math.PI); } }

        //Translation Vectors
        Vector3 projVTrans;

        //Direction Vectors
        Vector3 projVUnits, 
                projVPoint;

        public Vector3 ProjDir { get { return projVPoint; } }

        //Position
        Vector3 pos;
        public Vector3 Position { get { return pos; } }

        //Collision 
        //Collision Units are a fraction of texture size * scale to uniform collision to arbitrarily sized projectile
        //It's easier to see diagrammatically, use debug drawing for this
        float cU;

        //Vectors for positioning
        Vector3 bvCC, bvPC;

        //Actual spheres
        BoundingSphere bsCC, bsPC;

        //Bools used for detection measures
        bool cdCC, cdPC;

#endregion

#region Make, Break, Load

        //Constructor/
        public Projectile(ProjAssets assets, GUI grUsIn, Vector3? pos, float? scale, float? rot, char? projType, Color? colB, Color? colA, bool playerOwned)
        {
            active = true;
            alive = true;
            player = playerOwned;
            a = assets;
            g = grUsIn;

            type = (char)projType;

            LoadProjTextures(colB, colA);

            projVTrans = pos ?? Vector3.Zero;
            projFScale = scale ?? 1f;
            projFRotat = rot ?? 0f;

            //DEBUG 

            switch (type)
            {
                case ('B'):
                    {
                        projFMovMx = SPEEDB;
                        projFMovAc = SPEEDB;
                        damage = DAMAGEB * projFScale;
                        health = HEALTHB * projFScale;
                    }
                    break;
                case ('M'):
                    {
                        projFMovAc = SPEEDM;
                        damage = DAMAGEM * projFScale;
                        health = HEALTHM * projFScale;
                    }
                    break;
                case ('R'):
                    {
                        projFMovMx = SPEEDR;
                        projFMovAc = SPEEDR;
                        damage = DAMAGER * projFScale;
                        health = HEALTHR * projFScale;
                    }
                    break;
                case ('X'):
                        {
                        projFMovMx = SPEEDX;
                        projFMovAc = SPEEDX;
                        damage = DAMAGEX * projFScale;
                        health = HEALTHX * projFScale;
                        }
                    break;
                default:
                    active = false;
                    break;
            }
            DebugInit();
            UpdateFactors();
            //ENDEBUG
        }

        private void DebugInit()
        {
           
            projVOrigin = new Vector2(projBTex.Width / 2, projBTex.Height / 2);
          
        }

        private void LoadProjTextures(Color? colB, Color? colA)
        {
            switch(type)
            {
                case ('B'):
                    {
                        projBTex = a.BulletBase;
                        projATex = a.BulletAccent;
                        projLTex = a.BulletLight;

                        projBCol = colB ?? Color.Gray;
                        projACol = colA ?? Color.Orange;
                        projLCol = Color.Yellow;

                    }
                    break;
                case('M'):
                    {
                        projBTex = a.MineBase;
                        projATex = a.MineAccent;
                        projLTex = a.MineLight;

                        projBCol = colB ?? Color.DarkSlateGray;
                        projACol = colA ?? Color.LightGray;
                        projLCol = Color.Red;
                    }
                    break;
                case('R'):
                    {
                        projBTex = a.RocketBase;
                        projATex = a.RocketAccent;
                        projLTex = a.RocketLight;

                        projBCol = colB ?? Color.DarkGreen ;
                        projACol = colA ?? Color.Orange;
                        projLCol = Color.Red;
                    }
                    break;
                case ('X'):
                    {
                        projBTex = a.RocketBase;
                        projATex = a.RocketAccent;
                        projLTex = a.RocketLight;

                        projBCol = Color.White;
                        projACol = Color.Cyan;
                        projLCol = Color.Black;
                    }
                    break;
            }
        }

        public void Spawn()
        {
        }

        public void Death()
        {
            active = false;
        }

#endregion

#region Ch-ch-ch-ch-changes

        public void SetCols(Color? baseNew, Color? accentNew, Color? lightNew)
        {
            projBCol = baseNew ?? projBCol;
            projACol = accentNew ?? projACol;
            projLCol = lightNew ?? projLCol;     
        }


        public void SetScale(float newFScale)
        {
            projFScale = newFScale;
        }


        public void CrementScale(float amount)
        {
            if (projFScale + amount != 0)
            projFScale += amount;
        }

#endregion

#region Movement

        public void Move()
        {
            projFMovCh = projFMovAc;
        }

#endregion

#region Collision

        private void UpdateBounds()
        {
            //Transform Vectors for rotation and correct for origin

            bvCC = Vector3.Transform(bvCC - new Vector3(projVOrigin * projFScale, 0), projMRotat);

            bvPC = Vector3.Transform(bvPC - new Vector3(projVOrigin * projFScale, 0), projMRotat);

            //Create new Bounding Spheres

            bsCC = new BoundingSphere(projVTrans + bvCC, cU * 2f);
            bsPC = new BoundingSphere(projVTrans + bvPC, cU * 1f);
        }

        private void CheckBounds()
        {
            if (pos.X < -100 || pos.X > 4100
             || pos.Y < -100 || pos.Y > 4100)
            {
                active = false;
            }
        }

        public BoundingSphere GetSphereCoarse()
        {
            return bsCC;
        }


        public BoundingSphere GetSpherePrecis()
        {
            return bsPC;
        }


        public bool CheckCollisonCoarse(BoundingSphere other)
        {
            if (bsCC.Intersects(other))
            {
                cdCC = true;
                return true;
            }

            else return false;
        }

        public bool CheckCollisionPrecis(BoundingSphere other)
        {
            bool collision = false;

            if (bsPC.Intersects(other))
            {
                cdPC = true;
                collision = true;
            }

            return collision;
        }

        public void ResetCollisions()
        {
            cdPC = false;
            cdCC = false;
        }

#endregion

#region Combat

        public void Hit()
        {
        }

        public void Hurt()
        {
        }

#endregion

#region Movement Actual

        private void MovementActual()
        {
            //Directionality
            projVUnits = new Vector3(1, 0, 0);
            projVPoint = Vector3.Transform(projVUnits, projMRotat);

                //Add change to speed
                projFMovSp += projFMovCh;

                //Apply decel
                if (projFMovSp > 0)
                {
                    if (projFMovSp - projFMovDe > 0)
                    {
                        projFMovSp -= projFMovDe;
                    }

                    else
                    {
                        projFMovSp = 0;
                    }
                }

                //Limit to max speed
                if (projFMovSp > projFMovMx)
                {
                    projFMovSp = projFMovMx;
                }

                //Actual movement
                projVTrans += projVPoint * (projFMovSp);
            

            projFMovCh = 0;
            //projFMovMx = projFScale * 5f;
        }

#endregion

#region U&D

        /// <summary>
        /// Updates any variable that's a factor of any other variable. Called on construction and every Update cycle.
        /// </summary>
        private void UpdateFactors()
        {
            cU = (projBTex.Height / 2f) * projFScale;

            bvCC = new Vector3(cU, cU, 0);
            bvPC = new Vector3(cU, cU, 0);

            //projFMovAc = 4 / projFScale;
            //projFMovDe = projFMovAc * 0.25f;
            //projFMovMx = projFScale * 5f;
        }

        private void UpdateMatrices(Matrix cameraMFinal)
        {
            //Scale
            projMScale = Matrix.CreateScale(projFScale, projFScale, 0);

            //Rotation
            projMRotat = Matrix.CreateRotationZ(projFRotat);

            //Translation
            projMTrans = Matrix.CreateTranslation(projVTrans);

            //Get Position
            pos.X = projMTrans.M41;
            pos.Y = projMTrans.M42;

            //Finals
            projMFinal = projMScale * projMRotat * projMTrans * cameraMFinal;
            debugsMFinal = cameraMFinal;
        }

        public void Update(GameTime gameTime, Matrix cameraMFinal, bool pause)
        {
            UpdateFactors();

            if (!pause)
            {
                MovementActual();
                UpdateBounds();
            }

            UpdateMatrices(cameraMFinal);

            CheckBounds();
        }

        public void Draw(SpriteBatch sb, bool debug)
        {
            //DEBUG
            sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null);
            sb.End();
            //DEBUGEND
            if (active)
            {
                if (alive)
                {
                    //Projectile
                    sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, projMFinal);
                    sb.Draw(projLTex, Vector2.Zero, null, projLCol, 0, projVOrigin, 1, SpriteEffects.None, 0);
                    sb.Draw(projBTex, Vector2.Zero, null, projBCol, 0, projVOrigin, 1, SpriteEffects.None, 0);
                    sb.Draw(projATex, Vector2.Zero, null, projACol, 0, projVOrigin, 1, SpriteEffects.None, 0);
                    sb.End();
                }
            }

            //GUI junk

            if (debug)
            {
                sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, debugsMFinal);

                g.DrawSphere(bsCC, new Color(255, 000, 255, 096), false);
                g.DrawSphere(bsPC, new Color(255, 255, 255, 127), cdPC);

                g.DrawVector3(pos.X, pos.Y, "Pos: ", projVTrans, null);
                g.DrawFloat(pos.X, pos.Y + 20, "Speed:", projFMovMx, null);
                g.DrawBool(pos.X, pos.Y + 40, "Player: ", player, null);
                sb.End();
            }
        }
#endregion
    }
}
