using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class TriggerEventPair
{
    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerExitEvent;
}

public class Trigger : MonoBehaviour
{
    [Serializable]
    public class ClassEvent
    {
        public string ClassName;
        public TriggerEventPair Events;
    }

    public List<ClassEvent> ClassEvents;

    private Dictionary<Type, TriggerEventPair> classTypeToEventsMap = new Dictionary<Type, TriggerEventPair>();

    private void Start()
    {
        foreach (var classEvent in ClassEvents)
        {
            Type type = Type.GetType(classEvent.ClassName);
            if (type != null)
            {
                classTypeToEventsMap[type] = classEvent.Events;
            }
            else
            {
                Debug.LogError($"TriggerController: Specified class '{classEvent.ClassName}' could not be found. Please ensure the class name includes its namespace.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var entry in classTypeToEventsMap)
        {
            if (other.GetComponent(entry.Key) != null)
            {
                entry.Value.OnTriggerEnterEvent.Invoke();
                Debug.Log($"Trigger Enter for {entry.Key.Name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (var entry in classTypeToEventsMap)
        {
            if (other.GetComponent(entry.Key) != null)
            {
                entry.Value.OnTriggerExitEvent.Invoke();
                Debug.Log($"Trigger Exit for {entry.Key.Name}");
            }
        }
    }

    // Draw the BoxCollider and GameObject's name with Gizmos
    void OnDrawGizmos()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
            Gizmos.color = Color.green; // Set the color of the Gizmos
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(collider.center, collider.size); // Draw a wireframe cube with the size and position of the collider
            Gizmos.matrix = oldGizmosMatrix;

#if UNITY_EDITOR
            // Draw the name of the GameObject
            Handles.Label(transform.position, gameObject.name, new GUIStyle { fontSize = 12, normal = { textColor = Color.white } });
#endif
        }
    }
}
