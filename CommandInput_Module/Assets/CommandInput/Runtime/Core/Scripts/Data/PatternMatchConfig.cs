using UnityEngine;

namespace CommandInput
{
    /// <summary>
    /// 패턴 매칭 설정
    /// 유사도 계산 방식과 허용 범위를 정의
    /// </summary>
    [System.Serializable]
    public class PatternMatchConfig
    {
        [Header("Similarity Settings")]
        [Tooltip("패턴 매칭 최소 유사도 (0~1)")]
        [Range(0f, 1f)]
        public float similarityThreshold = 0.7f;

        [Tooltip("방향 간 최대 각도 차이 (도)")]
        [Range(0f, 90f)]
        public float maxAngleDifference = 45f;

        [Header("Matching Options")]
        [Tooltip("추가 입력 허용 (예: ↓↘→↗ 입력해도 ↓↘→ 매칭)")]
        public bool allowExtraInputs = true;

        [Tooltip("중간 입력 생략 허용 (예: ↓→ 입력해도 ↓↘→ 매칭)")]
        public bool allowSkippedInputs = false;
    }
}