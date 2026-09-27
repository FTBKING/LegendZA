using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// ?? Unity.Mathematics.math ?????????
/// </summary>
public enum MathDirection
{
    Up,
    Down,
    Left,
    Right,
    Forward,
    Back
}

public class TestRotation : MonoBehaviour
{
    public GameObject target;

    [Tooltip("LookRotation ? upwards????? math.up/down/left/right/forward/back")]
    public MathDirection upDirection = MathDirection.Up;

    void Update()
    {
        if (target == null) return;

        Vector3 targetDir = target.transform.position - transform.position;
        targetDir.Normalize();
        //safe和不safe貌似没区别
        transform.rotation = quaternion.LookRotationSafe(targetDir, ToFloat3(upDirection));
    }

    public static float3 ToFloat3(MathDirection direction)
    {
        return direction switch
        {
            MathDirection.Up => math.up(),
            MathDirection.Down => math.down(),
            MathDirection.Left => math.left(),
            MathDirection.Right => math.right(),
            MathDirection.Forward => math.forward(),
            MathDirection.Back => math.back(),
            _ => math.up()
        };
    }
}
