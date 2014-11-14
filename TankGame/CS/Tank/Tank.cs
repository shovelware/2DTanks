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

//

namespace TankGame
{
    class Tank
    {

        #region MVArs

        //Status
        bool alive;
        bool active;
        bool player;
        bool current;
        bool controlled;
        bool drawDebug;

        public bool Alive { get { return alive; } }
        public bool Active { get { return active; } }
        public bool Player { get { return player; } }
        public bool Current { get { return current; } set { current = value; } }
        public bool Controlled { get { return controlled; } set { controlled = value; } }
        public bool DrawDebug { get { return drawDebug; } }

        bool empty;

        //AI
        Brain brain;
        Tank lastHeard, lastSeen;

        public Brain Brain { get { return brain; } }
        public Tank LastHeard { get { return lastHeard; } }
        public Tank LastSeen { get { return lastSeen; } }
        public Tank LastFelt { get { return lastCollided; } }

        //Combat
        ProjManager p;

        const int UPDATE = 60;

        const float HEALTHMAX = 100;
        float health;

        public float HealthMax { get { return HEALTHMAX; } }
        public float Health { get { return health; } }

        Projectile lastHit;
        Tank lastCollided;
        public Projectile LastHit { get { return lastHit; } }
        public Tank LastCollided { get { return lastCollided; } set { lastCollided = value; } }

        //Weapons
        float cooldownBullet, cooldownMine, cooldownRocket, cooldownX;
        const float cooltimeBullet = 0.1f, cooltimeMine = 5f, cooltimeRocket = 1f, cooltimeX = 0f;

        float ammoBullet, ammoMine, ammoRocket, ammoX,
              reloadBullet, reloadMine, reloadRocket, reloadX;

        const float retimeBullet = 5f, retimeMine = 10f, retimeRocket = 7.5f, retimeX = 1f;

        const int totalBullet = 25, totalRocket = 10, totalMine = 5, totalX = 10;

        bool readyRocket, readyBullet, readyMine, readyX;

        public float AmmoBullet { get { return ammoBullet; } }
        public float AmmoMine { get { return ammoMine; } }
        public float AmmoRocket { get { return ammoRocket; } }

        //Sound
        float volume;

        const float VOLWHEELSROT = 10f,
            VOLWHEELSMOV = 15f,
            VOLFIREB = 40f,
            VOLFIRER = 40f,
            VOLFIREM = 20f,
            VOLTURRETROT = 5f,
            VOLIDLE = 5f;

        //Movement
        bool sprint, crawl;
        public bool Sprint { get { return sprint; } set { sprint = value; } }
        public bool Crawl { get { return crawl; } set { crawl = value; } }

        Vector3? wheelsVTarget, turretVTarget;
        float? wheelsFTrDif, turretFTrDif; //Difference between target and current

        public float? WheelsTargetDifference { get { return wheelsFTrDif; } }
        public float? TurretTargetDifference { get { return turretFTrDif; } }

        //Assets
        TankAssets a;
        GUI g;

        Texture2D wheelsBTex, turretBTex, barrelBTex, bannerBTex, arrowsBTex,
                  wheelsATex, turretATex, barrelATex, bannerATex;

        Color wheelsBCol, turretBCol, barrelBCol, bannerBCol, arrowsACol,
              wheelsACol, turretACol, barrelACol, bannerACol;

        Vector2 wheelsVOrigin, turretVOrigin, barrelVOrigin, arrowsVOrigin;

        //SRT Matrices
        Matrix wheelsMScale, turretMScale, barrelMScale, arrowsMScale,
               wheelsMRotat, turretMRotat, barrelMRotat, arrowsMRotat,
               wheelsMTrans, turretMTrans, barrelMTrans, arrowsMTrans,
               wheelsMFinal, turretMFinal, barrelMFinal, arrowsMFinal,
                                                         debugsMFinal;

        //Float values for Matrix Transformation
        float //Scale
              wheelsFScale, turretFScale, barrelFScale, arrowsFScale,
              wheelsFSclMx, turretFSclMx, barrelFSclMx,
              wheelsFSclMn, turretFSclMn, barrelFSclMn,
            //Rotation
              wheelsFRotat, turretFRotat, arrowsFRotat,
              wheelsFRotSp, turretFRotSp,
              wheelsFRotAc, turretFRotAc,
              wheelsFRotDe, turretFRotDe,
              wheelsFRotCh, turretFRotCh,
              wheelsFRotMx, turretFRotMx,
            //Movement
              wheelsFMovSp,
              wheelsFMovAc,
              wheelsFMovDe,
              wheelsFMovCh,
              wheelsFMovMx;

        public float WheelsFRotation { get { return wheelsFRotat; } }
        public float TurretFRotation { get { return turretFRotat; } }

        //Compound Barrel Direction
        float barrelFRotAb;
        public float BarrelDirAbs { get { return barrelFRotAb; } }

        //End of Barrel Vars
        Vector3 barrelVEndTr, barrelVEndOf, barrelVEndAb;
        public Vector3 EndOfBarrel { get { return position + barrelVEndOf; } }

        //Translation Vectors
        Vector3 wheelsVTrans, turretVTrans, barrelVTrans, arrowsVTrans;

        //Direction Vectors
        Vector3 wheelsVFront, turretVFront, barrelVFront,
                wheelsVRight, turretVRight, barrelVRight;

        public Vector3 WheelsVFront { get { return position + wheelsVFront; } }
        public Vector3 TurretVFront { get { return position + Vector3.Transform(turretVFront, wheelsMRotat); } }
        public Vector3 BarrelVFront { get { return position + barrelVFront; } }

        public Vector3 WheelsVBack { get { return position - wheelsVFront; } }
        public Vector3 TurretVBack { get { return position - Vector3.Transform(turretVFront, wheelsMRotat); } }
        public Vector3 BarrelVBack { get { return position - barrelVFront; } }

        public Vector3 WheelsVRight { get { return position + wheelsVRight; } }
        public Vector3 TurretVRight { get { return position + Vector3.Transform(turretVRight, wheelsMRotat); } }
        public Vector3 BarrelVRight { get { return position + barrelVRight; } }

        public Vector3 WheelsVLeft { get { return position - wheelsVRight; } }
        public Vector3 TurretVLeft { get { return position - Vector3.Transform(turretVRight, wheelsMRotat); } }
        public Vector3 BarrelVLeft { get { return position - barrelVRight; } }

