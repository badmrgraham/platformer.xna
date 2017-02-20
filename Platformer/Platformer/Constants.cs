using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    class Constants
    {
        public const int SpriteWidth = 32;
        public const int SpriteHeight = 32;
        public const int NumSpriteX = 4;
        public const int NumSpriteY = 4;
        public const int WindowX = 20 * SpriteWidth;
        public const int WindowY = 15 * SpriteHeight;


        public const int NumMapX = 32;
        public const int NumMapY = 32;
        public const int TilesToShowX = 20;
        public const int TilesToShowY = 15;
        //public const int MoveSpeed = 2;

        public const int NoScrollThresholdLeft = 5 * SpriteWidth;
        public const int NoScrollThresholdRight = TilesToShowX * SpriteWidth - 5 * SpriteWidth;
        public const int NoScrollThresholdTop = 5 * SpriteHeight;
        public const int NoScrollThresholdBottom = TilesToShowY * SpriteHeight - 5 * SpriteHeight;
    }
}
