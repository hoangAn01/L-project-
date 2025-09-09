using UnityEngine;
using TMPro;
using UnityEditor;

namespace LilithChat
{
    public class FontFixer : MonoBehaviour
    {
        [Header("Font Settings")]
        [SerializeField] private TMP_Text targetText;
        [SerializeField] private string fontName = "Arial";
        
        [Header("Character Sets")]
        [SerializeField] private bool includeVietnamese = true;
        [SerializeField] private bool includeEnglish = true;
        [SerializeField] private bool includeNumbers = true;
        [SerializeField] private bool includeSymbols = true;
        
        [Header("Auto Setup")]
        [SerializeField] private bool useSystemFont = true;
        
        private void Start()
        {
            if (targetText != null)
            {
                if (useSystemFont)
                {
                    ApplySystemFont();
                }
                else
                {
                    FixFontForVietnamese();
                }
            }
        }
        
        [ContextMenu("Fix Font for Vietnamese")]
        public void FixFontForVietnamese()
        {
            #if UNITY_EDITOR
            CreateVietnameseFontAsset();
            #else
            Debug.LogWarning("FontFixer: Font creation only works in Editor mode!");
            #endif
        }
        
        #if UNITY_EDITOR
        private void CreateVietnameseFontAsset()
        {
            // Find the font file
            string[] fontGuids = AssetDatabase.FindAssets(fontName + " t:Font");
            if (fontGuids.Length == 0)
            {
                Debug.LogError($"FontFixer: Could not find font '{fontName}'. Please check the font name.");
                return;
            }
            
            string fontPath = AssetDatabase.GUIDToAssetPath(fontGuids[0]);
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            
            if (sourceFont == null)
            {
                Debug.LogError($"FontFixer: Could not load font from path: {fontPath}");
                return;
            }
            
            // Create character set
            string characterSet = "";
            
            if (includeEnglish)
            {
                characterSet += "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            }
            
            if (includeNumbers)
            {
                characterSet += "0123456789";
            }
            
            if (includeSymbols)
            {
                characterSet += " .,!?;:()[]{}\"'`~@#$%^&*+-=_|\\/<>";
            }
            
            if (includeVietnamese)
            {
                // Vietnamese characters
                characterSet += "ÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴĐàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵđ";
            }
            
            // Create TMP Font Asset using simpler API
            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont);
            
            if (fontAsset != null)
            {
                // Save the font asset
                string assetPath = $"Assets/Fonts/{fontName}_Vietnamese.asset";
                AssetDatabase.CreateAsset(fontAsset, assetPath);
                AssetDatabase.SaveAssets();
                
                Debug.Log($"FontFixer: Created Vietnamese font asset at {assetPath}");
                
                // Apply to target text
                if (targetText != null)
                {
                    targetText.font = fontAsset;
                    Debug.Log("FontFixer: Applied Vietnamese font to target text");
                }
            }
            else
            {
                Debug.LogError("FontFixer: Failed to create font asset");
            }
        }
        #endif
        
        [ContextMenu("Apply System Font")]
        public void ApplySystemFont()
        {
            if (targetText != null)
            {
                // Try to find a system font that supports Vietnamese
                string[] systemFontNames = Font.GetOSInstalledFontNames();
                Font vietnameseFont = null;
                
                // Look for common fonts that support Vietnamese
                string[] vietnameseFontNames = {
                    "Arial", "Segoe UI", "Tahoma", "Verdana", "Times New Roman",
                    "Noto Sans", "Roboto", "Open Sans", "Source Sans Pro"
                };
                
                foreach (string fontName in vietnameseFontNames)
                {
                    if (System.Array.Exists(systemFontNames, f => f.Contains(fontName)))
                    {
                        vietnameseFont = Font.CreateDynamicFontFromOSFont(fontName, 26);
                        if (vietnameseFont != null)
                        {
                            Debug.Log($"FontFixer: Found system font: {fontName}");
                            break;
                        }
                    }
                }
                
                if (vietnameseFont != null)
                {
                    // Create a simple TMP font asset
                    #if UNITY_EDITOR
                    TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(vietnameseFont);
                    if (fontAsset != null)
                    {
                        string assetPath = $"Assets/Fonts/{vietnameseFont.name}_System.asset";
                        AssetDatabase.CreateAsset(fontAsset, assetPath);
                        AssetDatabase.SaveAssets();
                        
                        targetText.font = fontAsset;
                        Debug.Log($"FontFixer: Applied system font {vietnameseFont.name}");
                    }
                    #endif
                }
                else
                {
                    Debug.LogWarning("FontFixer: No suitable system font found for Vietnamese");
                }
            }
        }
        
        [ContextMenu("Manual Font Asset Creator")]
        public void OpenFontAssetCreator()
        {
            #if UNITY_EDITOR
            // Use Unity's menu system to open the Font Asset Creator
            EditorApplication.ExecuteMenuItem("Window/TextMeshPro/Font Asset Creator");
            Debug.Log("FontFixer: Opened Font Asset Creator window. Please select a font and set Character Set to 'Vietnamese'.");
            #endif
        }
    }
}
