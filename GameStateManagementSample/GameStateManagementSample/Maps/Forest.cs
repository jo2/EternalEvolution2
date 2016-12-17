using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameStateManagementSample.Maps {
    public class Forest : EternalEvolutionMap {

        public Forest(ContentManager lContent, Player lPlayer) : base(lContent) {
            //exit = new Cell(0, 0, true, true, false);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            throw new NotImplementedException();
        }

        public override void LoadContent() {
            base.LoadContent();
        }

        public override string Update(GameTime gameTime) {
            throw new NotImplementedException();
        }
    }
}
