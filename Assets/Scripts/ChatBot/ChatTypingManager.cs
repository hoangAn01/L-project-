using UnityEngine;
using TMPro;
using System.Collections;

namespace LilithChat
{
    public class ChatTypingManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text transcriptText;
        [SerializeField] private ChatUI chatUI;
        [SerializeField] private TextTypingEffect typingEffect;
        
        [Header("Typing Configuration")]
        [SerializeField] private bool enableTypingEffect = true;
        [SerializeField] private float defaultTypingSpeed = 0.05f;
        [SerializeField] private float fastTypingSpeed = 0.02f;
        [SerializeField] private bool showTypingIndicator = true;
        
        [Header("Typing Indicator")]
        [SerializeField] private GameObject typingIndicator;
        [SerializeField] private string[] typingDots = { ".", "..", "..." };
        
        private Coroutine typingIndicatorCoroutine;
        private bool isTyping = false;
        
        // Events
        public System.Action OnTypingStarted;
        public System.Action OnTypingCompleted;
        
        private void Start()
        {
            // Tự động tìm references nếu chưa được gán
            if (transcriptText == null)
            {
                transcriptText = FindObjectOfType<TMP_Text>();
            }

            if (chatUI == null)
            {
                chatUI = FindObjectOfType<ChatUI>();
            }
            
            if (typingEffect == null)
            {
                typingEffect = GetComponent<TextTypingEffect>();
                if (typingEffect == null)
                {
                    typingEffect = gameObject.AddComponent<TextTypingEffect>();
                }
            }
            
            // Thiết lập typing effect
            if (typingEffect != null)
            {
                typingEffect.SetTargetText(transcriptText);
                typingEffect.SetTypingSpeed(defaultTypingSpeed);
                
                // Đăng ký events
                typingEffect.OnTypingStarted += OnTypingEffectStarted;
                typingEffect.OnTypingCompleted += OnTypingEffectCompleted;
            }
            
            // Ẩn typing indicator ban đầu
            if (typingIndicator != null)
            {
                typingIndicator.SetActive(false);
            }
        }
        
        /// <summary>
        /// Hiển thị tin nhắn với typing effect (nếu được bật)
        /// </summary>
        public void DisplayMessageWithTyping(string message, bool isLilithMessage = true)
        {
            if (!enableTypingEffect || !isLilithMessage)
            {
                if (transcriptText != null)
                {
                    transcriptText.text = message;
                }
                return;
            }

            // Bắt đầu typing effect
            if (typingEffect != null && typingEffect.HasTargetText())
            {
                typingEffect.StartTyping(message);
            }
            else if (transcriptText != null)
            {
                // Fallback: hiển thị bình thường nếu không có typing effect
                transcriptText.text = message;
            }
        }
        
        /// <summary>
        /// Hiển thị tin nhắn với tốc độ typing nhanh
        /// </summary>
        public void DisplayMessageFast(string message)
        {
            if (typingEffect != null && typingEffect.HasTargetText())
            {
                typingEffect.SetTypingSpeed(fastTypingSpeed);
                typingEffect.StartTyping(message);
            }
            else if (transcriptText != null)
            {
                transcriptText.text = message;
            }
        }
        
        /// <summary>
        /// Hiển thị tin nhắn với tốc độ typing chậm
        /// </summary>
        public void DisplayMessageSlow(string message)
        {
            if (typingEffect != null && typingEffect.HasTargetText())
            {
                typingEffect.SetTypingSpeed(defaultTypingSpeed);
                typingEffect.StartTyping(message);
            }
            else if (transcriptText != null)
            {
                transcriptText.text = message;
            }
        }
        
        /// <summary>
        /// Bỏ qua typing hiện tại
        /// </summary>
        public void SkipCurrentTyping()
        {
            if (typingEffect != null)
            {
                typingEffect.SkipTyping();
            }
        }
        
        /// <summary>
        /// Hiển thị typing indicator
        /// </summary>
        public void ShowTypingIndicator()
        {
            if (!showTypingIndicator || typingIndicator == null) return;
            
            typingIndicator.SetActive(true);
            if (typingIndicatorCoroutine != null)
            {
                StopCoroutine(typingIndicatorCoroutine);
            }
            typingIndicatorCoroutine = StartCoroutine(AnimateTypingIndicator());
        }
        
        /// <summary>
        /// Ẩn typing indicator
        /// </summary>
        public void HideTypingIndicator()
        {
            if (typingIndicator != null)
            {
                typingIndicator.SetActive(false);
            }
            
            if (typingIndicatorCoroutine != null)
            {
                StopCoroutine(typingIndicatorCoroutine);
                typingIndicatorCoroutine = null;
            }
        }
        
