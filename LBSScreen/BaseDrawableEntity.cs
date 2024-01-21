using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBSScreen
{
    internal abstract class BaseDrawableEntity : BaseEntity
    {
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
