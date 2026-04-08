using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI
{
    public sealed class InventoryDragGhost : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image _image;
        
        private RectTransform _canvasRectTransform;
        private Camera _uiCamera;

        private void Awake()
        {
            _rectTransform ??= GetComponent<RectTransform>();
            _canvas ??= GetComponentInParent<Canvas>();
            _image ??= GetComponentInChildren<Image>(true);
            _canvasGroup ??= GetComponent<CanvasGroup>();
            
            _image?.gameObject.SetActive(false);
        }
        
        public void Show(Sprite sprite, Vector2 screenPosition)
        {
            if (_canvas == null || _rectTransform == null)
                return;
            
            if (_image != null)
            {
                _image.sprite = sprite;
                _image.enabled = sprite != null;
                _image.gameObject.SetActive(true);
            }
            
            if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                _uiCamera = null;
            else
                _uiCamera = _canvas.worldCamera;
            
            _canvasRectTransform = _canvas.transform as RectTransform;
            
            UpdatePosition(screenPosition);
        }

        public void Hide()
        {
            if (_image != null)
            {
                _image.gameObject.SetActive(false);
                _image.sprite = null;
            }
        }

        public void UpdatePosition(Vector2 screenPosition)
        {
            if (_canvasRectTransform == null && _canvas != null)
                _canvasRectTransform = _canvas.transform as RectTransform;
            
            if (_rectTransform == null || _canvasRectTransform == null)
                return;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRectTransform,
                screenPosition,
                _uiCamera,
                out Vector2 localPoint);
            
            _rectTransform.localPosition = localPoint;
        }
        public void SetRaycastBlocking(bool blocksRaycasts)
        {
            if (_canvasGroup != null)
                _canvasGroup.blocksRaycasts = blocksRaycasts;
        }
    }
}