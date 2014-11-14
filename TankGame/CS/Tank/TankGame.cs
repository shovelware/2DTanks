using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//x += (target - x) * .1;

/*QUESTIONS
 * Targetting mouse
 * 
 * 
 * 
 * 
 * 
*/
namespace TankGame
{
    /// <summary>
    /// This is the main type for your tankGame
    /// </summary>
    public class TankGame : Microsoft.Xna.Framework.Game
    {
        #region MVars
        //DEBUG
        //ENDEBUG

        //Display Vars
        const int screenWidth = 1280;
        const int screenHeight = 720;
        bool fullscreen;

        int worldWidth;
        int worldHight;
        public int WorldWidth { get { return worldWidth; } }
        public int WorldHight { get { return worldHight; } }

        //World bounds: -64, -64 to 4160, 4160

        //Objects
        InputManager input;
        GUI gui;
        Camera cam;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Assets
        TankAssets tankAssets;
        TileAssets tileAssets;
        ProjAssets projAssets;

        //Managers
        TankManager tankM;
        TileManager tileM;
        ProjManager projM;
        SoundManager sndM;

        //Spawning
        List<Vector3> spawns;
        Random rng;

        //Triggers
        bool debug;
        int currentLevel;
        int maxLevel;
        bool pause;

        bool music;

        bool menu;
        bool gameOver;


        #endregion

        #region Construct & Init

        public TankGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferMultiSampling = true;

            this.IsMouseVisible = true;
            rng = new Random();
            currentLevel = 0;
            maxLevel = 4;
            music = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the tankGame to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per tankGame and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.LoadContent(GraphicsDevice);
            gui = new GUI(spriteBatch, Content);
            input = new InputManager();

            //snd_bgm = Content.Load<SoundEffect>("BGM");

            tankAssets = new TankAssets(Content);
            tileAssets = new TileAssets(Content);
            projAssets = new ProjAssets(Content);
            sndM = new SoundManager(Content);
            Reset();
            menu = true;
            //DEBUG
            //ENDEBUG

        }

        /// <summary>
        /// UnloadContent will be called once per tankGame and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        #endregion

        #region Input

