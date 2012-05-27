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
    /// This class holds the source, which holds the particles.
    /// for now, there is only one particle per source,
    /// but in the future I may have reason to add more
    /// </summary>
    public class Source : Microsoft.Xna.Framework.GameComponent {

        public Vector2 Home;//where particles get launched from
        public List<Particle> particles;//array of particles
        public int TTL = 300 + (new Random().Next(50));//create a random TTL. 
        Game game;//the game passed in
        public int numParticles;//currently just 1

        /// <summary>
        /// the constructor that creates an instance of the source class
        /// </summary>
        /// <param name="_game">pass 'this'</param>
        /// <param name="h">the initial position of the source</param>
        /// <param name="n">number of particles. just 1 for now</param>
        public Source(Game _game, Vector2 h, int n)
            : base(_game) {
            // TODO: Construct any child components here
            Home = h;
            game = _game;
            numParticles = n;
        }

        /// <summary>
        /// Initialize the source. Create the new particles
        /// </summary>
        public override void Initialize() {
            // TODO: Add your initialization code here
            particles = new List<Particle>();
            for (int i = 0; i < numParticles; i++) {
                Random rand = new Random();//create a new random instance
                float sign = (rand.Next(2) == 0) ? (-1) : 1;//get a random number that determines whether particles X acceleration + velocity is +/-
                Vector2 velocity = new Vector2((1 + rand.Next(2)*i) * sign, (-1) * (80 + 10*i + rand.Next(50)));//randomize x and y velocity
                Vector2 acc = new Vector2(((float)2.3 * sign), (float)19.0);//intialize acceleration

                //add a new particle to the array
                particles.Add(new Particle(game, velocity, acc, Home, 300+rand.Next(100), (float)0.09));
                base.Initialize();
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime) {
            //remove all particles marked as removed!!
            for (int k = 0; k < particles.Count; k++) {
                if (particles[k].Remove) {
                    particles.Remove(particles[k]);
                    k--;
                }
            }
            base.Update(gameTime);
        }
    }
}
