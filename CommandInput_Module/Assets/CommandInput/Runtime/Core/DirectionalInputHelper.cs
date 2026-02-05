using UnityEngine;

namespace CommandInput
{
    /// <summary>
    /// 방향 입력 변환 유틸리티
    /// Vector2를 InputDirection으로 변환하거나 각도 계산 등을 처리
    /// </summary>
    public static class DirectionalInputHelper
    {
        /// <summary>
        /// Vector2 입력을 InputDirection으로 변환
        /// </summary>
        /// <param name="input">입력 벡터</param>
        /// <param name="config">입력 설정</param>
        /// <returns>변환된 방향</returns>
        public static InputDirection Vector2ToDirection(Vector2 input, CommandInputConfig config)
        {
            // 데드존 체크
            if (input.magnitude < config.deadzone)
                return InputDirection.None;

            // Vector2를 각도로 변환 (라디안 → 도)
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

            // 0~360 범위로 정규화
            if (angle < 0)
                angle += 360f;

            // 모드에 따라 4방향 또는 8방향으로 변환
            if (config.directionalMode == DirectionalMode.FourWay)
                return AngleTo4Direction(angle);
            else
                return AngleTo8Direction(angle);
        }

        /// <summary>
        /// 각도를 4방향으로 변환 (↑↓←→)
        /// </summary>
        private static InputDirection AngleTo4Direction(float angle)
        {
            // 각 방향의 범위: ±45도
            if (angle >= 315f || angle < 45f)
                return InputDirection.Right;      // → (0도)
            else if (angle >= 45f && angle < 135f)
                return InputDirection.Up;         // ↑ (90도)
            else if (angle >= 135f && angle < 225f)
                return InputDirection.Left;       // ← (180도)
            else // 225f ~ 315f
                return InputDirection.Down;       // ↓ (270도)
        }

        /// <summary>
        /// 각도를 8방향으로 변환 (↑↓←→ + 대각선)
        /// </summary>
        private static InputDirection AngleTo8Direction(float angle)
        {
            // 각 방향의 범위: ±22.5도
            if (angle >= 337.5f || angle < 22.5f)
                return InputDirection.Right;      // → (0도)
            else if (angle >= 22.5f && angle < 67.5f)
                return InputDirection.UpRight;    // ↗ (45도)
            else if (angle >= 67.5f && angle < 112.5f)
                return InputDirection.Up;         // ↑ (90도)
            else if (angle >= 112.5f && angle < 157.5f)
                return InputDirection.UpLeft;     // ↖ (135도)
            else if (angle >= 157.5f && angle < 202.5f)
                return InputDirection.Left;       // ← (180도)
            else if (angle >= 202.5f && angle < 247.5f)
                return InputDirection.DownLeft;   // ↙ (225도)
            else if (angle >= 247.5f && angle < 292.5f)
                return InputDirection.Down;       // ↓ (270도)
            else // 292.5f ~ 337.5f
                return InputDirection.DownRight;  // ↘ (315도)
        }

        /// <summary>
        /// InputDirection을 각도로 변환
        /// </summary>
        public static float DirectionToAngle(InputDirection direction)
        {
            return (int)direction;
        }

        /// <summary>
        /// InputDirection을 Vector2로 변환 (정규화된 방향 벡터)
        /// </summary>
        public static Vector2 DirectionToVector2(InputDirection direction)
        {
            if (direction == InputDirection.None)
                return Vector2.zero;

            float angle = DirectionToAngle(direction) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        /// <summary>
        /// 두 방향 사이의 각도 차이 계산 (0~180도)
        /// </summary>
        public static float GetAngleDifference(InputDirection dir1, InputDirection dir2)
        {
            if (dir1 == InputDirection.None || dir2 == InputDirection.None)
                return 180f; // None이면 최대 차이

            float angle1 = DirectionToAngle(dir1);
            float angle2 = DirectionToAngle(dir2);

            return Mathf.Abs(Mathf.DeltaAngle(angle1, angle2));
        }

        /// <summary>
        /// 방향을 화살표 문자로 변환 (디버깅용)
        /// </summary>
        public static string DirectionToArrow(InputDirection direction)
        {
            switch (direction)
            {
                case InputDirection.Up: return "↑";
                case InputDirection.UpRight: return "↗";
                case InputDirection.Right: return "→";
                case InputDirection.DownRight: return "↘";
                case InputDirection.Down: return "↓";
                case InputDirection.DownLeft: return "↙";
                case InputDirection.Left: return "←";
                case InputDirection.UpLeft: return "↖";
                case InputDirection.None: return "·";
                default: return "?";
            }
        }
    }
}