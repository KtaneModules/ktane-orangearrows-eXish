﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using KModkit;
using System;

public class GreenArrowsScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombInfo bomb;
    public KMColorblindMode Colorblind;

    public KMSelectable[] buttons;
    public GameObject numDisplay;
    public GameObject colorblindText;

    private int streak = 0;
    private string nextMove;

    private bool isanimating = true;
    private bool colorblindActive = false;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        foreach(KMSelectable obj in buttons){
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
    }

    void Start () {
        numDisplay.GetComponent<TextMesh>().text = " ";
        StartCoroutine(generateNewNum());
        StartCoroutine(colorblindDelay());
    }

    void PressButton(KMSelectable pressed)
    {
        if(moduleSolved != true && isanimating != true)
        {
            pressed.AddInteractionPunch(0.25f);
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if (nextMove.Equals("UP") && pressed != buttons[0])
            {
                GetComponent<KMBombModule>().HandleStrike();
                StartCoroutine(removeNum());
                streak = 0;
                Debug.LogFormat("[Green Arrows #{0}] 'UP' was not pressed and was expected! Streak set back to 0!", moduleId);
            }
            else if (nextMove.Equals("DOWN") && pressed != buttons[1])
            {
                GetComponent<KMBombModule>().HandleStrike();
                StartCoroutine(removeNum());
                streak = 0;
                Debug.LogFormat("[Green Arrows #{0}] 'DOWN' was not pressed and was expected! Streak set back to 0!", moduleId);
            }
            else if (nextMove.Equals("LEFT") && pressed != buttons[2])
            {
                GetComponent<KMBombModule>().HandleStrike();
                StartCoroutine(removeNum());
                streak = 0;
                Debug.LogFormat("[Green Arrows #{0}] 'LEFT' was not pressed and was expected! Streak set back to 0!", moduleId);
            }
            else if (nextMove.Equals("RIGHT") && pressed != buttons[3])
            {
                GetComponent<KMBombModule>().HandleStrike();
                StartCoroutine(removeNum());
                streak = 0;
                Debug.LogFormat("[Green Arrows #{0}] 'RIGHT' was not pressed and was expected! Streak set back to 0!", moduleId);
            }
            else
            {
                streak++;
                if (streak == 7)
                {
                    moduleSolved = true;
                    StartCoroutine(victory());
                    Debug.LogFormat("[Green Arrows #{0}] '{1}' pressed successfully! Streak is now {2}!", moduleId, nextMove, streak);
                    Debug.LogFormat("[Green Arrows #{0}] Streak of 7 reached! Module Disarmed!", moduleId);
                }
                else
                {
                    StartCoroutine(removeNum());
                    Debug.LogFormat("[Green Arrows #{0}] '{1}' pressed successfully! Streak is now {2}!", moduleId, nextMove, streak);
                }
            }
        }
    }

    private IEnumerator generateNewNum()
    {
        int rando = UnityEngine.Random.RandomRange(0,100);
        if(rando <= 9)
        {
            yield return new WaitForSeconds(0.5f);
            numDisplay.GetComponent<TextMesh>().text = "0";
            yield return new WaitForSeconds(0.5f);
            numDisplay.GetComponent<TextMesh>().text += rando;
        }
        else
        {
            string num = ""+rando;
            yield return new WaitForSeconds(0.5f);
            numDisplay.GetComponent<TextMesh>().text = num.Substring(0,1);
            yield return new WaitForSeconds(0.5f);
            numDisplay.GetComponent<TextMesh>().text += num.Substring(1,1);
        }
        getNextMove(rando);
        isanimating = false;
        StopCoroutine("generateNewNum");
    }

    private IEnumerator removeNum()
    {
        isanimating = true;
        yield return new WaitForSeconds(0.5f);
        string num = numDisplay.GetComponent<TextMesh>().text;
        numDisplay.GetComponent<TextMesh>().text = num.Substring(0,1);
        yield return new WaitForSeconds(0.5f);
        numDisplay.GetComponent<TextMesh>().text = " ";
        StopCoroutine("removeNum");
        Start();
    }

    private IEnumerator colorblindDelay()
    {
        yield return new WaitForSeconds(0.5f);
        colorblindActive = Colorblind.ColorblindModeActive;
        if (colorblindActive)
        {
            Debug.LogFormat("[Green Arrows #{0}] Colorblind mode is active!", moduleId);
            colorblindText.SetActive(true);
        }
    }

    private void getNextMove(int i)
    {
        if(i == 10 || i == 50 || i == 90 || i == 50 || i == 39 || i == 79 || i == 28 || i == 8 || i == 17 || i == 37 || i == 57 || i == 97 || i == 86 || i == 25 || i == 45 || i == 65 || i == 5 || i == 14 || i == 44 || i == 74 || i == 94 || i == 33 || i == 53 || i == 83 || i == 22 || i == 42 || i == 62 || i == 72 || i == 2 || i == 11 || i == 81)
        {
            nextMove = "UP";
        }else if (i == 20 || i == 40 || i == 60 || i == 80 || i == 29 || i == 9 || i == 38 || i == 58 || i == 78 || i == 67 || i == 87 || i == 26 || i == 46 || i == 6 || i == 34 || i == 54 || i == 23 || i == 43 || i == 63 || i == 73 || i == 3 || i == 82 || i == 31 || i == 71)
        {
            nextMove = "RIGHT";
        }else if (i == 30 || i == 70 || i == 19 || i == 59 || i == 99 || i == 48 || i == 88 || i == 77 || i == 16 || i == 36 || i == 56 || i == 96 || i == 75 || i == 84 || i == 13 || i == 93 || i == 41 || i == 61 || i == 1)
        {
            nextMove = "LEFT";
        }
        else
        {
            nextMove = "DOWN";
        }
        Debug.LogFormat("[Green Arrows #{0}] The number displayed is {1}, the next move should be '{2}'", moduleId, i, nextMove);
    }

    private IEnumerator victory()
    {
        isanimating = true;
        for(int i = 0; i < 100; i++)
        {
            int rand1 = UnityEngine.Random.RandomRange(0, 10);
            int rand2 = UnityEngine.Random.RandomRange(0, 10);
            if (i < 50)
            {
                numDisplay.GetComponent<TextMesh>().text = rand1 + "" + rand2;
            }
            else
            {
                numDisplay.GetComponent<TextMesh>().text = "G" + rand2;
            }
            yield return new WaitForSeconds(0.025f);
        }
        numDisplay.GetComponent<TextMesh>().text = "GG";
        isanimating = false;
        GetComponent<KMBombModule>().HandlePass();
        StopCoroutine("victory");
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} up/right/down/left [Presses the specified arrow button] | Words can be substituted as one letter (Ex. right as r)";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*up\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*u\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            buttons[0].OnInteract();
        }
        if (Regex.IsMatch(command, @"^\s*down\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*d\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            buttons[1].OnInteract();
        }
        if (Regex.IsMatch(command, @"^\s*left\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*l\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            buttons[2].OnInteract();
        }
        if (Regex.IsMatch(command, @"^\s*right\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*r\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            buttons[3].OnInteract();
        }
        if (moduleSolved) { yield return "solve"; }
        yield break;
        /**string[] parameters = command.Split(' ');
        var buttonsToPress = new List<KMSelectable>();
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

        yield return null;
        yield return buttonsToPress;*/
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while(streak <= 6)
        {
            while (isanimating) { yield return true; yield return new WaitForSeconds(0.1f); };
            if (nextMove.Equals("UP"))
            {
                yield return ProcessTwitchCommand("up");
            }
            else if (nextMove.Equals("DOWN"))
            {
                yield return ProcessTwitchCommand("down");
            }
            else if (nextMove.Equals("LEFT"))
            {
                yield return ProcessTwitchCommand("left");
            }
            else if (nextMove.Equals("RIGHT"))
            {
                yield return ProcessTwitchCommand("right");
            }
            if(streak == 7)
            {
                break;
            }
        }
        while (isanimating) { yield return true; yield return new WaitForSeconds(0.1f); };
    }
}
