using System.Collections.Generic;
using UnityEngine;
using CommandInput;

namespace CommandInput.UI
{
    /// <summary>
    /// Stratagem 카드들을 관리하는 매니저
    /// CommandInputManager와 연동하여 실시간으로 UI 업데이트
    /// </summary>
    public class StratagemDisplayManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CommandInputManager commandInputManager;
        [SerializeField] private Transform stratagemArea;
        [SerializeField] private GameObject stratagemCardPrefab;

        [Header("Command Data")]
        [Tooltip("표시할 커맨드 컬렉션")]
        [SerializeField] private CommandCollection commandCollection;

        [Tooltip("또는 직접 커맨드 배열 지정")]
        [SerializeField] private CommandData[] commands;

        [Header("Settings")]
        [Tooltip("초기에 모든 카드 표시")]
        [SerializeField] private bool showAllCardsInitially = true;

        [Tooltip("매칭 안 되는 카드 딤드 처리 (알파값)")]
        [SerializeField] private float dimmedAlpha = 0.3f;


        // 카드 관리
        private Dictionary<string, StratagemCardUI> cardMap = new Dictionary<string, StratagemCardUI>();
        private List<StratagemCardUI> allCards = new List<StratagemCardUI>();

        // 현재 입력
        private InputDirection[] currentInput;

        private void Start()
        {
            if (commandInputManager == null)
            {
                Debug.LogError("CommandInputManager가 할당되지 않음!");
                enabled = false;
                return;
            }

            // 이벤트 구독
            commandInputManager.onMatchingCommandsChanged.AddListener(OnMatchingCommandsChanged);
            commandInputManager.onCommandExecuted.AddListener(OnCommandExecuted);
            commandInputManager.onInputCleared.AddListener(OnInputCleared);

            // 초기화
            InitializeCards();
        }

        /// <summary>
        /// 카드 초기화 - 모든 커맨드에 대해 카드 생성
        /// </summary>
        private void InitializeCards()
        {
            // 표시할 커맨드 목록 가져오기
            List<CommandData> commandsToDisplay = GetCommandsToDisplay();

            if (commandsToDisplay == null || commandsToDisplay.Count == 0)
            {
                Debug.LogWarning("표시할 커맨드가 없음!");
                return;
            }

            // 각 커맨드에 대해 카드 생성
            foreach (var command in commandsToDisplay)
            {
                if (command == null)
                    continue;

                CreateCard(command);
            }

            Debug.Log($"Initialized {allCards.Count} stratagem cards");
        }

        /// <summary>
        /// 표시할 커맨드 목록 가져오기
        /// </summary>
        private List<CommandData> GetCommandsToDisplay()
        {
            // 1. 직접 지정된 배열 우선
            if (commands != null && commands.Length > 0)
            {
                return new List<CommandData>(commands);
            }

            // 2. CommandCollection에서 가져오기
            if (commandCollection != null)
            {
                return commandCollection.GetValidCommands();
            }

            // 3. CommandInputManager의 컬렉션 사용
            // (추후 구현 가능)

            return new List<CommandData>();
        }

        /// <summary>
        /// 카드 생성
        /// </summary>
        private void CreateCard(CommandData command)
        {
            GameObject cardObj = Instantiate(stratagemCardPrefab, stratagemArea);
            StratagemCardUI card = cardObj.GetComponent<StratagemCardUI>();

            if (card == null)
            {
                Debug.LogError("StratagemCardPrefab에 StratagemCardUI 컴포넌트가 없음!");
                Destroy(cardObj);
                return;
            }

            // 카드 초기화
            card.Setup(command);
            card.ResetProgress();

            // 카드 등록
            cardMap[command.commandId] = card;
            allCards.Add(card);

            // 초기 표시 여부
            if (!showAllCardsInitially)
            {
                cardObj.SetActive(false);
            }
        }

        /// <summary>
        /// 매칭 커맨드 변경 시
        /// </summary>
        private void OnMatchingCommandsChanged(List<CommandMatchResult> matches)
        {
            // 현재 입력 업데이트
            currentInput = commandInputManager.GetCurrentInput();
            int inputCount = currentInput.Length;

            if (matches == null || matches.Count == 0)
            {
                // 매칭되는 게 없으면 모든 카드 초기화
                ResetAllCards();
                return;
            }

            // 매칭되는 커맨드 ID 수집
            HashSet<string> matchingIds = new HashSet<string>();
            foreach (var match in matches)
            {
                matchingIds.Add(match.command.commandId);
            }

            // 모든 카드 업데이트
            foreach (var card in allCards)
            {
                var commandData = card.GetCommandData();

                if (matchingIds.Contains(commandData.commandId))
                {
                    // 매칭되는 카드
                    card.gameObject.SetActive(true);
                    SetCardDimmed(card, false);

                    // 진행도 업데이트
                    // inputCount와 패턴 길이 중 작은 값 사용
                    int progressCount = Mathf.Min(inputCount, card.GetPatternLength());
                    card.UpdateProgress(progressCount);
                }
                else
                {
                    // 매칭 안 되는 카드 - 딤드 처리
                    if (inputCount > 0)
                    {
                        SetCardDimmed(card, true);
                        card.ResetProgress();
                    }
                    else
                    {
                        // 입력 없으면 정상 표시
                        SetCardDimmed(card, false);
                        card.ResetProgress();
                    }
                }
            }
        }

        /// <summary>
        /// 카드 딤드 처리
        /// </summary>
        private void SetCardDimmed(StratagemCardUI card, bool dimmed)
        {
            card.SetCardAlpha(dimmed ? dimmedAlpha : 1f);
        }

        /// <summary>
        /// 모든 카드 초기화
        /// </summary>
        private void ResetAllCards()
        {
            foreach (var card in allCards)
            {
                card.ResetProgress();
                SetCardDimmed(card, false);
            }
        }

        /// <summary>
        /// 커맨드 실행 시
        /// </summary>
        private void OnCommandExecuted(CommandData command)
        {
            Debug.Log($"<color=cyan>Stratagem executed: {command.displayName}</color>");

            // 실행된 카드 강조 효과 (선택사항)
            if (cardMap.TryGetValue(command.commandId, out StratagemCardUI card))
            {
                // TODO: 애니메이션이나 이펙트 추가
            }
        }

        /// <summary>
        /// 입력 초기화 시
        /// </summary>
        private void OnInputCleared()
        {
            ResetAllCards();
        }

        private void OnDestroy()
        {
            if (commandInputManager != null)
            {
                commandInputManager.onMatchingCommandsChanged.RemoveListener(OnMatchingCommandsChanged);
                commandInputManager.onCommandExecuted.RemoveListener(OnCommandExecuted);
                commandInputManager.onInputCleared.RemoveListener(OnInputCleared);
            }
        }
    }
}