using UnityEngine;

namespace CommandInput
{
    /// <summary>
    /// Legacy Input Manager를 사용하는 방향 입력 구현
    /// Unity 구식 Input 시스템 (Input.GetKey, Input.GetAxis) 사용
    /// </summary>
    public class LegacyDirectionalInput : IDirectionalInput
    {
        private readonly CommandInputConfig config;

        public LegacyDirectionalInput(CommandInputConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// 위 방향 키가 이번 프레임에 눌렸는지
        /// </summary>
        public bool GetUpPressed()
        {
            return Input.GetKeyDown(config.upKey);
        }

        /// <summary>
        /// 아래 방향 키가 이번 프레임에 눌렸는지
        /// </summary>
        public bool GetDownPressed()
        {
            return Input.GetKeyDown(config.downKey);
        }

        /// <summary>
        /// 왼쪽 방향 키가 이번 프레임에 눌렸는지
        /// </summary>
        public bool GetLeftPressed()
        {
            return Input.GetKeyDown(config.leftKey);
        }

        /// <summary>
        /// 오른쪽 방향 키가 이번 프레임에 눌렸는지
        /// </summary>
        public bool GetRightPressed()
        {
            return Input.GetKeyDown(config.rightKey);
        }

        /// <summary>
        /// 아날로그 입력 (조이스틱 축 또는 WASD)
        /// </summary>
        public Vector2 GetDirectionalInput()
        {
            float horizontal = Input.GetAxis(config.horizontalAxis);
            float vertical = Input.GetAxis(config.verticalAxis);

            return new Vector2(horizontal, vertical);
        }
    }
}