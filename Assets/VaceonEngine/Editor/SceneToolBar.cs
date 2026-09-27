using GameScripts.HotFix.GameLogic.WildEntitySpawnSystem;
using UnityEditor;
using UnityEditor.Overlays; // 必须引入此命名空间
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace VaceonEngine.Editor
{
// 1. 注册 Overlay，并指定它只出现在 Scene 视图中
    [Overlay(typeof(SceneView), "TeamGeneratorOverlay", "场景快捷生成器")]
    public class SceneGeneratorOverlay : Overlay
    {
        //窗口内部的数据状态
        private string generatedName = "Spawner";
        private string 生成pokemon名字;
        private float 半径;
        private int 数量;
        private float 停留时间 = 10f;

        // 2. 核心：重写 CreatePanelContent，用 UI Toolkit 绘制悬浮窗
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement() { style = { minWidth = 200 } };
            //-------标题控件-叶子
            Label titleLabel = new Label("野生PokemonSpawner生成器");
            titleLabel.style.fontSize = 18;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 15;
            root.Add(titleLabel); //创建完后加入root节点
            // --- 名称输入框 (TextField) ---
            TextField nameField = new TextField("生成物体名称");
            nameField.value = generatedName; // 设置默认值
            nameField.RegisterValueChangedCallback(evt => generatedName = evt.newValue);
            root.Add(nameField);
            // --- 生成pokemon名字输入框 (TextField) ---
            TextField pokemonNameField = new TextField("生成pokemon名字");
            pokemonNameField.value = 生成pokemon名字; // 设置默认值
            pokemonNameField.RegisterValueChangedCallback(evt => 生成pokemon名字 = evt.newValue);
            root.Add(pokemonNameField);
            // --- 半径输入框 (TextField) ---
            TextField radiusField = new TextField("半径");
            radiusField.value = 半径.ToString(); // 设置默认值
            radiusField.RegisterValueChangedCallback(evt => 半径 = float.Parse(evt.newValue));
            root.Add(radiusField);
            // --- 数量输入框 (TextField) ---
            TextField countField = new TextField("数量");
            countField.value = 数量.ToString(); // 设置默认值
            countField.RegisterValueChangedCallback(evt => 数量 = int.Parse(evt.newValue));
            root.Add(countField);
            // --- 停留时间输入框 (TextField) ---
            TextField stayDurationField = new TextField("停留时间(秒)");
            stayDurationField.value = 停留时间.ToString();
            stayDurationField.RegisterValueChangedCallback(evt => 停留时间 = float.Parse(evt.newValue));
            root.Add(stayDurationField);
            //还可以再加个根组件，创建一行空隙
            root.Add(new VisualElement { style = { height = 20 } });
            // --- 生成大按钮 ---
            Button generateBtn = new Button(GenerateObject); //需要传入点击时触发的函数
            generateBtn.text = "一键生成并挂载组件";
            generateBtn.style.height = 40;
            generateBtn.style.backgroundColor = new StyleColor(new Color(0.2f, 0.6f, 0.2f)); // 护眼绿
            generateBtn.style.color = Color.white;
            generateBtn.style.fontSize = 16;
            generateBtn.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(generateBtn);
            return root;
        }

        // 3. 按钮逻辑（与之前的独立窗口逻辑基本一致）
        private void GenerateObject()
        {
            //记录当前的撤销操作组
            Undo.SetCurrentGroupName("生成物体");
            int group = Undo.GetCurrentGroup();
            //生成预制体实例，保持关联
            //问题：为何用PrefabUtility.InstantiatePrefab，它和直接Instantiate的区别是？
            GameObject newObj = new GameObject(generatedName);
            //注册创建撤销
            Undo.RegisterCreatedObjectUndo(newObj,"创建预制体");
            //挂载WildEntitySpawner组件，并按照输入信息设置字段
            WildEntitySpawner spawner = newObj.AddComponent<WildEntitySpawner>();
            spawner.name = 生成pokemon名字;
            spawner.radius = 半径;
            spawner.spawnCount = 数量;
            spawner.stayDuration = 停留时间;
            //结束撤销组，并自动选中新生成的物体
            Undo.CollapseUndoOperations(group);
            Selection.activeGameObject = newObj;
            Debug.Log($"成功生成物体{generatedName}");
            // 关键点：操作完成后强制刷新 Scene 视图，确保队友立马看到画面变化
            SceneView.RepaintAll();
        }
    }
}