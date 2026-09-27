using UnityEditor;
using UnityEngine;

namespace GameScripts.HotFix.GameLogic.WildEntitySpawnSystem
{
    public static class utils
    {
        public static void DrawSpawnDisc(Vector3 center, float radius)
        {
            if (radius <= 0f)
            {
                return;
            }

            Color wire = new Color(0.25f, 0.85f, 0.4f, 0.95f);
#if UNITY_EDITOR
            Handles.color = new Color(wire.r, wire.g, wire.b, 0.15f);
            Handles.DrawSolidDisc(center, Vector3.up, radius);
            Handles.color = wire;
            Handles.DrawWireDisc(center, Vector3.up, radius);
#endif
            Gizmos.color = wire;
            Gizmos.DrawSphere(center, Mathf.Min(0.2f, radius * 0.05f));
        }
    }
    }