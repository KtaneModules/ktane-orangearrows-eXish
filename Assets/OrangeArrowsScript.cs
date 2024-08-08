using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OrangeArrowsScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombInfo bomb;
    public KMColorblindMode colorblind;
    public KMSelectable[] buttons;
    public GameObject numDisplay;
    public GameObject colorblindText;
    public KMRuleSeedable RuleSeedable;

    public Renderer bulb1;
    public Renderer bulb2;
    public Renderer bulb3;

    public Material[] colors;

    private string[] moves;
    private string[] movesEDIT;
    private int current;

    private int rotator;

    private int stage;

    private bool cbEnabled;
    private bool activated;

    private Coroutine co;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    private bool trueModuleSolved;

    private string[] _dirs = new string[] { "UP", "DOWN", "LEFT", "RIGHT" };
    private int _ruleSeedCount = -1;
    private int _ruleSeedDir = -1;

    void Awake()
    {
        stage = 1;
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        cbEnabled = colorblind.ColorblindModeActive;
        foreach (KMSelectable obj in buttons)
        {
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
        GetComponent<KMBombModule>().OnActivate += OnActivate;
    }

    void Start()
    {
        var rnd = RuleSeedable.GetRNG();
        _ruleSeedCount = rnd.Next(0, 3) + 3; // 3, 4, 5
        _ruleSeedDir = rnd.Next(0, 3); // opp, cw, ccw
        if (rnd.Seed != 1)
            Debug.LogFormat("[Orange Arrows #{0}] Using rule seed {1}.", moduleId, rnd.Seed);

        current = 0;
        rotator = 0;
        if (stage == 2)
        {
            bulb1.material = colors[2];
            bulb2.material = colors[1];
        }
        else if (stage == 3)
        {
            bulb1.material = colors[2];
            bulb2.material = colors[2];
            bulb3.material = colors[1];
        }
        numDisplay.GetComponent<TextMesh>().text = " ";
        randomizeMoves();
        if (activated)
            co = StartCoroutine(makeFlashes(false));
    }

    void OnActivate()
    {
        bulb1.material = colors[1];
        co = StartCoroutine(makeFlashes(true));
        if (cbEnabled)
            colorblindText.SetActive(true);
        activated = true;
    }

    void PressButton(KMSelectable pressed)
    {
        if (moduleSolved != true && activated != false)
        {
            pressed.AddInteractionPunch(0.25f);
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
            if (pressed == buttons[0] && !movesEDIT[current].Equals("UP"))
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Orange Arrows #{0}] The button 'UP' was incorrect, expected '{1}'! Resetting sequence...", moduleId, movesEDIT[current]);
                StopCoroutine(co);
                Start();
            }
            else if (pressed == buttons[1] && !movesEDIT[current].Equals("DOWN"))
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Orange Arrows #{0}] The button 'DOWN' was incorrect, expected '{1}'! Resetting sequence...", moduleId, movesEDIT[current]);
                StopCoroutine(co);
                Start();
            }
            else if (pressed == buttons[2] && !movesEDIT[current].Equals("LEFT"))
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Orange Arrows #{0}] The button 'LEFT' was incorrect, expected '{1}'! Resetting sequence...", moduleId, movesEDIT[current]);
                StopCoroutine(co);
                Start();
            }
            else if (pressed == buttons[3] && !movesEDIT[current].Equals("RIGHT"))
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Orange Arrows #{0}] The button 'RIGHT' was incorrect, expected '{1}'! Resetting sequence...", moduleId, movesEDIT[current]);
                StopCoroutine(co);
                Start();
            }
            else
            {
                current++;
                StopCoroutine(co);
                numDisplay.GetComponent<TextMesh>().text = " ";
                if (stage == 3 && current == movesEDIT.Length)
                {
                    moduleSolved = true;
                    StartCoroutine(victory());
                }
                else if (current == movesEDIT.Length)
                {
                    stage++;
                    Start();
                }
            }
        }
    }

    private void randomizeMoves()
    {
        int rando = UnityEngine.Random.Range(5, 13);
        int counter = 0;
        moves = new string[rando];
        movesEDIT = new string[rando];
        for (int i = 0; i < rando; i++)
        {
            int random = UnityEngine.Random.Range(0, 4);
            if (counter == _ruleSeedCount - 1)
            {
                if (random == 0)
                {
                    moves[i] = "UP";
                    if (_ruleSeedDir == 0)
                        movesEDIT[i] = "DOWN";
                    else if (_ruleSeedDir == 1)
                        movesEDIT[i] = "RIGHT";
                    else if (_ruleSeedDir == 2)
                        movesEDIT[i] = "LEFT";
                    else
                        throw new InvalidOperationException("There was a bug in the ruleseed generation.");
                }
                else if (random == 1)
                {
                    moves[i] = "DOWN";
                    if (_ruleSeedDir == 0)
                        movesEDIT[i] = "UP";
                    else if (_ruleSeedDir == 1)
                        movesEDIT[i] = "LEFT";
                    else if (_ruleSeedDir == 2)
                        movesEDIT[i] = "RIGHT";
                    else
                        throw new InvalidOperationException("There was a bug in the ruleseed generation.");
                }
                else if (random == 2)
                {
                    moves[i] = "LEFT";
                    if (_ruleSeedDir == 0)
                        movesEDIT[i] = "RIGHT";
                    else if (_ruleSeedDir == 1)
                        movesEDIT[i] = "UP";
                    else if (_ruleSeedDir == 2)
                        movesEDIT[i] = "DOWN";
                    else
                        throw new InvalidOperationException("There was a bug in the ruleseed generation.");
                }
                else if (random == 3)
                {
                    moves[i] = "RIGHT";
                    if (_ruleSeedDir == 0)
                        movesEDIT[i] = "LEFT";
                    else if (_ruleSeedDir == 1)
                        movesEDIT[i] = "DOWN";
                    else if (_ruleSeedDir == 2)
                        movesEDIT[i] = "UP";
                    else
                        throw new InvalidOperationException("There was a bug in the ruleseed generation.");
                }
                counter = 0;
            }
            else
            {
                if (random == 0)
                {
                    moves[i] = "UP";
                    movesEDIT[i] = "UP";
                }
                else if (random == 1)
                {
                    moves[i] = "DOWN";
                    movesEDIT[i] = "DOWN";
                }
                else if (random == 2)
                {
                    moves[i] = "LEFT";
                    movesEDIT[i] = "LEFT";
                }
                else if (random == 3)
                {
                    moves[i] = "RIGHT";
                    movesEDIT[i] = "RIGHT";
                }
                counter++;
            }
        }
        string logger1 = "[Orange Arrows #{0}] The displayed sequence (#{1}) is";
        for (int j = 0; j < rando; j++)
        {
            logger1 += " '" + moves[j] + "'";
        }
        Debug.LogFormat(logger1, moduleId, stage);
        string logger2 = "[Orange Arrows #{0}] The inputs for sequence (#{1}) should be";
        for (int k = 0; k < rando; k++)
        {
            logger2 += " '" + movesEDIT[k] + "'";
        }
        Debug.LogFormat(logger2, moduleId, stage);
    }

    private IEnumerator makeFlashes(bool shortDelay)
    {
        yield return null;
        if (shortDelay)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(2.0f);
        rotator = 0;
        while (rotator < movesEDIT.Length)
        {
            if (moves[rotator].Equals("UP"))
            {
                numDisplay.GetComponent<TextMesh>().text = "▲";
            }
            else if (moves[rotator].Equals("DOWN"))
            {
                numDisplay.GetComponent<TextMesh>().text = "▼";
            }
            else if (moves[rotator].Equals("LEFT"))
            {
                numDisplay.GetComponent<TextMesh>().text = "◄";
            }
            else if (moves[rotator].Equals("RIGHT"))
            {
                numDisplay.GetComponent<TextMesh>().text = "►";
            }
            yield return new WaitForSeconds(0.75f);
            numDisplay.GetComponent<TextMesh>().text = " ";
            yield return new WaitForSeconds(0.1f);
            rotator++;
        }
        StopCoroutine(co);
        co = StartCoroutine(makeFlashes(false));
    }

    private IEnumerator victory()
    {
        yield return null;
        numDisplay.GetComponent<TextMesh>().fontSize = 160;
        for (int i = 0; i < 100; i++)
        {
            int rand1 = UnityEngine.Random.Range(0, 10);
            if (i < 50)
            {
                numDisplay.GetComponent<TextMesh>().text = rand1 + "";
            }
            else
            {
                numDisplay.GetComponent<TextMesh>().text = "G" + rand1;
            }
            yield return new WaitForSeconds(0.025f);
        }
        numDisplay.GetComponent<TextMesh>().text = "GG";
        bulb3.material = colors[2];
        Debug.LogFormat("[Orange Arrows #{0}] All sequences were correct! Module Disarmed!", moduleId);
        trueModuleSolved = true;
        GetComponent<KMBombModule>().HandlePass();
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} left right down up [Presses the corresponding arrow buttons. The ENTIRE sequence must be entered in one command.] | Direction words can be substituted as one letter (Ex. right as r)";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        var buttonsToPress = new List<KMSelectable>();
        if (parameters.Length == movesEDIT.Length)
        {
            foreach (string param in parameters)
            {
                if (param.EqualsIgnoreCase("up") || param.EqualsIgnoreCase("u"))
                    buttonsToPress.Add(buttons[0]);
                else if (param.EqualsIgnoreCase("down") || param.EqualsIgnoreCase("d"))
                    buttonsToPress.Add(buttons[1]);
                else if (param.EqualsIgnoreCase("left") || param.EqualsIgnoreCase("l"))
                    buttonsToPress.Add(buttons[2]);
                else if (param.EqualsIgnoreCase("right") || param.EqualsIgnoreCase("r"))
                    buttonsToPress.Add(buttons[3]);
                else
                    yield break;
            }
        }
        else
        {
            yield return null;
            yield return "sendtochaterror Please include the entire sequence of arrows to press!";
            yield break;
        }

        yield return null;
        yield return buttonsToPress;
        if (moduleSolved) { yield return "solve"; }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        while (!activated)
            yield return true;
        while (!moduleSolved)
        {
            buttons[Array.IndexOf(new string[] { "UP", "DOWN", "LEFT", "RIGHT" }, movesEDIT[current])].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        while (!trueModuleSolved)
            yield return true;
    }
}