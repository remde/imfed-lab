# Immersive Virtual Reality Laboratory for Federated Learning Experiments

<img src="https://github.com/remde/imfed-lab/assets/12024169/051e55fb-9c52-4c3c-9e89-af8804baa983" width="100" height="100">

This is a proof of concept for a immersive 3D lab for federated learning experiments. It has been tested with an Oculus Rift device, but should be multiplatform.

The experience is divided in three parts:
1. **Experiment definition**. The user can define an experiment choosing the dataset, client epochs, server rounds, and number of clients.
![Screenshot 2024-02-08 at 17 38 03](https://github.com/remde/imfed-lab/assets/12024169/4f0913dc-6c25-4b92-82ce-7c21064c20ee | width=200)
2. **Experiment execution.** The experiment defined in (1) is run (simulated), and all the abstract concepts (client, server, connections, rounds, epochs, uploads, downloads, etc.) are shown as concrete models.
![Screenshot 2024-02-08 at 17 32 29](https://github.com/remde/imfed-lab/assets/12024169/018e7d16-a2ae-4d08-b45d-fd4858b18ba1 | width=100)
3. **Results visualization.** The results for the experiments run is shown. Here, user can tweak the axis to show specific information, as well as playing around with the scatterplot: zooming, pressing each data point for more info, turning it around, etc., all with their own hands while using a VR headset.
![Screenshot 2024-02-08 at 17 31 26](https://github.com/remde/imfed-lab/assets/12024169/a0d53f02-022a-47a1-b036-2f3ecdf4328b)

The most interesting scripts that enable all of this are `ResultsOrchestrator` and `ExperimentOrchestrator` found in the [scripts folder](https://github.com/remde/imfed-lab/tree/main/Assets/Scripts).

# Credits

The work was done on top of [IntroToImmersiveAnalytics](https://github.com/jorge-wagner/IntroToImmersiveAnalytics) repository, a pre-configured project with a working MRTK scene. All credit to [Jorge Wagner](https://github.com/jorge-wagner).

There are other scenes as well (IATK, Bing Maps, etc.) in the base repository which I have not removed from my code (yet!), so there might be unnecessary files in my repository.
