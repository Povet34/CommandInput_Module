# CommandInput_Module
CommandInput_Module (like HELLDIVERS command)


https://github.com/user-attachments/assets/cf32a436-719d-44ba-8cbb-bce560c9aaa7


설치 방법

- Unity PackageManger Git url
  ```C#
  https://github.com/Povet34/CommandInput_Module.git?path=/CommandInput_Module/Assets/CommandInput
  
  ```

# Command Input System

헬다이버즈2 스트라타젬 스타일의 방향키 조합 커맨드 입력 시스템

## Features

- 8방향 패턴 인식 (↓→↑↓ 등)
- 부정확한 입력도 유사도 기반 매칭
- Legacy Input / New Input System 자동 전환
- ScriptableObject 데이터 관리

## Quick Start

### Setup

1. 프로젝트에 `CommandInput` 폴더 추가
2. 에셋 생성:
   - `Create > CommandInput > Input Config`
   - `Create > CommandInput > Command Data`
   - `Create > CommandInput > Command Collection`

### Example: Helldivers 2 Stratagems

**Resupply** `↓↓↑→`
```
Command ID: resupply
Display Name: Resupply
Pattern: Down, Down, Up, Right
Similarity Threshold: 0.75
Max Input Duration: 1.5
```

**500kg Bomb** `↑→↓↓↓`
```
Command ID: bomb_500kg
Display Name: 500kg Bomb
Pattern: Down, Down, Down, Up, Up
Similarity Threshold: 0.8
Max Input Duration: 2.0
```

### Implementation
```csharp
using CommandInput;
using UnityEngine;

public class StratagemController : MonoBehaviour
{
    [SerializeField] private CommandInputManager manager;
    
    void Start()
    {
        manager.onCommandExecuted.AddListener(OnStratagemCalled);
    }
    
    void OnStratagemCalled(CommandData command)
    {
        switch (command.commandId)
        {
            case "orbital_railcannon":
                CallOrbitalStrike();
                break;
            case "resupply":
                RequestResupply();
                break;
            case "bomb_500kg":
                Drop500kgBomb();
                break;
        }
    }
    
    void CallOrbitalStrike()
    {
        Debug.Log("Orbital Railcannon Strike Incoming!");
        // Your game logic here
    }
}
```

**Scene Setup:**
1. GameObject에 `CommandInputManager` 추가
2. Input Config 할당
3. Command Collection 할당
4. Use Execute Key: off

## Configuration

### Input Config
```
Directional Mode: EightWay
Deadzone: 0.3
Direction Change Interval: 0.05
Input System Type: Auto
```

### Pattern Match Config
```
Similarity Threshold: 0.7 (낮을수록 관대함)
Max Angle Difference: 45 (허용 각도)
Allow Extra Inputs: true (↓→↑↓↑ 입력 시 ↓→↑↓ 인식)
```

## Events
```csharp
// 커맨드 실행 시
manager.onCommandExecuted.AddListener(command => { });

// 매칭 중인 커맨드 변경 시 (UI 업데이트용)
manager.onMatchingCommandsChanged.AddListener(matches => { });

// 입력 초기화 시
manager.onInputCleared.AddListener(() => { });
```

## UI Feedback
```csharp
void UpdateUI(List matches)
{
    if (matches.Count == 0) return;
    
    var best = matches[0];
    
    progressBar.fillAmount = best.progress;
    nameText.text = best.command.displayName;
    similarityText.text = $"{best.similarity:P0}";
    
    if (best.isComplete)
        nameText.color = Color.green;
}
```

## API
```csharp
// 현재 상태
CommandInputState state = manager.GetCurrentState();
// Idle, Inputting, Completed, Executing

// 현재 입력
InputDirection[] input = manager.GetCurrentInput();

// 현재 입력 문자열
string arrows = manager.GetCurrentInputString(); // "↓ → ↑"

// 매칭 중인 커맨드
List matches = manager.GetMatchingCommands();

// 입력 초기화
manager.ClearInput();
```
