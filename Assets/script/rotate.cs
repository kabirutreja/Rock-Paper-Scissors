using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
   [SerializeField] private Vector3 rotationAxis = Vector3.up; // Y-axis by default
    [SerializeField] private float rotationSpeed = 90f; // degrees per second

    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
