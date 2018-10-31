﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace MonoMandlebrot
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<PositionContainer> calulatedPoints;
        List<Vector2> trappedPoints;
        Texture2D image;
        float xMin = -2f;
        float xMax = 2f;
        float yMin = -2f;
        float yMax = 2f;

        const int displayWidth = 1024;
        const int displayHeight = 768;

        bool reRunNumbers = true;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = displayHeight;
            graphics.PreferredBackBufferWidth = displayWidth;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            trappedPoints = new List<Vector2>();
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
            image = new Texture2D(GraphicsDevice, 1, 1);
            Color[] pixelColor = new Color[1];
            pixelColor[0] = Color.Green;
            image.SetData(pixelColor);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Left))
            {
                xMin -= .05f;
                xMax -= .05f;
                reRunNumbers = !reRunNumbers;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                xMin += .05f;
                xMax += .05f;
                reRunNumbers = !reRunNumbers;
            }
            if (state.IsKeyDown(Keys.Up))
            {
                yMin -= .05f;
                yMax -= .05f;
                reRunNumbers = !reRunNumbers;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                yMin += .05f;
                yMax += .05f;
                reRunNumbers = !reRunNumbers;
            }
            if(state.IsKeyDown(Keys.OemPlus) || state.IsKeyDown(Keys.Add))
            {
                xMin += .1f;
                xMax -= .1f;
                yMin += .1f;
                yMax -= .1f;
                reRunNumbers = !reRunNumbers;
            }
            if (state.IsKeyDown(Keys.OemMinus) || state.IsKeyDown(Keys.Subtract))
            {
                xMin -= .1f;
                xMax += .1f;
                yMin -= .1f;
                yMax += .1f;
                reRunNumbers = !reRunNumbers;
            }

            if (reRunNumbers)
            {
                trappedPoints.Clear();
                runSomeNumbers();
                reRunNumbers = !reRunNumbers;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(-1 * (GraphicsDevice.Viewport.X / 2), -1 * (GraphicsDevice.Viewport.Y / 2), 0) * Matrix.CreateScale(2));
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateScale(imageScale) * Matrix.CreateTranslation(position.X,position.Y,0));
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateScale(1));

            foreach (Vector2 point in trappedPoints ?? Enumerable.Empty<Vector2>())
            {
                Vector2 location = point;
                spriteBatch.Draw(image, location, Color.Black);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Vector2 squareComplexNumber(Vector2 complexNumber)
        {
            Vector2 result = new Vector2(complexNumber.X, complexNumber.Y);
            result.X = (complexNumber.X * complexNumber.X) - (complexNumber.Y * complexNumber.Y);
            result.Y = 2 * complexNumber.X * complexNumber.Y;
            return result;
        }

        private Vector2 convertPointToPixel(Vector2 point)
        {
            //Vector2 pixel = new Vector2(point.X + 2 / imageScale, point.Y + 2 / imageScale) * imageScale / ratio;
            //Vector2 pixel = new Vector2(point.X - (-1f * imageScale), point.Y - (-1f * imageScale)) / ((2 * imageScale)/ 600f);
            Vector2 pixel = new Vector2(
                ((point.X - xMin) / ((xMax - xMin) / displayWidth)),
                ((point.Y - yMin) / ((yMax - yMin) / displayHeight))
                );
            return pixel;
        }

        private void runSomeNumbers()
        {
            for (float x = xMin; x <= xMax; x += ((xMax - xMin) / displayWidth))
            {
                for (float y = yMin; y <= yMax; y += ((yMax - yMin) / displayHeight))
                {

                    Vector2 originalPoint = new Vector2(x, y);
                    Vector2 newPoint = new Vector2(x, y);
                    int whilecounter = 0;
                    while (newPoint.X >= -2F && newPoint.X <= 2F && newPoint.Y >= -2F && newPoint.Y <= 2F && whilecounter < 2000)
                    {
                        newPoint = squareComplexNumber(newPoint);
                        newPoint.X += originalPoint.X;
                        newPoint.Y += originalPoint.Y;
                        whilecounter++;
                    }
                    if ((newPoint.X > 2F || newPoint.X < -2F || newPoint.Y < -2f || newPoint.Y > 2F) && whilecounter < 2000)
                    {
                        //Do nothing if it escaped
                        //Console.WriteLine(originalPoint + ": escaped! With a final result of:" + newPoint);
                    }
                    else
                    {
                        //Convert to pixel then add it
                        Vector2 pixelFromPoint = convertPointToPixel(originalPoint);
                        trappedPoints.Add(pixelFromPoint);
                        //Console.WriteLine(originalPoint + ": DID NOT ESCAPE!");
                    }
                }
            }
        }
    }
}