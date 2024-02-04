using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;

public class ResultsOrchestrator : MonoBehaviour
{
    public TextMeshProUGUI xAxisLabel;
    public TextMeshProUGUI yAxisLabel;
    public TextMeshProUGUI zAxisLabel;

    public ObjectMover objectMover;
    public GameObject pointPrefab;
    public GameObject controlCanvas;

    public Material highlightedMaterial;
    public Material defaultMaterial;

    ExperimentResult[] experimentData;
    List<ScatterplotDataPoint> scatterPlotPoints;

    private string currentXCoordinate;
    private string currentYCoordinate;
    private string currentZCoordinate;

    private GameObject currentHighlightedX;
    private GameObject currentHighlightedY;
    private GameObject currentHighlightedZ;

    private bool shouldRenderMnist = true;
    private bool shouldRenderFashionMnist = true;


    // Start is called before the first frame update
    void Start()
    {
        this.controlCanvas.SetActive(false);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupResults()
    {
        this.controlCanvas.SetActive(true);
        gameObject.SetActive(true);
        this.objectMover.DestroyObjects();
        string json = File.ReadAllText("./Assets/Scripts/results.json");

        var data = JsonConvert.DeserializeObject<ExperimentResult[]>(json);
        data.Normalize();
        this.experimentData = data;
        this.HighlightButton("accuracy_x");
        this.HighlightButton("dataTransmitted_y");
        this.HighlightButton("experimentTime_z");
        this.CreateScatterplot("Accuracy", "DataTransmitted", "ExperimentTime");
	}

    public void setShouldRenderFashionMnist()
    {
        this.shouldRenderFashionMnist = !this.shouldRenderFashionMnist;
        this.CreateScatterplot("", "", "");
    }

    public void setShouldRenderMnist()
    {
        this.shouldRenderMnist = !this.shouldRenderMnist;
        this.CreateScatterplot("", "", "");
    }

    public void populateXCoordinate(string coordinate)
    {
        this.CreateScatterplot(coordinate, "", "");
    }

    public void populateYCoordinate(string coordinate)
    {
        this.CreateScatterplot("", coordinate, "");
    }

    public void populateZCoordinate(string coordinate)
    {
        this.CreateScatterplot("", "", coordinate);
    }

    public void HighlightButton(string buttonTag)
    {
        // make selected highlighted
        var button = GameObject.FindGameObjectWithTag(buttonTag);
        var image = button.GetComponent<Image>();
        image.material = this.highlightedMaterial;

        string axis = buttonTag.Split('_')[1];
        GameObject buttonToDefault;
        if (axis == "x")
        {
            if (this.currentHighlightedX == button)
            {
		       return;
	        }
            buttonToDefault = this.currentHighlightedX;
            this.currentHighlightedX = button;
	    }
        else if (axis == "y")
        {
            if (this.currentHighlightedY == button)
            {
		       return;
	        }
            buttonToDefault = this.currentHighlightedY;
            this.currentHighlightedY = button;
	    }
        else
        {
            if (this.currentHighlightedZ == button)
            {
		       return;
	        }
            buttonToDefault = this.currentHighlightedZ;
            this.currentHighlightedZ = button;
	    }
        if (buttonToDefault == null)
        {
            return;
	    }
        var imageToDefault = buttonToDefault.GetComponent<Image>();
        imageToDefault.material = defaultMaterial;
    }

    public void ExitResults()
    {
        if (this.scatterPlotPoints != null)
        {
            this.DumpPoints();
	    }

        this.controlCanvas.SetActive(false);
        this.objectMover.SpawnUI();
    }

    private void CreateScatterplot(string xCoordinate, string yCoordinate, string zCoordinate)
    { 
        if (this.scatterPlotPoints != null)
        {
            this.DumpPoints();
	    }

        this.currentXCoordinate = string.IsNullOrEmpty(xCoordinate) ? this.currentXCoordinate : xCoordinate;
        this.currentYCoordinate = string.IsNullOrEmpty(yCoordinate) ? this.currentYCoordinate : yCoordinate;
        this.currentZCoordinate = string.IsNullOrEmpty(zCoordinate) ? this.currentZCoordinate : zCoordinate;

        this.xAxisLabel.text = this.currentXCoordinate;
        this.yAxisLabel.text = this.currentYCoordinate;
        this.zAxisLabel.text = this.currentZCoordinate;

        this.scatterPlotPoints = new List<ScatterplotDataPoint>();

        ExperimentResult[] data = new ExperimentResult[this.experimentData.Length];
        Array.Copy(this.experimentData, data, this.experimentData.Length);

        if (!this.shouldRenderMnist)
        {
            data = data.Where(dataPoint => dataPoint.Dataset != "mnist").ToArray();
	    }

        if (!this.shouldRenderFashionMnist)
        {
            data = data.Where(dataPoint => dataPoint.Dataset != "fashion-mnist").ToArray();
	    }

        foreach (var point in data)
        {
            float x = System.Convert.ToSingle(point.GetPropertyValue(this.currentXCoordinate));
            float y = System.Convert.ToSingle(point.GetPropertyValue(this.currentYCoordinate));
            float z = System.Convert.ToSingle(point.GetPropertyValue(this.currentZCoordinate));
            
            ScatterplotDataPoint newDataPoint = Instantiate(this.pointPrefab, new Vector3(x, y, z), Quaternion.identity).
                GetComponent<ScatterplotDataPoint>();

            newDataPoint.transform.position += this.transform.position;
            newDataPoint.transform.parent = this.transform;
            newDataPoint.gameObject.name = point.NumberOfClients.ToString();

            newDataPoint.dataClass = point.Dataset;

            newDataPoint.textLabel.text = "Dataset: " + point.Dataset + "\n\n"
                + "Accuracy: " + point.Accuracy.ToString("0.00") + "\n\n"
                + "Experiment Time: " + point.ExperimentTime.ToString("0.00") + "\n\n"
                + "Number of Clients: " + point.NumberOfClients.ToString("0.00") + "\n\n"
                + "Server Rounds: " + point.ServerRounds.ToString("0.00") + "\n\n"
                + "Client Epochs: " + point.ClientEpochs.ToString("0.00") + "\n\n"
                + "Data Transmitted: " + point.DataTransmitted.ToString("0.00") + "\n\n";

            Color color = point.Dataset == "mnist" ? Color.red : Color.black;
            color.a = 1f;
            newDataPoint.GetComponent<Renderer>().material.color = color;
            newDataPoint.pointColor = color;

            scatterPlotPoints.Add(newDataPoint);
        }
    }

    private void DumpPoints()
    {
        foreach (ScatterplotDataPoint point in this.scatterPlotPoints)
        {
            Destroy(point.gameObject);
        }
    }
}
