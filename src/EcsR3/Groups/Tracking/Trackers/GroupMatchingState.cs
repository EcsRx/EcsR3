namespace EcsR3.Groups.Tracking.Trackers
{
    /// <summary>
    /// Represents the required changes needed to match a group
    /// </summary>
    public class GroupMatchingState
    {
        /// <summary>
        /// The amount of components that need adding for the match to be successful
        /// </summary>
        public int NeedsComponentsAdding;
        
        /// <summary>
        /// The amount of components that need removing for the match to be successful
        /// </summary>
        public int NeedsComponentsRemoving;

        public GroupMatchingState(int needsComponentsAdding, int needsComponentsRemoving)
        {
            NeedsComponentsAdding = needsComponentsAdding;
            NeedsComponentsRemoving = needsComponentsRemoving;
        }

        public GroupMatchingState(LookupGroup lookupGroup)
        {
            NeedsComponentsAdding = lookupGroup.RequiredComponents.Length;
            NeedsComponentsRemoving = 0;
        }
        
        /// <summary>
        /// Checks to see if the current state is a match for the group
        /// </summary>
        /// <returns></returns>
        public bool IsMatch() => (NeedsComponentsAdding == 0 && NeedsComponentsRemoving == 0);
    }
}