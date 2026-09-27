using GameScripts.HotFix.GameLogic.元素状态实现.元素形态;
using UnityEngine;

namespace GameScripts.HotFix.GameLogic.WorldEntity
{
    public interface I元素载体
    {
        EleType 获取元素类型();
    }

    //给Collider类扩展方法
    //1.声明一个静态类
    public static class ColliderExpand
    {
        //2.声明一个静态方法，必须this Collider collider
        public static bool TryGeEle(this Collider collider, out EleType eleType)
        {
            //实际也是获取接口，调用接口，封装上了
            eleType = default;
            //如果collider不存在，返回false
            if (collider == null) return false;
            //尝试获取I元素载体
            I元素载体 component = collider.GetComponent<I元素载体>();
            if (component == null) return false;
            eleType = component.获取元素类型();
            return true;
        }
        //编译时会编译成ColliderExpand.TryGeEle
        //但可以collder.TryGeEle
        //注意：C#里除了扩展方法，其他静态方法都无法用实例调用
    }
}
