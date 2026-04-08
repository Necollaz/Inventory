namespace _Project.UI
{
    public sealed class InventoryDragSession
    {
        public InventorySlotDragDrop SourceView { get; private set; }
        public bool IsActive { get; private set; }
        public int SourceSlotIndex { get; private set; } = -1;
        
        public void Begin(InventorySlotDragDrop sourceView, int sourceSlotIndex)
        {
            IsActive = true;
            SourceView = sourceView;
            SourceSlotIndex = sourceSlotIndex;
        }
        
        public void End()
        {
            IsActive = false;
            SourceView = null;
            SourceSlotIndex = -1;
        }
    }
}