using System;
using System.Timers;
using R3;

namespace SystemsR3.Scheduling
{
    /// <summary>
    /// This default implementation only provides millisecond level accuracy
    /// however in most cases it will instead use a platform specific implementation
    /// which will be as accurate as the underlying platform provides
    /// </summary>
    public class DefaultUpdateScheduler : IUpdateScheduler
    {
        private readonly Timer _timer;
        private DateTime _previousDateTime;
        private readonly Subject<ElapsedTime> _onPreUpdate = new Subject<ElapsedTime>();
        private readonly Subject<ElapsedTime> _onUpdate = new Subject<ElapsedTime>();
        private readonly Subject<ElapsedTime> _onPostUpdate = new Subject<ElapsedTime>();

        public ElapsedTime ElapsedTime { get; private set; }
        public Observable<ElapsedTime> OnUpdate => _onUpdate;
        public Observable<ElapsedTime> OnPreUpdate => _onPreUpdate;
        public Observable<ElapsedTime> OnPostUpdate => _onPostUpdate;
        
        public DefaultUpdateScheduler(int updateFrequencyPerSecond = 60)
        {
            _timer = new Timer { Interval = 1000f / updateFrequencyPerSecond };
            _timer.Elapsed += UpdateTick;

            _previousDateTime = DateTime.Now;
            _timer.Start();
        }

        private void UpdateTick(object sender, ElapsedEventArgs e)
        {
            var deltaTime = e.SignalTime - _previousDateTime;
            var totalTime = ElapsedTime.TotalTime + deltaTime;
            ElapsedTime = new ElapsedTime(deltaTime, totalTime);
            _onPreUpdate.OnNext(ElapsedTime);
            _onUpdate.OnNext(ElapsedTime);
            _onPostUpdate.OnNext(ElapsedTime);
            _previousDateTime = e.SignalTime;
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
            
            _onUpdate.Dispose();
            _onPreUpdate.Dispose();
            _onPostUpdate.Dispose();
        }
    }
}