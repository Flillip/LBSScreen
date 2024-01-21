using System;

namespace LBSScreen
{
    internal class Timer : BaseEntity
    {
        public event Action OnTimerFinished;

        private readonly float _maxTime;
        private readonly bool _isOneShot;
        private float _currentTime;
        private bool _hasFinished;
        private bool _running;

        public float CurrentTime { get => _currentTime; }
        public bool HasStopped { get => _hasFinished; }

        public Timer(float time, bool isOneShot = false) : base()
        {
            _maxTime = time;
            _currentTime = 0;
            _isOneShot = isOneShot;
            _hasFinished = false;
            _running = false;
        }

        public void Start()
        {
            _running = true;
            _hasFinished = false;
        }

        public void Restart()
        {
            _currentTime = 0;
            _running = true;
            _hasFinished = false;
        }

        public void Stop()
        {
            _running = false;
        }

        public override void Update(float delta)
        {
            if ((_hasFinished && _isOneShot) || _running == false) return;

            _currentTime += delta;

            if (_currentTime >= _maxTime )
            {
                _hasFinished = true;
                _currentTime = 0;
                OnTimerFinished?.Invoke();
            }
        }
    }
}