        private void CheckInput(GameTime gameTime)
        {

            //Exit
            if (input.CheckKey(Keys.Escape) > 0)
            {
                this.Exit();
            }

            //Refresh Game
            if (input.CheckKey(Keys.F5) == 1)
            {
                Reset();
            }

            //(Refresh Game)
            if (input.CheckKey(Keys.F6) > 0)
            {
                Reset();
            }

            if (input.MousePosPrvAbs != input.MousePosCurAbs)
            {
                //////Method 1
                //tankM.TurretTarget((float)Math.Atan2((double)mouseCur.Y - vCentre.Y, (double)mouseCur.X - vCentre.X));

                ////Method 2
                //Vector3 direction = vCentre - new Vector3(mouseCur.X, mouseCur.Y, 0);
                //direction.Normalize();
                //tankM.TurretTarget((float)Math.Atan2((double)direction.Y, (double)direction.X) + MathHelper.PiOver2);
            }

            //Movement
            if (input.CheckKey(Keys.Up) > 0 || input.CheckKey(Keys.W) > 0)
            {
                tankM.MoveFore();
            }
            if (input.CheckKey(Keys.Down) > 0 || input.CheckKey(Keys.S) > 0)
            {
                tankM.MoveBack();
            }

            //Wheel Rotation
            if (input.CheckKey(Keys.Right) > 0 || input.CheckKey(Keys.D) > 0)
            {
                tankM.RotateWheelsRight();
            }
            if (input.CheckKey(Keys.Left) > 0 || input.CheckKey(Keys.A) > 0)
            {
                tankM.RotateWheelsLeft();
            }

            //Turret Rotation
            if (input.CheckKey(Keys.E) > 0)
            {
                tankM.RotateTurretRight();
            }
            if (input.CheckKey(Keys.Q) > 0)
            {
                tankM.RotateTurretLeft();
            }

            //Sprint/Crawlspeed
            if (input.CheckKey(Keys.LeftShift) > 0)
            {
                tankM.SprintOn();
            }
            if (input.CheckKey(Keys.LeftShift) == -1)
            {
                tankM.SprintOff();
            }
            if (input.CheckKey(Keys.LeftControl) > 0)
            {
                tankM.CrawlOn();
            }
            if (input.CheckKey(Keys.LeftControl) == -1)
            {
                tankM.CrawlOff();
            }

            //Change Current Tank
            if (input.CheckKey(Keys.D0) == 1)
            {
                tankM.CrementCurrentTank(1);
            }
            if (input.CheckKey(Keys.D9) == 1)
            {
                tankM.CrementCurrentTank(-1);
            }

            //Random Tank Changes
            if (input.CheckKey(Keys.J) == 1)
            {
                //if (!multiTank)
                {
                    tankM.SetAccent(null, null, null, null);
                    //Only work for individual tanks
                }
            }
            if (input.CheckKey(Keys.K) == 1)
            {
                //if (!multiTank)
                {
                    Random rng = new Random();
                    tankM.SetBaseColor(new Color(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256)));
                    //Only work for individual tanks
                }
            }
            if (input.CheckKey(Keys.L) == 1)
            {
                //if (!multiTank)
                {
                    Random rng = new Random();
                    tankM.SetAccentColor(new Color(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256)));
                    //Only work for individual tanks
                }
            }

            //Relative Movement
            if (input.CheckKey(Keys.C) > 0)
            {
                tankM.MoveRelUp();
            }

            if (input.CheckKey(Keys.V) > 0)
            {
                tankM.AimRelRight();
            }

            //Toggle Multi Tank Control
            if (input.CheckKey(Keys.R) == 1)
            {
                tankM.ToggleMultiTank();
            }

            //Pause
            if (input.CheckKey(Keys.P) == 1)
            {
                pause = !pause;
            }

            if (input.CheckKey(Keys.I) > 1)
            {
                projM.ForceUpdate(gameTime, cam.CameraMFinal, tankM);
            }

            if (input.CheckKey(Keys.O) > 1)
            {
                tankM.ForceUpdate(gameTime, cam.CameraMFinal);
            }

            //Tilesets
            if (input.CheckKey(Keys.T) == 1)
            {
                Random rng = new Random();
                tileM.AddSet(new Color(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256)), new Color(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256)), 1, true, true, true);
            }
            if (input.CheckKey(Keys.Y) == 1)
            {
                tileM.CrementCurrentTileset(-1);
            }
            if (input.CheckKey(Keys.U) == 1)
            {
                tileM.CrementCurrentTileset(1);
            }

            //Firing
            if (input.CheckKey(Keys.Space) > 0)
            {
                tankM.FireRocket(projM, gui);
            }

            if (input.CheckKey(Keys.B) > 0)
            {
                tankM.FireBullet(projM, gui);
            }

            if (input.CheckKey(Keys.M) == 1)
            {
                tankM.FireMine(projM, gui);
            }

            if (input.CheckKey(Keys.X) == 1)
            {
                tankM.FireX(projM, gui);
            }

            //Music
            if (input.CheckKey(Keys.OemPeriod) == 1)
            {
                if (music)
                {
                    sndM.StopBGM();
                    music = false;
                }

                else if (!music)
                {
                    sndM.PlayBGM();
                    music = true;
                }
            }

            //Add Tanks
            if (input.CheckKey(Keys.Tab) == 1)//Player
            {
                tankM.AddPlayer(gui, new Vector3(worldWidth / 2, worldHight / 2, 0), null, Color.Coral, Color.CornflowerBlue, true);
            }

            if (input.CheckKey(Keys.LeftAlt) == 1)
            {
                tankM.AddAI(gui, new Vector3(worldWidth / 4, worldHight / 4, 0), null, Color.Gray, Color.Red, 50);
            }

            //Load Levels
            if (input.CheckKey(Keys.D1) == 1)
            {
                currentLevel = 1;
                LoadLevel(currentLevel);
            }
            if (input.CheckKey(Keys.D2) == 1)
            {
                currentLevel = 2;
                LoadLevel(currentLevel);
            }
            if (input.CheckKey(Keys.D3) == 1)
            {
                currentLevel = 3;
                LoadLevel(currentLevel);
            }
            if (input.CheckKey(Keys.D4) == 1)
            {
                currentLevel = 4;
                LoadLevel(currentLevel);
            }

            //Fullscreen (Experimental)
            if (input.CheckKey(Keys.Enter) == 1)
            {
                //if (!fullscreen)
                //{
                //    graphics.PreferredBackBufferWidth = 1920;
                //    graphics.PreferredBackBufferHeight = 1080;
                //    fullscreen = true;
                //}

                //else if (fullscreen)
                //{
                //    graphics.PreferredBackBufferWidth = screenWidth;
                //    graphics.PreferredBackBufferHeight = screenHeight;
                //    fullscreen = false;
                //}

                //graphics.ApplyChanges();
                //graphics.ToggleFullScreen();
                //cam.UpdateCentre(new Vector3(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2, 0));
            }

            if (input.CheckKey(Keys.OemSemicolon) == 1)
            {
                graphics.PreferMultiSampling = !graphics.PreferMultiSampling;
                graphics.ApplyChanges();
            }

            ////////DEBUG

            if (input.CheckKey(Keys.Back) == 1)
            {
                tankM.KillCurrentTank();
            }

            //Top row
            if (input.CheckKey(Keys.NumPad7) > 0)
            {
                cam.Zoom(-1);
            }
            if (input.CheckKey(Keys.NumPad8) > 0)
            {
                cam.Move(new Vector3(0, 10, 0));
            }
            if (input.CheckKey(Keys.NumPad9) > 0)
            {
                cam.Zoom(1f);
            }

            //Mid row
            if (input.CheckKey(Keys.NumPad4) > 0)
            {
                cam.Move(new Vector3(10, 0, 0));
            }
            if (input.CheckKey(Keys.NumPad5) == 1)
            {
                cam.ToggleFollow();
            }
            if (input.CheckKey(Keys.NumPad6) > 0)
            {
                cam.Move(new Vector3(-10, 0, 0));
            }

            //Bottom row
            if (input.CheckKey(Keys.NumPad1) == 1)
            {
                cam.Shake(20, 0.01f, 5);
            }
            if (input.CheckKey(Keys.NumPad2) > 0)
            {
                cam.Move(new Vector3(0, -10, 0));
            }
            if (input.CheckKey(Keys.NumPad3) == 1)
            {
            }

            //Misc
            if (input.CheckKey(Keys.NumPad0) == 1)
            {
                tankM.ToggleDebug();
                projM.ToggleDebug();
            }
            if (input.CheckKey(Keys.Add) == 1)
            {
                tankM.CrementScale(.5f);
            }
            if (input.CheckKey(Keys.Subtract) == 1)
            {
                tankM.CrementScale(-.5f);
            }
            if (input.CheckKey(Keys.Multiply) > 0)
            {
                tankM.RotateWheelsTarget(new Vector3(2048, 2048, 0));
            }
            if (input.CheckKey(Keys.Divide) == 1)
            {
                tankM.AssumeDirectControl();
            }
            if (input.CheckKey(Keys.Decimal) > 0)
            {
                cam.ResetZoom();
            }

            if (input.CheckMouseLeft() > 0)
            {
                tankM.RotateTurretTarget(input.MousePosCur(cam));
            }

            if (input.CheckMouseRight() > 0)
            {
                tankM.RotateWheelsTarget(input.MousePosCur(cam));
            }
        }

        #endregion

        #region GAEM

        private Vector3 RandomSpawnPoint()
        {
            Vector3 temp = spawns[rng.Next(0, spawns.Count())];
            spawns.Remove(temp);
            return temp;
        }

        private void GenerateSpawnPoints(float width, float height)
        {
            spawns = new List<Vector3>();

            for (float y = .125f; y < 1; y += .125f)
            {
                for (float x = .125f; x < 1; x += .125f)
                {
                    if ((y == .375 || y == .625) && (x > .25 && x < .75)) ;
                    else if (y == .5 && (x > .25 && x < .75)) ;

                    else spawns.Add(new Vector3((width * y), (height * x), 0));
                }
            }
        }

        public void Reset()
        {
            projM = new ProjManager(projAssets);
            tankM = new TankManager(tankAssets, projM);
            tileM = new TileManager(tileAssets);

            worldHight = tileM.TileHight * tileM.TilesVert;
            worldWidth = tileM.TileWidth * tileM.TilesHori;

            GenerateSpawnPoints(worldWidth, worldHight);

            cam = new Camera(null, null, null, null, null, null, null, new Vector3(screenWidth / 2, screenHeight / 2, 0), new Vector3(worldWidth, worldHight, 0));
            cam.ToggleFollow();

            tankM.AddPlayer(gui, new Vector3(worldWidth / 2, worldHight / 2, 0), null, Color.Coral, Color.CornflowerBlue, true);

            LoadLevel(currentLevel);
        }

        public void LoadLevel(int number)
        {
            //Common controls
            tankM.MaintainCurrent();
            tankM.KillAllButCurrent();
            projM.KillAll();
            sndM.ChangeBGM(currentLevel);

            if (music)
            {
                sndM.PlayBGM();
            }


            switch (number)
            {
                case 0:
                    tileM.AddSet(Color.CornflowerBlue, Color.Yellow, 1, true, true, true);
                    gui.DrawMsg(new Vector2(screenWidth / 2, screenHeight / 2), "Basic", Color.Red);

                    tankM.MoveCurrent(new Vector3(worldWidth * .5f, worldHight * .75f, 0));
                    tankM.AddAI(gui, new Vector3(worldWidth * .5f, worldHight * .25f, 0f), null, Color.DarkSlateGray, Color.Orange, 1);

                    break;
                case 1:
                    tileM.AddSet(Color.Plum, Color.PeachPuff, 0, true, true, true);

                    tankM.MoveCurrent(new Vector3(worldWidth * .5f, worldHight * .5f, 0));

                    GenerateSpawnPoints(worldWidth, worldHight);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.DarkSlateGray, Color.Green, 10);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.DarkSlateGray, Color.Green, 10);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.DarkSlateGray, Color.Green, 10);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.DarkSlateGray, Color.Green, 10);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.DarkSlateGray, Color.Green, 10);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.DarkSlateGray, Color.Green, 10);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.DarkSlateGray, Color.Green, 10);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.DarkSlateGray, Color.Green, 10);
                    break;
                case 2:
                    tileM.AddSet(Color.DarkBlue, Color.BurlyWood, 3, true, true, true);

                    tankM.MoveCurrent(new Vector3(worldWidth * .5f, worldHight * .5f, 0));

                    GenerateSpawnPoints(worldWidth, worldHight);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.CadetBlue, Color.Yellow, 25);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.CadetBlue, Color.Yellow, 25);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.CadetBlue, Color.Yellow, 25);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.CadetBlue, Color.Yellow, 25);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.CadetBlue, Color.Yellow, 25);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.CadetBlue, Color.Yellow, 25);
                    break;
                case 3:
                    tileM.AddSet(Color.DarkGray, Color.PaleVioletRed, 2, true, true, true);

                    tankM.MoveCurrent(new Vector3(worldWidth * .5f, worldHight * .5f, 0));

                    GenerateSpawnPoints(worldWidth, worldHight);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.Gray, Color.Red, 50);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.Gray, Color.Red, 50);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.Gray, Color.Red, 50);
                    tankM.AddAI(gui, RandomSpawnPoint(), null, Color.Gray, Color.Red, 50);

                    break;
                case 4:
                    tileM.AddSet(Color.Black, Color.Cyan, 1, true, false, false);

                    tankM.MoveCurrent(new Vector3(worldWidth * .5f, worldHight * .75f, 0));
                    tankM.AddBoss(gui, new Vector3(worldWidth * .25f, worldHight * .25f, 0f), 2, Color.DarkSlateGray, Color.Orange, 100);
                    tankM.AddBoss(gui, new Vector3(worldWidth * .75f, worldHight * .25f, 0f), 2, Color.DarkSlateGray, Color.Orange, 100);
                    break;
            }
        }

        #endregion

        #region U & D
        /// <summary>
        /// Allows the tankGame to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            input.Update(gameTime);
            CheckInput(gameTime);

            cam.Update(gameTime, worldWidth, worldHight);

            if (tankM.GetTankList().Count == 0)
            {
                cam.Follow(new Vector3(worldWidth / 2, worldHight / 2, 0));
            }

            else
            {
                cam.Follow(tankM.CurrentTank().Position);
            }

            tileM.Update(gameTime, cam.CameraMFinal, pause);
            tankM.Update(gameTime, cam.CameraMFinal, pause);
            projM.Update(gameTime, cam.CameraMFinal, pause, tankM);

            switch (tankM.CheckLevel())
            {
                case -1:
                    Reset();
                    tankM.LoadScore();
                    break;
                case 0:
                    break;
                case 1:
                    if (currentLevel < maxLevel)
                    {
                        currentLevel++;
                        LoadLevel(currentLevel);
                        tankM.SaveScore();
                    }
                    if (currentLevel == maxLevel)
                    {
                       // gui.DrawString(0, 0, "dicks", "cocks", null);
                    }

                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the tankGame should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.OrangeRed);

            tileM.Draw(spriteBatch);
            tankM.Draw(spriteBatch);
            projM.Draw(spriteBatch);
            //this.Window.Title = "" +Math.Atan2((double)mouseCur.Y - vCentre.Y, (double)mouseCur.X - vCentre.X);
            this.Window.Title = "Multi Tank Control: " + tankM.MultiTank + ", Level: " + currentLevel + ", Follow Cam: " + cam.FollowState + ", MultiSampling: " + graphics.PreferMultiSampling;

            gui.DrawMinimap(tileM, tankM, projM, cam.CameraMFinal);

            spriteBatch.Begin();
            //gui.DrawVector3(0, 0, "CameraPos: ", cam.CameraCentre, Color.Purple);
            //gui.DrawFloat(0, 20, "CameraZoom: ", cam.CameraZoom, Color.Purple);

            //gui.DrawVector3(input.MousePosCurAbs.X, input.MousePosCurAbs.Y, "Mouse", input.MousePosCur(cam), null);
            //gui.DrawVector3(input.MousePosCurAbs.X, input.MousePosCurAbs.Y - 20, "Mouse Abs", input.MousePosCurAbs, null);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
