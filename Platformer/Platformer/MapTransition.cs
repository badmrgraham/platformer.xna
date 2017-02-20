using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Platformer
{
    [Serializable]
    public class MapTransition
    {
        public string TargetMap { get; set; }
        public Rectangle Rectangle { get; set; }
        public int TransitionPath { get; set; }
        public bool Exit { get; set; }
    }
}
