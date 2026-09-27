using UnityEngine;
using UnityEditor;
namespace VaceonEngine.Editor
{
    // 必须引入此命名空间
    public class MyMenuTools
    {
        // 在顶部菜单栏生成 "Tools -> 打印测试" 按钮
        [MenuItem("Tools/打印测试")]
        public static void PrintTest()
        {
            Debug.Log("你好，编辑器开发！");
        }
    }
}