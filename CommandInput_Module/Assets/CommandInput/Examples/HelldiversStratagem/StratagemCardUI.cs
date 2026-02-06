using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;

namespace CommandInput.UI
{
    /// <summary>
    /// 개별 Stratagem 카드 UI
    /// 커맨드 정보와 입력 진행도를 표시
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
        [SerializeField] private Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 회색


        [Tooltip("입력된 색상")]
        [SerializeField] private Color inputColor = Color.white; // 하얀색

        [Header("Arrow Settings")]
        [Tooltip("화살표 기본 방향 (0도 = Right)")]
        [SerializeField] private float defaultArrowRotation = 0f;

        private CommandData commandData;
        private Image[] directionIcons;
        private CanvasGroup canvasGroup;

        /// <summary>
        /// 카드 초기화
        /// </summary>
        public void Setup(CommandData command)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            commandIconTemplate.gameObject.SetActive(false);

            commandData = command;

            // 커맨드 이름 설정
            if (commandText != null)
            {
                commandText.text = command.displayName.ToUpper();
            }

            // 아이콘 설정
            if (icon != null && command.icon != null)
            {
                icon.sprite = command.icon;
                icon.enabled = true;
            }
            else if (icon != null)
            {
                icon.enabled = false;
            }

            // 필요한 입력 패턴 표시
            CreatePatternIcons();
        }

        /// <summary>
        /// 입력 패턴 아이콘 생성
        /// </summary>
        private void CreatePatternIcons()
        {
            if (commandArea == null || commandIconTemplate == null || commandData == null)
                return;

            // 기존 아이콘 제거
            ClearPatternIcons();

            var pattern = commandData.pattern;
            if (pattern == null || pattern.Length == 0)
                return;

            // 패턴 길이만큼 아이콘 생성
            directionIcons = new Image[pattern.Length];

            for (int i = 0; i < pattern.Length; i++)
            {
                GameObject iconObj = Instantiate(commandIconTemplate, commandArea);
                iconObj.SetActive(true);

                Image iconImage = iconObj.GetComponent<Image>();
                if (iconImage != null)
                {
                    directionIcons[i] = iconImage;

                    // 회전 적용 (InputDirection의 각도 값 사용)
                    float angle = DirectionalInputHelper.DirectionToAngle(pattern[i]);
                    iconObj.transform.rotation = Quaternion.Euler(0, 0, angle + defaultArrowRotation);

                    // 기본 색상 (회색)
                    iconImage.color = defaultColor;
                }
            }
        }

        /// <summary>
        /// 기존 패턴 아이콘 제거
        /// </summary>
        private void ClearPatternIcons()
        {
            if (commandArea == null)
                return;

            // Template 제외하고 모두 삭제
            foreach (Transform child in commandArea)
            {
                if (child.gameObject != commandIconTemplate)
                {
                    Destroy(child.gameObject);
                }
            }

            directionIcons = null;
        }

        /// <summary>
        /// 진행도 업데이트
        /// </summary>
        /// <param name="inputCount">현재 입력된 개수</param>
        public void UpdateProgress(int inputCount)
        {
            if (directionIcons == null)
                return;

            for (int i = 0; i < directionIcons.Length; i++)
            {
                if (directionIcons[i] == null)
                    continue;

                if (i < inputCount)
                {
                    // 입력된 부분 - 하얀색
                    directionIcons[i].color = inputColor;
                }
                else
                {
                    // 아직 입력 안 된 부분 - 회색
                    directionIcons[i].color = defaultColor;
                }
            }
        }

        /// <summary>
        /// 진행도 초기화 (모두 회색으로)
        /// </summary>
        public void ResetProgress()
        {
            if (directionIcons == null)
                return;

            foreach (var icon in directionIcons)
            {
                if (icon != null)
                {
                    icon.color = defaultColor;
                }
            }
        }

        /// <summary>
        /// 현재 커맨드 데이터
        /// </summary>
        public CommandData GetCommandData()
        {
            return commandData;
        }

        /// <summary>
        /// 패턴 길이
        /// </summary>
        public int GetPatternLength()
        {
            return commandData?.pattern?.Length ?? 0;
        }

        public void SetCardAlpha(float alpha)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
        }
    }
}