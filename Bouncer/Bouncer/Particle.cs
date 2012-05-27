/* Written by Paul Demchuk
 * pademchuk@gmail.com
 * April 2012
 */

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


namespace BubbleBasket {
    /// <summary>
    /// This class represents a particle.
    /// </summary>
    public class Particle : Microsoft.Xna.Framework.GameComponent {

        public Vector2 Velocity;//x and y velocity of particle
        public Vector2 Acceleration;//x and y acceleration of a particle
        public Vector2 Position;//x and y position of the particle
        private int TTL { get; set; }//Determines when a vector will fade away and die
        private float dt { get; set; }//how fast particle velocity/position changes
        public Boolean OneBounce = false;//prevents a particle from getting stuck on the bottom
        public Boolean Remove = false;//flag for removal
        public Texture2D CircleTexture;

        /// <summary>
        /// craetes a new particle instance
        /// </summary>
        /// <param name="game">pass 'this'</param>
        /// <param name="v">initial velocity</param>
        /// <param name="a">intitial acceleration</param>
        /// <param name="h">initial position</param>
        /// <param name="t">initital TTL</param>
        /// <param name="d">intitial dt</param>
        public Particle(Game game, Vector2 v, Vector2 a, Vector2 h, int t, float d)
            : base(game) {

            Velocity = v;
            Acceleration = a;
            Position = h;
            TTL = t;
            dt = d; 
            Initialize();

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize() {
            // TODO: Add your initialization code here

            //LOAD A RANDOM COLOR'D CIRCLE TEXTURE!!!
            int numCircles = 6;
            string circleCol = "circle_red";
            switch ((new Random()).Next(numCircles)) {

                case 0: circleCol = "circle_blue";
                    break;
                case 1: circleCol = "circle_red";
                    break;
                case 2: circleCol = "circle_orange";
                    break;
                case 3: circleCol = "circle_green";
                    break;
                default: circleCol = "circle_purple";
                    break;


            }

            CircleTexture = Game.Content.Load<Texture2D>(circleCol);
            base.Initialize();
        }
        
        /// <summary>
        /// checks if the bubble is in the basket or not!
        /// </summary>
        /// <param name="basket">the basket. Passed just to get the xy position of the basket</param>
        /// <returns>true if the ball is in the basket</returns>
        public bool checkBasketCatch(Basket basket) {
            //compare the position of the particle with the position of the basket
            if ((Position.X + CircleTexture.Width / 2 > basket.Position.X && //check if it is in the right bound
                Position.X + CircleTexture.Width / 2 < basket.Position.X + basket.BasketTexture.Width) &&//check if it is in the left bound
                Position.Y + CircleTexture.Height > basket.Position.Y) {//make sure it is below the top of the basket

                Remove = true;//if it is, remove it!
                return true;
            }

            return false;
        }

        /// <summary>
        /// Where the magic happens. Particle position and velocity get updated here!!!!
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime) {
            // TODO: Add your update code here
            TTL--;//always decrement the ttl
            if (TTL <= 0) {
                Remove = true;//once it is 0, remove it
            }
            //update position. This is jsut integration
            Position.X = Position.X + (Velocity.X) * dt + Acceleration.X * (dt * dt);
            Position.Y = Position.Y + (Velocity.Y) * dt + Acceleration.Y * (dt * dt);
            //update the velocity based on current velocity/acceleration
            Velocity.X = Velocity.X + Acceleration.X * dt;
            Velocity.Y = Velocity.Y + Acceleration.Y * dt;
            //add collision detection for the edge of the screen
            if (Position.X < -12 || Position.X > 460) {
                Velocity.X *= (-1);
            }
            //add collision detection for the bottom of the screen
            //one bounce starts off as false, then set to true when this loop is entered.
            //this ensures that the particle doesn't get 'stuck' on the bottom!
            if(Position.Y > 775  && !OneBounce){
                Velocity.Y = Velocity.Y*((float)(-1.0))/(float)1.5;
                OneBounce = true;
            //reset onebounce
            } else if (OneBounce) {
                OneBounce = false;
            }

            base.Update(gameTime);
        }
    }
}
