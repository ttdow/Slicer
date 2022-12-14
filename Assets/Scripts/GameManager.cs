using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEngine.UIElements;
using Microsoft.MixedReality.Toolkit.UI;

public class GameManager : MonoBehaviour, IMixedRealitySpeechHandler
{
    #region Fields

    private int gameState;

    public AudioSource audioSource;
    public AudioSource bgSource;
    public AudioClip bgMusic;
    public AudioClip victory;

    public Camera player;

    public GameObject shell;
    public TextMeshProUGUI shellText;
    public GameObject shellInput;
    public MRTKUGUIInputField shellInputText;

    public bool weaponFound;
    public bool clueFound;
    public bool conversationStarted;
    public bool playerGuess;
    public bool caseClosed;

    public GameObject murderer;
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
                //bgSource.Stop();
                break;

            case "play music":
                //bgSource.Play();
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

            case "knife":
                if (weaponFound) QuestionSuspect("knife");
                break;

            case "hammer":
                if (weaponFound) QuestionSuspect("hammer");
                break;

            case "wrench":
                if (weaponFound) QuestionSuspect("wrench");
                break;

            case "book":
                if (clueFound) QuestionSuspect("book");
                break;

            case "lighter":
                if (clueFound) QuestionSuspect("lighter");
                break;

            case "glove":
                if (clueFound) QuestionSuspect("glove");
                break;

            case "accuse":
                if (clueFound && weaponFound) QuestionSuspect("accuse");
                break;

            case "replay":
                gameState = 0;
                Destroy(body);
                Destroy(mustard);
                Destroy(white);
                Destroy(peacock);
                Destroy(clue);
                Destroy(weapon);
                murderer = null;
                weapon = null;
                clue = null;
                SetupGame();
                break;

            case "marco":
                if (!clueFound) clue.GetComponent<AudioSource>().Play();
                else if (!weaponFound) weapon.GetComponent<AudioSource>().Play();
                break;

