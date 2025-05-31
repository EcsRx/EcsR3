namespace EcsR3.Systems.Augments
{
    public interface ISystemPreProcessor
    {
        /// <summary>
        /// Triggered before the group is processed
        /// </summary>
        void BeforeProcessing();
    }
}