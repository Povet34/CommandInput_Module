namespace CommandInput
{
    /// <summary>
    /// 8방향 입력을 나타내는 열거형
    /// 각 방향은 각도로 정의됨 (Right = 0도, Up = 90도)
    /// </summary>
    public enum InputDirection
    {
        None = -1,
        Right = 0,       // →  (0도)
        UpRight = 45,    // ↗  (45도)
        Up = 90,         // ↑  (90도)
        UpLeft = 135,    // ↖  (135도)
        Left = 180,      // ←  (180도)
        DownLeft = 225,  // ↙  (225도)
        Down = 270,      // ↓  (270도)
        DownRight = 315  // ↘  (315도)
    }
}