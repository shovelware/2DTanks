using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TankGame
{
    class TankManager
    {
#region MVars

        TankAssets assets;
        List<Tank> tankList = new List<Tank>();

        ProjManager projM;

        int playerScore;
        int playerKills;

        public int PlayerScore { get { return playerScore; } }
        public int PlayerKills { get { return playerKills; } }

        int savedScore;
        int savedKills;

        int currentTank;
        public int CurrentTankNum { get { return currentTank; } set { currentTank = value; } }

        public List<Tank> TankList { get { return tankList; } }

        bool multiTank;
        public bool MultiTank { get { return multiTank; } }

#endregion

#region Make

        public TankManager(TankAssets tankAssets, ProjManager inProjM)
        {
            assets = tankAssets;
            projM = inProjM;
        }

        public void RegisterProjectileManager(ProjManager inProjM)
        {
            projM = inProjM;
        }

#endregion

#region Management
          
        public void AddPlayer(GUI gui, Vector3? pos, float? scale, Color? colB, Color? colA, bool current)
        {
            tankList.Add(new Tank(assets, gui, projM, "player"+(PlayerList().Count + 1), pos, scale, '0', '0', '0', null, colB, colA));

            if (current)
            {
                tankList[currentTank].Current = false;
                currentTank = tankList.IndexOf(tankList.Last<Tank>());
                tankList[currentTank].Current = true;
            }
        }

        public void AddAI(GUI gui, Vector3? pos, float? scale, Color? colB, Color? colA, int? startHealth)
        {
            Brain newBrain = new Brain();
            Tank newTank = new Tank(newBrain, assets, gui, projM,"enemy"+(tankList.Count + 1 - PlayerList().Count), pos, scale, null, null, null, null, colB, colA, startHealth);

            tankList.Add(newTank);
        }

        public void AddBoss(GUI gui, Vector3? pos, float? scale, Color? colB, Color? colA, int? startHealth)
        {
            Brain newBrain = new Brain();
            Tank newTank = new Tank(newBrain, assets, gui, projM, "enemy" + (tankList.Count + 1 - PlayerList().Count), pos, scale, 'O', 'O', 'O', 'O', colB, colA, startHealth);

            tankList.Add(newTank);
        }

        public void MoveCurrent(Vector3 newPos)
        {
            tankList[currentTank].SetPos(newPos);
        }

        public Tank CurrentTank() 
        {
            if (tankList.Count != 0)
            {
                return tankList[currentTank];
            }

            else
            {
                if (tankList.Count == 0)
                {
                    tankList.Add(new Tank(null));
                }
                return tankList[0];
            }
        } 

        public int TotalTanks()
        {
            return tankList.Count;
        }

        public int TotalPlayers()
        {
            return PlayerList().Count;
        }

        public void AssumeDirectControl()
        {
            if (!multiTank)
            {
                tankList[currentTank].Controlled = !tankList[currentTank].Controlled;
            }
        }

        public void CrementCurrentTank(int direction)
        {
            int dir = direction / Math.Abs(direction);

            if (currentTank + dir >= 0 && currentTank + dir <= tankList.Count - 1)
            {
                tankList[currentTank].Current = false;
                currentTank += dir;
                tankList[currentTank].Current = true;
            }
        }

        public void SetCurrentTank(int newCurrent)
        {
            if (newCurrent >= 0 && newCurrent <= tankList.Count)
            {
                tankList[currentTank].Current = false;
                currentTank = newCurrent;
                tankList[currentTank].Current = true;
            }
        }

        public void ToggleMultiTank()
        {
            if (!multiTank)
            {
                foreach (Tank t in tankList)
                {
                    t.Current = true;
                }
                multiTank = true;
            }

            else if (multiTank)
            {
                foreach (Tank t in tankList)
                {
                    t.Current = false;
                }
                tankList[currentTank].Current = true;
                multiTank = false;
            }
        }

        public List<Tank> GetTankList()
        {
            return tankList;
        }

        public void KillCurrentTank()
        {
            if (!multiTank)
            {
                tankList[currentTank].Death();
            }
        }

        private void Cleanup()
        {
            for (int t = 0; t < tankList.Count; t++)
            {
                if (tankList[t].Active == false)// If inactive
                {
                    
                    if (tankList[t].LastHit != null && tankList[t].LastHit.Player)
                    {
                        playerKills++;
                        playerScore += 100;
                    }

                    if (t == currentTank || t < currentTank)//If we need to change current tank
                    {
                        CrementCurrentTank(-1);
                    }

                    tankList.RemoveAt(t);

                    if (tankList.Count == 0)//If the tank List is empty
                    {
                        
                    }
                }
            }
        }

        private List<int> PlayerList()
        {
            List<int> players = new List<int>();
            foreach (Tank t in tankList)
            {
                if (t.Player)
                {
                    players.Add(tankList.IndexOf(t));
                }
            }

            if (players.Count <= 0)//Dance check
            {
                PlayerDead();
            }

            else
            {
                PlayerAlive();
            }

            return players;
        }

        private void PlayerDead()
        {
                foreach (Tank t in tankList)
                {
                    if (t.Brain != null)
                    t.Brain.DanceOff();
                }
        }


        private void PlayerAlive()
        {
            foreach (Tank t in tankList)
            {
                if (t.Brain != null && t.Brain.StateCur == 4)
                    
                t.Brain.DanceCancel();
            }
        }

        public void KillAllButCurrent()
        {
            foreach (Tank t in tankList)
            {
                if (!t.Current)
                {
                    t.Death();
                }
            }
        }

        /// <summary>
        /// -1 = Death
        /// 0 = Not complete
        /// 1 = Success
        /// </summary>
        /// <returns></returns>
        public int CheckLevel()
        {
            bool enemies = false;
            bool player = false;

            int levelStatus = 0; //Defaults to none

            foreach (Tank t in TankList)//Check if there's a player, 
            {
                if (t.Player)
                    player = true;
            }

            foreach (Tank t in TankList)//Check if there's any enemies
            {
                if (!t.Player)
                    enemies = true;
            }

            if (player && !enemies)//Player and no enemies, success
                levelStatus = 1;
            else if (player && enemies)//Player and enemies, pending
                levelStatus = 0;
            else if (!player && enemies)//No player and enemies, failure
                levelStatus = -1;

            return levelStatus;
        }

        public void SaveScore()
        {
            savedScore = playerScore;
            savedKills = playerKills;
        }

        public void LoadScore()
        {
            playerScore = savedScore;
            playerKills = savedKills;
        }

        public int Remaining()
        {
            int remaining = 0;

            foreach (Tank t in tankList)
            {
                if (!t.Player)
                    remaining++;
            }

            return remaining;
        }

        //AddMultiple

        //Kill

        //KillAll


        //ResetAll

#endregion

#region Ch-ch-ch-ch-changes

        public void SetBaseColor(Color newBCol)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].SetBaseCol(newBCol);
                }
            }

            else
            {
                tankList[currentTank].SetBaseCol(newBCol);
            }
        }

        public void SetAccentColor(Color newACol)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].SetAccentCol(newACol);
                }
            }

            else
            {
                tankList[currentTank].SetAccentCol(newACol);
            }
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
        public void SetColours(Color? wheelsBNew, Color? turretBNew, Color? barrelBNew, Color? bannerBNew, Color? wheelsANew, Color? turretANew, Color? barrelANew, Color? bannerANew)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].SetCols(wheelsBNew, turretBNew, barrelBNew, bannerBNew, wheelsANew, turretANew, barrelANew, bannerANew);
                }
            }

            else
            {
                tankList[currentTank].SetCols(wheelsBNew, turretBNew, barrelBNew, bannerBNew, wheelsANew, turretANew, barrelANew, bannerANew);
            }
        }

        public void SetAccent(char? wheels, char? turret, char? barrel, char? banner)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].SetAccent(wheels, turret, barrel, banner);
                }
            }

            else
            {
                tankList[currentTank].SetAccent(wheels, turret, barrel, banner);
            }
            
        }

        public void CrementAccent(int part)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].CrementAccent(part);
                }
            }

            else
            {
                tankList[currentTank].CrementAccent(part);
            }
        }

        public void SetScale(float newFScale)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].SetScale(newFScale);
                }
            }

            else
            {
                tankList[currentTank].SetScale(newFScale);
            }
        }

        public void SetScales(float? wheelsFNew, float? turretFNew, float? barrelFNew)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].SetScales(wheelsFNew, turretFNew, barrelFNew);
                }
            }

            else
            {
                tankList[currentTank].SetScales(wheelsFNew, turretFNew, barrelFNew);
            }
        }

        public void SetSpeeds(float? wheelsRot, float? turretRot, float? wheelsMov)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].SetSpeeds(wheelsRot, turretRot, wheelsMov);
                }
            }

            else
            {
                tankList[currentTank].SetSpeeds(wheelsRot, turretRot, wheelsMov);
            }
        }

        public void CrementScale(float amount)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].CrementScale(amount);
                }
            }

            else
            {
                tankList[currentTank].CrementScale(amount);
            }
        }

        public void CrementSpeeds(float? wheelsRotAmount, float? turretRotAmount, float? wheelsMovAmount)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].CrementSpeeds(wheelsRotAmount, turretRotAmount, wheelsMovAmount);
                }
            }

            else
            {
                tankList[currentTank].CrementSpeeds(wheelsRotAmount, turretRotAmount, wheelsMovAmount);
            }
        }

        public void MaintainCurrent()
        {
            tankList[currentTank].ResetHealth();
            tankList[currentTank].ResetWeapons();
        }

        public void ToggleDebug()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].ToggleDebug();
                }
            }

            else
            {
                tankList[currentTank].ToggleDebug();
            }
        }

