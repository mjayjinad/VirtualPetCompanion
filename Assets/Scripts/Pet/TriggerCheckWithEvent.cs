using UnityEngine;
using UnityEngine.Events;

public class TriggerCheckWithEvent : MonoBehaviour
{
    // Custom tag to check for
    [SerializeField] private string customTag = "YourCustomTag";

    // Unity Event to invoke
    [SerializeField] private UnityEvent onTriggerEnterEvent;
    [SerializeField] private UnityEvent onTriggerExitEvent;

    // OnTriggerEnter is called when another collider enters this object's trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has the custom tag
        if (other.CompareTag(customTag))
        {
            Debug.Log($"Object with tag '{customTag}' has entered the trigger.");
            // Invoke the Unity Event
            onTriggerEnterEvent.Invoke();
        }
        else
        {
            Debug.Log($"Object with tag '{other.tag}' has entered the trigger, but it's not the custom tag.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object has the custom tag
        if (other.CompareTag(customTag))
        {
            Debug.Log($"Object with tag '{customTag}' has entered the trigger.");
            // Invoke the Unity Event
            onTriggerExitEvent.Invoke();
        }
        else
        {
            Debug.Log($"Object with tag '{other.tag}' has entered the trigger, but it's not the custom tag.");
        }
    }
}