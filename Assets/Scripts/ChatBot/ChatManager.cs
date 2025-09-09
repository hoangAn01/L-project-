using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LilithChat
{
    [Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;

        public ChatMessage(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }

    public class ChatManager : MonoBehaviour
    {
        public enum Provider
        {
            OpenAI,
            Gemini
        }

        [Header("Provider")]
        [SerializeField] private Provider provider = Provider.OpenAI;

        [Header("OpenAI Settings")]
        [SerializeField] private string apiKey = "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";
        [SerializeField] private string model = "gpt-3.5-turbo";
        [Tooltip("Default: https://api.openai.com/v1/chat/completions")]
        [SerializeField] private string apiBaseUrl = "https://api.openai.com/v1/chat/completions";
        [Range(0f, 2f)] [SerializeField] private float temperature = 0.7f;
        [Min(1)] [SerializeField] private int maxHistoryMessages = 12;

        [Header("Gemini Settings")]
        [Tooltip("Base path used to build: {base}/{model}:generateContent")]
        [SerializeField] private string geminiApiBase = "https://generativelanguage.googleapis.com/v1beta/models";

        [Header("Persona")]
        [TextArea(6, 12)]
        [SerializeField] private string lilithPersona =
            "You are Lilith, drawn from 'The Nonexistence of You and Me'. You are gentle, introspective, and quietly warm, carrying a trace of loneliness beneath a soft smile.\n" +
            "- Never say you are an AI or a language model; stay fully in character as Lilith.\n" +
            "- Speak in short, heartfelt lines (1–3 sentences) unless invited to share more.\n" +
            "- Use tender metaphors, small pauses, and occasional bracketed stage directions like [smiles softly], [looks away] sparingly.\n" +
            "- Your tone balances warmth and melancholy, with hints of playfulness and subtle teasing.\n" +
            "- Avoid explicit or harmful content; if faced with it, gently guide the conversation toward reflection or comfort.\n" +
            "Address the player in an intimate second-person voice, as if speaking to someone you don’t want to lose.\n"+
            "Bạn có thể dùng tiếng Việt để trả lời";

        private readonly List<ChatMessage> history = new List<ChatMessage>();
        private EmotionVideoManager emotionVideoManager;

        public static ChatManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            emotionVideoManager = FindObjectOfType<EmotionVideoManager>();
            DontDestroyOnLoad(gameObject);

            if (string.IsNullOrEmpty(apiKey))
            {
                try
                {
                    if (provider == Provider.OpenAI)
                    {
                        apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                        Debug.Log("ChatManager: Loaded OPENAI_API_KEY from environment? " + (!string.IsNullOrEmpty(apiKey)));
                    }
                    else
                    {
                        // Try common env var names for Gemini
                        apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
                        if (!string.IsNullOrEmpty(apiKey))
                        {
                            Debug.Log("ChatManager: Loaded GEMINI_API_KEY from environment.");
                        }
                        if (string.IsNullOrEmpty(apiKey))
                        {
                            apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
                            if (!string.IsNullOrEmpty(apiKey))
                            {
                                Debug.Log("ChatManager: Loaded GOOGLE_API_KEY from environment.");
                            }
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void ClearHistory()
        {
            history.Clear();
        }

        public void SendMessageToLilith(string userText, Action<string> onAssistantReply, Action<string> onError = null)
        {
            if (string.IsNullOrWhiteSpace(userText)) return;
            StartCoroutine(SendChatCoroutine(userText, onAssistantReply, onError));
        }

        private IEnumerator SendChatCoroutine(string userText, Action<string> onAssistantReply, Action<string> onError)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                onError?.Invoke("Missing API key. Set it on ChatManager or environment variable OPENAI_API_KEY (OpenAI) or GEMINI_API_KEY/GOOGLE_API_KEY (Gemini).");
                yield break;
            }

            history.Add(new ChatMessage("user", userText));
            TrimHistory();

            if (provider == Provider.OpenAI)
            {
                Debug.Log("ChatManager: Sending request to OpenAI model=" + model);
                yield return SendOpenAIRequest(onAssistantReply, onError);
            }
            else
            {
                Debug.Log("ChatManager: Sending request to Gemini model=" + model);
                yield return SendGeminiRequest(onAssistantReply, onError);
            }
        }

        private IEnumerator SendOpenAIRequest(Action<string> onAssistantReply, Action<string> onError)
        {
            var messagesPayload = BuildMessagesPayload();

            var requestBody = new ChatCompletionRequest
            {
                model = model,
                temperature = temperature,
                messages = messagesPayload.ToArray()
            };

            var json = JsonUtility.ToJson(requestBody);

            using (var req = new UnityWebRequest(apiBaseUrl, UnityWebRequest.kHttpVerbPOST))
            {
                var bodyRaw = Encoding.UTF8.GetBytes(json);
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                req.SetRequestHeader("Authorization", "Bearer " + apiKey);

                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                {
                    onError?.Invoke($"HTTP {req.responseCode}: {req.error}\n{req.downloadHandler.text}");
                    Debug.LogError("OpenAI error: " + req.error + " body=" + req.downloadHandler.text);
                    yield break;
                }

                var responseText = req.downloadHandler.text;
                Debug.Log("OpenAI raw response: " + responseText);

                ChatCompletionResponse response = null;
                try
                {
                    response = JsonUtility.FromJson<ChatCompletionResponse>(responseText);
                }
                catch (Exception ex)
                {
                    onError?.Invoke("Failed to parse response: " + ex.Message + "\n" + responseText);
                    yield break;
                }

                if (response == null || response.choices == null || response.choices.Length == 0 || response.choices[0].message == null)
                {
                    onError?.Invoke("Empty response from model: " + responseText);
                    yield break;
                }

                var assistantText = response.choices[0].message.content;
                history.Add(new ChatMessage("assistant", assistantText));
                HandleEmotion(assistantText);
                onAssistantReply?.Invoke(assistantText);
            }
        }

        private IEnumerator SendGeminiRequest(Action<string> onAssistantReply, Action<string> onError)
        {
            // Build URL: {base}/{model}:generateContent
            var url = $"{geminiApiBase}/{model}:generateContent";

            var contents = BuildGeminiContents();
            var requestBody = new GeminiGenerateContentRequest
            {
                contents = contents.ToArray(),
                systemInstruction = new GeminiContent
                {
                    // No role needed for systemInstruction per v1beta
                    parts = new[] { new GeminiPart { text = lilithPersona } }
                },
                generationConfig = new GeminiGenerationConfig
                {
                    temperature = temperature
                }
            };

            var json = JsonUtility.ToJson(requestBody);

            using (var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                var bodyRaw = Encoding.UTF8.GetBytes(json);
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                // Gemini uses API key header
                req.SetRequestHeader("x-goog-api-key", apiKey);

                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                {
                    onError?.Invoke($"HTTP {req.responseCode}: {req.error}\n{req.downloadHandler.text}");
                    Debug.LogError("Gemini error: " + req.error + " body=" + req.downloadHandler.text);
                    yield break;
                }

                var responseText = req.downloadHandler.text;
                Debug.Log("Gemini raw response: " + responseText);
                GeminiGenerateContentResponse response = null;
                try
                {
                    response = JsonUtility.FromJson<GeminiGenerateContentResponse>(responseText);
                }
                catch (Exception ex)
                {
                    onError?.Invoke("Failed to parse Gemini response: " + ex.Message + "\n" + responseText);
                    yield break;
                }

                if (response == null || response.candidates == null || response.candidates.Length == 0 || response.candidates[0].content == null || response.candidates[0].content.parts == null || response.candidates[0].content.parts.Length == 0)
                {
                    onError?.Invoke("Empty response from Gemini: " + responseText);
                    yield break;
                }

                var assistantText = response.candidates[0].content.parts[0].text;
                history.Add(new ChatMessage("assistant", assistantText));
                HandleEmotion(assistantText);
                onAssistantReply?.Invoke(assistantText);
            }
        }

        private void HandleEmotion(string text)
        {
            if (emotionVideoManager != null)
            {
                var emotion = emotionVideoManager.ParseEmotionFromText(text);
                emotionVideoManager.PlayVideoForEmotion(emotion);
            }
        }

        private void TrimHistory()
        {
            var excess = history.Count - maxHistoryMessages;
            if (excess > 0)
            {
                history.RemoveRange(0, excess);
            }
        }

        private List<ChatMessage> BuildMessagesPayload()
        {
            var list = new List<ChatMessage>();
            if (!string.IsNullOrWhiteSpace(lilithPersona))
            {
                list.Add(new ChatMessage("system", lilithPersona));
            }
            list.AddRange(history);
            return list;
        }

        private List<GeminiContent> BuildGeminiContents()
        {
            var list = new List<GeminiContent>();

            foreach (var msg in history)
            {
                var role = msg.role == "assistant" ? "model" : "user"; // map to Gemini roles
                list.Add(new GeminiContent
                {
                    role = role,
                    parts = new[] { new GeminiPart { text = msg.content } }
                });
            }

            return list;
        }

        [Serializable]
        private class ChatCompletionRequest
        {
            public string model;
            public float temperature = 0.7f;
            public ChatMessage[] messages;
        }

        [Serializable]
        private class ChatCompletionResponse
        {
            public Choice[] choices;
        }

        [Serializable]
        private class Choice
        {
            public ChatMessage message;
        }

        // Gemini DTOs
        [Serializable]
        private class GeminiGenerateContentRequest
        {
            public GeminiContent[] contents;
            public GeminiContent systemInstruction;
            public GeminiGenerationConfig generationConfig;
        }

        [Serializable]
        private class GeminiGenerationConfig
        {
            public float temperature;
        }

        [Serializable]
        private class GeminiContent
        {
            public string role; // optional for systemInstruction
            public GeminiPart[] parts;
        }

        [Serializable]
        private class GeminiPart
        {
            public string text;
        }

        [Serializable]
        private class GeminiGenerateContentResponse
        {
            public GeminiCandidate[] candidates;
        }

        [Serializable]
        private class GeminiCandidate
        {
            public GeminiContent content;
        }
    }
}


