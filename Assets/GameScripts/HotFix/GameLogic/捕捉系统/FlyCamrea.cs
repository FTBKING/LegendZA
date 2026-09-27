using UnityEngine;

/// <summary>
/// 旧版 Input Manager：WASD 平移 + 鼠标转向的简易飞行相机。
/// 需要 Input Manager 里已有 Horizontal / Vertical / Mouse X / Mouse Y。
/// </summary>
public class FlyCamrea : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float sprintMultiplier = 2.5f;
    [SerializeField] float lookSensitivity = 2f;
    [SerializeField] float minPitch = -89f;
    [SerializeField] float maxPitch = 89f;

    float _yaw;
    float _pitch;

    void Start()
    {
        Vector3 euler = transform.eulerAngles;
        _yaw = euler.y;
        _pitch = euler.x;
        if (_pitch > 180f)
            _pitch -= 360f;

        SetCursorLocked(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursorLocked(false);
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
            SetCursorLocked(true);

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            _yaw += Input.GetAxis("Mouse X") * lookSensitivity;
            _pitch -= Input.GetAxis("Mouse Y") * lookSensitivity;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= sprintMultiplier;

        transform.position += move * speed * Time.deltaTime;
    }

    static void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
