namespace EcsR3.Groups.Observable.Tracking.Trackers
{
    public class GroupMatchingState
    {
        public int NeedsComponentsAdding;
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
        
        public bool IsMatch() => (NeedsComponentsAdding == 0 && NeedsComponentsRemoving == 0);
    }
}