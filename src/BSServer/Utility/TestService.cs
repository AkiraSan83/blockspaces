using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.World.Actors;

namespace JollyBit.BS.Server.Utility
{
    public class TestService
    {
        IActor _actor;
        IPositionableComponent _pos;
        private double _timeTillMove = 10;
        public TestService(ITimeService timeService, IActor actor)
        {
            _actor = actor;
            _pos = _actor.Get<IPositionableComponent>();
            timeService.Tick += new EventHandler<TimeTickEventArgs>(timeService_Tick);
        }

        void timeService_Tick(object sender, TimeTickEventArgs e)
        {
            _timeTillMove -= e.ElapsedTime;
            if (_timeTillMove < 0)
            {
                _timeTillMove = 10;
                _pos.Position += new OpenTK.Vector3(1, 1, 1);
            }
        }

    }
}
