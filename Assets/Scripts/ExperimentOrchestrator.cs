using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExperimentOrchestrator : MonoBehaviour
{
    public GameObject client;
    public GameObject server;
    public ObjectMover objectMover;
    public Gradient uploadGradient;
    public Gradient downloadGradient;

    private List<GameObject> instantiatedClients;
    private GameObject instantiatedServer;
    private List<GameObject> instantiatedWires = new();

    private int clientEpochs;
    private int serverRounds;
    private int currentServerRound = 1;

    private bool shouldBeginServerRounds = false;
    private bool shouldBeginClientEpochs = false;
    private bool shouldBeginClientToServerUpload = false;
    private bool shouldBeginServerToClientDownload = false;

    // Start is called before the first frame update
    void Start()
    {
        this.client.gameObject.SetActive(false);
        this.server.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldBeginClientEpochs)
        {
            StartCoroutine(StartClientEpochs());
	    }

        if(shouldBeginServerRounds)
        {
            StartCoroutine(StartServerRound());
	    }

        if(shouldBeginClientToServerUpload)
        {
            StartCoroutine(StartClientToServerUpload());
	    }

        if(shouldBeginServerToClientDownload)
        {
            StartCoroutine(StartServerToClientDownload());
	    }
    }

    public void RunExperiment(string dataset, int clientEpochs, int serverRounds, int numberOfClients)
    {
        this.client.gameObject.SetActive(true);
        this.server.gameObject.SetActive(true);
        this.objectMover.DestroyObjects();
        this.objectMover.SetupExperiment(numberOfClients, clientEpochs, dataset, serverRounds);

        this.clientEpochs = clientEpochs;
        this.serverRounds = serverRounds;
        this.instantiatedClients = GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name == "Client(Clone)").ToList();
        this.instantiatedServer = GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name == "Server(Clone)").First();
        this.shouldBeginClientEpochs = true;
    }

    private IEnumerator StartClientEpochs()
    {
        this.shouldBeginClientEpochs = false;

        Color startColor = Color.white;
        Color endColor = Color.red;

        for (int i = 0; i < this.clientEpochs; i++)
        {
            float duration = 5f;
            float timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                float t = timeElapsed / duration;
                foreach (GameObject obj in this.instantiatedClients)
                {
				    var textMesh = obj.GetComponentInChildren<TextMesh>();
				    textMesh.text = "Training\nepoch " + (i + 1) + ": " + Math.Floor(t*100) + "%";
                    obj.GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
                }
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }

        this.shouldBeginClientToServerUpload = true;
    }

    private IEnumerator StartClientToServerUpload()
    {
        this.shouldBeginClientToServerUpload = false;
        this.SetupWires();

        foreach (var client in this.instantiatedClients)
        {
            var textMesh = client.GetComponentInChildren<TextMesh>();
            textMesh.text = "Uploading\nmodel...";
	    }

        float t = 0.0f;
        float speed = 0.1f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            foreach (GameObject wire in this.instantiatedWires)
            {
                Renderer renderer = wire.GetComponent<Renderer>();
                renderer.material.color = uploadGradient.Evaluate(t);
            }
            yield return null;
        }

        // Reset the color of the client objects back to white
        foreach (GameObject obj in this.instantiatedClients)
        {
            obj.GetComponent<Renderer>().material.color = Color.white;
        }

        this.DestroyWires();

        foreach (var client in this.instantiatedClients)
        {
            var textMesh = client.GetComponentInChildren<TextMesh>();
            textMesh.text = "";
	    }

        this.shouldBeginServerRounds = true;
    }

    private IEnumerator StartServerRound()
    {
        this.shouldBeginServerRounds = false;
        print("start server round");
        float duration = 15f;
        float timeElapsed = 0.0f;
        Color startColor = Color.white;
        Color endColor = Color.green;

	    var textMesh = this.instantiatedServer.transform.Find("StatusText").GetComponent<TextMesh>();
	    textMesh.text = "Aggregating\nmodels...";

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            this.instantiatedServer.GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        this.serverRounds -= 1;
        this.currentServerRound++;

        this.shouldBeginServerToClientDownload = true;
    }

    private IEnumerator StartServerToClientDownload()
    {
        this.shouldBeginServerToClientDownload = false;
        this.SetupWires();

	    var textMesh = this.instantiatedServer.GetComponentInChildren<TextMesh>();
        textMesh.text = "Updating\nlocal\nmodels...";

        float t = 0.0f;
        float speed = 0.1f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            foreach (GameObject wire in instantiatedWires)
            {
                Renderer renderer = wire.GetComponent<Renderer>();
                renderer.material.color = downloadGradient.Evaluate(t);
            }
            yield return null;
        }

	    foreach (GameObject wire in instantiatedWires)
	    {
            wire.GetComponent<Renderer>().material.color = Color.white;
	    }

        this.DestroyWires();

	    this.instantiatedServer.GetComponent<Renderer>().material.color = Color.white;

	    var roundTextMesh = this.instantiatedServer.transform.Find("RoundText").GetComponent<TextMesh>();
        roundTextMesh.text = "Round " + this.currentServerRound;

        if (this.serverRounds > 0)
        { 
		    this.shouldBeginClientEpochs = true;
	    }
        else
        {
            roundTextMesh.text = "";
            this.EndExperiment();
	    }


        textMesh.text = "";
    }

    private void SetupWires()
    {
        if (this.instantiatedWires is not null && this.instantiatedWires.Count() > 0)
        { 
            foreach(var wire in this.instantiatedWires)
            {
                wire.SetActive(true);
	        }
	    }
        else
        { 
		    Vector3 serverPosition = this.instantiatedServer.transform.position;

		    foreach (var client in this.instantiatedClients)
			{
				GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			    cylinder.transform.localScale = new Vector3(0.01f, 1f, 0.01f);

			    Vector3 clientPosition = client.transform.position;
			    Vector3 direction = serverPosition - clientPosition;
			    Vector3 position = clientPosition + direction.normalized * 0.5f - direction.normalized * 0.25f;
			    cylinder.transform.position = position;
			    cylinder.transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0f, 90f, 0f) * Quaternion.Euler(0f, 0f, 90f);
			    this.instantiatedWires.Add(cylinder);
		    }
	    }
    }

    private void DestroyWires()
    {
        foreach (var wire in this.instantiatedWires)
        {
            wire.SetActive(false);
	    }
    }

    private void EndExperiment()
    { 
        foreach (var client in this.instantiatedClients)
        {
            Destroy(client);
	    }

        Destroy(this.instantiatedServer);

        this.client.gameObject.SetActive(false);
        this.server.gameObject.SetActive(false);
        this.objectMover.SpawnUI();
    }
}
