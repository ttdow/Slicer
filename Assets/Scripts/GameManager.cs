using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using TMPro;
using static UnityEngine.ParticleSystem;
using System.Net.Sockets;
using UnityEditor.Experimental.GraphView;

public class GameManager : MonoBehaviour, IMixedRealitySpeechHandler, IMixedRealityTouchHandler
{
    #region Fields

    private int gameState;

    public AudioSource audioSource;

    public Camera player;

    public GameObject shell;
    public TextMeshProUGUI shellText;
    public GameObject shellInput;

    public GameObject suspect;
    public GameObject weapon;
    public GameObject clue;

    private GameObject body;
    private GameObject white;
    private GameObject mustard;
    private GameObject peacock;

    public GameObject bodyPrefab;
    public GameObject mustardPrefab;
    public GameObject whitePrefab;
    public GameObject peacockPrefab;

    public GameObject bookPrefab;
    public GameObject lighterPrefab;
    public GameObject glovePrefab;

    public GameObject knifePrefab;
    public GameObject wrenchPrefab;
    public GameObject hammerPrefab;

    #endregion

    #region SpeechHandler Callbacks

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        switch(eventData.Command.Keyword.ToLower())
        {
            case "stop music":
                audioSource.Stop();
                break;

            case "play music":
                audioSource.Play();
                break;

            case "command":
                if (shell.activeInHierarchy)
                {
                    shell.SetActive(false);
                }
                else
                {
                    shell.SetActive(true);
                }
                break;

            case "start":
                StartGame();
                break;

            case "hello":
                SayHello();
                break;

            default:
                break;
        }
    }

    #endregion

    #region TouchHandler Callbacks
    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        Debug.Log("Touched!");
    }

    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Unity Callbacks

    // Start is called before the first frame update
    void Start()
    {
        // Activate speech handling
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);

        // Get the BG music audio source
        audioSource = GetComponent<AudioSource>();

        // Set the initial game state
        gameState = 0;

        SetupGame();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case 0:
                // SETUP THE SCENE
                if (CheckReady())
                {
                    gameState = 1;

                    StartGame();
                }
                break;

            case 1:
                // BUSINESS AS USUAL
                break;

            case 2:
                // YOU WIN
                break;
        }
    }

    #endregion

    #region Methods

    public void SetupGame()
    {
        shellText.text = "The game will start shortly. Please move around and look at your surroundings to build a spatial mesh.";
        shellInput.SetActive(false);

        SpawnRandomCase();
    }

    public void StartGame()
    {
        shellInput.SetActive(true);
        shellText.text = "In the year 3022 SOCIETY has moved beyond the physical world and into the cloud. People live as immortals in the digital world with nothing to fear. You are part of the Slicers, cybersecurity experts who investigate crimes in this world. For the first time in the history of your organization you've been tasked with solving a murder. Not only did someone murder this individual, but every copy of them on the network. Who could have perpetrated this heinous crime? Question the suspects, search your area for clues, and find the killer!";
    }

    public void SpawnRandomCase()
    {
        // Pick random suspect, weapon, clue
        int suspectChoice = Random.Range(0, 2);
        int weaponChoice = Random.Range(0, 2);
        int clueChoice = Random.Range(0, 2);

        // Randomly place objects in the scene
        Vector3 bodyPos = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(1.0f, 2.0f), Random.Range(-9.0f, 9.0f));
        Vector3 whitePos = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(1.0f, 2.0f), Random.Range(-9.0f, 9.0f));
        Vector3 mustardPos = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(1.0f, 2.0f), Random.Range(-9.0f, 9.0f));
        Vector3 peacockPos = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(1.0f, 2.0f), Random.Range(-9.0f, 9.0f));
        Vector3 weaponPos = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(1.0f, 2.0f), Random.Range(-9.0f, 9.0f));
        Vector3 cluePos = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(1.0f, 2.0f), Random.Range(-9.0f, 9.0f));

        // Create the avatars
        body = GameObject.Instantiate(bodyPrefab, bodyPos, Quaternion.identity);
        white = GameObject.Instantiate(whitePrefab, whitePos, Quaternion.identity);
        mustard = GameObject.Instantiate(mustardPrefab, mustardPos, Quaternion.identity);
        peacock = GameObject.Instantiate(peacockPrefab, peacockPos, Quaternion.identity);

        // Select whodunnit
        switch(suspectChoice)
        {
            case 0:
                suspect = white;
                break;
            case 1:
                suspect = mustard;
                break;
            case 2:
                suspect = peacock;
                break;
        }

        // Select the murder weapon
        switch(weaponChoice)
        {
            case 0:
                weapon = GameObject.Instantiate(knifePrefab, weaponPos, Quaternion.identity);
                break;
            case 1:
                weapon = GameObject.Instantiate(hammerPrefab, weaponPos, Quaternion.identity);
                break;
            case 2:
                weapon = GameObject.Instantiate(wrenchPrefab, weaponPos, Quaternion.identity);
                break;
        }

        // Select the clue left behind
        switch (clueChoice)
        {
            case 0:
                clue = GameObject.Instantiate(bookPrefab, cluePos, Quaternion.identity);
                break;
            case 1:
                clue = GameObject.Instantiate(lighterPrefab, cluePos, Quaternion.identity);
                break;
            case 2:
                clue = GameObject.Instantiate(glovePrefab, cluePos, Quaternion.identity);
                break;
        }
    }

    public bool CheckReady()
    {
        // Detect when all the objects in the scene stop moving
        Vector3 totalV = body.GetComponent<Rigidbody>().velocity +
                     white.GetComponent<Rigidbody>().velocity +
                     mustard.GetComponent<Rigidbody>().velocity +
                     peacock.GetComponent<Rigidbody>().velocity +
                     weapon.GetComponent<Rigidbody>().velocity +
                     clue.GetComponent<Rigidbody>().velocity;

        if (totalV.magnitude < 1.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void HideObjects()
    {
        Vector3 totalV = Vector3.zero;

        do
        {
            totalV = weapon.GetComponent<Rigidbody>().velocity +
                     clue.GetComponent<Rigidbody>().velocity;

        } while (totalV.magnitude < 1.0f);

        int dir = Random.Range(0, 8);

        // Hide weapon
        switch (dir)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
        }
    }

    public void SayHello()
    {
        if(Vector3.Distance(player.transform.position,  white.transform.position) < 2.0f)
        {
            white.GetComponent<AudioSource>().Play();
        }
        else if(Vector3.Distance(player.transform.position, mustard.transform.position) < 2.0f)
        {
            mustard.GetComponent<AudioSource>().Play();
        }
        else if(Vector3.Distance(player.transform.position, peacock.transform.position) < 2.0f)
        {
            peacock.GetComponent<AudioSource>().Play();
        }
    }

    public void Accuse()
    {
        bool playerGuess = false;

        if (playerGuess)
        {
            gameState = 2;
        }
    }

    #endregion
}
