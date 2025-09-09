using UnityEngine;

namespace LilithChat
{
    public class ChatDevTools : MonoBehaviour
    {
        [TextArea]
        public string testPrompt = "Hello Lilith, are you there?";

        [ContextMenu("Send Test Prompt")]
        public void SendTestPrompt()
        {
            if (ChatManager.Instance == null)
            {
                Debug.LogError("ChatDevTools: ChatManager.Instance is null.");
                return;
            }

            Debug.Log("ChatDevTools: Sending test prompt -> " + testPrompt);
            ChatManager.Instance.SendMessageToLilith(
                testPrompt,
                reply => Debug.Log("ChatDevTools: Reply <- " + reply),
                error => Debug.LogError("ChatDevTools: Error <- " + error)
            );
        }
    }
}


