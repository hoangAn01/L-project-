using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace LilithChat
{
    public class ChatUI : MonoBehaviour
    {
        [Header("UI Refs")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private TMP_Text transcriptText;
        [SerializeField] private ScrollRect scrollRect;

        [Header("Integration")]
        [SerializeField] private ChatTypingIntegration typingIntegration;

        [Header("Typing Indicator")]
        [SerializeField] private GameObject typingIndicator;

        [Header("Display Settings")]
        [Tooltip("Maximum number of messages to show in transcript (A message is a pair of You/Lilith lines)")]
        [SerializeField] private int maxMessages = 20;
        [Tooltip("Auto-scroll to bottom when new message arrives")]
        [SerializeField] private bool autoScrollToBottom = true;

        // List to hold the history of messages
        private readonly List<string> m_MessageHistory = new List<string>();

        private void Awake()
        {
            if (sendButton != null)
            {
                sendButton.onClick.AddListener(OnSendClicked);
            }
            if (inputField != null)
            {
                inputField.onSubmit.AddListener(OnSubmit);
                inputField.onEndEdit.AddListener(OnEndEdit);
            }

            if (typingIntegration == null)
            {
                typingIntegration = FindObjectOfType<ChatTypingIntegration>();
            }
            SetupScrollView();

            SetTyping(false);
        }

        private void OnDestroy()
        {
            if (sendButton != null)
            {
                sendButton.onClick.RemoveListener(OnSendClicked);
            }
            if (inputField != null)
            {
                inputField.onSubmit.RemoveListener(OnSubmit);
                inputField.onEndEdit.RemoveListener(OnEndEdit);
            }
        }

        private void OnSubmit(string _)
        {
            OnSendClicked();
        }

        private void OnEndEdit(string value)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnSendClicked();
            }
        }

        private void OnSendClicked()
        {
            if (inputField == null || string.IsNullOrWhiteSpace(inputField.text))
            {
                return;
            }

            // Clear previous messages
            m_MessageHistory.Clear();
            if (transcriptText != null)
            {
                transcriptText.text = string.Empty;
            }

            // Use typing integration to display user message if available
            if (typingIntegration != null)
            {
                typingIntegration.DisplayUserMessage($"You: {inputField.text}");
            }

            var userMessage = inputField.text;
            inputField.text = string.Empty;
            inputField.ActivateInputField();

            SetTyping(true);
            ChatManager.Instance.SendMessageToLilith(
                userMessage,
                assistantText =>
                {
                    SetTyping(false);
                    // Use typing integration to display Lilith's message with typing effect
                    typingIntegration?.DisplayLilithMessage($"Lilith: {assistantText}");
                },
                error =>
                {
                    SetTyping(false);
                    AppendLine($"Error: {error}");
                }
            );
        }

        /// <summary>
        /// Appends a new line of text to the transcript.
        /// This will manage history and scrolling.
        /// </summary>
        /// <param name="text">The text to append.</param>
        public void AppendLine(string text)
        {
            if (transcriptText == null) return;

            m_MessageHistory.Add(text);

            // Limit history size
            if (maxMessages > 0 && m_MessageHistory.Count > maxMessages * 2) // Each interaction is ~2 lines
            {
                int removeCount = m_MessageHistory.Count - (maxMessages * 2);
                m_MessageHistory.RemoveRange(0, removeCount);
            }

            transcriptText.text = string.Join("\n", m_MessageHistory);

            // Auto-scroll to bottom
            if (autoScrollToBottom && scrollRect != null)
            {
                StartCoroutine(ScrollToBottomCoroutine());
            }
        }

        /// <summary>
        /// Appends text to the last line without creating a new line.
        /// Used for the typing effect.
        /// </summary>
        public void AppendToLastLine(string textToAppend)
        {
            if (transcriptText == null || m_MessageHistory.Count == 0) return;

            m_MessageHistory[m_MessageHistory.Count - 1] += textToAppend;
            transcriptText.text = string.Join("\n", m_MessageHistory);
            if (autoScrollToBottom && scrollRect != null)
            {
                StartCoroutine(ScrollToBottomCoroutine());
            }
        }

        /// <summary>
        /// Configures the ScrollView and its components for proper chat behavior.
        /// This replaces the various "Fixer" scripts.
        /// </summary>
        private void SetupScrollView()
        {
            if (scrollRect == null || transcriptText == null) return;

            // Ensure the viewport has a mask
            if (scrollRect.viewport && scrollRect.viewport.GetComponent<RectMask2D>() == null)
            {
                scrollRect.viewport.gameObject.AddComponent<RectMask2D>();
            }

            // Ensure the content has a ContentSizeFitter
            var content = scrollRect.content;
            if (content != null && content.GetComponent<ContentSizeFitter>() == null)
            {
                var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        private System.Collections.IEnumerator ScrollToBottomCoroutine()
        {
            // Wait for next frame to ensure layout is updated
            yield return null;
            
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        private void SetTyping(bool isTyping)
        {
            if (typingIndicator != null)
            {
                typingIndicator.SetActive(isTyping);
            }
        }

        [ContextMenu("Clear Transcript")]
        public void ClearTranscript()
        {
            if (transcriptText != null)
            {
                transcriptText.text = string.Empty;
            }
        }

        [ContextMenu("Scroll to Bottom")]
        public void ManualScrollToBottom()
        {
            if (scrollRect != null)
            {
                StartCoroutine(ScrollToBottomCoroutine());
            }
        }
    }
}
