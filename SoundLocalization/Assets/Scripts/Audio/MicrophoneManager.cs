using HoloToolkit;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class MicrophoneManager : MonoBehaviour
{
    [Tooltip("How long a line of words can be before a new line is inserted")]
    private int lengthLimit;

    [Tooltip("A text area for the recognizer to display the recognized strings.")]
    public Text DictationDisplay;

    private DictationRecognizer dictationRecognizer;

    // Use this string to cache the text currently displayed in the text box.
    private StringBuilder textSoFar;

    // Using an empty string specifies the default microphone. 
    private static string deviceName = string.Empty;
    private int samplingRate;
    private const int messageLength = 10;
    public GameObject speechText;
    // Use this to reset the UI once the Microphone is done recording after it was started.
    private bool hasRecordingStarted;

    void Awake()
    {
        lengthLimit = 20;
        speechText = GameObject.Find("SpeechText");



        // 3.a: Create a new DictationRecognizer and assign it to dictationRecognizer variable.
        dictationRecognizer = new DictationRecognizer();


        // 3.a: Register for dictationRecognizer.DictationHypothesis and implement DictationHypothesis below
        // This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;

        //This sets the timeout to be 5 seconds after the user has started talking.
        dictationRecognizer.AutoSilenceTimeoutSeconds = 5;

        //We make this a large number so that the microphone does not continuously reset in CreateObjects
        dictationRecognizer.InitialSilenceTimeoutSeconds = 1000;

        // 3.a: Register for dictationRecognizer.DictationResult and implement DictationResult below
        // This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;

        // 3.a: Register for dictationRecognizer.DictationComplete and implement DictationComplete below
        // This event is fired when the recognizer stops, whether from Stop() being called, a timeout occurring, or some other error.
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;

        // 3.a: Register for dictationRecognizer.DictationError and implement DictationError below
        // This event is fired when an error occurs.
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;

        // Query the maximum frequency of the default microphone. Use 'unused' to ignore the minimum frequency.
        int unused;
        Microphone.GetDeviceCaps(deviceName, out unused, out samplingRate);

        // Use this string to cache the text currently displayed in the text box.
        textSoFar = new StringBuilder();

        // Use this to reset the UI once the Microphone is done recording after it was started.
        hasRecordingStarted = false;
    }

    void Update()
    {
        // 3.a: Add condition to check if dictationRecognizer.Status is Running
        if (hasRecordingStarted && !Microphone.IsRecording(deviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
            // Look at the StopRecording function.
            SendMessage("RecordStop");
        }

        if(!speechText.Equals(null))
        {
            speechText.transform.LookAt(Camera.main.transform);
            speechText.transform.Rotate(new Vector3(0, 180, 0));
        }

    }

    /// <summary>
    /// Turns on the dictation recognizer and begins recording audio from the default microphone.
    /// </summary>
    /// <returns>The audio clip recorded from the microphone.</returns>
    public AudioClip StartRecording()
    {
        // 3.a Shutdown the PhraseRecognitionSystem. This controls the KeywordRecognizers
        PhraseRecognitionSystem.Shutdown();

        // 3.a: Start dictationRecognizer
        dictationRecognizer.Start();

        // 3.a Uncomment this line
        speechText.GetComponent<TextMesh>().text = "Microphone is recording.";

        // Set the flag that we've started recording.
        hasRecordingStarted = true;

        // Start recording from the microphone for 10 seconds.
        return Microphone.Start(deviceName, false, messageLength, samplingRate);
    }

    /// <summary>
    /// Ends the recording session.
    /// </summary>
    public void StopRecording()
    {
        //This method has been changed to no longer end the recording session.
        //Instead, it will simply erase what has been recorded.
        // 3.a: Check if dictationRecognizer.Status is Running and stop it if so
        if (dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            dictationRecognizer.Stop();
        }

        textSoFar.Remove(0, textSoFar.ToString().Length);
        Microphone.End(deviceName);
        
    }

    /// <summary>
    /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
    /// </summary>
    /// <param name="text">The currently hypothesized recognition.</param>
    private void DictationRecognizer_DictationHypothesis(string text)
    {
        // 3.a: Set DictationDisplay text to be textSoFar and new hypothesized text
        // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event
        
        speechText.GetComponent<TextMesh>().text = textSoFar.ToString() + " " + text + "...";
        if (textSoFar.ToString().Length > lengthLimit)
        {
            lengthLimit += 20;
            textSoFar.Append("\n");
            if(lengthLimit == 100)
            {
                textSoFar.Remove(0, textSoFar.ToString().Length);
                lengthLimit = 0;
            }
        }
    }

    /// <summary>
    /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
    /// </summary>
    /// <param name="text">The text that was heard by the recognizer.</param>
    /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        // 3.a: Append textSoFar with latest text
        textSoFar.Append(text + ". ");

        // 3.a: Set DictationDisplay text to be textSoFar
        speechText.GetComponent<TextMesh>().text = textSoFar.ToString();
    }

    /// <summary>
    /// This event is fired when the recognizer stops, whether from Stop() being called, a timeout occurring, or some other error.
    /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
    /// </summary>
    /// <param name="cause">An enumerated reason for the session completing.</param>
    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        // If Timeout occurs, the user has been silent for too long.
        // With dictation, the default timeout after a recognition is 20 seconds.
        // The default timeout with initial silence is 5 seconds.
        if (cause == DictationCompletionCause.TimeoutExceeded)
        {
            Microphone.End(deviceName);

            speechText.GetComponent<TextMesh>().text = "Speech has ended.";
            textSoFar.Remove(0, textSoFar.ToString().Length);
            SendMessage("ResetAfterTimeout");
        }
    }

    /// <summary>
    /// This event is fired when an error occurs.
    /// </summary>
    /// <param name="error">The string representation of the error reason.</param>
    /// <param name="hresult">The int representation of the hresult.</param>
    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        // 3.a: Set DictationDisplay text to be the error string

        speechText.GetComponent<TextMesh>().text = error + "\nHRESULT: " + hresult;
    }
}