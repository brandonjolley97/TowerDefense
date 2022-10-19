using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TowerDefense.Input;
using TowerDefense.Objects;

namespace TowerDefense.Menu
{
    public class GamePlayView : GameStateView
    {
        private SpriteFont m_time;
        private SpriteFont m_lives;
        private SpriteFont m_score;
        private SpriteFont m_end;
        private SpriteFont m_round;
        private SpriteFont m_mouseCoords;

        private SpriteFont m_shopHeader;
        private SpriteFont m_machineGunHeader;
        private SpriteFont m_missileLauncherHeader;
        private SpriteFont m_bombLauncherHeader;
        private SpriteFont m_railgunHeader;

        private const string BADMESSAGE = "GAME OVER!";
        private const string GOODMESSAGE = "YOU WIN!";

        private const string userMessage = "Turret Shop";
        private const string machineGunCost = "Machine Gun: $200";
        private const string missileLauncherCost = "Missle Launcher: $1000";
        private const string bombLauncherCost = "Bomb Launcher: $500";
        private const string railgunCost = "Railgun: $1500";


        string[] messageBoard = new string[] { "Score: ", "Time: ", "Lives: ", "Round: " };
        string[] messageStats = new string[] { "", "", "", "" };
        int healthReference;

        private bool alreadyLoaded = false;
        private bool boardCleared = false;
        private bool nextLevel = false;

        List<Objects.AnimatedSprite> objectsToRemove = new List<Objects.AnimatedSprite>();
        
        List<Turret> fieldTurrets = new List<Turret>();
        List<Turret> shopTurrets = new List<Turret>();

        List<Enemy> allEnemies = new List<Enemy>();
        List<Projectile> allProjectiles = new List<Projectile>();

        List<AnimatedSprite> otherSprites = new List<AnimatedSprite>();

        KeyboardState currState;

        double runTime;
        double roundTime;

        List<List<Cell>> gameBoard = new List<List<Cell>>();
        private int gameGridXSize;
        private int gameGridYSize;
        private Vector2 upperLeftLimit;
        private Vector2 lowerRightLimit;

        private float mouseX;
        private float mouseY;

        /////////////////// Sprite Variables ///////////////////

        private Turret m_machineGun;
        private Turret m_missileLauncher;
        private Turret m_bombLauncher;
        private Turret m_railgun;

        private Turret m_selectedTurret;
        private AnimatedSprite m_markSelectedTurret;
        private Vector2 turretLastPosition;


        ///////////////////////////////////////////////////////

        private KeyboardInput m_inputKeyboard = new KeyboardInput();
        private MouseInput m_inputMouse = new MouseInput();

        Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();
        Dictionary<string, int> towerTypes = new Dictionary<string, int>();

        public Keys[] keysToRemove = new Keys[] { Keys.S, Keys.U, Keys.G };


