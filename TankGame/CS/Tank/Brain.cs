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
 
//Fix tank manager stepping

namespace TankGame
{
    class Brain
    {

        #region MVars

        Tank tank;
        bool registered;

        const byte IDLE = 0, SEARCH = 1, COMBAT = 2, FLEE = 3, DANCE = 4;
        byte stateCur, statePrv;

        public byte StateCur { get { return stateCur; } }

        Tank lastTankSeen, lastTankHeard, lastTankFelt;

        Vector3 lastFriendlySeen, lastFriendlyHeard, lastFriendlyFelt,
                lastPlayerSeen, lastPlayerHeard, lastPlayerFelt,
                wheelsTarget, turretTarget,
                aim, spawnPos, patrolPos;

        int? timeFriendlySeen, timeFriendlyHeard, timeFriendlyFelt, timePlayerSeen, timePlayerHeard, timePlayerFelt;

        const float SEENMAX = 1, HEARDMAX = 1, FELTMAX = 1,
                    SEENREM = 0.01f, HEARDREM = 0.01f, FELTREM = 0.1f;
        float seen, heard, felt,
              seenRate, heardRate, feltRate,
              seenSens, heardSens, feltSens,
              fleeThresh, accuracy, wheelsTol, aimTol;

        bool leftRight;

        public float Seen { get { return seen; } }
        public float Heard { get { return heard; } }
        public float Felt { get { return felt; } }
        public float SeenRate { get { return seenRate; } }
        public float HeardRate { get { return heardRate; } }
        public float FeltRate { get { return feltRate; } }
        private Vector3 BestSense { get { return CalcBestSensePlayer(); } }


        #endregion

        #region Make

        public Brain()
        {
            //Tolerance
            wheelsTol = 15;
            aimTol = 15;

            //Reset detection
            seen = 0;
            heard = 0;
            felt = 0;

            //Sensitivity
            seenSens = 25;
            heardSens = 25;
            feltSens = 100f;
            fleeThresh = 20f;

            //Combat
            accuracy = 1000;

            

            //Set States
            stateCur = IDLE;
            statePrv = IDLE;

        }

        /// <summary>
        /// Registers Tank as slave of Brain. Required for all functionality
        /// </summary>
        /// <param name="inTank">Tank to be connected</param>
        public void RegisterTank(Tank inTank)
        {
            tank = inTank;

            //Set positions
            spawnPos = inTank.Position;
            patrolPos = spawnPos;

            registered = true;
        }

        #endregion

        #region Senses

        private void Sight()
        {
            if (tank.Look())//If there's something to be seen (Colliding with sight spheres)
            {
                See();
            }

            if (seen > 0)//Cooldown sight.
            {
                seen -= SEENREM;

                if (seen < 0)
                {
                    seen = 0;
                }
            }
        }

        private void Sound()
        {
            if (tank.Listen())//If there's something to be heard (Colliding with Listen Sphere)
            {
                Hear();
            }

            if (heard > 0)
            {
                heard -= HEARDREM;

                if (heard < 0)
                {
                    heard = 0;
                }
            }
        }

        private void Touch()
        {
            if (tank.Feel())//If there's somthing to be felt (Collision)
            {
                Feel();
            }

            if (felt > 0)
            {
                felt -= FELTREM;

                if (felt < 0)
                {
                    felt = 0;
                }
            }
        }

        private void See()
        {
            //How fast the tank cops player, calculated by distance between centres and sensitivity
            seenRate = seenSens / Vector3.Distance(tank.Position, lastTankSeen.Position);

            if (seen < SEENMAX && lastTankSeen.Alive)
            {
                seen += seenRate;
            }

            else if (seen >= SEENMAX)
            {
                if (lastTankSeen.Player && lastTankSeen.Alive)
                {
                    lastPlayerSeen = lastTankSeen.Position;
                    timePlayerSeen = 0;
                }

                else if (lastTankSeen != tank && lastTankSeen.Alive)
                {
                    lastFriendlySeen = lastTankSeen.Position;
                    timeFriendlySeen = 0;
                }
            }
        }

