using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MazeGuy
{
    static class Extension
    {
        public static bool WasKeyJustPressed(this KeyboardState state, Keys key, KeyboardState oldState)
        {
            return (state.IsKeyDown(key) && !oldState.IsKeyDown(key));
        }
    }

    enum TileType
    {
        Floor = 0,
        Wall = 1,
        Exit = 2,
        Ice = 3,
        Step = 4,
        KillingFloor = 5,
        Pit = 6,
        KillingStep = 7,
        UnclimableWall = 8,
        COUNT
    }

    public partial class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TileType[,] maze;
        Texture2D[] textures = new Texture2D[(int)TileType.COUNT];
        Texture2D manTexture;
        int WIDTH;
        int HEIGHT;
        Vector2 manPos = new Vector2(0, 0);
        Vector2 manAnimationPos = new Vector2(0, 0);
        Vector2 manStepping = new Vector2(0, 0);
        Vector2 startPos = new Vector2(0, 0);
        KeyboardState oldState;
        bool won = false;
        bool dead = false;
        bool on_the_wall = false;
        bool on_a_step = false;
        bool start;
        SpriteFont font;
        int level = 1;
        float stepSize = 0.1f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            LoadLevel(level);
        }

        private void LoadLevel(int levelNum)
        {
            if (!Levels.ContainsKey(levelNum))
            {
                won = true;
                return;
            }

            string[] look = Levels[levelNum];
            WIDTH = look.Max(line => line.Length);
            HEIGHT = look.Length;
            maze = new TileType[WIDTH, HEIGHT];

            // Initialize all tiles to floor
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    maze[x, y] = TileType.Floor;
                }
            }

            int py = 0;
            start = true;
            foreach (string s in look)
            {
                int px = 0;
                foreach (char c in s)
                {
                    ParseTile(c, px, py);
                    px++;
                }
                py++;
            }

            if (start)
            {
                // No start position found, default to 0,0
                manPos = new Vector2(0, 0);
                start = false;
            }

            startPos = manPos;
            manAnimationPos = manPos;
            stepSize = 0.1f;
            on_a_step = false;
            on_the_wall = false;
            won = false;
            dead = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected void ParseTile(char c, int x, int y)
        {
            try
            {
                if (c == 'x')
                {
                    maze[x, y] = TileType.Exit;
                }
                else if (c == ' ')
                {
                    maze[x, y] = TileType.Floor;
                }
                else if (c == 's')
                {
                    manPos = new Vector2(x, y);
                    maze[x, y] = TileType.Floor;
                    start = false;
                }
                else if (c == 'i')
                {
                    maze[x, y] = TileType.Ice;
                }
                else if (c == '_')
                {
                    maze[x, y] = TileType.Step;
                }
                else if (Char.ToUpper(c) == 'O')
                {
                    maze[x, y] = TileType.Pit;
                }
                else if (c == 'd')
                {
                    maze[x, y] = TileType.KillingFloor;
                }
                else if (c == 'u')
                {
                    maze[x, y] = TileType.UnclimableWall;
                }
                else if (c == 't')
                {
                    maze[x, y] = TileType.KillingStep;
                }
                else if (c == '#')
                {
                    maze[x, y] = TileType.Wall;
                }
                else
                {
                    maze[x, y] = TileType.Wall;
                }
            }
            catch
            {
            }
        }

        protected void Die()
        {
            dead = true;
        }

        protected void RestartLevel()
        {
            LoadLevel(level);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            textures[(int)TileType.Ice] = Content.Load<Texture2D>("ice");
            textures[(int)TileType.Exit] = Content.Load<Texture2D>("exit");
            textures[(int)TileType.Wall] = Content.Load<Texture2D>("wall");
            textures[(int)TileType.Floor] = Content.Load<Texture2D>("floor");
            textures[(int)TileType.Step] = Content.Load<Texture2D>("step");
            textures[(int)TileType.KillingFloor] = textures[(int)TileType.Floor];
            textures[(int)TileType.Pit] = Content.Load<Texture2D>("Pit");
            textures[(int)TileType.KillingStep] = textures[(int)TileType.Step];
            textures[(int)TileType.UnclimableWall] = textures[(int)TileType.Wall];
            manTexture = Content.Load<Texture2D>("guy");

            font = Content.Load<SpriteFont>("SpriteFont1");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (dead || won)
            {
                KeyboardState ks = Keyboard.GetState();
                if (ks.WasKeyJustPressed(Keys.Space, oldState) || ks.WasKeyJustPressed(Keys.Enter, oldState))
                {
                    if (dead)
                    {
                        RestartLevel();
                    }
                    else if (won)
                    {
                        level++;
                        LoadLevel(level);
                    }
                }
                if (ks.WasKeyJustPressed(Keys.R, oldState))
                {
                    level = 1;
                    LoadLevel(level);
                }
                oldState = ks;
                base.Update(gameTime);
                return;
            }

            try
            {
                if (maze[(int)manPos.X, (int)manPos.Y] != TileType.Step)
                {
                    on_a_step = false;
                }
            }
            catch
            {
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState state = Keyboard.GetState();

            // Restart level with R
            if (state.WasKeyJustPressed(Keys.R, oldState))
            {
                RestartLevel();
                oldState = state;
                base.Update(gameTime);
                return;
            }

            Vector2 newPos = manPos;
            if (state.WasKeyJustPressed(Keys.Left, oldState) || state.WasKeyJustPressed(Keys.A, oldState))
            {
                newPos.X--;
            }
            if (state.WasKeyJustPressed(Keys.Right, oldState) || state.WasKeyJustPressed(Keys.D, oldState))
            {
                newPos.X++;
            }
            if (state.WasKeyJustPressed(Keys.Up, oldState) || state.WasKeyJustPressed(Keys.W, oldState))
            {
                newPos.Y--;
            }
            if (state.WasKeyJustPressed(Keys.Down, oldState) || state.WasKeyJustPressed(Keys.S, oldState))
            {
                newPos.Y++;
            }

            try
            {
                TileType newTile = maze[(int)newPos.X, (int)newPos.Y];
                if (on_a_step)
                {
                    ProcessMovementOnStep(newPos, newTile);
                }
                else if (on_the_wall)
                {
                    ProcessMovementOnWall(newPos, newTile);
                }
                else
                {
                    ProcessMovementNormal(newPos, newTile);
                }
                oldState = state;
            }
            catch
            {
            }
            base.Update(gameTime);
        }

        private void ProcessMovementOnStep(Vector2 newPos, TileType newTile)
        {
            if (newTile == TileType.Ice)
            {
                SlideOnIce(newPos);
            }
            else if (newTile == TileType.Exit)
            {
                won = true;
            }
            else if (newTile == TileType.Step)
            {
                on_a_step = true;
                manPos = newPos;
            }
            else if (newTile == TileType.KillingFloor || newTile == TileType.Pit || newTile == TileType.KillingStep)
            {
                Die();
            }
            else if (newTile == TileType.UnclimableWall)
            {
                // Can't move there
            }
            else if (newTile != TileType.Wall)
            {
                manPos = newPos;
            }
        }

        private void ProcessMovementOnWall(Vector2 newPos, TileType newTile)
        {
            if (newTile == TileType.Ice)
            {
                SlideOnIce(newPos);
            }
            else if (newTile == TileType.Exit)
            {
                won = true;
            }
            else if (newTile == TileType.UnclimableWall)
            {
                // Can't move there
            }
            else if (newTile == TileType.Step)
            {
                on_a_step = true;
                on_the_wall = !on_the_wall;
                SlideOnIce(newPos);
            }
            else if (newTile == TileType.KillingFloor || newTile == TileType.KillingStep || newTile == TileType.Pit || newTile == TileType.Floor)
            {
                Die();
            }
            else
            {
                manPos = newPos;
            }
        }

        private void ProcessMovementNormal(Vector2 newPos, TileType newTile)
        {
            if (newTile == TileType.Ice)
            {
                SlideOnIce(newPos);
            }
            else if (newTile == TileType.Exit)
            {
                won = true;
            }
            else if (newTile == TileType.UnclimableWall || newTile == TileType.Wall)
            {
                // Can't move there
            }
            else if (newTile == TileType.Step)
            {
                on_a_step = true;
                on_the_wall = !on_the_wall;
                SlideOnIce(newPos);
            }
            else if (newTile == TileType.KillingFloor || newTile == TileType.Pit || newTile == TileType.KillingStep)
            {
                Die();
            }
            else
            {
                manPos = newPos;
            }
        }

        private void SlideOnIce(Vector2 newPos)
        {
            if (manPos.X > newPos.X)
                manPos = new Vector2(newPos.X - 1, newPos.Y);
            else if (manPos.X < newPos.X)
                manPos = new Vector2(newPos.X + 1, newPos.Y);
            else if (manPos.Y > newPos.Y)
                manPos = new Vector2(newPos.X, newPos.Y - 1);
            else if (manPos.Y < newPos.Y)
                manPos = new Vector2(newPos.X, newPos.Y + 1);

            ClampPosition();
        }

        private void ClampPosition()
        {
            if (manPos.X >= WIDTH)
                manPos.X = WIDTH - 1;
            else if (manPos.X < 0)
                manPos.X = 0;
            if (manPos.Y >= HEIGHT)
                manPos.Y = HEIGHT - 1;
            else if (manPos.Y < 0)
                manPos.Y = 0;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            Vector2 manScreenPos = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            if (dead)
            {
                // Draw maze faded
                DrawMaze(manScreenPos, 0.3f);
                spriteBatch.DrawString(font, "You Died!", new Vector2(manScreenPos.X - 50, manScreenPos.Y - 50), Color.Red);
                spriteBatch.DrawString(font, "Press SPACE to restart level", new Vector2(manScreenPos.X - 130, manScreenPos.Y), Color.White);
                spriteBatch.DrawString(font, "Press R to restart game", new Vector2(manScreenPos.X - 110, manScreenPos.Y + 30), Color.Gray);
            }
            else if (won && !Levels.ContainsKey(level + 1) && level > 1)
            {
                // Game complete
                spriteBatch.DrawString(font, "Congratulations! You Won!", new Vector2(manScreenPos.X - 120, manScreenPos.Y - 30), Color.Gold);
                spriteBatch.DrawString(font, "Press R to play again", new Vector2(manScreenPos.X - 100, manScreenPos.Y + 10), Color.White);
            }
            else if (!start)
            {
                // Normal gameplay
                if ((manPos - manAnimationPos).Length() < stepSize)
                {
                    manAnimationPos = manPos;
                    stepSize = 0.1f;
                }
                else
                {
                    Vector2 step = manPos - manAnimationPos;
                    step.Normalize();
                    manAnimationPos += step * stepSize;
                }

                DrawMaze(manScreenPos, 1.0f);

                // Draw player
                spriteBatch.Draw(manTexture,
                    new Rectangle(
                        (int)(manScreenPos.X),
                        (int)(manScreenPos.Y),
                        manTexture.Width * 2,
                        manTexture.Height * 2), Color.White);

                // Draw level indicator
                spriteBatch.DrawString(font, "Level " + level, new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(font, "Arrow Keys or WASD to move", new Vector2(10, 30), Color.Gray);
                spriteBatch.DrawString(font, "R to restart", new Vector2(10, 50), Color.Gray);
            }
            else
            {
                spriteBatch.DrawString(font, "No start position in level!", new Vector2(300, 300), Color.Red);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawMaze(Vector2 manScreenPos, float alpha)
        {
            // Only draw visible tiles (culling)
            int tileSize = 32; // texture.Width * 2
            int viewWidth = GraphicsDevice.Viewport.Width;
            int viewHeight = GraphicsDevice.Viewport.Height;

            int startX = Math.Max(0, (int)manAnimationPos.X - (viewWidth / tileSize / 2) - 1);
            int endX = Math.Min(WIDTH, (int)manAnimationPos.X + (viewWidth / tileSize / 2) + 2);
            int startY = Math.Max(0, (int)manAnimationPos.Y - (viewHeight / tileSize / 2) - 1);
            int endY = Math.Min(HEIGHT, (int)manAnimationPos.Y + (viewHeight / tileSize / 2) + 2);

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Texture2D texture = textures[(int)maze[x, y]];
                    Color color = Color.White;
                    if (maze[x, y] == TileType.Exit)
                        color = Color.Gold;
                    if (alpha < 1.0f)
                        color = new Color(color.R, color.G, color.B, (byte)(255 * alpha));

                    spriteBatch.Draw(texture, new Rectangle(
                        (int)((x - manAnimationPos.X) * texture.Width * 2 + manScreenPos.X),
                        (int)((y - manAnimationPos.Y) * texture.Height * 2 + manScreenPos.Y),
                        texture.Width * 2,
                        texture.Height * 2), color);
                }
            }
        }
    }
}
