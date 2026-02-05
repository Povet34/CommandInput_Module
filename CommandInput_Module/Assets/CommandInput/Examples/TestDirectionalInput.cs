using UnityEngine;
using CommandInput;

public class TestDirectionalInput : MonoBehaviour
{
    [SerializeField] private CommandInputConfig config;
    private IDirectionalInput directionalInput;
    private DirectionalInputTracker inputTracker;

    void Start()
    {
        directionalInput = DirectionalInputFactory.Create(config);
        inputTracker = new DirectionalInputTracker(config.directionChangeInterval);
    }

    void Update()
    {
        Vector2 input = directionalInput.GetDirectionalInput();

        // 입력 값 확인
        if (input != Vector2.zero)
        {
            Debug.Log($"Raw Input: ({input.x:F3}, {input.y:F3})");
        }

        if (inputTracker.UpdateDirection(input, config, out InputDirection newDirection))
        {
            string arrow = DirectionalInputHelper.DirectionToArrow(newDirection);
            Debug.Log($"Direction Changed: {newDirection} {arrow}");
            Debug.Log($"History: {inputTracker.GetHistoryString()}");
        }
    }
}