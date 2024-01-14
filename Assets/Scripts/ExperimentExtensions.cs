using System;
using System.Reflection;

public static class ExperimentExtensions
{
    public static void Normalize(this ExperimentResult[] results)
    {
        double minNumberOfClients = int.MaxValue;
        double maxNumberOfClients = int.MinValue;
        double minClientEpochs = int.MaxValue;
        double maxClientEpochs = int.MinValue;
        double minServerRounds = int.MaxValue;
        double maxServerRounds = int.MinValue;
        double minExperimentTime = int.MaxValue;
        double maxExperimentTime = int.MinValue;
        double minAccuracy = int.MaxValue;
        double maxAccuracy = int.MinValue;
        double minDataTransmitted = int.MaxValue;
        double maxDataTransmitted = int.MinValue;

        foreach (var result in results)
        {
            minNumberOfClients = Math.Min(minNumberOfClients, result.NumberOfClients);
            maxNumberOfClients = Math.Max(maxNumberOfClients, result.NumberOfClients);
            minClientEpochs = Math.Min(minClientEpochs, result.ClientEpochs);
            maxClientEpochs = Math.Max(maxClientEpochs, result.ClientEpochs);
            minServerRounds = Math.Min(minServerRounds, result.ServerRounds);
            maxServerRounds = Math.Max(maxServerRounds, result.ServerRounds);
            minExperimentTime = Math.Min(minExperimentTime, result.ExperimentTime);
            maxExperimentTime = Math.Max(maxExperimentTime, result.ExperimentTime);
            minAccuracy = Math.Min(minAccuracy, result.Accuracy);
            maxAccuracy = Math.Max(maxAccuracy, result.Accuracy);
            minDataTransmitted = Math.Min(minDataTransmitted, result.DataTransmitted);
            maxDataTransmitted = Math.Max(maxDataTransmitted, result.DataTransmitted);
        }

        foreach (var result in results)
        {
            result.NumberOfClients = ScaleValue(result.NumberOfClients, minNumberOfClients, maxNumberOfClients) * 0.5;
            result.ClientEpochs = ScaleValue(result.ClientEpochs, minClientEpochs, maxClientEpochs) * 0.5;
            result.ServerRounds = ScaleValue(result.ServerRounds, minServerRounds, maxServerRounds) * 0.5;
            result.ExperimentTime = ScaleValue(result.ExperimentTime, minExperimentTime, maxExperimentTime) * 0.5;
            result.Accuracy = ScaleValue(result.Accuracy, minAccuracy, maxAccuracy) * 0.5;
            result.DataTransmitted = ScaleValue(result.DataTransmitted, minDataTransmitted, maxDataTransmitted) * 0.5;
        }
    }

    public static object GetPropertyValue(this ExperimentResult result, string propertyName)
    {
        Type type = result.GetType();
        PropertyInfo propertyInfo = type.GetProperty(propertyName);
        return propertyInfo.GetValue(result, null);
    }

    private static double ScaleValue(double value, double min, double max)
    {
        return (value - ((max + min) / 2)) / ((max - min) / 2);
    }
}
