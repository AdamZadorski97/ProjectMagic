using UnityEngine;

public class ResolutionSetter : MonoBehaviour
{
    void Start()
    {
        SetOptimalResolution(2560, 1440);  // You can change this target resolution as needed
    }

    void SetOptimalResolution(int maxWidth, int maxHeight)
    {
        // Get all available resolutions
        Resolution[] resolutions = Screen.resolutions;

        // Initialize variables to find the closest resolution to the target
        Resolution bestResolution = new Resolution();
        float desiredAspectRatio = 16f / 9f;
        int targetWidth = maxWidth;
        int targetHeight = maxHeight;

        float minDifference = float.MaxValue;

        foreach (Resolution res in resolutions)
        {
            // Calculate the aspect ratio of the current resolution
            float aspectRatio = (float)res.width / (float)res.height;

            // Calculate the difference to the desired aspect ratio
            float aspectDifference = Mathf.Abs(aspectRatio - desiredAspectRatio);

            // Calculate how close this resolution is to the target resolution
            float resolutionDifference = Mathf.Sqrt(Mathf.Pow(res.width - targetWidth, 2) + Mathf.Pow(res.height - targetHeight, 2));

            // Check if the aspect ratio matches 16:9 closely and if the resolution is closer to the target than previous ones
            if (aspectDifference < 0.01 && resolutionDifference < minDifference && res.width <= maxWidth && res.height <= maxHeight)
            {
                minDifference = resolutionDifference;
                bestResolution = res;
            }
        }

        // Set the screen resolution to the best found that fits the conditions
        if (bestResolution.width != 0 && bestResolution.height != 0)
        {
            Screen.SetResolution(bestResolution.width, bestResolution.height, false);
            Debug.Log("Resolution set to: " + bestResolution.width + "x" + bestResolution.height);
        }
        else
        {
            Debug.LogError("No suitable 16:9 resolution found that matches the criteria.");
        }
     
    }
}