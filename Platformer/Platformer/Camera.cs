using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Platformer
{
    public static class Camera
    {
        static public Vector2 Location = Vector2.Zero;

        public static bool IsOnCamera(Vector2 point)
        {
            bool isOnCamera = false;
            try
            {
                Rectangle viewRect = new Rectangle((int)Location.X, (int)Location.Y, Constants.TilesToShowX * Constants.SpriteWidth, Constants.TilesToShowY * Constants.SpriteHeight);
                isOnCamera = viewRect.Contains(new Point((int)point.X, (int)point.Y));
            }
            catch (Exception ex)
            {
                Debug.Fail("Exception in IsOnCamera: " + ex.ToString());
            }
            return isOnCamera;
        }
    }
}