        //Screen Centre Translation
        Vector3 centreVTrans = new Vector3(640, 360, 0);

        //Position
        Vector3 position;
        public Vector3 Position { get { return position; } }

        //Collision 
        //It's easier to see diagrammatically, use debug drawing for this
        //Collision/Sight Units are a fraction of texture size * scale to uniform collision to arbitrarily sized tanks
        float cU, sU;

        //Coarse
        //FRONT MIDDLE REAR * LEFT MIDDLE RIGHT
        //Sound, Sound

        //Vectors for positioning
        Vector3
            bvSF,
            bvSM,
            bvSN,
            bvCC,
      bvFL, bvFM, bvFR,
            bvMF,
            bvMM,
            bvMR,
      bvRL, bvRM, bvRR,
            bvSS,
            bvLS;

        //Actual spheres
        BoundingSphere
            bsSF,
            bsSM,
            bsSN,
            bsCC,
      bsFL, bsFM, bsFR,
            bsMF,
            bsMM,
            bsMR,
      bsRL, bsRM, bsRR,
            bsSS,
            bsLS;

        //Bools used for detection measures
        bool
            cdSF,
            cdSM,
            cdSN,
            cdCC,
      cdFL, cdFM, cdFR,
            cdMF,
            cdMM,
            cdMR,
      cdRL, cdRM, cdRR,
            cdLS;

        float
            sfSF,
            sfSM,
            sfSN;

        //Scoring
        int score;
        public int Score { get { return score; } set { score = value; } }

        string name;
        public string Name { get { return name; } set { name = value; } }

        #endregion

        #region Make, Break, Load

        //Constructor Player
        public Tank(TankAssets assets, GUI grUsIn, ProjManager projM, string title, Vector3? pos, float? scale, char? wheelsA, char? turretA, char? barrelA, char? bannerA, Color? colB, Color? colA)
        {
            //Make alive, set as player
            active = true;
            alive = true;
            player = true;

            //Apply passed objects
            a = assets;
            g = grUsIn;
            p = projM;

            //Apply passed vars
            wheelsVTrans = pos ?? centreVTrans;
            wheelsFScale = scale ?? 1f;
            turretFScale = scale ?? 1f;
            arrowsFScale = scale ?? 1f;
            barrelFScale = scale ?? 1f;

            //Load textures
            LoadTankTextures(wheelsA, turretA, barrelA, bannerA);

            //Paint Tank
            SetBaseCol(colB ?? Color.DarkGray);
            SetAccentCol(colA ?? Color.Cyan);

            //Load weapons
            ResetWeapons();
            ResetHealth();

            //Ready Tank
            wheelsFRotat = MathHelper.ToRadians(270);
            name = title;
            health = HEALTHMAX;
            volume = 0;

            //Methods
            UpdateFactors();
            Spawn();
        }

        //Constructor AI
        public Tank(Brain inBrain, TankAssets assets, GUI grUsIn, ProjManager projM, string title, Vector3? pos, float? scale, char? wheelsA, char? turretA, char? barrelA, char? bannerA, Color? colB, Color? colA, int? startHealth)
        {
            //AI Link
            brain = inBrain;
            inBrain.RegisterTank(this);
            lastSeen = this;
            lastHeard = this;
            lastCollided = this;

            //Make alive
            active = true;
            alive = true;

            //Apply passed objects
            a = assets;
            g = grUsIn;
            p = projM;

            //Apply passed vars
            wheelsVTrans = pos ?? centreVTrans;
            wheelsFScale = scale ?? 1f;
            turretFScale = scale ?? 1f;
            arrowsFScale = scale ?? 1f;
            barrelFScale = scale ?? 1f;

            //Set maximum scales
            wheelsFSclMx = 10;
            wheelsFSclMn = 0.5f;

            turretFSclMx = 10;
            turretFSclMn = 0.5f;

            barrelFSclMx = 10;
            barrelFSclMn = 0.5f;

            //Load textures
            LoadTankTextures(wheelsA, turretA, barrelA, bannerA);

            //Paint Tank
            SetBaseCol(colB ?? Color.DarkGray);
            SetAccentCol(colA ?? Color.Red);

            //Load weapons
            ResetWeapons();

            //Ready Tank
            wheelsFRotat = MathHelper.ToRadians(270);
            name = title;
            health = startHealth ?? HEALTHMAX;
            volume = 0;

            //Methods
            UpdateFactors();
            Spawn();
        }

        public Tank(Vector3? pos)
        {
            wheelsVTrans = pos ?? centreVTrans;
            empty = true;
        }

        private void DebugInit()
        {
        }

        /// <summary>
        /// Loads textures in a specific order, stops at the end of List. Don't fuck up the order or your tank will look really dumb.
        /// </summary>
        /// <param name="texnames">Base: Wheels, Turret, Barrel, Banner.
        /// Accent: Wheels, Turret, Barrel, Banner.
        /// </param>
        private void LoadTankTextures(char? wheelsA, char? turretA, char? barrelA, char? bannerA)
        {
            wheelsBTex = a.BaseWheels;
            turretBTex = a.BaseTurret;
            barrelBTex = a.BaseBarrel;
            arrowsBTex = a.BaseArrows;

            wheelsATex = a.Wheels(wheelsA);
            turretATex = a.Turret(turretA);
            barrelATex = a.Barrel(barrelA);
            bannerATex = a.Banner(bannerA);
        }

        public void Spawn()
        {
            //INSERT JUICE HERE
        }

        public void Reset()
        {
        }

        public void Death()
        {
            alive = false;
            active = false;
        }

        #endregion

        #region Ch-ch-ch-ch-changes

        public void SetBaseCol(Color newBCol)
        {
            wheelsBCol = newBCol;
            turretBCol = newBCol;
            barrelBCol = newBCol;
            bannerBCol = newBCol;
        }

        public void SetAccentCol(Color newACol)
        {
            wheelsACol = newACol;
            turretACol = newACol;
            barrelACol = newACol;
            bannerACol = newACol;
        }

