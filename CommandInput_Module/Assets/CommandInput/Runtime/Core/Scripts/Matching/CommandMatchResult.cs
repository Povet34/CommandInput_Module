namespace CommandInput
{
    /// <summary>
    /// 커맨드 매칭 결과
    /// 현재 입력이 특정 커맨드와 얼마나 일치하는지 정보를 담음
    /// </summary>
    [System.Serializable]
    public class CommandMatchResult
    {
        /// <summary>
        /// 매칭된 커맨드
        /// </summary>
        public CommandData command;

        /// <summary>
        /// 진행도 (0~1)
        /// 0 = 시작 안 함, 0.5 = 절반 진행, 1 = 완료
        /// </summary>
        public float progress;

        /// <summary>
        /// 현재까지의 유사도 (0~1)
        /// 1 = 완벽히 일치, 0.7 = 70% 일치
        /// </summary>
        public float similarity;

        /// <summary>
        /// 커맨드가 완성되었는지 여부
        /// </summary>
        public bool isComplete;

        /// <summary>
        /// 생성자
        /// </summary>
        public CommandMatchResult(CommandData command, float progress, float similarity, bool isComplete)
        {
            this.command = command;
            this.progress = progress;
            this.similarity = similarity;
            this.isComplete = isComplete;
        }

        /// <summary>
        /// 디버그 정보
        /// </summary>
        public override string ToString()
        {
            string status = isComplete ? "[완성]" : "[진행중]";
            return $"{status} {command.displayName}: 진행도 {progress:P0}, 유사도 {similarity:P0}";
        }
    }
}