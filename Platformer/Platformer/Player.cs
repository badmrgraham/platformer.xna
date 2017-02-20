using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class Player
    {
        private Animation runningAnimation;
        private Animation idleAnimation;
        private Animation swordSlash;
        private AnimationPlayer sprite;
        private AnimationPlayer swordAnimator;
        private SpriteFont font;
        private bool isAttacking;
        private int speed = 2;
        private TimeSpan attackTicks = TimeSpan.MinValue;

        public Matrix RotationMatrix { get; set; }
        private float rotation;
        public float Rotation
        {
            get { return rotation; }
            set
            {
                float newVal = value;
                while (newVal >= MathHelper.TwoPi)
                {
                    newVal -= MathHelper.TwoPi;
                }
                while (newVal < 0)
                {
                    newVal += MathHelper.TwoPi;
                }

                if (rotation != newVal)
                {
                    rotation = newVal;
                    RotationMatrix = Matrix.CreateRotationY(rotation);
                }

            }
        }

        public Vector2 MapLocation { get; set; }
        public bool IsAttacking { get; set; }
        public int Speed
        {
            get
            {
                return speed;
            }
            set
            {
                if (speed != value)
                {
                    runningAnimation.FrameTime = value != 2 ? 0.1f : 0.3f;
                    speed = value;
                }
            }
        }
        
        public Vector2 Center
        {
            get { return new Vector2(MapLocation.X + Constants.SpriteWidth / 2, MapLocation.Y + Constants.SpriteHeight / 2); }
        }
        public bool isMoving { get; set; }
        public float Angle { get; set; }

        public Player(Vector2 mapLocation)
        {
            RotationMatrix = Matrix.Identity;
            MapLocation = mapLocation;
            Angle = 0;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var playerScreenLocation = new Vector2();

            playerScreenLocation.X = MapLocation.X - Camera.Location.X;
            playerScreenLocation.Y = MapLocation.Y - Camera.Location.Y;

            if (IsAttacking)
            {
                swordAnimator.Draw(gameTime, spriteBatch, getSwordLocation(playerScreenLocation), SpriteEffects.None, Angle);
            }
            sprite.Draw(gameTime, spriteBatch, playerScreenLocation + new Vector2(Constants.SpriteWidth / 2, Constants.SpriteHeight / 2), SpriteEffects.None, Angle);

        }

        private Vector2 getSwordLocation(Vector2 playerScreenLocation)
        {
            var swordLocation = playerScreenLocation + new Vector2(Constants.SpriteWidth / 2, Constants.SpriteHeight / 2);

            swordLocation = Vector2.Transform(new Vector2(5,-12), Matrix.CreateRotationZ(Angle)) + swordLocation;

            return swordLocation;
        }

        public void Update(GameTime gameTime)
        {
            if (IsAttacking && attackTicks == TimeSpan.MinValue)
            {
                attackTicks = gameTime.TotalGameTime + new TimeSpan(0,0,0,0,250);
                swordAnimator.PlayAnimation(swordSlash, true);
            }
            else if (IsAttacking && attackTicks < gameTime.TotalGameTime)
            {
                IsAttacking = false;
                attackTicks = TimeSpan.MinValue;
            }
            if (isMoving)
            {
                sprite.PlayAnimation(runningAnimation, false);
            }
            else
            {
                sprite.PlayAnimation(idleAnimation, false);
            }
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            runningAnimation = new Animation(Content.Load<Texture2D>("Content/Sprites/running"), 0.3f, true);
            idleAnimation = new Animation(Content.Load<Texture2D>("Content/Sprites/idle"), 0.5f, true);
            swordSlash = new Animation(Content.Load<Texture2D>("Content/Sprites/swordslash"), 0.05f, false);
            font = Content.Load<SpriteFont>("Content/Fonts/baseFont");
        }
    }
}