using UnityEngine;
using UnityEngine.Events;

namespace CommandInput
{
    /// <summary>
    /// 개별 커맨드 데이터
    /// 입력 패턴과 실행 콜백을 정의
    /// </summary>
    [CreateAssetMenu(fileName = "NewCommand", menuName = "CommandInput/Command Data")]
    public class CommandData : ScriptableObject
    {
        [Header("Command Info")]
        [Tooltip("커맨드 고유 ID")]
        public string commandId = "command_01";

        [Tooltip("커맨드 표시 이름")]
        public string displayName = "New Command";

        [Tooltip("커맨드 아이콘 (UI용, 선택사항)")]
        public Sprite icon;

        [Header("Input Pattern")]
        [Tooltip("입력 패턴 (순서대로)")]
        public InputDirection[] pattern = new InputDirection[] { };

        [Header("Pattern Matching")]
        [Tooltip("패턴 매칭 설정")]
        public PatternMatchConfig matchConfig = new PatternMatchConfig();

        [Header("Timing")]
        [Tooltip("전체 입력 제한 시간 (초)")]
        [Range(0.5f, 5f)]
        public float maxInputDuration = 1.5f;

        /// <summary>
        /// 패턴을 화살표 문자열로 변환 (디버깅/UI용)
        /// </summary>
        public string GetPatternString()
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

        /// <summary>
        /// 패턴 길이
        /// </summary>
        public int GetPatternLength()
        {
            return pattern?.Length ?? 0;
        }

        /// <summary>
        /// 커맨드가 유효한지 검증
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(commandId))
            {
                Debug.LogError($"CommandData '{name}': commandId가 비어있음!");
                return false;
            }

            if (pattern == null || pattern.Length == 0)
            {
                Debug.LogError($"CommandData '{name}': pattern이 비어있음!");
                return false;
            }

            // None이 패턴에 포함되어 있으면 경고
            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] == InputDirection.None)
                {
                    Debug.LogWarning($"CommandData '{name}': pattern에 None이 포함되어 있음 (인덱스 {i})");
                }
            }

            return true;
        }
    }
}