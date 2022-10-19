using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TowerDefense.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace TowerDefense.Menu
{
    public class HighScoresView : GameStateView
    {
        private SpriteFont m_font;
        private SpriteFont m_scores;

        public const string HIGHSCORES_FILE = "Highscores";
        private const string MESSAGE = "High Scores";

        private int listSize;


        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/MenuFont");
            m_scores = contentManager.Load<SpriteFont>("Fonts/MenuFont");

            if (File.Exists(HIGHSCORES_FILE))
            {
                string contents = File.ReadAllText(HIGHSCORES_FILE);
                highScores = (List<int>)JsonSerializer.Deserialize(contents, typeof(List<int>));
            }

            highScores.Sort();
            highScores.Reverse();

            listSize = highScores.Count;

        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.HighScores;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            Vector2 stringSize = m_font.MeasureString(MESSAGE);
            m_spriteBatch.DrawString(m_font, MESSAGE,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, 200), Color.Yellow);

            int count = 0;
            for (int i = 0; i < listSize; i++)
            {
                count++;
                if (count <= 5)
                {
                    string scoreMessage = (i + 1) + ": " + highScores[i] + " points";
                    Vector2 string1Size = m_font.MeasureString(scoreMessage);
                    m_spriteBatch.DrawString(m_font, scoreMessage,
                        new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, 250 + (40 * i)), Color.Yellow);
                }
            }


            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }

    }
}
