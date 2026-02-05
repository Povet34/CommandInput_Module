#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace CommandInput
{
    public class NewDirectionalInput : IDirectionalInput
    {
        private readonly CommandInputConfig config;
        private readonly InputAction upAction;
        private readonly InputAction downAction;
        private readonly InputAction leftAction;
        private readonly InputAction rightAction;
        private readonly InputAction moveAction;

        public NewDirectionalInput(CommandInputConfig config)
        {
            this.config = config;

            if (config.inputActions == null)
            {
                Debug.LogError("CommandInputConfig에 InputActionAsset이 할당되지 않음!");
                return;
            }

            upAction = config.inputActions.FindAction("Up");
            downAction = config.inputActions.FindAction("Down");
            leftAction = config.inputActions.FindAction("Left");
            rightAction = config.inputActions.FindAction("Right");
            moveAction = config.inputActions.FindAction("Move");

            EnableActions();
        }

        private void EnableActions()
        {
            upAction?.Enable();
            downAction?.Enable();
            leftAction?.Enable();
            rightAction?.Enable();
            moveAction?.Enable();
        }

        public bool GetUpPressed()
        {
            return upAction?.WasPressedThisFrame() ?? false;
        }

        public bool GetDownPressed()
        {
            return downAction?.WasPressedThisFrame() ?? false;
        }

        public bool GetLeftPressed()
        {
            return leftAction?.WasPressedThisFrame() ?? false;
        }

        public bool GetRightPressed()
        {
            return rightAction?.WasPressedThisFrame() ?? false;
        }

        public Vector2 GetDirectionalInput()
        {
            Vector2 value = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;

            // 노이즈 필터링: 임계값 이하 입력 무시
            // Gamepad/Joystick 드리프트나 초기화 노이즈 방지
            if (value.magnitude < config.inputNoiseThreshold)
                return Vector2.zero;

            return value;
        }

        public void Dispose()
        {
            upAction?.Disable();
            downAction?.Disable();
            leftAction?.Disable();
            rightAction?.Disable();
            moveAction?.Disable();
        }
    }
}
#endif