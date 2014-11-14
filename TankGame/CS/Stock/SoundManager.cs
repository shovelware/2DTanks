using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TankGame
{
    class SoundManager
    {

        #region MVars
        ContentManager c;
        Song level0, level1, level2, level3, level4,
             currentSong;

        SoundEffect fireBullet, fireRocket, fireMine, explosion, bulletHit, tankIdle, tankMove, tankTurn;

        List<SoundEffectInstance> nowPlaying = new List<SoundEffectInstance>();
        List<SoundEffectInstance> prvPlaying = new List<SoundEffectInstance>();

        #endregion

        #region Make & Load

        public SoundManager(ContentManager content)
        {
            c = content;

            LoadContent();

            currentSong = level0;
        }

        private void LoadContent()
        {
            level0 = c.Load<Song>(".\\Assets\\Sound\\Music\\00 - Crowds I");
            level1 = c.Load<Song>(".\\Assets\\Sound\\Music\\01 - Crowds II");
            level2 = c.Load<Song>(".\\Assets\\Sound\\Music\\02 - Milestone I");
            level3 = c.Load<Song>(".\\Assets\\Sound\\Music\\03 - Milestone II");
            level4 = c.Load<Song>(".\\Assets\\Sound\\Music\\04 - Firebird");

            fireBullet = c.Load<SoundEffect>(".\\Assets\\Sound\\SFX\\Bullet");
            fireRocket = c.Load<SoundEffect>(".\\Assets\\Sound\\SFX\\Rocket");
            fireMine = c.Load<SoundEffect>(".\\Assets\\Sound\\SFX\\Mine");

            explosion = c.Load<SoundEffect>(".\\Assets\\Sound\\SFX\\Explosion");
            bulletHit = c.Load<SoundEffect>(".\\Assets\\Sound\\SFX\\BulletHit");

            tankIdle = c.Load<SoundEffect>(".\\Assets\\Sound\\SFX\\Idle");
            tankMove = c.Load<SoundEffect>(".\\Assets\\Sound\\SFX\\Move");
            tankTurn = c.Load<SoundEffect>(".\\Assets\\Sound\\SFX\\Turret");
        }

        #endregion

        #region Management

        public void ChangeBGM(int levelNo)
        {
            switch (levelNo)
            {
                case 0:
                    currentSong = level0;
                    break;
                case 1:
                    currentSong = level1;
                    break;
                case 2:
                    currentSong = level2;
                    break;
                case 3:
                    currentSong = level3;
                    break;
                case 4:
                    currentSong = level4;
                    break;
            }
        }

        public void PlayBGM()
        {
            MediaPlayer.Volume = 0.25f;
            MediaPlayer.Play(currentSong);
            MediaPlayer.IsRepeating = true;
        }

        public void StopBGM()
        {
            MediaPlayer.Stop();
        }

        public void FireWeapon(char weaponid)
        {
            switch(weaponid)
            {
                case 'B':
                    nowPlaying.Add(fireBullet.CreateInstance());
                    break;
                case 'R':
                    nowPlaying.Add(fireRocket.CreateInstance());
                    break;
                case 'M':
                    nowPlaying.Add(fireMine.CreateInstance());
                    break;
            }
        }
        #endregion

        #region U&D

        public void Update()
        {
            foreach (SoundEffectInstance s in nowPlaying)
            {
                if (s.State != SoundState.Playing && !prvPlaying.Contains(s))//If it's not playing and was not in the previous instance
                {
                    s.Volume = 0.25f;
                    s.Play();
                }

                else s.Dispose();
            }

            for (int i = 0; i < nowPlaying.Count; i++)//Remove disposed SFX from the list
            {
                if (nowPlaying[i].IsDisposed)
                {
                    nowPlaying.RemoveAt(i);
                }
            }

            prvPlaying = nowPlaying;
        }

        #endregion
    }
}
