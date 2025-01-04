using SystemsR3.Scheduling;

namespace SystemsR3.Systems.Conventional
{
    /// <summary>
    /// A system which processes every entity every update
    /// </summary>
    /// <remarks>
    /// This relies upon the underlying IObservableScheduler implementation and
    /// is by default aiming for 60 updates per second.
    /// </remarks>
    public interface IBasicSystem : ISystem
    {
        /// <summary>
        /// The method to execute every update
        /// </summary>
        void Execute(ElapsedTime elapsedTime);
    }
}