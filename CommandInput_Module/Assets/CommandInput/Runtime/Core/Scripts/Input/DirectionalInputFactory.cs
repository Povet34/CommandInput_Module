using UnityEngine;

namespace CommandInput
{
    /// <summary>
    /// 입력 시스템을 생성하는 팩토리 클래스
    /// 설정에 따라 적절한 IDirectionalInput 구현체를 생성
    /// </summary>
    public static class DirectionalInputFactory
    {
        /// <summary>
        /// CommandInputConfig 설정에 따라 입력 시스템 생성
        /// </summary>
        public static IDirectionalInput Create(CommandInputConfig config)
        {
            InputSystemType type = config.inputSystemType;

            // Auto면 자동 감지
            if (type == InputSystemType.Auto)
            {
                type = DetectInputSystem();
            }

            switch (type)
            {
                case InputSystemType.LegacyInput:
                    return CreateLegacyInput(config);

                case InputSystemType.NewInputSystem:
                    return CreateNewInput(config);

                default:
                    Debug.LogWarning($"알 수 없는 InputSystemType: {type}, Legacy Input으로 fallback");
                    return CreateLegacyInput(config);
            }
        }

        /// <summary>
        /// 현재 프로젝트에서 사용 가능한 입력 시스템 자동 감지
        /// </summary>
        private static InputSystemType DetectInputSystem()
        {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            return InputSystemType.NewInputSystem;
#elif ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
            return InputSystemType.LegacyInput;
#elif ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            // 둘 다 활성화된 경우 New Input System 우선
            return InputSystemType.NewInputSystem;
#else
            return InputSystemType.LegacyInput;
#endif
        }

        /// <summary>
        /// Legacy Input 생성
        /// </summary>
        private static IDirectionalInput CreateLegacyInput(CommandInputConfig config)
        {
            Debug.Log("Legacy Input System 사용");
            return new LegacyDirectionalInput(config);
        }

        /// <summary>
        /// New Input System 생성
        /// </summary>
        private static IDirectionalInput CreateNewInput(CommandInputConfig config)
        {
#if ENABLE_INPUT_SYSTEM
            if (config.inputActions == null)
            {
                Debug.LogError("InputActionAsset이 할당되지 않음! Legacy Input으로 fallback");
                return CreateLegacyInput(config);
            }

            Debug.Log("New Input System 사용");
            return new NewDirectionalInput(config);  // ← config 전달
#else
            Debug.LogWarning("New Input System이 활성화되지 않음. Legacy Input으로 fallback");
            return CreateLegacyInput(config);
#endif
        }
    }
}