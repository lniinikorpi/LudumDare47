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
    AudioSource audioSource;
    public AudioClip goodClip;
    public AudioClip badClip;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        audioSource = GetComponent<AudioSource>();
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
                    audioSource.clip = goodClip;
                    audioSource.Play();
                }
                else
                {
                    print("väärä portti");
                    audioSource.clip = badClip;
                    audioSource.Play();
                }
                gm.PassFirstGate();
            }
            else
            {
                gm.tutorialCanvas.SetActive(false);
                if (gm.playerLastGateIndex == gateIndex - 1)
                {
                    gm.playerLastGateIndex = gateIndex;
                    print("läpi meni");
                    gm.AddScore(gm.gateScore);
                    audioSource.clip = goodClip;
                    audioSource.Play();
                }
                else
                {
                    print("väärä portti");
                    audioSource.clip = badClip;
                    audioSource.Play();
                }
            }
        }
    }
}
