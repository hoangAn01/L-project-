using UnityEngine;
using TMPro;

namespace LilithChat
{
    public class ChatTypingIntegration : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChatTypingManager typingManager;
        [SerializeField] private ChatUI chatUI; // Keep this reference
        
        [Header("Integration Settings")]
        [SerializeField] private bool enableTypingForLilith = true;
        [SerializeField] private bool enableTypingForUser = false;
        
        private void Start()
        {
            // Auto-find references if not assigned
            if (typingManager == null)
            {
                typingManager = FindObjectOfType<ChatTypingManager>();
            }
            
            if (chatUI == null)
            {
                chatUI = FindObjectOfType<ChatUI>();
            }
        }
        
        /// <summary>
        /// Displays Lilith's message with a typing effect.
        /// </summary>
        public void DisplayLilithMessage(string message)
        {
            if (enableTypingForLilith && typingManager != null && typingManager.IsTypingEffectReady())
            {
                typingManager.AppendMessageWithTyping(message, true);
            }
            else
            {
                chatUI?.AppendLine(message);
            }
        }
        
        /// <summary>
        /// Displays the User's message.
        /// </summary>
        public void DisplayUserMessage(string message)
        {
            if (enableTypingForUser && typingManager != null && typingManager.IsTypingEffectReady())
            {
                typingManager.AppendMessageWithTyping(message, false);
            }
            else
            {
                chatUI?.AppendLine(message);
            }
        }
        
        /// <summary>
        /// Skips the current typing effect.
        /// </summary>
        public void SkipCurrentTyping()
        {
            if (typingManager != null)
            {
                typingManager.SkipCurrentTyping();
            }
        }
        
        public void ClearTranscript()
        {
            if (chatUI != null)
            {
                chatUI.ClearTranscript();
            }
        }
        
        /// <summary>
        /// Checks if the typing effect is currently active.
        /// </summary>
        public bool IsTyping()
        {
            return typingManager != null && typingManager.IsTyping;
        }
        
        /// <summary>
        /// Enables or disables the typing effect for Lilith's messages.
        /// </summary>
        public void SetTypingForLilith(bool enabled)
        {
            enableTypingForLilith = enabled;
        }
        
        /// <summary>
        /// Enables or disables the typing effect for the user's messages.
        /// </summary>
        public void SetTypingForUser(bool enabled)
        {
            enableTypingForUser = enabled;
        }
        
        /// <summary>
        /// Sets the typing speed.
        /// </summary>
        public void SetTypingSpeed(float speed)
        {
            if (typingManager != null)
            {
                typingManager.SetDefaultTypingSpeed(speed);
            }
        }
        
        // Context menu for testing
        [ContextMenu("Test Lilith Message")]
        public void TestLilithMessage()
        {
            string testMessage = "Xin chào! Tôi là Lilith... [mỉm cười nhẹ nhàng]\nTôi rất vui được gặp bạn hôm nay.";
            DisplayLilithMessage($"Lilith: {testMessage}");
        }
    }
}