        /// <summary>
        /// Changes color of specified parts, null retains currentTileset colour. Neat huh?
        /// </summary>
        /// <param name="wheelsBNew">Wheels Base Color</param>
        /// <param name="turretBNew">Turret Base Color</param>
        /// <param name="barrelBNew">Barrel Base Color</param>
        /// <param name="bannerBNew">Banner Base Color</param>
        /// <param name="wheelsANew">Wheels Accent Color</param>
        /// <param name="turretANew">Turret Accent Color</param>
        /// <param name="barrelANew">Barrel Accent Color</param>
        /// <param name="bannerANew">Barrel Accent Color</param>
        public void SetCols(Color? wheelsBNew, Color? turretBNew, Color? barrelBNew, Color? bannerBNew, Color? wheelsANew, Color? turretANew, Color? barrelANew, Color? bannerANew)
        {
            wheelsBCol = wheelsBNew ?? wheelsBCol;
            turretBCol = turretBNew ?? turretBCol;
            barrelBCol = barrelBNew ?? barrelBCol;
            bannerBCol = bannerBNew ?? bannerBCol;

            wheelsACol = wheelsANew ?? wheelsACol;
            turretACol = turretANew ?? turretACol;
            barrelACol = barrelANew ?? barrelACol;
            bannerACol = bannerANew ?? bannerACol;
        }

        /// <summary>
        /// Sets accent pattern for each part. Use Capital char from A-Z inclusive. null char returns random, anything else returns blank.
        /// </summary>
        /// <param name="wheels">Wheels pattern code. A-Z or null</param>
        /// <param name="turret">Turret pattern code. A-Z or null</param>
        /// <param name="barrel">Barrel pattern code. A-Z or null</param>
        /// <param name="banner">Banner pattern code. A-Z or null</param>
        public void SetAccent(char? wheels, char? turret, char? barrel, char? banner)
        {
            wheelsATex = a.Wheels(wheels);
            turretATex = a.Turret(turret);
            barrelATex = a.Barrel(barrel);
            bannerATex = a.Banner(banner);
        }

        public void CrementAccent(int part)
        {
            switch (part)
            {
                case 1:
                    char tex = wheelsATex.Name[1];

                    //If it's over the line, reset it
                    if ((char)(tex + 1) > 'Z')
                    {
                        tex = '0';
                    }

                    //If it's been reset, bump it into the alphabet
                    else if (tex == '0')
                    {
                        tex = 'A';
                    }

                    //Otherwise just bump it up by one
                    else tex = (char)(tex + 1);

                    wheelsATex = a.Wheels(tex);
                    break;

                case 2: 
                    tex = turretATex.Name[1];

                    //If it's over the line, reset it
                    if ((char)(tex + 1) > 'Z')
                    {
                        tex = '0';
                    }

                    //If it's been reset, bump it into the alphabet
                    else if (tex == '0')
                    {
                        tex = 'A';
                    }

                    //Otherwise just bump it up by one
                    else tex = (char)(tex + 1);

                    turretATex = a.Turret(tex);
                    break;

                case 3:
                    tex = barrelATex.Name[1];

                    //If it's over the line, reset it
                    if ((char)(tex + 1) > 'Z')
                    {
                        tex = '0';
                    }

                    //If it's been reset, bump it into the alphabet
                    else if (tex == '0')
                    {
                        tex = 'A';
                    }

                    //Otherwise just bump it up by one
                    else tex = (char)(tex + 1);

                    barrelATex = a.Barrel(tex);
                    break;

                case 4:
                    tex = bannerATex.Name[1];

                    //If it's over the line, reset it
                    if ((char)(tex + 1) > 'Z')
                    {
                        tex = '0';
                    }

                    //If it's been reset, bump it into the alphabet
                    else if (tex == '0')
                    {
                        tex = 'A';
                    }

                    //Otherwise just bump it up by one
                    else tex = (char)(tex + 1);

                    bannerATex = a.Banner(tex);
                    break;
            }
        }

        public void SetScale(float newFScale)
        {
            if (wheelsFSclMn <= newFScale && newFScale <= wheelsFSclMx)
            {
                wheelsFScale = newFScale;
                turretFScale = newFScale;
                barrelFScale = newFScale;
            }
        }

        public void SetScales(float? wheelsFNew, float? turretFNew, float? barrelFNew)
        {

            wheelsFScale = wheelsFNew ?? wheelsFScale;
            turretFScale = turretFNew ?? turretFScale;
            barrelFScale = barrelFNew ?? barrelFScale;
        }

        public void CrementScale(float amount)
        {
            if (wheelsFSclMn < wheelsFScale + amount && wheelsFScale + amount < wheelsFSclMx)
            {
                wheelsFScale += amount;
            }

            if (turretFSclMn < turretFScale + amount && turretFScale + amount < turretFSclMx)
            {
                turretFScale += amount;
            }

            if (barrelFSclMn < barrelFScale + amount && barrelFScale + amount < barrelFSclMx)
            {
                barrelFScale += amount;
            }
        }

        public void SetSpeeds(float? wheelsRot, float? turretRot, float? wheelsMov)
        {
            wheelsFRotAc = wheelsRot ?? wheelsFRotAc;
            turretFRotAc = turretRot ?? turretFRotAc;
            wheelsFMovAc = wheelsMov ?? wheelsFMovAc;
        }

        public void CrementSpeeds(float? wheelsRotAmount, float? turretRotAmount, float? wheelsMovAmount)
        {
            if (wheelsFRotAc + wheelsRotAmount != 0)
                wheelsFRotAc += wheelsRotAmount ?? 0;

            if (turretFRotAc + turretRotAmount != 0)
                turretFRotAc += turretRotAmount ?? 0;

            if (wheelsFMovAc + wheelsMovAmount != 0)
                wheelsFMovAc += wheelsMovAmount ?? 0;
        }

        public void SetPos(Vector3 newPos)
        {
            wheelsVTrans = newPos;
        }

        public void ToggleDebug()
        {
            drawDebug = !drawDebug;
        }

        #endregion

        #region Movement

        public void MoveFore()
        {
            if (!cdFM)//Not colliding
            {
                wheelsFMovCh = wheelsFMovAc;

                if (crawl)
                {
                    wheelsFMovCh /= 2f;
                    AddSound(VOLWHEELSMOV / 2f);
                }

                else if (sprint)
                {
                    wheelsFMovCh *= 1.5f;
                    AddSound(VOLWHEELSMOV * 1.5f);
                }

                else
                {
                    AddSound(VOLWHEELSMOV);
                }
            }

            else MoveStop();
        }

