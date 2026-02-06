using System.Collections.Generic;
using UnityEngine;
using CommandInput;

namespace CommandInput.UI
{
    /// <summary>
    /// Stratagem 카드 컨테이너
    /// 카드 생성과 이벤트 중계만 담당
    /// </summary>
    public class StratagemDisplayManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CommandInputManager commandInputManager;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private GameObject cardPrefab;

        [Header("Command Data")]
        [SerializeField] private CommandCollection commandCollection;
        [SerializeField] private CommandData[] commands;

        private List<StratagemCardUI> cards = new List<StratagemCardUI>();

        // 전역 이벤트 (Card들이 구독)
        public static event System.Action<List<CommandMatchResult>> OnMatchingResultsUpdated;
        public static event System.Action OnInputCleared;
        public static event System.Action<string> OnCommandExecuted;

        private void Start()
        {
            if (commandInputManager == null)
            {
                Debug.LogError("CommandInputManager가 할당되지 않음!");
                enabled = false;
                return;
            }

            // CommandInputManager 이벤트 구독 → 중계
            commandInputManager.onMatchingCommandsChanged.AddListener(RelayMatchingResults);
            commandInputManager.onInputCleared.AddListener(RelayInputCleared);
            commandInputManager.onCommandExecuted.AddListener(RelayCommandExecuted);

            // 카드 생성
            CreateCards();
        }

        /// <summary>
        /// 카드 생성
        /// </summary>
        private void CreateCards()
        {
            var commandsToDisplay = GetCommandsToDisplay();

            if (commandsToDisplay == null || commandsToDisplay.Count == 0)
            {
                Debug.LogWarning("표시할 커맨드가 없음!");
                return;
            }

            foreach (var command in commandsToDisplay)
            {
                if (command == null) continue;

                GameObject cardObj = Instantiate(cardPrefab, cardContainer);
                StratagemCardUI card = cardObj.GetComponent<StratagemCardUI>();

                if (card != null)
                {
                    card.Initialize(command);
                    cards.Add(card);
                }
            }

            Debug.Log($"Created {cards.Count} stratagem cards");
        }

        /// <summary>
        /// 표시할 커맨드 목록 가져오기
        /// </summary>
        private List<CommandData> GetCommandsToDisplay()
        {
            if (commands != null && commands.Length > 0)
            {
                return new List<CommandData>(commands);
            }

            if (commandCollection != null)
            {
                return commandCollection.GetValidCommands();
            }

            return new List<CommandData>();
        }

        /// <summary>
        /// 매칭 결과 중계
        /// </summary>
        private void RelayMatchingResults(List<CommandMatchResult> results)
        {
            OnMatchingResultsUpdated?.Invoke(results);
        }

        /// <summary>
        /// 입력 초기화 중계
        /// </summary>
        private void RelayInputCleared()
        {
            OnInputCleared?.Invoke();
        }

        /// <summary>
        /// 커맨드 실행 중계
        /// </summary>
        private void RelayCommandExecuted(CommandData command)
        {
            OnCommandExecuted?.Invoke(command.commandId);
        }

        /// <summary>
        /// 모든 카드 리셋
        /// </summary>
        public void ResetAllCards()
        {
            OnInputCleared?.Invoke();
        }

        private void OnDestroy()
        {
            if (commandInputManager != null)
            {
                commandInputManager.onMatchingCommandsChanged.RemoveListener(RelayMatchingResults);
                commandInputManager.onInputCleared.RemoveListener(RelayInputCleared);
                commandInputManager.onCommandExecuted.RemoveListener(RelayCommandExecuted);
            }
        }
    }
}