using UnityEngine;

namespace VaceonEngine.Editor.ComponentEditor
{
    

// 这个脚本挂载在场景中的一个空物体上，用来存储生成参数
    public class ObjectGenerator : MonoBehaviour
    {
        [Header("生成设置")] public GameObject targetPrefab; // 要生成的预制体

        public string generatedName = "My New Object"; // 生成的物体名称

        public float spawnHeight = 0f; // 生成的高度
    }
}