        private void Hear()
        {
            heardRate = heardSens / Vector3.Distance(tank.Position, lastTankHeard.Position);

            if (heard < HEARDMAX && lastTankHeard.Alive)
            {
                heard += heardRate;
            }

            else if (heard >= HEARDMAX)
            {
                if (lastTankHeard.Player && lastTankHeard.Alive)
                {
                    lastPlayerHeard = lastTankHeard.Position;
                    timePlayerHeard = 0;
                }

                else if (lastTankHeard != tank && lastTankHeard.Alive)
                {
                    lastFriendlyHeard = lastTankHeard.Position;
                    timeFriendlyHeard = 0;
                }
            }
        }

        private void Feel()
        {
             feltRate = feltSens / Vector3.Distance(tank.Position, lastTankFelt.Position);

            if (felt < FELTMAX && lastTankFelt.Alive)
            {
                felt += feltRate;
            }

            else if (felt >= FELTMAX)
            {
                if (lastTankFelt.Player && lastTankFelt.Alive)
                {
                    lastPlayerFelt = lastTankFelt.Position;
                    timePlayerFelt = 0;
                }

                else if (lastTankFelt != tank && lastTankFelt.Alive)
                {
                    lastFriendlyFelt = lastTankFelt.Position;
                    timeFriendlyFelt = 0;
                }
            }
        }

        public void Pain()
        {
            statePrv = stateCur;
            stateCur = COMBAT;
        }

        private void WatchEdges()
        {
            if (tank.Position.X < 256 || tank.Position.X > 3600 || tank.Position.Y < 256 || tank.Position.Y > 3600)
            {
                MoveToward(new Vector3(2048, 2048, 0));
            }
        }

        #endregion

        #region Movement Basic

        private void TurnToward(Vector3 target)
        {
            if (AngleBetweenWheelsAbs(target) > wheelsTol)
            {
                tank.RotateWheelsTarget(target);
            }
        }

        private void TurnTowardSlow(Vector3 target)
        {
            tank.SprintOff();
            tank.CrawlOn();
            if (AngleBetweenWheelsAbs(target) > wheelsTol)
            {
                tank.RotateWheelsTarget(target);
            }
        }

        private void TurnTurretToward(Vector3 target)
        {
            if (AngleBetweenTurretAbs(target) > aimTol)
            {
                tank.RotateTurretTarget(target);
            }
        }

        private void MoveFore()
        {
            tank.SprintOff();
            tank.CrawlOff();
            tank.MoveFore();

        }

        private void MoveForeSlow()
        {
            tank.SprintOff();
            tank.CrawlOn();
            tank.MoveFore();
        }

        private void MoveForeFast()
        {
            tank.SprintOn();
            tank.CrawlOff();
            tank.MoveFore();
        }

        private void MoveStop()
        {
            tank.MoveStop();
        }

        private void MoveBack()
        {
            tank.MoveBack();
        }

        private void Fire()
        {
            tank.FireRocket();
        }

        private float DistanceTo(Vector3 target)
        {
            return Vector3.Distance(tank.Position, target);
        }

        private float AngleBetweenWheels(Vector3 target)
        {
            float deg = (float)Math.Atan2(target.Y - tank.WheelsVFront.Y, target.X - tank.WheelsVFront.X);
            deg -= tank.WheelsFRotation;
            return MathHelper.ToDegrees(deg) % 360;
        }

        private float AngleBetweenTurret(Vector3 target)
        {
            float deg = (float)Math.Atan2(target.Y - tank.TurretVFront.Y, target.X - tank.TurretVFront.X);
            deg -= tank.TurretFRotation;
            return Math.Abs(MathHelper.ToDegrees(deg) % 360);
        }

        private float AngleBetweenWheelsAbs(Vector3 target)
        {
            return Math.Abs(AngleBetweenWheels(target));
        }

        private float AngleBetweenTurretAbs(Vector3 target)
        {
            return Math.Abs(AngleBetweenTurret(target));
        }

        #endregion

        #region Movement Advanced

        //Moves & turns towards target, Sprints if target is distant, doesn't move if target is close
        private void MoveToward(Vector3 target)
        {
            TurnToward(target);

            //Only move if I'm facing at least some ways towards target
            if (AngleBetweenWheelsAbs(target) < 90)
            {
                if (DistanceTo(target) > 1500)
                {
                    MoveForeFast();
                }

                //If he's far away, move fast
                else if (DistanceTo(target) > 500)
                {
                    MoveFore();
                }
            }
        }

        //Aims towards target with 15 degree tolerance to stop vibrating
        private void AimToward(Vector3 target)
        {
            if (AngleBetweenTurretAbs(target) > aimTol)
            {
                TurnTurretToward(target);
            }
        }

