using Microsoft.Xna.Framework;
using System;

namespace LBSScreen.PictureDisplayer.Transitions
{
    internal class EaseOutCircTransition : ITransition
    {
        public void UpdateTransition(float lerpAmount, ref Picture currentPicture, ref Picture previousPicture)
        {
            Color currentColor = currentPicture.GetColor();
            Color previousColor = previousPicture.GetColor();

            currentColor.A = (byte)(EaseOutCirc(lerpAmount) * 255);
            previousColor.A = (byte)(255 - EaseOutCirc(lerpAmount) * 255);

            currentPicture.SetColor(currentColor);
            previousPicture.SetColor(previousColor);
        }

        private float EaseOutCirc(float x)
        {
            return MathF.Sqrt(1f - MathF.Pow(x - 1f, 2f));
        }
    }
}
