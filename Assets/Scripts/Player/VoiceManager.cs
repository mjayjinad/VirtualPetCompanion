using UnityEngine;
using Meta.WitAi.CallbackHandlers;
using Oculus.Voice;
using TMPro;
using UnityEngine.Events;
using System.Reflection;
public class VoiceManager : MonoBehaviour
{
    [Header("WIT Configuration")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;
    [SerializeField] private WitResponseMatcher responseMatcher;
    [SerializeField] private TMP_Text transcriptionText;

    [Header("Voice Events")]
    [SerializeField] private UnityEvent wakeWordDetected;
    [SerializeField] private UnityEvent<string> completeTranscription;

    private bool voiceCommandReady;

    private void Awake()
    {
        appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(ReactivateVoice);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscription);

        var eventField = typeof(WitResponseMatcher).GetField("onMultiValueEvent", BindingFlags.NonPublic | BindingFlags.Instance);

        if(eventField != null && eventField.GetValue(responseMatcher) is MultiValueEvent onMultiValueEvent)
        {
            onMultiValueEvent.AddListener(WakeWordDetected);   
        }

        appVoiceExperience.Activate();
    }

    private void ReactivateVoice()
    {
        Debug.Log("Voice Reactivated");
        appVoiceExperience.Activate();
    } 

    private void WakeWordDetected(string[] arg)
    {
        voiceCommandReady = true;
        wakeWordDetected?.Invoke();
    }
    
    private void OnPartialTranscription(string transcription)
    {
        if (!voiceCommandReady) return;
        transcriptionText.text = transcription;
    }

    private void OnFullTranscription(string transcription)
    {
        if(!voiceCommandReady) return;
        voiceCommandReady = false;
        appVoiceExperience.Deactivate();
        completeTranscription?.Invoke(transcription);
    }

    private void OnDestroy()
    {
        appVoiceExperience.VoiceEvents.OnRequestCompleted.RemoveListener(ReactivateVoice);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);
        appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnFullTranscription);

        var eventField = typeof(WitResponseMatcher).GetField("OnMultiValueEvent", BindingFlags.NonPublic | BindingFlags.Instance);

        if (eventField != null && eventField.GetValue(responseMatcher) is MultiValueEvent onMultiValueEvent)
        {
            onMultiValueEvent.RemoveListener(WakeWordDetected);
        }
    }
}
