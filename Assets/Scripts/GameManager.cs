using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;

public class GameManager : MonoBehaviour, IMixedRealitySpeechHandler, IMixedRealityTouchHandler
{
    #region Fields

    public AudioSource audioSource;

    public Camera player;

    public GameObject shell;
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
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);

        audioSource = GetComponent<AudioSource>();

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        SayHello();
    }

    #endregion

    #region Methods

    public void StartGame()
    {
        SpawnRandomCase();
    }

    public void SpawnRandomCase()
    {
        // Pick random suspect, weapon, clue
        int suspectChoice = Random.Range(0, 2);
        int weaponChoice = Random.Range(0, 2);
        int clueChoice = Random.Range(0, 2);

        // Randomly place objects in the scene
        Vector3 bodyPos = new Vector3(Random.Range(-9.0f, 9.0f), 2.0f, Random.Range(-9.0f, 9.0f));
        Vector3 whitePos = new Vector3(Random.Range(-9.0f, 9.0f), 2.0f, Random.Range(-9.0f, 9.0f));
        Vector3 mustardPos = new Vector3(Random.Range(-9.0f, 9.0f), 2.0f, Random.Range(-9.0f, 9.0f));
        Vector3 peacockPos = new Vector3(Random.Range(-9.0f, 9.0f), 2.0f, Random.Range(-9.0f, 9.0f));
        Vector3 weaponPos = new Vector3(Random.Range(-9.0f, 9.0f), 2.0f, Random.Range(-9.0f, 9.0f));
        Vector3 cluePos = new Vector3(Random.Range(-9.0f, 9.0f), 2.0f, Random.Range(-9.0f, 9.0f));

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

    #endregion
}
