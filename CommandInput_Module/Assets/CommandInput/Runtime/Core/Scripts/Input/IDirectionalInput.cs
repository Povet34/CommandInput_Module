using UnityEngine;

namespace CommandInput
{
    /// <summary>
    /// 방향 입력 인터페이스
    /// Legacy Input, New Input System 등 다양한 입력 방식을 추상화
    /// </summary>
    public interface IDirectionalInput
    {
        /// <summary>
        /// 위 방향 키가 눌렸는지
        /// </summary>
        bool GetUpPressed();

        /// <summary>
        /// 아래 방향 키가 눌렸는지
        /// </summary>
        bool GetDownPressed();

        /// <summary>
        /// 왼쪽 방향 키가 눌렸는지
        /// </summary>
        bool GetLeftPressed();

        /// <summary>
        /// 오른쪽 방향 키가 눌렸는지
        /// </summary>
        bool GetRightPressed();

        /// <summary>
        /// 아날로그 입력 (조이스틱, WASD 등)
        /// </summary>
        /// <returns>정규화된 방향 벡터</returns>
        Vector2 GetDirectionalInput();
    }
}