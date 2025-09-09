using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace LilithChat
{
    public class TextTypingEffect : MonoBehaviour
    {
        [Header("Typing Settings")]
        [SerializeField] public TMP_Text targetText;
        [SerializeField] private float typingSpeed = 0.05f; // Thời gian giữa mỗi ký tự (giây)
        [SerializeField] private float pauseAtPunctuation = 0.3f; // Tạm dừng ở dấu câu
        [SerializeField] private bool autoStart = false;
        [SerializeField] private bool skipOnClick = true; // Bỏ qua typing khi click
        
        [Header("Audio (Optional)")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip typingSound;
        [SerializeField] private float typingVolume = 0.3f;
        
        [Header("Visual Effects")]
        [SerializeField] private bool showCursor = true;
        [SerializeField] private string cursorChar = "|";
        [SerializeField] private float cursorBlinkSpeed = 0.5f;
        
        private string fullText = "";
        private string currentText = "";
        private Coroutine typingCoroutine;
        private Coroutine cursorCoroutine;
        private bool isTyping = false;
        private bool cursorVisible = true;
        
        // Events
        public System.Action OnTypingStarted;
        public System.Action OnTypingCompleted;
        public System.Action OnTypingSkipped;
        
        private void Start()
        {
            if (targetText == null)
            {
                targetText = GetComponent<TMP_Text>();
            }
            
            if (autoStart && !string.IsNullOrEmpty(targetText.text))
            {
                StartTyping(targetText.text);
            }
        }
        
        private void Update()
        {
            // Bỏ qua typing khi click chuột hoặc phím Space
            if (skipOnClick && isTyping && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
            {
                SkipTyping();
            }
        }
        
        /// <summary>
        /// Bắt đầu hiệu ứng typing với text mới
        /// </summary>
        public void StartTyping(string text)
        {
            if (isTyping)
            {
                StopTyping();
            }
            
            fullText = text;
            currentText = "";
            isTyping = true;
            
            OnTypingStarted?.Invoke();
            
            // Bắt đầu typing coroutine
            typingCoroutine = StartCoroutine(TypeText());
            
            // Bắt đầu cursor blinking
            if (showCursor)
            {
                cursorCoroutine = StartCoroutine(BlinkCursor());
            }
        }
        
        /// <summary>
        /// Dừng typing và hiển thị toàn bộ text
        /// </summary>
        public void SkipTyping()
        {
            if (!isTyping) return;
            
            StopTyping();
            currentText = fullText;
            targetText.text = currentText;
            
            OnTypingSkipped?.Invoke();
            OnTypingCompleted?.Invoke();
        }
        
        /// <summary>
        /// Dừng typing
        /// </summary>
        public void StopTyping()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            
            if (cursorCoroutine != null)
            {
                StopCoroutine(cursorCoroutine);
                cursorCoroutine = null;
            }
            
            isTyping = false;
            cursorVisible = false;
        }
        
        /// <summary>
        /// Coroutine để thực hiện typing effect
        /// </summary>
        private IEnumerator TypeText()
        {
            for (int i = 0; i < fullText.Length; i++)
            {
                currentText += fullText[i];
                
                // Hiển thị text với cursor
                if (showCursor && cursorVisible)
                {
                    targetText.text = currentText + cursorChar;
                }
                else
                {
                    targetText.text = currentText;
                }
                
                // Phát âm thanh typing
                if (audioSource != null && typingSound != null)
                {
                    audioSource.PlayOneShot(typingSound, typingVolume);
                }
                
                // Tạm dừng ở dấu câu
                float delay = typingSpeed;
                if (IsPunctuation(fullText[i]))
                {
                    delay = pauseAtPunctuation;
                }
                
                yield return new WaitForSeconds(delay);
            }
            
            // Hoàn thành typing
            isTyping = false;
            targetText.text = currentText;
            
            OnTypingCompleted?.Invoke();
        }
        
        /// <summary>
        /// Coroutine để nhấp nháy cursor
        /// </summary>
        private IEnumerator BlinkCursor()
        {
            while (isTyping)
            {
                cursorVisible = !cursorVisible;
                
                if (cursorVisible)
                {
                    targetText.text = currentText + cursorChar;
                }
                else
                {
                    targetText.text = currentText;
                }
                
                yield return new WaitForSeconds(cursorBlinkSpeed);
            }
        }
        
        /// <summary>
        /// Kiểm tra xem ký tự có phải là dấu câu không
        /// </summary>
        private bool IsPunctuation(char c)
        {
            return c == '.' || c == ',' || c == '!' || c == '?' || c == ':' || c == ';' || 
                   c == '。' || c == '，' || c == '！' || c == '？' || c == '：' || c == '；';
        }
        
        /// <summary>
        /// Thiết lập tốc độ typing
        /// </summary>
        public void SetTypingSpeed(float speed)
        {
            typingSpeed = Mathf.Max(0.01f, speed);
        }
        
        /// <summary>
        /// Kiểm tra xem có đang typing không
        /// </summary>
        public bool IsTyping => isTyping;
        
        /// <summary>
        /// Lấy text hiện tại đang được hiển thị
        /// </summary>
        public string GetCurrentText => currentText;
        
        /// <summary>
        /// Lấy text đầy đủ
        /// </summary>
        public string GetFullText => fullText;
        
        /// <summary>
        /// Property để truy cập targetText
        /// </summary>
        public TMP_Text TargetText
        {
            get => targetText;
            set => targetText = value;
        }
        
        /// <summary>
        /// Thiết lập target text
        /// </summary>
        public void SetTargetText(TMP_Text text)
        {
            targetText = text;
        }
        
        /// <summary>
        /// Kiểm tra xem target text có được thiết lập chưa
        /// </summary>
        public bool HasTargetText()
        {
            return targetText != null;
        }
        
        // Context menu để test
        [ContextMenu("Test Typing Effect")]
        public void TestTypingEffect()
        {
            string testText = "Xin chào! Tôi là Lilith... [mỉm cười nhẹ nhàng] Tôi rất vui được gặp bạn hôm nay. Bạn có muốn trò chuyện với tôi không?";
            StartTyping(testText);
        }
        
        [ContextMenu("Skip Current Typing")]
        public void SkipCurrentTyping()
        {
            SkipTyping();
        }
    }
}
