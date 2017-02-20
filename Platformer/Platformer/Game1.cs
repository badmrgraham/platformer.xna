using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Platformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont baseFont;

        // We store our input states so that we only poll once per frame, 
        // then we use the same input state wherever needed
        private GamePadState gamePadState;
        private KeyboardState keyboardState;
        private bool wasContinuePressed;
        private static Level level;
        private static Player player;
        public static Texture2D SpriteMap { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Constants.WindowX;
            graphics.PreferredBackBufferHeight = Constants.WindowY;
            graphics.ApplyChanges();
            graphics.IsFullScreen = false;
//            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            player = new Player(new Vector2(256, 256));
            level = Level.ImportLevel(@"maps\map1.dat");
            level.Player = player;
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteMap = Content.Load<Texture2D>(@"Content\Sprites\spritemap");
            player.LoadContent(Content);
            foreach (var obj in level.Enemies)
            {
                obj.LoadContent(Content);
            }
            // TODO: use this.Content to load your game content here
            baseFont = Content.Load<SpriteFont>("Content/Fonts/baseFont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            player.Update(gameTime);
            if (player.IsAttacking)
            {
                level.ResolveAttack(gameTime);
            }
            foreach (var enemy in level.Enemies)
            {
                enemy.Update(gameTime);
            }

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            // get all of our input states
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);

            // Exit the game when back is pressed.
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            level.HandleMove(keyboardState, player.Speed);

            if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                player.Speed = 4;
            else if (keyboardState.IsKeyUp(Keys.LeftShift) || keyboardState.IsKeyUp(Keys.RightShift))
                player.Speed = 2;

            if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl))
            {
                if (!player.IsAttacking) player.IsAttacking = true;
            }

            bool continuePressed =
                keyboardState.IsKeyDown(Keys.Space) ||
                gamePadState.IsButtonDown(Buttons.A);

            wasContinuePressed = continuePressed;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here  
            spriteBatch.Begin();
            level.Draw(spriteBatch);
            player.Draw(spriteBatch, gameTime);
            foreach (var enemy in level.Enemies)
            {
                enemy.Draw(spriteBatch, gameTime);
            }
            spriteBatch.End();

            base.Draw(gameTime);

        }

        public static void TransitionMaps(MapTransition transition)
        {
            level = Level.ImportLevel(transition.TargetMap);
            level.Player = player;
            var spawnLocation = level.MapTransitions.First(a => a.TransitionPath == transition.TransitionPath).Rectangle;
            level.Player.MapLocation = new Vector2(spawnLocation.X * Constants.SpriteWidth, spawnLocation.Y * Constants.SpriteWidth);
            var cameraLocation = level.Player.MapLocation - new Vector2(Constants.WindowX / 2, Constants.WindowY / 2);
            if (cameraLocation.X < 0)
            {
                cameraLocation.X = 0;
            }
            if (cameraLocation.Y < 0)
            {
                cameraLocation.Y = 0;
            }
            if (cameraLocation.X > Constants.NumMapX * Constants.SpriteWidth - Constants.TilesToShowX * Constants.SpriteWidth)
            {
                cameraLocation.X = Constants.NumMapX * Constants.SpriteWidth - Constants.TilesToShowX * Constants.SpriteWidth;
            }
            if (cameraLocation.Y > Constants.NumMapY * Constants.SpriteHeight - Constants.TilesToShowY * Constants.SpriteHeight)
            {
                cameraLocation.Y = Constants.NumMapY * Constants.SpriteHeight - Constants.TilesToShowY * Constants.SpriteHeight;
            }
            Camera.Location = cameraLocation;
        }
    }
}
