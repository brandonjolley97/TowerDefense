using TowerDefense.Input;
using TowerDefense.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TowerDefense
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager m_graphics;

        double runTime;

        private IGameState m_prevState;

        /////////////////// Menu Variables //////////////////////
        
        private IGameState m_currentState;
        private GameStateEnum m_nextStateEnum = GameStateEnum.MainMenu;
        private Dictionary<GameStateEnum, IGameState> m_states;

        ////////////////////////////////////////////////////////

        private KeyboardInput m_inputKeyboard = new KeyboardInput();

        public Game1()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ////////////////////////// Initializing a Sprite ///////////////////////////////


            ///////////////////////////////////////////////////////////////////////////////

            ////////////////////////// Initializing the Window /////////////////////////////
           
            m_graphics.PreferredBackBufferWidth = 1920;
            m_graphics.PreferredBackBufferHeight = 1080;
            //m_graphics.PreferredBackBufferWidth = 2560;
            //m_graphics.PreferredBackBufferHeight = 1440;
            
            m_graphics.ApplyChanges();

            ///////////////////////////////////////////////////////////////////////////////

            ///////////////////////Initializing Menu States ///////////////////////////////
            
            m_states = new Dictionary<GameStateEnum, IGameState>();
            m_states.Add(GameStateEnum.MainMenu, new MainMenuView());
            m_states.Add(GameStateEnum.GamePlay, new GamePlayView());
            m_states.Add(GameStateEnum.HighScores, new HighScoresView());
            m_states.Add(GameStateEnum.Credits, new CreditsView());
            m_states.Add(GameStateEnum.Controls, new ControlsView());

            // We are starting with the main menu
            m_currentState = m_states[GameStateEnum.MainMenu];

            ///////////////////////////////////////////////////////////////////////////////

            base.Initialize();

        }

        protected override void LoadContent()
        {

            foreach (var item in m_states)
            {
                item.Value.initialize(this.GraphicsDevice, m_graphics);
                item.Value.loadContent(this.Content);
            }


        }

        protected override void Update(GameTime gameTime)
        {
            runTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            
            m_nextStateEnum = m_currentState.processInput(gameTime);

            // Special case for exiting the game
            if (m_nextStateEnum == GameStateEnum.Exit)
            {
                Exit();
            }

            m_currentState.update(gameTime);

            base.Update(gameTime);


        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MediumSlateBlue);

            m_prevState = m_currentState;

            m_currentState.render(gameTime);
            
            m_currentState = m_states[m_nextStateEnum];

            if(m_prevState != m_currentState)
            {
                m_currentState.loadContent(this.Content);
            }


            base.Draw(gameTime);

        }
    }

}