        /// <summary>
        /// Thêm tin nhắn vào transcript với typing effect
        /// </summary>
        public void AppendMessageWithTyping(string message, bool isLilithMessage = true)
        {
            if (!enableTypingEffect || !isLilithMessage)
            {
                // Thêm ngay lập tức
                if (transcriptText != null)
                {
                    transcriptText.text += "\n" + message;
                }
                return;
            }
            
            // Thêm tin nhắn mới vào cuối
            string currentText = transcriptText != null ? transcriptText.text : "";
            string newText = string.IsNullOrEmpty(currentText) ? message : currentText + "\n" + message;
            
            // Bắt đầu typing effect với text mới
            if (typingEffect != null && typingEffect.HasTargetText())
            {
                typingEffect.StartTyping(newText);
            }
            else if (transcriptText != null)
            {
                transcriptText.text = newText;
            }
        }
        
        /// <summary>
        /// Xóa toàn bộ text và bắt đầu typing mới
        /// </summary>
        public void ClearAndType(string message)
        {
            if (typingEffect != null && typingEffect.HasTargetText())
            {
                if (transcriptText != null)
                {
                    transcriptText.text = "";
                }
                typingEffect.StartTyping(message);
            }
            else if (transcriptText != null)
            {
                transcriptText.text = message;
            }
        }
        
        // Event handlers
        private void OnTypingEffectStarted()
        {
            isTyping = true;
            OnTypingStarted?.Invoke();
        }
        
        private void OnTypingEffectCompleted()
        {
            isTyping = false;
            OnTypingCompleted?.Invoke();
        }
        
        /// <summary>
        /// Coroutine để animate typing indicator
        /// </summary>
        private IEnumerator AnimateTypingIndicator()
        {
            int dotIndex = 0;
            
            while (typingIndicator.activeInHierarchy)
            {
                if (typingIndicator.GetComponentInChildren<TMP_Text>() is TMP_Text indicatorText)
                {
                    indicatorText.text = "Lilith đang nhập" + typingDots[dotIndex];
                }
                
                dotIndex = (dotIndex + 1) % typingDots.Length;
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        /// <summary>
        /// Kiểm tra xem có đang typing không
        /// </summary>
        public bool IsTyping => isTyping;
        
        /// <summary>
        /// Bật/tắt typing effect
        /// </summary>
        public void SetTypingEffectEnabled(bool enabled)
        {
            enableTypingEffect = enabled;
        }
        
        /// <summary>
        /// Thiết lập tốc độ typing mặc định
        /// </summary>
        public void SetDefaultTypingSpeed(float speed)
        {
            defaultTypingSpeed = speed;
            if (typingEffect != null)
            {
                typingEffect.SetTypingSpeed(speed);
            }
        }
        
        /// <summary>
        /// Kiểm tra xem typing effect có sẵn sàng không
        /// </summary>
        public bool IsTypingEffectReady()
        {
            return enableTypingEffect && typingEffect != null && typingEffect.HasTargetText();
        }
        
        /// <summary>
        /// Thiết lập transcript text
        /// </summary>
        public void SetTranscriptText(TMP_Text text)
        {
            transcriptText = text;
            if (typingEffect != null)
            {
                typingEffect.SetTargetText(text);
            }
        }
        
        // Context menu để test
        [ContextMenu("Test Typing Effect")]
        public void TestTypingEffect()
        {
            string testMessage = "Xin chào! Tôi là Lilith... [mỉm cười nhẹ nhàng]\nTôi rất vui được gặp bạn hôm nay.\nBạn có muốn trò chuyện với tôi không?";
            DisplayMessageWithTyping(testMessage, true);
        }
        
        [ContextMenu("Test Fast Typing")]
        public void TestFastTyping()
        {
            string testMessage = "Đây là tin nhắn được gõ nhanh!";
            DisplayMessageFast(testMessage);
        }
        
        [ContextMenu("Test Typing Indicator")]
        public void TestTypingIndicator()
        {
            ShowTypingIndicator();
            StartCoroutine(TestTypingIndicatorCoroutine());
        }
        
        private IEnumerator TestTypingIndicatorCoroutine()
        {
            yield return new WaitForSeconds(3f);
            HideTypingIndicator();
        }
        
        private void OnDestroy()
        {
            // Hủy đăng ký events
            if (typingEffect != null)
            {
                typingEffect.OnTypingStarted -= OnTypingEffectStarted;
                typingEffect.OnTypingCompleted -= OnTypingEffectCompleted;
            }
        }
    }
}
