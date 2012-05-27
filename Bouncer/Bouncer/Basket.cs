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
    /// Helper class. Keeps track of basket related variables
    /// </summary>
    public class Basket : Microsoft.Xna.Framework.GameComponent {
        public Vector2 Position;//Where the basket is located
        public Texture2D BasketTexture;//the texture that the basket will have
        public Basket(Game game)
            : base(game) {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize() {
            //set up the texture.
            //the basket starts in the middle of the screen
            BasketTexture = Game.Content.Load<Texture2D>("basket");
            Position = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - BasketTexture.Width/2, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 24);
            
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>/// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime) {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
