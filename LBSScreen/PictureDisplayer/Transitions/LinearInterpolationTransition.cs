using Microsoft.Xna.Framework;

namespace LBSScreen.PictureDisplayer.Transitions
{
    internal class LinearInterpolationTransition : ITransition
    {
        public void UpdateTransition(float lerpAmount, ref Picture currentPicture, ref Picture previousPicture)
        {
            currentPicture.SetColor(Color.Lerp(new Color(255, 255, 255, 0), Color.White, lerpAmount));
            previousPicture.SetColor(Color.Lerp(new Color(255, 255, 255, 0), Color.White, 1f - lerpAmount));
        }
    }
}
