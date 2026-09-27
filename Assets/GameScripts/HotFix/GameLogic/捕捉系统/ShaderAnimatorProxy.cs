using UnityEngine;

// 加上此属性，让你在编辑器里拖动时间线时不运行游戏也能实时预览效果
[ExecuteAlways] 
[RequireComponent(typeof(Renderer))]
public class ShaderAnimatorProxy : MonoBehaviour
{
    [Header("在这里暴露需要K帧的属性")]
    [ColorUsage(true, true)] // 支持HDR颜色（HDRP/URP常用发光色）
    public Color emissionColor = Color.black;
    [Range(0f, 1f)]
    public float dissolveAmount = 0f;

    // 缓存渲染器引用
    private Renderer _renderer;
    
    // 【核心优化点1】全局静态复用 MaterialPropertyBlock，避免运行时产生任何GC垃圾
    private static MaterialPropertyBlock _mpb;

    // 【核心优化点2】缓存 Shader 属性的 Hash ID，比直接传字符串快得多
    private readonly int _emissionColorId = Shader.PropertyToID("_EmissionColor");
    private readonly int _dissolveAmountId = Shader.PropertyToID("_DissolveAmount"); // 替换为你Shader中的真实名称

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    // 建议在 LateUpdate 中执行，确保 Animator / Animation 修改完变量后再应用给渲染器
    private void LateUpdate()
    {
        if (_renderer == null) return;

        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();

        // 1. 先获取当前渲染器已有的属性块，避免覆盖掉其他代码设置的属性
        _renderer.GetPropertyBlock(_mpb);

        // 2. 将 C# 变量（也就是动画控制的变量）写入属性块
        _mpb.SetColor(_emissionColorId, emissionColor);
        _mpb.SetFloat(_dissolveAmountId, dissolveAmount);

        // 3. 将属性块应用回渲染器
        _renderer.SetPropertyBlock(_mpb);
    }
}