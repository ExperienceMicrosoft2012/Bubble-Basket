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
    /// Helper class to keep track of button variables
    /// </summary>
    public class Button : Microsoft.Xna.Framework.GameComponent {

        public Texture2D ButtonTexture;//texture used for the button
        public Vector2 Position;//top left corner of the button
        public int Width;
        public int Height;

        /// <summary>
        /// Creates a new button object
        /// </summary>
        /// <param name="game">pass 'this'</param>
        /// <param name="spriteName">the sprite for the button</param>
        /// <param name="yPos">where you want the button to be on the y scale</param>
        public Button(Game game, string spriteName, int yPos)
            : base(game) {
            ButtonTexture = Game.Content.Load<Texture2D>(spriteName);
            //For now all buttons will be centered in the screen horizontally
            Position = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - (ButtonTexture.Width / 2), yPos);
            //height and width come from the size of a texture
            Height = ButtonTexture.Height;
            Width = ButtonTexture.Width;
            
        }
        //checks if when the user clicks, it is inside the bounds of the button
        public Boolean checkClick(Vector2 tapPos) {
            return (tapPos.X > this.Position.X &&
                        tapPos.X < this.Position.X + this.Width &&
                        tapPos.Y > this.Position.Y &&
                        tapPos.Y < this.Position.Y + this.Height);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize() {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime) {
            // TODO: Add your update code here

            base.Update(gameTime);
        }


    }
}
