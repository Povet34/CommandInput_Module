#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace CommandInput
{
    /// <summary>
    /// New Input System을 사용하는 방향 입력 구현
    /// InputActionAsset 기반으로 동작
    /// </summary>
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

            // Config에서 InputActionAsset 가져오기
            if (config.inputActions == null)
            {
                Debug.LogError("CommandInputConfig에 InputActionAsset이 할당되지 않음!");
                return;
            }

            // InputActionAsset에서 액션 찾기
            upAction = config.inputActions.FindAction("Up");
            downAction = config.inputActions.FindAction("Down");
            leftAction = config.inputActions.FindAction("Left");
            rightAction = config.inputActions.FindAction("Right");
            moveAction = config.inputActions.FindAction("Move");

            // 액션 활성화
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
            return moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        /// <summary>
        /// 리소스 정리
        /// </summary>
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