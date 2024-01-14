using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ObjectMover : MonoBehaviour
{
    public Canvas datasetCanvas;
    public Canvas mainButtonsCanvas;
    public Canvas slidersCanvas;
    public GameObject uiScreen;

    public GameObject clientContainer;
    public GameObject serverContainer;
    private float distanceBetweenCubes = 0.5f;

    public void DestroyObjects()
    {
        IEnumerable<Canvas> canvases = new List<Canvas> { this.datasetCanvas, this.mainButtonsCanvas, this.slidersCanvas };
        foreach (Canvas element in canvases)
        { 
		    foreach (Transform child in element.transform)
		    {
			    child.gameObject.SetActive(false);
		    }
	    }

        uiScreen.SetActive(false);
    }

    public void SpawnUI()
    {
        IEnumerable<Canvas> canvases = new List<Canvas> { this.datasetCanvas, this.mainButtonsCanvas, this.slidersCanvas };
        foreach (Canvas element in canvases)
        { 
		    foreach (Transform child in element.transform)
		    {
			    child.gameObject.SetActive(true);
		    }
	    }

        uiScreen.SetActive(true);
    }

    public void SetupExperiment(int numberOfClients, int clientEpochs, string dataset, int serverRounds)
    {
        this.SpawnClients(numberOfClients);
        this.SpawnServer();
    }

    private void SpawnClients(int numberOfClients)
    {
        float xPositionOffset = -0.5f;
        for (int i = 0; i < numberOfClients; i++)
        {
            Vector3 position = new Vector3(
                (i * this.distanceBetweenCubes) + xPositionOffset,
                -0.9f,
                0
            );

            var finalPosition = transform.position + position;
            GameObject client = Instantiate(this.clientContainer, finalPosition, Quaternion.identity);

            GameObject textObject = new GameObject();
            TextMesh textMesh = textObject.AddComponent<TextMesh>();
            textMesh.text = "";
            textMesh.fontSize = 35;
            textMesh.characterSize = 0.5f;
            textMesh.color = Color.black;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textObject.transform.position = client.transform.position + new Vector3(0, 0, 0);
            textObject.transform.rotation = Quaternion.Euler(90, 0, 0);
            textObject.transform.SetParent(client.transform);
            textObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            textObject.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
    }

    private void SpawnServer()
    {
        var clientObjects = GameObject.FindObjectsOfType<GameObject>()
	        .Where(obj => obj.name == "Client(Clone)")
	        .OrderBy(obj => obj.transform.position.x)
	        .ToList();
        int midPoint = (int)Math.Floor(clientObjects.Count() / 2f);
        var midPointXCoordinate = clientObjects[midPoint].transform.position.x;

        Vector3 position = new Vector3(
            midPointXCoordinate,
            0.26f,
            1.32f
	    );

	    GameObject server = Instantiate(this.serverContainer, position, Quaternion.identity);

        GameObject textObject = new GameObject();
        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = "";
	    textMesh.fontSize = 25;
	    textMesh.characterSize = 0.5f;
        textMesh.color = Color.black;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textObject.transform.position = server.transform.position + new Vector3(0, 0, 0);
        textObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        textObject.transform.SetParent(server.transform);
        textObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        textObject.transform.localPosition = new Vector3(0, 0, -0.5f);
        textObject.name = "StatusText";

        GameObject roundTextObject = new GameObject();
        TextMesh roundTextMesh = roundTextObject.AddComponent<TextMesh>();
        roundTextMesh.text = "Round 1";
	    roundTextMesh.fontSize = 18;
	    roundTextMesh.characterSize = 0.5f;
        roundTextMesh.color = Color.black;
        roundTextMesh.anchor = TextAnchor.MiddleCenter;
        roundTextMesh.alignment = TextAlignment.Center;
        roundTextObject.transform.position = server.transform.position + new Vector3(0, 0, 0);
        roundTextObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        roundTextObject.transform.SetParent(server.transform);
        roundTextObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        roundTextObject.transform.localPosition = new Vector3(0, 0.35f, -0.5f);
        roundTextObject.name = "RoundText";
    }
}
