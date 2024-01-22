using LBSScreen.Bot;
using LBSScreen.PictureDisplayer;
using LBSScreen.PictureDisplayer.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LBSScreen
{
    internal class Core : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Displayer _displayer;
        private DiscordBot bot;
        private string _downloadedImagesPath;
        private SpriteFont _font;
        private Vector2 initalTextOffset = new Vector2(10);
        private Vector2 textPadding = new Vector2(0, 10);

        public EntityManager EntityManager { get; private set; }
        public readonly int Width;
        public readonly int Height;
        public readonly Random Random;


        public static Core Instance { get; private set; }

        public Core()
        {
            Instance = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            Width = Settings.GetData<int>("screenWidth");
            Height = Settings.GetData<int>("screenHeight");

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Width;
            _graphics.PreferredBackBufferHeight = Height;

            _downloadedImagesPath = Settings.GetData<string>("downloadPath");
            Random = new Random();
        }

        protected override void Initialize()
        {
            //bot = new DiscordBot();
            //bot.UpdatedImages += this.BotUpdatedImages;

            EntityManager = new EntityManager();

            _displayer = new Displayer();
            Picture picture = new Picture(Content.Load<Texture2D>("updatera"));

            _displayer.AddPicture(picture);

            List<ITransition> transitions = GetTransitionInstances();
            //_displayer.AddTransition(transitions[0]);
            transitions.ForEach((transition) => _displayer.AddTransition(transition));

            base.Initialize();
        }

        private void BotUpdatedImages()
        {
            string[] files = Directory.GetFiles(_downloadedImagesPath);
            List<Picture> newPictures = new List<Picture>();
            foreach (string file in files)
            {
                Picture picture = new Picture(file);

                if (picture.Errored == false)
                    newPictures.Add(picture);
            }

            _displayer.SetPictures(newPictures.ToArray());
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _font = Content.Load<SpriteFont>("font");
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            //bot.Stop();
            base.OnExiting(sender, args);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            EntityManager.UpdateEntities(delta);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(26, 27, 36));

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            EntityManager.DrawEntities(_spriteBatch);

            CultureInfo culture = new CultureInfo("sv-SE");
            DrawText(_font, DateTime.Now.ToString("HH:mm:ss"), Color.Black, Color.White, 1f, Vector2.Zero);
            DrawText(_font, culture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek), Color.Black, Color.White, .75f, new Vector2(0, 32) + textPadding);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawText(SpriteFont font, string text, Color backColor, Color frontColor, float scale, Vector2 position)
        {
            Vector2 origin = Vector2.Zero;

            _spriteBatch.DrawString(font, text, initalTextOffset + position + new Vector2(1 * scale, 1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);
            _spriteBatch.DrawString(font, text, initalTextOffset + position + new Vector2(-1 * scale, 1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);
            _spriteBatch.DrawString(font, text, initalTextOffset + position + new Vector2(-1 * scale, -1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);
            _spriteBatch.DrawString(font, text, initalTextOffset + position + new Vector2(1 * scale, -1 * scale), backColor, 0, origin, scale, SpriteEffects.None, 1f);          

            _spriteBatch.DrawString(font, text, initalTextOffset + position, frontColor, 0, origin, scale, SpriteEffects.None, 0f);
        }

        private List<ITransition> GetTransitionInstances()
        {
            List<ITransition> transitions = new List<ITransition>();

            Assembly assembly = Assembly.GetExecutingAssembly(); // You may need to adjust this based on your project structure

            IEnumerable<Type> transitionTypes = assembly.GetTypes()
                .Where(t => typeof(ITransition).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (Type transitionType in transitionTypes)
            {
                ITransition transition = Activator.CreateInstance(transitionType) as ITransition;
                if (transition != null)
                {
                    transitions.Add(transition);
                }
            }

            return transitions;
        }
    }
}