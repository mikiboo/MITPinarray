#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;

public class CameraRecorder : MonoBehaviour
{
    public Button recordButton;
    public Camera cameraToRecord;
    private RecorderController recorderController;
    private bool isRecording = false;

    void Start()
    {
        recordButton.onClick.AddListener(ToggleRecording);
        SetupRecorder();
    }

    void SetupRecorder()
    {
        RecorderControllerSettings controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        recorderController = new RecorderController(controllerSettings);

        MovieRecorderSettings videoRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        videoRecorder.name = "My Video Recorder";
        videoRecorder.Enabled = true;
        videoRecorder.VideoBitRateMode = UnityEditor.VideoBitrateMode.High;

        CameraInputSettings cameraInputSettings = new CameraInputSettings
        {
            Source = ImageSource.TaggedCamera, // Use MainCamera instead of Camera
            OutputWidth = 1920,
            OutputHeight = 1080,
            CameraTag = cameraToRecord.tag,
            FlipFinalOutput = true // Make sure your camera has a tag
        };
        videoRecorder.ImageInputSettings = cameraInputSettings;
        videoRecorder.AudioInputSettings.PreserveAudio = true;
        videoRecorder.OutputFile = $"{Application.dataPath}/RecordedVideo/Camera2_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.mp4";

        controllerSettings.AddRecorderSettings(videoRecorder);
        controllerSettings.SetRecordModeToManual();
        controllerSettings.FrameRate = 60;
    }

    void ToggleRecording()
    {
        if (isRecording)
        {
            StopRecording();
        }
        else
        {
            RecorderControllerSettings controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            recorderController = new RecorderController(controllerSettings);

            MovieRecorderSettings videoRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            videoRecorder.name = "My Video Recorder";
            videoRecorder.Enabled = true;
            videoRecorder.VideoBitRateMode = UnityEditor.VideoBitrateMode.High;

            CameraInputSettings cameraInputSettings = new CameraInputSettings
            {
                Source = ImageSource.TaggedCamera, // Use MainCamera instead of Camera
                OutputWidth = 1920,
                OutputHeight = 1080,
                CameraTag = cameraToRecord.tag,
                FlipFinalOutput = true // Make sure your camera has a tag
            };
            videoRecorder.ImageInputSettings = cameraInputSettings;
            videoRecorder.AudioInputSettings.PreserveAudio = true;
            videoRecorder.OutputFile = $"{Application.dataPath}/RecordedVideo/Camera2_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.mp4";

            controllerSettings.AddRecorderSettings(videoRecorder);
            controllerSettings.SetRecordModeToManual();
            controllerSettings.FrameRate = 30;
            StartRecording();
        }
        isRecording = !isRecording;
    }

    void StartRecording()
    {
        recorderController.PrepareRecording();
        recorderController.StartRecording();
    }

    void StopRecording()
    {
        recorderController.StopRecording();
    }
}
#endif
