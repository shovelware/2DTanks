using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TankGame
{
    class InputManager
    {
#region MVArs

        KeyboardState keysCur, keysPrv;
        MouseState mouseCur, mousePrv;
        Vector3 mousePosCur, mousePosPrv;

        public Vector3 MousePosPrvAbs { get { return mousePosPrv; } }
        public Vector3 MousePosCurAbs { get { return mousePosCur; } }

#endregion

#region Make

        public InputManager()
        {

        }

#endregion

#region Update & Work

        public void Update(GameTime gametime)
        {
            keysPrv = keysCur;
            keysCur = Keyboard.GetState();

            mousePrv = mouseCur;
            mouseCur = Mouse.GetState();

            mousePosPrv = new Vector3(mousePrv.X, mousePrv.Y, 0);
            mousePosCur = new Vector3(mouseCur.X, mouseCur.Y, 0);

        }

        /// <summary>
        /// Checks a key, returns an short based on status.
        /// Protip: Use CheckKey(Key) > 0 to check for either press or hold.
        /// </summary>
        /// <param name="inKey">Key to be checked</param>
        /// <returns>
        /// -1: Relased
        /// 0: Neutral
        /// 1: Pressed
        /// 2: Held
        /// </returns>
        public short CheckKey(Keys inKey)
        {
            //Press
            if (keysCur.IsKeyDown(inKey) && keysPrv.IsKeyUp(inKey))
            {
                return 1;
            }

            //Hold
            else if (keysCur.IsKeyDown(inKey) && keysPrv.IsKeyDown(inKey))
            {
                return 2;
            }

            //Release
            else if (keysCur.IsKeyUp(inKey) && keysPrv.IsKeyDown(inKey))
            {
                return -1;
            }

            //Neutral
            else if (keysCur.IsKeyUp(inKey) && keysPrv.IsKeyUp(inKey))
            {
                return 0;
            }

            //Nothing (Error)
            else return 0;
        }

        public short CheckMouseLeft()
        {
            //Press
            if (mouseCur.LeftButton == ButtonState.Pressed && mousePrv.LeftButton == ButtonState.Released)
            {
                return 1;
            }

            //Hold
            else if (mouseCur.LeftButton == ButtonState.Pressed && mousePrv.LeftButton == ButtonState.Pressed)
            {
                return 2;
            }

            //Release
            else if (mouseCur.LeftButton == ButtonState.Released && mousePrv.LeftButton == ButtonState.Pressed)
            {
                return -1;
            }

            //Neutral
            else if (mouseCur.LeftButton == ButtonState.Released && mousePrv.LeftButton == ButtonState.Released)
            {
                return 0;
            }

            //Nothing (Error)
            else return 0;
        }

        public short CheckMouseRight()
        {
            //Press
            if (mouseCur.RightButton == ButtonState.Pressed && mousePrv.RightButton == ButtonState.Released)
            {
                return 1;
            }

            //Hold
            else if (mouseCur.RightButton == ButtonState.Pressed && mousePrv.RightButton == ButtonState.Pressed)
            {
                return 2;
            }

            //Release
            else if (mouseCur.RightButton == ButtonState.Released && mousePrv.RightButton == ButtonState.Pressed)
            {
                return -1;
            }

            //Neutral
            else if (mouseCur.RightButton == ButtonState.Released && mousePrv.RightButton == ButtonState.Released)
            {
                return 0;
            }

            //Nothing (Error)
            else return 0;
        }

        public short CheckMouseMiddle()
        {
            //Press
            if (mouseCur.MiddleButton == ButtonState.Pressed && mousePrv.MiddleButton == ButtonState.Released)
            {
                return 1;
            }

            //Hold
            else if (mouseCur.MiddleButton == ButtonState.Pressed && mousePrv.MiddleButton == ButtonState.Pressed)
            {
                return 2;
            }

            //Release
            else if (mouseCur.MiddleButton == ButtonState.Released && mousePrv.MiddleButton == ButtonState.Pressed)
            {
                return -1;
            }

            //Neutral
            else if (mouseCur.MiddleButton == ButtonState.Released && mousePrv.MiddleButton == ButtonState.Released)
            {
                return 0;
            }

            //Nothing (Error)
            else return 0;
        }

        private int CheckScroll()
        {
            //UP
            if (mouseCur.ScrollWheelValue - mousePrv.ScrollWheelValue > 0)
            {
                return 1;
            }

            //DOWN
            else if (mouseCur.ScrollWheelValue - mousePrv.ScrollWheelValue < 0)
            {
                return -1;
            }

            else return 0;
        }

        public Vector3 MousePosCur(Camera cam)
        {
            Vector3 mousePosRel = -((cam.CameraCentre - cam.CameraVScreen) + mousePosCur);

            return mousePosRel;
            //return Vector3.Transform(mousePosCur, cam.CameraMFinal);
        }

#endregion
    }
}
