/* Written by Paul Demchuk
 * pademchuk@gmail.com
 * April 2012
 * 
 * Feel free to use any part of this code no problems :)
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using Microsoft.Advertising.Mobile.Xna;

namespace BubbleBasket {
    /// <summary>
    /// This is the main class for my game. Where all the magic happens
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // These 3 boolean values determine which screen the user is currently on. If they are all false, the user
        // is currently in game
        bool mIsTitleScreenShown = false;
        bool mIsHiScoreScreenShown = false;
        bool mIsInstructionsScreenShown = false;
        List<Source> Sources;//list of all Sources. each source has 1 particle (for now)
        KeyboardState OldState;//used to keep track if a new key was hit or not
        SpriteFont SmallFont;
        SpriteFont LargeFont;
        int PointCounter = 0;
        Basket basket;
        TimeSpan TimeLeft;//keeps track of how much time is left in play
        TimeSpan StartTime;//when the user hits start, this variable grabs the current gameTime
        //the various buttons for Home/hi score screens
        Button StartButton;
        Button MainMenuButton;
        Button ResetButton;
        Button InstructionsButton;
        string hiScoreFileName = "hiScore";//the location where hiscores will be stored.
        int highScore = 0;

        /// <summary>
        /// saves the new high score
        /// </summary>
        /// <param name="score">the score to save</param>
        private void saveHiScores(int score){
            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();

            // open isolated storage, and write the savefile.  
            IsolatedStorageFileStream fs = null;
            using (fs = savegameStorage.CreateFile(hiScoreFileName)) {
                if (fs != null) {
                    // just overwrite the existing info for this example. 

                    byte[] bytes = System.BitConverter.GetBytes(score);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }

        /// <summary>
        /// Load the old high score
        /// </summary>
        /// <returns>the old high score</returns>
        private int loadHiScore() {
            int hiScore = 0;
            using (IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication()){
                if (savegameStorage.FileExists(hiScoreFileName)) {
                    using (IsolatedStorageFileStream fs = savegameStorage.OpenFile(hiScoreFileName, System.IO.FileMode.Open)) {
                        if (fs != null) {
                            // Reload the saved high-score data.  
                            byte[] saveBytes = new byte[4];
                            int count = fs.Read(saveBytes, 0, 4);
                            if (count > 0) {
                                hiScore = System.BitConverter.ToInt32(saveBytes, 0);
                            }
                        }
                    }
                }
            }
            return hiScore;
        }

        /// <summary>
        /// Create a new instance of Game1
        /// </summary>
        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            Sources = new List<Source>();
            //set the display to all orientations, then set the size of the screen, then FORCE potrait mode.
            //this makes it full screen portrait. There is most definitely a better way, but I do not know it.
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            StartTime = new TimeSpan(0, 0, 0);//initialize start time to something...it will change for sure

            //Add some ads to the app!
            //CURRENTLY THERE ARE NO ADS

            /*
            AdManager ads = new AdManager(this);
            AdGameComponent.Initialize(this, ads.ApplicationId);
            Components.Add(AdGameComponent.Current);
            ads.CreateAd();
            */

            //get the old highscore
            highScore = loadHiScore();
            //create all the buttons we need. Chances are the Y values of these may end up changing
            //pass in the image representing the text the button should display
            StartButton = new Button(this,"play_game", 300);
            MainMenuButton = new Button(this, "home", 600);
            ResetButton = new Button(this, "Reset", 475);
            InstructionsButton = new Button(this, "Instructions", 400);

            TimeLeft = TimeSpan.FromSeconds(30);//as of now games will start off lasting 30 seconds

            mIsTitleScreenShown = true;//start off with titlescreen shown

            basket = new Basket(this);
            basket.Initialize();

            //setup the fonts
            SmallFont = Content.Load<SpriteFont>("InGameFont");
            LargeFont = Content.Load<SpriteFont>("HiScoreFont");

            graphics.ApplyChanges();
            OldState = Keyboard.GetState();//get initial state of keyboard
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// update the Position of particles, check button presses.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            // When the user hits the back button, go back to the title screen
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
                mIsTitleScreenShown = true;
                mIsHiScoreScreenShown = false;
                mIsInstructionsScreenShown = false;
                Sources.Clear();
                TimeLeft = TimeSpan.FromSeconds(30);//as of now games will start off lasting 30 seconds
            }
            //if we are in game, check to make sure there is still time left.
            if (!mIsTitleScreenShown && !mIsHiScoreScreenShown && !mIsInstructionsScreenShown) {
                //make sure there is time left
                if (TimeLeft.Subtract(gameTime.TotalGameTime.Subtract(StartTime)) <= new TimeSpan(0, 0, 0)) {
                    //once game ends, we want to show the high score screen
                    mIsTitleScreenShown = false;
                    mIsHiScoreScreenShown = true;
                    mIsInstructionsScreenShown = false;
                    //check if the new score is better than the old high score
                    if (highScore < PointCounter) {
                        highScore = PointCounter;
                        saveHiScores(highScore);
                    }
                }
            }

            //get the touches on the screen
            TouchCollection touchCollection = TouchPanel.GetState();
            //loop through each touch on the screen
            foreach (TouchLocation tl in touchCollection) {
                if (tl.State == TouchLocationState.Pressed) {
                    //if we are on the title screen, check if the start/instruction buttons are pressed
                    if(mIsTitleScreenShown){
                        if (StartButton.checkClick(tl.Position)) {//start button pressed
                            //initialize a new game
                            PointCounter = 0;
                            StartTime = gameTime.TotalGameTime;
                            mIsTitleScreenShown = false;
                            mIsHiScoreScreenShown = false;
                            mIsInstructionsScreenShown = false;

                        } else if (InstructionsButton.checkClick(tl.Position)) {//instructions button hit
                            //show instructions screen
                            mIsInstructionsScreenShown = true;
                            mIsHiScoreScreenShown = false;
                            mIsTitleScreenShown = false;
                        }
                    //if we are on the high score screen
                    }else if(mIsHiScoreScreenShown){
                        
                        if (ResetButton.checkClick(tl.Position)) {//reset high score button pressed
                            //save a new high score of 0, and set point counter accordinly
                            saveHiScores(0);
                            highScore = 0;
                            PointCounter = 0;
                        } else if (MainMenuButton.checkClick(tl.Position)) {// 'go Home' button pressed
                            //go to main menu, clear the particles, reset timer
                            mIsTitleScreenShown = true;
                            mIsHiScoreScreenShown = false;
                            mIsInstructionsScreenShown = false;

                            Sources.Clear();
                            TimeLeft = TimeSpan.FromSeconds(30);//as of now games will start off lasting 30 seconds

                        }
                    }else if(mIsInstructionsScreenShown){//if we are on the instruction screen, if the user taps anywhere, instruction scren is cleared
                        mIsTitleScreenShown = true;
                        mIsInstructionsScreenShown = false;
                        mIsHiScoreScreenShown = false;
                    }else if(!mIsTitleScreenShown && !mIsHiScoreScreenShown && !mIsInstructionsScreenShown) {//else we are IN GAME
                        //make sure we are above the small buffer on the bottom of the screen (~10% of screen)
                        if (tl.Position.Y < GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1.1) {
                            //allow a max of 40 particles on screen
                            if (Sources.Count < 40) {
                                //if there are less than 40 particles, create a new particle where the user tapped
                                Source newSource = new Source(this, new Vector2(tl.Position.X, tl.Position.Y), 1);
                                Sources.Add(newSource);
                                newSource.Initialize();
                            }
                        }
                    }
                    //if the touch was a move, move the basket
                } else if (tl.State == TouchLocationState.Moved ) {
                    if (!mIsTitleScreenShown && !mIsHiScoreScreenShown) {
                        basket.Position.X = tl.Position.X;
                    }
                }
            }
            //if we are ingame, update the Position of sources
            if (!mIsHiScoreScreenShown && !mIsTitleScreenShown && !mIsInstructionsScreenShown) {
                updateSources(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Goes through all the Sources and updates particles.
        /// Then it checks if any Sources should be removed or not.
        /// </summary>
        /// <param name="gameTime"></param>
        private void updateSources(GameTime gameTime) {
            //loop through all sources
            foreach (Source source in Sources) {
                if (source != null && source.particles != null) {//error checking
                    //loop through all particles of a source, for now it is just one per source!!
                    foreach (Particle particle in source.particles) {
                        particle.Update(gameTime);//update Position and Velocity of particle
                        //check if the particle landed in the basket
                        if (particle.checkBasketCatch(basket)) {//if there was a catch
                            PointCounter++;//add a point
                            TimeLeft = TimeLeft.Add(new TimeSpan(0, 0, 0, 0, 200));//add a fraction of a second to the time
                        }
                    }
                    source.Update(gameTime);

                }
            }

            for (int i = 0; i < Sources.Count; i++) {
                if (Sources[i].particles.Count == 0) {
                    Sources.Remove(Sources[i]);
                    i--;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //check which screen we are on and draw the appropriate screen
            if (mIsTitleScreenShown) {
                drawTitleScreen();
            } else if (mIsHiScoreScreenShown) {
                drawHiScoreScreen();
            }else if(mIsInstructionsScreenShown){
                drawInstructionScreen();
            } else if (!mIsTitleScreenShown && !mIsHiScoreScreenShown && !mIsInstructionsScreenShown) {
                drawGame(gameTime);
            }


            base.Draw(gameTime);
        }

        /// <summary>
        /// want to draw in game
        /// </summary>
        /// <param name="gameTime"></param>
        private void drawGame(GameTime gameTime) {
            spriteBatch.Begin();//anything in here will be drawn!!
            
            //draw each particle
            foreach (Source source in Sources) {
                foreach (Particle particle in source.particles) {
                    if (particle != null) {
                        spriteBatch.Draw(particle.CircleTexture, particle.Position, null, Color.White, 0, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
                    }
                }
            }
            //draw the counter and time left and basket
            spriteBatch.DrawString(SmallFont, "Count: " + PointCounter, new Vector2(20, 100), Color.Black);
            spriteBatch.DrawString(SmallFont, "\nTime Remaining: " + TimeLeft.Subtract(gameTime.TotalGameTime.Subtract(StartTime)), new Vector2(20, 100), Color.Black);
            spriteBatch.Draw(basket.BasketTexture, basket.Position, Color.White);

            spriteBatch.End();
        }

        /// <summary>
        /// want to draw the title screen
        /// </summary>
        private void drawTitleScreen() {
            spriteBatch.Begin();
            //add title
            Texture2D welcomeText = this.Content.Load<Texture2D>("BounceWelcome");
            spriteBatch.Draw(welcomeText, new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - (welcomeText.Width/2), 
                100), Color.White);
            //add the other buttons :)
            InstructionsButton.Position.Y = StartButton.Position.Y + 100;

            spriteBatch.Draw(StartButton.ButtonTexture, StartButton.Position, Color.Black);
            spriteBatch.Draw(InstructionsButton.ButtonTexture, InstructionsButton.Position, Color.Black);


            spriteBatch.End();
        }

        /// <summary>
        /// want to draw the hi score screen
        /// </summary>
        private void drawHiScoreScreen() {
            spriteBatch.Begin();
            //draw buttons/title
            Texture2D welcomeText = this.Content.Load<Texture2D>("BounceWelcome");
            spriteBatch.Draw(welcomeText, new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - (welcomeText.Width / 2),
                100), Color.White);

            spriteBatch.DrawString(LargeFont, "High Score: " + highScore, 
                new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/2 - 
                    LargeFont.MeasureString("High Score: " + highScore).X/2, 400), Color.Black);

            spriteBatch.Draw(ResetButton.ButtonTexture, new Vector2(ResetButton.Position.X, ResetButton.Position.Y), Color.White);

            spriteBatch.Draw(MainMenuButton.ButtonTexture, new Vector2(MainMenuButton.Position.X, MainMenuButton.Position.Y), Color.White);

            spriteBatch.End();
        }

        /// <summary>
        /// want to draw the instruction screen
        /// </summary>
        private void drawInstructionScreen() {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;//get the width of the screen
            spriteBatch.Begin();
            
            spriteBatch.DrawString(LargeFont, "How to Play",new Vector2(screenWidth/2 - LargeFont.MeasureString("How to Play").X/2, 100), Color.Black);
            //the instruction string
            string instructions = "It's easy! All you have to do is tap the screen to shoot a bubble. " +
                "Then move your finger to move the basket. Catch as many bubbles as you can before the time runs out! \n\nTap Here";
            
            //inserts new lines where needed
            instructions = WrapText(LargeFont, instructions, screenWidth - 50);

            spriteBatch.DrawString(LargeFont, instructions, 
                new Vector2(screenWidth / 2 - LargeFont.MeasureString(instructions).X / 2, 200), Color.Black);

            spriteBatch.End();
        }

        /// <summary>
        /// I got the code for this function from:
        /// http://www.xnawiki.com/index.php?title=Basic_Word_Wrapping
        /// </summary>
        /// <param name="spriteFont"> the spritefont that will render the text</param>
        /// <param name="text">the text to render</param>
        /// <param name="maxLineWidth">width of the screen (minus 50)</param>
        /// <returns></returns>
        public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth) {
            string[] words = text.Split(' ');//split on spaces....BE CAREFUL NOT TO HAVE EXTRA SPACES!!!!

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0f;

            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words) {//loop through words and check the size of each. Insert new lines when size gets bigger than maxLineWidth
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth) {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                } else {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }


    }
}
