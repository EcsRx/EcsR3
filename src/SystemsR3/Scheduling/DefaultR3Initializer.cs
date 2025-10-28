using System;
using R3;

namespace SystemsR3.Scheduling
{
    public static class DefaultR3Initializer
    {
        /// <summary>
        /// This provides an out the box default solution for setting up R3 EveryUpdate timings etc
        /// </summary>
        /// <remarks>In Unity/Monogame etc you should use the library specific R3 initializer</remarks>
        /// <param name="updateFrequencyPerSecond"></param>
        public static void SetDefaultObservableSystem(int updateFrequencyPerSecond = 60)
        {
            var updateInterval = 1000f / updateFrequencyPerSecond;
            var timeFrameProvider = new TimerFrameProvider(TimeSpan.FromMilliseconds(updateInterval));
            ObservableSystem.DefaultFrameProvider = timeFrameProvider;
        }
    }
}