
using System;
using System.Collections.Generic;

using Ninject;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK.Input;
using System.Drawing;
using JollyBit.BS.Core;

using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Client {
	public class InputConfig : IConfigSection {
		public float KeySpeed = 0.25f;
        public float MouseSpeed = 0.001f;
		public bool InvertMouse = false;
		public float ShiftKeyMultiplier = 8f;
	}
	
	public class Input {
		private readonly BSClient _gameWindow;
		private Point _center;
		private bool _registered = true;
		private InputConfig _config;
		
		public Input(BSClient client) {
			this._gameWindow = client;
			
			// Get the config
			_config = Constants.Kernel.Get<IConfigManager>().GetConfig<InputConfig>();
			
			// Center and hide the cursor
			this.recenter();
			
			// Bind events
			this._gameWindow.Move += new EventHandler<EventArgs>(this.handler);
			this._gameWindow.Resize += new EventHandler<EventArgs>(this.handler);
			this._gameWindow.FocusedChanged += new EventHandler<EventArgs>(this.OnFocusedChanged);
			this._gameWindow.UpdateFrame += new EventHandler<FrameEventArgs>(this.OnUpdateFrame);
		}
		
		protected void recenter() {
			// Recompute the center
			this._center = new Point();
	        _center.X = (int)(this._gameWindow.Location.X + this._gameWindow.Width / 2.0f);
	        _center.Y = (int)(this._gameWindow.Location.Y + this._gameWindow.Height / 2.0f);
	
			// Center and hide the cursor
			System.Windows.Forms.Cursor.Position = this._center;
			System.Windows.Forms.Cursor.Hide();
		}
		
		protected void register() {
			this._registered = true;
			this._gameWindow.TitleSuffix = "";
			this.recenter();
		}
		
		protected void unregister() {
			this._registered = false;
			this._gameWindow.TitleSuffix = " (Input Disconnected)";
			System.Windows.Forms.Cursor.Show();	// Unhide the cursor
		}
		
		protected void handler(Object sender, EventArgs e) {
			if(this._registered)
				this.recenter();
		}
		
		protected void OnFocusedChanged(Object sender, EventArgs e) {
			if(!this._gameWindow.Focused) // We lost the focus - unregister
				this.unregister();
		}
		
		protected void OnUpdateFrame(Object sender, FrameEventArgs e) {
			if (this._gameWindow.Keyboard[Key.Space]) {
				if(this._registered)
					this.unregister();
				else
					this.register();
			}
			
			if(_registered) {             
				if (this._gameWindow.Keyboard[Key.Escape])
	                this._gameWindow.Exit();
			
				float speedMultiplier = 1f;
				if (this._gameWindow.Keyboard[Key.ShiftLeft] || this._gameWindow.Keyboard[Key.ShiftRight])
					speedMultiplier = _config.ShiftKeyMultiplier;
				
				if (this._gameWindow.Keyboard[Key.W])
	                this._gameWindow.Camera.MoveForward(_config.KeySpeed * speedMultiplier);
	            if (this._gameWindow.Keyboard[Key.S])
	                this._gameWindow.Camera.MoveForward(-_config.KeySpeed * speedMultiplier);
	            if (this._gameWindow.Keyboard[Key.D])
	                this._gameWindow.Camera.StrafeRight(_config.KeySpeed * speedMultiplier);
	            if (this._gameWindow.Keyboard[Key.A])
	                this._gameWindow.Camera.StrafeRight(-_config.KeySpeed * speedMultiplier);
				if (this._gameWindow.Keyboard[Key.Q])
					this._gameWindow.Camera.MoveUpward(_config.KeySpeed * speedMultiplier);
				if (this._gameWindow.Keyboard[Key.E])
					this._gameWindow.Camera.MoveUpward(-_config.KeySpeed * speedMultiplier);
				
	            if (this._gameWindow.Keyboard[Key.Left])
	                this._gameWindow.Camera.RotateY(_config.KeySpeed/4);
	            if (this._gameWindow.Keyboard[Key.Right])
	                this._gameWindow.Camera.RotateY(-_config.KeySpeed/4);
				if (this._gameWindow.Keyboard[Key.Up])
	                this._gameWindow.Camera.RotateX(-_config.KeySpeed/4);
	            if (this._gameWindow.Keyboard[Key.Down])
	                this._gameWindow.Camera.RotateX(_config.KeySpeed/4);
	
				Point delta = new Point(_center.X - System.Windows.Forms.Cursor.Position.X, _center.Y - System.Windows.Forms.Cursor.Position.Y );
				this._gameWindow.Camera.RotateY(delta.X * _config.MouseSpeed);
				this._gameWindow.Camera.RotateX(delta.Y * (_config.InvertMouse ? -1 : 1) * _config.MouseSpeed);
				System.Windows.Forms.Cursor.Position = _center;
			}
		}
	}
}