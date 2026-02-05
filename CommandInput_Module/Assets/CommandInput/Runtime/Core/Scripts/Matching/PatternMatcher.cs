using UnityEngine;

namespace CommandInput
{
    /// <summary>
    /// 패턴 매칭 로직
    /// 입력 패턴과 커맨드 패턴의 유사도를 계산
    /// </summary>
    public static class PatternMatcher
    {
        /// <summary>
        /// 두 패턴의 유사도 계산 (0~1)
        /// </summary>
        /// <param name="commandPattern">커맨드 패턴</param>
        /// <param name="inputPattern">입력 패턴</param>
        /// <param name="config">매칭 설정</param>
        /// <returns>유사도 (0~1)</returns>
        public static float CalculateSimilarity(
            InputDirection[] commandPattern,
            InputDirection[] inputPattern,
            PatternMatchConfig config)
        {
            if (commandPattern == null || commandPattern.Length == 0)
                return 0f;

            if (inputPattern == null || inputPattern.Length == 0)
                return 0f;

            // 입력이 패턴보다 짧으면 부분 매칭
            if (inputPattern.Length < commandPattern.Length)
            {
                return CalculatePartialSimilarity(commandPattern, inputPattern, config);
            }

            // 입력이 패턴과 같거나 길면 슬라이딩 윈도우로 가장 유사한 구간 찾기
            if (config.allowExtraInputs)
            {
                return CalculateSlidingWindowSimilarity(commandPattern, inputPattern, config);
            }
            else
            {
                // 추가 입력 허용 안 하면 길이가 같아야 함
                if (inputPattern.Length != commandPattern.Length)
                    return 0f;

                return CalculateExactSimilarity(commandPattern, inputPattern, config);
            }
        }

        /// <summary>
        /// 부분 패턴 유사도 계산 (입력이 패턴보다 짧을 때)
        /// </summary>
        private static float CalculatePartialSimilarity(
            InputDirection[] commandPattern,
            InputDirection[] inputPattern,
            PatternMatchConfig config)
        {
            float totalSimilarity = 0f;

            // 입력된 부분만 비교
            for (int i = 0; i < inputPattern.Length; i++)
            {
                float dirSimilarity = CalculateDirectionSimilarity(
                    commandPattern[i],
                    inputPattern[i],
                    config.maxAngleDifference);

                totalSimilarity += dirSimilarity;
            }

            // 입력된 부분의 평균 유사도
            return totalSimilarity / inputPattern.Length;
        }

        /// <summary>
        /// 정확한 길이 매칭 유사도 계산
        /// </summary>
        private static float CalculateExactSimilarity(
            InputDirection[] commandPattern,
            InputDirection[] inputPattern,
            PatternMatchConfig config)
        {
            float totalSimilarity = 0f;

            for (int i = 0; i < commandPattern.Length; i++)
            {
                float dirSimilarity = CalculateDirectionSimilarity(
                    commandPattern[i],
                    inputPattern[i],
                    config.maxAngleDifference);

                totalSimilarity += dirSimilarity;
            }

            return totalSimilarity / commandPattern.Length;
        }

        /// <summary>
        /// 슬라이딩 윈도우로 가장 유사한 구간 찾기
        /// </summary>
        private static float CalculateSlidingWindowSimilarity(
            InputDirection[] commandPattern,
            InputDirection[] inputPattern,
            PatternMatchConfig config)
        {
            float bestSimilarity = 0f;
            int patternLength = commandPattern.Length;

            // 입력에서 패턴 길이만큼의 윈도우를 슬라이드하며 비교
            for (int offset = 0; offset <= inputPattern.Length - patternLength; offset++)
            {
                float similarity = CalculateSegmentSimilarity(
                    commandPattern,
                    inputPattern,
                    offset,
                    config.maxAngleDifference);

                bestSimilarity = Mathf.Max(bestSimilarity, similarity);
            }

            return bestSimilarity;
        }

        /// <summary>
        /// 특정 오프셋에서의 구간 유사도 계산
        /// </summary>
        private static float CalculateSegmentSimilarity(
            InputDirection[] commandPattern,
            InputDirection[] inputPattern,
            int offset,
            float maxAngleDiff)
        {
            float totalSimilarity = 0f;

            for (int i = 0; i < commandPattern.Length; i++)
            {
                float dirSimilarity = CalculateDirectionSimilarity(
                    commandPattern[i],
                    inputPattern[offset + i],
                    maxAngleDiff);

                totalSimilarity += dirSimilarity;
            }

            return totalSimilarity / commandPattern.Length;
        }

        /// <summary>
        /// 두 방향의 유사도 계산 (0~1)
        /// </summary>
        private static float CalculateDirectionSimilarity(
            InputDirection dir1,
            InputDirection dir2,
            float maxAngleDiff)
        {
            // None이면 유사도 0
            if (dir1 == InputDirection.None || dir2 == InputDirection.None)
                return 0f;

            // 완전히 같으면 유사도 1
            if (dir1 == dir2)
                return 1f;

            // 각도 차이 계산
            float angle1 = DirectionalInputHelper.DirectionToAngle(dir1);
            float angle2 = DirectionalInputHelper.DirectionToAngle(dir2);

            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(angle1, angle2));

            // 각도 차이를 0~1 유사도로 변환
            // maxAngleDiff 이상 차이나면 유사도 0
            // 0도 차이면 유사도 1
            float similarity = 1f - Mathf.Clamp01(angleDiff / maxAngleDiff);

            return similarity;
        }

        /// <summary>
        /// 패턴 매칭 시도
        /// </summary>
        /// <param name="commandPattern">커맨드 패턴</param>
        /// <param name="inputPattern">입력 패턴</param>
        /// <param name="config">매칭 설정</param>
        /// <param name="similarity">계산된 유사도 (out)</param>
        /// <returns>매칭 성공 여부</returns>
        public static bool TryMatch(
            InputDirection[] commandPattern,
            InputDirection[] inputPattern,
            PatternMatchConfig config,
            out float similarity)
        {
            similarity = CalculateSimilarity(commandPattern, inputPattern, config);
            return similarity >= config.similarityThreshold;
        }
    }
}