using Meta.WitAi;
using Meta.XR.MRUtilityKit;
using Oculus.Voice;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;

public class VoiceCommandManager : MonoBehaviour
{
    [Header("Wit.ai Voice Service")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;

    [Header("Voice Command Dictionary")]
    public VoiceCommand[] voiceCommands;

    [SerializeField] private Pet pet;
    [SerializeField] private PetSpawner spawner;

    private string currentTranscription;

    private void Start()
    {
        MRUK.Instance.SceneLoadedEvent.AddListener(InitializeSpawnedPet);
    }

    private void InitializeSpawnedPet()
    {
        StartCoroutine(InitializeSpawnedPetCoroutine());
    }

    private IEnumerator InitializeSpawnedPetCoroutine()
    {
        yield return new WaitForEndOfFrame();
        pet = spawner.spawnedPet.GetComponent<Pet>();
    }

    public void OnFullTranscription(string transcription)
    {
        currentTranscription = transcription.ToLower();
        CheckForCommandMatch(currentTranscription);
    }

    private void CheckForCommandMatch(string transcription)
    {
        foreach (var command in voiceCommands)
        {
            // Check if any of the synonyms match the transcription
            foreach (var phrase in command.commandPhrases)
            {
                if (transcription.Contains(phrase.ToLower()))
                {
                    appVoiceExperience.Deactivate();
                    Debug.LogWarning($"Command detected: {phrase}");
                    command.onCommandTriggered.Invoke();

                    return;
                }
            }
        }
        Debug.Log("No matching command found.");
    }

    public void SitCommand()
    {
        pet.SitCommand();
    }

    public void StartWiggle()
    {
        pet.WiggleCommand();
    }

    public void BarkCommand()
    {
        pet.BarkCommand();
    }

    public void StandCommand()
    {
        pet.StandCommand();
    }

    public void WalkToPlayerCommand()
    {
        pet.WalkToPlayerCommand();
    }

    public void FollowPlayerCommand()
    {
        pet.FollowPlayerCommand();
    }
}

[System.Serializable]
public class VoiceCommand
{
    public string[] commandPhrases;
    public UnityEvent onCommandTriggered;
}
