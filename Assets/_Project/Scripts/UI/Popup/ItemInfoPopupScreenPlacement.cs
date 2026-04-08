using UnityEngine;

namespace _Project.UI
{
    public class ItemInfoPopupScreenPlacement
    {
        private const float ANCHOR_CENTER_COORDINATE = 0.5f;
        private const float SCREEN_POINT_CENTER_BLEND_FACTOR = 0.5f;
        private const int RECT_TRANSFORM_CORNER_COUNT = 4;
        private const int BOTTOM_LEFT_WORLD_CORNER_INDEX = 0;
        private const int TOP_RIGHT_WORLD_CORNER_INDEX = 2;
        private const float PIVOT_AXIS_MIN = 0f;
        private const float PIVOT_AXIS_MAX = 1f;
        private const float SCREEN_DELTA_SQR_MAGNITUDE_EPSILON = 0.0001f;

        public void PlacePopupAtSlot(
            RectTransform popupRectTransform,
            RectTransform placementParentRectTransform,
            RectTransform slotRectTransform,
            Camera canvasCamera,
            float screenPaddingPixels)
        {
            Vector2 screenPoint = GetScreenPointForSlot(slotRectTransform, canvasCamera);
            Rect safeArea = Screen.safeArea;
            Vector2 pivot = CalculatePivotFromScreenPointAndSafeArea(screenPoint, safeArea);
            popupRectTransform.pivot = pivot;
            popupRectTransform.anchorMin = new Vector2(ANCHOR_CENTER_COORDINATE, ANCHOR_CENTER_COORDINATE);
            popupRectTransform.anchorMax = new Vector2(ANCHOR_CENTER_COORDINATE, ANCHOR_CENTER_COORDINATE);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                placementParentRectTransform,
                screenPoint,
                canvasCamera,
                out Vector2 localPoint);
            popupRectTransform.SetParent(placementParentRectTransform, false);
            popupRectTransform.localRotation = Quaternion.identity;
            popupRectTransform.localScale = Vector3.one;
            popupRectTransform.anchoredPosition = localPoint;
            
            Canvas.ForceUpdateCanvases();
            
            ClampPopupInsideSafeArea(
                popupRectTransform,
                placementParentRectTransform,
                canvasCamera,
                safeArea,
                screenPaddingPixels);
        }

        private Vector2 GetScreenPointForSlot(RectTransform slotRectTransform, Camera canvasCamera)
        {
            Vector3[] worldCorners = new Vector3[RECT_TRANSFORM_CORNER_COUNT];
            slotRectTransform.GetWorldCorners(worldCorners);
            
            Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(
                canvasCamera,
                worldCorners[BOTTOM_LEFT_WORLD_CORNER_INDEX]);
            Vector2 topRight = RectTransformUtility.WorldToScreenPoint(
                canvasCamera,
                worldCorners[TOP_RIGHT_WORLD_CORNER_INDEX]);
            
            return new Vector2(
                (bottomLeft.x + topRight.x) * SCREEN_POINT_CENTER_BLEND_FACTOR,
                (bottomLeft.y + topRight.y) * SCREEN_POINT_CENTER_BLEND_FACTOR);
        }

        private Vector2 CalculatePivotFromScreenPointAndSafeArea(Vector2 screenPoint, Rect safeArea)
        {
            Vector2 safeCenter = safeArea.center;
            float pivotX = screenPoint.x < safeCenter.x ? PIVOT_AXIS_MIN : PIVOT_AXIS_MAX;
            float pivotY = screenPoint.y < safeCenter.y ? PIVOT_AXIS_MIN : PIVOT_AXIS_MAX;
            
            return new Vector2(pivotX, pivotY);
        }

        private void ClampPopupInsideSafeArea(
            RectTransform popupRectTransform,
            RectTransform placementParentRectTransform,
            Camera canvasCamera,
            Rect safeArea,
            float screenPaddingPixels)
        {
            Vector3[] corners = new Vector3[RECT_TRANSFORM_CORNER_COUNT];
            popupRectTransform.GetWorldCorners(corners);
            
            float minScreenX = float.PositiveInfinity;
            float maxScreenX = float.NegativeInfinity;
            float minScreenY = float.PositiveInfinity;
            float maxScreenY = float.NegativeInfinity;
            
            for (int i = 0; i < RECT_TRANSFORM_CORNER_COUNT; i++)
            {
                Vector2 screen = RectTransformUtility.WorldToScreenPoint(canvasCamera, corners[i]);
                minScreenX = Mathf.Min(minScreenX, screen.x);
                maxScreenX = Mathf.Max(maxScreenX, screen.x);
                minScreenY = Mathf.Min(minScreenY, screen.y);
                maxScreenY = Mathf.Max(maxScreenY, screen.y);
            }

            float safeMinX = safeArea.xMin + screenPaddingPixels;
            float safeMaxX = safeArea.xMax - screenPaddingPixels;
            float safeMinY = safeArea.yMin + screenPaddingPixels;
            float safeMaxY = safeArea.yMax - screenPaddingPixels;
            Vector2 screenDelta = Vector2.zero;
            
            if (minScreenX < safeMinX)
                screenDelta.x += safeMinX - minScreenX;
            
            if (maxScreenX > safeMaxX)
                screenDelta.x -= maxScreenX - safeMaxX;
            
            if (minScreenY < safeMinY)
                screenDelta.y += safeMinY - minScreenY;
            
            if (maxScreenY > safeMaxY)
                screenDelta.y -= maxScreenY - safeMaxY;
            
            if (screenDelta.sqrMagnitude < SCREEN_DELTA_SQR_MAGNITUDE_EPSILON)
                return;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                placementParentRectTransform,
                screenDelta,
                canvasCamera,
                out Vector2 localDelta);
            
            popupRectTransform.anchoredPosition += localDelta;
        }
    }
}