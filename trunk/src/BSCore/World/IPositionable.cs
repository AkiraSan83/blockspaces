using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace JollyBit.BS.Core.World
{
    public interface IPositionable
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
    }
}
