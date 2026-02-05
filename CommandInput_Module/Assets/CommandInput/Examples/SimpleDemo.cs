using UnityEngine;
using CommandInput;

public class SimpleDemo : MonoBehaviour
{
    [SerializeField] private CommandInputManager commandInputManager;

    private void Start()
    {
        commandInputManager.onCommandExecuted.AddListener(OnCommandExecuted);
    }

    private void OnCommandExecuted(CommandData command)
    {
        Debug.Log($"Stratagem called: {command.displayName}");
    }
}