
using UnityEngine;
using VaceonEngine;

public class BallEmitter : MonoBehaviour
{
    public GameObject ballPrefab;

    public int ballspeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Log.Info("发射球");
            GameObject o = Instantiate(ballPrefab, transform.position, transform.rotation);
            Rigidbody component = o.GetComponent<Rigidbody>();
            if (component == null)
            {
                Log.Error("BallEmitter: 没有Rigidbody组件");
                return;
            }
            component.linearVelocity = transform.forward * ballspeed;
        }
    }
}
