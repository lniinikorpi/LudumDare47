using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject firstCollider;
    public GameObject secondCollider;
    public bool firstColliderDone;
    public bool secondColliderDone;
    public bool firstGate;
    public int gateIndex;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckIfThrough()
    {
        if(firstColliderDone)
        {
            firstColliderDone = false;
            if(firstGate)
            {
                if(gm.playerLastGateIndex == gm.gates.Count - 1)
                {
                    GameManager.instance.playerLastGateIndex = gateIndex;
                    print("läpi meni");
                    gm.AddScore(gm.lapScore);
                }
                else
                {
                    print("väärä portti");
                }
            }
            else
            {
                if (gm.playerLastGateIndex == gateIndex - 1)
                {
                    gm.playerLastGateIndex = gateIndex;
                    print("läpi meni");
                    gm.AddScore(gm.gateScore);
                }
                else
                {
                    print("väärä portti");
                }
            }
        }
    }
}
