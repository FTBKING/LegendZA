using System.Numerics;
using UnityEngine.UIElements;

namespace GameScripts.HotFix.GameLogic.UISystem
{
    public class BoxManipulator : PointerManipulator 
    {

        public BoxManipulator()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }
        void OnPointerUp(PointerUpEvent evt)
        {
            if (!CanStopManipulation(evt)) return;

            evt.StopPropagation();
        }
    }
}
