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
    public class ControlsView : GameStateView
    {
        private SpriteFont m_font;

        public const string CONTROLFILE = "Controls";
        private string userMessage = "While hovering over the control you wish to change, press ENTER.";

        private string messageSell;
        private string messageUpgrade;
        private string messageNextRound;

        KeyboardState currState;
        KeyboardState prevState;

        private bool listening = false;
        private bool loadPrompt = false;

        private Keys[] controlsArray = new Keys[] { Keys.S, Keys.U, Keys.G };

        private KeyboardInput m_inputKeyboard = new KeyboardInput();
        private enum MenuState
        {
            Sell = 0,
            Upgrade = 1,
            NextRound = 2
        }

        private MenuState m_currentSelection = MenuState.Sell;
        private bool m_waitForKeyRelease;

        public override void loadContent(ContentManager contentManager)
        {
            
            File.WriteAllText(CONTROLFILE, JsonSerializer.Serialize(controlsArray));

            m_font = contentManager.Load<SpriteFont>("Fonts/MenuFont");
            messageSell = "SELL TURRET: " + controlsArray[0].ToString();
            messageUpgrade = "UPGRADE TURRET: " + controlsArray[1].ToString();
            messageNextRound = "NEXT ROUND: " + controlsArray[2].ToString();

            m_waitForKeyRelease = true;

        }

        public override GameStateEnum processInput(GameTime gameTime)
        {

            prevState = currState;
            currState = Keyboard.GetState();




            if (!m_waitForKeyRelease)
            {
                if (listening)
                {
                    
                    Keys[] allKeys = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToArray();
                    foreach (Keys key in allKeys)
                    {
                        if (currState.IsKeyDown(key))
                        {
                            controlsArray[(int)m_currentSelection] = key;
                            File.WriteAllText(CONTROLFILE, JsonSerializer.Serialize(controlsArray));
                            listening = false;
                            loadPrompt = false;
                            break;
                        }
                    }
                    
                }
                else
                {
                    // Arrow keys to navigate the menu
                    if (currState.IsKeyDown(Keys.Down) && (int) m_currentSelection <= 1)
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

            messageSell = "SELL TURRET: " + controlsArray[0].ToString();
            messageUpgrade = "UPGRADE TURRET: " + controlsArray[1].ToString();
            messageNextRound = "NEXT LEVEL: " + controlsArray[2].ToString();

            if (loadPrompt)
            {
                userMessage = "Press the Key you wish to assign.";
            }
            else
            {
                userMessage = "While hovering over the control you wish to change, press ENTER.";
            }
            float top = drawMenuItem(m_font, userMessage, 20, Color.White);

            float bottom = drawMenuItem(m_currentSelection == MenuState.Sell ? m_font : m_font, messageSell, 250, m_currentSelection == MenuState.Sell ? Color.Yellow : Color.Blue);
            bottom = drawMenuItem(m_currentSelection == MenuState.Upgrade ? m_font : m_font, messageUpgrade, bottom, m_currentSelection == MenuState.Upgrade ? Color.Yellow : Color.Blue);
            drawMenuItem(m_currentSelection == MenuState.NextRound ? m_font : m_font, messageNextRound, bottom, m_currentSelection == MenuState.NextRound ? Color.Yellow : Color.Blue);

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