#endregion

#region Movement
        
        public void MoveFore()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].MoveFore();
                }
            }

            else
            {
                tankList[currentTank].MoveFore();
            }
        }

        public void MoveBack()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].MoveBack();
                }
            }

            else
            {
                tankList[currentTank].MoveBack();
            }
        }

        public void MoveStop()
        {
        }

        public void RotateWheelsRight()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].RotateWheelsRight();
                }
            }

            else
            {
                tankList[currentTank].RotateWheelsRight();
            }
        }

        public void RotateWheelsLeft()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].RotateWheelsLeft();
                }
            }

            else
            {
                tankList[currentTank].RotateWheelsLeft();
            }
        }

        public void RotateTurretRight()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].RotateTurretRight();
                }
            }

            else
            {
                tankList[currentTank].RotateTurretRight();
            }
        }

        public void RotateTurretLeft()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].RotateTurretLeft();
                }
            }

            else
            {
                tankList[currentTank].RotateTurretLeft();
            }
        }

        public void SprintOn()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].SprintOn();
                }
            }

            else
            {
                tankList[currentTank].SprintOn();
            }
        }

        public void SprintOff()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].SprintOff();
                }
            }

            else
            {
                tankList[currentTank].SprintOff();
            }
        }

        public void CrawlOn()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].CrawlOn();
                }
            }

            else
            {
                tankList[currentTank].CrawlOn();
            }
        }

        public void CrawlOff()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].CrawlOff();
                }
            }

            else
            {
                tankList[currentTank].CrawlOff();
            }
        }

        public void MoveRelUp()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].MoveRelUp();
                }
            }

            else
            {
                tankList[currentTank].MoveRelUp();
            }
        }

        public void MoveRelRight()
        {
        }

        public void MoveRelLeft()
        {
        }

        public void MoveRelDown()
        {
        }

        public void AimRelFore()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].RotateTurretTarget(tankList[i].WheelsVFront);
                }
            }

            else
            {
                tankList[currentTank].RotateTurretTarget(tankList[currentTank].WheelsVFront);
            }
        }

        public void AimRelRight()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].RotateTurretTarget(tankList[i].WheelsVRight);
                }
            }

            else
            {
                tankList[currentTank].RotateTurretTarget(tankList[currentTank].WheelsVRight);
            }
        }

        public void AimRelLeft()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].RotateTurretTarget(tankList[i].WheelsVLeft);
                }
            }

            else
            {
                tankList[currentTank].RotateTurretTarget(tankList[currentTank].WheelsVLeft);
            }
        }

        public void AimRelBack()
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].RotateTurretTarget(tankList[i].WheelsVBack);
                }
            }

            else
            {
                tankList[currentTank].RotateTurretTarget(tankList[currentTank].WheelsVBack);
            }
        }

        public void RotateWheelsTarget(Vector3 target)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].RotateWheelsTarget(target);
                }
            }

            else
            {
                tankList[currentTank].RotateWheelsTarget(target);
            }
        }

        public void RotateTurretTarget(Vector3 target)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].RotateTurretTarget(target);
                }
            }

            else
            {
                tankList[currentTank].RotateTurretTarget(target);
            }
        }

