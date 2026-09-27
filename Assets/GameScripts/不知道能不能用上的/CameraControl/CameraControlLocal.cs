// using Unity.Cinemachine;
// using Unity.Netcode;
// using UnityEngine;
//
// namespace FSMBrainandState
// {
//     public class CameraControlLocal : NetworkBehaviour
//     {
//         #region 相机参数
//
//         [Header("相机参数表")] private float _cinemachineTargetYaw;
//         private float _cinemachineTargetPitch;
//         public Vector2 LookInput;
//         private const float _threshold = 0.01f;
//         public GameObject CinemachineCameraTarget;
//
//         [Tooltip("How far in degrees can you move the camera up")]
//         public float TopClamp = 70.0f;
//
//         [Tooltip("How far in degrees can you move the camera down")]
//         public float BottomClamp = -30.0f;
//
//         [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
//         public float CameraAngleOverride = 0.0f;
//
//         [Tooltip("For locking the camera position on all axis")]
//         public bool LockCameraPosition = false;
//
//         #endregion
//
//         #region 相机旋转
//         public CharacterBrainNetWork CharacterBrainNetWork;
//
//         private void CameraRotation()
//         {
//             LookInput = CharacterBrainNetWork.PLinput.Player.Look.ReadValue<Vector2>();
//             // if there is an input and camera position is not fixed
//             if (LookInput.sqrMagnitude >= _threshold && !LockCameraPosition)
//             {
//                 _cinemachineTargetYaw += LookInput.x;
//                 _cinemachineTargetPitch += LookInput.y;
//             }
//
//             // clamp our rotations so our values are limited 360 degrees
//             _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
//             _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
//
//             // Cinemachine will follow this target
//             CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
//                 _cinemachineTargetYaw, 0.0f);
//         }
//
//         private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
//         {
//             if (lfAngle < -360f) lfAngle += 360f;
//             if (lfAngle > 360f) lfAngle -= 360f;
//             return Mathf.Clamp(lfAngle, lfMin, lfMax);
//         }
//
//         #endregion
//
//         void LateUpdate()
//         {
//             if (!IsOwner) return;
//             CameraRotation();
//         }
//         #region 网络绑定相机
//         public override void OnNetworkSpawn()
//         {
//             base.OnNetworkSpawn();
//             if (!IsOwner) return;
//             CinemachineCamera MainCamera = GameObject.FindWithTag("CameraMachine").GetComponent<CinemachineCamera>();
//             if (MainCamera != null)
//             {
//                 // 【Cinemachine 3.1 推荐写法】
//                 // 通过赋予一个新的 CameraTarget 结构体来设置目标
//                 MainCamera.Target = new CameraTarget
//                 {
//                     // 1. 设置主追踪目标（通常用于决定相机移动位置）
//                     TrackingTarget = CinemachineCameraTarget.transform, 
//
//                     // 2. 是否需要独立的 LookAt 目标？
//                     // 设为 false：Cinemachine 会默认把 TrackingTarget 同时也当成 LookAt 目标。
//                     // 设为 true：你可以给 LookAtTarget 传另一个 Transform（比如锁定的敌人）。
//                     CustomLookAtTarget = false
//                 };
//             }
//             else
//             {
//                 Debug.LogError("Can't find CC相机");
//             }
//         }
//
//         public override void OnNetworkDespawn()
//         {
//             base.OnNetworkDespawn();
//             CinemachineCamera MainCamera = GameObject.FindWithTag("CameraMachine").GetComponent<CinemachineCamera>();
//             if (MainCamera != null)
//             {
//                 // 【Cinemachine 3.1 推荐写法】
//                 // 通过赋予一个新的 CameraTarget 结构体来设置目标
//                 MainCamera.Target = new CameraTarget
//                 {
//                     // 1. 设置主追踪目标（通常用于决定相机移动位置）
//                     TrackingTarget = null, 
//
//                     // 2. 是否需要独立的 LookAt 目标？
//                     // 设为 false：Cinemachine 会默认把 TrackingTarget 同时也当成 LookAt 目标。
//                     // 设为 true：你可以给 LookAtTarget 传另一个 Transform（比如锁定的敌人）。
//                     CustomLookAtTarget = false
//                 };
//             }
//             else
//             {
//                 Debug.LogError("Can't find CC相机");
//             }
//         }
//         #endregion
//     }
// }
