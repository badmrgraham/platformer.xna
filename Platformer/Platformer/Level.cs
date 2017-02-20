using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Platformer
{
    [Serializable]
    public class Level
    {
        private Random random;
        private Block[,] spritemap = new Block[Constants.NumMapX, Constants.NumMapY];
        private Rectangle?[] sprites;

        [XmlIgnore]
        public Player Player { get; set; }
        public Vector2 MapDimensions { get; set; }
        public string MapName { get; set; }
        public List<Enemy> Enemies { get; set; }
        [XmlIgnore]
        public Block[,] SpriteMap
        {
            get
            {
                return spritemap;
            }
            set
            {
                spritemap = value;
            }
        }

        public List<MapTransition> MapTransitions { get; set; }

        [XmlIgnore]
        public Rectangle?[] Sprites
        {
            get
            {
                if (sprites == null)
                {
                    sprites = new Rectangle?[Constants.NumSpriteX * Constants.NumSpriteY];
                    int count = 0;
                    for (int y = 0; y < Constants.NumSpriteY; y++)
                    {
                        for (int x = 0; x < Constants.NumSpriteX; x++)
                        {
                            sprites[count++] = new Rectangle?(new Rectangle(Constants.SpriteWidth * x, Constants.SpriteHeight * y, Constants.SpriteWidth, Constants.SpriteHeight));
                        }
                    }
                }
                return sprites;
            }
        }

        public string SpriteMapAsString
        {
            get
            {
                return ConvertSpritemapToString();
            }
            set
            {
                SpriteMap = ConvertStringToSpritemap(value);
            }
        }

        private Block[,] ConvertStringToSpritemap(string str)
        {
            Block[,] sm = new Block[(int)MapDimensions.X, (int)MapDimensions.Y];
            int x = 0;
            int y = 0;

            var smap = str.Split(new []{' ', '\n'});
            foreach (var val in smap)
            {
                if (string.IsNullOrEmpty(val))
                {
                    y++;
                    x = 0;
                }
                else
                {
                    sm[x++, y] = new Block() { BlockID = Convert.ToInt32(val) };
                }
            }

            return sm;
        }

        private string ConvertSpritemapToString()
        {
            StringBuilder str = new StringBuilder();
            for (int y = 0; y < MapDimensions.Y; y++)
            {
                for (int x = 0; x < MapDimensions.X; x++)
                {
                    str.Append(string.Format("{0} ", spritemap[x, y].BlockID));
                }
                str.Append(Environment.NewLine);
            }
            return str.ToString();
        }

        public void ExportLevel()
        {
            var serializer = new XmlSerializer(this.GetType());
            using (var sw = new FileStream("map.dat", FileMode.Create))
            {
                serializer.Serialize(sw, this);
            }
        }

        public static Level ImportLevel(string filename)
        {
            var serializer = new XmlSerializer(typeof(Level));
            using (var sr = new FileStream(filename, FileMode.Open))
            {
                return (Level)serializer.Deserialize(sr);
            }
        }

        public Level() 
        {

        }

        public Level(Player player, string map)
        {
            Player = player;
            MapName = "map1";
            random = new Random(DateTime.Now.Millisecond);
            MapDimensions = new Vector2(Constants.NumMapX, Constants.NumMapY);

            for (int y = 0; y < Constants.NumMapY; y++)
            {
                for (int x = 0; x < Constants.NumMapX; x++)
                {
                    if (y == Constants.NumMapY - 1 || x == Constants.NumMapX - 1 || x == 0 || y == 0)
                    {
                        spritemap[x, y] = new Block() { BlockID = 3 };
                    }
                    else
                    {
                        spritemap[x, y] = new Block() { BlockID = random.Next(0, 3) };
                    }
                }
            }
        }

        public void HandleMove(KeyboardState keyboardState, int moveSpeed)
        {
            var vector = new Vector2(0,0);
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                var intvector = new Vector2(-moveSpeed, 0);
                if (CanMove(intvector))
                {
                    vector += intvector;
                }

                var playerPositionOnCamera = new Vector2(Player.MapLocation.X - Camera.Location.X, Player.MapLocation.Y - Camera.Location.Y);
                if (playerPositionOnCamera.X < Constants.NoScrollThresholdLeft) // should we scroll the camera?
                {
                    Camera.Location.X = MathHelper.Clamp(Camera.Location.X - moveSpeed, 0, (Constants.NumMapX - Constants.TilesToShowX) * Constants.SpriteWidth);
                }
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                var intvector = new Vector2(moveSpeed, 0);
                if (CanMove(intvector))
                {
                    vector += intvector;
                }

                var playerPositionOnCamera = new Vector2(Player.MapLocation.X - Camera.Location.X, Player.MapLocation.Y - Camera.Location.Y);
                if (playerPositionOnCamera.X > Constants.NoScrollThresholdRight) // should we scroll the camera?
                {
                    Camera.Location.X = MathHelper.Clamp(Camera.Location.X + moveSpeed, 0, (Constants.NumMapX - Constants.TilesToShowX) * Constants.SpriteWidth);
                }
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                var intvector = new Vector2(0, -moveSpeed);
                if (CanMove(intvector))
                {
                    vector += intvector;
                }

                var playerPositionOnCamera = new Vector2(Player.MapLocation.X - Camera.Location.X, Player.MapLocation.Y - Camera.Location.Y);
                if (playerPositionOnCamera.Y < Constants.NoScrollThresholdTop) // should we scroll the camera?
                {
                    Camera.Location.Y = MathHelper.Clamp(Camera.Location.Y - moveSpeed, 0, (Constants.NumMapY - Constants.TilesToShowY) * Constants.SpriteHeight);
                }
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                var intvector = new Vector2(0, moveSpeed);
                if (CanMove(intvector))
                {
                    vector += intvector;
                }

                var playerPositionOnCamera = new Vector2(Player.MapLocation.X - Camera.Location.X, Player.MapLocation.Y - Camera.Location.Y);
                if (playerPositionOnCamera.Y > Constants.NoScrollThresholdBottom) // should we scroll the camera?
                {
                    Camera.Location.Y = MathHelper.Clamp(Camera.Location.Y + moveSpeed, 0, (Constants.NumMapY - Constants.TilesToShowY) * Constants.SpriteHeight);
                }
            }

            Player.MapLocation += vector;
            Player.Angle = GetAngle(vector);
            Player.isMoving = vector != new Vector2(0, 0);
            
            var rect = new Rectangle(((int)Player.MapLocation.X + ((int)vector.X > 0 ? Constants.SpriteWidth : 0)) / Constants.SpriteWidth, 
                                     ((int)Player.MapLocation.Y + ((int)vector.Y > 0 ? Constants.SpriteHeight : 0)) / Constants.SpriteHeight, 1,1);
            foreach (var transition in MapTransitions)
            {
                if (transition.Exit && transition.Rectangle.Intersects(rect))
                {
                    Game1.TransitionMaps(transition);
                }
            }
        }

        private float GetAngle(Vector2 vector)
        {
            if (vector.X > 0)
            {
                if (vector.Y > 0)
                {
                    return (3.0f / 4.0f * MathHelper.Pi);
                }
                else if (vector.Y < 0) return (MathHelper.Pi / 4);
                else return MathHelper.Pi / 2;
            }
            else if (vector.X < 0)
            {
                if (vector.Y > 0) return (MathHelper.Pi + MathHelper.Pi / 4);
                else if (vector.Y < 0) return (MathHelper.Pi + 3.0f / 4.0f * MathHelper.Pi);
                else return MathHelper.Pi + MathHelper.Pi / 2;
            }
            else
            {
                if (vector.Y > 0) return MathHelper.Pi;
                else if (vector.Y < 0) return 0;
                else return Player.Angle;
            }
        }

        private bool CanMove(Vector2 vector)
        {
            bool canMove = true;
            try
            {
                var destination = Player.Center + vector;

                var mapBlock = new Vector2(((int)destination.X + (vector.X > 0 ? 10 : -10)) / Constants.SpriteWidth, ((int)destination.Y + (vector.Y > 0 ? 10 : -10)) / Constants.SpriteHeight);

                if (spritemap[(int)mapBlock.X, (int)mapBlock.Y].BlockID == 3)
                {
                    canMove = false;
                }
            }
            catch (Exception ex)
            {
                Debug.Fail("Exception in CanMove: " + ex);
            }
            return canMove;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawBackground(spriteBatch);
        }

        private void DrawBackground(SpriteBatch spriteBatch)
        {
            Vector2 firstSquare = new Vector2(Camera.Location.X / Constants.SpriteWidth, Camera.Location.Y / Constants.SpriteHeight);
            int firstX = (int)firstSquare.X;
            int firstY = (int)firstSquare.Y;

            Vector2 squareOffset = new Vector2(Camera.Location.X % Constants.SpriteWidth, Camera.Location.Y % Constants.SpriteHeight);
            int offsetX = (int)squareOffset.X;
            int offsetY = (int)squareOffset.Y;

            for (int y = 0; y <= Constants.TilesToShowY; y++)
            {
                for (int x = 0; x <= Constants.TilesToShowX; x++)
                {
                    if (x == Constants.TilesToShowX && y == Constants.TilesToShowY)
                    {
                        if (offsetX != 0 && offsetY != 0)
                        {
                            var offsetSprite = Sprites[spritemap[x + firstX, y + firstY].BlockID].Value;
                            offsetSprite.Height = offsetY;
                            offsetSprite.Width = offsetX;
                            spriteBatch.Draw(
                                    Game1.SpriteMap,
                                    new Rectangle((x * Constants.SpriteWidth) - offsetX, (y * Constants.SpriteHeight) - offsetY, offsetX, offsetY),
                                    offsetSprite,
                                    Color.White);
                        }
                    }
                    else if (x == Constants.TilesToShowX)
                    {
                        if (offsetX != 0)
                        {
                            var offsetSprite = Sprites[spritemap[x + firstX, y + firstY].BlockID].Value;
                            offsetSprite.Width = offsetX;
                            spriteBatch.Draw(
                                Game1.SpriteMap,
                                new Rectangle((x * Constants.SpriteWidth) - offsetX, (y * Constants.SpriteHeight) - offsetY, offsetX, Constants.SpriteHeight),
                                offsetSprite,
                                Color.White);
                        }
                    }
                    else if (y == Constants.TilesToShowY)
                    {
                        if (offsetY != 0)
                        {
                            var offsetSprite = Sprites[spritemap[x + firstX, y + firstY].BlockID].Value;
                            offsetSprite.Height = offsetY;
                            spriteBatch.Draw(
                                Game1.SpriteMap,
                                new Rectangle((x * Constants.SpriteWidth) - offsetX, (y * Constants.SpriteHeight) - offsetY, Constants.SpriteWidth, offsetY),
                                offsetSprite,
                                Color.White);
                        }
                    }
                    else
                    {
                        spriteBatch.Draw(
                            Game1.SpriteMap,
                            new Rectangle((x * Constants.SpriteWidth) - offsetX, (y * Constants.SpriteHeight) - offsetY, Constants.SpriteWidth, Constants.SpriteHeight),
                            Sprites[spritemap[x + firstX, y + firstY].BlockID],
                            Color.White);
                    }
                }
            }
        }

        public void ResolveAttack(GameTime gameTime)
        {
            var attackZone = new Rectangle((int)Player.MapLocation.X, (int)Player.MapLocation.Y - Constants.SpriteHeight, Constants.SpriteWidth, Constants.SpriteWidth);
            foreach (var enemy in Enemies)
            {
                var enemyRect = new Rectangle((int)enemy.MapLocation.X, (int)enemy.MapLocation.Y, Constants.SpriteHeight, Constants.SpriteWidth);
                if (enemyRect.Intersects(attackZone))
                {
                    enemy.isHit(new Vector2(Player.MapLocation.X + 0.5f * (float)Constants.SpriteWidth, Player.MapLocation.Y));
                }
            }
        }
    }
}
