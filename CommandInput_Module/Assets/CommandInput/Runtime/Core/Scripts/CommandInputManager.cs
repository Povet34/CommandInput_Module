using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace CommandInput
{
    /// <summary>
    /// 커맨드 입력 상태
    /// </summary>
    public enum CommandInputState
    {
        Idle,              // 대기 중 (입력 없음)
        Inputting,         // 입력 중
        Completed,         // 커맨드 완성 (실행 대기)
        Executing          // 실행 중
    }

    /// <summary>
    /// 커맨드 입력 시스템 통합 매니저
    /// 모든 컴포넌트를 통합하여 커맨드 입력을 관리
    /// </summary>
    public class CommandInputManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private CommandInputConfig inputConfig;
        [SerializeField] private CommandCollection commandCollection;

        [Header("Execute Key")]
        [Tooltip("커맨드 실행 키 (None이면 자동 실행)")]
        [SerializeField] private bool useExecuteKey = false;
        [SerializeField] private KeyCode executeKey = KeyCode.Space;

        [Header("Events")]
        [Tooltip("매칭 중인 커맨드가 변경될 때")]
        public UnityEvent<List<CommandMatchResult>> onMatchingCommandsChanged;

        [Tooltip("커맨드가 실행되었을 때")]
        public UnityEvent<CommandData> onCommandExecuted;

        [Tooltip("입력이 초기화되었을 때")]
        public UnityEvent onInputCleared;

        // 컴포넌트들
        private IDirectionalInput directionalInput;
        private DirectionalInputTracker inputTracker;
        private CommandMatcher commandMatcher;

        // 상태
        private CommandInputState currentState = CommandInputState.Idle;
        private float inputStartTime;
        private List<CommandMatchResult> currentMatches = new List<CommandMatchResult>();

        private void Awake()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // 설정 검증
            if (inputConfig == null)
            {
                Debug.LogError("CommandInputManager: InputConfig가 할당되지 않음!");
                enabled = false;
                return;
            }

            if (commandCollection == null)
            {
                Debug.LogError("CommandInputManager: CommandCollection이 할당되지 않음!");
                enabled = false;
                return;
            }

            // 입력 시스템 생성
            directionalInput = DirectionalInputFactory.Create(inputConfig);

            // 입력 추적기 생성
            inputTracker = new DirectionalInputTracker(inputConfig.directionChangeInterval);

            // 매칭 엔진 생성
            commandMatcher = new CommandMatcher(commandCollection);

            Debug.Log("CommandInputManager initialized");
        }

        private void Update()
        {
            if (directionalInput == null)
                return;

            HandleInput();

            // Inputting 상태일 때만 타임아웃 체크
            if (currentState == CommandInputState.Inputting)
            {
                CheckTimeout();
            }
        }

        /// <summary>
        /// 입력 처리
        /// </summary>
        private void HandleInput()
        {
            Vector2 input = directionalInput.GetDirectionalInput();

            // 방향 변화 감지
            if (inputTracker.UpdateDirection(input, inputConfig, out InputDirection newDirection))
            {
                OnDirectionInput(newDirection);
            }

            // 실행 키 체크 (Completed 상태일 때만)
            if (executeKey != KeyCode.None)
            {
                if (useExecuteKey && !Input.GetKeyDown(executeKey))
                    return;

                if (currentState == CommandInputState.Completed)
                {
                    TryExecuteCommand();
                }
            }

            // 조이스틱 중립 체크 (키보드는 무시)
            if (IsUsingAnalogInput() &&
                input.magnitude < inputConfig.deadzone &&
                (currentState == CommandInputState.Inputting || currentState == CommandInputState.Completed))
            {
                OnInputNeutral();
            }
        }

        /// <summary>
        /// 아날로그 입력(조이스틱) 사용 중인지 체크
        /// </summary>
        private bool IsUsingAnalogInput()
        {
            Vector2 input = directionalInput.GetDirectionalInput();

            if (input.magnitude > 0.01f)
            {
                float absX = Mathf.Abs(input.x);
                float absY = Mathf.Abs(input.y);

                bool isDigital = (Mathf.Approximately(absX, 0f) || Mathf.Approximately(absX, 1f)) &&
                                (Mathf.Approximately(absY, 0f) || Mathf.Approximately(absY, 1f));

                return !isDigital;
            }

            return false;
        }

        /// <summary>
        /// 방향 입력 발생
        /// </summary>
        private void OnDirectionInput(InputDirection direction)
        {
            if (currentState == CommandInputState.Idle || currentState == CommandInputState.Completed)
            {
                ChangeState(CommandInputState.Inputting);
                inputStartTime = Time.time;
            }

            // 매칭 엔진에 입력 추가
            currentMatches = commandMatcher.AddInput(direction);

            // 매칭 변경 이벤트 발생
            onMatchingCommandsChanged?.Invoke(currentMatches);

            // 완성 체크
            CheckCompletedCommands();
        }

        /// <summary>
        /// 완성된 커맨드 체크
        /// </summary>
        private void CheckCompletedCommands()
        {
            foreach (var match in currentMatches)
            {
                if (match.isComplete)
                {
                    // 상태 전이: Inputting → Completed
                    ChangeState(CommandInputState.Completed);
                    Debug.Log($"Command completed: {match.command.displayName} (similarity: {match.similarity:P0})");
                    return;
                }
            }
        }

        /// <summary>
        /// 입력 중립 상태로 돌아옴 (조이스틱)
        /// </summary>
        private void OnInputNeutral()
        {
            if (currentState == CommandInputState.Completed)
            {
                // 완성된 커맨드 실행
                TryExecuteCommand();
            }
            else if (currentState == CommandInputState.Inputting)
            {
                // 미완성 → 초기화
                ClearInput();
            }
        }

        /// <summary>
        /// 커맨드 실행 시도
        /// </summary>
        private void TryExecuteCommand()
        {
            var completedCommand = commandMatcher.GetBestCompletedMatch();
            if (completedCommand != null)
            {
                ExecuteCommand(completedCommand.command);
            }
            else
            {
                Debug.Log("<color=yellow>Command not completed</color>");
                ClearInput();
            }
        }

        /// <summary>
        /// 커맨드 실행
        /// </summary>
        private void ExecuteCommand(CommandData command)
        {
            ChangeState(CommandInputState.Executing);

            Debug.Log($"<color=green>Command Executed: {command.displayName}</color>");

            onCommandExecuted?.Invoke(command);

            ClearInput();
        }

        /// <summary>
        /// 타임아웃 체크 (Inputting 상태일 때만)
        /// </summary>
        private void CheckTimeout()
        {
            // 가장 긴 제한 시간 찾기
            float maxDuration = 0f;
            foreach (var match in currentMatches)
            {
                maxDuration = Mathf.Max(maxDuration, match.command.maxInputDuration);
            }

            // 매칭되는 커맨드가 없으면 기본값
            if (maxDuration == 0f)
                maxDuration = 2f;

            // 시간 초과 체크
            if (Time.time - inputStartTime > maxDuration)
            {
                Debug.Log("<color=yellow>Input timeout</color>");
                ClearInput();
            }
        }

        /// <summary>
        /// 상태 변경
        /// </summary>
        private void ChangeState(CommandInputState newState)
        {
            if (currentState == newState)
                return;

            Debug.Log($"State: {currentState} → {newState}");
            currentState = newState;
        }

        /// <summary>
        /// 입력 초기화
        /// </summary>
        public void ClearInput()
        {
            commandMatcher.Clear();
            inputTracker.Clear();
            currentMatches.Clear();

            ChangeState(CommandInputState.Idle);

            onInputCleared?.Invoke();
        }


        /// <summary>
        /// 현재 입력 상태
        /// </summary>
        public CommandInputState GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// 현재 입력 히스토리
        /// </summary>
        public InputDirection[] GetCurrentInput()
        {
            return commandMatcher.GetCurrentInput();
        }

        /// <summary>
        /// 현재 매칭되는 커맨드들
        /// </summary>
        public List<CommandMatchResult> GetMatchingCommands()
        {
            return new List<CommandMatchResult>(currentMatches);
        }

        /// <summary>
        /// 입력 진행 중인지
        /// </summary>
        public bool IsInputActive()
        {
            return currentState != CommandInputState.Idle;
        }

        /// <summary>
        /// 현재 입력을 문자열로 변환
        /// </summary>
        public string GetCurrentInputString()
        {
            return commandMatcher.GetInputString();
        }

        private void OnDestroy()
        {
#if ENABLE_INPUT_SYSTEM
            // New Input System 정리
            if (directionalInput is NewDirectionalInput newInput)
            {
                newInput.Dispose();
            }
#endif
        }
    }
}