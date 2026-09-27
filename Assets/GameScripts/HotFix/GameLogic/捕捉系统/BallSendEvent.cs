using System.Collections;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;
using UnityEngine;
using VaceonEngine;

public class BallSendEvent : MonoBehaviour
{
    public float floatspeed;

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("WildPokemon"))
            return;

        SendEvent(other.gameObject);
        FloatToTarget(other.gameObject);
    }
    //发送碰撞事件
    public void SendEvent(GameObject targetAgent)
    {
        var graphOwner = targetAgent.GetComponent<GraphOwner>();
        if (graphOwner != null)
            graphOwner.SendEvent("BallHit");
    }
    //自己飘到在碰撞体的位置Ypos上+2
    public void FloatToTarget(GameObject targetAgent)
    {
        //先将刚体关掉
        GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(gototarget(targetAgent)); 
    }
    private IEnumerator gototarget(GameObject targetAgent)
    {
        float continuetime = 2f;
        while (continuetime > 0)
        {
            if (targetAgent == null) yield break;
            //lerp移动
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(targetAgent.transform.position.x, targetAgent.transform.position.y + 2, targetAgent.transform.position.z),
                Time.deltaTime*floatspeed);
            continuetime -= Time.deltaTime;
            Log.Info($"<Blue>FloatToTarget:{continuetime}</Blue>");
            yield return null;
        }
        //结束
        yield break;
    }
}
