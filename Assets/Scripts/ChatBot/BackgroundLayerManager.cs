using UnityEngine;
using UnityEngine.UI;

namespace LilithChat
{
    public class BackgroundLayerManager : MonoBehaviour
    {
        [Header("Background Settings")]
        [Tooltip("The UI Image component for the background.")]
        [SerializeField] private Image backgroundImage;
        [Tooltip("The sorting order for the background canvas. A lower number renders it further back.")]
        [SerializeField] private int backgroundSortingOrder = -1;
        [Tooltip("If checked, the background layer will be set up automatically on Start.")]
        [SerializeField] private bool autoSetupOnStart = true;
        
        [Header("UI Layer Settings")]
        [Tooltip("The main UI Canvas that contains other UI elements.")]
        [SerializeField] private Canvas mainCanvas;
        [Tooltip("The sorting order for the main UI canvas. Should be higher than the background's sorting order.")]
        [SerializeField] private int uiSortingOrder = 0;
        
        [Header("Debug")]
        [Tooltip("Show debug messages in the console.")]
        [SerializeField] private bool showDebugInfo = true;
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupBackgroundLayer();
            }
        }
        
        [ContextMenu("Setup Background Layer")]
        public void SetupBackgroundLayer()
        {
            // Find background image if not assigned (REMOVED FOR BETTER PRACTICE)
            if (backgroundImage == null)
            {
                Debug.LogError("BackgroundLayerManager: Background Image is not assigned in the Inspector.", this);
                return;
            }
            
            // Find main canvas if not assigned (REMOVED FOR BETTER PRACTICE)
            if (mainCanvas == null)
            {
                Debug.LogError("BackgroundLayerManager: Main Canvas is not assigned in the Inspector.", this);
                return;
            }
            
            if (backgroundImage != null)
            {
                SetupBackgroundSorting();
                if (showDebugInfo)
                {
                    Debug.Log($"BackgroundLayerManager: Successfully set up background '{backgroundImage.name}' with sorting order {backgroundSortingOrder}.", this);
                }
            }
            else
            {
                Debug.LogWarning("BackgroundLayerManager: Không tìm thấy background image!");
            }
        }
        
        private void SetupBackgroundSorting()
        {
            if (backgroundImage == null) return;
            
            // 1. Set background as the first sibling in the hierarchy (to render behind)
            backgroundImage.transform.SetAsFirstSibling();
            
            // 2. Set a lower sorting order
            Canvas backgroundCanvas = backgroundImage.GetComponent<Canvas>();
            if (backgroundCanvas == null)
            {
                backgroundCanvas = backgroundImage.gameObject.AddComponent<Canvas>();
            }
            backgroundCanvas.overrideSorting = true;
            backgroundCanvas.sortingOrder = backgroundSortingOrder;
            
            // 3. Ensure the background has a suitable RectTransform
            RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
            if (bgRect != null)
            {
                // Stretch to fill
                bgRect.anchorMin = Vector2.zero;
                bgRect.anchorMax = Vector2.one;
                bgRect.offsetMin = Vector2.zero;
                bgRect.offsetMax = Vector2.zero;
            }
            
            // 4. Set the main canvas to have a higher sorting order
            if (mainCanvas != null)
            {
                mainCanvas.overrideSorting = true;
                mainCanvas.sortingOrder = uiSortingOrder;
            }
        }
        
        [ContextMenu("Move Background to Back")]
        public void MoveBackgroundToBack()
        {
            if (backgroundImage != null)
            {
                backgroundImage.transform.SetAsFirstSibling();
                Debug.Log("BackgroundLayerManager: Moved background to back.");
            }
        }
        
        [ContextMenu("Move Background to Front")]
        public void MoveBackgroundToFront()
        {
            if (backgroundImage != null)
            {
                backgroundImage.transform.SetAsLastSibling();
                Debug.Log("BackgroundLayerManager: Moved background to front.");
            }
        }
        
        [ContextMenu("Set Background Sorting Order")]
        public void SetBackgroundSortingOrder()
        {
            if (backgroundImage != null)
            {
                Canvas bgCanvas = backgroundImage.GetComponent<Canvas>();
                if (bgCanvas == null)
                {
                    bgCanvas = backgroundImage.gameObject.AddComponent<Canvas>();
                }
                bgCanvas.overrideSorting = true;
                bgCanvas.sortingOrder = backgroundSortingOrder;
                Debug.Log($"BackgroundLayerManager: Set background sorting order to {backgroundSortingOrder}");
            }
        }
        
        [ContextMenu("Show Layer Info")]
        public void ShowLayerInfo()
        {
            if (!showDebugInfo) return;
            
            Debug.Log("=== LAYER INFO ===");
            
            if (backgroundImage != null)
            {
                Canvas bgCanvas = backgroundImage.GetComponent<Canvas>();
                Debug.Log($"Background: {backgroundImage.name}");
                Debug.Log($"  - Sibling Index: {backgroundImage.transform.GetSiblingIndex()}");
                Debug.Log($"  - Has Canvas: {bgCanvas != null}");
                if (bgCanvas != null)
                {
                    Debug.Log($"  - Sorting Order: {bgCanvas.sortingOrder}");
                    Debug.Log($"  - Override Sorting: {bgCanvas.overrideSorting}");
                }
            }
            
            if (mainCanvas != null)
            {
                Debug.Log($"Main Canvas: {mainCanvas.name}");
                Debug.Log($"  - Sorting Order: {mainCanvas.sortingOrder}");
                Debug.Log($"  - Override Sorting: {mainCanvas.overrideSorting}");
                Debug.Log($"  - Render Mode: {mainCanvas.renderMode}");
            }
            
            // Show info about all UI elements
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            Debug.Log($"Total Canvases: {allCanvases.Length}");
            
            Image[] allImages = FindObjectsOfType<Image>();
            Debug.Log($"Total Images: {allImages.Length}");
        }
        
        private void OnValidate()
        {
            // Automatically update when values are changed in the Inspector
            if (Application.isPlaying && backgroundImage != null)
            {
                SetupBackgroundSorting();
            }
        }
    }
}
