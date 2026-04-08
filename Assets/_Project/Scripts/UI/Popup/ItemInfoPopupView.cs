using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Project.UI
{
    public sealed class ItemInfoPopupView : MonoBehaviour
    {
        private const float SCREEN_PADDING_PIXELS = 8f;
        
        private const string WEIGHT_LABEL = "Weight";
        private const string TYPE_LABEL = "Type";
        private const string AMOUNT_LABEL = "Amount";
        private const string PROTECTION_LABEL = "Protection";
        private const string DAMAGE_LABEL = "Damage";
        private const string AMMO_LABEL = "Ammo";
        
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _popupRootRectTransform;
        [SerializeField] private RectTransform _placementParentRectTransform;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _bodyText;
        [SerializeField] private Button _closeButton;

        private readonly ItemInfoPopupScreenPlacement _itemInfoPopupScreenPlacement = new ItemInfoPopupScreenPlacement();
        
        private Canvas _rootCanvas;
        
        private void Awake()
        {
            _rootCanvas ??= GetComponentInParent<Canvas>().rootCanvas;
            _popupRootRectTransform ??= GetComponent<RectTransform>();
            
            _closeButton?.onClick.AddListener(Hide);
            
            Hide();
        }

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveListener(Hide);
        }

        public void Show(ItemPopupData data, RectTransform slotRectTransform)
        {
            _titleText.text = data.title;
            _bodyText.text = BuildBodyText(data);
            
            Camera canvasCamera = _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null 
                : _rootCanvas.worldCamera;
            _itemInfoPopupScreenPlacement.PlacePopupAtSlot(
                _popupRootRectTransform,
                _placementParentRectTransform,
                slotRectTransform,
                canvasCamera,
                SCREEN_PADDING_PIXELS);
            
            SetVisible(true);
        }
        
        private string BuildBodyText(ItemPopupData data)
        {
            string weight = data.weight.ToString("0.###", CultureInfo.InvariantCulture);
            string text =
                $"{TYPE_LABEL}: {data.type}\n" +
                $"{WEIGHT_LABEL}: {weight}\n" +
                $"{AMOUNT_LABEL}: {data.amount}\n";
            
            if (data.hasProtection)
                text += $"{PROTECTION_LABEL}: {data.protection}\n";
            
            if (data.hasWeaponStats)
            {
                text += $"{DAMAGE_LABEL}: {data.damage}\n";
                text += $"{AMMO_LABEL}: {data.ammoId}\n";
            }

            return text;
        }

        private void Hide()
        {
            SetVisible(false);
        }

        private void SetVisible(bool isVisible)
        {
            if (_canvasGroup == null)
            {
                gameObject.SetActive(isVisible);
                
                return;
            }

            _canvasGroup.alpha = isVisible ? 1f : 0f;
            _canvasGroup.interactable = isVisible;
            _canvasGroup.blocksRaycasts = isVisible;
        }
    }
}