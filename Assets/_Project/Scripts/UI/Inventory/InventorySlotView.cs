using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Project.UI
{
    public sealed class InventorySlotView : MonoBehaviour
    {
        private Action<int> _onSlotClicked;
        
        [SerializeField] private Button _slotButton;
        [SerializeField] private Image _iconItemImage;
        [SerializeField] private TextMeshProUGUI _stackValueText;
        [SerializeField] private Transform _stackValueRoot;
        [SerializeField] private Image _slotBlockImage;
        [SerializeField] private TextMeshProUGUI _blockValueText;
        
        private int _slotIndex;
        
        public int SlotIndex => _slotIndex;

        private void Awake()
        {
            _slotButton.onClick.AddListener(OnSlotButtonClicked);
        }

        private void OnDestroy()
        {
            _slotButton.onClick.RemoveListener(OnSlotButtonClicked);
        }

        public void SetSlotIndex(int slotIndex)
        {
            _slotIndex = slotIndex;
        }

        public void BindClick(Action<int> onSlotClicked)
        {
            _onSlotClicked = onSlotClicked;
        }

        public void Refresh(InventorySlotViewState state)
        {
            if (state.IsLocked)
            {
                _slotBlockImage.gameObject.SetActive(true);
                
                if (state.ShowUnlockCost && state.UnlockCost > 0)
                    _blockValueText.text = state.UnlockCost.ToString();
                else
                    _blockValueText.text = string.Empty;
                
                SetIconAndStackVisible(false);
                
                return;
            }

            _slotBlockImage.gameObject.SetActive(false);
            
            if (state.IsEmpty)
            {
                SetIconAndStackVisible(false);
                
                return;
            }

            SetIconAndStackVisible(true);
            
            _iconItemImage.sprite = state.ItemSprite;
            _iconItemImage.enabled = state.ItemSprite != null;
            
            bool showStackAmount = state.StackAmount > 1;
            
            if (_stackValueRoot != null)
                _stackValueRoot.gameObject.SetActive(showStackAmount);
            else
                _stackValueText.gameObject.SetActive(showStackAmount);
            
            if (showStackAmount)
                _stackValueText.text = state.StackAmount.ToString();
        }

        private void SetIconAndStackVisible(bool isVisible)
        {
            if (_iconItemImage != null)
                _iconItemImage.gameObject.SetActive(isVisible);
            else
                _iconItemImage.enabled = isVisible && _iconItemImage.sprite != null;

            if (_stackValueRoot != null)
                _stackValueRoot.gameObject.SetActive(isVisible && false);
            else
                _stackValueText.gameObject.SetActive(false);
        }

        private void OnSlotButtonClicked()
        {
            _onSlotClicked?.Invoke(_slotIndex);
        }
    }
}