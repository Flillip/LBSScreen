using Microsoft.Xna.Framework;
using System;

namespace LBSScreen.PictureDisplayer.Transitions
{
    internal class EaseOutSineTransition : ITransition
    {
        public void UpdateTransition(float lerpAmount, ref Picture currentPicture, ref Picture previousPicture)
        {
            Color currentColor = currentPicture.GetColor();
            Color previousColor = previousPicture.GetColor();

            currentColor.A = (byte)(EaseOutSine(lerpAmount) * 255);
            previousColor.A = (byte)(255 - EaseOutSine(lerpAmount) * 255);

            currentPicture.SetColor(currentColor);
            previousPicture.SetColor(previousColor);
        }

        private float EaseOutSine(float x)
        {
            return MathF.Sin(x * MathF.PI / 2f);
        }
    }
}