        public void MoveBack()
        {
            if (!cdRM)
            {
                wheelsFMovCh = -wheelsFMovAc;

                if (crawl)
                {
                    wheelsFMovCh /= 2f;
                    AddSound(VOLWHEELSMOV / 2f);
                }

                else if (sprint)
                {
                    wheelsFMovCh *= 1.5f;
                    AddSound(VOLWHEELSMOV * 1.5f);
                }

                else
                {
                    AddSound(VOLWHEELSMOV);
                }
            }

            else MoveStop();
        }

        public void MoveStop()
        {
            wheelsFMovCh = -wheelsFMovSp;
        }

        public void RotateWheelsRight()
        {
            if (!cdFR && !cdRR)
            {
                wheelsFRotCh += wheelsFRotAc;
                if (!sprint && !crawl)
                {
                    AddSound(VOLWHEELSROT);
                }
            }

            else RotateWheelsStop();
        }

        public void RotateWheelsLeft()
        {
            if (!cdFL && !cdRL)
            {
                wheelsFRotCh += -wheelsFRotAc;
                if (!sprint && !crawl)
                {
                    AddSound(VOLWHEELSROT);
                }
            }

            else RotateWheelsStop();
        }

        public void RotateWheelsStop()
        {
            wheelsFRotCh = -wheelsFRotSp;
        }

        public void RotateTurretRight()
        {
            turretFRotCh = turretFRotAc;
            AddSound(VOLTURRETROT);
        }

        public void RotateTurretLeft()
        {
            turretFRotCh = -turretFRotAc;
            AddSound(VOLTURRETROT);
        }

        public void SprintOn()
        {
            sprint = true;
        }

        public void SprintOff()
        {
            sprint = false;
        }

        public void CrawlOn()
        {
            crawl = true;
        }

        public void CrawlOff()
        {
            crawl = false;
        }

        public void RotateWheelsTarget(Vector3 target)
        {
            wheelsVTarget = target;
        }

        public void RotateTurretTarget(Vector3 target)
        {
            turretVTarget = target;
        }

        public void ResetTargets()
        {
            turretVTarget = null;
            wheelsVTarget = null;
        }

        public void MoveRelUp()
        {
            Vector3 up = position - Vector3.UnitY;

            if (AngleBetweenWheelsAbs(up) < 15)
            {
                RotateWheelsTarget(up);
            }

            else MoveFore();
        }

        public float AngleBetweenWheels(Vector3 target)
        {
            float deg = (float)Math.Atan2(target.Y - WheelsVFront.Y, target.X - WheelsVFront.X);
            deg -= WheelsFRotation;
            return MathHelper.ToDegrees(deg) % 360;
        }

        public float AngleBetweenTurret(Vector3 target)
        {
            float deg = (float)Math.Atan2(target.Y - TurretVFront.Y, target.X - TurretVFront.X);
            deg -= TurretFRotation;
            return Math.Abs(MathHelper.ToDegrees(deg) % 360);
        }

        public float AngleBetweenWheelsAbs(Vector3 target)
        {
            return Math.Abs(AngleBetweenWheels(target));
        }

        public float AngleBetweenTurretAbs(Vector3 target)
        {
            return Math.Abs(AngleBetweenTurret(target));
        }

        #endregion

        #region Collision

        private void UpdateCollisionFactors()
        {
            //Collision Data
            //cU (Collision Unit) is a fraction of the texture size * the scale,
            cU = (wheelsBTex.Width / 12f) * wheelsFScale;
            sU = (turretBTex.Width / 2f) * turretFScale;

            sfSF = 4.5f;
            sfSM = 3;
            sfSN = 1.5f;

            //cU is used for everything from here out, and since it's a fraction of the tank size, when you scale the tank everything else is affected

            bvSF = new Vector3(sU * (sfSF + sfSM + sfSN), sU * 1, 0);
            bvSM = new Vector3(sU * (sfSM + sfSN), sU * 1, 0);
            bvSN = new Vector3(sU * sfSN, sU * 1, 0);

            bvCC = new Vector3(cU * 6, cU * 3, 0);

            bvRL = new Vector3(cU * 1, cU * 1, 0);
            bvRM = new Vector3(cU * 2, cU * 3, 0);
            bvRR = new Vector3(cU * 1, cU * 5, 0);

            bvMR = new Vector3(cU * 4, cU * 3, 0);
            bvMM = new Vector3(cU * 6, cU * 3, 0);
            bvMF = new Vector3(cU * 8, cU * 3, 0);

            bvFL = new Vector3(cU * 11, cU * 1, 0);
            bvFM = new Vector3(cU * 10, cU * 3, 0);
            bvFR = new Vector3(cU * 11, cU * 5, 0);

            bvSS = new Vector3(cU * 6, cU * 3, 0);
            bvLS = new Vector3(cU * 6, cU * 3, 0);
        }

        private void UpdateBounds()
        {
            //Transform Vectors for rotation and correct for origin]
            bvSF = Vector3.Transform(bvSF - new Vector3(turretVOrigin * turretFScale, 0), turretMRotat * wheelsMRotat);
            bvSM = Vector3.Transform(bvSM - new Vector3(turretVOrigin * turretFScale, 0), turretMRotat * wheelsMRotat);
            bvSN = Vector3.Transform(bvSN - new Vector3(turretVOrigin * turretFScale, 0), turretMRotat * wheelsMRotat);

            bvCC = Vector3.Transform(bvCC - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);

            bvRL = Vector3.Transform(bvRL - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);
            bvRM = Vector3.Transform(bvRM - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);
            bvRR = Vector3.Transform(bvRR - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);

            bvMR = Vector3.Transform(bvMR - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);
            bvMM = Vector3.Transform(bvMM - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);
            bvMF = Vector3.Transform(bvMF - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);

            bvFL = Vector3.Transform(bvFL - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);
            bvFM = Vector3.Transform(bvFM - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);
            bvFR = Vector3.Transform(bvFR - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);

            bvSS = Vector3.Transform(bvSS - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);
            bvLS = Vector3.Transform(bvLS - new Vector3(wheelsVOrigin * wheelsFScale, 0), wheelsMRotat);

            //Create new Bounding Spheres
            bsSF = new BoundingSphere(wheelsVTrans + bvSF, sU * sfSF);
            bsSM = new BoundingSphere(wheelsVTrans + bvSM, sU * sfSM);
            bsSN = new BoundingSphere(wheelsVTrans + bvSN, sU * sfSN);

            bsCC = new BoundingSphere(wheelsVTrans + bvCC, cU * 7.5f);

            bsRL = new BoundingSphere(wheelsVTrans + bvRL, cU * 1f);
            bsRM = new BoundingSphere(wheelsVTrans + bvRM, cU * 2f);
            bsRR = new BoundingSphere(wheelsVTrans + bvRR, cU * 1f);

            bsMR = new BoundingSphere(wheelsVTrans + bvMR, cU * 3.5f);
            bsMM = new BoundingSphere(wheelsVTrans + bvMM, cU * 3.5f);
            bsMF = new BoundingSphere(wheelsVTrans + bvMF, cU * 3.5f);

            bsFL = new BoundingSphere(wheelsVTrans + bvFL, cU * 1f);
            bsFM = new BoundingSphere(wheelsVTrans + bvFM, cU * 2f);
            bsFR = new BoundingSphere(wheelsVTrans + bvFR, cU * 1f);

            bsSS = new BoundingSphere(wheelsVTrans + bvSS, cU * volume);
            bsLS = new BoundingSphere(wheelsVTrans + bvLS, cU * 30);

            if (volume != VOLIDLE)
            {
                if (--volume >= VOLIDLE)
                {
                    volume--; //Lower volume;
                }

                if (volume < VOLIDLE)
                {
                    volume = VOLIDLE;
                }
            }
        }

