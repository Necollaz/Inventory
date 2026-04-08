using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using _Project.Data;
using _Project.Inventory;
using _Project.Persistence;
using _Project.State;

namespace _Project.UI
{
    public sealed class InventorySlotDragDrop :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler
    {
        [SerializeField] private InventorySlotView _slotView;
        
        private InventoryDragGhost _dragGhost;
        private InventoryDragSession _dragSession;
        private GameStateInitializer _initializer;
        private InventoryFacade _inventoryFacade;
        private ItemDatabase _itemDatabase;
        
        private bool _isDraggingFromThisSlot;

        [Inject]
        public void Construct(
            InventoryDragSession dragSession,
            GameStateInitializer initializer,
            InventoryFacade inventoryFacade,
            ItemDatabase itemDatabase)
        {
            _dragSession = dragSession;
            _initializer = initializer;
            _inventoryFacade = inventoryFacade;
            _itemDatabase = itemDatabase;
        }
        
        public void SetDragGhost(InventoryDragGhost dragGhost)
        {
            _dragGhost = dragGhost;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            int slotIndex = _slotView.SlotIndex;
            InventorySlotData slot = _initializer.State.Slots[slotIndex];
            
            if (!slot.IsUnlocked || slot.IsEmpty)
                return;
            
            Sprite sprite = _itemDatabase.Get(slot.Stack.ItemId).InventorySlotSprite;
            _dragSession.Begin(this, slotIndex);
            _isDraggingFromThisSlot = true;
            
            if (_dragGhost != null)
            {
                _dragGhost.SetRaycastBlocking(false);
                _dragGhost.Show(sprite, eventData.position);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDraggingFromThisSlot)
                return;
            
            if (_dragGhost != null)
                _dragGhost.UpdatePosition(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDraggingFromThisSlot)
                return;
            
            _isDraggingFromThisSlot = false;
            
            if (_dragGhost != null)
            {
                _dragGhost.Hide();
                _dragGhost.SetRaycastBlocking(true);
            }

            _dragSession.End();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!_dragSession.IsActive)
                return;
            
            int fromIndex = _dragSession.SourceSlotIndex;
            int toIndex = _slotView.SlotIndex;
            
            if (fromIndex == toIndex)
                return;
            
            _inventoryFacade.TryApplyDragDrop(fromIndex, toIndex);
        }
    }
}