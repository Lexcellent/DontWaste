using System;
using System.IO;
using System.Reflection;
using Duckov.UI;
using ItemStatsSystem;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace DontWaste
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public static int Probability = 30;

        protected override void OnAfterSetup()
        {
            LoadConfig();
            Item.onUseStatic -= OnUseStatic;
            Item.onUseStatic += OnUseStatic;
        }

        protected override void OnBeforeDeactivate()
        {
            Item.onUseStatic -= OnUseStatic;
        }

        void OnUseStatic(Item item, object user)
        {
            var characterMainControl = (CharacterMainControl)user;
            // 如果是粑粑
            if (item.TypeID == 938 && characterMainControl.IsMainCharacter)
            {
                var range = Random.Range(0, 100);
                Debug.Log($"DontWaste模组：概率为{range}");
                if (range < Probability)
                {
                    // 克隆一个targetItem
                    var clonedItem = Instantiate(item);
                    clonedItem.StackCount = 1;
                    // 丢到玩家附近
                    if ((bool)(Object)LevelManager.Instance?.MainCharacter)
                    {
                        clonedItem.Drop(LevelManager.Instance.MainCharacter, true);
                        NotificationText.Push("哇！运气爆棚，触发变废为宝");
                    }
                }
            }
        }

        private void LoadConfig()
        {
            try
            {
                string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "info.ini");
                if (File.Exists(configPath))
                {
                    string[] lines = File.ReadAllLines(configPath);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Probability="))
                        {
                            string value = line.Substring("Probability=".Length).Trim();
                            if (int.TryParse(value, out int val))
                            {
                                Probability = Math.Clamp(val, 0, 100);
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("DontWaste模组：未找到info.ini文件，使用默认值");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"DontWaste模组：读取配置文件时出错：{e.Message}，使用默认值");
            }
        }
    }
}