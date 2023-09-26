using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControl : MonoBehaviour {
    [SerializeField]
    private GameObject BulletPrefab;
    [SerializeField]
    private GameObject SpecialBullet;

    //shooting of player
    [SerializeField]
    private Transform firePoint;
    [Tooltip("The time it took for player to shoot again"),SerializeField]
    private float shootSpeed;
    private float shootTime;
    

    //lazer
    private int lazerSwitch;
    [SerializeField] 
    private GameObject ThunderLight;

    //shield
    public GameObject Protecter;

    //player's collider
    public GameObject plane;
    private Collider collider;

	// Use this for initialization
	void Start () {
        shootSpeed = 0.2f;
        lazerSwitch = 0;
        collider = transform.GetComponent<CapsuleCollider>();
	}
	
	// Update is called once per frame
	void Update () {
        
		//WSAD to control player movement
        if (transform.position.x >= -4.4)
        {
            if (Input.GetKey(KeyCode.A)) {
                    transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * 6);
            }
        }

        if (transform.position.x <= 4.4)
        {
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * 6);
            }
        }

         if (transform.position.y <= 6.8) 
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime * 6);
            }
        }

        if (transform.position.y >= -6.8) 
        {
             if (Input.GetKey(KeyCode.S))
             {
                 transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * 6);
             }
        }
        //close lazer switch
        if (GameObject.Find("FireLight1") == null)
        {
            lazerSwitch = 0;
        }

        //shoot bullet when space key is pressed
        if (Input.GetKey(KeyCode.Space) && lazerSwitch == 0)
        {
            //control the shooting speed
            shootTime += Time.deltaTime;
            if (shootTime > shootSpeed)
            {
                shootTime = 0;
                GetComponent<AudioSource>().Play();
                Instantiate(BulletPrefab, firePoint.transform.position, Quaternion.identity);
            }
        }
        //make sure bullet shoot right when space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
                GetComponent<AudioSource>().Play();
                Instantiate(BulletPrefab, firePoint.transform.position, Quaternion.identity);
        }
        //clear time when space key is up
        if (Input.GetKeyUp(KeyCode.Space))
        {
            shootTime = 0;
        }

        //destroy player if hp is less than or equals to 0
        if(Status.status.life <= 0)
        {
            Destroy(gameObject);
        }

        //clear the screen with super bomb when left shift is clicked
        if (Input.GetKeyDown(KeyCode.LeftShift)&&Status.status.SBNumber>0)
        {
            Status.status.SBNumber--;
            GetComponent<AudioSource>().Play();
            Instantiate(SpecialBullet, firePoint.transform.position, Quaternion.identity);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        //deduct hp if collide with enemy
        if (other.tag == "Enemy") { 
            Status.status.lifeModify(-1);
        }

        //if collide with an add life add life to player if life is less than 3
        if (other.name == "AddLifeItem" || other.name == "AddLifeItem(Clone)")
        {
            Destroy(other.gameObject);
            if (Status.status.life < 3)
            {
                Status.status.lifeModify(+1);
            }
        }

        //if collide with a power up, unlock lazer
        if (other.name == "Power(Clone)" || other.name == "Power")
        {
            Destroy(other.gameObject);
            lazerSwitch = 1;
            Instantiate(ThunderLight,firePoint.transform.position,Quaternion.identity);

        }

        //if collide with SPBullet, add one SPBullet if the amount is less than 3
        if (other.name == "SPBullet" || other.name == "SPBullet(Clone)")
        {
            Destroy(other.gameObject);
            if (Status.status.SBNumber < 3)
            {
                Status.status.SBNumber++;
            }
        }

        //if collide with protect, activate shield
        if (other.name == "Protect" || other.name == "Protect(Clone)")
        {
            Destroy(other.gameObject);
            Instantiate(Protecter, plane.transform.position, Protecter.transform.rotation);
            collider.enabled = false;
        }
    }
}
