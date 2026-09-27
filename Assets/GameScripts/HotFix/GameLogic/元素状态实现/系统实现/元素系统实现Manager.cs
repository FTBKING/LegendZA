using GameScripts.HotFix.GameLogic.元素状态实现.元素形态;
using GameScripts.HotFix.GameLogic.WorldEntity;
using Narakacomponent;
using UnityEngine;

namespace GameScripts.HotFix.GameLogic.元素状态实现.系统实现
{
    public class 元素系统实现Manager : ActorComponent
    {
        private const int RayHitBufferSize = 8;

        [Header("吸取元素")]
        [SerializeField] private KeyCode _absorbKey = KeyCode.F;
        [SerializeField] private float _rayDistance = 50f;
        [SerializeField] private LayerMask _rayMask = ~0;

        [Header("准星")]
        [SerializeField] private float _crosshairSize = 8f;
        [SerializeField] private float _crosshairThickness = 2f;
        [SerializeField] private Color _crosshairColor = Color.white;

        private 元素系统上下文 _元素系统上下文;
        private readonly RaycastHit[] _rayHits = new RaycastHit[RayHitBufferSize];

        public override int TickOrder => ActorTickOrder.Element;

        protected override void OnBind()
        {
            if (_元素系统上下文 == null)
            {
                _元素系统上下文 = new 元素系统上下文();
            }
        }

        public override void Tick(float dt)
        {
            if (!Input.GetKeyDown(_absorbKey))
            {
                return;
            }

            TryAbsorbElementFromRay();
        }

        private void OnGUI()
        {
            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;
            float halfSize = _crosshairSize;
            float halfThickness = _crosshairThickness * 0.5f;

            Color previous = GUI.color;
            GUI.color = _crosshairColor;
            GUI.DrawTexture(new Rect(cx - halfSize, cy - halfThickness, _crosshairSize * 2f, _crosshairThickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(cx - halfThickness, cy - halfSize, _crosshairThickness, _crosshairSize * 2f), Texture2D.whiteTexture);
            GUI.color = previous;
        }

        public void 改变元素形态(EleType eleType)
        {
            _元素系统上下文._type = eleType;
            Debug.Log($"元素形态已更改为：{_元素系统上下文._type}");
        }

        private void TryAbsorbElementFromRay()
        {
            Ray ray = BuildAbsorbRay();
            if (!TryGetClosestHit(ray, out RaycastHit hit))
            {
                Debug.Log("射线未命中碰撞体");
                return;
            }

            Debug.Log($"射线命中碰撞体：{hit.collider.name}");

            if (!hit.collider.TryGeEle(out EleType eleType))
            {
                Debug.Log($"碰撞体没有元素载体：{hit.collider.name}");
                return;
            }

            改变元素形态(eleType);
        }

        // public void 改变元素攻击形态(抽象攻击形态 攻击形态)
        // {
        //     _元素系统上下文._currentElementState.设置形态(攻击形态);
        //     Debug.Log($"元素攻击形态已更改为：{攻击形态}");
        // }
        //
        // public void BaseAttk()
        // {
        //     _元素系统上下文._currentElementState.BaseAttk(_元素系统上下文);
        //     Debug.Log($"元素基础攻击已释放");
        // }
        //
        // public void Sk1()
        // {
        //     _元素系统上下文._currentElementState.Sk1(_元素系统上下文);
        //     Debug.Log($"元素技能1已释放");
        // }
        //
        // public void Sk2()
        // {
        //     _元素系统上下文._currentElementState.Sk2(_元素系统上下文);
        //     Debug.Log($"元素技能2已释放");
        // }
        //
        // public void BigSkill()
        // {
        //     _元素系统上下文._currentElementState.BigSkill(_元素系统上下文);
        //     Debug.Log($"元素大技能已释放");
        // }

        private Ray BuildAbsorbRay()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                return cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            }

            return new Ray(transform.position + Vector3.up * 1.5f, transform.forward);
        }

        private bool TryGetClosestHit(Ray ray, out RaycastHit hit)
        {
            int count = Physics.RaycastNonAlloc(ray, _rayHits, _rayDistance, _rayMask, QueryTriggerInteraction.Collide);
            int bestIndex = -1;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider col = _rayHits[i].collider;
                if (col == null)
                {
                    continue;
                }

                Transform colTransform = col.transform;
                if (colTransform == transform || colTransform.IsChildOf(transform))
                {
                    continue;
                }

                float distance = _rayHits[i].distance;
                if (distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestIndex = i;
            }

            if (bestIndex < 0)
            {
                hit = default;
                return false;
            }

            hit = _rayHits[bestIndex];
            return true;
        }
    }
}
