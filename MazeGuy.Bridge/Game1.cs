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
using System.IO;
using System.Runtime.InteropServices;

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

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
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
        KeyboardState oldState;
        bool won = false;
        bool on_the_wall = false;
        bool on_a_step = false;
        bool start;
        SpriteFont font;
        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";

            // MICHAEL - an exercise for you, instead of building the maze like this,
            // make it load C:/Users/Public/Public Documents/maze.txt
            // # = wall
            // s = player start
            // x = exit
            // i = ice
            //   = floor
            // _ = step
            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferHeight = 768;
            //graphics.PreferredBackBufferWidth = 1366;
            string[] look = File.ReadAllLines("../Maze.txt");
            WIDTH = look[0].Length;
            HEIGHT = look.Length;
            maze = new TileType[WIDTH, HEIGHT];
            int y = 0;
            int x = 0;
            start = true;
            foreach (string s in look)
            {
                x = 0;
                foreach (char c in s)
                {
                    Eqiuvalent(c, x, y);
                    x++;
                }
                y++;
            }
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

            base.Initialize();
        }
        protected void Eqiuvalent(char c, int x, int y)
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
                else
                {
                    maze[x, y] = TileType.Wall;
                } if (c == 'x')
                {
                    maze[x, y] = TileType.Exit;
                }
                else if (c == ' ')
                {
                    maze[x, y] = TileType.Floor;
                }
                else if (c == 'u')
                {
                    maze[x, y] = TileType.UnclimableWall;
                }
                else if (c == 't')
                {
                    maze[x, y] = TileType.KillingStep;
                }
                else if (c == 's')
                {
                    manPos = new Vector2(x, y);
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
                else
                {
                    maze[x, y] = TileType.Wall;
                }
            }
            catch (IndexOutOfRangeException)
            {

            }
        }
        protected void Die(GameTime gameTime)
        {
            System.Windows.Forms.MessageBox.Show("You Lost MazeGuy!", "MazeGuy", System.Windows.Forms.MessageBoxButtons.OK);
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("Would you like to restart?", "MazeGuy", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                new Game1().Run();
            }
            else
            {
                result = System.Windows.Forms.MessageBox.Show("Would you like to restart the level?", "MazeGuy", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    if (File.Exists("../Maze " + level + ".txt"))
                    {
                        stepSize = 0.2f;
                        on_a_step = false;
                        on_the_wall = false;
                        string[] look = File.ReadAllLines("../Maze " + level + ".txt");
                        WIDTH = look[0].Length;
                        HEIGHT = look.Length;
                        maze = new TileType[WIDTH, HEIGHT];
                        int y = 0;
                        int x = 0;
                        start = true;
                        foreach (string s in look)
                        {
                            x = 0;
                            foreach (char c in s)
                            {
                                Eqiuvalent(c, x, y);
                                x++;
                            }
                            y++;
                        }
                    }
                }
            }
            Environment.Exit(0);
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D iceTex = Content.Load<Texture2D>("ice");
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

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        float stepSize = 0.1f;
        /// <summary>
        /// Allows the game to run; logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            try
            {
                if (maze[(int)manPos.X, (int)manPos.Y] != TileType.Step)
                {
                    on_a_step = false;
                }
            }
            catch(IndexOutOfRangeException)
            {

            }
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            KeyboardState state = Keyboard.GetState();
            // TODO: Add your update logic here
            
            Vector2 newPos = manPos;
            if (state.WasKeyJustPressed(Keys.Left, oldState))
            {
                newPos.X--;
            }
            if (state.WasKeyJustPressed(Keys.Right, oldState))
            {
                newPos.X++;
            }
            if (state.WasKeyJustPressed(Keys.Up, oldState))
            {
                newPos.Y--;
            }
            if (state.WasKeyJustPressed(Keys.Down, oldState))
            {
                newPos.Y++;
            }
            try
            {
                TileType newTile = maze[(int)newPos.X, (int)newPos.Y];
                if (on_a_step)
                {
                    if (newTile == TileType.Ice)
                    {
                        if (manPos.X > newPos.X)
                            manPos = new Vector2(newPos.X - 1, newPos.Y);
                        else if (manPos.X < newPos.X)
                            manPos = new Vector2(newPos.X + 1, newPos.Y);
                        else if (manPos.Y > newPos.Y)
                            manPos = new Vector2(newPos.X, newPos.Y - 1);
                        else if (manPos.Y < newPos.Y)
                            manPos = new Vector2(newPos.X, newPos.Y + 1);
                    }
                    else if (newTile == TileType.Exit)
                    {
                        won = true;
                    }
                    else if (newTile == TileType.Step)
                    {
                        on_a_step = true;
                    }
                    else if ((newTile == TileType.KillingFloor) || (newTile == TileType.Pit) || (newTile == TileType.KillingStep))
                    {
                        Die(gameTime);
                    }
                    else if (newTile == TileType.UnclimableWall)
                    {
                        
                    }
                    else
                    {
                        manPos = newPos;
                    }
                }
                else if (on_the_wall)
                {
                    if (newTile == TileType.Ice)
                    {
                        if (manPos.X > newPos.X)
                            manPos = new Vector2(newPos.X - 1, newPos.Y);
                        else if (manPos.X < newPos.X)
                            manPos = new Vector2(newPos.X + 1, newPos.Y);
                        else if (manPos.Y > newPos.Y)
                            manPos = new Vector2(newPos.X, newPos.Y - 1);
                        else if (manPos.Y < newPos.Y)
                            manPos = new Vector2(newPos.X, newPos.Y + 1);
                        if (manPos.X > WIDTH)
                        {
                            manPos.X = WIDTH - 1;
                        }
                        else if (manPos.X < 0)
                        {
                            manPos.X = 0;
                        }
                        else if (manPos.Y > HEIGHT)
                        {
                            manPos.Y = HEIGHT - 1;
                        }
                        else if (manPos.Y < 0)
                        {
                            manPos.Y = 0;
                        }
                    }
                    else if (newTile == TileType.Exit)
                    {
                        won = true;
                    }
                    else if (newTile == TileType.UnclimableWall)
                    {

                    }
                    else if (newTile == TileType.Step)
                    {
                        on_a_step = true;
                        on_the_wall = !on_the_wall;
                        if (manPos.X > newPos.X)
                            manPos = new Vector2(newPos.X - 1, newPos.Y);
                        else if (manPos.X < newPos.X)
                            manPos = new Vector2(newPos.X + 1, newPos.Y);
                        else if (manPos.Y > newPos.Y)
                            manPos = new Vector2(newPos.X, newPos.Y - 1);
                        else if (manPos.Y < newPos.Y)
                            manPos = new Vector2(newPos.X, newPos.Y + 1);
                        if (manPos.X > WIDTH)
                        {
                            manPos.X = WIDTH - 1;
                        }
                        else if (manPos.X < 0)
                        {
                            manPos.X = 0;
                        }
                        else if (manPos.Y > HEIGHT)
                        {
                            manPos.Y = HEIGHT - 1;
                        }
                        else if (manPos.Y < 0)
                        {
                            manPos.Y = 0;
                        }
                    }
                    else if (newTile == TileType.KillingFloor || newTile == TileType.KillingStep || newTile == TileType.Pit || newTile == TileType.Floor || newTile == TileType.KillingStep)
                    {
                        Die(gameTime);
                    }
                    else
                    {
                        manPos = newPos;
                    }
                }
                else
                {
                    if (newTile == TileType.Ice)
                    {
                        if (manPos.X > newPos.X)
                            manPos = new Vector2(newPos.X - 1, newPos.Y);
                        else if (manPos.X < newPos.X)
                            manPos = new Vector2(newPos.X + 1, newPos.Y);
                        else if (manPos.Y > newPos.Y)
                            manPos = new Vector2(newPos.X, newPos.Y - 1);
                        else if (manPos.Y < newPos.Y)
                            manPos = new Vector2(newPos.X, newPos.Y + 1);
                        if (manPos.X > WIDTH)
                        {
                            manPos.X = WIDTH - 1;
                        }
                        else if (manPos.X < 0)
                        {
                            manPos.X = 0;
                        }
                        else if (manPos.Y > HEIGHT)
                        {
                            manPos.Y = HEIGHT - 1;
                        }
                        else if (manPos.Y < 0)
                        {
                            manPos.Y = 0;
                        }
                    }
                    else if (newTile == TileType.Exit)
                    {
                        won = true;
                    }
                    else if (newTile == TileType.UnclimableWall || newTile == TileType.Wall)
                    {

                    }
                    else if (newTile == TileType.Step)
                    {
                        on_a_step = true;
                        on_the_wall = !on_the_wall;
                        if (manPos.X > newPos.X)
                            manPos = new Vector2(newPos.X - 1, newPos.Y);
                        else if (manPos.X < newPos.X)
                            manPos = new Vector2(newPos.X + 1, newPos.Y);
                        else if (manPos.Y > newPos.Y)
                            manPos = new Vector2(newPos.X, newPos.Y - 1);
                        else if (manPos.Y < newPos.Y)
                            manPos = new Vector2(newPos.X, newPos.Y + 1);
                        if (manPos.X > WIDTH)
                        {
                            manPos.X = WIDTH - 1;
                        }
                        else if (manPos.X < 0)
                        {
                            manPos.X = 0;
                        }
                        else if (manPos.Y > HEIGHT)
                        {
                            manPos.Y = HEIGHT - 1;
                        }
                        else if (manPos.Y < 0)
                        {
                            manPos.Y = 0;
                        }
                    }
                    else if (newTile == TileType.KillingFloor || newTile == TileType.Pit || newTile == TileType.KillingStep)
                    {
                        Die(gameTime);
                    }
                    else
                    {
                        manPos = newPos;
                    }
                }
                oldState = state;
            }
            catch (IndexOutOfRangeException)
            {
            }
            base.Update(gameTime);
        }
        int level = 1;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            Vector2 manScreenPos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            if (!won && !start)
            {
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

                for (int x = 0; x < WIDTH; x++)
                {
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        Texture2D texture = textures[(int)maze[x, y]];
                        Color color = Color.White;
                        if (maze[x, y] == TileType.Exit)
                            color = Color.Gold;

                        spriteBatch.Draw(texture, new Rectangle(
                            (int)((x - manAnimationPos.X) * texture.Width * 2 + manScreenPos.X),
                            (int)((y - manAnimationPos.Y) * texture.Height * 2 + manScreenPos.Y),
                            texture.Width * 2,
                            texture.Height * 2), color);
                    }
                }
                spriteBatch.Draw(manTexture,
                    new Rectangle(
                        (int)(manScreenPos.X),
                        (int)(manScreenPos.Y),
                        manTexture.Width * 2,
                        manTexture.Height * 2), Color.White); 
            }
            else if (won)
            {
                level++;
                if (File.Exists("../Maze " + level + ".txt"))
                {
                    stepSize = 0.2f;
                    on_a_step = false;
                    on_the_wall = false;
                    string[] look = File.ReadAllLines("../Maze " + level + ".txt");
                    WIDTH = look[0].Length;
                    HEIGHT = look.Length;
                    maze = new TileType[WIDTH, HEIGHT];
                    int y = 0;
                    int x = 0;
                    start = true;
                    foreach (string s in look)
                    {
                        x = 0;
                        foreach (char c in s)
                        {
                            Eqiuvalent(c, x, y);
                            x++;
                        }
                        y++;
                    }
                    won = false;
                }
                else
                {
                    spriteBatch.DrawString(font, "Congratulations! Yow Won!", new Vector2(300, 300), Color.Gold);
                    spriteBatch.End();
                    System.Threading.Thread.Sleep(1000);
                    Environment.Exit(0);
                }
               
            }
            else
            {
                spriteBatch.DrawString(font, "You didn't choose a start!", new Vector2(300, 300), Color.Gold);
                spriteBatch.End();
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(0);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
