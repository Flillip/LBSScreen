using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LBSScreen.PictureDisplayer
{
    internal class Picture
    {
        public readonly bool Errored = false;

        private Texture2D _texture;
        private Vector2 _scale;
        private Color _color;

        public Picture(string path) 
        {
            try
            {
                _texture = Texture2D.FromFile(Core.Instance.GraphicsDevice, path);
            }

            catch (Exception ex)
            {
                Errored = true;
                Logger.Error(ex);
            }

            _scale = Vector2.Zero;
            _color = Color.White;
        }

        public Texture2D GetTexture() => _texture;

        public Vector2 ToScale(int width, int height)
        {
            if (_scale != Vector2.Zero) return _scale;

            float scaled = MathF.Min((float)width / _texture.Width, (float)height / _texture.Height);
            _scale = new Vector2(scaled);

            return _scale;
        }

        public void Draw(SpriteBatch spriteBatch, int width, int height)
        {
            ToScale(width, height);

            spriteBatch.Draw(
                _texture,
                new Vector2(Core.Instance.Width / 2, Core.Instance.Height / 2),
                null,
                GetColor(),
                0f,
                new Vector2(_texture.Width / 2, _texture.Height / 2),
                _scale,
                SpriteEffects.None,
                0f
                );
        }

        public void SetColor(Color color) => _color = color;
        public Color GetColor() => _color;
    }
}
