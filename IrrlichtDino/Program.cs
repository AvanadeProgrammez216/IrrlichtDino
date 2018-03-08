using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IrrlichtLime;
using IrrlichtLime.Core;
using IrrlichtLime.Video;
using IrrlichtLime.Scene;
using IrrlichtLime.GUI;

namespace IrrlichtDino
{
    class Program
    {
        // Settings
        public static float VerticalGravity = -98.3f;
        public static float GroundAltitude = -30.0f;
        public static int ResolutionX = 800;
        public static int ResolutionY = 600;

        // Internal
        private static bool _isGrounded = false;
        private static float _playerVerticalSpeed = 0.0f; // used to calculate vertical speed for gravity and jump
        static void Main(string[] args)
        {
            IrrlichtDevice device = IrrlichtDevice.CreateDevice(DriverType.OpenGL, new Dimension2Di(ResolutionX, ResolutionY), 24, false, false, false);

            device.OnEvent += Device_OnEvent;
            device.SetWindowCaption("Hello World! - Irrlicht Engine Demo");

            VideoDriver driver = device.VideoDriver;
            SceneManager smgr = device.SceneManager;

            GUIEnvironment gui = device.GUIEnvironment;

            gui.AddStaticText("Hello World! This is the Irrlicht Software renderer!",
                new Recti(10, 10, 260, 22), true);

            AnimatedMesh mesh = smgr.GetMesh("../../media/sydney.md2");
            AnimatedMeshSceneNode sydneyNode = smgr.AddAnimatedMeshSceneNode(mesh);

            var cubeSceneNode = smgr.AddCubeSceneNode(2.0f);

            cubeSceneNode.Scale = new Vector3Df(6.0f, 2.0f, 100.0f);
            cubeSceneNode.Position = new Vector3Df(cubeSceneNode.Position.X,  GroundAltitude + 0.5f, cubeSceneNode.Position.Z);

            if (sydneyNode != null)
            {
                sydneyNode.SetMaterialFlag(MaterialFlag.Lighting, true);
                sydneyNode.SetMD2Animation(AnimationTypeMD2.Stand);
                sydneyNode.SetMaterialTexture(0, driver.GetTexture("../../media/sydney.bmp"));
            }

            var lightSceneNode = smgr.AddLightSceneNode();
            var light = lightSceneNode.LightData;
            light.DiffuseColor = new Colorf(0.8f, 1.0f, 1.0f);
            light.Type = LightType.Directional;
            light.Position = new Vector3Df(0.0f, 5.0f, 5.0f);


            var cam = smgr.AddCameraSceneNode(null, new Vector3Df(20, 30, -40), new Vector3Df(20, 5, 0));
            device.Timer.Start();

            uint then = device.Timer.RealTime;
            while (device.Run())
            {
                // As the game is simple we will handle our simple physics ourselves
                uint now = device.Timer.RealTime;
                uint elapsed = now - then;
                float elapsedTimeSec = (float)elapsed / 1000.0f;
                then = now;

                // we target 58.8 FPS
                if (elapsed < 17)
                    device.Sleep(17 - (int)elapsed);
                else
                {
                    // uh-oh, it took more than .125 sec to do render loop!
                    // what do we do now?
                }

                _playerVerticalSpeed += elapsedTimeSec * -(VerticalGravity * 10.0f);

                var calculatedNewPos = sydneyNode.Position - new Vector3Df(0.0f, _playerVerticalSpeed * elapsedTimeSec, 0.0f);

                float offsetSydney = sydneyNode.BoundingBox.Extent.Y / 2.0f;
                _isGrounded = calculatedNewPos.Y <= (GroundAltitude + offsetSydney);
                if (_isGrounded)
                {
                    _playerVerticalSpeed = 0.0f;
                    calculatedNewPos.Y = GroundAltitude + offsetSydney;
                }
                else
                {
                    calculatedNewPos.Y = calculatedNewPos.Y;
                }

                sydneyNode.Position = calculatedNewPos;

                driver.BeginScene(true, true, new Color(100, 101, 140));
                smgr.DrawAll();
                gui.DrawAll();

                driver.EndScene();
            }

            device.Drop();
        }

        private static bool Device_OnEvent(Event evnt)
        {
            if (evnt.Key.Key == KeyCode.Space)
            {
                if (_isGrounded)
                {
                    _playerVerticalSpeed -= 350.0f; // we apply a vertical force to the player for him to jump
                    Console.WriteLine("JUMP");
                }
            }
            return false;
        }
    }
}