        private void CheckBounds()
        {
            if (position.X < -100 || position.X > 4100
             || position.Y < -100 || position.Y > 4100)
            {
                active = false;
            }
        }

        public BoundingSphere GetSphereCoarse()
        {
            return bsCC;
        }

        public BoundingSphere[] GetSpherePrecis()
        {
            return new BoundingSphere[9] { bsFL, bsFM, bsFR, bsMF, bsMM, bsMR, bsRL, bsRM, bsRR };
        }

        public BoundingSphere GetSphereSound()
        {
            return bsSS;
        }

        public BoundingSphere GetSphereListen()
        {
            return bsLS;
        }

        public bool Look()
        {
            if (cdSF || cdSM || cdSF)
                return true;
            else return false;
        }

        public bool Listen()
        {
            if (cdLS)
                return true;
            else return false;
        }

        public bool Feel()
        {
            if ((cdFL || cdFM || cdFR || cdMF || cdMM || cdMR || cdRL || cdRM || cdRR) && lastCollided != null && lastCollided != this)
                return true;
            else return false;
        }

        private void AddSound(float sound)
        {
            if (volume < sound)//Only change if current sound is quieter than new
            {
                volume = sound;
            }
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

            if (bsFL.Intersects(other))
            {
                cdFL = true;
                collision = true;
            }

            if (bsFM.Intersects(other))
            {
                cdFM = true;
                collision = true;
            }

            if (bsFR.Intersects(other))
            {
                cdFR = true;
                collision = true;
            }

            if (bsMF.Intersects(other))
            {
                cdMF = true;
                collision = true;
            }

            if (bsMM.Intersects(other))
            {
                cdMM = true;
                collision = true;
            }

            if (bsMR.Intersects(other))
            {
                cdMR = true;
                collision = true;
            }

            if (bsRL.Intersects(other))
            {
                cdRL = true;
                collision = true;
            }
            if (bsRM.Intersects(other))
            {
                cdRM = true;
                collision = true;
            }

            if (bsRR.Intersects(other))
            {
                cdRR = true;
                collision = true;
            }

            return collision;
        }

        public bool CheckSenseListen(Tank otherTank)
        {
            if (bsLS.Intersects(otherTank.GetSphereSound()))
            {
                cdLS = true;
                lastHeard = otherTank;
                return true;
            }

            else return false;
        }

        public bool CheckSenseLook(Tank otherTank)
        {
            bool collision = false;

            if (bsSN.Intersects(otherTank.GetSphereCoarse()))
            {
                cdSN = true;
                lastSeen = otherTank;
                collision = true;
            }

            if (bsSM.Intersects(otherTank.GetSphereCoarse()))
            {
                cdSM = true;
                lastSeen = otherTank;
                collision = true;
            }

            if (bsSF.Intersects(otherTank.GetSphereCoarse()))
            {
                cdSF = true;
                lastSeen = otherTank;
                collision = true;
            }

            return collision;
        }

        public void ResetCollisions()
        {
            cdCC = false;

            cdFL = false;
            cdFM = false;
            cdFR = false;

            cdMF = false;
            cdMM = false;
            cdMR = false;

            cdRL = false;
            cdRM = false;
            cdRR = false;
        }

        public void ResetSenses()
        {
            cdLS = false;
            cdSF = false;
            cdSM = false;
            cdSN = false;
        }

        #endregion

        #region Combat

        public void FireBullet()
        {
            if (cooldownBullet <= 0 && readyBullet)
            {
                p.AddProj(g, barrelVEndAb, barrelFScale, barrelFRotAb, 'B', barrelBCol, barrelACol, player);
                cooldownBullet = cooltimeBullet * UPDATE;
                ammoBullet--;
                AddSound(VOLFIREB);
                if (ammoBullet <= 0)
                {
                    readyBullet = false;
                    reloadBullet = retimeBullet * UPDATE;
                }
            }
        }

        public void FireMine()
        {
            if (cooldownMine <= 0 && readyMine)
            {
                p.AddProj(g, barrelVEndAb, barrelFScale, barrelFRotAb, 'M', barrelBCol, barrelACol, player);
                cooldownMine = cooltimeMine * UPDATE;
                ammoMine--;
                AddSound(VOLFIREM);
                if (ammoMine <= 0)
                {
                    readyMine = false;
                    reloadMine = retimeMine * UPDATE;
                }
            }
        }

        public void FireRocket()
        {
            if (cooldownRocket <= 0 && readyRocket)
            {
                p.AddProj(g, barrelVEndAb, barrelFScale, barrelFRotAb, 'R', barrelBCol, barrelACol, player);
                cooldownRocket = cooltimeRocket * UPDATE;
                ammoRocket--;
                AddSound(VOLFIRER);
                if (ammoRocket <= 0)
                {
                    readyRocket = false;
                    reloadRocket = retimeRocket * UPDATE;
                }
            }
        }

