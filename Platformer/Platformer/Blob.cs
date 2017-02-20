using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;

namespace Platformer
{
    public class Blob : Enemy
    {
        private Animation idleAnimation;
        private AnimationPlayer sprite;
        private TimeSpan recoilTicks = TimeSpan.MinValue;
        private Vector2 recoilVector;
        private int recoilCount;

        public override Vector2 MapLocation { get; set; }
        public override bool IsRecoiling { get; set; }

        public Blob(Vector2 mapLocation)
        {
            MapLocation = mapLocation;
        }

        public Blob() { }

        public override void LoadContent(ContentManager Content)
        {
            idleAnimation = new Animation(Content.Load<Texture2D>("Content/Sprites/blobIdle"), 0.5f, true);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var enemyScreenLocation = new Vector2();

            enemyScreenLocation.X = MapLocation.X - Camera.Location.X;
            enemyScreenLocation.Y = MapLocation.Y - Camera.Location.Y;

            sprite.Draw(gameTime, spriteBatch, enemyScreenLocation + new Vector2(Constants.SpriteWidth / 2, Constants.SpriteHeight / 2), SpriteEffects.None, 0);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsRecoiling && recoilTicks == TimeSpan.MinValue)
            {
                recoilTicks = gameTime.TotalGameTime + new TimeSpan(0, 0, 0, 0, 250);
            }
            else if (IsRecoiling && recoilTicks >= gameTime.TotalGameTime)
            {
                if (gameTime.TotalGameTime - recoilTicks < new TimeSpan(0, 0, 0, 0, 83) && recoilCount == 0)
                {
                    recoilCount++;
                    MapLocation += recoilVector;
                }
                else if (gameTime.TotalGameTime - recoilTicks < new TimeSpan(0, 0, 0, 0, 166) && recoilCount == 1)
                {
                    recoilCount++;
                    MapLocation += recoilVector;
                }
            }
            else if (IsRecoiling && recoilTicks < gameTime.TotalGameTime)
            {
                IsRecoiling = false;
                recoilTicks = TimeSpan.MinValue;
            }
            sprite.PlayAnimation(idleAnimation, false);
        }

        public override void isHit(Vector2 sourceVector)
        {
            recoilVector = new Vector2((MapLocation.X + Constants.SpriteWidth / 2) - sourceVector.X, (MapLocation.Y + Constants.SpriteHeight / 2) - sourceVector.Y);
            IsRecoiling = true;
            recoilCount = 0;
        }
    }

    [XmlInclude(typeof(Blob))]
    public abstract class Enemy
    {
        public abstract Vector2 MapLocation { get; set; }
        public abstract bool IsRecoiling { get; set; }

        public abstract void LoadContent(ContentManager Content);
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        public abstract void Update(GameTime gameTime);

        public abstract void isHit(Vector2 vector2);
    }
}
