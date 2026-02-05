using UnityEngine;
using CommandInput;

public class TestCommandMatcher : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private CommandInputConfig inputConfig;
    [SerializeField] private CommandCollection commandCollection;

    [Header("Test Input")]
    [SerializeField] private InputDirection[] testPattern;

    private CommandMatcher commandMatcher;

    void Start()
    {
        // 컬렉션 검증
        if (commandCollection != null)
        {
            commandCollection.Validate();
            Debug.Log(commandCollection.GetDebugInfo());
        }

        // CommandMatcher 생성
        commandMatcher = new CommandMatcher(commandCollection);
    }

    void Update()
    {
        // Space 키로 테스트 패턴 입력
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestPattern();
        }

        // C 키로 Clear
        if (Input.GetKeyDown(KeyCode.C))
        {
            commandMatcher.Clear();
            Debug.Log("Cleared input history");
        }

        // 숫자 키로 개별 커맨드 테스트
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestCommand(0); // Arc Thrower
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestCommand(1); // Orbital Strike
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestCommand(2); // Napalm Airstrike
        }
    }

    void TestPattern()
    {
        if (testPattern == null || testPattern.Length == 0)
        {
            Debug.LogWarning("Test pattern is empty!");
            return;
        }

        commandMatcher.Clear();
        Debug.Log($"=== Testing Pattern: {GetPatternString(testPattern)} ===");

        // 패턴을 하나씩 입력하면서 매칭 결과 확인
        for (int i = 0; i < testPattern.Length; i++)
        {
            var results = commandMatcher.AddInput(testPattern[i]);

            Debug.Log($"\n[Step {i + 1}] Input: {DirectionalInputHelper.DirectionToArrow(testPattern[i])}");
            Debug.Log($"Current: {commandMatcher.GetInputString()}");

            if (results.Count > 0)
            {
                Debug.Log($"Matching commands: {results.Count}");
                foreach (var result in results)
                {
                    Debug.Log($"  {result}");
                }
            }
            else
            {
                Debug.Log("No matches");
            }
        }

        // 최종 완성된 커맨드 찾기
        var best = commandMatcher.GetBestCompletedMatch();
        if (best != null)
        {
            Debug.Log($"\n<color=green>COMPLETED: {best.command.displayName} (similarity: {best.similarity:P0})</color>");
        }
        else
        {
            Debug.Log($"\n<color=red>No completed command</color>");
        }
    }

    void TestCommand(int index)
    {
        if (commandCollection == null || index >= commandCollection.GetCommandCount())
        {
            Debug.LogError($"Invalid command index: {index}");
            return;
        }

        var command = commandCollection.GetCommandAt(index);
        if (command == null)
        {
            Debug.LogError($"Command at index {index} is null");
            return;
        }

        Debug.Log($"=== Testing {command.displayName} ===");
        Debug.Log($"Pattern: {command.GetPatternString()}");

        // 실제 패턴을 입력
        testPattern = command.pattern;
        TestPattern();
    }

    string GetPatternString(InputDirection[] pattern)
    {
        if (pattern == null || pattern.Length == 0)
            return "Empty";

        var arrows = new string[pattern.Length];
        for (int i = 0; i < pattern.Length; i++)
        {
            arrows[i] = DirectionalInputHelper.DirectionToArrow(pattern[i]);
        }
        return string.Join(" ", arrows);
    }
}