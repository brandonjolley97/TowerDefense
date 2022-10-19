using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace TowerDefense.Menu
{
    public abstract class GameStateView : IGameState
    {
        public Objects.Player m_player = new Objects.Player();
        public List<int> highScores = new List<int>();
        protected GraphicsDeviceManager m_graphics;
        protected SpriteBatch m_spriteBatch;

        public void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);

            if (File.Exists(HighScoresView.HIGHSCORES_FILE))
            {
                string contents = File.ReadAllText(HighScoresView.HIGHSCORES_FILE);
                highScores = (List<int>)JsonSerializer.Deserialize(contents, typeof(List<int>));
            }
            else
            {
                highScores.AddRange(new int[] { 0, 0, 0, 0, 0 });
            }
        }
        public abstract void loadContent(ContentManager contentManager);
        public abstract GameStateEnum processInput(GameTime gameTime);
        public abstract void render(GameTime gameTime);
        public abstract void update(GameTime gameTime);
    }
}
