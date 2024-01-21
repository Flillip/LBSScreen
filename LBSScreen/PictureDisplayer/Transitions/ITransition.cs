namespace LBSScreen.PictureDisplayer.Transitions
{
    internal interface ITransition
    {
        void UpdateTransition(float lerpAmount, ref Picture currentPicture, ref Picture previousPicture);
    }
}
