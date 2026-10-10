using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameScripts.HotFix.GameLogic.UISystem
{
    public class UIController:MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _root;
        private MenuData _menuData;
        private void Awake()
        {
            //获取document,menudata
            _uiDocument = GetComponent<UIDocument>();
            _menuData = GetComponent<MenuData>();
            //取得root
            _root = _uiDocument.rootVisualElement;
            //设置datasource
            _root.dataSource = _menuData;
        }
    }
}