        #endregion

        #region IDLE

        private void LookAround()
        {
            //if (!leftRight)
            //{
            //    AimToward(tank.WheelsVRight);

            //    if (AngleBetweenTurretAbs(tank.WheelsVRight) < -45)
            //    {
            //        leftRight = !leftRight;
            //    }
            //}

            //else if (leftRight)
            //{
            //    AimToward(tank.WheelsVLeft);

            //    if (AngleBetweenTurretAbs(tank.WheelsVLeft) <- 45)
            //    {
            //        leftRight = !leftRight;
            //    }
            //}
        }

        private void LookAllAround()
        {
            AimToward(tank.WheelsVFront);
        }


        private void Patrol(Vector3 patrolCentre, float patrolRadius)
        {
            //If far away, move to patrolCentre
            //Move around patrolCentre
        }

        private void IDLEStateCheck()
        {
            //If anything is sensed, go take a peep
            if (seen > 0.25 || heard > 0.25 || felt > 0.25)
            {
                stateCur = SEARCH;
                statePrv = IDLE;
            }

            //If I've looked around before I'll enter search state more readily
            if (seen != 0 || heard != 0 || felt != 0 && statePrv == SEARCH)
            {
                stateCur = SEARCH;
                statePrv = IDLE;
            }
        }

        #endregion

        #region SEARCH

        private Vector3 CalcBestSensePlayer()
        {
            Vector3 best = tank.Position;
            if (felt == 0 && seen == 0 && heard == 0) ;

            else if (felt >= seen && felt >= heard)
            {
                best = lastPlayerFelt;
            }

            else if (seen >= heard)
            {
                best = lastPlayerSeen;
            }

            else //if (heard > seen)
            {
                best = lastPlayerHeard;
            }

            return best;
        }

        private Vector3 CalcTimeSensePlayer()
        {            
            Vector3 best = tank.Position;
            if (timePlayerFelt == null && timePlayerSeen == null && timePlayerFelt == null) ;

            else if (timePlayerFelt <= timePlayerSeen && timePlayerFelt <= timePlayerHeard)
            {
                best = lastPlayerFelt;
            }

            else if (timePlayerSeen <= timePlayerHeard)
            {
                best = lastPlayerSeen;
            }

            else //if (heard > seen)
            {
                best = lastPlayerHeard;
            }

            return best;
        }

        private void Search(Vector3 target)
        {
            //If I have a definite target, move towards it and look at it

            if (seen < 1)
            {
                AimToward(target);
                MoveToward(target);
            }

            //Else patrol while keeping an eye out
            if (seen < 0.25)
            {
                //Scan(target)
            }
        }

        private void Scan(Vector3 target)
        {
            if (AngleBetweenTurretAbs(target) < 0)
            {

            }
        }

        private void SEARCHStateCheck()
        {
            //If player is copped or cops a feel, enter combat
            if (seen >= 1 || felt >= 1)
            {
                stateCur = COMBAT;
                statePrv = SEARCH;
            }

            //Huh, guess it was just the wind...
            //Unless I was in combat, in which case I won't go back to being idle. EVER. I will remain on alert for eternity, because you picked the wrong Tank to fuck with. We're like elephants, we NEVER forget.
            else if (seen <= 0 && heard <= 0 && felt <= 0 && statePrv != COMBAT)
            {
                stateCur = IDLE;
                statePrv = SEARCH;
            }
        }

        #endregion

        #region COMBAT

        private void TacticalReposition(Vector3 target)
        {
            //Move to target with a view to shooting it. Range depends on accuracy,
            if (Vector3.Distance(tank.Position, target) < accuracy && statePrv != FLEE)
            {
                AimToward(target);
                ShootAt(target);
            }

            else if (statePrv != FLEE)
            {
                MoveToward(target);
                //Scan(target);
            }
            
            else AimToward(target);
        }

        private void ShootAt(Vector3 target)
        {
            AimToward(target);
            if (seen != 0)
            {
                Fire();
            }
        }

        private void COMBATStateCheck()
        {
            //If I lose you, I will find you. And I will kill you. Liam Neeson all up in this.
            if (seen < 0.5 && heard < 0.5 && felt < 0.5)
            {
                stateCur = SEARCH;
                statePrv = COMBAT;
            }

            //Oh dang I'm almost dead better book it.
            if (tank.Health <= tank.HealthMax * (fleeThresh / 100))
            {
                stateCur = FLEE;
                statePrv = COMBAT;
            }
        }