            default:
                break;
        }
    }

    #endregion

    #region Unity Callbacks

    // Start is called before the first frame update
    void Start()
    {
        // Activate speech handling
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);

        // Get the BG music audio source
        //audioSource = GetComponent<AudioSource>();
        //bgSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();

        //if (bgSource == null)
        //{
          //  Debug.Log("bgSource is null.");
        //}

        // Set the initial game state
        gameState = 0;

        shell.GetComponent<FollowMeToggle>().ToggleFollowMeBehavior();

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
                    StartGame();

                    body.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                    mustard.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                    white.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                    peacock.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                    weapon.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                    clue.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                }
                break;

            case 1:
                // BUSINESS AS USUAL
                break;

            case 2:
                //bgSource.Stop();
                //bgSource.clip = victory;
                //bgSource.Play();
                shellText.text = "Congratulations! You solved the case.\n\n";
                shellText.text += "Type or say REPLAY to play again.";
                caseClosed = true;
                break;
        }

        if (gameState == 0)
        {
            if (body.transform.position.y > 0.5f)
            {
                body.GetComponent<CapsuleCollider>().enabled = false;
            }
            else
            {
                body.GetComponent<CapsuleCollider>().enabled = true;
            }

            if (mustard.transform.position.y > 0.5f)
            {
                mustard.GetComponent<CapsuleCollider>().enabled = false;
            }
            else
            {
                mustard.GetComponent<CapsuleCollider>().enabled = true;
            }

            if (white.transform.position.y > 0.5f)
            {
                white.GetComponent<CapsuleCollider>().enabled = false;
            }
            else
            {
                white.GetComponent<CapsuleCollider>().enabled = true;
            }

            if (peacock.transform.position.y > 0.5f)
            {
                peacock.GetComponent<CapsuleCollider>().enabled = false;
            }
            else
            {
                peacock.GetComponent<CapsuleCollider>().enabled = true;
            }

            if (clue.transform.position.y > 0.5f)
            {
                if (clue.GetComponent<CapsuleCollider>() != null)
                {
                    clue.GetComponent<CapsuleCollider>().enabled = false;
                }
                else
                {
                    clue.GetComponent<BoxCollider>().enabled = false;
                }
            }
            else
            {
                if (clue.GetComponent<CapsuleCollider>() != null)
                {
                    clue.GetComponent<CapsuleCollider>().enabled = true;
                }
                else
                {
                    clue.GetComponent<BoxCollider>().enabled = true;
                }
            }

            if (weapon.transform.position.y > 0.5f)
            {
                weapon.GetComponent<CapsuleCollider>().enabled = false;
            }
            else
            {
                weapon.GetComponent<CapsuleCollider>().enabled = true;
            }
        }

        if (conversationStarted)
        {
            if (!shell.activeInHierarchy)
            {
                conversationStarted = false;
            }
        }
    }

    #endregion

    #region Methods

    // Prompts the player to look around to ensure enough spatial mesh is available for the game objects
    public void SetupGame()
    {
        // Set initial game state
        weaponFound = false;
        clueFound = false;
        conversationStarted = false;
        playerGuess = false;
        caseClosed = false;

        // Prepare audio
        //audioSource.Play();
        //bgSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        //bgSource.clip = bgMusic;
        //bgSource.Play();

        shellText.text = "The game will start shortly. Please move around and look at your surroundings to build a spatial mesh.";
        shellInput.SetActive(false);

        SpawnRandomCase();
    }

    // Spawns a random assortment of objects into the scene
    public void SpawnRandomCase()
    {
        // Pick random suspect, weapon, clue
        int suspectChoice = Random.Range(0, 2);
        int weaponChoice = Random.Range(0, 2);
        int clueChoice = Random.Range(0, 2);

        // Randomly place objects in the scene
        Vector3 bodyPos = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.1f, 0.2f), Random.Range(-5.0f, 5.0f));
        Vector3 whitePos = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.1f, 0.2f), Random.Range(-5.0f, 5.0f));
        Vector3 mustardPos = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.1f, 0.2f), Random.Range(-5.0f, 5.0f));
        Vector3 peacockPos = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.1f, 0.2f), Random.Range(-5.0f, 5.0f));
        Vector3 weaponPos = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.1f, 0.2f), Random.Range(-5.0f, 5.0f));
        Vector3 cluePos = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.1f, 0.2f), Random.Range(-5.0f, 5.0f));

        // Create the avatars
        body = GameObject.Instantiate(bodyPrefab, bodyPos, Quaternion.identity);
        white = GameObject.Instantiate(whitePrefab, whitePos, Quaternion.identity);
        mustard = GameObject.Instantiate(mustardPrefab, mustardPos, Quaternion.identity);
        peacock = GameObject.Instantiate(peacockPrefab, peacockPos, Quaternion.identity);

        // Mr. Body is dead; he should not be standing up
        body.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));

        // Select whodunnit
        switch(suspectChoice)
        {
            case 0:
                murderer = white;
                break;
            case 1:
                murderer = mustard;
                break;
            case 2:
                murderer = peacock;
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

    // Checks if the objects have been placed successfully
    public bool CheckReady()
    {
        GameObject obj = body;
        int counter = 0;

        if (obj.transform.position.y < -0.1f && obj.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            counter++;
        }

        obj = mustard;

        if (obj.transform.position.y < -0.1f && obj.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            counter++;
        }

        obj = white;

        if (obj.transform.position.y < -0.1f && obj.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            counter++;
        }

        obj = peacock;

        if (obj.transform.position.y < -0.1f && obj.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            counter++;
        }

        obj = weapon;

        if (obj.transform.position.y < -0.1f && obj.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            counter++;
        }

        obj = clue;

        if (obj.transform.position.y < -0.1f && obj.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            counter++;
        }

        Vector3 totalV = body.GetComponent<Rigidbody>().velocity +
                         mustard.GetComponent<Rigidbody>().velocity +
                         white.GetComponent<Rigidbody>().velocity +
                         peacock.GetComponent<Rigidbody>().velocity +
                         weapon.GetComponent<Rigidbody>().velocity +
                         clue.GetComponent<Rigidbody>().velocity;

        // If all the objects are stationary, return true
        if (totalV.magnitude < 0.1f && counter > 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartGame()
    {
        shellInput.SetActive(true);
        gameState = 1;

        shellText.text = "In the year 3022 SOCIETY has moved beyond the physical world and into the cloud. ";
        shellText.text += "People live as near immortals in the digital world with nothing to fear. ";
        shellText.text += "You are part of the Slicers, cybersecurity experts who investigate crimes in this world. ";
        shellText.text += "For the first time in the history of your organization you've been tasked with solving a murder. ";
        shellText.text += "Not only did someone murder this individual, but every copy of them on the network. ";
        shellText.text += "Who could have perpetrated this heinous crime? Question the suspects, search your area for clues, and find the killer!";
    }

    // Called if an object is interacted with
    public void ObjectFound(string tag)
    {
        switch(tag)
        {
            case "weapon":
                weaponFound = true;
                break;

            case "clue":
                clueFound = true;
                break;

            case "mustard":
                SayHello();
                break;

            case "white":
                SayHello();
                break;

            case "peacock":
                SayHello();
                break;

            case "body":
                Debug.Log("Yep. It's a dead body.");
                break;

            default:
                break;
        }
    }

    // Start a conversation with an NPC
    public void SayHello()
    {
        string name = "";
        GameObject partner = null;

        if (!conversationStarted)
        {
            if (Vector3.Distance(player.transform.position, white.transform.position) < 2.0f)
            {
                white.GetComponent<AudioSource>().Play();
                conversationStarted = true;
                name += "Ms. White";
                partner = white;
            }
            else if (Vector3.Distance(player.transform.position, mustard.transform.position) < 2.0f)
            {
                mustard.GetComponent<AudioSource>().Play();
                conversationStarted = true;
                name += "Col. Mustard";
                partner = mustard;
            }
            else if (Vector3.Distance(player.transform.position, peacock.transform.position) < 2.0f)
            {
                peacock.GetComponent<AudioSource>().Play();
                conversationStarted = true;
                name += "Mrs. Peacock";
                partner = peacock;
            }
        }

        if (conversationStarted && partner != null)
        {
            shell.SetActive(true);
            shell.transform.position = (player.transform.position + partner.transform.position) / 2.0f;

            shellText.text = "Hello, I'm " + name + ".\n\n";
            shellText.text += "What can I help you with, officer? (try typing or saying a KEYWORD)\n";
            shellInputText.text = "";
        }
    }

    // Called if text is submitted in an interrogation window
    public void QuestionSuspect()
    {
        string keyword = shellInputText.text;
        QuestionSuspect(keyword);
    }

    // Controls the logic behind asking suspects about objects
    public void QuestionSuspect(string keyword)
    {
        keyword = keyword.ToLower();

        if (conversationStarted)
        {
            GameObject conversationPartner = null;

            if (Vector3.Distance(player.transform.position, white.transform.position) < 1.0f)
            {
                conversationPartner = white;
            }
            else if (Vector3.Distance(player.transform.position, mustard.transform.position) < 1.0f)
            {
                conversationPartner = mustard;
            }
            else if (Vector3.Distance(player.transform.position, peacock.transform.position) < 1.0f)
            {
                conversationPartner = peacock;
            }

            switch (keyword)
            {
                case "knife":
                    if (weaponFound)
                    {
                        if (murderer == conversationPartner)
                        {
                            shellText.text = "Hey! My knife... I've been looking everywhere for that.";
                        }
                        else
                        {
                            shellText.text = "I've never seen that knife before in my life.";
                        }
                    }
                    break;

                case "wrench":
                    if (weaponFound)
                    {
                        if (murderer == conversationPartner)
                        {
                            shellText.text = "Hey! My wrench... I've been looking everywhere for that.";
                        }
                        else
                        {
                            shellText.text = "I've never seen that wrench before in my life.";
                        }
                    }
                    break;

                case "hammer":
                    if (weaponFound)
                    {
                        if (murderer == conversationPartner)
                        {
                            shellText.text = "Hey! My hammer... I've been looking everywhere for that.";
                        }
                        else
                        {
                            shellText.text = "I've never seen that hammer before in my life.";
                        }
                    }
                    break;

                case "book":
                    if (clueFound)
                    {
                        if (murderer == conversationPartner)
                        {
                            shellText.text = "Hey! My book... I've been looking everywhere for that.";
                        }
                        else
                        {
                            shellText.text = "I've never seen that book before in my life.";
                        }
                    }
                    break;

                case "lighter":
                    if (clueFound)
                    {
                        if (murderer == conversationPartner)
                        {
                            shellText.text = "Hey! My lighter... I've been looking everywhere for that.";
                        }
                        else
                        {
                            shellText.text = "I've never seen that lighter before in my life.";
                        }
                    }
                    break;

                case "glove:":
                    if (clueFound)
                    {
                        if (murderer == conversationPartner)
                        {
                            shellText.text = "Hey! My glove... I've been looking everywhere for that.";
                        }
                        else
                        {
                            shellText.text = "I've never seen that glove before in my life.";
                        }
                    }
                    break;

                case "accuse":
                    Accuse(conversationPartner);
                    break;

                default:
                    break;
            }
        }
    }

    // Accuses the suspect being interrogated
    public void Accuse(GameObject suspect)
    {
        if (suspect == murderer)
        {
            playerGuess = true;
        }

        if (playerGuess)
        {
            gameState = 2;
        }
    }

    #endregion
}
