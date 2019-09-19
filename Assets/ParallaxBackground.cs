using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public float backgroundSizeY;
    public float backgroundSizeX;
    public float parallaxSpeed;

    private Transform cameraTransform;
    private Transform[] layers;
    private float viewZone = 10;
    private int bottomIndex;
    private int topIndex;
    private int leftIndex;
    private int rightIndex;
    private float lastCameraY;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraY = cameraTransform.position.y;
        layers = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }

        bottomIndex = 0;
        topIndex = layers.Length - 1;
        leftIndex = 0;
        rightIndex = layers[0].childCount - 1;
        print(layers[0].childCount);
    }

    private void ScrollUp()
    {
        layers[bottomIndex].position = layers[topIndex].position + (Vector3.up * backgroundSizeY);
        topIndex = bottomIndex;
        bottomIndex++;
        if (bottomIndex == layers.Length)
            bottomIndex = 0;
    }

    private void ScrollDown()
    {
        layers[topIndex].position = layers[bottomIndex].position - (Vector3.up * backgroundSizeY);
        bottomIndex = topIndex;
        topIndex--;
        if (topIndex < 0)
            topIndex = layers.Length - 1;
    }

    private void ScrollLeft()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].GetChild(rightIndex).position = layers[i].GetChild(leftIndex).position - (Vector3.right * backgroundSizeX);
        }
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
            rightIndex = layers[0].childCount - 1;
    }

    private void ScrollRight()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].GetChild(leftIndex).position = layers[i].GetChild(rightIndex).position + (Vector3.right * backgroundSizeX);
        }
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers[0].childCount)
            leftIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaY = cameraTransform.position.y - lastCameraY;
        transform.position += Vector3.up * (deltaY * parallaxSpeed);
        lastCameraY = cameraTransform.position.y;
        if (cameraTransform.position.y > (layers[topIndex].transform.position.y - viewZone))
            ScrollUp();
        if (cameraTransform.position.y < (layers[bottomIndex].transform.position.y + viewZone))
            ScrollDown();
        if (cameraTransform.position.x > (layers[0].GetChild(rightIndex).transform.position.x - viewZone))
            ScrollRight();
        if (cameraTransform.position.x < (layers[0].GetChild(leftIndex).transform.position.x + viewZone))
            ScrollLeft();
    }
}
