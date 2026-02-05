using UnityEngine;
using CommandInput;

public class TestDirectionalInput : MonoBehaviour
{
    [SerializeField] private CommandInputConfig config;

    void Update()
    {
        // 키보드 WASD 또는 화살표 입력
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 input = new Vector2(h, v);

        // 방향 변환
        InputDirection dir = DirectionalInputHelper.Vector2ToDirection(input, config);

        if (dir != InputDirection.None)
        {
            string arrow = DirectionalInputHelper.DirectionToArrow(dir);
            Debug.Log($"Direction: {dir} {arrow}");
        }
    }
}