using System;
using GameScripts.HotFix.GameLogic.WildEntitySpawnSystem;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace VaceonEngine.Editor
{
    public class SpawnerGenerator:EditorWindow
    {
        //窗口内部的数据状态
        private string generatedName = "Spawner";
        private string 生成pokemon名字;
        private float 半径;
        private int 数量;
        private float 停留时间 = 10f;
        
        //1.顶部菜单栏创建一个入口
        [MenuItem("Spawner/WildPkSpawner")]
        public static void ShowWindow()
        {
            //聚焦自定义窗口
            //自己继承了Editorwindow，算是1个窗口实体
            //窗口实体还继承了SO，可以存数据
            //可以用GetWindow获取窗口
            SpawnerGenerator sgwindow = GetWindow<SpawnerGenerator>();
            //设置大小和标题
            sgwindow.titleContent = new GUIContent("生成器");
            sgwindow.minSize = new Vector2(300, 200);
        }
        //UITK入口，CreateGUI
        private void CreateGUI()
        {
            //问题：VisualElement是什么？
            //所有UI控件的基类
            //操作和存储规则是？
            //组合模式的容器+叶子节点，不同点是叶子节点还可以再装叶子节点
            VisualElement root = rootVisualElement; //根控件
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
            Button generateBtn = new Button(GenerateObj); //需要传入点击时触发的函数
            generateBtn.text = "一键生成并挂载组件";
            generateBtn.style.height = 40;
            generateBtn.style.backgroundColor = new StyleColor(new Color(0.2f, 0.6f, 0.2f)); // 护眼绿
            generateBtn.style.color = Color.white;
            generateBtn.style.fontSize = 16;
            generateBtn.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(generateBtn);
            VisualElement buttonContainer = new VisualElement();
            generateBtn.Add(buttonContainer);
            nameField.Add(buttonContainer);
        }
        //按钮点击后执行的逻辑
        private void GenerateObj()
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
        }
    }
}