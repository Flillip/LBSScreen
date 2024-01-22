using LBSScreen.PictureDisplayer.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LBSScreen.PictureDisplayer
{
    internal class Displayer : BaseDrawableEntity
    {
        private List<Picture> _pictures;
        private Timer _timer;
        private Timer _lerpTimer;
        private int _currentPictureIndex;
        private int _previousPictureIndex;
        private int _lerpTimerTime;

        private List<ITransition> _transitions;
        private int _currentTransition;

        public Displayer()
        {
            _pictures = new List<Picture>();
            _transitions = new List<ITransition>();

            int delay = Settings.GetData<int>("delayBetweenPictures");
            _timer = new Timer(delay);
            _timer.OnTimerFinished += OnTimerFinished;
            _timer.Start();

            _currentPictureIndex = 0;
            _previousPictureIndex = -1;
            _currentTransition = 0;

            _lerpTimerTime = Settings.GetData<int>("lerpTimeBetweenPictures");
            _lerpTimer = new Timer(_lerpTimerTime, true);
            _lerpTimer.OnTimerFinished += OnLerpTimerFinished;
        }

        public void SetPictures(Picture[] pictures)
        {
            _pictures = pictures.ToList();
            _timer.Restart();
            _lerpTimer.Restart();
            _lerpTimer.Stop();
            _currentPictureIndex = 0;
            _previousPictureIndex = -1;
        }

        public void AddPicture(Picture picture)
        {
            _pictures.Add(picture);
            _timer.Restart();
            _lerpTimer.Restart();
            _lerpTimer.Stop();
            _currentPictureIndex = 0;
            _previousPictureIndex = -1;
        }

        public void AddTransition(ITransition transition)
        {
            _transitions.Add(transition);
        }

        public override void Update(float delta)
        {
            if (_pictures.Count == 1) return;
            if (_lerpTimer.HasStopped == false && _previousPictureIndex != -1)
            {
                float lerpValue = MathF.Min(_lerpTimer.CurrentTime / _lerpTimerTime, 1.0f);

                Picture currentPicture = _pictures[_currentPictureIndex];
                Picture previousPicture = _pictures[_previousPictureIndex];

                _transitions[_currentTransition].UpdateTransition(lerpValue, ref currentPicture, ref previousPicture);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Picture currentPicture = _pictures[_currentPictureIndex];

            currentPicture.Draw(spriteBatch, Core.Instance.Width, Core.Instance.Height);

            if (_previousPictureIndex != -1)
            {
                Picture previousPicture = _pictures[_previousPictureIndex];
                previousPicture.Draw(spriteBatch, Core.Instance.Width, Core.Instance.Height);
            }
        }

        private void OnTimerFinished()
        {
            if (_pictures.Count == 1) return;

            _currentTransition = Core.Instance.Random.Next(0, _transitions.Count);

            _previousPictureIndex = _currentPictureIndex;
            _currentPictureIndex = (_currentPictureIndex + 1) % _pictures.Count;
            _lerpTimer.Restart();

            Picture currentPicture = _pictures[_currentPictureIndex];
            Picture previousPicture = _pictures[_previousPictureIndex];

            currentPicture.SetColor(new Color(255, 255, 255, 0));
            previousPicture.SetColor(Color.White);
        }

        private void OnLerpTimerFinished()
        {
            Picture currentPicture = _pictures[_currentPictureIndex];
            Picture previousPicture = _pictures[_previousPictureIndex];

            currentPicture.SetColor(Color.White);
            previousPicture.SetColor(new Color(255, 255, 255, 0));
        }
    }
}
