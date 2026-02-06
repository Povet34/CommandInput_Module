using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace CommandInput.UI
{
    /// <summary>
    /// 개별 Stratagem 카드 UI
    /// 스스로 매칭 판단하고 UI 업데이트
    /// </summary>
    public class StratagemCardUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI commandText;
        [SerializeField] private Transform commandArea;
        [SerializeField] private GameObject commandIconTemplate;

        [Header("Visual Settings")]
        [Tooltip("기본 색상 (입력 안 됨)")]
        [SerializeField] private Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        [Tooltip("입력된 색상")]
        [SerializeField] private Color inputColor = Color.white;

        [Tooltip("딤드 알파값")]
        [SerializeField] private float dimmedAlpha = 0.3f;

        [Header("Arrow Settings")]
        [Tooltip("화살표 기본 방향 (0도 = Right)")]
        [SerializeField] private float defaultArrowRotation = 0f;

        private CommandData myCommand;
        private Image[] directionIcons;
        private CanvasGroup canvasGroup;

        /// <summary>
        /// 카드 초기화
        /// </summary>
        public void Initialize(CommandData command)
        {
            myCommand = command;

            canvasGroup = GetComponent<CanvasGroup>();

            // UI 설정
            SetupUI();
            CreatePatternIcons();

            // 이벤트 구독
            SubscribeEvents();
        }

        private void SetupUI()
        {
            if (commandText != null)
            {
                commandText.text = myCommand.displayName.ToUpper();
            }

            if (icon != null && myCommand.icon != null)
            {
                icon.sprite = myCommand.icon;
                icon.enabled = true;
            }
            else if (icon != null)
            {
                icon.enabled = false;
            }
        }

        private void CreatePatternIcons()
        {
            if (commandArea == null || commandIconTemplate == null || myCommand == null)
                return;

            var pattern = myCommand.pattern;
            if (pattern == null || pattern.Length == 0)
                return;

            directionIcons = new Image[pattern.Length];

            for (int i = 0; i < pattern.Length; i++)
            {
                GameObject iconObj = Instantiate(commandIconTemplate, commandArea);
                iconObj.SetActive(true);

                Image iconImage = iconObj.GetComponent<Image>();
                if (iconImage != null)
                {
                    directionIcons[i] = iconImage;

                    // 회전 적용
                    float angle = DirectionalInputHelper.DirectionToAngle(pattern[i]);
                    iconObj.transform.rotation = Quaternion.Euler(0, 0, angle + defaultArrowRotation);

                    // 기본 색상
                    iconImage.color = defaultColor;
                }
            }
        }

        private void SubscribeEvents()
        {
            StratagemDisplayManager.OnMatchingResultsUpdated += OnMatchingResultsHandler;
            StratagemDisplayManager.OnInputCleared += OnInputClearedHandler;
            StratagemDisplayManager.OnCommandExecuted += OnCommandExecutedHandler;
        }

        /// <summary>
        /// 매칭 결과 업데이트
        /// </summary>
        private void OnMatchingResultsHandler(List<CommandMatchResult> results)
        {
            if (results == null || results.Count == 0)
            {
                // 매칭 없음
                ResetProgress();
                SetDimmed(false);
                return;
            }

            // 자기 것 찾기
            CommandMatchResult myResult = results.Find(r => r.command.commandId == myCommand.commandId);

            if (myResult != null)
            {
                // 매칭됨
                int inputCount = Mathf.RoundToInt(myResult.progress * myCommand.pattern.Length);
                UpdateProgress(inputCount);
                SetDimmed(false);
            }
            else
            {
                // 매칭 안 됨 (다른 커맨드가 매칭 중)
                ResetProgress();
                SetDimmed(true);
            }
        }

        /// <summary>
        /// 입력 초기화
        /// </summary>
        private void OnInputClearedHandler()
        {
            ResetProgress();
            SetDimmed(false);
        }

        /// <summary>
        /// 커맨드 실행
        /// </summary>
        private void OnCommandExecutedHandler(string commandId)
        {
            if (commandId == myCommand.commandId)
            {
                // 실행 애니메이션 (TODO)
                Debug.Log($"Card {myCommand.displayName} executed!");
            }
        }

        /// <summary>
        /// 진행도 업데이트
        /// </summary>
        private void UpdateProgress(int inputCount)
        {
            if (directionIcons == null) return;

            for (int i = 0; i < directionIcons.Length; i++)
            {
                if (directionIcons[i] == null) continue;

                directionIcons[i].color = i < inputCount ? inputColor : defaultColor;
            }
        }

        /// <summary>
        /// 진행도 리셋
        /// </summary>
        private void ResetProgress()
        {
            if (directionIcons == null) return;

            foreach (var icon in directionIcons)
            {
                if (icon != null)
                {
                    icon.color = defaultColor;
                }
            }
        }

        /// <summary>
        /// 딤드 처리
        /// </summary>
        private void SetDimmed(bool dimmed)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = dimmed ? dimmedAlpha : 1f;
            }
        }

        private void OnDestroy()
        {
            StratagemDisplayManager.OnMatchingResultsUpdated -= OnMatchingResultsHandler;
            StratagemDisplayManager.OnInputCleared -= OnInputClearedHandler;
            StratagemDisplayManager.OnCommandExecuted -= OnCommandExecutedHandler;
        }
    }
}