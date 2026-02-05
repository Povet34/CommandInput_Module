using System.Collections.Generic;

namespace CommandInput
{
    /// <summary>
    /// 커맨드 매칭 엔진
    /// 실시간으로 입력을 추적하고 매칭되는 커맨드를 찾음
    /// </summary>
    public class CommandMatcher
    {
        private CommandCollection commandCollection;
        private List<InputDirection> currentInput = new List<InputDirection>();

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection">커맨드 컬렉션</param>
        public CommandMatcher(CommandCollection collection)
        {
            commandCollection = collection;
        }

        /// <summary>
        /// 입력 추가하고 매칭 결과 반환
        /// </summary>
        /// <param name="direction">입력된 방향</param>
        /// <returns>매칭되는 커맨드 결과 목록</returns>
        public List<CommandMatchResult> AddInput(InputDirection direction)
        {
            if (direction == InputDirection.None)
                return GetMatchingCommands();

            currentInput.Add(direction);
            return GetMatchingCommands();
        }

        /// <summary>
        /// 현재 입력과 매칭되는 커맨드들 반환
        /// </summary>
        /// <returns>매칭 결과 목록 (유사도 높은 순)</returns>
        public List<CommandMatchResult> GetMatchingCommands()
        {
            var results = new List<CommandMatchResult>();

            if (currentInput.Count == 0)
                return results;

            if (commandCollection == null || commandCollection.commands == null)
                return results;

            // 모든 커맨드와 비교
            foreach (var command in commandCollection.commands)
            {
                if (command == null)
                    continue;

                var result = CheckCommandMatch(command);
                if (result != null)
                {
                    results.Add(result);
                }
            }

            // 유사도 높은 순으로 정렬
            results.Sort((a, b) => b.similarity.CompareTo(a.similarity));

            return results;
        }

        /// <summary>
        /// 개별 커맨드 매칭 체크
        /// </summary>
        /// <param name="command">체크할 커맨드</param>
        /// <returns>매칭 결과 (매칭 안 되면 null)</returns>
        private CommandMatchResult CheckCommandMatch(CommandData command)
        {
            var commandPattern = command.pattern;
            var inputArray = currentInput.ToArray();

            // 패턴이 비어있으면 무시
            if (commandPattern == null || commandPattern.Length == 0)
                return null;

            // 입력이 패턴보다 너무 길면 무시 (allowExtraInputs 체크)
            if (!command.matchConfig.allowExtraInputs)
            {
                if (inputArray.Length > commandPattern.Length)
                    return null;
            }

            // 유사도 계산
            float similarity = PatternMatcher.CalculateSimilarity(
                commandPattern,
                inputArray,
                command.matchConfig);

            // threshold 이하면 후보에서 제외
            if (similarity < command.matchConfig.similarityThreshold)
                return null;

            // 진행도 계산
            int inputLength = inputArray.Length;
            int patternLength = commandPattern.Length;

            // allowExtraInputs가 true면 슬라이딩 윈도우로 매칭했을 수 있으므로
            // 진행도는 입력 길이 기준
            float progress;
            bool isComplete;

            if (inputLength >= patternLength)
            {
                progress = 1.0f;
                isComplete = true;
            }
            else
            {
                progress = (float)inputLength / patternLength;
                isComplete = false;
            }

            return new CommandMatchResult(command, progress, similarity, isComplete);
        }

        /// <summary>
        /// 완성된 커맨드 찾기 (가장 유사도 높은 것)
        /// </summary>
        /// <returns>완성된 커맨드 중 가장 유사도 높은 것 (없으면 null)</returns>
        public CommandMatchResult GetBestCompletedMatch()
        {
            var matches = GetMatchingCommands();

            CommandMatchResult best = null;
            float bestSimilarity = 0f;

            foreach (var match in matches)
            {
                if (match.isComplete && match.similarity > bestSimilarity)
                {
                    best = match;
                    bestSimilarity = match.similarity;
                }
            }

            return best;
        }

        /// <summary>
        /// 입력 초기화
        /// </summary>
        public void Clear()
        {
            currentInput.Clear();
        }

        /// <summary>
        /// 현재 입력 히스토리 가져오기
        /// </summary>
        public InputDirection[] GetCurrentInput()
        {
            return currentInput.ToArray();
        }

        /// <summary>
        /// 현재 입력 개수
        /// </summary>
        public int GetInputCount()
        {
            return currentInput.Count;
        }

        /// <summary>
        /// 현재 입력을 화살표 문자열로 변환
        /// </summary>
        public string GetInputString()
        {
            if (currentInput.Count == 0)
                return "Empty";

            var arrows = new string[currentInput.Count];
            for (int i = 0; i < currentInput.Count; i++)
            {
                arrows[i] = DirectionalInputHelper.DirectionToArrow(currentInput[i]);
            }

            return string.Join(" ", arrows);
        }
    }
}