        public void FireX()
        {
            if (cooldownX <= 0 && readyX)
            {
                p.AddProj(g, barrelVEndAb, barrelFScale, barrelFRotAb, 'X', null, null, player);
                cooldownX = cooltimeX * UPDATE;
                ammoX--;
                if (ammoX <= 0)
                {
                    readyX = false;
                    reloadX = retimeX * UPDATE;
                }
            }
        }

        private void CoolWeapons()
        {
            if (cooldownBullet > 0)
            {
                cooldownBullet--;
            }

            if (cooldownMine > 0)
            {
                cooldownMine--;
            }

            if (cooldownRocket > 0)
            {
                cooldownRocket--;
            }

            if (cooldownX > 0)
            {
                cooldownX--;
            }
        }

        public void ReloadWeapons()
        {
            if (!readyBullet && reloadBullet <= 0)
            {
                ammoBullet = totalBullet;
                readyBullet = true;
            }

            else reloadBullet--;

            if (!readyMine && reloadMine <= 0)
            {
                ammoMine = totalMine;
                readyMine = true;
            }

            else reloadMine--;

            if (!readyRocket && reloadRocket <= 0)
            {
                ammoRocket = totalRocket;
                readyRocket = true;
            }

            else reloadRocket--;

            if (!readyX && reloadX <= 0)
            {
                ammoX = totalX;
                readyX = true;
            }

            else reloadX--;
        }

        public void Hit(float damage, BoundingSphere hit, Projectile p)
        {
            if (lastHit != p)
            {
                lastHit = p;

                if (damage > 0)
                {
                    if (hit.Equals(bsFL) || hit.Equals(bsFM) || hit.Equals(bsFR))
                    {
                        damage *= 0.5f;
                    }

                    if (hit.Equals(bsRL) || hit.Equals(bsRM) || hit.Equals(bsRR))
                    {
                        damage *= 2;
                    }

                    if (brain != null)
                    {
                        brain.Pain();
                    }

                    Hurt(damage);
                }

                if (damage < 0)
                {
                    Heal(damage);
                }
            }

        }

        private void Hurt(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Death();
            }
        }

        private void Heal(float damage)
        {
            if (health + damage > HEALTHMAX)
            {
                health = HEALTHMAX;
            }

            else health = health + damage;
        }

        public void ResetWeapons()
        {
            ammoBullet = totalBullet;
            readyBullet = true;
            ammoMine = totalMine;
            readyMine = true;
            ammoRocket = totalRocket;
            readyRocket = true;
        }

        public void ResetHealth()
        {
            health = HEALTHMAX;
        }

        #endregion

        #region Movement Actual

        private float? RotateWheelsTargetActual()
        {
            Vector3 dir = new Vector3(wheelsVTarget.Value.X - position.X, wheelsVTarget.Value.Y - position.Y, 0);
            Vector3 dirNorm = new Vector3(-dir.Y, dir.X, 0);

            if (Vector3.Dot(wheelsVFront, dirNorm) < 0)
            {
                RotateWheelsRight();
            }

            if (Vector3.Dot(wheelsVFront, dirNorm) > 0)
            {
                RotateWheelsLeft();
            }

            return Vector3.Dot(wheelsVFront, dir);
        }

        private float? RotateTurretTargetActual()
        {
            Vector3 dir = new Vector3(turretVTarget.Value.X - position.X, turretVTarget.Value.Y - position.Y, 0);
            Vector3 dirNorm = new Vector3(-dir.Y, dir.X, 0);

            if (Vector3.Dot(Vector3.Transform(turretVFront, wheelsMRotat), dirNorm) < 0)
            {
                RotateTurretRight();
            }

            if (Vector3.Dot(Vector3.Transform(turretVFront, wheelsMRotat), dirNorm) > 0)
            {
                RotateTurretLeft();
            }

            return Vector3.Dot(Vector3.Transform(turretVFront, wheelsMRotat), dir);
           
        }

        private void MovementActual()
        {
            //Directionality
            wheelsVFront = Vector3.Transform(Vector3.UnitX, wheelsMRotat);
            wheelsVRight = Vector3.Transform(Vector3.UnitY, wheelsMRotat);

            //Top speed adjustments
            if (sprint)
            {
                wheelsFMovMx *= 1.5f;
            }

            if (crawl)
            {
                wheelsFMovMx /= 2f;
            }

            //Add change to speed
            wheelsFMovSp += wheelsFMovCh;

            //Apply decel
            if (wheelsFMovSp > 0)
            {
                if (wheelsFMovSp - wheelsFMovDe > 0)
                {
                    wheelsFMovSp -= wheelsFMovDe;
                }

                else
                {
                    wheelsFMovSp = 0;
                }
            }

            if (wheelsFMovSp < 0)
            {
                if (wheelsFMovSp + wheelsFMovDe < 0)
                {
                    wheelsFMovSp += wheelsFMovDe;
                }

                else
                {
                    wheelsFMovSp = 0;
                }
            }

            //Limit to max speed
            if (wheelsFMovSp > wheelsFMovMx)
            {
                wheelsFMovSp = wheelsFMovMx;
            }

            else if (wheelsFMovSp < -wheelsFMovMx)
            {
                wheelsFMovSp = -wheelsFMovMx;
            }

            //Actual movement
            wheelsVTrans += wheelsVFront * (wheelsFMovSp);
        }

        private void RotateWheelsActual()
        {
            //Top speed adjustments
            if (sprint)
            {
                wheelsFRotMx /= 2f;
            }

            if (crawl)
            {
                wheelsFRotMx /= 2f;
            }

            //Add change to speed
            wheelsFRotSp += wheelsFRotCh;

            //Apply decel
            if (wheelsFRotSp > 0)
            {
                if (wheelsFRotSp - wheelsFRotDe > 0)
                {
                    wheelsFRotSp -= wheelsFRotDe;
                }

                else
                {
                    wheelsFRotSp = 0;
                }
            }

            if (wheelsFRotSp < 0)
            {
                if (wheelsFRotSp + wheelsFRotDe < 0)
                {
                    wheelsFRotSp += wheelsFRotDe;
                }

                else
                {
                    wheelsFRotSp = 0;
                }
            }

            //Limit to max speed
            if (wheelsFRotSp > wheelsFRotMx)
            {
                wheelsFRotSp = wheelsFRotMx;
            }

            else if (wheelsFRotSp < -wheelsFRotMx)
            {
                wheelsFRotSp = -wheelsFRotMx;
            }

            //Actual Rotation
            wheelsFRotat += wheelsFRotSp;

        }

