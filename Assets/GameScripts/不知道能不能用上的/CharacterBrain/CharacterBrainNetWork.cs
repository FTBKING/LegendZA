// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using System.Collections.Generic;
//
// namespace FSMBrainandState
// {
//     public class CharacterBrainNetWork:NetworkBehaviour
//     {
//         private struct PredictedCommand
//         {
//             public int Sequence;
//             public CommandType CommandType;
//             public float BufferWindow;
//         }
//         public PLcon PLinput;
//         public CharacterStateManager characterStateManager;
//         private 输入缓冲队列接口 _localBufferCommand;
//         private 输入缓冲队列接口 _serverBufferCommand;
//         private readonly Queue<PredictedCommand> _pendingCommands = new Queue<PredictedCommand>();
//         private int _nextSequence = 1;
//
//         private void Update()
//         {
//             if (IsOwner)
//             {
//                 _localBufferCommand?.UpdateAction();
//                 ReadCombatInput();
//             }
//
//             if (IsServer)
//             {
//                 _serverBufferCommand?.UpdateAction();
//             }
//         }
//
//         // 战斗输入：F = 普通攻击(叠冰锥层数)，R = 冰锥(1 技能)。
//         // 直接读 Keyboard（同属 InputSystem），避免改动 .inputactions 资产与重新生成包装类。
//         private void ReadCombatInput()
//         {
//             var kb = Keyboard.current;
//             if (kb == null) return;
//
//             if (kb.fKey.wasPressedThisFrame)
//             {
//                 PushPredictedCommand(CommandType.BasicAttack, 0.5f);
//             }
//             if (kb.rKey.wasPressedThisFrame)
//             {
//                 PushPredictedCommand(CommandType.Skill1, 0.5f);
//             }
//         }
//         #region 新状态机回调方法
//         private void LeftHitCancel(InputAction.CallbackContext obj)
//         {
//             PushPredictedCommand(CommandType.Mup, 0.5f);
//         }
//         private void LeftHit(InputAction.CallbackContext obj)
//         {
//             PushPredictedCommand(CommandType.MPress, 0.5f);
//         }
//         private void Highjumpperform(InputAction.CallbackContext obj)
//         {
//             PushPredictedCommand(CommandType.HighJump, 0.5f);
//         }
//
//         private void LowJumpperform(InputAction.CallbackContext obj)
//         {
//             PushPredictedCommand(CommandType.LowJump, 0.5f);
//         }
//
//         private void Spintcancel(InputAction.CallbackContext obj)
//         {
//             PushPredictedCommand(CommandType.SprintCancel, 0.5f);
//         }
//
//         private void SprintStart(InputAction.CallbackContext obj)
//         {
//             PushPredictedCommand(CommandType.SprintStart, 0.5f);
//         }
//
//         private void MoveCancel(InputAction.CallbackContext obj)
//         {
//             PushPredictedCommand(CommandType.MoveCancel, 0.5f);
//         }
//
//         private void MoveStart(InputAction.CallbackContext obj)
//         {
//             PushPredictedCommand(CommandType.MoveStart, 0.5f);
//         }
//
//         private void Landed(Vector3 vector3)
//         {
//             Debug.Log("角色落地");
//             PushPredictedCommand(CommandType.Landed, 0.5f);
//         }
//
//         private void InAir()
//         {
//             Debug.Log("角色在空中");
//             PushPredictedCommand(CommandType.InAir, 0.5f);
//         }
//         #endregion
//
//         public override void OnNetworkSpawn()
//         {
//             base.OnNetworkSpawn();
//
//             if (IsServer)
//             {
//                 _serverBufferCommand = new QueueBufferCommand(characterStateManager.PlayerStateMachine);
//             }
//
//             // 仅本地拥有者采集输入并做预测。
//             if (!IsOwner) return;
//
//             PLinput = new PLcon();
//             PLinput.Enable();
//             PLinput.Player.Move.started += MoveStart;
//             PLinput.Player.Move.canceled += MoveCancel;
//             PLinput.Player.Sprint.started += SprintStart;
//             PLinput.Player.Sprint.canceled += Spintcancel;
//             PLinput.Player.LowJump.performed += LowJumpperform;
//             PLinput.Player.HighJump.performed += Highjumpperform;
//             PLinput.Player.Lefthit.started += LeftHit;
//             PLinput.Player.Lefthit.canceled += LeftHitCancel;
//             characterStateManager.CharacterActor.OnStableStateEnter += Landed;
//             characterStateManager.CharacterActor.OnStableStateExit += InAir;
//             _localBufferCommand = new QueueBufferCommand(characterStateManager.PlayerStateMachine);
//         }
//         public override void OnNetworkDespawn()
//         {
//             base.OnNetworkDespawn();
//             if (!IsOwner) return;
//             PLinput.Disable();
//             PLinput.Player.Move.started -= MoveStart;
//             PLinput.Player.Move.canceled -= MoveCancel;
//             PLinput.Player.Sprint.started -= SprintStart;
//             PLinput.Player.Sprint.canceled -= Spintcancel;
//             PLinput.Player.LowJump.performed -= LowJumpperform;
//             PLinput.Player.HighJump.performed -= Highjumpperform;
//             PLinput.Player.Lefthit.started -= LeftHit;
//             PLinput.Player.Lefthit.canceled -= LeftHitCancel;
//             characterStateManager.CharacterActor.OnStableStateEnter -= Landed;
//             characterStateManager.CharacterActor.OnStableStateExit -= InAir;
//             _pendingCommands.Clear();
//         }
//
//         private void PushPredictedCommand(CommandType commandType, float bufferWindow)
//         {
//             if (!IsOwner)
//             {
//                 return;
//             }
//
//             int sequence = _nextSequence++;
//             var predicted = new PredictedCommand
//             {
//                 Sequence = sequence,
//                 CommandType = commandType,
//                 BufferWindow = bufferWindow
//             };
//
//             _pendingCommands.Enqueue(predicted);
//             _localBufferCommand?.bufferCommand(commandType, bufferWindow);
//             SubmitInputServerRpc(sequence, commandType, bufferWindow);
//         }
//
//         [Rpc(SendTo.Server)]
//         private void SubmitInputServerRpc(int sequence, CommandType commandType, float bufferWindow)
//         {
//             _serverBufferCommand?.bufferCommand(commandType, bufferWindow);
//             ConfirmInputClientRpc(sequence, commandType);
//         }
//
//         [Rpc(SendTo.Owner)]
//         private void ConfirmInputClientRpc(int acknowledgedSequence, CommandType acknowledgedCommand)
//         {
//             bool ackFound = false;
//             while (_pendingCommands.Count > 0)
//             {
//                 var pending = _pendingCommands.Dequeue();
//                 if (pending.Sequence == acknowledgedSequence)
//                 {
//                     ackFound = true;
//                     break;
//                 }
//             }
//             //没找到确认的命令，直接返回
//             if (!ackFound)
//             {
//                 return;
//             }
//             // 清空后对未确认输入重演，避免校正后丢按键。
//             _localBufferCommand = new QueueBufferCommand(characterStateManager.PlayerStateMachine);
//             foreach (var command in _pendingCommands)
//             {
//                 _localBufferCommand.bufferCommand(command.CommandType, command.BufferWindow);
//             }
//         }
//     }
// }