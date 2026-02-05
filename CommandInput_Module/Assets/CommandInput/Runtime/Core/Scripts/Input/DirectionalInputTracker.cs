using System.Collections.Generic;
using UnityEngine;

namespace CommandInput
{
    /// <summary>
    /// 방향 입력의 변화를 감지하고 히스토리를 추적
    /// 중복된 방향 입력을 필터링하고 입력 시퀀스를 기록
    /// </summary>
    public class DirectionalInputTracker
    {
        private InputDirection currentDirection = InputDirection.None;
        private List<InputDirection> directionHistory = new List<InputDirection>();
        private float lastChangeTime;
        private readonly float minChangeInterval;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="minChangeInterval">방향 변화 감지 최소 간격 (초)</param>
        public DirectionalInputTracker(float minChangeInterval = 0.05f)
        {
            this.minChangeInterval = minChangeInterval;
        }

        /// <summary>
        /// 방향 입력 업데이트
        /// </summary>
        /// <param name="input">입력 벡터</param>
        /// <param name="config">입력 설정</param>
        /// <param name="newDirection">변경된 새 방향 (out)</param>
        /// <returns>방향이 변경되었으면 true</returns>
        public bool UpdateDirection(Vector2 input, CommandInputConfig config, out InputDirection newDirection)
        {
            newDirection = DirectionalInputHelper.Vector2ToDirection(input, config);

            // 방향이 변경되고 충분한 시간이 지났는지 확인
            if (newDirection != currentDirection &&
                Time.time - lastChangeTime >= minChangeInterval)
            {
                // None이 아닌 방향만 히스토리에 추가
                if (newDirection != InputDirection.None)
                {
                    currentDirection = newDirection;
                    directionHistory.Add(newDirection);
                    lastChangeTime = Time.time;
                    return true;
                }

                // None으로 변경된 경우 (입력 중립)
                if (newDirection == InputDirection.None && currentDirection != InputDirection.None)
                {
                    currentDirection = InputDirection.None;
                    lastChangeTime = Time.time;
                }
            }

            return false;
        }

        /// <summary>
        /// 현재 방향 가져오기
        /// </summary>
        public InputDirection GetCurrentDirection()
        {
            return currentDirection;
        }

        /// <summary>
        /// 방향 히스토리 가져오기
        /// </summary>
        public List<InputDirection> GetHistory()
        {
            return directionHistory;
        }

        /// <summary>
        /// 히스토리를 배열로 가져오기
        /// </summary>
        public InputDirection[] GetHistoryArray()
        {
            return directionHistory.ToArray();
        }

        /// <summary>
        /// 히스토리 개수
        /// </summary>
        public int GetHistoryCount()
        {
            return directionHistory.Count;
        }

        /// <summary>
        /// 히스토리 초기화
        /// </summary>
        public void Clear()
        {
            directionHistory.Clear();
            currentDirection = InputDirection.None;
        }

        /// <summary>
        /// 히스토리를 화살표 문자열로 변환 (디버깅용)
        /// </summary>
        public string GetHistoryString()
        {
            if (directionHistory.Count == 0)
                return "Empty";

            var arrows = new string[directionHistory.Count];
            for (int i = 0; i < directionHistory.Count; i++)
            {
                arrows[i] = DirectionalInputHelper.DirectionToArrow(directionHistory[i]);
            }

            return string.Join(" → ", arrows);
        }
    }
}