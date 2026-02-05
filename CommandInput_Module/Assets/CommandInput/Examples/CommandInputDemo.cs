using UnityEngine;
using CommandInput;
using System.Collections.Generic;

/// <summary>
/// CommandInputManager 사용 예시 데모
/// 실시간으로 커맨드 입력 상태를 표시
/// </summary>
public class CommandInputDemo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CommandInputManager commandInputManager;

    [Header("Debug Display")]
    [SerializeField] private bool showDebugInfo = true;

    private void Start()
    {
        if (commandInputManager == null)
        {
            commandInputManager = FindObjectOfType<CommandInputManager>();
        }

        if (commandInputManager == null)
        {
            Debug.LogError("CommandInputManager를 찾을 수 없음!");
            return;
        }

        // 이벤트 연결
        commandInputManager.onMatchingCommandsChanged.AddListener(OnMatchingChanged);
        commandInputManager.onCommandExecuted.AddListener(OnCommandExecuted);
        commandInputManager.onInputCleared.AddListener(OnInputCleared);

        Debug.Log("CommandInputDemo started");
    }

    private void Update()
    {
        // R 키로 수동 초기화
        if (Input.GetKeyDown(KeyCode.R))
        {
            commandInputManager.ClearInput();
            Debug.Log("Manual reset");
        }
    }

    private void OnGUI()
    {
        if (!showDebugInfo || commandInputManager == null)
            return;

        GUILayout.BeginArea(new Rect(10, 10, 400, 300));
        GUILayout.Box("Command Input System - Debug Info");

        // 현재 입력
        string currentInput = commandInputManager.GetCurrentInputString();
        GUILayout.Label($"Current Input: {currentInput}");

        // 입력 활성화 상태
        bool isActive = commandInputManager.IsInputActive();
        GUILayout.Label($"Input Active: {isActive}");

        GUILayout.Space(10);

        // 매칭 중인 커맨드들
        var matches = commandInputManager.GetMatchingCommands();
        GUILayout.Label($"Matching Commands: {matches.Count}");

        foreach (var match in matches)
        {
            string status = match.isComplete ? "[COMPLETE]" : "[IN PROGRESS]";
            GUILayout.Label($"  {status} {match.command.displayName}");
            GUILayout.Label($"    Progress: {match.progress:P0}, Similarity: {match.similarity:P0}");
        }

        GUILayout.Space(10);
        GUILayout.Label("Press R to reset");

        GUILayout.EndArea();
    }

    private void OnMatchingChanged(List<CommandMatchResult> matches)
    {
        if (!showDebugInfo)
            return;

        Debug.Log($"Matching changed: {matches.Count} commands");
        foreach (var match in matches)
        {
            Debug.Log($"  - {match}");
        }
    }

    private void OnCommandExecuted(CommandData command)
    {
        Debug.Log($"<color=cyan>DEMO: {command.displayName} executed!</color>");

        // 여기에 실제 게임 로직 추가
        // 예: SpawnStratagem(command.commandId);
    }

    private void OnInputCleared()
    {
        Debug.Log("Input cleared");
    }

    private void OnDestroy()
    {
        if (commandInputManager != null)
        {
            commandInputManager.onMatchingCommandsChanged.RemoveListener(OnMatchingChanged);
            commandInputManager.onCommandExecuted.RemoveListener(OnCommandExecuted);
            commandInputManager.onInputCleared.RemoveListener(OnInputCleared);
        }
    }
}