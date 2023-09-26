using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FindSBNumer : MonoBehaviour {
    //the sb bullets
    [SerializeField]
    private GameObject SBCount1;
    [SerializeField]
    private GameObject SBCount2;
    [SerializeField]
    private GameObject SBCount3;

	
	// Update is called once per frame
	void Update () {
        //show the sb bullet
        if (Status.status.SBNumber == 3)
        {
            SBCount3.gameObject.SetActive(true);
        }
        else if (Status.status.SBNumber == 2)
        {
            SBCount2.gameObject.SetActive(true);
            SBCount3.gameObject.SetActive(false);
        }
        else if (Status.status.SBNumber == 1)
        {
            SBCount1.gameObject.SetActive(true);
            SBCount2.gameObject.SetActive(false);
            SBCount3.gameObject.SetActive(false);
        }
        else
        {
            SBCount1.gameObject.SetActive(false);
            SBCount2.gameObject.SetActive(false);
            SBCount3.gameObject.SetActive(false);
        }
	}
}
