using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "MenuData",menuName = "UISO")]
public class MenuData : ScriptableObject
{
    //盒子选项卡
    [SerializeField] public Color Boxcolor;
    [SerializeField] public StyleColor _boxcolor;
    [SerializeField] public int Value;
    [CreateProperty] public string mylevel = "L72";

    private void Awake()
    {
        CreateALLProperty();
    }

    private void CreateALLProperty()
    {
        //它可以根据Color动态改变么？
        _boxcolor = new StyleColor(Boxcolor);
    }
}
