using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCodeForCameraTest : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, 0, vertical);

        transform.Translate(move * speed * Time.deltaTime);
    }
}
