using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TowerDefense.Input;
using System.ComponentModel;
using System;
using System.Linq;
using System.Text.Json;
using System.IO;


namespace TowerDefense.Menu
{
    class TurretView : GameStateView
    {
        private SpriteFont m_font;

        
        private string userMessage = "Turret Shop";
        private string machineGunCost = "Machine Gun: $200";
        private string missileLauncherCost = "Missle Launcher: $1000";
        private string bombLauncherCost = "Bomb Launcher: $500";
        private string railgunCost = "Railgun: $1500";
        private string exit = "Return To Game";


        KeyboardState currState;
        KeyboardState prevState;

        private bool listening = false;
        private bool loadPrompt = false;

        private KeyboardInput m_inputKeyboard = new KeyboardInput();
        private enum MenuState
        {
            MachineGun = 0,
            Missile = 1,
            Bomb = 2,
            Railgun = 3,
            Exit = 4
        }

        private MenuState m_currentSelection = MenuState.MachineGun;
        private bool m_waitForKeyRelease;

        public override void loadContent(ContentManager contentManager)
        {

            m_font = contentManager.Load<SpriteFont>("Fonts/MenuFont");

            m_waitForKeyRelease = true;

        }

        public override GameStateEnum processInput(GameTime gameTime)
        {

            prevState = currState;
            currState = Keyboard.GetState();

            if (!m_waitForKeyRelease)
            {

                if (currState.IsKeyDown(Keys.Down) && (int)m_currentSelection <= 3)
                {
                    m_currentSelection = m_currentSelection + 1;
                    m_waitForKeyRelease = true;
                }
                if (currState.IsKeyDown(Keys.Up) && (int)m_currentSelection >= 1)
                {
                    m_currentSelection = m_currentSelection - 1;
                    m_waitForKeyRelease = true;
                }
                if (currState.IsKeyDown(Keys.Escape))
                {
                    return GameStateEnum.MainMenu;
                }

                if (currState.IsKeyDown(Keys.Enter))
                {
                    loadPrompt = true;
                    listening = true;
                    m_waitForKeyRelease = true;
                }


                // If enter is pressed, return the appropriate new state



            }
            else if (currState.IsKeyUp(Keys.Down) && currState.IsKeyUp(Keys.Up) && currState.IsKeyUp(Keys.Enter))
            {
                m_waitForKeyRelease = false;
            }

            return GameStateEnum.Controls;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            float top = drawMenuItem(m_font, userMessage, 20, Color.White);

            float bottom = drawMenuItem(m_currentSelection == MenuState.MachineGun ? m_font : m_font, machineGunCost, 250, m_currentSelection == MenuState.MachineGun ? Color.Yellow : Color.Blue);
            bottom = drawMenuItem(m_currentSelection == MenuState.Missile ? m_font : m_font, missileLauncherCost, bottom, m_currentSelection == MenuState.Bomb ? Color.Yellow : Color.Blue);
            bottom = drawMenuItem(m_currentSelection == MenuState.Bomb ? m_font : m_font, bombLauncherCost, bottom, m_currentSelection == MenuState.Missile ? Color.Yellow : Color.Blue);
            bottom = drawMenuItem(m_currentSelection == MenuState.Railgun ? m_font : m_font, railgunCost, bottom, m_currentSelection == MenuState.Railgun ? Color.Yellow : Color.Blue);
            drawMenuItem(m_currentSelection == MenuState.Exit ? m_font : m_font, exit, bottom, m_currentSelection == MenuState.Exit ? Color.Yellow : Color.Blue);

            m_spriteBatch.End();
        }

        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            m_spriteBatch.DrawString(
                font,
                text,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, y),
                color);

            return y + stringSize.Y;
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
