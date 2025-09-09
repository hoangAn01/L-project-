using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using System;

namespace LilithChat
{
    public enum Emotion
    {
        Neutral,
        Happy,
        Sad,
        Playful,
        Thinking
    }

    [Serializable]
    public class EmotionVideo
    {
        public Emotion emotion;
        public VideoClip clip;
    }

    public class EmotionVideoManager : MonoBehaviour
    {
        [Header("Video Player Setup")]
        [SerializeField] private VideoPlayer videoPlayerA;
        [SerializeField] private RawImage videoDisplayA;
        [SerializeField] private VideoPlayer videoPlayerB;
        [SerializeField] private RawImage videoDisplayB;

        [Header("Transition Settings")]
        [SerializeField] private float crossfadeDuration = 1.0f;

        [Header("Emotion Videos")]
        [SerializeField] private List<EmotionVideo> emotionVideos = new List<EmotionVideo>();
        [SerializeField] private VideoClip defaultClip; // Fallback clip

        private Dictionary<Emotion, VideoClip> videoMap;
        private bool isPlayerAActive = true;
        private Coroutine crossfadeCoroutine;

        public static EmotionVideoManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // ********** NEW DIAGNOSTIC CHECK **********
            if (videoPlayerA == videoPlayerB)
            {
                Debug.LogError("FATAL ERROR: videoPlayerA and videoPlayerB are assigned to the SAME VideoPlayer component in the inspector! They MUST be two different components. Please fix this assignment.");
                enabled = false; // Disable this component to prevent further errors
                return;
            }
            // ******************************************

            videoMap = new Dictionary<Emotion, VideoClip>();
            foreach (var ev in emotionVideos)
            {
                if (ev.clip != null)
                {
                    videoMap[ev.emotion] = ev.clip;
                }
            }
        }

        private void Start()
        {
            SetupPlayer(videoPlayerA, videoDisplayA);
            SetupPlayer(videoPlayerB, videoDisplayB);

            videoDisplayA.color = Color.white;
            videoDisplayB.color = new Color(1, 1, 1, 0);

            PlayVideoForEmotion(Emotion.Neutral, true);
        }

        private void SetupPlayer(VideoPlayer player, RawImage display)
        {
            if (player == null || display == null) {
                Debug.LogError("EmotionVideoManager: A VideoPlayer or RawImage is not assigned!");
                return;
            }

            player.renderMode = VideoRenderMode.RenderTexture;
            if (player.targetTexture == null)
            {
                player.targetTexture = new RenderTexture(1920, 1080, 24);
            }
            display.texture = player.targetTexture;
            player.isLooping = true;
            player.playOnAwake = false; // Ensure we control playback manually
        }

        public void PlayVideoForEmotion(Emotion emotion, bool forceImmediate = false)
        {
            VideoClip clipToPlay = videoMap.ContainsKey(emotion) ? videoMap[emotion] : defaultClip;

            if (clipToPlay == null) return;

            VideoPlayer activePlayer = isPlayerAActive ? videoPlayerA : videoPlayerB;

            if (activePlayer.isPlaying && activePlayer.clip == clipToPlay) return; // Already playing the correct clip

            if (crossfadeCoroutine != null)
            {
                StopCoroutine(crossfadeCoroutine);
            }

            if (forceImmediate || crossfadeDuration <= 0)
            {
                activePlayer.clip = clipToPlay;
                activePlayer.Prepare();
                StartCoroutine(PlayWhenPrepared(activePlayer));
            }
            else
            {
                crossfadeCoroutine = StartCoroutine(CrossfadeToVideo(clipToPlay));
            }
        }

        private IEnumerator PlayWhenPrepared(VideoPlayer player)
        {
            while (!player.isPrepared)
            {
                yield return null;
            }
            player.Play();
        }

        private IEnumerator CrossfadeToVideo(VideoClip newClip)
        {
            Debug.Log($"Crossfade started for clip: {newClip.name}");

            VideoPlayer activePlayer = isPlayerAActive ? videoPlayerA : videoPlayerB;
            RawImage activeDisplay = isPlayerAActive ? videoDisplayA : videoDisplayB;
            VideoPlayer inactivePlayer = isPlayerAActive ? videoPlayerB : videoPlayerA;
            RawImage inactiveDisplay = isPlayerAActive ? videoDisplayB : videoDisplayA;

            Debug.Log($"Inactive player is {(isPlayerAActive ? "B" : "A")}. Setting clip and preparing.");
            // Setup and prepare the new video on the inactive player
            inactivePlayer.clip = newClip;
            inactivePlayer.Prepare();

            // Wait until the video is prepared, with a timeout
            float preparationTimeout = 5f; // 5 second timeout
            float preparationTimer = 0f;
            while (!inactivePlayer.isPrepared)
            {
                preparationTimer += Time.deltaTime;
                if (preparationTimer > preparationTimeout)
                {
                    Debug.LogError($"Video clip {newClip.name} failed to prepare within {preparationTimeout} seconds.");
                    crossfadeCoroutine = null; // Reset coroutine state
                    yield break; // Exit the coroutine
                }
                yield return null;
            }

            Debug.Log($"Clip {newClip.name} is prepared after {preparationTimer:F2} seconds. Calling Play().");
            // Once prepared, play the video and start the fade
            inactivePlayer.Play();

            // "Voodoo" fix: Sometimes the player needs a kick to start rendering
            yield return null; 
            inactivePlayer.Pause();
            yield return null;
            inactivePlayer.Play();

            // Check if playback started
            yield return new WaitForSeconds(0.1f); // Wait a very short moment
            if (!inactivePlayer.isPlaying)
            {
                Debug.LogWarning($"Player for {newClip.name} is NOT playing even after Play() was called.");
            }
            else
            {
                Debug.Log($"Player for {newClip.name} is now playing.");
            }


            float time = 0f;
            while (time < crossfadeDuration)
            {
                float alpha = time / crossfadeDuration;
                activeDisplay.color = new Color(1, 1, 1, 1 - alpha);
                inactiveDisplay.color = new Color(1, 1, 1, alpha);
                time += Time.deltaTime;
                yield return null;
            }

            Debug.Log("Crossfade animation finished.");
            activeDisplay.color = new Color(1, 1, 1, 0);
            inactiveDisplay.color = Color.white;

            Debug.Log($"Stopping old player ({(isPlayerAActive ? "A" : "B")}).");
            activePlayer.Stop();
            isPlayerAActive = !isPlayerAActive;
            crossfadeCoroutine = null;
            Debug.Log("Crossfade complete. New active player is " + (isPlayerAActive ? "A" : "B"));
        }

        public Emotion ParseEmotionFromText(string text)
        {
            if (string.IsNullOrEmpty(text)) return Emotion.Neutral;

            string lowerText = text.ToLower();

            if (lowerText.Contains("[smiles") || lowerText.Contains("[chuckles") || lowerText.Contains("giggles") || lowerText.Contains("mỉm cười") || lowerText.Contains("vui vẻ"))
            {
                return Emotion.Happy;
            }
            if (lowerText.Contains("[teasing]") || lowerText.Contains("trêu chọc"))
            {
                return Emotion.Playful;
            }
            if (lowerText.Contains("[looks away]") || lowerText.Contains("[sighs]") || lowerText.Contains("buồn") || lowerText.Contains("cô đơn")|| lowerText.Contains("thất vọng")|| lowerText.Contains("thở dài")|| lowerText.Contains("nhìn đi chỗ khác"))
            {
                return Emotion.Sad;
            }

            return Emotion.Neutral;
        }
    }
}
