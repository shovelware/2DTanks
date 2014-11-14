using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TankGame
{
    class Camera
    {
#region MVars

        Matrix cameraMPTran, cameraMScale, cameraMRotat, cameraMTrans, cameraMFinal;
        public Matrix CameraMFinal { get { return cameraMFinal; } }

        float shakeStrength, shakeDisplace;
        int shakeTimer;

        //Scale
        float cameraFZoomL, cameraFRotat, cameraFZmSpd,
              cameraFZmMin, cameraFZmMax, cameraFZmTar;
        bool follow;
        public bool FollowState { get { return follow; } }

        Vector3 cameraVTrans, cameraVFollo, cameraVMoves, cameraVLeans, cameraVShake, cameraVEdges;

        Vector3 cameraVSpeed;

        Vector3 playerVPos;
        Vector3 cameraVScreen = new Vector3(640, 360, 0);

        public Vector3 CameraVScreen { get { return cameraVScreen; } }
        public Vector3 CameraCentre { get { return cameraVTrans; } }
        public float CameraZoom { get { return cameraFZoomL; } }

#endregion

#region Make, Break, Load

        public Camera(float? zoom, float? minZoom, float? maxZoom, float? zoomSpeed, float? rotation, Vector3? translation, float? speed, Vector3 halfscreen, Vector3 worldSize)
       {
            cameraFZoomL = zoom ?? -0.3f;
            cameraFZmTar = 0;
            cameraFZmMin = minZoom ?? -0.9f;
            cameraFZmMax = maxZoom ?? 2f;
            cameraFZmSpd = zoomSpeed ?? 0.2f;
            cameraFRotat = rotation ?? 0f;
            cameraVTrans = translation ?? Vector3.Zero;
            cameraVSpeed = new Vector3(speed ?? .1f, speed ?? .1f, speed ?? .1f);
            cameraVScreen = halfscreen;
            cameraVEdges = (worldSize) - new Vector3(128, 128, 0);

            ResetZoom();
        }

#endregion

#region Basics

        public void Move(Vector3 move)
        {
            if (follow)
            {
                cameraVLeans += move * 2;
            }

            else
            {
                cameraVMoves += move;
            }
        }

        public void Point(Vector3 target)
        {
            cameraVTrans = target;
        }

        public void Follow(Vector3 leader)
        {
            playerVPos = leader;
            cameraVFollo = (-playerVPos);
        }

        public void ToggleFollow()
        {
            follow = !follow;
        }

        public void Zoom(float direction)
        {
            if ((cameraFZmTar + (direction / (Math.Abs(direction) * 10))) >= cameraFZmMin - 0.1 && (cameraFZmTar + (direction / (Math.Abs(direction) * 10))) <= cameraFZmMax + 0.1)
            {
                cameraFZmTar += (direction / (Math.Abs(direction) * 10));
            }
        }

        public void ResetZoom()
        {
            cameraFZmTar = -0.3f;
        }

        public void UpdateCentre(Vector3 newHalfScreen)
        {
            cameraVScreen = newHalfScreen;
        }
        
#endregion

#region Juice

        public void Shake(int shakeTime, float pitchAmount,  float displaceAmount)
        {
            shakeStrength = pitchAmount;
            shakeTimer = shakeTime;
            shakeDisplace = displaceAmount;
        }

        private void DoShake()
        {
            switch (shakeTimer % 6)
            {
                case 0:
                    shakeTimer--;
                    cameraFRotat = 0;
                    cameraVShake = Vector3.Zero;
                    break;
                case 1:
                    shakeTimer--;
                    cameraFRotat = -shakeStrength;
                    cameraVShake.Y = -shakeDisplace;
                    break;
                case 2:
                    shakeTimer--;
                    cameraFRotat = shakeStrength;
                    cameraVShake.X = shakeDisplace;
                    break;
                case 3:
                    shakeTimer--;
                    cameraFRotat = 0;
                    cameraVShake.X = -shakeDisplace;
                    break;
                case 4:
                    shakeTimer--;
                    cameraFRotat = -shakeStrength;
                    cameraVShake.Y = shakeDisplace;
                    break;
                case 5:
                    shakeTimer--;
                    cameraFRotat = shakeStrength;
                    break;
            }
        }

        private void CheckEdges()
        {
            if (cameraVFollo.X < -cameraVEdges.X + cameraVScreen.X)
            {
                cameraVFollo.X = -cameraVEdges.X + cameraVScreen.X;
            }

            else if (cameraVFollo.X > -cameraVScreen.X)
            {
                cameraVFollo.X = -cameraVScreen.X;
            }

            if (cameraVFollo.Y < -cameraVEdges.Y + cameraVScreen.Y)
            {
                cameraVFollo.Y = -cameraVEdges.Y + cameraVScreen.Y;
            }

            else if (cameraVFollo.Y > -cameraVScreen.Y)
            {
                cameraVFollo.Y = -cameraVScreen.Y;
            }
        }

#endregion

#region U&D

        public void Update(GameTime gameTime, int width, int height)
        {
          
            {
                if (shakeTimer >= 0)
                {
                    DoShake();
                }

                if (cameraFZoomL > -0.60 && cameraFZoomL < -0.40)
                {
                    CheckEdges();
                }

                if (follow)
                {
                    cameraVTrans += (cameraVFollo - cameraVTrans) * cameraVSpeed;
                    cameraVTrans += cameraVLeans + cameraVShake;
                }

                if (!follow)
                {
                    cameraVTrans += cameraVMoves;
                }

                cameraFZoomL += (cameraFZmTar - cameraFZoomL) * cameraFZmSpd;

                //Reset values for next update
                //cameraFZmTar = 0;
                cameraVLeans = Vector3.Zero;
                cameraVMoves = Vector3.Zero;
                
                //Edge Checking


                //Matxrix generation
                cameraMPTran = Matrix.CreateTranslation(cameraVTrans);
                cameraMScale = Matrix.CreateScale(cameraFZoomL + 1, cameraFZoomL + 1, 0);
                cameraMRotat = Matrix.CreateRotationZ(cameraFRotat);
                cameraMTrans = Matrix.CreateTranslation(cameraVScreen);

                cameraMFinal = cameraMPTran * cameraMScale * cameraMRotat * cameraMTrans;
            }
            //DEBUG

            //ENDEBUG
        }

#endregion
    }
}