        #endregion

        #region FLEE

        private Vector3 CalcBestFriendly()
        {
            Vector3 best = tank.Position;
            if (timeFriendlySeen == null && timeFriendlyHeard == null && timeFriendlyFelt == null) ;

            else if (timeFriendlyFelt <= timeFriendlySeen && timeFriendlyFelt <= timeFriendlyHeard)
            {
                best = lastFriendlyFelt;
            }

            else if (timeFriendlySeen <= timeFriendlyHeard)
            {
                best = lastFriendlySeen;
            }

            else //if (heard > seen)
            {
                best = lastFriendlyHeard;
            }

            return best;
        }

        private void RunAway(Vector3 scaryThing)
        {
            //Move away from the scary thing
            MoveToward(-scaryThing);
            AimToward(scaryThing);
        }

        private void RunTowards(Vector3 target)
        {
            //Move towards the nice thing
            MoveToward(CalcBestFriendly());
        }

        private void FLEEStateCheck()
        {
            //I think I got away, I don't sense him anywhere, guess I better just hang out.
            if (seen == 0 || heard == 0 || felt == 0)
            {
                stateCur = IDLE;
                statePrv = FLEE;
            }

            //I found a friend, now we can take him on.
            if (Vector3.Distance(CalcBestFriendly(), tank.Position) < 100 /*check that friendly is actually there somehow*/)
            {
                //stateCur = COMBAT;
                //statePrv = FLEE;
            }
        }

        #endregion

        #region DANCE

        public void DanceOff()
        {
            statePrv = stateCur;
            stateCur = DANCE;
        }

        public void DanceCancel()
        {
            stateCur = statePrv;
            statePrv = IDLE;
        }

        #endregion

        #region Info

        public string StateCurString()
        {
            switch (stateCur)
            {
                case 0:
                    return "IDLE";
                case 1:
                    return "SEARCH";
                case 2:
                    return "COMBAT";
                case 3:
                    return "FLEE";
                default:
                    return "NULL";
            }
        }

        #endregion

        #region Update

        public void Update()
        {
            if (registered && tank.Active && tank.Alive)
            {
                TestMethod();
                switch (stateCur)
                {
                    case IDLE:
                        if (statePrv != FLEE)
                        {
                            LookAround();
                            TurnTowardSlow(new Vector3(2048, 2048, 0));
                        }
                        else
                        {
                            LookAllAround();
                        }
                        IDLEStateCheck();
                        break;
                    case SEARCH:
                        Search(CalcBestSensePlayer());
                        SEARCHStateCheck();
                        break;
                    case COMBAT:
                        TacticalReposition(CalcBestSensePlayer());
                        COMBATStateCheck();
                        break;
                    case FLEE:
                        RunAway(CalcBestSensePlayer());
                        FLEEStateCheck();
                        break;
                    case DANCE:
                        tank.RotateTurretLeft();
                        tank.RotateWheelsRight();
                        break;
                }
                //Senses
                ReceiveSenseData();
                Sight();
                Sound();
                Touch();
                SenseTimers();
                WatchEdges();
            }
        }

        private void ReceiveSenseData()
        {
            //Grab all the last sensed tanks
            lastTankSeen = tank.LastSeen;
            lastTankHeard = tank.LastHeard;
            lastTankFelt = tank.LastFelt;
        }

        private void SenseTimers()
        {
            //Increment Friendly Timer
            if (timeFriendlySeen != null)
                timeFriendlySeen++;
            if (timeFriendlyHeard != null)
                timeFriendlyHeard++;
            if (timeFriendlyFelt != null)
                timeFriendlyFelt++;

            //Increment Player Timer
            if (timePlayerSeen != null)
                timePlayerSeen++;
            if (timePlayerHeard != null)
                timePlayerHeard++;
            if (timePlayerFelt != null)
                timePlayerFelt++;

        }

        //Testing stuff, remove for release
        private void TestMethod()
        {
        }

        public void Draw(GUI g)
        {
            g.DrawBool(tank.Position.X + 40, tank.Position.Y - 80, "LR: ", leftRight, null);
        }

        #endregion

    }
}
