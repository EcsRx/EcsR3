namespace EcsR3.Systems.Augments
{
    public interface ISystemPostProcessor
    {
        /// <summary>
        /// Triggered after the group is processed
        /// </summary>
        void AfterProcessing();
    }
}