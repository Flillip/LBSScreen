using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBSScreen
{
    internal class EntityManager
    {
        private List<BaseEntity> _entities;

        public EntityManager()
        {
            _entities = new List<BaseEntity>();
        }

        public void AddEntity(BaseEntity entity) =>
            _entities.Add(entity);

        public void RemoveEntity(BaseEntity entity) =>
            _entities.Remove(entity);

        public void UpdateEntities(float delta)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                BaseEntity entity = _entities[i];
                entity.Update(delta);
            }
        }

        public void DrawEntities(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                BaseEntity entity = _entities[i];
                if (entity is BaseDrawableEntity drawableEntity)
                    drawableEntity.Draw(spriteBatch);
            }
        }
    }
}