        public override void loadContent(ContentManager contentManager)
        {
            runTime = 0;
            roundTime = 0;

            towerTypes.Clear();
            towerTypes.Add("machineGun", 0);
            towerTypes.Add("missileLauncher", 1);
            towerTypes.Add("bombLauncher", 2);
            towerTypes.Add("railgun", 3);

            
            if (!alreadyLoaded)
            {

                //////////////////////////// Load Sprite Assets /////////////////////////////
                
                m_time = contentManager.Load<SpriteFont>("Fonts/MenuFont");
                m_lives = contentManager.Load<SpriteFont>("Fonts/MenuFont");
                m_score = contentManager.Load<SpriteFont>("Fonts/MenuFont");
                m_end = contentManager.Load<SpriteFont>("Fonts/MenuFont");
                m_round = contentManager.Load<SpriteFont>("Fonts/MenuFont");

                m_shopHeader = contentManager.Load<SpriteFont>("Fonts/MenuFont");
                m_machineGunHeader = contentManager.Load<SpriteFont>("Fonts/TurretMenuFont");
                m_missileLauncherHeader = contentManager.Load<SpriteFont>("Fonts/TurretMenuFont");
                m_bombLauncherHeader = contentManager.Load<SpriteFont>("Fonts/TurretMenuFont");
                m_railgunHeader = contentManager.Load<SpriteFont>("Fonts/TurretMenuFont");

                Texture2D singleGridSquare = contentManager.Load<Texture2D>("Images/SingleGridSquare");
                loadedTextures.Add("singleGridSquare", singleGridSquare);

                Texture2D testPixelTexture = contentManager.Load<Texture2D>("Images/PinkPixel");
                loadedTextures.Add("testPixelTexture", testPixelTexture);

                Texture2D blueGruntTexture = contentManager.Load<Texture2D>("Images/BlueCreepMoving");
                loadedTextures.Add("blueGruntTexture", blueGruntTexture);
                Texture2D blueTankTexture = contentManager.Load<Texture2D>("Images/BlueCreep2Moving");
                loadedTextures.Add("blueTankTexture", blueTankTexture);
                Texture2D blueFlyingTexture = contentManager.Load<Texture2D>("Images/BlueFlyingMoving");
                loadedTextures.Add("blueFlyingTexture", blueFlyingTexture);

                Texture2D greenGruntTexture = contentManager.Load<Texture2D>("Images/GreenCreepMoving");
                loadedTextures.Add("greenGruntTexture", greenGruntTexture);
                Texture2D greenTankTexture = contentManager.Load<Texture2D>("Images/GreenCreep2Moving");
                loadedTextures.Add("greenTankTexture", greenTankTexture);
                Texture2D greenFlyingTexture = contentManager.Load<Texture2D>("Images/GreenFlyingMoving");
                loadedTextures.Add("greenFlyingTexture", greenFlyingTexture);

                Texture2D redGruntTexture = contentManager.Load<Texture2D>("Images/RedCreepMoving");
                loadedTextures.Add("redGruntTexture", redGruntTexture);
                Texture2D redTankTexture = contentManager.Load<Texture2D>("Images/RedCreep2Moving");
                loadedTextures.Add("redTankTexture", redTankTexture);
                Texture2D redFlyingTexture = contentManager.Load<Texture2D>("Images/RedFlyingMoving");
                loadedTextures.Add("redFlyingTexture", redFlyingTexture);

                Texture2D yellowGruntTexture = contentManager.Load<Texture2D>("Images/YellowCreepMoving");
                loadedTextures.Add("yellowGruntTexture", yellowGruntTexture);
                Texture2D yellowTankTexture = contentManager.Load<Texture2D>("Images/YellowCreep2Moving");
                loadedTextures.Add("yellowTankTexture", yellowTankTexture);
                Texture2D yellowFlyingTexture = contentManager.Load<Texture2D>("Images/YellowFlyingMoving");
                loadedTextures.Add("yellowFlyingTexture", yellowFlyingTexture);

                Texture2D bossTexture = contentManager.Load<Texture2D>("Images/BossMoving");
                loadedTextures.Add("bossTexture", bossTexture);

                Texture2D gruntTexture = contentManager.Load<Texture2D>("Images/GruntCreepResized");
                loadedTextures.Add("gruntTexture", gruntTexture);
                Texture2D tankTexture = contentManager.Load<Texture2D>("Images/TankCreepResized");
                loadedTextures.Add("tankTexture", tankTexture);
                Texture2D flyingTexture = contentManager.Load<Texture2D>("Images/FlyingCreepResized");
                loadedTextures.Add("flyingTexture", flyingTexture);

                Texture2D machineGunTexture = contentManager.Load<Texture2D>("Images/MachineGunResized");
                loadedTextures.Add("machineGunTexture", machineGunTexture);
                Texture2D missileLauncherTexture = contentManager.Load<Texture2D>("Images/MissileLauncherResized");
                loadedTextures.Add("missileLauncherTexture", missileLauncherTexture);
                Texture2D bombLauncherTexture = contentManager.Load<Texture2D>("Images/BombLauncherResized");
                loadedTextures.Add("bombLauncherTexture", bombLauncherTexture);
                Texture2D railgunTexture = contentManager.Load<Texture2D>("Images/RailgunResized");
                loadedTextures.Add("railgunTexture", railgunTexture);

                Texture2D radiusTexture = contentManager.Load<Texture2D>("Images/RadiusTexture");
                loadedTextures.Add("radiusTexture", radiusTexture);

                Texture2D machineGunProjectileTexture = contentManager.Load<Texture2D>("Images/MachineGunProjectile");
                loadedTextures.Add("machineGunProjectileTexture", machineGunProjectileTexture);
                Texture2D machineGunProjectile2Texture = contentManager.Load<Texture2D>("Images/MachineGunProjectile2");
                loadedTextures.Add("machineGunProjectile2Texture", machineGunProjectile2Texture);
                Texture2D missileLauncherProjectileTexture = contentManager.Load<Texture2D>("Images/MissileLauncherProjectile");
                loadedTextures.Add("missileLauncherProjectileTexture", missileLauncherProjectileTexture);
                Texture2D bombLauncherProjectileTexture = contentManager.Load<Texture2D>("Images/BombLauncherProjectile");
                loadedTextures.Add("bombLauncherProjectileTexture", bombLauncherProjectileTexture);
                Texture2D railgunProjectileTexture = contentManager.Load<Texture2D>("Images/RailgunProjectile");
                loadedTextures.Add("railgunProjectileTexture", railgunProjectileTexture);

                Texture2D bulletTexture = contentManager.Load<Texture2D>("Images/BlasterSprite");
                loadedTextures.Add("bulletTexture", bulletTexture);

                Texture2D markedTurretTexture = contentManager.Load<Texture2D>("Images/TurretSelectedAnimation");
                loadedTextures.Add("markedTurretTexture", markedTurretTexture);



                ///////////////////////////////////////////////////////////////////////////

                m_inputMouse.registerCommand(MouseInput.MouseEvent.MouseDown, new InputDeviceHelper.CommandDelegatePosition(onMouseDown));
                m_inputMouse.registerCommand(MouseInput.MouseEvent.MouseUp, new InputDeviceHelper.CommandDelegatePosition(onMouseUp));
                m_inputMouse.registerCommand(MouseInput.MouseEvent.MouseMove, new InputDeviceHelper.CommandDelegatePosition(onMouseMove));
            }
            else
            {
                /////////////////////////// Prevents double registering mouse commands ////////////////////////////////
                
                m_inputMouse.removeCommand(MouseInput.MouseEvent.MouseDown);
                m_inputMouse.removeCommand(MouseInput.MouseEvent.MouseUp);
                m_inputMouse.removeCommand(MouseInput.MouseEvent.MouseMove);

                m_inputMouse.registerCommand(MouseInput.MouseEvent.MouseDown, new InputDeviceHelper.CommandDelegatePosition(onMouseDown));
                m_inputMouse.registerCommand(MouseInput.MouseEvent.MouseUp, new InputDeviceHelper.CommandDelegatePosition(onMouseUp));
                m_inputMouse.registerCommand(MouseInput.MouseEvent.MouseMove, new InputDeviceHelper.CommandDelegatePosition(onMouseMove));

                ////////////////////////////////////////////////////////////////////////////////////////////////

                // Clear all the lists of objects each time a new game is started.
                allEnemies.Clear();
                shopTurrets.Clear();
                fieldTurrets.Clear();
                allProjectiles.Clear();
                gameBoard.Clear();

                ///////////////////////////////// Create Game Board /////////////////////////////////////////
                gameGridXSize = (int)((m_graphics.PreferredBackBufferWidth - 576) / 64);
                gameGridYSize = (int)((m_graphics.PreferredBackBufferHeight - 256) / 64);

                upperLeftLimit = new Vector2(192, 128);
                lowerRightLimit = new Vector2((m_graphics.PreferredBackBufferWidth - 320), (m_graphics.PreferredBackBufferHeight - 128));


                for (int i = 0; i <= gameGridXSize; i++)
                {
                    List<Cell> tempList = new List<Cell>();
                    gameBoard.Add(tempList);

                    for (int j = 0; j <= gameGridYSize; j++)
                    {
                        gameBoard[i].Add(new Cell(new Vector2(i, j), new Vector2((i * 64) + 192, (j * 64) + 128)));
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////

                /////////////// Loading a Texture //////////////////

                

                m_machineGun = new Turret(
                    new Vector2(64, 64),
                    new Vector2(m_graphics.PreferredBackBufferWidth - 130, 200),
                    loadedTextures["machineGunTexture"],
                    towerTypes["machineGun"],
                    loadedTextures["radiusTexture"]);

                m_machineGun.isShopTurret = true;

                m_missileLauncher = new Turret(
                    new Vector2(64, 64),
                    new Vector2(m_graphics.PreferredBackBufferWidth - 130, 300),
                    loadedTextures["missileLauncherTexture"],
                    towerTypes["missileLauncher"],
                    loadedTextures["radiusTexture"]);

                m_missileLauncher.isShopTurret = true;

                m_bombLauncher = new Turret(
                    new Vector2(64, 64),
                    new Vector2(m_graphics.PreferredBackBufferWidth - 130, 400),
                    loadedTextures["bombLauncherTexture"],
                    towerTypes["bombLauncher"],
                    loadedTextures["radiusTexture"]);

                m_bombLauncher.isShopTurret = true;

                m_railgun = new Turret(
                    new Vector2(64, 64),
                    new Vector2(m_graphics.PreferredBackBufferWidth - 130, 500),
                    loadedTextures["railgunTexture"],
                    towerTypes["railgun"],
                    loadedTextures["radiusTexture"]);

                m_railgun.isShopTurret = true;

                m_markSelectedTurret = new AnimatedSprite(
                    new Vector2(64, 64),
                    new Vector2(0, 0),
                    loadedTextures["markedTurretTexture"],
                    new int[] { 300, 150, 150, 150 });


                shopTurrets.AddRange(new Turret[] { m_missileLauncher, m_machineGun, m_bombLauncher, m_railgun });
                otherSprites.Add(m_markSelectedTurret);

                ////////////////////////////////////////////////////

                //////////////////// Setting up Message Board //////////////////////

                // Testing the Highscore System and Shop
                m_player.gameScore = 2000;
                m_player.overallScore = 0;

                messageStats[0] = m_player.gameScore.ToString();
                messageStats[1] = Math.Round(runTime / 1000, 1).ToString();
                messageStats[2] = m_player.health.ToString();
                messageStats[3] = m_player.round.ToString();

                ////////////////////////////////////////////////////////////////////



                if (File.Exists(ControlsView.CONTROLFILE))
                {
                    for(int i = 0; i < keysToRemove.Length; i++)
                    {
                        m_inputKeyboard.removeCommand(keysToRemove[i]);
                    }

                    string contents = File.ReadAllText(ControlsView.CONTROLFILE);
                    Keys[] controlsArray = (Keys[])JsonSerializer.Deserialize(contents, typeof(Keys[]));

                    for (int i = 0; i < keysToRemove.Length; i++)
                    {
                        m_inputKeyboard.removeCommand(keysToRemove[i]);
                        keysToRemove[i] = controlsArray[i];
                    }

                    m_inputKeyboard.registerCommand(controlsArray[0], true, new InputDeviceHelper.CommandDelegate((gameTime, value) => { sellTurret(m_selectedTurret); }));
                    m_inputKeyboard.registerCommand(controlsArray[1], true, new InputDeviceHelper.CommandDelegate((gameTime, value) => { upgradeTurret(m_selectedTurret); }));
                    m_inputKeyboard.registerCommand(controlsArray[2], true, new InputDeviceHelper.CommandDelegate((gameTime, value) => { nextRound(); }));

                }
                else
                {
                    for (int i = 0; i < keysToRemove.Length; i++)
                    {
                        m_inputKeyboard.removeCommand(keysToRemove[i]);
                    }

                    keysToRemove[0] = Keys.S;
                    keysToRemove[1] = Keys.U;
                    keysToRemove[2] = Keys.G;
                    

                    m_inputKeyboard.registerCommand(Keys.S, true, new InputDeviceHelper.CommandDelegate((gameTime, value) => { sellTurret(m_selectedTurret); }));
                    m_inputKeyboard.registerCommand(Keys.U, true, new InputDeviceHelper.CommandDelegate((gameTime, value) => { upgradeTurret(m_selectedTurret); }));
                    m_inputKeyboard.registerCommand(Keys.G, true, new InputDeviceHelper.CommandDelegate((gameTime, value) => { nextRound(); }));
                }
            }
            
            alreadyLoaded = true;

        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            currState = Keyboard.GetState();

            if (currState.IsKeyDown(Keys.Escape))
            {
                // Temporary way to test if the Highscores view is working correctly.  In the future, move this logic to the gameOver detection.
                if (base.m_player.gameOver)
                {
                    highScores.Add(m_player.overallScore);
                    File.WriteAllText(HighScoresView.HIGHSCORES_FILE, JsonSerializer.Serialize(highScores));
                }

                return GameStateEnum.MainMenu;
            }


            return GameStateEnum.GamePlay;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            //Vector2 stringSize = m_font.MeasureString(MESSAGE);
            //m_spriteBatch.DrawString(m_font, MESSAGE, new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, m_graphics.PreferredBackBufferHeight / 2 - stringSize.Y), Color.Yellow);

            /////////////////////////////// Draw all enemies ////////////////////////////////
            
            foreach (Enemy currCreep in allEnemies)
            {
                if (roundTime > currCreep.delay)
                {
                    currCreep.draw(m_spriteBatch);
                }

            }

            //////////////////////////////////////////////////////////////////////////////////
            
            /////////////////////////////// Draw Game Board //////////////////////////////////

            for(int i = 0; i <= gameGridXSize; i++)
            {
                for(int j = 0; j <= gameGridYSize; j++)
                {
                    m_spriteBatch.Draw(loadedTextures["singleGridSquare"], new Rectangle((int) gameBoard[i][j].cartCoords.X, (int)gameBoard[i][j].cartCoords.Y, 64, 64), Color.White);
                }
            }

            /////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////// Draw All Turrets //////////////////////////////////
            
            foreach (Turret shopTurret in shopTurrets)
            {
                shopTurret.draw(m_spriteBatch);
            }

            foreach (Turret fieldTurret in fieldTurrets)
            {
                fieldTurret.draw(m_spriteBatch);
            }

            if (m_markSelectedTurret.mark)
            {
                m_markSelectedTurret.draw(m_spriteBatch);
            }

            /////////////////////////////////////////////////////////////////////////////////

            //////////////////////////////// Draw Projectiles ////////////////////////////////////
            
            foreach(Projectile p in allProjectiles)
            {
                p.draw(m_spriteBatch);
            }

            /////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////// Game Headers //////////////////////////////////////////

            Vector2 string1Size = m_time.MeasureString(messageBoard[1] + messageStats[1]);
            m_spriteBatch.DrawString(m_time, messageBoard[1] + messageStats[1], new Vector2((m_graphics.PreferredBackBufferWidth / 5) - 100, 20), Color.White);

            Vector2 string2Size = m_score.MeasureString(messageBoard[0] + messageStats[0]);
            m_spriteBatch.DrawString(m_score, messageBoard[0] + messageStats[0], new Vector2(((m_graphics.PreferredBackBufferWidth / 5) * 2) - (string2Size.X / 2) - 100, 20), Color.White);

            Vector2 string3Size = m_lives.MeasureString(messageBoard[2] + messageStats[2]);
            m_spriteBatch.DrawString(m_lives, messageBoard[2] + messageStats[2], new Vector2(((m_graphics.PreferredBackBufferWidth / 5) * 3) - (string2Size.X / 2) - 100, 20), Color.White);

            Vector2 string9Size = m_round.MeasureString(messageBoard[3] + messageStats[3]);
            m_spriteBatch.DrawString(m_round, messageBoard[3] + messageStats[3], new Vector2(((m_graphics.PreferredBackBufferWidth / 5) * 4) - (string2Size.X / 2) - 100, 20), Color.White);

            /////////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////// Drawing the shop Information /////////////////////////////////////////
            Vector2 string4Size = m_shopHeader.MeasureString(userMessage);
            m_spriteBatch.DrawString(m_shopHeader, userMessage, new Vector2(m_graphics.PreferredBackBufferWidth - string4Size.X - 50, 20), Color.White);

            Vector2 string5Size = m_machineGunHeader.MeasureString(machineGunCost);
            m_spriteBatch.DrawString(m_machineGunHeader, machineGunCost, new Vector2(m_graphics.PreferredBackBufferWidth - string5Size.X - 50, 225), Color.White);

            Vector2 string6Size = m_missileLauncherHeader.MeasureString(missileLauncherCost);
            m_spriteBatch.DrawString(m_missileLauncherHeader, missileLauncherCost, new Vector2(m_graphics.PreferredBackBufferWidth - string6Size.X - 50, 325), Color.White);

            Vector2 string7Size = m_bombLauncherHeader.MeasureString(bombLauncherCost);
            m_spriteBatch.DrawString(m_bombLauncherHeader, bombLauncherCost, new Vector2(m_graphics.PreferredBackBufferWidth - string7Size.X - 50, 425), Color.White);

            Vector2 string8Size = m_railgunHeader.MeasureString(railgunCost);
            m_spriteBatch.DrawString(m_railgunHeader, railgunCost, new Vector2(m_graphics.PreferredBackBufferWidth - string8Size.X - 50, 525), Color.White);

            ///////////////////////////////////////////////////////////////////////////////////////////


            //////////////////////////////// Mouse Position for Testing ///////////////////////////////
            
            m_spriteBatch.DrawString(m_time, "X: " + mouseX + ", Y: " + mouseY, new Vector2(20, 20), Color.White);

            ///////////////////////////////////////////////////////////////////////////////////////////

            //////////////////////////////// End Game Message ///////////////////////////////////////
            
            if (m_player.gameOver)
            {

                Vector2 stringSize = m_end.MeasureString(BADMESSAGE);
                m_spriteBatch.DrawString(
                    m_end,
                    BADMESSAGE,
                    new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, 300),
                    Color.Green);

            }

            /////////////////////////////////////////////////////////////////////////////////////////



            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {

            runTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            roundTime += gameTime.ElapsedGameTime.TotalSeconds;

            // Checks if all enemies have all either been killed or made it through the field.  If so, the round is over.
            if(allEnemies.Count() == 0)
            {
                m_player.roundOver = true;
            }

            ///////////////////////// Logic for Message Stats //////////////////////////

            messageStats[0] = m_player.gameScore.ToString();
            messageStats[1] = Math.Round(runTime / 1000, 1).ToString();
            messageStats[2] = m_player.health.ToString();
            messageStats[3] = m_player.round.ToString();

            ////////////////////////////////////////////////////////////////////////////

            //////////////////////// Logic for all Collision //////////////////////////////

            // Creeps and Projectiles
            for (int i = 0; i < allEnemies.Count; i++)
            {
                for (int j = 0; j < allProjectiles.Count; j++)
                {
                    if (allEnemies[i].collisionRec.Intersects(allProjectiles[j].collisionRec))
                    {
                        allEnemies[i].onCollison(allProjectiles[j]);
                        allProjectiles[j].onCollison(allEnemies[i]);
                    }
                }
            }

            // Causing the turrets to face the creeps.  Each turret only worries about whatever is closest.  Then it shoots at that creep
            for (int i = 0; i < fieldTurrets.Count; i++)
            {
                Enemy closestCreep = null;
                float shortestDistance = 10000f;
                float currDistance;

                if (fieldTurrets[i].startShooting)
                {
                    fieldTurrets[i].fireTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }

                for (int j = 0; j < allEnemies.Count; j++)
                {
                    currDistance = Vector2.Distance(fieldTurrets[i].Center, allEnemies[j].Center);

                    if (currDistance < fieldTurrets[i].radius)
                    {
                        if(closestCreep == null)
                        {
                            closestCreep = allEnemies[j];
                            shortestDistance = currDistance;
                        }

                        if(currDistance < shortestDistance)
                        {
                            closestCreep = allEnemies[j];
                            shortestDistance = currDistance;
                        }
                    }
                }

                if(closestCreep != null)
                {

                    fieldTurrets[i].faceCreep(closestCreep);

                    // Checks if the turret can target flying if the creep is flying.
                    if (closestCreep is FlyingCreep)
                    {
                        if (fieldTurrets[i].canTargetFlying)
                        {
                            fieldTurrets[i].startShooting = true;
                            // Only shoots a bullet on at a fixed fire rate
                            if (fieldTurrets[i].fireTime > fieldTurrets[i].fireRate)
                            {
                                fieldTurrets[i].fireTime -= fieldTurrets[i].fireRate;
                                createBullet(fieldTurrets[i]);
                            }
                        }
                        else
                        {
                            fieldTurrets[i].startShooting = false;
                        }
                    }
                    else
                    {
                        fieldTurrets[i].startShooting = true;
                        // Only shoots a bullet on at a fixed fire rate
                        if (fieldTurrets[i].fireTime > fieldTurrets[i].fireRate)
                        {
                            fieldTurrets[i].fireTime -= fieldTurrets[i].fireRate;
                            createBullet(fieldTurrets[i]);
                        }
                    }
                    
                }
                else
                {
                    fieldTurrets[i].startShooting = false;
                }

            }
            

            ////////////////////////////////////////////////////////////////////////////

            /////////////////////// Update all Objects ////////////////////////////

            foreach (Enemy currCreep in allEnemies)
            {
                // Only spawns the creeps in after their delay
                if (roundTime > currCreep.delay)
                {
                    currCreep.targetable = true;
                    currCreep.update(gameTime);

                }
            }

            foreach(Projectile projectile in allProjectiles)
            {
                projectile.update(gameTime);
            }

            foreach(Turret currTurret in shopTurrets)
            {
                while(currTurret.currentTier < currTurret.expectedTier)
                {
                    currTurret.currentTier++;
                    currTurret.update(gameTime);
                }
            }

            foreach (Turret fieldTurret in fieldTurrets)
            {
                while (fieldTurret.currentTier < fieldTurret.expectedTier)
                {
                    fieldTurret.currentTier++;
                    fieldTurret.update(gameTime);
                }
            }

            // Animates the marked indicator for the selected turret
            if (m_markSelectedTurret.mark)
            {
                m_markSelectedTurret.update(gameTime);
            }

            ////////////////////////////////////////////////////////////////////////

            ///////////////////// Remove Unnecessary Objects ////////////////////////

            foreach (Enemy currCreep in allEnemies)
            {
                if (currCreep.destroy)
                {
                    if (currCreep.killed)
                    {
                        m_player.gameScore += currCreep.pointValue;
                        m_player.overallScore += currCreep.pointValue;
                    }
                    else
                    {
                        // Remove a life for each creep that makes it through.
                        if(m_player.health > 0)
                        {
                            m_player.health -= 1;
                        }
                    }

                    objectsToRemove.Add(currCreep);

                }
            }

            foreach (Turret fieldTurret in fieldTurrets)
            {
                if (fieldTurret.destroy)
                {
                    objectsToRemove.Add(fieldTurret);

                }
            }

            foreach(Projectile projectile in allProjectiles)
            {
                if (projectile.destroy)
                {
                    objectsToRemove.Add(projectile);
                }
            }

            foreach (AnimatedSprite currObject in objectsToRemove)
            {
                if(currObject is Enemy)
                {
                    allEnemies.Remove((Enemy)currObject);
                }
                else if (currObject is Turret)
                {
                    fieldTurrets.Remove((Turret)currObject);
                }
                else if (currObject is Projectile)
                {
                    allProjectiles.Remove((Projectile)currObject);
                }

            }

            /////////////////////////////////////////////////////////////////////////
            
            if(allEnemies.Count == 0 && !boardCleared)
            {
                boardCleared = true;

                foreach(List<Cell> cellList in gameBoard)
                {
                    foreach(Cell c in cellList)
                    {
                        c.creepCount = 0;
                    }
                }
            }

            
            ////////////////////// Check if the player has lost //////////////////////
            
            if(m_player.health <= 0)
            {
                m_player.gameOver = true;
            }

            /////////////////////////////////////////////////////////////////////////

            m_inputKeyboard.Update(gameTime);
            m_inputMouse.Update(gameTime);
        }

        public void createBullet(Turret turretRef)
        {
            Projectile nextShot = null;
            Vector2 bulletPos = turretRef.Center;

            if (turretRef.towerType == 0)
            {
                nextShot = new Projectile(
                new Vector2(4, 16),
                bulletPos,
                loadedTextures["machineGunProjectileTexture"],
                new int[] { 1 },
                120 / 1000.0,
                turretRef);
            }
            else if (turretRef.towerType == 1)
            {
                nextShot = new Projectile(
                new Vector2(7, 22),
                bulletPos,
                loadedTextures["missileLauncherProjectileTexture"],
                new int[] { 1 },
                200 / 1000.0,
                turretRef);
            }
            else if (turretRef.towerType == 2)
            {
                nextShot = new Projectile(
                new Vector2(19, 19),
                bulletPos,
                loadedTextures["bombLauncherProjectileTexture"],
                new int[] { 1 },
                90 / 1000.0,
                turretRef);
            }
            else if (turretRef.towerType == 3)
            {
                nextShot = new Projectile(
                new Vector2(5, 20),
                bulletPos,
                loadedTextures["railgunProjectileTexture"],
                new int[] { 1 },
                150 / 1000.0,
                turretRef);
            }

            if(nextShot != null)
            {
                nextShot.m_direction = turretRef.m_direction;
                nextShot.m_rotation = turretRef.m_rotation;
                allProjectiles.Add(nextShot);
            }

        }

        public void dropPixel(Vector2 mousePos)
        {
            AnimatedSprite m_testPixel = new AnimatedSprite(new Vector2(2, 2), mousePos, loadedTextures["testPixelTexture"], new int[] { 1 });
            //allProjectiles.Add(m_testPixel);
        }

        // Lambda for the S key.  Sells the selected turret, if there is one.  Gives the player half the value back.
        public void sellTurret(Turret turret)
        {
            if(turret != null)
            {
                m_player.gameScore += turret.sellValue;
                turret.destroy = true;
                turret.location.hasTurret = false;

                // Removes marker from selected turret once it's sold
                m_markSelectedTurret.mark = false;

                // Update the path in case the sold turret caused there to be a shorter one for nearby creeps.
                if (m_player.round % 2 == 0)
                {
                    foreach (Enemy e in allEnemies)
                    {
                        e.path = findPath(e.currCell, gameBoard[gameBoard.Count / 2][gameBoard[0].Count - 1]);
                    }

                }
                else
                {
                    foreach (Enemy e in allEnemies)
                    {
                        e.path = findPath(e.currCell, gameBoard[gameBoard.Count - 1][gameBoard[0].Count / 2]);
                    }
                }

            }
        }

        // Lambda for the U key.  Upgrades the selected turret, if there is one.  Charges the player points equal to the cost of the turret
        public void upgradeTurret(Turret turret)
        {
            if(turret != null)
            {
                if (m_player.gameScore >= turret.cost && turret.currentTier < turret.m_spriteTime.Length)
                {
                    turret.expectedTier++;
                    turret.sellValue += turret.cost / 2;
                    m_player.gameScore -= turret.cost;
                }
                else
                {
                    // Let the player know that the turret is maxed out.
                }
            }
            
        }

        // Starts the next round
        public void nextRound()
        {
            int gruntCount = 5;
            int tankCount = 0;
            int flyingCount = 0;
            Vector2 startingPointCart;
            Vector2 startingPointGrid;
            Vector2 endPointCart;
            Vector2 endPointGrid;
            int creepDifficulty = 0;
            Texture2D creepTexture;
            List<Cell> shortestPath;

            // Variables to stagger the deployment of new creeps each round.  Meant to help control the spawns so the player isn't overwhelmed with a blob of creeps at round start
            double delayAdjustmentGrunt = 0;
            double delayAdjustmentTank = 0;
            double delayAdjustmentFlying = 0;

            if (!m_player.gameOver)
            {
                for (int i = 6; i <= m_player.round; i += 6)
                {
                    creepDifficulty += 1;
                }

                if (m_player.roundOver)
                {
                    m_player.nextRound = true;
                    m_player.round += 1;
                    m_player.roundOver = false;

                    // Resets the round timer so the creep spawn delays are correct.
                    roundTime = 0;
                }
                else
                {
                    // Let the player know that the current round isn't over yet
                }

                if (m_player.nextRound)
                {
                    // Resets the board clear functionality of the update method
                    boardCleared = false;

                    // Sets the initial position and direction of the creeps
                    if (m_player.round % 2 == 0)
                    {
                        int gridXStart = gameBoard.Count / 2;
                        int gridYStart = 0;

                        int gridXEnd = gameBoard.Count / 2;
                        int gridYEnd = gameBoard[0].Count - 1;

                        startingPointCart = gameBoard[gridXStart][gridYStart].cartCoords + new Vector2(32, 0);
                        startingPointGrid = new Vector2(gridXStart, gridYStart);

                        endPointCart = gameBoard[gridXEnd][gridYEnd].cartCoords + new Vector2(32, 0);
                        endPointGrid = new Vector2(gridXEnd, gridYEnd);

                        shortestPath = findPath(gameBoard[gridXStart][gridYStart], gameBoard[gridXEnd][gridYEnd]);

                    }
                    else
                    {
                        int gridXStart = 0;
                        int gridYStart = gameBoard[0].Count / 2;

                        int gridXEnd = gameBoard.Count - 1;
                        int gridYEnd = gameBoard[0].Count / 2;

                        startingPointCart = gameBoard[gridXStart][gridYStart].cartCoords + new Vector2(0, 32);
                        startingPointGrid = new Vector2(gridXStart, gridYStart);

                        endPointCart = gameBoard[gridXEnd][gridYEnd].cartCoords + new Vector2(0, 32);
                        endPointGrid = new Vector2(gridXEnd, gridYEnd);

                        shortestPath = findPath(gameBoard[gridXStart][gridYStart], gameBoard[gridXEnd][gridYEnd]);
                    }

                    // Allows tanks after round 2 and flying creeps after round 3
                    if (m_player.round > 2)
                    {
                        tankCount += (m_player.round - 1) * 3;
                    }

                    if (m_player.round > 3)
                    {
                        flyingCount = (int)(m_player.round * 1.5);
                    }

                    gruntCount += m_player.round * 5;

                    for (int i = 0; i < gruntCount; i++)
                    {
                        creepTexture = loadedTextures["gruntTexture"];

                        //if (creepDifficulty == 0)
                        //{
                        //    creepTexture = loadedTextures["blueGruntTexture"];
                        //}
                        //else if (creepDifficulty == 1)
                        //{
                        //    creepTexture = loadedTextures["greenGruntTexture"];
                        //}
                        //else if (creepDifficulty == 2)
                        //{
                        //    creepTexture = loadedTextures["redGruntTexture"];
                        //}
                        //else
                        //{
                        //    creepTexture = loadedTextures["yellowGruntTexture"];
                        //}

                        GruntCreep tempCreep = new GruntCreep(
                            new Vector2(64, 64),
                            startingPointCart,
                            creepTexture,
                            shortestPath);

                        if (i == 0)
                        {
                            tempCreep.tag = 1;
                        }

                        if (m_player.round % 2 == 0)
                        {
                            tempCreep.movingHorizontal = false;
                        }

                        tempCreep.health += (int)(1 * m_player.round);

                        tempCreep.delay += delayAdjustmentGrunt;
                        allEnemies.Add(tempCreep);

                        delayAdjustmentGrunt++;
                    }

                    for (int i = 0; i < tankCount; i++)
                    {
                        creepTexture = loadedTextures["tankTexture"];

                        //if (creepDifficulty == 0)
                        //{
                        //    creepTexture = loadedTextures["blueTankTexture"];
                        //}
                        //else if (creepDifficulty == 1)
                        //{
                        //    creepTexture = loadedTextures["greenTankTexture"];
                        //}
                        //else if (creepDifficulty == 2)
                        //{
                        //    creepTexture = loadedTextures["redTankTexture"];
                        //}
                        //else
                        //{
                        //    creepTexture = loadedTextures["yellowTankTexture"];
                        //}

                        TankCreep tempCreep = new TankCreep(
                            new Vector2(64, 64),
                            startingPointCart,
                            creepTexture,
                            shortestPath);

                        if (m_player.round % 2 == 0)
                        {
                            tempCreep.movingHorizontal = false;
                        }

                        tempCreep.health += (int)(1.5 * m_player.round);

                        tempCreep.delay += delayAdjustmentTank;
                        allEnemies.Add(tempCreep);

                        delayAdjustmentTank += 2.5;
                    }

                    for (int i = 0; i < flyingCount; i++)
                    {
                        creepTexture = loadedTextures["flyingTexture"];

                        //if (creepDifficulty == 0)
                        //{
                        //    creepTexture = loadedTextures["blueFlyingTexture"];
                        //}
                        //else if (creepDifficulty == 1)
                        //{
                        //    creepTexture = loadedTextures["greenFlyingTexture"];
                        //}
                        //else if (creepDifficulty == 2)
                        //{
                        //    creepTexture = loadedTextures["redFlyingTexture"];
                        //}
                        //else
                        //{
                        //    creepTexture = loadedTextures["yellowFlyingTexture"];
                        //}

                        FlyingCreep tempCreep = new FlyingCreep(
                            new Vector2(64, 64),
                            startingPointCart,
                            creepTexture,
                            shortestPath);

                        if (m_player.round % 2 == 0)
                        {
                            tempCreep.movingHorizontal = false;
                        }

                        tempCreep.health += (int)(0.75 * m_player.round);

                        tempCreep.delay += delayAdjustmentFlying;
                        allEnemies.Add(tempCreep);

                        delayAdjustmentFlying += 2.0;
                    }

                    m_player.nextRound = false;

                }
            }
                
        }

        // Checks whether a certain cell is within the game board
        private bool isValid(int numCol, int numRow, Vector2 gridCoords)
        {
            // Returns true if row number and column number
            // is in range
            return ((int)gridCoords.Y >= 0) && ((int)gridCoords.Y < numRow) && ((int)gridCoords.X >= 0)
                   && ((int)gridCoords.X < numCol);
        }

        // Sets the shortestPath list to the shortest path if possible.
        private List<Cell> findPath(Cell startPoint, Cell endPoint, Cell blockedCell = null)
        {
            if (startPoint == blockedCell || endPoint == blockedCell)
            {
                return null;
            }

            Queue<Visit> visits = new Queue<Visit>();
            List<List<Cell>> allPaths = new List<List<Cell>>();
            List<Cell> visitedCells = new List<Cell>();

            Visit start = new Visit(null, startPoint);
            visits.Enqueue(start);
            visitedCells.Add(startPoint);

            while(visits.Count > 0)
            {
                Visit currVisit = visits.Dequeue();
                if(currVisit.subject == endPoint)
                {
                    allPaths.Add(currVisit.getPath());
                }
                else
                {
                    
                    if (currVisit.subject.isUnBlocked() && currVisit.subject != blockedCell)
                    {
                        Action<Vector2> addVisit = (neighborCoordinates) =>
                        {
                            // Changed the contains() coords from y, x to x, y to fix an out of bounds issue.  Also, added the isUnblocked condition to prevent adding visits with cells that have turrets.
                            if (isValid(gameBoard.Count, gameBoard[0].Count, neighborCoordinates)
                                && !visitedCells.Contains(gameBoard[(int)neighborCoordinates.X][(int)neighborCoordinates.Y]) && gameBoard[(int)neighborCoordinates.X][(int)neighborCoordinates.Y].isUnBlocked())
                            {
                                Visit tempVisit = new Visit(currVisit, gameBoard[(int)neighborCoordinates.X][(int)neighborCoordinates.Y]);
                                visitedCells.Add(tempVisit.subject);
                                visits.Enqueue(tempVisit);
                            }
                        };

                        addVisit(new Vector2(currVisit.subject.gridCoords.X + 1, currVisit.subject.gridCoords.Y));
                        addVisit(new Vector2(currVisit.subject.gridCoords.X, currVisit.subject.gridCoords.Y + 1));
                        addVisit(new Vector2(currVisit.subject.gridCoords.X - 1, currVisit.subject.gridCoords.Y));
                        addVisit(new Vector2(currVisit.subject.gridCoords.X, currVisit.subject.gridCoords.Y - 1));

                    }
                }

            }

            var ordered = from path in allPaths orderby path.Count select path;
            return ordered.FirstOrDefault();
        }

        #region Input Handlers

        private bool m_mouseCapture = false;

        private void onMouseDown(GameTime gameTime, int x, int y)
        {
            // Used to track where exactly the x and y coords are
            //dropPixel(mousePos);

            Vector2 mousePos = new Vector2(x, y);
            Vector3 convertedMousePos = new Vector3(x, y, 0);
            bool setTurretNull = false;

            // Removes the radius from the selected turret in case the player clicked somewhere else;
            if (m_selectedTurret != null)
            {
                m_selectedTurret.selected = false;
            }


            //shopTurrets.OfType<Turret>()  (Different way to make this check)
            // Checks if a Shop Turret has been selected.  If so, sets the selectedTurret object.
            foreach (Turret shopTurret in shopTurrets)
            {

                if (shopTurret.collisionRec.Contains(mousePos.ToPoint()))
                {

                    // Checks if the player can afford the tower.  Otherwise it doesn't let them select it
                    if (m_player.gameScore >= shopTurret.cost)
                    {
                        turretLastPosition = shopTurret.Center;
                        m_selectedTurret = shopTurret;
                    }

                    // Used this to test for a specific turret with the debugger
                    //selectedTurret.tag = 1;

                    // Prevents the selectedTurret from being nullified
                    setTurretNull = false;

                    // Removes the selected marker from a field turret once a shop turret is selected
                    m_markSelectedTurret.mark = false;

                    break;
                }
                else
                {
                    // Sets the selectedTurret from being nullified
                    setTurretNull = true;
                }
            }

            // Checks if a Field Turret has been selected.  If so, sets the selectedTurret object.
            foreach (Turret fieldTurret in fieldTurrets)
            {
                if (fieldTurret.collisionRec.Contains(mousePos.ToPoint()))
                {
                    int gridX = (int)((x - 192) / 64);
                    int gridY = (int)((y - 128) / 64);


                    // I don't know what the actually heck I was doing here.  Leaving it commented out in case I need it later.
                    //if (x >= upperLeftLimit.X && y >= upperLeftLimit.Y && x <= lowerRightLimit.X && y <= lowerRightLimit.Y)
                    //{
                    //    gameBoard[gridX][gridY].currTurret = null;
                    //    gameBoard[gridX][gridY].hasTurret = false;
                    //}

                    m_selectedTurret = fieldTurret;

                    // Sets a marker to show the turret is selected
                    m_markSelectedTurret.mark = true;
                    m_markSelectedTurret.Center = m_selectedTurret.Center;

                    // Shows the radius of the selected turret
                    m_selectedTurret.selected = true;

                    // Prevents the selectedTurret from being nullified
                    setTurretNull = false;

                    break;
                }
            }

            // Sets the selectedTurret to null if an area without a turret is clicked
            if (setTurretNull)
            {
                m_selectedTurret = null;

                // Removes the marker since there isn't a selected turret
                m_markSelectedTurret.mark = false;
            }
            
            
            m_mouseCapture = true;
        }

        private void onMouseUp(GameTime gameTime, int x, int y)
        {
            // Used to track where exactly the x and y coords are
            //dropPixel(new Vector2(x, y));

            // Checks if a turret has been selected and is currently held by the mouse
            if (m_selectedTurret != null)
            {
                // If it is a shop turret, a turret replaces it in the shop and the selected turret becomes a field turret
                if (m_selectedTurret.isShopTurret)
                {
                    int gridX = (int)((x - 192) / 64);
                    int gridY = (int)((y - 128) / 64);

                    List<Cell> checkPathHorizontal = null;
                    List<Cell> checkPathVertical = null;

                    if (gridX < gameBoard.Count && gridX >= 0 && gridY < gameBoard[0].Count && gridY >= 0)
                    {
                        checkPathHorizontal = findPath(gameBoard[0][gameBoard[0].Count / 2], gameBoard[gameBoard.Count - 1][gameBoard[0].Count / 2], gameBoard[gridX][gridY]);
                        checkPathVertical = findPath(gameBoard[gameBoard.Count / 2][0], gameBoard[gameBoard.Count / 2][gameBoard[0].Count - 1], gameBoard[gridX][gridY]);
                    }
                    

                    // Makes sure the position to drop the turret is in the playing field and isn't already occupied with another turret or creep.  Else, it returns the turret to its previous position.
                    if (x >= upperLeftLimit.X && y >= upperLeftLimit.Y && x <= lowerRightLimit.X && y <= lowerRightLimit.Y && !gameBoard[gridX][gridY].hasTurret && gameBoard[gridX][gridY].creepCount == 0 
                        && checkPathHorizontal != null && checkPathVertical != null)
                    {
                        Turret m_newShopTurret = new Turret(m_selectedTurret.Size, m_selectedTurret.Center, m_selectedTurret.m_spriteSheet, m_selectedTurret.towerType, m_selectedTurret.radiusTexture);
                        m_newShopTurret.isShopTurret = true;

                        // Charges the player for the tower once its placed.  Wont charge them unless it is actually placed
                        m_player.gameScore -= m_selectedTurret.cost;

                        shopTurrets.Remove(m_selectedTurret);
                        shopTurrets.Add(m_newShopTurret);

                        m_selectedTurret.isShopTurret = false;
                        fieldTurrets.Add(m_selectedTurret);

                        m_newShopTurret.Center = turretLastPosition;

                        // Updating all the positional information for the turret
                        // This may be causing issues with the rotational values for the turrets.
                        Vector2 newTurretCenter = (gameBoard[gridX][gridY].cartCoords + new Vector2(32, 32));
                        gameBoard[gridX][gridY].currTurret = m_selectedTurret;
                        gameBoard[gridX][gridY].currTurret.location = gameBoard[gridX][gridY];
                        gameBoard[gridX][gridY].hasTurret = true;
                        m_selectedTurret.Center = newTurretCenter;

                        // Re-establish the shortest path given the new turret being added.
                        if (m_player.round % 2 == 0)
                        {
                            foreach (Enemy e in allEnemies)
                            {
                                e.path = findPath(e.currCell, gameBoard[gameBoard.Count / 2][gameBoard[0].Count - 1]);
                            }

                        }
                        else
                        {
                            foreach (Enemy e in allEnemies)
                            {
                                e.path = findPath(e.currCell, gameBoard[gameBoard.Count - 1][gameBoard[0].Count / 2]);
                            }
                        }

                        // Set the new path for all the enemies on the board.
                        
                    }
                    else
                    {
                        m_selectedTurret.Center = turretLastPosition;
                    }

                    m_selectedTurret = null;

                }
                else
                {
                    // Let the player know that a turret is selected.  Maybe add a sprite to indicate the turret is selected

                }   
            }

            m_mouseCapture = false;
        }

        private void onMouseMove(GameTime gameTime, int x, int y)
        {
            // Simply updates the turrets position so its center follows the mouse's position.  It also updates the mouse position display vars.
            mouseX = x;
            mouseY = y;

            if (m_mouseCapture)
            {
                if (m_selectedTurret != null)
                {
                    if (m_selectedTurret.isShopTurret)
                    {
                        m_selectedTurret.Center = new Vector2(x, y);
                    }
                }
            }
        }

        #endregion
    }

}
