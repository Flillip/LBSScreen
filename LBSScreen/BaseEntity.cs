namespace LBSScreen
{
     abstract class BaseEntity
    {
        public BaseEntity()
        {
            Core.Instance.EntityManager.AddEntity(this);
        }

        public abstract void Update(float delta);

        public void Destroy()
        {
            Core.Instance.EntityManager.RemoveEntity(this);
        }
    }
}
