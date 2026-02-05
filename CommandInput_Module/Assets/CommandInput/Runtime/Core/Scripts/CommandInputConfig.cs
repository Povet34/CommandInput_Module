using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace CommandInput
{
    /// <summary>
    /// 방향 입력 모드 (4방향 또는 8방향)
    /// </summary>
    public enum DirectionalMode
    {
        FourWay,    // 4방향만 (↑↓←→)
        EightWay    // 8방향 전부 (↑↓←→ + 대각선)
    }
    
    /// <summary>
    /// 입력 시스템 타입
    /// </summary>
    public enum InputSystemType
    {
        Auto,              // 자동 감지
        LegacyInput,       // 구식 Input Manager
        NewInputSystem     // New Input System
    }
    
    /// <summary>
    /// 커맨드 입력 시스템의 전역 설정
    /// </summary>
    [CreateAssetMenu(fileName = "CommandInputConfig", menuName = "CommandInput/Input Config")]
    public class CommandInputConfig : ScriptableObject
    {
        [Header("Directional Settings")]
        [Tooltip("4방향 또는 8방향 입력 모드")]
        public DirectionalMode directionalMode = DirectionalMode.EightWay;
        
        [Tooltip("조이스틱 데드존 (이 값 이하는 무시)")]
        [Range(0.1f, 0.9f)]
        public float deadzone = 0.3f;
        
        [Tooltip("방향 변화 감지 최소 간격 (초)")]
        [Range(0.01f, 0.2f)]
        public float directionChangeInterval = 0.05f;
        
        [Header("Input System")]
        [Tooltip("사용할 입력 시스템 (Auto는 자동 감지)")]
        public InputSystemType inputSystemType = InputSystemType.Auto;
        
        [Header("Legacy Input Settings")]
        [Tooltip("Legacy Input용 키 설정")]
        public KeyCode upKey = KeyCode.UpArrow;
        public KeyCode downKey = KeyCode.DownArrow;
        public KeyCode leftKey = KeyCode.LeftArrow;
        public KeyCode rightKey = KeyCode.RightArrow;
        
        [Tooltip("조이스틱 축 이름")]
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";
        
#if ENABLE_INPUT_SYSTEM
        [Header("New Input System Settings")]
        [Tooltip("New Input System용 InputActionAsset")]
        public InputActionAsset inputActions;
#endif
        
        /// <summary>
        /// 현재 설정이 유효한지 검증
        /// </summary>
        public bool Validate()
        {
#if ENABLE_INPUT_SYSTEM
            if (inputSystemType == InputSystemType.NewInputSystem && inputActions == null)
            {
                Debug.LogError("InputSystemType이 NewInputSystem인데 InputActionAsset이 할당되지 않음!");
                return false;
            }
#else
            if (inputSystemType == InputSystemType.NewInputSystem)
            {
                Debug.LogWarning("New Input System이 활성화되지 않았습니다. Legacy Input으로 fallback합니다.");
                return false;
            }
#endif
            return true;
        }
    }
}