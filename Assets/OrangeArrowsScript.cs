using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System;

public class OrangeArrowsScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombInfo bomb;

    public KMSelectable[] buttons;
    public GameObject numDisplay;

    public Renderer bulb1;
    public Renderer bulb2;
    public Renderer bulb3;

    public Material[] colors;

    private string[] moves;
    private string[] movesEDIT;
    private int current;

    private int rotator;

    private int stage;

    private Coroutine co;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        stage = 1;
        current = 0;
        bulb1.material = colors[1];
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        foreach(KMSelectable obj in buttons){
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
    }

    void Start () {
        current = 0;
        rotator = 0;
        if (stage == 2)
        {
            bulb1.material = colors[2];
            bulb2.material = colors[1];
        }else if(stage == 3)
        {
            bulb1.material = colors[2];
            bulb2.material = colors[2];
            bulb3.material = colors[1];
        }
        numDisplay.GetComponent<TextMesh>().text = " ";
        randomizeMoves();
        co = StartCoroutine(makeFlashes());
    }

    void PressButton(KMSelectable pressed)
    {
        if(moduleSolved != true)
        {
            pressed.AddInteractionPunch(0.25f);
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if(pressed == buttons[0] && !movesEDIT[current].Equals("UP"))
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Orange Arrows #{0}] The button 'UP' was incorrect, expected '{1}'! Resetting sequence...", moduleId, moves[current]);
                StopCoroutine(co);
                Start();
            }
            else if (pressed == buttons[1] && !movesEDIT[current].Equals("DOWN"))
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Orange Arrows #{0}] The button 'DOWN' was incorrect, expected '{1}'! Resetting sequence...", moduleId, moves[current]);
                StopCoroutine(co);
                Start();
            }
            else if (pressed == buttons[2] && !movesEDIT[current].Equals("LEFT"))
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Orange Arrows #{0}] The button 'LEFT' was incorrect, expected '{1}'! Resetting sequence...", moduleId, moves[current]);
                StopCoroutine(co);
                Start();
            }
            else if (pressed == buttons[3] && !movesEDIT[current].Equals("RIGHT"))
            {
                GetComponent<KMBombModule>().HandleStrike();
                Debug.LogFormat("[Orange Arrows #{0}] The button 'RIGHT' was incorrect, expected '{1}'! Resetting sequence...", moduleId, moves[current]);
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
        int rando = UnityEngine.Random.RandomRange(5,13);
        int random = 0;
        int counter = 0;
        moves = new string[rando];
        movesEDIT = new string[rando];
        for(int i = 0; i < rando; i++)
        {
            random = UnityEngine.Random.RandomRange(0, 4);
            if(counter == 3)
            {
                if (random == 0)
                {
                    moves[i] = "UP";
                    movesEDIT[i] = "DOWN";
                }
                else if (random == 1)
                {
                    moves[i] = "DOWN";
                    movesEDIT[i] = "UP";
                }
                else if (random == 2)
                {
                    moves[i] = "LEFT";
                    movesEDIT[i] = "RIGHT";
                }
                else if (random == 3)
                {
                    moves[i] = "RIGHT";
                    movesEDIT[i] = "LEFT";
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
        for(int j = 0; j < rando; j++)
        {
            logger1 += " '"+moves[j]+"'";
        }
        Debug.LogFormat(logger1, moduleId, stage);
        string logger2 = "[Orange Arrows #{0}] The inputs for sequence (#{1}) should be";
        for (int k = 0; k < rando; k++)
        {
            logger2 += " '" + movesEDIT[k] + "'";
        }
        Debug.LogFormat(logger2, moduleId, stage);
    }

    private IEnumerator makeFlashes()
    {
        yield return null;
        yield return new WaitForSeconds(2.0f);
        rotator = 0;
        while(rotator < movesEDIT.Length)
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
        co = StartCoroutine(makeFlashes());
    }

    private IEnumerator victory()
    {
        yield return null;
        numDisplay.GetComponent<TextMesh>().fontSize = 160;
        for (int i = 0; i < 100; i++)
        {
            int rand1 = UnityEngine.Random.RandomRange(0, 10);
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
        StopCoroutine("victory");
        bulb3.material = colors[2];
        Debug.LogFormat("[Orange Arrows #{0}] All Sequences were correct! Module Disarmed!", moduleId);
        GetComponent<KMBombModule>().HandlePass();
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} left right down up [Presses the corresponding arrow buttons. The ENTIRE sequence must be entered in one command or else an error will occur.] | Direction words can be substituted as one letter (Ex. right as r)";
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
            yield return "sendtochaterror Error - Please include the entire sequence of arrows to press!";
            yield break;
        }

        yield return null;
        yield return buttonsToPress;
        if (moduleSolved) { yield return "solve"; }
    }
}
