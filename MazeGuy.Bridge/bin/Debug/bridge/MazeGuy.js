/**
 * MazeGuy game ported to Bridge.NET
 * @version 1.0.0.0
 * @compiler Bridge.NET 17.10.0
 */
Bridge.assembly("MazeGuy", function ($asm, globals) {
    "use strict";

    Bridge.define("MazeGuy.Extension", {
        statics: {
            methods: {
                WasKeyJustPressed: function (state, key, oldState) {
                    return (state.IsKeyDown(key) && !oldState.IsKeyDown(key));
                }
            }
        }
    });

    Bridge.define("MazeGuy.Game1", {
        inherits: [Microsoft.Xna.Framework.Game],
        statics: {
            fields: {
                Levels: null
            },
            ctors: {
                init: function () {
                    this.Levels = $asm.$.MazeGuy.Game1.f1(new (System.Collections.Generic.Dictionary$2(System.Int32,System.Array.type(System.String))).ctor());
                }
            }
        },
        fields: {
            graphics: null,
            spriteBatch: null,
            maze: null,
            textures: null,
            manTexture: null,
            WIDTH: 0,
            HEIGHT: 0,
            manPos: null,
            manAnimationPos: null,
            manStepping: null,
            startPos: null,
            oldState: null,
            won: false,
            dead: false,
            on_the_wall: false,
            on_a_step: false,
            start: false,
            font: null,
            level: 0,
            stepSize: 0
        },
        ctors: {
            init: function () {
                this.manPos = new Microsoft.Xna.Framework.Vector2();
                this.manAnimationPos = new Microsoft.Xna.Framework.Vector2();
                this.manStepping = new Microsoft.Xna.Framework.Vector2();
                this.startPos = new Microsoft.Xna.Framework.Vector2();
                this.oldState = new Microsoft.Xna.Framework.Input.KeyboardState();
                this.textures = System.Array.init(MazeGuy.TileType.COUNT, null, Microsoft.Xna.Framework.Graphics.Texture2D);
                this.manPos = new Microsoft.Xna.Framework.Vector2.$ctor2(0, 0);
                this.manAnimationPos = new Microsoft.Xna.Framework.Vector2.$ctor2(0, 0);
                this.manStepping = new Microsoft.Xna.Framework.Vector2.$ctor2(0, 0);
                this.startPos = new Microsoft.Xna.Framework.Vector2.$ctor2(0, 0);
                this.won = false;
                this.dead = false;
                this.on_the_wall = false;
                this.on_a_step = false;
                this.level = 2;
                this.stepSize = 0.1;
            },
            ctor: function () {
                this.$initialize();
                Microsoft.Xna.Framework.Game.ctor.call(this);
                this.graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager(this);
                this.Content.RootDirectory = "Content";
                this.LoadLevel(this.level);
            }
        },
        methods: {
            LoadLevel: function (levelNum) {
                var $t, $t1;
                if (!MazeGuy.Game1.Levels.containsKey(levelNum)) {
                    this.won = true;
                    return;
                }

                var look = MazeGuy.Game1.Levels.getItem(levelNum);
                this.WIDTH = System.Linq.Enumerable.from(look, System.String).max($asm.$.MazeGuy.Game1.f2);
                this.HEIGHT = look.length;
                this.maze = System.Array.create(0, null, MazeGuy.TileType, this.WIDTH, this.HEIGHT);

                // Initialize all tiles to floor
                for (var x = 0; x < this.WIDTH; x = (x + 1) | 0) {
                    for (var y = 0; y < this.HEIGHT; y = (y + 1) | 0) {
                        this.maze.set([x, y], MazeGuy.TileType.Floor);
                    }
                }

                var py = 0;
                this.start = true;
                $t = Bridge.getEnumerator(look);
                try {
                    while ($t.moveNext()) {
                        var s = $t.Current;
                        var px = 0;
                        $t1 = Bridge.getEnumerator(s);
                        try {
                            while ($t1.moveNext()) {
                                var c = $t1.Current;
                                this.ParseTile(c, px, py);
                                px = (px + 1) | 0;
                            }
                        } finally {
                            if (Bridge.is($t1, System.IDisposable)) {
                                $t1.System$IDisposable$Dispose();
                            }
                        }
                        py = (py + 1) | 0;
                    }
                } finally {
                    if (Bridge.is($t, System.IDisposable)) {
                        $t.System$IDisposable$Dispose();
                    }
                }

                if (this.start) {
                    // No start position found, default to 0,0
                    this.manPos = new Microsoft.Xna.Framework.Vector2.$ctor2(0, 0);
                    this.start = false;
                }

                this.startPos = this.manPos.$clone();
                this.manAnimationPos = this.manPos.$clone();
                this.stepSize = 0.1;
                this.on_a_step = false;
                this.on_the_wall = false;
                this.won = false;
                this.dead = false;
            },
            Initialize: function () {
                Microsoft.Xna.Framework.Game.prototype.Initialize.call(this);
            },
            ParseTile: function (c, x, y) {
                try {
                    if (c === 120) {
                        this.maze.set([x, y], MazeGuy.TileType.Exit);
                    } else if (c === 32) {
                        this.maze.set([x, y], MazeGuy.TileType.Floor);
                    } else if (c === 115) {
                        this.manPos = new Microsoft.Xna.Framework.Vector2.$ctor2(x, y);
                        this.maze.set([x, y], MazeGuy.TileType.Floor);
                        this.start = false;
                    } else if (c === 105) {
                        this.maze.set([x, y], MazeGuy.TileType.Ice);
                    } else if (c === 95) {
                        this.maze.set([x, y], MazeGuy.TileType.Step);
                    } else if (String.fromCharCode(c).toUpperCase().charCodeAt(0) === 79) {
                        this.maze.set([x, y], MazeGuy.TileType.Pit);
                    } else if (c === 100) {
                        this.maze.set([x, y], MazeGuy.TileType.KillingFloor);
                    } else if (c === 117) {
                        this.maze.set([x, y], MazeGuy.TileType.UnclimableWall);
                    } else if (c === 116) {
                        this.maze.set([x, y], MazeGuy.TileType.KillingStep);
                    } else if (c === 35) {
                        this.maze.set([x, y], MazeGuy.TileType.Wall);
                    } else {
                        this.maze.set([x, y], MazeGuy.TileType.Wall);
                    }
                } catch ($e1) {
                    $e1 = System.Exception.create($e1);
                }
            },
            Die: function () {
                this.dead = true;
            },
            RestartLevel: function () {
                this.LoadLevel(this.level);
            },
            LoadContent: function () {
                this.spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(this.GraphicsDevice);

                this.textures[System.Array.index(MazeGuy.TileType.Ice, this.textures)] = this.Content.Load(Microsoft.Xna.Framework.Graphics.Texture2D, "ice");
                this.textures[System.Array.index(MazeGuy.TileType.Exit, this.textures)] = this.Content.Load(Microsoft.Xna.Framework.Graphics.Texture2D, "exit");
                this.textures[System.Array.index(MazeGuy.TileType.Wall, this.textures)] = this.Content.Load(Microsoft.Xna.Framework.Graphics.Texture2D, "wall");
                this.textures[System.Array.index(MazeGuy.TileType.Floor, this.textures)] = this.Content.Load(Microsoft.Xna.Framework.Graphics.Texture2D, "floor");
                this.textures[System.Array.index(MazeGuy.TileType.Step, this.textures)] = this.Content.Load(Microsoft.Xna.Framework.Graphics.Texture2D, "step");
                this.textures[System.Array.index(MazeGuy.TileType.KillingFloor, this.textures)] = this.textures[System.Array.index(MazeGuy.TileType.Floor, this.textures)];
                this.textures[System.Array.index(MazeGuy.TileType.Pit, this.textures)] = this.Content.Load(Microsoft.Xna.Framework.Graphics.Texture2D, "Pit");
                this.textures[System.Array.index(MazeGuy.TileType.KillingStep, this.textures)] = this.textures[System.Array.index(MazeGuy.TileType.Step, this.textures)];
                this.textures[System.Array.index(MazeGuy.TileType.UnclimableWall, this.textures)] = this.textures[System.Array.index(MazeGuy.TileType.Wall, this.textures)];
                this.manTexture = this.Content.Load(Microsoft.Xna.Framework.Graphics.Texture2D, "guy");

                this.font = this.Content.Load(Microsoft.Xna.Framework.Graphics.SpriteFont, "SpriteFont1");
            },
            UnloadContent: function () { },
            Update: function (gameTime) {
                if (this.dead || this.won) {
                    var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                    if (MazeGuy.Extension.WasKeyJustPressed(ks, Microsoft.Xna.Framework.Input.Keys.Space, this.oldState.$clone()) || MazeGuy.Extension.WasKeyJustPressed(ks, Microsoft.Xna.Framework.Input.Keys.Enter, this.oldState.$clone())) {
                        if (this.dead) {
                            this.RestartLevel();
                        } else if (this.won) {
                            this.level = (this.level + 1) | 0;
                            this.LoadLevel(this.level);
                        }
                    }
                    if (MazeGuy.Extension.WasKeyJustPressed(ks, Microsoft.Xna.Framework.Input.Keys.R, this.oldState.$clone())) {
                        this.level = 1;
                        this.LoadLevel(this.level);
                    }
                    this.oldState = ks.$clone();
                    Microsoft.Xna.Framework.Game.prototype.Update.call(this, gameTime);
                    return;
                }

                try {
                    if (this.maze.get([Bridge.Int.clip32(this.manPos.X), Bridge.Int.clip32(this.manPos.Y)]) !== MazeGuy.TileType.Step) {
                        this.on_a_step = false;
                    }
                } catch ($e1) {
                    $e1 = System.Exception.create($e1);
                }

                if (Microsoft.Xna.Framework.Input.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).Buttons.Back === Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
                    this.Exit();
                }

                var state = Microsoft.Xna.Framework.Input.Keyboard.GetState();

                // Restart level with R
                if (MazeGuy.Extension.WasKeyJustPressed(state, Microsoft.Xna.Framework.Input.Keys.R, this.oldState.$clone())) {
                    this.RestartLevel();
                    this.oldState = state.$clone();
                    Microsoft.Xna.Framework.Game.prototype.Update.call(this, gameTime);
                    return;
                }

                var newPos = this.manPos.$clone();
                if (MazeGuy.Extension.WasKeyJustPressed(state, Microsoft.Xna.Framework.Input.Keys.Left, this.oldState.$clone()) || MazeGuy.Extension.WasKeyJustPressed(state, Microsoft.Xna.Framework.Input.Keys.A, this.oldState.$clone())) {
                    newPos.X--;
                }
                if (MazeGuy.Extension.WasKeyJustPressed(state, Microsoft.Xna.Framework.Input.Keys.Right, this.oldState.$clone()) || MazeGuy.Extension.WasKeyJustPressed(state, Microsoft.Xna.Framework.Input.Keys.D, this.oldState.$clone())) {
                    newPos.X++;
                }
                if (MazeGuy.Extension.WasKeyJustPressed(state, Microsoft.Xna.Framework.Input.Keys.Up, this.oldState.$clone()) || MazeGuy.Extension.WasKeyJustPressed(state, Microsoft.Xna.Framework.Input.Keys.W, this.oldState.$clone())) {
                    newPos.Y--;
                }
                if (MazeGuy.Extension.WasKeyJustPressed(state, Microsoft.Xna.Framework.Input.Keys.Down, this.oldState.$clone()) || MazeGuy.Extension.WasKeyJustPressed(state, Microsoft.Xna.Framework.Input.Keys.S, this.oldState.$clone())) {
                    newPos.Y++;
                }

                try {
                    var newTile = this.maze.get([Bridge.Int.clip32(newPos.X), Bridge.Int.clip32(newPos.Y)]);
                    if (this.on_a_step) {
                        this.ProcessMovementOnStep(newPos.$clone(), newTile);
                    } else if (this.on_the_wall) {
                        this.ProcessMovementOnWall(newPos.$clone(), newTile);
                    } else {
                        this.ProcessMovementNormal(newPos.$clone(), newTile);
                    }
                    this.oldState = state.$clone();
                } catch ($e2) {
                    $e2 = System.Exception.create($e2);
                }
                Microsoft.Xna.Framework.Game.prototype.Update.call(this, gameTime);
            },
            ProcessMovementOnStep: function (newPos, newTile) {
                if (newTile === MazeGuy.TileType.Ice) {
                    this.SlideOnIce(newPos.$clone());
                } else if (newTile === MazeGuy.TileType.Exit) {
                    this.won = true;
                } else if (newTile === MazeGuy.TileType.Step) {
                    this.on_a_step = true;
                    this.manPos = newPos.$clone();
                } else if (newTile === MazeGuy.TileType.KillingFloor || newTile === MazeGuy.TileType.Pit || newTile === MazeGuy.TileType.KillingStep) {
                    this.Die();
                } else if (newTile === MazeGuy.TileType.UnclimableWall) {
                    // Can't move there
                } else if (newTile !== MazeGuy.TileType.Wall) {
                    this.manPos = newPos.$clone();
                }
            },
            ProcessMovementOnWall: function (newPos, newTile) {
                if (newTile === MazeGuy.TileType.Ice) {
                    this.SlideOnIce(newPos.$clone());
                } else if (newTile === MazeGuy.TileType.Exit) {
                    this.won = true;
                } else if (newTile === MazeGuy.TileType.UnclimableWall) {
                    // Can't move there
                } else if (newTile === MazeGuy.TileType.Step) {
                    this.on_a_step = true;
                    this.on_the_wall = !this.on_the_wall;
                    this.SlideOnIce(newPos.$clone());
                } else if (newTile === MazeGuy.TileType.KillingFloor || newTile === MazeGuy.TileType.KillingStep || newTile === MazeGuy.TileType.Pit || newTile === MazeGuy.TileType.Floor) {
                    this.Die();
                } else {
                    this.manPos = newPos.$clone();
                }
            },
            ProcessMovementNormal: function (newPos, newTile) {
                if (newTile === MazeGuy.TileType.Ice) {
                    this.SlideOnIce(newPos.$clone());
                } else if (newTile === MazeGuy.TileType.Exit) {
                    this.won = true;
                } else if (newTile === MazeGuy.TileType.UnclimableWall || newTile === MazeGuy.TileType.Wall) {
                    // Can't move there
                } else if (newTile === MazeGuy.TileType.Step) {
                    this.on_a_step = true;
                    this.on_the_wall = !this.on_the_wall;
                    this.SlideOnIce(newPos.$clone());
                } else if (newTile === MazeGuy.TileType.KillingFloor || newTile === MazeGuy.TileType.Pit || newTile === MazeGuy.TileType.KillingStep) {
                    this.Die();
                } else {
                    this.manPos = newPos.$clone();
                }
            },
            SlideOnIce: function (newPos) {
                if (this.manPos.X > newPos.X) {
                    this.manPos = new Microsoft.Xna.Framework.Vector2.$ctor2(newPos.X - 1, newPos.Y);
                } else {
                    if (this.manPos.X < newPos.X) {
                        this.manPos = new Microsoft.Xna.Framework.Vector2.$ctor2(newPos.X + 1, newPos.Y);
                    } else {
                        if (this.manPos.Y > newPos.Y) {
                            this.manPos = new Microsoft.Xna.Framework.Vector2.$ctor2(newPos.X, newPos.Y - 1);
                        } else {
                            if (this.manPos.Y < newPos.Y) {
                                this.manPos = new Microsoft.Xna.Framework.Vector2.$ctor2(newPos.X, newPos.Y + 1);
                            }
                        }
                    }
                }

                this.ClampPosition();
            },
            ClampPosition: function () {
                if (this.manPos.X >= this.WIDTH) {
                    this.manPos.X = (this.WIDTH - 1) | 0;
                } else {
                    if (this.manPos.X < 0) {
                        this.manPos.X = 0;
                    }
                }
                if (this.manPos.Y >= this.HEIGHT) {
                    this.manPos.Y = (this.HEIGHT - 1) | 0;
                } else {
                    if (this.manPos.Y < 0) {
                        this.manPos.Y = 0;
                    }
                }
            },
            Draw: function (gameTime) {
                this.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black.$clone());
                this.spriteBatch.Begin();

                var manScreenPos = new Microsoft.Xna.Framework.Vector2.$ctor2(((Bridge.Int.div(this.GraphicsDevice.Viewport.Width, 2)) | 0), ((Bridge.Int.div(this.GraphicsDevice.Viewport.Height, 2)) | 0));

                if (this.dead) {
                    // Draw maze faded
                    this.DrawMaze(manScreenPos.$clone(), 0.3);
                    this.spriteBatch.DrawString(this.font, "You Died!", new Microsoft.Xna.Framework.Vector2.$ctor2(manScreenPos.X - 50, manScreenPos.Y - 50), Microsoft.Xna.Framework.Color.Red.$clone());
                    this.spriteBatch.DrawString(this.font, "Press SPACE to restart level", new Microsoft.Xna.Framework.Vector2.$ctor2(manScreenPos.X - 130, manScreenPos.Y), Microsoft.Xna.Framework.Color.White.$clone());
                    this.spriteBatch.DrawString(this.font, "Press R to restart game", new Microsoft.Xna.Framework.Vector2.$ctor2(manScreenPos.X - 110, manScreenPos.Y + 30), Microsoft.Xna.Framework.Color.Gray.$clone());
                } else if (this.won) {
                    // Auto-advance to next level (like original)
                    this.level = (this.level + 1) | 0;
                    if (MazeGuy.Game1.Levels.containsKey(this.level)) {
                        this.LoadLevel(this.level);
                    } else {
                        // Game complete - no more levels
                        this.spriteBatch.DrawString(this.font, "Congratulations! You Won!", new Microsoft.Xna.Framework.Vector2.$ctor2(manScreenPos.X - 120, manScreenPos.Y - 30), Microsoft.Xna.Framework.Color.Gold.$clone());
                        this.spriteBatch.DrawString(this.font, "Press R to play again", new Microsoft.Xna.Framework.Vector2.$ctor2(manScreenPos.X - 100, manScreenPos.Y + 10), Microsoft.Xna.Framework.Color.White.$clone());
                    }
                } else if (!this.start) {
                    // Normal gameplay
                    if ((Microsoft.Xna.Framework.Vector2.op_Subtraction(this.manPos.$clone(), this.manAnimationPos.$clone())).Length() < this.stepSize) {
                        this.manAnimationPos = this.manPos.$clone();
                        this.stepSize = 0.1;
                    } else {
                        var step = Microsoft.Xna.Framework.Vector2.op_Subtraction(this.manPos.$clone(), this.manAnimationPos.$clone());
                        step.Normalize();
                        this.manAnimationPos = Microsoft.Xna.Framework.Vector2.op_Addition(this.manAnimationPos.$clone(), Microsoft.Xna.Framework.Vector2.op_Multiply$1(step.$clone(), this.stepSize));
                    }

                    this.DrawMaze(manScreenPos.$clone(), 1.0);

                    // Draw player
                    this.spriteBatch.Draw(this.manTexture, new Microsoft.Xna.Framework.Rectangle.$ctor2(Bridge.Int.clip32(manScreenPos.X), Bridge.Int.clip32(manScreenPos.Y), Bridge.Int.mul(this.manTexture.Width, 2), Bridge.Int.mul(this.manTexture.Height, 2)), Microsoft.Xna.Framework.Color.White.$clone());

                    // Draw level indicator
                    this.spriteBatch.DrawString(this.font, "Level " + this.level, new Microsoft.Xna.Framework.Vector2.$ctor2(10, 10), Microsoft.Xna.Framework.Color.White.$clone());
                    this.spriteBatch.DrawString(this.font, "Arrow Keys or WASD to move", new Microsoft.Xna.Framework.Vector2.$ctor2(10, 30), Microsoft.Xna.Framework.Color.Gray.$clone());
                    this.spriteBatch.DrawString(this.font, "R to restart", new Microsoft.Xna.Framework.Vector2.$ctor2(10, 50), Microsoft.Xna.Framework.Color.Gray.$clone());
                } else {
                    this.spriteBatch.DrawString(this.font, "No start position in level!", new Microsoft.Xna.Framework.Vector2.$ctor2(300, 300), Microsoft.Xna.Framework.Color.Red.$clone());
                }

                this.spriteBatch.End();
                Microsoft.Xna.Framework.Game.prototype.Draw.call(this, gameTime);
            },
            DrawMaze: function (manScreenPos, alpha) {
                // Only draw visible tiles (culling)
                var tileSize = 32; // texture.Width * 2
                var viewWidth = this.GraphicsDevice.Viewport.Width;
                var viewHeight = this.GraphicsDevice.Viewport.Height;

                var startX = Math.max(0, ((((Bridge.Int.clip32(this.manAnimationPos.X) - (((Bridge.Int.div(((Bridge.Int.div(viewWidth, tileSize)) | 0), 2)) | 0))) | 0) - 1) | 0));
                var endX = Math.min(this.WIDTH, ((((Bridge.Int.clip32(this.manAnimationPos.X) + (((Bridge.Int.div(((Bridge.Int.div(viewWidth, tileSize)) | 0), 2)) | 0))) | 0) + 2) | 0));
                var startY = Math.max(0, ((((Bridge.Int.clip32(this.manAnimationPos.Y) - (((Bridge.Int.div(((Bridge.Int.div(viewHeight, tileSize)) | 0), 2)) | 0))) | 0) - 1) | 0));
                var endY = Math.min(this.HEIGHT, ((((Bridge.Int.clip32(this.manAnimationPos.Y) + (((Bridge.Int.div(((Bridge.Int.div(viewHeight, tileSize)) | 0), 2)) | 0))) | 0) + 2) | 0));

                for (var x = startX; x < endX; x = (x + 1) | 0) {
                    for (var y = startY; y < endY; y = (y + 1) | 0) {
                        var texture = this.textures[System.Array.index(this.maze.get([x, y]), this.textures)];
                        var color = Microsoft.Xna.Framework.Color.White.$clone();
                        if (this.maze.get([x, y]) === MazeGuy.TileType.Exit) {
                            color = Microsoft.Xna.Framework.Color.Gold.$clone();
                        }
                        if (alpha < 1.0) {
                            color = new Microsoft.Xna.Framework.Color.$ctor5(color.R, color.G, color.B, Bridge.Int.clipu8(255 * alpha));
                        }

                        this.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle.$ctor2(Bridge.Int.clip32((x - this.manAnimationPos.X) * texture.Width * 2 + manScreenPos.X), Bridge.Int.clip32((y - this.manAnimationPos.Y) * texture.Height * 2 + manScreenPos.Y), Bridge.Int.mul(texture.Width, 2), Bridge.Int.mul(texture.Height, 2)), color.$clone());
                    }
                }
            }
        }
    });

    Bridge.ns("MazeGuy.Game1", $asm.$);

    Bridge.apply($asm.$.MazeGuy.Game1, {
        f1: function (_o1) {
            _o1.add(2, System.Array.init(["s      #   #   ####   #    ######    ##      #    #  #", "#####  # # # # #### # #  #      #         #  #    #  #", "       # # # #      #         #     ##    #  #  # #  #", "  ###### # # ############   #    #        #  #  # #  #", "         # #    #             #           #  #  # #  #", "########## ###  #   #  ###  ##### ##      #  #  # #  #", "         #      #  #  #            #####  #  #  #    #", " ################  # #                  ###########  #", "                     #    ###############            #", "#############  #######                     ###########", "                        ###########                  #", "   ############################       ############   #                     ", "                            ###############          #", "####################          #      #        ########", "                              #  #   #               #", "   #########################  #  #   #############   #", "                                 #                   #", "########  #############          #####################", "                #   # #  ####                        #", "#############   #   # #  #     ###################   #", "                    # #  #  #               #    #   #", "   ################## #  #  ##############  #  # #   #", "                      #  #  #               #  # #   #", "#######################  #  ##########  #####  # #####", "                                               #     x", "######################################################"], System.String));
            _o1.add(3, System.Array.init(["siiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiidddddddddddddddddddddddddddddddddddddddoooooooooooooooooo33333,,,,,,;[['['nbbvdfdgffdddddddddddddddmmmmm,m,mmmmmnbbffgtyutrr", "iiiiiiiiiiiiiiiiiiiiigggggggggggggggggggggggggggeqqqttttoiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiim"], System.String));
            _o1.add(4, System.Array.init(["siiiiiiiiiiiiiiiiiiiiiooooooooooooooooooooooooooooooooooooooooooooooooooxooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo", "                                                                        do", "                                                                        do", "oooooooooooddddddddddiiiiiiiiiiiiiiiiiiiiiiididddddddddoooo             do", "oooooooooooooooiooooooooooooooooooooooooiooooooodoooooooooo             do", "oooooooooooodddoooooooooooooooooooooooodiddddddddddddddddoxd            do                        o", "ddddddddddddddddddddddddddiiiiddddddddddiiiioiiiiioiiiiidddddddddddddddddo ", "dddddddddiiiddddddddddddd_iiiioooooodooooodiiiddoiiiiioiiddddddddddddddddo   ", "ooooooooxoooooxooooooooooooooooxooooooooooooxooooooooooooooooooooooooooooo"], System.String));
            _o1.add(5, System.Array.init(["eeeeexniiiiiiiiiiisssssssfsdrdddddettreeeeeewqqqsssseiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiit", "", "", "", "", "", "gfdsesfrtefiieeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeweee5767ertyer65r4565xxcdvgbhmkjkjukiiiikiuiytytyuyujuhjmnh,jnjhfjkfhvjkbfvjk"], System.String));
            return _o1;
        },
        f2: function (line) {
            return line.length;
        }
    });

    Bridge.define("MazeGuy.Program", {
        main: function Main () {
            var game = new MazeGuy.Game1();
            try {
                game.Run();
            }
            finally {
                if (Bridge.hasValue(game)) {
                    game.System$IDisposable$Dispose();
                }
            }
        }
    });

    Bridge.define("MazeGuy.TileType", {
        $kind: "enum",
        statics: {
            fields: {
                Floor: 0,
                Wall: 1,
                Exit: 2,
                Ice: 3,
                Step: 4,
                KillingFloor: 5,
                Pit: 6,
                KillingStep: 7,
                UnclimableWall: 8,
                COUNT: 9
            }
        }
    });
});
