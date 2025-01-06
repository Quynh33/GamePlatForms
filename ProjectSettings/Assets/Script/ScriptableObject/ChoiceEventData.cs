using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChoiceEvent
{
    public string choiceText; 
    public UnityEvent onChoiceSelected;
}

[CreateAssetMenu(fileName = "New Dialogue Choices", menuName = "Dialogue/Choice Event Data")]
public class ChoiceEventData : ScriptableObject
{
    public ChoiceEvent[] choices; 
}