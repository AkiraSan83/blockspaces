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
#endregion

namespace JollyBit.BS.Client
{
	public class ClientConfig : IConfigSection {
		public int DebugSleepTime = 0;
		public bool EnableTrident = false;
		//public double MaxFPS = 60.0;
		//public bool VSync = true;
	}
	
	public class BSClient : GameWindow
    {
		private IList<IRenderable> _renderList = new List<IRenderable>();
		private Camera _camera = new Camera();
		
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
			
            //Setup Ninject
            Constants.Kernel = new StandardKernel();
            Constants.Kernel.Load(new INinjectModule[] { new BSCoreNinjectModule(), new BSClientNinjectModule() });

			// Handle mouse and keyboard events
			new Input(this);
			
			// Get client config
			_config = Constants.Kernel.Get<IConfigManager>().GetConfig<ClientConfig>();
			//this.TargetRenderFrequency = _config.MaxFPS;
			//Console.WriteLine(this.TargetRenderFrequency);
			
			//this.VSync = _config.VSync ? VSyncMode.On : VSyncMode.Off;
			
            // Set OpenGL options
            Constants.Kernel.Get<GLState>();

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
            _renderList.Add(mapRenderer);
            _camera.Position = new Vector3(0, 50, 0);
            _camera.RotateX(-MathHelper.PiOver2);
            IChunk c;
//            c = mapRenderer.Map[new Point3L(0, 0, 0)];
//            c = mapRenderer.Map[new Point3L(-1, 0, 0)];
//            c = mapRenderer.Map[new Point3L(-1, 0, -1)];
//            c = mapRenderer.Map[new Point3L(0, 0, -1)];
			for(int i = 0; i < 2; i++) {
				for(int j = 0; j < 2; j++) {
					c = mapRenderer.Map[new Point3L(i*Constants.CHUNK_SIZE_X-1, 0, j*Constants.CHUNK_SIZE_Z-1)];
					Console.WriteLine("Generating chunk {0}x{1}",i,j);
				}
			}
			// Build trident and add to the render list
			//_renderList.Add( new Trident() );
			
			GL.Enable(EnableCap.DepthTest); //enable the depth testing
			GL.Enable(EnableCap.Fog);
			GL.Fog(FogParameter.FogMode,(int)FogMode.Exp2); //glFogi (GL_FOG_MODE, GL_EXP2); //set the fog mode to GL_EXP2
			GL.Fog(FogParameter.FogColor,new float[]{0.25f,0.25f,0.25f}); 
			GL.Fog(FogParameter.FogDensity,0.02f);
			GL.Hint(HintTarget.FogHint,HintMode.Nicest);
			
			GC.Collect();
        }    

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
			
            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, Constants.Kernel.Get<GLState>().FarClippingPlane);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
            GL.MatrixMode(MatrixMode.Modelview);
        }

		private double _fps = 0; // render frequency adder
		private int _fpsCount = 0; // Frames since last FPS update
		private readonly int _maxFpsCount = 60; // Max number of frames between FPS updates
		public string TitleSuffix = "";
		private Trident _trident = new Trident( new Vector3(3.8f,-3.5f,-10) );
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

            ////Render a texture
            //tex.Render();
            //GL.Color3(Color.White);
            //GL.Begin(BeginMode.Quads);
            ////GL.Color3(Color.Pink); 
            //GL.Vertex2(-0.6f, 0.4f); GL.TexCoord2(0.0f, 0.0f);
            ////GL.Color3(Color.Blue);
            //GL.Vertex2(0.6f, 0.4f); GL.TexCoord2(1.0f, 0.0f);
            ////GL.Color3(Color.Green);
            //GL.Vertex2(0.6f, -0.4f); GL.TexCoord2(1.0f, 1.0f);
            ////GL.Color3(Color.Red);
            //GL.Vertex2(-0.6f, -0.4f); GL.TexCoord2(0.0f, 1.0f);
            //GL.End();

			// Render each item in the render list
			foreach(var renderable in _renderList) {
				renderable.Render();
			}
           
            SwapBuffers();
			
			if(_config.DebugSleepTime != 0)
				Thread.Sleep(_config.DebugSleepTime);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Constants.Kernel.Get<IConfigManager>().SaveConfig();
            base.OnClosing(e);
        }

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (BSClient example = new BSClient())
            {
				example.Run(30.0);
            }
        }
    }
}