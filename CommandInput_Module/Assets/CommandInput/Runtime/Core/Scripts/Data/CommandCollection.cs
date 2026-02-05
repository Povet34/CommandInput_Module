using System.Collections.Generic;
using UnityEngine;

namespace CommandInput
{
    /// <summary>
    /// 커맨드 모음
    /// 여러 CommandData를 관리하고 검색 기능 제공
    /// </summary>
    [CreateAssetMenu(fileName = "NewCommandCollection", menuName = "CommandInput/Command Collection")]
    public class CommandCollection : ScriptableObject
    {
        [Header("Commands")]
        [Tooltip("관리할 커맨드 목록")]
        public List<CommandData> commands = new List<CommandData>();

        /// <summary>
        /// ID로 커맨드 찾기
        /// </summary>
        public CommandData GetCommand(string commandId)
        {
            if (string.IsNullOrEmpty(commandId))
                return null;

            return commands.Find(c => c != null && c.commandId == commandId);
        }

        /// <summary>
        /// 인덱스로 커맨드 가져오기
        /// </summary>
        public CommandData GetCommandAt(int index)
        {
            if (index < 0 || index >= commands.Count)
                return null;

            return commands[index];
        }

        /// <summary>
        /// 커맨드 개수
        /// </summary>
        public int GetCommandCount()
        {
            return commands.Count;
        }

        /// <summary>
        /// 모든 커맨드 가져오기
        /// </summary>
        public List<CommandData> GetAllCommands()
        {
            return new List<CommandData>(commands);
        }

        /// <summary>
        /// 유효한 커맨드만 가져오기 (null 제외)
        /// </summary>
        public List<CommandData> GetValidCommands()
        {
            var validCommands = new List<CommandData>();

            foreach (var command in commands)
            {
                if (command != null && command.Validate())
                {
                    validCommands.Add(command);
                }
            }

            return validCommands;
        }

        /// <summary>
        /// 컬렉션 검증
        /// </summary>
        public bool Validate()
        {
            if (commands == null || commands.Count == 0)
            {
                Debug.LogWarning($"CommandCollection '{name}': 커맨드가 비어있음!");
                return false;
            }

            // null 커맨드 체크
            int nullCount = 0;
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i] == null)
                {
                    Debug.LogWarning($"CommandCollection '{name}': 인덱스 {i}의 커맨드가 null");
                    nullCount++;
                }
            }

            // 중복 ID 체크
            var idSet = new HashSet<string>();
            var duplicates = new List<string>();

            foreach (var command in commands)
            {
                if (command == null) continue;

                if (!string.IsNullOrEmpty(command.commandId))
                {
                    if (!idSet.Add(command.commandId))
                    {
                        duplicates.Add(command.commandId);
                    }
                }
            }

            if (duplicates.Count > 0)
            {
                Debug.LogError($"CommandCollection '{name}': 중복된 commandId 발견: {string.Join(", ", duplicates)}");
                return false;
            }

            // 개별 커맨드 검증
            int invalidCount = 0;
            foreach (var command in commands)
            {
                if (command != null && !command.Validate())
                {
                    invalidCount++;
                }
            }

            if (nullCount > 0 || invalidCount > 0)
            {
                Debug.LogWarning($"CommandCollection '{name}': {nullCount}개 null, {invalidCount}개 invalid");
            }

            return duplicates.Count == 0;
        }

        /// <summary>
        /// 디버그 정보 출력
        /// </summary>
        public string GetDebugInfo()
        {
            var info = $"CommandCollection '{name}':\n";
            info += $"Total: {commands.Count} commands\n";

            int validCount = 0;
            foreach (var command in commands)
            {
                if (command != null && command.Validate())
                {
                    validCount++;
                    info += $"  - {command.commandId}: {command.GetPatternString()}\n";
                }
            }

            info += $"Valid: {validCount}/{commands.Count}";

            return info;
        }
    }
}