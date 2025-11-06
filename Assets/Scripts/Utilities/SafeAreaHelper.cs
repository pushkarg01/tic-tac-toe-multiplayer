using UnityEngine;
using static UnityEngine.Screen;


namespace W3.Tambola.Utils
{
    public class SafeAreaHelper : MonoBehaviour
    {
        public static bool IsBottomSafeAreaFound;
        public static Vector2 ScreenCenterPoint;

        public bool UpdateContinuously;
        private RectTransform _panel;
        private void Awake()
        {
            _panel = GetComponent<RectTransform>();

            //#if UNITY_IOS
            ApplySafeArea();
            ScreenCenterPoint = new Vector2(width / 2, height / 2);

            //#endif
        }

        private void Update()
        {
            if (UpdateContinuously)
                ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= width;
            anchorMin.y /= height;
            anchorMax.x /= width;
            anchorMax.y /= height;
            _panel.anchorMin = anchorMin;
            _panel.anchorMax = anchorMax;


            if (anchorMin.sqrMagnitude > 0f)
                IsBottomSafeAreaFound = true;
            else
                IsBottomSafeAreaFound = false;
        }
    }
}