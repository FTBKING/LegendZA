using System;
using UnityEngine;
using UnityEngine.UIElements;
using VaceonEngine.Runtime.Module;
using VaceonEngine.Runtime.SimpleEventbus;

[RequireComponent(typeof(UIDocument))]
public sealed class UIController : MonoBehaviour
{
    private const string MenuDataLocation = "MenuData";
    private const string MethodViewLocation = "Method-1";
    private const string BoxViewLocation = "BoxUi-1";
    private const string BoxElementName = "Box";
    private const string BackButtonName = "BackButton";

    private UIDocument _uiDocument;
    private MenuData _menuData;
    private VisualTreeAsset _methodView;
    private VisualTreeAsset _boxView;
    private VisualElement _boxElement;
    private VisualElement _backButton;
    private bool _resourcesLoaded;
    private View _currentView = View.Method;

    private enum View
    {
        Method,
        Box
    }

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        GameEvents.OnResourceInitComplete += LoadResource;

        // 兼容资源包先于该组件完成初始化的情况。
        if (GameModule.Data.IsInitialized)
        {
            LoadResource();
        }
    }

    private void OnDisable()
    {
        GameEvents.OnResourceInitComplete -= LoadResource;
        UnregisterElementCallbacks();
    }

    private void LoadResource()
    {
        if (!_resourcesLoaded)
        {
            try
            {
                _menuData = GameModule.Data.LoadAsset<MenuData>(MenuDataLocation);
                _methodView = GameModule.Data.LoadAsset<VisualTreeAsset>(MethodViewLocation);
                _boxView = GameModule.Data.LoadAsset<VisualTreeAsset>(BoxViewLocation);
                _resourcesLoaded = true;
                _currentView = View.Method;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                return;
            }
        }
        //也只能放这了
        ShowCurrentView();
    }

    private void ShowCurrentView()
    {
        switch (_currentView)
        {
            case View.Method:
                ShowMethodView();
                break;
            case View.Box:
                ShowBoxView();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ShowMethodView()
    {
        _currentView = View.Method;
        VisualElement root = SetVisualTree(_methodView);

        _boxElement = root.Q<VisualElement>(BoxElementName);
        if (_boxElement == null)
        {
            Debug.LogError($"在 {MethodViewLocation} 中未找到 #{BoxElementName} 节点。", this);
            return;
        }

        _boxElement.RegisterCallback<PointerUpEvent>(OnBoxPointerUp);
        Debug.Log("完成注册Box");
    }

    private void ShowBoxView()
    {
        _currentView = View.Box;
        VisualElement root = SetVisualTree(_boxView);

        _backButton = root.Q<VisualElement>(BackButtonName);
        if (_backButton == null)
        {
            Debug.LogError($"在 {BoxViewLocation} 中未找到 #{BackButtonName} 节点。", this);
            return;
        }

        _backButton.RegisterCallback<PointerUpEvent>(OnBackButtonPointerUp);
        Debug.Log("完成注册Back");
    }

    private VisualElement SetVisualTree(VisualTreeAsset visualTreeAsset)
    {
        UnregisterElementCallbacks();
        _uiDocument.visualTreeAsset = visualTreeAsset;

        VisualElement root = _uiDocument.rootVisualElement;
        root.dataSource = _menuData;
        return root;
    }

    private void OnBoxPointerUp(PointerUpEvent pointerUpEvent)
    {
        pointerUpEvent.StopPropagation();
        Debug.Log("Box被点击");
        ShowBoxView();
    }

    private void OnBackButtonPointerUp(PointerUpEvent pointerUpEvent)
    {
        pointerUpEvent.StopPropagation();
        Debug.Log("BackButton被点击");
        ShowMethodView();
    }

    private void UnregisterElementCallbacks()
    {
        if (_boxElement != null)
        {
            _boxElement.UnregisterCallback<PointerUpEvent>(OnBoxPointerUp);
            _boxElement = null;
        }

        if (_backButton != null)
        {
            _backButton.UnregisterCallback<PointerUpEvent>(OnBackButtonPointerUp);
            _backButton = null;
        }
    }
}
