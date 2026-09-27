using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace VaceonEngine.Editor.ComponentEditor
{
// 告诉 Unity 这个脚本用来重绘 ObjectGenerator 的面板
[CustomEditor(typeof(ObjectGenerator))]
public class ObjectGeneratorEditor : UnityEditor.Editor
{
    // 使用 UI Toolkit 重绘 Inspector
    public override VisualElement CreateInspectorGUI()
    {
        // 创建一个根节点
        VisualElement root = new VisualElement();

        // 1. 极其方便的一步：自动绘制 ObjectGenerator 脚本中所有 public 变量的默认输入框
        InspectorElement.FillDefaultInspector(root, serializedObject, this);

        // 加点空隙
        root.Add(new VisualElement { style = { height = 15 } });

        // 2. 创建一个“生成”按钮
        Button generateBtn = new Button(GenerateObject);
        generateBtn.text = "一键生成并挂载 Rigidbody";
        generateBtn.style.height = 35; // 按钮高度
        generateBtn.style.backgroundColor = new StyleColor(new Color(0.2f, 0.6f, 0.2f)); // 护眼绿
        generateBtn.style.color = Color.white;
        generateBtn.style.unityFontStyleAndWeight = FontStyle.Bold;

        // 将按钮添加到根节点
        root.Add(generateBtn);

        return root;
    }

    // 按钮点击后执行的逻辑
    private void GenerateObject()
    {
        // target 是 Editor 基类里的变量，代表当前正在检查的组件实例
        //问题：为什么能这样强转，不都是GetComponent？
        ObjectGenerator generator = (ObjectGenerator)target;

        // 容错：如果没有设置预制体，拦截并提示
        if (generator.targetPrefab == null)
        {
            Debug.LogWarning("请先在面板中指定 Target Prefab！");
            return;
        }

        // 记录当前的撤销操作组，方便一次性 Ctrl+Z 撤销下面所有的步骤
        Undo.SetCurrentGroupName("面板生成物体");
        int group = Undo.GetCurrentGroup();

        // 1. 生成预制体实例，并保持其“预制体蓝色”关联状态
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(generator.targetPrefab);
        
        // 注册创建撤销
        Undo.RegisterCreatedObjectUndo(newObj, "创建预制体");

        // 2. 修改属性
        newObj.name = generator.generatedName;
        newObj.transform.position = new Vector3(0, generator.spawnHeight, 0);

        // 3. 动态挂载组件 (使用 Undo.AddComponent 支持撤销)
        if (newObj.GetComponent<Rigidbody>() == null)
        {
            Undo.AddComponent<Rigidbody>(newObj);
        }

        // 收尾：结束撤销组，并在场景中自动选中新生成的物体
        Undo.CollapseUndoOperations(group);
        Selection.activeGameObject = newObj;
        
        Debug.Log($"生成成功！");
    }
}
}