        private void RotateTurretActual()
        {
            //Add change to speed
            turretFRotSp += turretFRotCh;

            //Apply decel
            if (turretFRotSp > 0)
            {
                if (turretFRotSp - turretFRotDe > 0)
                {
                    turretFRotSp -= turretFRotDe;
                }

                else
                {
                    turretFRotSp = 0;
                }
            }

            if (turretFRotSp < 0)
            {
                if (turretFRotSp + turretFRotDe < 0)
                {
                    turretFRotSp += turretFRotDe;
                }

                else
                {
                    turretFRotSp = 0;
                }
            }

            //Limit to max speed
            if (turretFRotSp > turretFRotMx)
            {
                turretFRotSp = turretFRotMx;
            }

            else if (turretFRotSp < -turretFRotMx)
            {
                turretFRotSp = -turretFRotMx;
            }

            //Actual Rotation
            turretFRotat += turretFRotSp;

            //Update directionality with new rotation
            turretVFront = Vector3.Transform(Vector3.UnitX, turretMRotat);
            turretVRight = Vector3.Transform(Vector3.UnitY, turretMRotat);
            barrelVFront = turretVFront;
            barrelVRight = turretVRight;
        }

        private void ResetChanges()
        {
            wheelsFMovCh = 0;
            wheelsFRotCh = 0;
            turretFRotCh = 0;
        }

        #endregion

        #region U&D

        /// <summary>
        /// Updates any variable that's a factor of any other variable. Called on construction and every Update cycle.
        /// </summary>
        private void UpdateFactors()
        {
            //Centers
            wheelsVOrigin = new Vector2(wheelsBTex.Width / 2, wheelsBTex.Height / 2);
            turretVOrigin = new Vector2(turretBTex.Width / 2, turretBTex.Height / 2);
            barrelVOrigin = new Vector2(barrelBTex.Width / 2, barrelBTex.Height / 2);
            arrowsVOrigin = new Vector2(arrowsBTex.Width / 2, arrowsBTex.Height / 2);

            UpdateCollisionFactors();

            //Barrel Absolutes
            barrelFRotAb = turretFRotat + wheelsFRotat;
            barrelVEndAb = position + barrelVEndOf;

            //Relative translations
            turretVTrans = Vector3.Zero;
            barrelVTrans = new Vector3(((barrelBTex.Width * barrelFScale / 2) + turretBTex.Width * turretFScale / 2), 0, 0);
            arrowsVTrans = new Vector3((float)(wheelsBTex.Width * 0.6), 0, 0);

            barrelVEndTr = new Vector3((barrelBTex.Width * barrelFScale + ((turretBTex.Width * turretFScale) / 2)), 0, 0);

            //Wheels Speed correction
            wheelsFMovAc = 1 / wheelsFScale;
            wheelsFMovDe = wheelsFMovAc * 0.25f;
            wheelsFMovMx = wheelsFScale * 5f;

            //Wheel Rotate Speed Correction
            wheelsFRotAc = 0.02f / wheelsFScale;
            wheelsFRotDe = wheelsFRotAc * 0.25f;
            wheelsFRotMx = 0.05f / wheelsFScale;

            //Turret Rotate Speed Correction
            turretFRotAc = 0.005f / turretFScale;
            turretFRotDe = turretFRotAc * 0.5f;
            turretFRotMx = 0.05f / turretFScale;
        }

        private void UpdateMatrices(Matrix cameraMFinal)
        {
            //Scale (Uniform is best to fit sprites together)
            wheelsMScale = Matrix.CreateScale(wheelsFScale, wheelsFScale, 0);
            turretMScale = Matrix.CreateScale(turretFScale, turretFScale, 0);
            barrelMScale = Matrix.CreateScale(barrelFScale, barrelFScale, 0);
            arrowsMScale = Matrix.CreateScale(arrowsFScale, arrowsFScale, 0);

            //Rotation
            wheelsMRotat = Matrix.CreateRotationZ(wheelsFRotat);
            turretMRotat = Matrix.CreateRotationZ(turretFRotat);
            barrelMRotat = Matrix.Identity;
            arrowsMRotat = Matrix.Identity;

            //Translation
            wheelsMTrans = Matrix.CreateTranslation(wheelsVTrans);
            turretMTrans = Matrix.CreateTranslation(turretVTrans);
            barrelMTrans = Matrix.CreateTranslation(barrelVTrans);
            arrowsMTrans = Matrix.CreateTranslation(arrowsVTrans);

            //Get Position
            position.X = wheelsMTrans.M41;
            position.Y = wheelsMTrans.M42;

            //Point Vectors
            wheelsVFront = Vector3.Transform(Vector3.UnitX, wheelsMRotat);
            turretVFront = Vector3.Transform(Vector3.UnitX, turretMRotat);

            //EndOfBarrel
            barrelVEndOf = Vector3.Transform(barrelVEndTr, wheelsMRotat * turretMRotat);

            //Finals
            wheelsMFinal = wheelsMScale * wheelsMRotat * wheelsMTrans * cameraMFinal;
            turretMFinal = turretMScale * turretMRotat * turretMTrans * wheelsMRotat * wheelsMTrans * cameraMFinal;
            barrelMFinal = barrelMScale * barrelMRotat * barrelMTrans * turretMRotat * turretMTrans * wheelsMRotat * wheelsMTrans * cameraMFinal;
            arrowsMFinal = arrowsMScale * arrowsMRotat * arrowsMTrans * wheelsMFinal;
            debugsMFinal = cameraMFinal;
        }

        private void UpdateTargets()
        {
            if (wheelsVTarget != null)
            {
                wheelsFTrDif = RotateWheelsTargetActual();
            }

            if (turretVTarget != null)
            {
                turretFTrDif = RotateTurretTargetActual();
            }

            ResetTargets();
        }

        public void Update(GameTime gameTime, Matrix cameraMFinal, bool pause)
        {

            //Gameplay updates

            if (!empty)
            {
                UpdateFactors();
                if (!pause)
                {
                    if (!player && !controlled)
                    {
                        brain.Update();
                    }
                    CoolWeapons();
                    ReloadWeapons();
                    UpdateTargets();
                    RotateWheelsActual();
                    MovementActual();
                    RotateTurretActual();
                    UpdateBounds();
                }

                ResetChanges();
                CheckBounds();
            }
            UpdateMatrices(cameraMFinal);
        }

