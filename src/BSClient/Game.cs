#if !_WIN32
    #define MONO
#endif

#region --- Using directives ---

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK.Input;
using System.Drawing;
using System.Linq;
using Ninject;
using Ninject.Modules;

using JollyBit.BS.Client.Rendering;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core;
using JollyBit.BS.Client.Networking;
#endregion

namespace JollyBit.BS.Client
{
    [ConfigSection]
    public class ClientConfig {
        public int DebugSleepTime = 0;
        public bool EnableTrident = false;
        //public double MaxFPS = 60.0;
        //public bool VSync = true;
    }
    
    public class BSClient : GameWindow, ITimeService
    {
        private IList<IRenderable> _renderList = new List<IRenderable>();
        private Camera _camera;
        
        public IList<IRenderable> RenderList {
            get { return _renderList; }
        }
        public Camera Camera {
            get { return _camera; }
        }
        
        public BSClient() : base(800, 600) { }

        //GLTextureObject tex;
        SkyBox skyBox;
        private ClientConfig _config;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Console.WriteLine(System.Diagnostics.Stopwatch.Frequency);
            
            //Setup Ninject
            Constants.Kernel = new StandardKernel();
            Constants.Kernel.Load(new BSClientNinjectModule());
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            Constants.Kernel.Bind<ITimeService>().ToConstant(this);
            Constants.Kernel.Get<IStartupService>().ActivateStartupTypes();
            // Handle mouse and keyboard events
            new Input(this);

            // Create camera
            _camera = new Camera();
            
            // Get client config
            _config = Constants.Kernel.Get<ClientConfig>();
            //this.TargetRenderFrequency = _config.MaxFPS;
            //Console.WriteLine(this.TargetRenderFrequency);
            
            //this.VSync = _config.VSync ? VSyncMode.On : VSyncMode.Off;
            
            // Set OpenGL options
            Constants.Kernel.Get<GLState>();

            // Hook camera to WindowResize operation
            this.Resize += new EventHandler<EventArgs>( (Object sender, EventArgs eargs) => { _camera.RecalculateProjection(this.Width,this.Height); } );
            
            // Move the camera to (0,0,5)
            //_camera.Position = new Vector3(0, 0, 5);

            //Add skybox
            ContentManager contentManager = Constants.Kernel.Get<ContentManager>();
            skyBox = new SkyBox(
                contentManager.LoadBitmap(new FileReference("skybox/neg_x.png")),
                contentManager.LoadBitmap(new FileReference("skybox/pos_x.png")),
                contentManager.LoadBitmap(new FileReference("skybox/neg_y.png")),
                contentManager.LoadBitmap(new FileReference("skybox/pos_y.png")),
                contentManager.LoadBitmap(new FileReference("skybox/neg_z.png")),
                contentManager.LoadBitmap(new FileReference("skybox/pos_z.png")));
            //_renderList.Add(skyBox);
            
            // Create World Renderer
            MapRenderer mapRenderer = Constants.Kernel.Get<MapRenderer>();

            mapRenderer.camera = _camera;

            _renderList.Add(mapRenderer);
            _camera.Position = new Vector3(0, 50, 0);
            _camera.RotateX(-MathHelper.PiOver2);
            IChunk c;
//            c = mapRenderer.Map[new Point3L(0, 0, 0)];
//            c = mapRenderer.Map[new Point3L(-1, 0, 0)];
//            c = mapRenderer.Map[new Point3L(-1, 0, -1)];
//            c = mapRenderer.Map[new Point3L(0, 0, -1)];

            // Build trident and add to the render list
            //_renderList.Add( new Trident() );

            // Disabled fog... I'm not sure how to make it do what I want
//			GL.Enable(EnableCap.DepthTest); //enable the depth testing
//			GL.Enable(EnableCap.Fog);
//			GL.Fog(FogParameter.FogMode,(int)FogMode.Exp2); //glFogi (GL_FOG_MODE, GL_EXP2); //set the fog mode to GL_EXP2
//			GL.Fog(FogParameter.FogColor,new float[]{0.5f,0.5f,0.5f,1.0f}); 
//			GL.Fog(FogParameter.FogDensity,0.03f);
//			GL.Fog(FogParameter.FogStart,1.0f);//3*glState.FarClippingPlane/4);
//			GL.Fog(FogParameter.FogEnd,5.0f);//glState.FarClippingPlane+1);
//			GL.Hint(HintTarget.FogHint,HintMode.Nicest);

            Constants.Kernel.Get<IClientConnection>().Connect("127.0.0.1", 12421);
            //Constants.Kernel.Get<IClientConnection>().Connect("192.168.0.110", 12421);
            GC.Collect();
        }

        void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Constants.Kernel.Dispose();
        }    

        private double _fps = 0; // render frequency adder
        private int _fpsCount = 0; // Frames since last FPS update
        private readonly int _maxFpsCount = 60; // Max number of frames between FPS updates
        public string TitleSuffix = "";
        private Trident _trident = new Trident( );
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Put FPS in title bar
            _fps += this.RenderFrequency;
            _fpsCount += 1;
            if(_fpsCount == _maxFpsCount) {
                this.Title = string.Format("BlockSpaces - {0:0d}FPS{1}", _fps / _fpsCount, TitleSuffix);
                _fps = 0;
                _fpsCount = 0;
            }

            // Remove previous rendering
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            // Render camera and skybox
            _camera.RenderRotation();
            skyBox.Render();
            if(_config.EnableTrident)
                _trident.Render();
            _camera.RenderTranslation();

            // Render each item in the render list
            foreach(var renderable in _renderList) {
                renderable.Render();
            }
           
            SwapBuffers();
            
            if(_config.DebugSleepTime != 0)
                Thread.Sleep(_config.DebugSleepTime);
        }

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Console.WindowWidth = 100;
            using (BSClient example = new BSClient())
            {
                example.Run(30.0);
            }
        }

        private double _currentTime = 0;
        private double _elapsedTime = 0;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _currentTime += e.Time;
            _elapsedTime = e.Time;
            if (Tick != null) Tick(this, new TimeTickEventArgs(this.ElapsedTime, this.CurrentTime));
        }

        public event EventHandler<TimeTickEventArgs> Tick;

        public double CurrentTime
        {
            get { return _currentTime; }
        }

        public double ElapsedTime
        {
            get { return _elapsedTime; }
        }
    }
}