#endregion

#region Collision

        public void CheckCollisions()
        {
            ResetCollisions();

            foreach (Tank t in tankList)//One Tank
            {
                foreach (Tank o in tankList)//Check against others
                {
                    if (tankList.IndexOf(t) != tankList.IndexOf(o))//Make sure not same tank
                    {
                        if (t.CheckCollisonCoarse(o.GetSphereCoarse()))//Coarse collision
                        {
                            foreach (BoundingSphere ob in o.GetSpherePrecis())//Fine collision
                            {
                                if (t.CheckCollisionPrecis(ob))
                                {
                                    t.LastCollided = o;
                                    o.LastCollided = t;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CheckSenses()
        {
            ResetSenses();

            foreach (Tank t in tankList)//One Tank
            {
                if (!PlayerList().Contains(tankList.IndexOf(t)))//Not a Player
                {
                    foreach (int p in PlayerList())//Check against others
                    {
                        if (tankList.IndexOf(t) != p)//Make sure not same tank
                        {
                            if (p < tankList.Count)
                            {
                                t.CheckSenseListen(tankList[p]);//Check Listen Sphere against Sound Sphere
                            }
                        }
                    }
                }
            }
        

            foreach (Tank t in tankList)//One Tank
            {
                if (!PlayerList().Contains(tankList.IndexOf(t)))//Not a Player
                {
                    foreach (int p in PlayerList())//Check against others
                    {
                        if (tankList.IndexOf(t) != p)//Make sure not same tank
                        {
                            if (p < tankList.Count)
                            {
                                t.CheckSenseLook(tankList[p]);//Check Sight Sphere against Coarse Sphere
                            }
                        }
                    }
                }
            }
        }

        private void ResetCollisions()
        {
            foreach (Tank t in tankList)
            {
                t.ResetCollisions();
            }
        }

        private void ResetSenses()
        {
            foreach (Tank t in tankList)//One Tank
            {
                if (!PlayerList().Contains(tankList.IndexOf(t)))//Not a Player
                {
                    t.ResetSenses();
                }
            }
        }

#endregion

#region Combat

        public void FireBullet(ProjManager p, GUI gui)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].FireBullet();
                }
            }

            else
            {
                tankList[currentTank].FireBullet();
            }
        }

        public void FireMine(ProjManager p, GUI gui)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].FireMine();
                }
            }

            else
            {
                tankList[currentTank].FireMine();
            }
        }

        public void FireRocket(ProjManager p, GUI gui)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].FireRocket();
                }
            }

            else
            {
                tankList[currentTank].FireRocket();
            }
        }

        public void FireX(ProjManager p, GUI gui)
        {
            if (multiTank)
            {
                foreach (int i in PlayerList())
                {
                    tankList[i].FireX();
                }
            }

            else
            {
                tankList[currentTank].FireX();
            }
        }

#endregion

#region U&D

        public void Update(GameTime gameTime, Matrix cameraM, bool pause)
        {
            foreach (Tank t in tankList)
            {
                t.Update(gameTime, cameraM, pause);
            }
            CheckCollisions();
            CheckSenses();
            Cleanup();

            if (tankList.Count == 0)
            {
                tankList.Add(new Tank(null));
            }
        }

        public void ForceUpdate(GameTime gameTime, Matrix cameraM)
        {
            foreach (Tank t in tankList)
            {
                t.Update(gameTime, cameraM, false);
            }
            CheckCollisions();
            CheckSenses();
            Cleanup();

            if (tankList.Count == 0)
            {
                tankList.Add(new Tank(null));
            }
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            foreach (Tank t in tankList)
            {
                t.Draw(sb);
            }
        }

#endregion
    }
}