        public void Draw(SpriteBatch sb)
        {
            //DEBUG
            sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null);
            sb.End();
            //DEBUGEND
            if (active && !empty)
            {
                if (alive)
                {
                    //Wheels
                    sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, wheelsMFinal);
                    sb.Draw(wheelsBTex, Vector2.Zero, null, wheelsBCol, 0, wheelsVOrigin, 1, SpriteEffects.None, 0.35f);
                    sb.Draw(wheelsATex, Vector2.Zero, null, wheelsACol, 0, wheelsVOrigin, 1, SpriteEffects.None, 0.3f);
                    sb.End();

                    //Arrow
                    if (current)
                    {
                        sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, arrowsMFinal);
                        sb.Draw(arrowsBTex, Vector2.Zero, null, Color.Green, 0, arrowsVOrigin, 1, SpriteEffects.None, 0);
                        sb.End();
                    }

                    //Barrel
                    sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, barrelMFinal);
                    sb.Draw(barrelBTex, Vector2.Zero, null, barrelBCol, 0, barrelVOrigin, 1, SpriteEffects.None, 0.15f);
                    sb.Draw(barrelATex, Vector2.Zero, null, barrelACol, 0, barrelVOrigin, 1, SpriteEffects.None, 0.1f);
                    sb.End();

                    //Turret            
                    sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, turretMFinal);
                    sb.Draw(turretBTex, Vector2.Zero, null, turretBCol, 0, turretVOrigin, 1, SpriteEffects.None, 0.25f);
                    sb.Draw(turretATex, Vector2.Zero, null, turretACol, 0, turretVOrigin, 1, SpriteEffects.None, 0.2f);
                    sb.End();

                    //GUI junk

                    DebugDraw(sb);
                }
            }
        }

        private void DebugDraw(SpriteBatch sb)
        {
            //g.drawHUD(currentTileset tank, banner etc.)
            if (drawDebug && !empty)
            {
                sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, debugsMFinal);

                //Collision Spheres
                g.DrawSphere(bsCC, new Color(255, 000, 255, 096), false);
                g.DrawSphere(bsMM, new Color(255, 255, 255, 127), cdMM);
                g.DrawSphere(bsMR, new Color(000, 000, 255, 127), cdMR);
                g.DrawSphere(bsMF, new Color(000, 000, 255, 127), cdMF);
                g.DrawSphere(bsRL, new Color(000, 255, 000, 127), cdRL);
                g.DrawSphere(bsRM, new Color(255, 255, 000, 127), cdRM);
                g.DrawSphere(bsRR, new Color(255, 000, 000, 127), cdRR);
                g.DrawSphere(bsFL, new Color(000, 255, 000, 127), cdFL);
                g.DrawSphere(bsFM, new Color(255, 255, 000, 127), cdFM);
                g.DrawSphere(bsFR, new Color(255, 000, 000, 127), cdFR);

                //Sound
                g.DrawSphere(bsSS, new Color(000, 127, 255, 096), false);
                g.DrawSphere(bsLS, new Color(255, 127, 255, 096), cdLS);

                //Sight
                g.DrawSphere(bsSF, new Color(255, 255, 000, 064), cdSF);
                g.DrawSphere(bsSM, new Color(255, 255, 000, 064), cdSM);
                g.DrawSphere(bsSN, new Color(255, 255, 000, 064), cdSN);

                //Vectors

                g.DrawVector3(position.X, position.Y - 40, "", WheelsVBack, null);
                g.DrawString(position.X, position.Y - 20, "", name, Color.White);
                g.DrawVector3(position.X, position.Y + 0, "Pos: ", wheelsVTrans, null);
                g.DrawFloat(position.X, position.Y + 20, "Speed: ", wheelsFMovSp, null);
                g.DrawFloat(position.X, position.Y + 40, "Health: ", health, null);

                if (brain == null)
                {
                    g.DrawFloat(position.X, position.Y + 60, "Radius: ", bsSS.Radius / cU, null);
                    g.DrawFloat(position.X, position.Y + 80, "Distance Between: ", wheelsFTrDif, null);
                }

                if (brain != null)
                {
                    brain.Draw(g);
                    g.DrawFloat(position.X, position.Y + 80, "Seen: ", brain.Seen, null);
                    g.DrawFloat(position.X, position.Y + 100, "SeenRate: ", brain.SeenRate, null);
                    g.DrawVector3(position.X, position.Y + 120, "LastSeen: ", lastSeen.Position, null);
                    g.DrawFloat(position.X, position.Y + 140, "Heard: ", brain.Heard, null);
                    g.DrawFloat(position.X, position.Y + 160, "HeardRate: ", brain.HeardRate, null);
                    g.DrawVector3(position.X, position.Y + 180, "lastHeard: ", lastHeard.Position, null);
                    g.DrawFloat(position.X, position.Y + 200, "Felt: ", brain.Felt, null);
                    g.DrawFloat(position.X, position.Y + 220, "FeltRate: ", brain.FeltRate, null);
                    g.DrawVector3(position.X, position.Y + 240, "lastFelt: ", lastCollided.Position, null);
                    g.DrawString(position.X, position.Y + 260, "State: ", brain.StateCurString(), null);
                }
                //g.DrawVector3(position.X, position.Y + 20, "Wheels: ", wheelsVFront, null);
                //g.DrawVector3(position.X, position.Y + 40, "Barrel: ", barrelVFront, null);
                //g.DrawFloat(position.X, position.Y + 60, "Scale: ", wheelsFScale, null);
                //g.DrawFloat(position.X, position.Y + 100, "Accel: ", wheelsFMovAc, null);
                //g.DrawFloat(position.X, position.Y + 120, "Decel: ", wheelsFMovDe, null);
                //g.DrawFloat(position.X, position.Y + 140, "Max: ", wheelsFMovMx, null);
                //g.DrawFloat(position.X, position.Y + 160, "WheelsRot: ", wheelsFRotAc, null);
                //g.DrawBool(position.X, position.Y + 180, "", cdCC, null);


                sb.End();
            }
        }

        #endregion

    }
}