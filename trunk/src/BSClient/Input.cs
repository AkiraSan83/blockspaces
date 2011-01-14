
using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK.Input;
using System.Drawing;

//using JollyBit.BS;

namespace JollyBit.BS {
	public class Input {
		private float _keySpeed = 0.05f;
        private float _mouseSpeed = 0.001f;
		
		private readonly BSClient _gameWindow;
		private Point _center;
		private bool _registered = true;
		
		public Input(BSClient client) {
			this._gameWindow = client;
			
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
			this.recenter();
		}
		
		protected void unregister() {
			this._registered = false;
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
			
				if (this._gameWindow.Keyboard[Key.W])
	                this._gameWindow.Camera.MoveForward(_keySpeed);
	            if (this._gameWindow.Keyboard[Key.S])
	                this._gameWindow.Camera.MoveForward(-_keySpeed);
	            if (this._gameWindow.Keyboard[Key.D])
	                this._gameWindow.Camera.StrafeRight(_keySpeed);
	            if (this._gameWindow.Keyboard[Key.A])
	                this._gameWindow.Camera.StrafeRight(-_keySpeed);
	            if (this._gameWindow.Keyboard[Key.Q])
	                this._gameWindow.Camera.RotateY(_keySpeed);
	            if (this._gameWindow.Keyboard[Key.E])
	                this._gameWindow.Camera.RotateY(-_keySpeed);
				if (this._gameWindow.Keyboard[Key.Z])
	                this._gameWindow.Camera.RotateX(_keySpeed);
	            if (this._gameWindow.Keyboard[Key.X])
	                this._gameWindow.Camera.RotateX(-_keySpeed);
	
				Point delta = new Point(_center.X - System.Windows.Forms.Cursor.Position.X, _center.Y - System.Windows.Forms.Cursor.Position.Y );
				this._gameWindow.Camera.RotateY(-delta.X * _mouseSpeed);
				this._gameWindow.Camera.RotateX(-delta.Y * _mouseSpeed);
				System.Windows.Forms.Cursor.Position = _center;
			}
		}
	}
}