using UnityEngine;
using TMPro;
using System;
using Microsoft.MixedReality.Toolkit.UI;

public class ScreenText : MonoBehaviour
{
    public TMP_Text text;
    public ExperimentOrchestrator experimentOrchestrator;

    private string dataset;
    private int numberOfClients;
    private int clientEpochs;
    private int serverRounds;

    private float TimeLeft;
    private bool TimerOn;

    private bool isStandardTextShowing;

    // Start is called before the first frame update
    void Start()
    {
        this.isStandardTextShowing = true;
        this.dataset = "Not defined";
        this.numberOfClients = 0;
        this.clientEpochs = 0;
        this.serverRounds = 0;
        this.TimeLeft = 5;
        this.UpdateDefinitionText();
    }

    private void Update()
    {
        if (TimerOn)
        { 
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                UpdateRunText(TimeLeft);
	        }
            else
            {
                TimeLeft = 0;
                TimerOn = false;
                this.experimentOrchestrator.RunExperiment(this.dataset, this.clientEpochs, this.serverRounds, this.numberOfClients);
	        }
	    }
    }

    public void StartExperimentTimer()
    {
        TimerOn = true;
    }

    public void ShowInstructions()
    {
        if (isStandardTextShowing)
        {
            text.text = "You can use the buttons and sliders to define a Federated Learning experiment training one of the MNIST datasets. Once you start it, the experiment will follow the two steps for each server round:\n\n\t1 - Client training: each client will train a local model for each client epoch and upload the model to the server.\n\t2 - Server aggregation: the global server will aggregate these models and update each client with the new aggregated model.";
            this.isStandardTextShowing = false;
	    }
        else
        {
            this.UpdateDefinitionText();
	    }

    }

    public void ModifyDataset(string dataset)
    {
        this.dataset = dataset;
        this.UpdateDefinitionText();
    }

    public void ModifyServerRounds(PinchSlider slider)
    {
        this.serverRounds = (int)Math.Round(slider.SliderValue*10);
        this.UpdateDefinitionText();
    }

    public void ModifyClientEpochs(PinchSlider slider)
    {
        this.clientEpochs = (int)Math.Round(slider.SliderValue*10);
        this.UpdateDefinitionText();
    }

    public void ModifyNumberOfClients(PinchSlider slider)
    {
        this.numberOfClients = (int)Math.Round(slider.SliderValue*10);
        this.UpdateDefinitionText();
    }

    private void UpdateDefinitionText()
    { 
        text.text = "Dataset: " + dataset + "\n\nNumber of clients: " + numberOfClients + "\n\nClient Epochs: " + this.clientEpochs + "\n\nServer Rounds: " + this.serverRounds;
    }

    private void UpdateRunText(float currentTime)
    {
        currentTime += 1;
        currentTime = (int)Math.Round(currentTime);
        text.text = "Your experiment will start in " + (currentTime - 1);
    }
}
