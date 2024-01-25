using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    /*private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }*/

    [SerializeField] private GameObject bulletPrefab;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //shoot shoot
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            //rb.velocity = new Vector3(gameObject.transform.rotation.x, gameObject.transform.rotation.y, 0);
            rb.velocity = Vector2.up * 20;
            Destroy(bullet.gameObject, 2.0f);
        }
    }
}
