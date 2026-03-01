using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System;
using Random = UnityEngine.Random;
using System.Text.RegularExpressions;
using Metapuzzle;

public class metapuzzleScript : MonoBehaviour
{

    // Module stuff
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMAudio Audio;
    public KMRuleSeedable Ruleseed;
    // Answer button stuff
    public GameObject answerObject;
    public KMSelectable answerSelectable, leftSelectable, rightSelectable;
    public MeshRenderer answerRenderer;
    public TextMesh answerText;
    public Material[] borderMats;
    // Submission stuff
    public GameObject submissionObject;
    public KMSelectable[] submitKeyboardSelectables;
    public KMSelectable clearSelectable;
    public TextMesh answerSubmissionText;
    // Encoding quiz stuff
    public GameObject encodingQuiz;
    public GameObject morseObject, brailleObject, semaphoreObject;
    public MeshRenderer[] morseLEDRenderers;
    public MeshRenderer[] brailleBumpRenderers;
    public GameObject[] semaphoreHandObjects;
    public TextMesh[] passwordTexts;
    public KMSelectable[] passwordButtons;
    public KMSelectable encodingQuizSubmitSelectable;
    public Material[] morseMats, brailleMats;
    // Hangman stuff
    public GameObject hangman;
    public GameObject[] gallowsObjects;
    public TextMesh blanksText;
    public TextMesh guessesText;
    public KMSelectable[] keyboardSelectables;
    public MeshRenderer[] keyboardRenderers;
    public Material[] keyboardMats;
    // Mental math stuff
    public GameObject mentalMath;
    public TextMesh startingNumText;
    public TextMesh[] operatorTexts;
    public TextMesh submissionText;
    public KMSelectable[] numberKeypadSelectables;
    // Nonogram stuff
    public GameObject nonogram;
    public TextMesh[] rowClues, colClues;
    public KMSelectable[] cellSelectables;
    public KMSelectable fillSelectable, checkSelectable, clearNonogramSelectable;
    public MeshRenderer[] cellRenderers;
    public Material[] nonogramMats;
    public TextMesh fillText;
    public TextMesh[] nonogramTexts;
    // Sorting stuff
    public GameObject sorting;
    public KMSelectable[] sortingButtons;
    public SpriteRenderer[] iconRenderers;
    public MeshRenderer[] sortingBtnRenderers;
    public Material[] sortingMats;
    // Spelling bee stuff
    public GameObject spellingBee;
    public TextMesh[] hexagonTexts;
    public TextMesh spellingBeeWordText;
    public KMSelectable[] hexagonSelectables;
    public KMSelectable spellingBeeSubmitSelectable;
    public MeshRenderer[] spellingBeeLEDRenderers;
    public Material[] spellingBeeMats;
    // Spot the difference stuff
    public GameObject spotTheDifference;
    public KMSelectable[] gridSelectables1, gridSelectables2;
    public MeshRenderer[] gridRenderers1, gridRenderers2;
    public TextMesh[] gridText1, gridText2;
    public Material[] spotTheDifferenceMats;
    public Color[] spotTheDifferenceColors;
    public TextMesh timerText, progressText;

    // Normal variables

    // Overall stuff
    static int _moduleIdCounter = 1;
    int _moduleId;
    bool solved;
    bool animationPlaying = false;
    bool _readyToPress = false;

    // Metapuzzle generation
    int wordListLength;
    int sortingMethod = 0, extractionMethod = 0;
    static readonly string[] extractionNames = { "first letters", "double letters", "triple letters", "last letters", "diagonalization", "reverse diagonalization", "sandwich inner", "sandwich outer" };
    static readonly string[] sortingNames = { "first letters", "last letters", "reverse first letters", "reverse last letters", "given order", "reverse given order", "ascending length order", "descending length order" };
    bool metaSuccessful = false;
    string metaAnswer = "";
    string[] feederAnswers = { "", "", "", "", "", "", "" };
    static readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    static readonly char[] badFirstLetters = "RST".ToCharArray();
    static readonly char[] badLastLetters = "DEFGHIJKLMNOPQ".ToCharArray();

    // Ruleseed generation
    int[] sortRuleOrder = { 0, 1, 2, 3, 4, 5, 6, 7 };
    int[] extractRuleOrder = { 0, 1, 2, 3, 4, 5, 6, 7 };
    int inversionMethod = 0;

    // For displaying things
    int shownPuzzle = 0;
    Subpuzzle[] puzzleOrder = { Subpuzzle.EncodingQuiz, Subpuzzle.Hangman, Subpuzzle.MentalMath, Subpuzzle.Nonogram, Subpuzzle.Sorting, Subpuzzle.SpellingBee, Subpuzzle.SpotTheDifference };
    static readonly string[] puzzleNames = { "Encoding Quiz", "Hangman", "Mental Math", "Nonogram", "Sorting", "Spelling Bee", "Spot the Difference" };
    readonly bool[] solvedPuzzles = { false, false, false, false, false, false, false };

    bool inSubmissionMode = false;

    // Encoding quiz variables
    string encodedWord = "PLACEHOLDER";
    int encodingMethod = 0;
    readonly char[][] passwordOptions = { "12345678".ToCharArray(), "12345678".ToCharArray(), "12345678".ToCharArray(), "12345678".ToCharArray(), "12345678".ToCharArray() };

    static readonly string[] encodingNames = { "Morse code", "Braille", "flag semaphore" };
    static readonly string[] morseAlphabet = { ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---", "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-", "..-", "...-", ".--", "-..-", "-.--", "--.." };
    static readonly string[] brailleAlphabet = { ".-----", "..----", ".--.--", ".--..-", ".---.-", "..-.--", "..-..-", "..--.-", "-.-.--", "-.-..-", ".-.---", "...---", ".-..--", ".-...-", ".-.-.-", "....--", ".....-", "...-.-", "-...--", "-....-", ".-.--.", "...--.", "-.-...", ".-..-.", ".-....", ".-.-.." };
    static readonly int[] semaphoreAlphabet = { 4, 5, 4, 6, 4, 7, 4, 0, 4, 1, 4, 2, 4, 3, 5, 6, 5, 7, 0, 2, 5, 0, 5, 1, 5, 2, 5, 3, 6, 7, 6, 0, 6, 1, 6, 2, 6, 3, 7, 0, 7, 1, 0, 3, 1, 2, 1, 3, 7, 2, 2, 3 };

    readonly int[] passwordIndices = { 0, 0, 0, 0, 0 };

    // Hangman variables
    string[] validModuleNames;
    string hangmanSolution;
    char[] hangmanArray;
    string hangmanState;

    int guesses = 0;

    const string qwerty = "QWERTYUIOPASDFGHJKLZXCVBNM";
    bool[] guessedLetters = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
    string guessedLettersButActuallyLetters = "";

    // Mental math variables
    int startingNum, answerNum, operatorNum, operatorType;
    /*  0: add
     *  1: subtract
     *  2: multiply
     *  3: divide. if not divisible, set up for division next step */
    bool savedOperator = false;
    string[] operatorStrings = new string[7];
    int[] submittedDigits = { -1, -1 };

    // Nonogram variables
    bool[] nonogramSolution = new bool[36];
    bool[] nonogramState = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
    bool[] nonogramMarked = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
    int currentStreak = 0;
    string nonogramText = "";
    bool filling = true;
    static readonly string[][] bazinga = {
        new string[1] { "------" }, // 0
        new string[6] { "X-----", "-X----", "--X---", "---X--", "----X-", "-----X" }, // 1
        new string[5] { "XX----", "-XX---", "--XX--", "---XX-", "----XX" }, // 2
        new string[4] { "XXX---", "-XXX--", "--XXX-", "---XXX" }, // 3
        new string[3] { "XXXX--", "-XXXX-", "--XXXX" }, // 4
        new string[2] { "XXXXX-", "-XXXXX" }, // 5
        new string[1] { "XXXXXX" }, // 6
        new string[10] { "X-X---", "X--X--", "X---X-", "X----X", "-X-X--", "-X--X-", "-X---X", "--X-X-", "--X--X", "---X-X" }, // 1 1
        new string[6] { "X-XX--", "X--XX-", "X---XX", "-X-XX-", "-X--XX", "--X-XX" }, // 1 2
        new string[3] { "X-XXX-", "X--XXX", "-X-XXX" }, // 1 3
        new string[1] { "X-XXXX" }, // 1 4
        new string[6] { "XX-X--", "XX--X-", "XX---X", "-XX-X-", "-XX--X", "--XX-X" }, // 2 1
        new string[3] { "XX-XX-", "XX--XX", "-XX-XX" }, // 2 2
        new string[1] { "XX-XXX" }, // 2 3
        new string[3] { "XXX-X-", "XXX--X", "-XXX-X" }, // 3 1
        new string[1] { "XXX-XX" }, // 3 2
        new string[1] { "XXXX-X" }, // 4 1
        new string[4] { "X-X-X-", "X-X--X", "X--X-X", "-X-X-X" }, // 1 1 1
        new string[1] { "X-X-XX" }, // 1 1 2
        new string[1] { "X-XX-X" }, // 1 2 1
        new string[1] { "XX-X-X" }, // 2 1 1
    };
    static readonly string[] possibleClues = { "0", "1", "2", "3", "4", "5", "6", "1 1", "1 2", "1 3", "1 4", "2 1", "2 2", "2 3", "3 1", "3 2", "4 1", "1 1 1", "1 1 2", "1 2 1", "2 1 1" };
    int solutionCount = 0;

    // Sorting variables
    int[] sortingIndices = { -1, -1, -1, -1 };
    string[] sortingIconNames = { "", "", "", "" };
    bool[] pressedBtns = { false, false, false, false };
    string[] sortedOrder;
    int[] pressOrder = { 0, 0, 0, 0 };

    private static T[] NewArray<T>(params T[] array) { return array; }

    // Spelling bee variables
    string seedWord = "";
    char[] spellingBeeLetters;
    List<string> submittedWords = new List<string>();
    string currentWord = "";
    int currentPoints = 0;

    // Spot the difference variables
    int[] bgColors = new int[25];
    int[] symbolColors = new int[25];
    int[] symbols = new int[25];
    int[] impostors = new int[3];
    static readonly string[] actualSymbols = { "+", "-", "×", "•" };
    static readonly string[] colorNames = { "red", "yellow", "green", "blue" };
    bool[] pressedSTDBtns = { false, false, false };
    string[] progressStrings = { "", "✓  ", "✓✓ ", "✓✓✓" };
    bool stdTimerStarted = false;

    // Submission variables
    string submittedAnswer = "";
    int submittedCount = 0;
    static readonly string[] victoryAnswers = { "C O N G R A T", "V I C T O R Y", "A M A Z I N G", "A W E S O M E", "G O O D J O B" };
    static readonly string[] strikeAnswers = { "N O T G O O D", "I N V A L I D", "G O A G A I N", "S T R I K E S", "M I S T A K E" };

    private void Awake()
    {
        _moduleId = _moduleIdCounter++;
        // Answer button selectables
        leftSelectable.OnInteract += delegate ()
        {
            if (!_readyToPress)
                return false;
            leftSelectable.AddInteractionPunch();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
            if (solved)
                return false;
            CycleAnswer(-1);
            return false;
        };
        rightSelectable.OnInteract += delegate ()
        {
            if (!_readyToPress)
                return false;
            leftSelectable.AddInteractionPunch();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
            if (solved)
                return false;
            CycleAnswer(1);
            return false;
        };
        answerSelectable.OnInteract += delegate ()
        {
            if (!_readyToPress)
                return false;
            leftSelectable.AddInteractionPunch();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, Module.transform);
            if (solved)
                return false;
            ToggleSubmission();
            return false;
        };

        // Encoding quiz selectables
        for (int i = 0; i < 5; i++)
        {
            int j = i;
            passwordButtons[i].OnInteract += delegate ()
            {
                if (!_readyToPress)
                    return false;
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, Module.transform);
                if (solved)
                    return false;
                CycleLetter(j);
                return false;
            };
        }
        encodingQuizSubmitSelectable.OnInteract += delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
            if (solved)
                return false;
            if (!_readyToPress)
                return false;
            SubmitEncodingQuiz();
            return false;
        };
        // Hangman selectables
        for (int i = 0; i < 26; i++)
        {
            int j = i;
            keyboardSelectables[i].OnInteract += delegate ()
            {
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, Module.transform);
                if (solved)
                    return false;
                if (!_readyToPress)
                    return false;
                Guess(j);
                return false;
            };
        }
        // Mental math selectables
        for (int i = 0; i < 10; i++)
        {
            int j = i;
            numberKeypadSelectables[i].OnInteract += delegate ()
            {
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
                if (solved)
                    return false;
                if (!_readyToPress)
                    return false;
                EnterMath(j);
                return false;
            };
        }
        // Nonogram selectables
        fillSelectable.OnInteract += delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
            if (solved)
                return false;
            if (!_readyToPress)
                return false;
            Toggle();
            return false;
        };
        checkSelectable.OnInteract += delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
            if (solved)
                return false;
            if (!_readyToPress)
                return false;
            CheckNonogram();
            return false;
        };
        clearNonogramSelectable.OnInteract += delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
            if (solved)
                return false;
            if (!_readyToPress)
                return false;
            ClearNonogram();
            return false;
        };
        for (int i = 0; i < 36; i++)
        {
            int j = i;
            cellSelectables[i].OnInteract += delegate ()
            {
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, Module.transform);
                if (solved)
                    return false;
                if (!_readyToPress)
                    return false;
                ClickCell(j);
                return false;
            };
        }
        // Sorting selectables
        for (int i = 0; i < 4; i++)
        {
            int j = i;
            sortingButtons[i].OnInteract += delegate ()
            {
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
                if (solved)
                    return false;
                if (!_readyToPress)
                    return false;
                PressIcon(j);
                return false;
            };
        }
        // Spelling bee selectables
        for (int i = 0; i < 7; i++)
        {
            int j = i;
            hexagonSelectables[i].OnInteract += delegate ()
            {
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, Module.transform);
                if (solved)
                    return false;
                if (!_readyToPress)
                    return false;
                LetterPress(j);
                return false;
            };
        }
        spellingBeeSubmitSelectable.OnInteract += delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
            if (solved)
                return false;
            if (!_readyToPress)
                return false;
            SubmitSpellingBee();
            return false;
        };
        // Spot the difference selectables
        for (int i = 0; i < 25; i++)
        {
            int j = i;
            gridSelectables1[i].OnInteract += delegate ()
            {
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
                if (solved)
                    return false;
                if (!_readyToPress)
                    return false;
                PressButton(j);
                return false;
            };
            gridSelectables2[i].OnInteract += delegate ()
            {
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
                if (solved)
                    return false;
                if (!_readyToPress)
                    return false;
                PressButton(j);
                return false;
            };
        }
        // Submission selectables
        for (int i = 0; i < 26; i++)
        {
            int j = i;
            submitKeyboardSelectables[i].OnInteract += delegate ()
            {
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, Module.transform);
                if (solved)
                    return false;
                if (!_readyToPress)
                    return false;
                AnswerKeyboard(j);
                return false;
            };
        }
        clearSelectable.OnInteract += delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
            if (solved)
                return false;
            if (!_readyToPress)
                return false;
            ClearAnswer();
            return false;
        };
    }

    void Start()
    {
        submissionObject.SetActive(false);
        shownPuzzle = Random.Range(0, 7);
        puzzleOrder = puzzleOrder.Shuffle();
        UpdateDisplay();

        // Meta generation
        char[] evenNumbers = "02468".ToCharArray();
        char[] firstHalfAlphabet = "ABCDEFGHIJKLM".ToCharArray();
        if (alphabet.Contains(Info.GetSerialNumber().ElementAt(0)))
            extractionMethod += 4;
        if (alphabet.Contains(Info.GetSerialNumber().ElementAt(1)))
            sortingMethod += 4;
        if (evenNumbers.Contains(Info.GetSerialNumber().ElementAt(2)))
            extractionMethod += 2;
        if (firstHalfAlphabet.Contains(Info.GetSerialNumber().ElementAt(3)))
            sortingMethod += 2;
        if (firstHalfAlphabet.Contains(Info.GetSerialNumber().ElementAt(4)))
            extractionMethod += 1;
        if (evenNumbers.Contains(Info.GetSerialNumber().ElementAt(5)))
            sortingMethod += 1;


        bool inverted = false;
        if (extractRuleOrder[extractionMethod] == 0 && (sortRuleOrder[sortingMethod] == 0 || sortRuleOrder[sortingMethod] == 2))
        {
            inverted = true;
            DebugMsg("1");
        }
        else if (extractRuleOrder[extractionMethod] == 3 && (sortRuleOrder[sortingMethod] == 1 || sortRuleOrder[sortingMethod] == 3))
        {
            inverted = true;
            DebugMsg("2");
        }
        else if ((extractRuleOrder[extractionMethod] == 4 || extractionMethod == 5) && (sortRuleOrder[sortingMethod] == 4 || sortRuleOrder[sortingMethod] == 5))
        {
            inverted = true;
            DebugMsg("3");
        }
        if (inverted)
            switch (inversionMethod)
            {
                case 0:
                    sortingMethod = (sortingMethod + 4) % 8;
                    break;
                case 1:
                    sortingMethod = (sortingMethod + 2) % 4 + sortingMethod / 4 * 4;
                    break;
                case 2:
                    sortingMethod = (sortingMethod + 1) % 2 + sortingMethod / 2 * 2;
                    break;
                case 3:
                    extractionMethod = (extractionMethod + 4) % 8;
                    break;
                case 4:
                    extractionMethod = (extractionMethod + 2) % 4 + extractionMethod / 4 * 4;
                    break;
                case 5:
                    extractionMethod = (extractionMethod + 1) % 2 + extractionMethod / 2 * 2;
                    break;
            }

        sortingMethod = sortRuleOrder[sortingMethod];
        extractionMethod = extractRuleOrder[extractionMethod];

        DebugMsg("The extraction method is " + extractionNames[extractionMethod] + " and the sorting method is " + sortingNames[sortingMethod] + ".");

        while (!metaSuccessful)
        {
            DebugMsg("Attempting to generate a meta...");
            metaAnswer = SevenLetterWords.List.PickRandom();
            while ((sortingMethod == 0 && extractionMethod == 4 && badFirstLetters.Contains(metaAnswer[0])) || (sortingMethod == 1 && extractionMethod == 5 && badLastLetters.Contains(metaAnswer[0])) || (sortingMethod == 2 && extractionMethod == 4 && badFirstLetters.Contains(metaAnswer[6])) || (sortingMethod == 3 && extractionMethod == 5 && badLastLetters.Contains(metaAnswer[6])) || !MainWordList.List.Contains(metaAnswer))
                metaAnswer = SevenLetterWords.List.PickRandom();
            metaSuccessful = true;

            switch (sortingMethod)
            {
                case 0: // first letters
                    int[] randomLetter1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                    randomLetter1 = randomLetter1.Shuffle();
                    for (int j = 0; j < randomLetter1.Length; j++)
                    {
                        metaSuccessful = false;
                        for (int i = 0; i < 7; i++)
                        {
                            string[] placeholder = wordsThatExtractThisLetter(metaAnswer[i], i, extractionMethod);
                            string[] placeholder2 = wordsThatExtractThisLetter(alphabet[randomLetter1[j] + i], i, 0);
                            string[] placeholder3 = placeholder.Intersect(placeholder2).ToArray();
                            if (placeholder3.Length <= 0) { break; }
                            feederAnswers[i] = placeholder3.PickRandom();
                            if (!MainWordList.List.Contains(feederAnswers[i]))
                                feederAnswers[i] = placeholder3.PickRandom();

                            if (i == 6)
                                metaSuccessful = true;
                        }
                        if (metaSuccessful)
                        {
                            feederAnswers = feederAnswers.Shuffle();
                            break;
                        }
                    }
                    break;
                case 1: // last letters
                    int[] randomLetter2 = { 0, 1, 2, 14, 15, 16 };
                    randomLetter2 = randomLetter2.Shuffle();
                    for (int j = 0; j < randomLetter2.Length; j++)
                    {
                        metaSuccessful = false;
                        for (int i = 0; i < 7; i++)
                        {
                            string[] placeholder = wordsThatExtractThisLetter(metaAnswer[i], i, extractionMethod);
                            string[] placeholder2 = wordsThatExtractThisLetter(alphabet[randomLetter2[j] + i], i, 3);
                            string[] placeholder3 = placeholder.Intersect(placeholder2).ToArray();
                            if (placeholder3.Length <= 0) { break; }
                            feederAnswers[i] = placeholder3.PickRandom();
                            if (!MainWordList.List.Contains(feederAnswers[i]))
                                feederAnswers[i] = placeholder3.PickRandom();

                            if (i == 6)
                                metaSuccessful = true;
                        }
                        if (metaSuccessful)
                        {
                            feederAnswers = feederAnswers.Shuffle();
                            break;
                        }
                    }
                    break;
                case 2: // reverse first letters
                    int[] randomLetter3 = { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22 };
                    randomLetter3 = randomLetter3.Shuffle();
                    for (int j = 0; j < randomLetter3.Length; j++)
                    {
                        metaSuccessful = false;
                        for (int i = 0; i < 7; i++)
                        {
                            string[] placeholder = wordsThatExtractThisLetter(metaAnswer[i], i, extractionMethod);
                            string[] placeholder2 = wordsThatExtractThisLetter(alphabet[randomLetter3[j] - i], i, 0);
                            string[] placeholder3 = placeholder.Intersect(placeholder2).ToArray();
                            if (placeholder3.Length <= 0) { break; }
                            feederAnswers[i] = placeholder3.PickRandom();
                            if (!MainWordList.List.Contains(feederAnswers[i]))
                                feederAnswers[i] = placeholder3.PickRandom();

                            if (i == 6)
                                metaSuccessful = true;
                        }
                        if (metaSuccessful)
                        {
                            feederAnswers = feederAnswers.Shuffle();
                            break;
                        }
                    }
                    break;
                case 3: // reverse last letters
                    int[] randomLetter4 = { 6, 7, 8 };
                    randomLetter4 = randomLetter4.Shuffle();
                    for (int j = 0; j < randomLetter4.Length; j++)
                    {
                        metaSuccessful = false;
                        for (int i = 0; i < 7; i++)
                        {
                            string[] placeholder = wordsThatExtractThisLetter(metaAnswer[i], i, extractionMethod);
                            string[] placeholder2 = wordsThatExtractThisLetter(alphabet[randomLetter4[j] - i], i, 3);
                            string[] placeholder3 = placeholder.Intersect(placeholder2).ToArray();
                            if (placeholder3.Length <= 0) { break; }
                            feederAnswers[i] = placeholder3.PickRandom();
                            if (!MainWordList.List.Contains(feederAnswers[i]))
                                feederAnswers[i] = placeholder3.PickRandom();

                            if (i == 6)
                                metaSuccessful = true;
                        }
                        if (metaSuccessful)
                        {
                            feederAnswers = feederAnswers.Shuffle();
                            break;
                        }
                    }
                    break;
                case 4: // given order
                    for (int i = 0; i < 7; i++)
                    {
                        string[] placeholder = wordsThatExtractThisLetter(metaAnswer[i], i, extractionMethod);
                        if (placeholder.Length <= 0) { metaSuccessful = false; break; }
                        feederAnswers[i] = placeholder.PickRandom();
                        if (!MainWordList.List.Contains(feederAnswers[i]))
                            feederAnswers[i] = placeholder.PickRandom();
                    }
                    break;
                case 5: // reverse given order
                    for (int i = 0; i < 7; i++)
                    {
                        string[] placeholder = wordsThatExtractThisLetter(metaAnswer[i], i, extractionMethod);
                        if (placeholder.Length <= 0) { metaSuccessful = false; break; }
                        feederAnswers[6 - i] = placeholder.PickRandom();
                        if (!MainWordList.List.Contains(feederAnswers[i]))
                            feederAnswers[i] = placeholder.PickRandom();
                    }
                    break;
                case 6: // length order
                    int[] randomOrder = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                    randomOrder = randomOrder.Shuffle();
                    for (int j = 0; j < 9; j++)
                    {
                        metaSuccessful = false;
                        for (int i = 0; i < 7; i++)
                        {
                            string[] placeholder = wordsThatExtractThisLetter(metaAnswer[i], i, extractionMethod);
                            string[] placeholder2 = placeholder.Where(x => x.Length == i + randomOrder[j] + 1).ToArray();
                            if (placeholder2.Length <= 0) { break; }
                            feederAnswers[i] = placeholder2.PickRandom();
                            if (!MainWordList.List.Contains(feederAnswers[i]))
                                feederAnswers[i] = placeholder2.PickRandom();

                            if (i == 6)
                                metaSuccessful = true;
                        }
                        if (metaSuccessful)
                        {
                            feederAnswers = feederAnswers.Shuffle();
                            break;
                        }
                    }
                    break;
                case 7: // reverse length order
                    int[] reverseRandomOrder = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                    reverseRandomOrder = reverseRandomOrder.Shuffle();
                    for (int j = 0; j < 9; j++)
                    {
                        metaSuccessful = false;
                        for (int i = 0; i < 7; i++)
                        {
                            string[] placeholder = wordsThatExtractThisLetter(metaAnswer[i], i, extractionMethod);
                            string[] placeholder2 = placeholder.Where(x => x.Length == (6 - i) + reverseRandomOrder[j] + 1).ToArray();
                            if (placeholder2.Length <= 0) { break; }
                            feederAnswers[i] = placeholder2.PickRandom();
                            if (!MainWordList.List.Contains(feederAnswers[i]))
                                feederAnswers[i] = placeholder2.PickRandom();

                            if (i == 6)
                                metaSuccessful = true;
                        }
                        if (metaSuccessful)
                        {
                            feederAnswers = feederAnswers.Shuffle();
                            break;
                        }
                    }
                    break;
            }
        }

        DebugMsg("The answer to the meta is " + metaAnswer + ".");
        DebugMsg("The answers that feed into the meta are:");
        for (int i = 0; i < 7; i++)
            DebugMsg(feederAnswers[i]);

        // Encoding quiz generation
        while (encodedWord.Length != 5 || !MainWordList.List.Contains(encodedWord))
            encodedWord = FiveLetterWords.List.PickRandom();
        encodingMethod = Random.Range(0, 3);

        DebugMsg("--- ENCODING QUIZ ---");
        DebugMsg("The encoded word is " + encodedWord + ", and is encrypted using " + encodingNames[encodingMethod] + ".");

        morseObject.SetActive(false);
        brailleObject.SetActive(false);
        semaphoreObject.SetActive(false);

        switch (encodingMethod)
        {
            case 0: // morse code
                morseObject.SetActive(true);
                for (int i = 0; i < 5; i++)
                    StartCoroutine(MorseCode(i));
                break;
            case 1: // braille
                brailleObject.SetActive(true);
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 6; j++)
                        if (brailleAlphabet[Array.IndexOf(alphabet, encodedWord[i])][j] == '-') { brailleBumpRenderers[i * 6 + j].material = brailleMats[0]; }
                break;
            case 2: // semaphore
                semaphoreObject.SetActive(true);
                for (int i = 0; i < 5; i++)
                {
                    semaphoreHandObjects[i * 2].transform.Rotate(0, 45 * semaphoreAlphabet[Array.IndexOf(alphabet, encodedWord[i]) * 2], 0);
                    semaphoreHandObjects[i * 2 + 1].transform.Rotate(0, 45 * semaphoreAlphabet[Array.IndexOf(alphabet, encodedWord[i]) * 2 + 1], 0);
                }
                break;
        }

        for (int i = 0; i < 5; i++)
        {
            passwordOptions[i][0] = encodedWord[i];
            for (int j = 1; j < 8; j++)
            {
                char placeholder = alphabet[Random.Range(0, 26)];
                while (passwordOptions[i].Contains(placeholder))
                    placeholder = alphabet[Random.Range(0, 26)];
                passwordOptions[i][j] = placeholder;
            }
            passwordOptions[i] = passwordOptions[i].OrderBy(x => x).ToArray();
            passwordIndices[i] = Random.Range(0, 8);
            passwordTexts[i].text = passwordOptions[i][passwordIndices[i]].ToString();
        }

        // Hangman generation
        StartCoroutine(HangmanGen());

        // Mental math generation
        DebugMsg("--- MENTAL MATH ---");

        startingNum = Random.Range(0, 99);
        answerNum = startingNum;
        if (startingNum >= 10)
            startingNumText.text = startingNum.ToString();
        else
            startingNumText.text = "0" + startingNum.ToString();
        DebugMsg("The starting number is " + startingNum + ".");
        DebugMsg("The operators are:");

        for (int i = 0; i < 7; i++)
        {
            operatorType = Random.Range(0, 4);
            if (savedOperator)
                operatorType = 3;
            switch (operatorType)
            {
                case 0: // addition
                    if (answerNum >= 90)
                        goto case 1;
                    operatorNum = Random.Range(1, 21);
                    while (answerNum + operatorNum >= 100)
                        operatorNum = Random.Range(1, 51);
                    operatorStrings[i] = "+ " + operatorNum.ToString();
                    answerNum += operatorNum;
                    break;
                case 1:
                    if (answerNum <= -90)
                        goto case 0;
                    operatorNum = Random.Range(1, 21);
                    while (answerNum - operatorNum <= -100)
                        operatorNum = Random.Range(1, 51);
                    operatorStrings[i] = "- " + operatorNum.ToString();
                    answerNum -= operatorNum;
                    break;
                case 2:
                    if (answerNum >= 50 || answerNum <= -50)
                        goto case 3;
                    operatorNum = Random.Range(2, 10);
                    while (answerNum * operatorNum >= 100 || answerNum * operatorNum <= -100)
                        operatorNum = Random.Range(2, 10);
                    if (Random.Range(0, 4) == 0)
                        operatorNum *= -1;
                    operatorStrings[i] = "× " + operatorNum.ToString();
                    answerNum *= operatorNum;
                    break;
                case 3:
                    if (answerNum % 2 != 0 && answerNum % 3 != 0 && answerNum % 5 != 0 && answerNum % 7 != 0)
                        goto default;
                    operatorNum = Random.Range(2, 10);
                    while (answerNum % operatorNum != 0)
                        operatorNum = Random.Range(2, 10);
                    if (operatorNum == 2 || Random.Range(0, 2) == 0)
                    {
                        operatorStrings[i] = "÷ " + operatorNum.ToString();
                        answerNum /= operatorNum;
                    }
                    else
                    {
                        int placeholder = Random.Range(2, operatorNum);
                        operatorStrings[i] = "× " + placeholder.ToString() + "/" + operatorNum.ToString();
                        answerNum = (answerNum / operatorNum) * placeholder;
                    }
                    break;
                default:
                    savedOperator = true;
                    operatorNum = Random.Range(1, 21);
                    if (answerNum >= 90)
                    {
                        while (answerNum - operatorNum <= -100 || ((answerNum - operatorNum) % 2 != 0 && (answerNum - operatorNum) % 3 != 0 && (answerNum - operatorNum) % 5 != 0 && (answerNum - operatorNum) % 7 != 0))
                            operatorNum = Random.Range(1, 21);
                        operatorStrings[i] = "- " + operatorNum.ToString();
                        answerNum -= operatorNum;
                    }
                    else if (answerNum <= -90)
                    {
                        while (answerNum + operatorNum >= 100 || ((answerNum + operatorNum) % 2 != 0 && (answerNum + operatorNum) % 3 != 0 && (answerNum + operatorNum) % 5 != 0 && (answerNum + operatorNum) % 7 != 0))
                            operatorNum = Random.Range(1, 21);
                        operatorStrings[i] = "+ " + operatorNum.ToString();
                        answerNum += operatorNum;
                    }
                    else if (Random.Range(0, 2) == 0)
                    {
                        while (answerNum - operatorNum <= -100 || ((answerNum - operatorNum) % 2 != 0 && (answerNum - operatorNum) % 3 != 0 && (answerNum - operatorNum) % 5 != 0 && (answerNum - operatorNum) % 7 != 0))
                            operatorNum = Random.Range(1, 21);
                        operatorStrings[i] = "- " + operatorNum.ToString();
                        answerNum -= operatorNum;
                    }
                    else
                    {
                        while (answerNum + operatorNum >= 100 || ((answerNum + operatorNum) % 2 != 0 && (answerNum + operatorNum) % 3 != 0 && (answerNum + operatorNum) % 5 != 0 && (answerNum + operatorNum) % 7 != 0))
                            operatorNum = Random.Range(1, 21);
                        operatorStrings[i] = "+ " + operatorNum.ToString();
                        answerNum += operatorNum;
                    }
                    break;
            }
            operatorTexts[i].text = operatorStrings[i];
            DebugMsg(operatorStrings[i]);
        }
        answerNum = Math.Abs(answerNum);
        DebugMsg("The final answer is " + answerNum.ToString() + ".");

        // Nonogram generation
        DebugMsg("--- NONOGRAM ---");
        DebugMsg("The solution to the nonogram is:");
        while (solutionCount != 1)
        {
            for (int i = 0; i < 36; i++)
            {
                if (Random.Range(0, 2) == 0)
                    nonogramSolution[i] = true;
                else
                    nonogramSolution[i] = false;
            }
            while (nonogramSolution.Where(x => x).Count() < 15)
                nonogramSolution[Random.Range(0, 36)] = true;
            while (nonogramSolution.Where(x => x).Count() > 25)
                nonogramSolution[Random.Range(0, 36)] = false;

            for (int i = 0; i < 6; i++)
            {
                currentStreak = 0;

                for (int j = 0; j < 6; j++)
                {
                    if (nonogramSolution[i * 6 + j])
                    {
                        nonogramText += "X";
                        currentStreak++;
                        if (j == 5)
                            rowClues[i].text += currentStreak.ToString() + " ";
                    }
                    else
                    {
                        nonogramText += "-";
                        if (currentStreak > 0)
                            rowClues[i].text += currentStreak.ToString() + " ";
                        currentStreak = 0;
                    }
                }

                rowClues[i].text = rowClues[i].text.TrimEnd();
                if (rowClues[i].text == "")
                    rowClues[i].text = "0";
                currentStreak = 0;


                for (int j = 0; j < 6; j++)
                {
                    if (nonogramSolution[j * 6 + i])
                    {
                        currentStreak++;
                        if (j == 5)
                            colClues[i].text += currentStreak.ToString() + "\n";
                    }
                    else
                    {
                        if (currentStreak > 0)
                            colClues[i].text += currentStreak.ToString() + "\n";
                        currentStreak = 0;
                    }
                }
                colClues[i].text = colClues[i].text.TrimEnd();
                if (colClues[i].text == "")
                    colClues[i].text = "0";
            }

            int[] rowClueIndices = { Array.IndexOf(possibleClues, rowClues[0].text), Array.IndexOf(possibleClues, rowClues[1].text), Array.IndexOf(possibleClues, rowClues[2].text), Array.IndexOf(possibleClues, rowClues[3].text), Array.IndexOf(possibleClues, rowClues[4].text), Array.IndexOf(possibleClues, rowClues[5].text) };
            int[] colClueIndices = { Array.IndexOf(possibleClues, colClues[0].text.Replace('\n', ' ')), Array.IndexOf(possibleClues, colClues[1].text.Replace('\n', ' ')), Array.IndexOf(possibleClues, colClues[2].text.Replace('\n', ' ')), Array.IndexOf(possibleClues, colClues[3].text.Replace('\n', ' ')), Array.IndexOf(possibleClues, colClues[4].text.Replace('\n', ' ')), Array.IndexOf(possibleClues, colClues[5].text.Replace('\n', ' ')) };

            solutionCount = 0;
            for (int a = 0; a < bazinga[rowClueIndices[0]].Length; a++) // F̴̮̈́̆͛́͛̀̾̓͊͑̕E̵̤̬̘̓̏̏̇͛̕̕͝ͅͅÃ̸͖̮̠̣͕͔̰̙͖͖͔͖͙̦͊͋̈̈͊͂̋͋̊̚Ȓ̵̢͉̥̠̟̩̠̺̱͉̝͋́̈ͅ ̸̖͎͖͉͎̪̟̬̺͓̂̃̏̊̿̔̉̍̋̕͜͝M̶̛̤̙̭͉̩̄̏̐̀́͂͌̄͑͠͝E̴̡͎̼̝͖̫͇̱̟͖͒̄́̀͌̇̾̓
            {
                for (int b = 0; b < bazinga[rowClueIndices[1]].Length; b++)
                {
                    for (int c = 0; c < bazinga[rowClueIndices[2]].Length; c++)
                    {
                        for (int d = 0; d < bazinga[rowClueIndices[3]].Length; d++)
                        {
                            for (int e = 0; e < bazinga[rowClueIndices[4]].Length; e++)
                            {
                                for (int f = 0; f < bazinga[rowClueIndices[5]].Length; f++)
                                {
                                    for (int g = 0; g < 6; g++)
                                    {
                                        if (solutionCount > 1 || !bazinga[colClueIndices[g]].Contains(bazinga[rowClueIndices[0]][a][g].ToString() + bazinga[rowClueIndices[1]][b][g].ToString() + bazinga[rowClueIndices[2]][c][g].ToString() + bazinga[rowClueIndices[3]][d][g].ToString() + bazinga[rowClueIndices[4]][e][g].ToString() + bazinga[rowClueIndices[5]][f][g].ToString()))
                                            break;
                                        if (g == 5)
                                            solutionCount += 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (solutionCount > 1)
            {
                DebugMsg("Puzzle was non-unique. Generating new puzzle...");
                nonogramText = "";
                for (int j = 0; j < 6; j++)
                {
                    rowClues[j].text = "";
                    colClues[j].text = "";
                }
            }
        }
        for (int i = 0; i < 6; i++)
            DebugMsg(nonogramText.Substring(i * 6, 6));

        // sortsortsort

        IconFetch.Instance.WaitForFetch(OnFetched);

        // Spelling bee generation
        seedWord = MainWordList.List.Where(x => x.Distinct().Count() == 7).PickRandom();
        spellingBeeLetters = seedWord.Distinct().ToArray().Shuffle();
        while (spellingBeeLetters.First() == 'Q' || spellingBeeLetters.First() == 'Z' || spellingBeeLetters.First() == 'J' || spellingBeeLetters.First() == 'X')
            spellingBeeLetters = seedWord.Distinct().ToArray().Shuffle();
        while (MainWordList.List.Where(x => x.All(y => spellingBeeLetters.Contains(y)) && x.Contains(spellingBeeLetters.First())).Count() < 20 || spellingBeeLetters.Contains('S')) // :trollface:
        {
            seedWord = MainWordList.List.Where(x => x.Distinct().Count() == 7).PickRandom();
            spellingBeeLetters = seedWord.Distinct().ToArray().Shuffle();
            while (spellingBeeLetters.First() == 'Q' || spellingBeeLetters.First() == 'Z' || spellingBeeLetters.First() == 'J' || spellingBeeLetters.First() == 'X')
                spellingBeeLetters = seedWord.Distinct().ToArray().Shuffle();
        }

        DebugMsg("--- SPELLING BEE ---");
        DebugMsg("The letter in the center of the Spelling Bee is " + spellingBeeLetters[0] + ", and the other letters are " + spellingBeeLetters[1] + spellingBeeLetters[2] + spellingBeeLetters[3] + spellingBeeLetters[4] + spellingBeeLetters[5] + spellingBeeLetters[6] + ".");
        for (int i = 0; i < 7; i++)
            hexagonTexts[i].text = spellingBeeLetters[i].ToString();

        // Spot the difference generation
        for (int i = 0; i < 25; i++)
        {
            bgColors[i] = Random.Range(0, 4);
            symbolColors[i] = Random.Range(0, 4);
            while (symbolColors[i] == bgColors[i])
                symbolColors[i] = Random.Range(0, 4);
            symbols[i] = Random.Range(0, 4);

            gridRenderers1[i].material = spotTheDifferenceMats[bgColors[i]];
            gridRenderers2[i].material = spotTheDifferenceMats[bgColors[i]];
            gridText1[i].color = spotTheDifferenceColors[symbolColors[i]];
            gridText2[i].color = spotTheDifferenceColors[symbolColors[i]];
            gridText1[i].text = actualSymbols[symbols[i]];
            gridText2[i].text = actualSymbols[symbols[i]];
        }

        DebugMsg("--- SPOT THE DIFFERENCE ---");
        for (int i = 0; i < 3; i++)
        {
            impostors[i] = Random.Range(0, 25);
            while (impostors.Where(x => x == impostors[i]).Count() > 1)
                impostors[i] = Random.Range(0, 25);

            DebugMsg("Difference #" + (i + 1) + " is at position " + (impostors[i] + 1) + " in reading order.");
            switch (Random.Range(0, 3))
            {
                case 0:
                    int placeholder = Random.Range(0, 4);
                    while (placeholder == symbolColors[impostors[i]] || placeholder == bgColors[impostors[i]])
                        placeholder = Random.Range(0, 4);
                    gridRenderers2[impostors[i]].material = spotTheDifferenceMats[placeholder];
                    DebugMsg("In the left grid the background's " + colorNames[bgColors[impostors[i]]] + ", but in the right it's " + colorNames[placeholder] + ".");
                    break;
                case 1:
                    int placeholder2 = Random.Range(0, 4);
                    while (placeholder2 == bgColors[impostors[i]] || placeholder2 == symbolColors[impostors[i]])
                        placeholder2 = Random.Range(0, 4);
                    gridText2[impostors[i]].color = spotTheDifferenceColors[placeholder2];
                    DebugMsg("In the left grid the symbol's " + colorNames[symbolColors[impostors[i]]] + ", but in the right it's " + colorNames[placeholder2] + ".");
                    break;
                case 2:
                    int placeholder3 = Random.Range(0, 4);
                    while (placeholder3 == symbols[impostors[i]])
                        placeholder3 = Random.Range(0, 4);
                    gridText2[impostors[i]].text = actualSymbols[placeholder3];
                    DebugMsg("In the left grid the symbol's " + actualSymbols[symbols[impostors[i]]] + ", but in the right it's " + actualSymbols[placeholder3] + ".");
                    break;
            }
        }

    }

    private IEnumerator HangmanGen()
    {
        yield return null;
        DebugMsg("--- HANGMAN ---");
        bool nameIsValid = false;
        validModuleNames = Info.GetModuleNames().Select(i => i.ToUpperInvariant()).Where(x => x.All(y => "ABCDEFGHIJKLMNOPQRSTUVWXYZ ".Contains(y))).Distinct().ToArray();
        Debug.Log("<> " + validModuleNames.Join(", "));
        while (!nameIsValid)
        {
            hangmanSolution = validModuleNames.PickRandom().ToUpper();
            if (hangmanSolution.Split(' ').Length <= 5 && hangmanSolution.Split(' ').All(x => x.Length < 15))
                nameIsValid = true;
        }

        hangmanArray = hangmanSolution.ToCharArray();
        for (int i = 0; i < hangmanArray.Length; i++)
        {
            if (hangmanArray[i] == ' ')
            {
                hangmanState = hangmanState.TrimEnd();
                hangmanState += "\n";
            }
            else
                hangmanState += "_ ";
        }

        hangmanState = hangmanState.TrimEnd();
        blanksText.text = hangmanState;
        guessesText.text = guesses.ToString();
        DebugMsg("The solution to the puzzle is " + hangmanSolution + ".");

        for (int i = 0; i < 10; i++)
            gallowsObjects[i].SetActive(false);
    }

    // Generic stuff
    void CycleAnswer(int direction)
    {
        shownPuzzle = (shownPuzzle + direction + 7) % 7;
        if (!solvedPuzzles[Array.IndexOf(puzzleOrder, Subpuzzle.MentalMath)])
        {
            submissionText.text = "";
            submittedDigits[0] = -1;
            submittedDigits[1] = -1;
        }
        UpdateDisplay();
    }
    void UpdateDisplay()
    {
        SetInactive();
        if (!inSubmissionMode)
        {
            switch (puzzleOrder[shownPuzzle])
            {
                case Subpuzzle.EncodingQuiz:
                    encodingQuiz.SetActive(true);
                    break;
                case Subpuzzle.Hangman:
                    hangman.SetActive(true);
                    break;
                case Subpuzzle.MentalMath:
                    mentalMath.SetActive(true);
                    break;
                case Subpuzzle.Nonogram:
                    nonogram.SetActive(true);
                    break;
                case Subpuzzle.Sorting:
                    sorting.SetActive(true);
                    break;
                case Subpuzzle.SpellingBee:
                    spellingBee.SetActive(true);
                    break;
                case Subpuzzle.SpotTheDifference:
                    spotTheDifference.SetActive(true);
                    if (!stdTimerStarted)
                    {
                        StartCoroutine(SpotTheDifferenceTimer());
                        stdTimerStarted = true;
                    }
                    break;
            }
        }
        if (solvedPuzzles[shownPuzzle])
        {
            answerRenderer.material = borderMats[1];
            answerText.text = feederAnswers[shownPuzzle];
        }
        else
        {
            answerRenderer.material = borderMats[0];
            answerText.text = puzzleNames[(int)puzzleOrder[shownPuzzle]];
        }
    }
    void ToggleSubmission()
    {
        inSubmissionMode = !inSubmissionMode;
        if (inSubmissionMode)
        {
            SetInactive();
            submissionObject.SetActive(true);
        }
        else
        {
            submissionObject.SetActive(false);
            UpdateDisplay();
        }
    }

    // Encoding quiz stuff  
    void CycleLetter(int letterPos)
    {
        passwordIndices[letterPos] = (passwordIndices[letterPos] + 1) % 8;
        passwordTexts[letterPos].text = passwordOptions[letterPos][passwordIndices[letterPos]].ToString();
    }
    void SubmitEncodingQuiz()
    {
        if (!solvedPuzzles[shownPuzzle])
        {
            bool answerIncorrect = false;
            for (int i = 0; i < 5; i++)
                if (passwordOptions[i][passwordIndices[i]] != encodedWord[i]) { answerIncorrect = true; break; }
            if (answerIncorrect)
            {
                Module.HandleStrike();
                DebugMsg("Strike! You submitted " + passwordTexts[0].text + passwordTexts[1].text + passwordTexts[2].text + passwordTexts[3].text + passwordTexts[4].text + " in Encoding Quiz, but the answer was " + encodedWord + ".");
                Audio.PlaySoundAtTransform("strikeSound", Module.transform);
            }
            else
            {
                solvedPuzzles[shownPuzzle] = true;
                DebugMsg("You submitted " + encodedWord + " for the Encoding Quiz and it was correct.");
                DebugMsg("You unlocked the answer " + feederAnswers[shownPuzzle] + ".");
                Audio.PlaySoundAtTransform("subsolveSound", Module.transform);
                UpdateDisplay();
            }
        }
    }
    IEnumerator MorseCode(int ledNumber)
    {
        string letterToDisplay = morseAlphabet[Array.IndexOf(alphabet, encodedWord[ledNumber])];
        int indexIntoLetter = 0;

        while (!solved)
        {
            if (indexIntoLetter == -1)
            {
                yield return new WaitForSeconds(1f);
            }
            else if (letterToDisplay[indexIntoLetter] == '.')
            {
                morseLEDRenderers[ledNumber].material = morseMats[1];
                yield return new WaitForSeconds(.2f);
                morseLEDRenderers[ledNumber].material = morseMats[0];
            }
            else
            {
                morseLEDRenderers[ledNumber].material = morseMats[1];
                yield return new WaitForSeconds(.6f);
                morseLEDRenderers[ledNumber].material = morseMats[0];
            }

            indexIntoLetter += 1;
            if (indexIntoLetter >= letterToDisplay.Length)
                indexIntoLetter = -1;

            yield return new WaitForSeconds(.2f);
        }
    }

    // Hangman stuff
    void Guess(int letter)
    {
        if (!guessedLetters[letter] && !solvedPuzzles[shownPuzzle])
        {
            DebugMsg("You guessed " + qwerty[letter] + " in Hangman.");
            if (hangmanSolution.Contains(qwerty[letter]))
            {
                DebugMsg("That was in the solution.");
                guessedLetters[letter] = true;
                guessedLettersButActuallyLetters += qwerty[letter];
                hangmanState = "";
                for (int i = 0; i < hangmanArray.Length; i++)
                {
                    if (hangmanArray[i] == ' ')
                    {
                        hangmanState = hangmanState.TrimEnd();
                        hangmanState += "\n";
                    }
                    else if (guessedLettersButActuallyLetters.Contains(hangmanArray[i]))
                        hangmanState += hangmanArray[i] + " ";
                    else
                        hangmanState += "_ ";
                }

                hangmanState = hangmanState.TrimEnd();
                blanksText.text = hangmanState;
            }
            else
            {
                DebugMsg("That was not in the solution.");
                for (int i = 0; i < 10; i++)
                {
                    gallowsObjects[i].SetActive(false);
                    if (i == guesses)
                        gallowsObjects[i].SetActive(true);
                }
                guesses++;
                if (guesses < 10)
                    guessesText.text = guesses.ToString();
                else if (guesses == 10)
                    guessesText.text = "-";
                else
                {
                    Module.HandleStrike();
                    DebugMsg("Strike! You went over 10 incorrect guesses in Hangman.");
                    Audio.PlaySoundAtTransform("strikeSound", Module.transform);
                }
            }

            keyboardRenderers[letter].material = keyboardMats[1];

            if (!hangmanState.Contains('_'))
            {
                DebugMsg("You finished the Hangman puzzle.");
                solvedPuzzles[shownPuzzle] = true;
                Audio.PlaySoundAtTransform("subsolveSound", Module.transform);
                UpdateDisplay();
                DebugMsg("You unlocked the answer " + feederAnswers[shownPuzzle] + ".");
            }
        }
    }

    // Mental math stuff
    void EnterMath(int btnPos)
    {
        if (solvedPuzzles[shownPuzzle])
        {

        }
        else if (submittedDigits[0] != -1)
        {
            submittedDigits[1] = btnPos;
            if (submittedDigits[0] * 10 + submittedDigits[1] == answerNum)
            {
                solvedPuzzles[shownPuzzle] = true;
                submissionText.text = submittedDigits[0].ToString() + submittedDigits[1].ToString();
                DebugMsg("You submitted " + answerNum.ToString() + " for Mental Math. That was correct.");
                DebugMsg("You unlocked the answer " + feederAnswers[shownPuzzle] + ".");
                Audio.PlaySoundAtTransform("subsolveSound", Module.transform);
                UpdateDisplay();
            }
            else
            {
                Module.HandleStrike();
                DebugMsg("Strike! You submitted " + submittedDigits[0].ToString() + submittedDigits[1].ToString() + " for Mental Math, but the answer was " + answerNum.ToString() + ".");
                Audio.PlaySoundAtTransform("strikeSound", Module.transform);
                submissionText.text = "";
                submittedDigits[0] = -1;
                submittedDigits[1] = -1;
            }
        }
        else
        {
            submittedDigits[0] = btnPos;
            submissionText.text = submittedDigits[0].ToString() + '\t';
        }
    }

    // Nonogram stuff
    void Toggle()
    {
        if (filling) { filling = false; fillText.text = "Mark Mode"; }
        else { filling = true; fillText.text = "Fill Mode"; }
    }
    void CheckNonogram()
    {
        bool nonogramCorrect = true;
        for (int i = 0; i < 36; i++)
            if (nonogramSolution[i] != nonogramState[i])
                nonogramCorrect = false;
        if (solvedPuzzles[shownPuzzle])
        {

        }
        else if (nonogramCorrect)
        {
            solvedPuzzles[shownPuzzle] = true;
            DebugMsg("You submitted the correct solution for Nonogram.");
            DebugMsg("You unlocked the answer " + feederAnswers[shownPuzzle] + ".");
            Audio.PlaySoundAtTransform("subsolveSound", Module.transform);
            UpdateDisplay();
        }
        else
        {
            Module.HandleStrike();
            DebugMsg("Strike! You submitted an invalid solution for Nonogram.");
            Audio.PlaySoundAtTransform("strikeSound", Module.transform);
        }
    }
    void ClearNonogram()
    {
        for (int i = 0; i < 36; i++)
        {
            nonogramState[i] = false;
            cellRenderers[i].material = nonogramMats[0];
            nonogramMarked[i] = false;
            nonogramTexts[i].text = "";
        }
    }
    void ClickCell(int cellPos)
    {
        if (filling)
        {
            nonogramState[cellPos] = !nonogramState[cellPos];
            if (nonogramState[cellPos])
                cellRenderers[cellPos].material = nonogramMats[1];
            else
                cellRenderers[cellPos].material = nonogramMats[0];
        }

        else if (!nonogramState[cellPos])
        {
            nonogramMarked[cellPos] = !nonogramMarked[cellPos];
            if (nonogramMarked[cellPos])
                nonogramTexts[cellPos].text = "✘";
            else
                nonogramTexts[cellPos].text = "";
        }
    }

    // Sorting stuff
    void PressIcon(int iconPos)
    {
        if (solvedPuzzles[shownPuzzle])
        {

        }
        if (!pressedBtns[iconPos] && iconPos == pressOrder[pressedBtns.Where(x => x).ToArray().Length])
        {
            DebugMsg("You pressed " + sortingIconNames[iconPos] + " in Sorting. That was correct.");
            sortingBtnRenderers[iconPos].material = sortingMats[1];
            pressedBtns[iconPos] = true;
            // poob

            if (!pressedBtns.Contains(false))
            {
                DebugMsg("You finished Sorting.");
                DebugMsg("You unlocked the answer " + feederAnswers[shownPuzzle] + ".");
                solvedPuzzles[shownPuzzle] = true;
                Audio.PlaySoundAtTransform("subsolveSound", Module.transform);
                UpdateDisplay();
            }
        }
        else if (!pressedBtns[iconPos])
        {
            Module.HandleStrike();
            DebugMsg("Strike! You pressed " + sortingIconNames[iconPos] + " in Sorting. That was not correct.");
            Audio.PlaySoundAtTransform("strikeSound", Module.transform);
        }
    }

    // Spelling bee stuff
    void LetterPress(int letterPos)
    {
        if (!solvedPuzzles[shownPuzzle])
        {
            if (currentWord.Length < 15)
                currentWord += spellingBeeLetters[letterPos];
            spellingBeeWordText.text = currentWord;
        }
    }
    void SubmitSpellingBee()
    {
        if (MainWordList.List.Contains(currentWord) && currentWord.All(x => spellingBeeLetters.Contains(x)) && currentWord.Contains(spellingBeeLetters[0]) && currentWord.Length > 3 && !submittedWords.Contains(currentWord))
        {
            submittedWords.Add(currentWord);
            currentPoints += currentWord.Length;
            DebugMsg("You submitted " + currentWord + " in Spelling Bee. That was valid.");

            if (currentPoints >= 30)
            {
                currentPoints = 30;
                solvedPuzzles[shownPuzzle] = true;
                UpdateDisplay();
                Audio.PlaySoundAtTransform("subsolveSound", Module.transform);
                DebugMsg("Reached 30+ points. Spelling Bee solved.");
                DebugMsg("You unlocked the answer " + feederAnswers[shownPuzzle] + ".");
            }

            for (int i = 0; i < currentPoints; i++)
                spellingBeeLEDRenderers[i].material = spellingBeeMats[1];
        }
        else
            DebugMsg("You submitted " + currentWord + " in Spelling Bee. That was invalid.");
        currentWord = "";
        spellingBeeWordText.text = currentWord;
    }

    // Spot the difference stuff
    void PressButton(int btnPos)
    {
        if (solvedPuzzles[shownPuzzle])
        {

        }
        else if (impostors.Contains(btnPos))
        {
            if (!pressedSTDBtns[Array.IndexOf(impostors, btnPos)])
            {
                DebugMsg("You pressed one of the differences in Spot the Difference.");
                gridRenderers1[btnPos].material.color = Color.black;
                gridRenderers2[btnPos].material.color = Color.black;
                gridText1[btnPos].color = Color.black;
                gridText2[btnPos].color = Color.black;
            }
            pressedSTDBtns[Array.IndexOf(impostors, btnPos)] = true;
            if (!pressedSTDBtns.Contains(false))
            {
                solvedPuzzles[shownPuzzle] = true;
                Audio.PlaySoundAtTransform("subsolveSound", Module.transform);
                DebugMsg("You unlocked the answer " + feederAnswers[shownPuzzle] + ".");
                UpdateDisplay();
            }
        }
        else
        {
            Module.HandleStrike();
            DebugMsg("Strike! You pressed an incorrect cell in Spot the Difference.");
            Audio.PlaySoundAtTransform("strikeSound", Module.transform);
        }
        progressText.text = progressStrings[pressedSTDBtns.Where(x => x).Count()];
    }
    IEnumerator SpotTheDifferenceTimer()
    {
        int secondsRemaining = 30;
        while (!solvedPuzzles[Array.IndexOf(puzzleOrder, Subpuzzle.SpotTheDifference)])
        {
            if (secondsRemaining <= 0)
            {
                DebugMsg("Resetting Spot the Difference...");
                for (int i = 0; i < 25; i++)
                {
                    bgColors[i] = Random.Range(0, 4);
                    symbolColors[i] = Random.Range(0, 4);
                    while (symbolColors[i] == bgColors[i])
                        symbolColors[i] = Random.Range(0, 4);
                    symbols[i] = Random.Range(0, 4);

                    gridRenderers1[i].material = spotTheDifferenceMats[bgColors[i]];
                    gridRenderers2[i].material = spotTheDifferenceMats[bgColors[i]];
                    gridText1[i].color = spotTheDifferenceColors[symbolColors[i]];
                    gridText2[i].color = spotTheDifferenceColors[symbolColors[i]];
                    gridText1[i].text = actualSymbols[symbols[i]];
                    gridText2[i].text = actualSymbols[symbols[i]];
                }

                for (int i = 0; i < 3; i++)
                {
                    impostors[i] = Random.Range(0, 25);
                    while (impostors.Where(x => x == impostors[i]).Count() > 1)
                        impostors[i] = Random.Range(0, 25);

                    DebugMsg("Difference #" + (i + 1) + " is at position " + (impostors[i] + 1) + " in reading order.");
                    switch (Random.Range(0, 3))
                    {
                        case 0:
                            int placeholder = Random.Range(0, 4);
                            while (placeholder == symbolColors[impostors[i]] || placeholder == bgColors[impostors[i]])
                                placeholder = Random.Range(0, 4);
                            gridRenderers2[impostors[i]].material = spotTheDifferenceMats[placeholder];
                            DebugMsg("In the left grid the background's " + colorNames[bgColors[impostors[i]]] + ", but in the right it's " + colorNames[placeholder] + ".");
                            break;
                        case 1:
                            int placeholder2 = Random.Range(0, 4);
                            while (placeholder2 == bgColors[impostors[i]] || placeholder2 == symbolColors[impostors[i]])
                                placeholder2 = Random.Range(0, 4);
                            gridText2[impostors[i]].color = spotTheDifferenceColors[placeholder2];
                            DebugMsg("In the left grid the symbol's " + colorNames[symbolColors[impostors[i]]] + ", but in the right it's " + colorNames[placeholder2] + ".");
                            break;
                        case 2:
                            int placeholder3 = Random.Range(0, 4);
                            while (placeholder3 == symbols[impostors[i]])
                                placeholder3 = Random.Range(0, 4);
                            gridText2[impostors[i]].text = actualSymbols[placeholder3];
                            DebugMsg("In the left grid the symbol's " + actualSymbols[symbols[impostors[i]]] + ", but in the right it's " + actualSymbols[placeholder3] + ".");
                            break;
                    }
                }

                for (int i = 0; i < 3; i++)
                    pressedSTDBtns[i] = false;
                secondsRemaining = 30;
                progressText.text = "";
            }
            yield return new WaitForSeconds(1);
            secondsRemaining--;
            if (secondsRemaining < 10)
                timerText.text = "0" + secondsRemaining.ToString();
            else
                timerText.text = secondsRemaining.ToString();
        }
    }

    // Submission stuff
    void AnswerKeyboard(int keyPos)
    {
        if (!animationPlaying)
        {
            answerSubmissionText.text = "";
            submittedAnswer += qwerty[keyPos];
            submittedCount++;

            for (int i = 0; i < 7; i++)
            {
                if (i < submittedCount)
                    answerSubmissionText.text += submittedAnswer[i] + " ";
                else
                    answerSubmissionText.text += "- ";
            }

            answerSubmissionText.text = answerSubmissionText.text.TrimEnd();

            if (submittedCount == 7)
            {
                if (submittedAnswer == metaAnswer)
                {
                    solved = true;
                    Module.HandlePass();
                    DebugMsg("You submitted " + submittedAnswer + " for the final answer. Module solved!");
                    Audio.PlaySoundAtTransform("solveSound", Module.transform);
                    answerSubmissionText.color = spotTheDifferenceColors[2];
                    answerSubmissionText.text = victoryAnswers[Random.Range(0, victoryAnswers.Length)];
                }
                else if (sortingMethod == 4 || sortingMethod == 5)
                {
                    string placeholder = metaAnswer;
                    for (int i = 0; i < 6; i++)
                    {
                        string placeholder2 = placeholder.Substring(0, 1);
                        placeholder = placeholder.Substring(1, 6) + placeholder2;
                        if (MainWordList.List.Contains(placeholder) && submittedAnswer == placeholder)
                        {
                            solved = true;
                            Module.HandlePass();
                            DebugMsg("You submitted " + submittedAnswer + " for the final answer. It was supposed to be " + metaAnswer + ", but this works too. Module solved!");
                            Audio.PlaySoundAtTransform("solveSound", Module.transform);
                            answerSubmissionText.color = spotTheDifferenceColors[2];
                            answerSubmissionText.text = victoryAnswers[Random.Range(0, victoryAnswers.Length)];
                            break;
                        }
                    }
                    if (!solved)
                    {
                        submittedAnswer = "";
                        Module.HandleStrike();
                        DebugMsg("Strike! You submitted " + submittedAnswer + " for the final answer, but it was " + metaAnswer + ".");
                        Audio.PlaySoundAtTransform("strikeSound", Module.transform);
                        submittedCount = 0;
                        StartCoroutine(StrikeAnim());
                    }
                }
                else
                {
                    submittedAnswer = "";
                    Module.HandleStrike();
                    DebugMsg("Strike! You submitted " + submittedAnswer + " for the final answer, but it was " + metaAnswer + ".");
                    Audio.PlaySoundAtTransform("strikeSound", Module.transform);
                    submittedCount = 0;
                    StartCoroutine(StrikeAnim());
                }
            }
        }

    }
    void ClearAnswer()
    {
        submittedAnswer = "";
        answerSubmissionText.text = "- - - - - - -";
        submittedCount = 0;
    }
    IEnumerator StrikeAnim()
    {
        animationPlaying = true;
        answerSubmissionText.color = spotTheDifferenceColors[0];
        answerSubmissionText.text = strikeAnswers[Random.Range(0, strikeAnswers.Length)];
        yield return new WaitForSeconds(1f);
        answerSubmissionText.color = Color.white;
        answerSubmissionText.text = "- - - - - - -";
        animationPlaying = false;
    }

    // Unimportant stuff
    void SetInactive()
    {
        encodingQuiz.SetActive(false);
        hangman.SetActive(false);
        mentalMath.SetActive(false);
        nonogram.SetActive(false);
        sorting.SetActive(false);
        spellingBee.SetActive(false);
        spotTheDifference.SetActive(false);
    }
    void DebugMsg(string msg)
    {
        Debug.LogFormat("[Metapuzzle #{0}] {1}", _moduleId, msg);
    }
    string[] wordsThatExtractThisLetter(char letter, int indexIntoAnswer, int method)
    {
        switch (method)
        {
            case 0: // first letter
                return MainWordList.List.Where(x => x.First() == letter).ToArray();
            case 1: // double letter
                return DoubleLetterWordList.List.Where(x => x.Contains(letter.ToString() + letter.ToString())).ToArray();
            case 2: // triple letter
                return TripleLetterWordList.List.Where(x => x.Count(y => y == letter) == 3).ToArray();
            case 3: // last letter
                return MainWordList.List.Where(x => x.Last() == letter).ToArray();
            case 4: // diagonal
                string[] placeholder1 = MainWordList.List.Where(x => x.Length > indexIntoAnswer).ToArray();
                return placeholder1.Where(x => x.ElementAt(indexIntoAnswer) == letter).ToArray();
            case 5: // reverse diagonal
                string[] placeholder2 = MainWordList.List.Where(x => x.Length > indexIntoAnswer).ToArray();
                return placeholder2.Where(x => x.ElementAt(x.Length - 1 - indexIntoAnswer) == letter).ToArray();
            case 6: // sandwich meat
                switch (letter) // yanderedev type code
                {
                    case 'A':
                        return SandwichWordList.aMeat;
                    case 'B':
                        return SandwichWordList.bMeat;
                    case 'C':
                        return SandwichWordList.cMeat;
                    case 'D':
                        return SandwichWordList.dMeat;
                    case 'E':
                        return SandwichWordList.eMeat;
                    case 'F':
                        return SandwichWordList.fMeat;
                    case 'G':
                        return SandwichWordList.gMeat;
                    case 'H':
                        return SandwichWordList.hMeat;
                    case 'I':
                        return SandwichWordList.iMeat;
                    case 'J':
                        return SandwichWordList.jMeat;
                    case 'K':
                        return SandwichWordList.kMeat;
                    case 'L':
                        return SandwichWordList.lMeat;
                    case 'M':
                        return SandwichWordList.mMeat;
                    case 'N':
                        return SandwichWordList.nMeat;
                    case 'O':
                        return SandwichWordList.oMeat;
                    case 'P':
                        return SandwichWordList.pMeat;
                    case 'Q':
                        return SandwichWordList.qMeat;
                    case 'R':
                        return SandwichWordList.rMeat;
                    case 'S':
                        return SandwichWordList.sMeat;
                    case 'T':
                        return SandwichWordList.tMeat;
                    case 'U':
                        return SandwichWordList.uMeat;
                    case 'V':
                        return SandwichWordList.vMeat;
                    case 'W':
                        return SandwichWordList.wMeat;
                    case 'X':
                        return SandwichWordList.xMeat;
                    case 'Y':
                        return SandwichWordList.yMeat;
                    case 'Z':
                        return SandwichWordList.zMeat;
                    default:
                        return new string[] { };
                }
            case 7:
                switch (letter) // yanderedev type code
                {
                    case 'A':
                        return SandwichWordList.aCrust;
                    case 'B':
                        return SandwichWordList.bCrust;
                    case 'C':
                        return SandwichWordList.cCrust;
                    case 'D':
                        return SandwichWordList.dCrust;
                    case 'E':
                        return SandwichWordList.eCrust;
                    case 'F':
                        return SandwichWordList.fCrust;
                    case 'G':
                        return SandwichWordList.gCrust;
                    case 'H':
                        return SandwichWordList.hCrust;
                    case 'I':
                        return SandwichWordList.iCrust;
                    case 'J':
                        return SandwichWordList.jCrust;
                    case 'K':
                        return SandwichWordList.kCrust;
                    case 'L':
                        return SandwichWordList.lCrust;
                    case 'M':
                        return SandwichWordList.mCrust;
                    case 'N':
                        return SandwichWordList.nCrust;
                    case 'O':
                        return SandwichWordList.oCrust;
                    case 'P':
                        return SandwichWordList.pCrust;
                    case 'R':
                        return SandwichWordList.rCrust;
                    case 'S':
                        return SandwichWordList.sCrust;
                    case 'T':
                        return SandwichWordList.tCrust;
                    case 'U':
                        return SandwichWordList.uCrust;
                    case 'V':
                        return SandwichWordList.vCrust;
                    case 'W':
                        return SandwichWordList.wCrust;
                    case 'Y':
                        return SandwichWordList.yCrust;
                    case 'Z':
                        return SandwichWordList.zCrust;
                    default:
                        return new string[] { };
                }
            default:
                break;
        }

        return new string[] { };
    }

    private readonly Texture2D[] Textures = new Texture2D[4];

    private void OnFetched(bool error)
    {
        /* Use for testing if Mod IDs work. Either add or remove a / after this asterisk: *
        for (int i = 0; i < _moduleList.Length; i++)
        {
            var a = _moduleList[i][1];
            Debug.Log(a);
            var b = IconFetch.Instance.GetIcon(a);
        }
        /**/

        DebugMsg("--- SORTING ---");
        DebugMsg("The modules shown are:");

        if (error)
        {
            //show error message, allow button solve etc. 
            Debug.LogFormat("[Metapuzzle #{0}] The module failed to fetch the icons. Sorting has been solved early.", _moduleId);
            solvedPuzzles[Array.IndexOf(puzzleOrder, Subpuzzle.Sorting)] = true;
            _readyToPress = true;
            for (int i = 0; i < 4; i++)
            {
                pressedBtns[i] = true;
                sortingBtnRenderers[i].material = sortingMats[1];
            }
            DebugMsg("You unlocked the answer " + feederAnswers[shownPuzzle] + ".");
            UpdateDisplay();
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            sortingIndices[i] = Random.Range(0, _moduleList.Length);
            while (sortingIndices.Where(x => x == sortingIndices[i]).Count() > 1)
                sortingIndices[i] = Random.Range(0, _moduleList.Length);

            var modId = _moduleList[sortingIndices[i]][1];

            Textures[i] = IconFetch.Instance.GetIcon(modId);
            Textures[i].wrapMode = TextureWrapMode.Clamp;
            Textures[i].filterMode = FilterMode.Point;

            sortingIconNames[i] = _moduleList[sortingIndices[i]][0];
            DebugMsg(sortingIconNames[i]);
            var t = Textures[i];
            iconRenderers[i].sprite = Sprite.Create(t, new Rect(0.0f, 0.0f, t.width, t.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        sortedOrder = sortingIconNames.OrderBy(x => x).ToArray();
        for (int i = 0; i < 4; i++)
            pressOrder[i] = Array.IndexOf(sortingIconNames, sortedOrder[i]);

        _readyToPress = true;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = new string[]
    {
        @"!{0} switch [switch between solve screen and current subpuzzle]",
        @"!{0} left/right <#> [move between subpuzzles]",

        @"!{0} set ABCDE [Encoding Quiz]",
        @"!{0} guess A [Hangman]",
        @"!{0} input 47 [Mental Math]",
        @"!{0} clear/check/fill A1 A2/mark A1 A2 [Nonogram]",
        @"!{0} press 1423 [Sorting]",
        @"!{0} enter WORD [Spelling Bee]",
        @"!{0} tap A2 B5 [Spot The Difference]",

        @"!{0} submit ABCDEFG [clears input and submits answer]"
    }.Join(" | ");
#pragma warning restore 414

    public IEnumerator ProcessTwitchCommand(string command)
    {
        Match m;
        int n = 1;

        if (Regex.IsMatch(command, @"^\s*switch\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            yield return new[] { answerSelectable };
            yield break;
        }

        if ((m = Regex.Match(command, @"^\s*(?<dir>l(eft)?|(?<r>r(ight)?))(\s+(?<n>\d))?\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success
            && (!m.Groups["n"].Success || (int.TryParse(m.Groups["n"].Value, out n) && n > 0 && n < 10)))
        {
            yield return null;
            yield return Enumerable.Repeat(m.Groups["r"].Success ? rightSelectable : leftSelectable, m.Groups["n"].Success ? n : 1).ToArray();
            yield break;
        }

        if ((m = Regex.Match(command, @"^\s*submit\s+(?<word>[A-Z]{7})\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            yield return null;
            var list = new List<KMSelectable>();
            if (!inSubmissionMode)
                list.Add(answerSelectable);
            list.Add(clearSelectable);
            foreach (var ch in m.Groups["word"].Value)
                list.Add(submitKeyboardSelectables[qwerty.IndexOf(char.ToUpperInvariant(ch))]);
            yield return list;
            yield break;
        }

        if (puzzleOrder[shownPuzzle] == Subpuzzle.EncodingQuiz && (m = Regex.Match(command, @"^\s*set\s+(?<ltrs>[A-Z]{5})\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            yield return null;
            var input = m.Groups["ltrs"].Value.ToUpperInvariant();
            for (var letterPos = 0; letterPos < 5; letterPos++)
            {
                if (!passwordOptions[letterPos].Contains(input[letterPos]))
                {
                    yield return string.Format("sendtochaterror Slot #{0} does not contain a letter {1}.", letterPos + 1, input[letterPos]);
                    yield break;
                }
                while (passwordOptions[letterPos][passwordIndices[letterPos]] != input[letterPos])
                    yield return new[] { passwordButtons[letterPos] };
            }
            yield return new[] { encodingQuizSubmitSelectable };
            yield break;
        }

        if (puzzleOrder[shownPuzzle] == Subpuzzle.Hangman && (m = Regex.Match(command, @"^\s*guess\s+(?<ltr>[A-Z])\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            yield return null;
            yield return new[] { keyboardSelectables[qwerty.IndexOf(char.ToUpperInvariant(m.Groups["ltr"].Value[0]))] };
            yield break;
        }

        if (puzzleOrder[shownPuzzle] == Subpuzzle.MentalMath && (m = Regex.Match(command, @"^\s*input\s+(?<n>\d{2})\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            yield return null;
            yield return m.Groups["n"].Value.Select(c => numberKeypadSelectables[c - '0']).ToArray();
            yield break;
        }

        if (puzzleOrder[shownPuzzle] == Subpuzzle.Nonogram && Regex.IsMatch(command, @"^\s*clear\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            yield return new[] { clearNonogramSelectable };
            yield break;
        }

        if (puzzleOrder[shownPuzzle] == Subpuzzle.Nonogram && Regex.IsMatch(command, @"^\s*check\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            yield return new[] { checkSelectable };
            yield break;
        }

        if (puzzleOrder[shownPuzzle] == Subpuzzle.Nonogram && (m = Regex.Match(command, @"^\s*((?<fill>fill)|mark)\s+(?<coords>.*?)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            var list = new List<KMSelectable>();
            if (m.Groups["fill"].Success != filling)
                list.Add(fillSelectable);
            foreach (var elem in m.Groups["coords"].Value.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var tr = elem.Trim().ToUpperInvariant();
                if (tr.Length != 2 || tr[0] < 'A' || tr[0] > 'F' || tr[1] < '1' || tr[1] > '6')
                    yield break;
                list.Add(cellSelectables[tr[0] - 'A' + 6 * (tr[1] - '1')]);
            }
            yield return null;
            yield return list;
            yield break;
        }

        if (puzzleOrder[shownPuzzle] == Subpuzzle.Sorting && (m = Regex.Match(command, @"^\s*press\s+(?<order>[1-4]{1,4})\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            yield return null;
            yield return m.Groups["order"].Value.Select(ch => sortingButtons[ch - '1']).ToArray();
            yield break;
        }

        if (puzzleOrder[shownPuzzle] == Subpuzzle.SpellingBee && (m = Regex.Match(command, @"^\s*enter\s+(?<word>[A-Z]{1,20})\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            var input = m.Groups["word"].Value.Trim().ToUpperInvariant();
            if (input.Any(ch => !spellingBeeLetters.Contains(ch)))
                yield break;
            yield return null;
            yield return input.Select(ch => hexagonSelectables[Array.IndexOf(spellingBeeLetters, ch)]).Concat(new[] { spellingBeeSubmitSelectable }).ToArray();
            yield break;
        }

        if (puzzleOrder[shownPuzzle] == Subpuzzle.SpotTheDifference && (m = Regex.Match(command, @"^\s*tap\s+(?<coords>.*?)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            var list = new List<KMSelectable>();
            foreach (var elem in m.Groups["coords"].Value.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var tr = elem.Trim().ToUpperInvariant();
                if (tr.Length != 2 || tr[0] < 'A' || tr[0] > 'E' || tr[1] < '1' || tr[1] > '5')
                    yield break;
                list.Add(gridSelectables1[tr[0] - 'A' + 5 * (tr[1] - '1')]);
            }
            yield return null;
            yield return list;
            yield break;
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        if (!inSubmissionMode)
        {
            answerSelectable.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        if (!metaAnswer.StartsWith(submittedAnswer))
        {
            clearSelectable.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = submittedAnswer.Length; i < 7; i++)
        {
            submitKeyboardSelectables[qwerty.IndexOf(metaAnswer[i])].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }

    private static readonly string[][] _moduleList = NewArray
    (
        new string[] { "Capacitor Discharge", "NeedyCapacitor" },
        new string[] { "Complicated Wires", "Venn" },
        new string[] { "Keypad", "Keypad" },
        new string[] { "Knob", "NeedyKnob" },
        new string[] { "Maze", "Maze" },
        new string[] { "Memory", "Memory" },
        new string[] { "Morse Code", "Morse" },
        new string[] { "Password", "Password" },
        new string[] { "Simon Says", "Simon" },
        new string[] { "The Button", "BigButton" },
        new string[] { "Venting Gas", "NeedyVentGas" },
        new string[] { "Who's on First", "WhosOnFirst" },
        new string[] { "Wire Sequence", "WireSequence" },
        new string[] { "Wires", "Wires" },
        new string[] { "Colour Flash", "ColourFlash" },
        new string[] { "Piano Keys", "PianoKeys" },
        new string[] { "Semaphore", "Semaphore" },
        new string[] { "Emoji Math", "Emoji Math" },
        new string[] { "Math", "Needy Math" },
        new string[] { "Lights Out", "LightsOut" },
        new string[] { "Switches", "switchModule" },
        new string[] { "Two Bits", "TwoBits" },
        new string[] { "Anagrams", "AnagramsModule" },
        new string[] { "Word Scramble", "WordScrambleModule" },
        new string[] { "Combination Lock", "combinationLock" },
        new string[] { "Filibuster", "Filibuster" },
        new string[] { "Motion Sense", "MotionSense" },
        new string[] { "Answering Questions", "NeedyVentV2" },
        new string[] { "Foreign Exchange Rates", "ForeignExchangeRates" },
        new string[] { "Listening", "Listening" },
        new string[] { "Round Keypad", "KeypadV2" },
        new string[] { "Connection Check", "graphModule" },
        new string[] { "Morsematics", "MorseV2" },
        new string[] { "Orientation Cube", "OrientationCube" },
        new string[] { "Forget Me Not", "MemoryV2" },
        new string[] { "Letter Keys", "LetterKeys" },
        new string[] { "Astrology", "spwizAstrology" },
        new string[] { "Rotary Phone", "NeedyKnobV2" },
        new string[] { "Logic", "Logic" },
        new string[] { "Adventure Game", "spwizAdventureGame" },
        new string[] { "Crazy Talk", "CrazyTalk" },
        new string[] { "Mystic Square", "MysticSquareModule" },
        new string[] { "Turn The Key", "TurnTheKey" },
        new string[] { "Cruel Piano Keys", "CruelPianoKeys" },
        new string[] { "Plumbing", "MazeV2" },
        new string[] { "Safety Safe", "PasswordV2" },
        new string[] { "Tetris", "spwizTetris" },
        new string[] { "Chess", "ChessModule" },
        new string[] { "Cryptography", "CryptModule" },
        new string[] { "Turn The Keys", "TurnTheKeyAdvanced" },
        new string[] { "Mouse In The Maze", "MouseInTheMaze" },
        new string[] { "Silly Slots", "SillySlots" },
        new string[] { "Number Pad", "NumberPad" },
        new string[] { "Simon States", "SimonV2" },
        new string[] { "Laundry", "Laundry" },
        new string[] { "Alphabet", "alphabet" },
        new string[] { "Probing", "Probing" },
        new string[] { "Caesar Cipher", "CaesarCipherModule" },
        new string[] { "Resistors", "resistors" },
        new string[] { "Skewed Slots", "SkewedSlotsModule" },
        new string[] { "Microcontroller", "Microcontroller" },
        new string[] { "Perspective Pegs", "spwizPerspectivePegs" },
        new string[] { "Murder", "murder" },
        new string[] { "The Gamepad", "TheGamepadModule" },
        new string[] { "Tic Tac Toe", "TicTacToeModule" },
        new string[] { "Monsplode, Fight!", "monsplodeFight" },
        new string[] { "Who's That Monsplode?", "monsplodeWho" },
        new string[] { "Shape Shift", "shapeshift" },
        new string[] { "Follow the Leader", "FollowTheLeaderModule" },
        new string[] { "Friendship", "FriendshipModule" },
        new string[] { "The Bulb", "TheBulbModule" },
        new string[] { "Blind Alley", "BlindAlleyModule" },
        new string[] { "English Test", "EnglishTest" },
        new string[] { "Sea Shells", "SeaShells" },
        new string[] { "Rock-Paper-Scissors-Lizard-Spock", "RockPaperScissorsLizardSpockModule" },
        new string[] { "Square Button", "ButtonV2" },
        new string[] { "Hexamaze", "HexamazeModule" },
        new string[] { "Bitmaps", "BitmapsModule" },
        new string[] { "Colored Squares", "ColoredSquaresModule" },
        new string[] { "Adjacent Letters", "AdjacentLettersModule" },
        new string[] { "Third Base", "ThirdBase" },
        new string[] { "Souvenir", "SouvenirModule" },
        new string[] { "Word Search", "WordSearchModule" },
        new string[] { "Broken Buttons", "BrokenButtonsModule" },
        new string[] { "Simon Screams", "SimonScreamsModule" },
        new string[] { "Modules Against Humanity", "ModuleAgainstHumanity" },
        new string[] { "Complicated Buttons", "complicatedButtonsModule" },
        new string[] { "Battleship", "BattleshipModule" },
        new string[] { "Symbolic Password", "symbolicPasswordModule" },
        new string[] { "Text Field", "TextField" },
        new string[] { "Wire Placement", "WirePlacementModule" },
        new string[] { "Double-Oh", "DoubleOhModule" },
        new string[] { "Cheap Checkout", "CheapCheckoutModule" },
        new string[] { "Coordinates", "CoordinatesModule" },
        new string[] { "Light Cycle", "LightCycleModule" },
        new string[] { "HTTP Response", "http" },
        new string[] { "Color Math", "colormath" },
        new string[] { "Rhythms", "MusicRhythms" },
        new string[] { "Only Connect", "OnlyConnectModule" },
        new string[] { "Neutralization", "neutralization" },
        new string[] { "Web Design", "webDesign" },
        new string[] { "Chord Qualities", "ChordQualities" },
        new string[] { "Creation", "CreationModule" },
        new string[] { "Rubik's Cube", "RubiksCubeModule" },
        new string[] { "FizzBuzz", "fizzBuzzModule" },
        new string[] { "The Clock", "TheClockModule" },
        new string[] { "LED Encryption", "LEDEnc" },
        new string[] { "Bitwise Operations", "BitOps" },
        new string[] { "Edgework", "EdgeworkModule" },
        new string[] { "Fast Math", "fastMath" },
        new string[] { "Minesweeper", "MinesweeperModule" },
        new string[] { "Zoo", "ZooModule" },
        new string[] { "Binary LEDs", "BinaryLeds" },
        new string[] { "Boolean Venn Diagram", "booleanVennModule" },
        new string[] { "Point of Order", "PointOfOrderModule" },
        new string[] { "Ice Cream", "iceCreamModule" },
        new string[] { "Hex To Decimal", "EternitySDec" },
        new string[] { "The Screw", "screw" },
        new string[] { "Yahtzee", "YahtzeeModule" },
        new string[] { "X-Ray", "XRayModule" },
        new string[] { "QR Code", "QRCode" },
        new string[] { "Button Masher", "buttonMasherNeedy" },
        new string[] { "Random Number Generator", "rng" },
        new string[] { "Color Morse", "ColorMorseModule" },
        new string[] { "Mastermind Cruel", "Mastermind Cruel" },
        new string[] { "Mastermind Simple", "Mastermind Simple" },
        new string[] { "Gridlock", "GridlockModule" },
        new string[] { "Big Circle", "BigCircle" },
        new string[] { "Morse-A-Maze", "MorseAMaze" },
        new string[] { "Colored Switches", "ColoredSwitchesModule" },
        new string[] { "Perplexing Wires", "PerplexingWiresModule" },
        new string[] { "Monsplode Trading Cards", "monsplodeCards" },
        new string[] { "Game of Life Simple", "GameOfLifeSimple" },
        new string[] { "Game of Life Cruel", "GameOfLifeCruel" },
        new string[] { "Nonogram", "NonogramModule" },
        new string[] { "Refill that Beer!", "NeedyBeer" },
        new string[] { "S.E.T.", "SetModule" },
        new string[] { "Color Generator", "Color Generator" },
        new string[] { "Painting", "Painting" },
        new string[] { "Shape Memory", "needyShapeMemory" },
        new string[] { "Symbol Cycle", "SymbolCycleModule" },
        new string[] { "Hunting", "hunting" },
        new string[] { "Extended Password", "ExtendedPassword" },
        new string[] { "Curriculum", "curriculum" },
        new string[] { "Braille", "BrailleModule" },
        new string[] { "Mafia", "MafiaModule" },
        new string[] { "Festive Piano Keys", "FestivePianoKeys" },
        new string[] { "Flags", "FlagsModule" },
        new string[] { "Timezone", "timezone" },
        new string[] { "Polyhedral Maze", "PolyhedralMazeModule" },
        new string[] { "Poker", "Poker" },
        new string[] { "Symbolic Coordinates", "symbolicCoordinates" },
        new string[] { "Poetry", "poetry" },
        new string[] { "Sonic the Hedgehog", "sonic" },
        new string[] { "Button Sequence", "buttonSequencesModule" },
        new string[] { "Algebra", "algebra" },
        new string[] { "Visual Impairment", "visual_impairment" },
        new string[] { "The Jukebox", "jukebox" },
        new string[] { "Identity Parade", "identityParade" },
        new string[] { "Backgrounds", "Backgrounds" },
        new string[] { "Blind Maze", "BlindMaze" },
        new string[] { "Maintenance", "maintenance" },
        new string[] { "Mortal Kombat", "mortalKombat" },
        new string[] { "Faulty Backgrounds", "FaultyBackgrounds" },
        new string[] { "Mashematics", "mashematics" },
        new string[] { "Modern Cipher", "modernCipher" },
        new string[] { "Radiator", "radiator" },
        new string[] { "LED Grid", "ledGrid" },
        new string[] { "Sink", "Sink" },
        new string[] { "The iPhone", "iPhone" },
        new string[] { "The Swan", "theSwan" },
        new string[] { "Waste Management", "wastemanagement" },
        new string[] { "Human Resources", "HumanResourcesModule" },
        new string[] { "Skyrim", "skyrim" },
        new string[] { "Burglar Alarm", "burglarAlarm" },
        new string[] { "Press X", "PressX" },
        new string[] { "Error Codes", "errorCodes" },
        new string[] { "European Travel", "europeanTravel" },
        new string[] { "Rapid Buttons", "rapidButtons" },
        new string[] { "LEGOs", "LEGOModule" },
        new string[] { "Rubik's Clock", "rubiksClock" },
        new string[] { "Font Select", "FontSelect" },
        new string[] { "Pie", "pieModule" },
        new string[] { "The Stopwatch", "stopwatch" },
        new string[] { "Forget Everything", "HexiEvilFMN" },
        new string[] { "Logic Gates", "logicGates" },
        new string[] { "The London Underground", "londonUnderground" },
        new string[] { "The Wire", "wire" },
        new string[] { "Color Decoding", "Color Decoding" },
        new string[] { "Grid Matching", "GridMatching" },
        new string[] { "The Sun", "sun" },
        new string[] { "Playfair Cipher", "Playfair" },
        new string[] { "Tangrams", "Tangrams" },
        new string[] { "Cooking", "cooking" },
        new string[] { "The Number", "theNumber" },
        new string[] { "Superlogic", "SuperlogicModule" },
        new string[] { "The Moon", "moon" },
        new string[] { "The Cube", "cube" },
        new string[] { "Dr. Doctor", "DrDoctorModule" },
        new string[] { "Tax Returns", "taxReturns" },
        new string[] { "The Jewel Vault", "jewelVault" },
        new string[] { "Digital Root", "digitalRoot" },
        new string[] { "Graffiti Numbers", "graffitiNumbers" },
        new string[] { "Marble Tumble", "MarbleTumbleModule" },
        new string[] { "X01", "X01" },
        new string[] { "Logical Buttons", "logicalButtonsModule" },
        new string[] { "The Code", "theCodeModule" },
        new string[] { "Tap Code", "tapCode" },
        new string[] { "Simon Sends", "SimonSendsModule" },
        new string[] { "Simon Sings", "SimonSingsModule" },
        new string[] { "Greek Calculus", "greekCalculus" },
        new string[] { "Synonyms", "synonyms" },
        new string[] { "Simon Shrieks", "SimonShrieksModule" },
        new string[] { "Complex Keypad", "complexKeypad" },
        new string[] { "Lasers", "lasers" },
        new string[] { "Subways", "subways" },
        new string[] { "Turtle Robot", "turtleRobot" },
        new string[] { "Guitar Chords", "guitarChords" },
        new string[] { "Calendar", "calendar" },
        new string[] { "USA Maze", "USA" },
        new string[] { "Binary Tree", "binaryTree" },
        new string[] { "The Time Keeper", "timeKeeper" },
        new string[] { "Black Hole", "BlackHoleModule" },
        new string[] { "Lightspeed", "lightspeed" },
        new string[] { "Simon's Star", "simonsStar" },
        new string[] { "Morse War", "MorseWar" },
        new string[] { "Maze Scrambler", "MazeScrambler" },
        new string[] { "Mineseeker", "mineseeker" },
        new string[] { "The Stock Market", "stockMarket" },
        new string[] { "The Number Cipher", "numberCipher" },
        new string[] { "Alphabet Numbers", "alphabetNumbers" },
        new string[] { "British Slang", "britishSlang" },
        new string[] { "Double Color", "doubleColor" },
        new string[] { "Equations", "equations" },
        new string[] { "Maritime Flags", "MaritimeFlagsModule" },
        new string[] { "Determinants", "determinant" },
        new string[] { "Pattern Cube", "PatternCubeModule" },
        new string[] { "Know Your Way", "KnowYourWay" },
        new string[] { "Splitting The Loot", "SplittingTheLootModule" },
        new string[] { "Character Shift", "characterShift" },
        new string[] { "Simon Samples", "simonSamples" },
        new string[] { "Dragon Energy", "dragonEnergy" },
        new string[] { "Uncolored Squares", "UncoloredSquaresModule" },
        new string[] { "Flashing Lights", "flashingLights" },
        new string[] { "Synchronization", "SynchronizationModule" },
        new string[] { "The Switch", "BigSwitch" },
        new string[] { "Reverse Morse", "reverseMorse" },
        new string[] { "Manometers", "manometers" },
        new string[] { "Shikaku", "shikaku" },
        new string[] { "Wire Spaghetti", "wireSpaghetti" },
        new string[] { "Module Homework", "KritHomework" },
        new string[] { "Tennis", "TennisModule" },
        new string[] { "Benedict Cumberbatch", "benedictCumberbatch" },
        new string[] { "Boggle", "boggle" },
        new string[] { "Horrible Memory", "horribleMemory" },
        new string[] { "Signals", "Signals" },
        new string[] { "Command Prompt", "KritCMDPrompt" },
        new string[] { "Boolean Maze", "boolMaze" },
        new string[] { "Sonic & Knuckles", "sonicKnuckles" },
        new string[] { "Quintuples", "quintuples" },
        new string[] { "The Sphere", "sphere" },
        new string[] { "Coffeebucks", "coffeebucks" },
        new string[] { "Colorful Madness", "ColorfulMadness" },
        new string[] { "Bases", "bases" },
        new string[] { "Lion's Share", "LionsShareModule" },
        new string[] { "Snooker", "snooker" },
        new string[] { "Blackjack", "KritBlackjack" },
        new string[] { "Party Time", "PartyTime" },
        new string[] { "Accumulation", "accumulation" },
        new string[] { "The Plunger Button", "plungerButton" },
        new string[] { "The Digit", "TheDigitModule" },
        new string[] { "The Jack-O'-Lantern", "jackOLantern" },
        new string[] { "T-Words", "tWords" },
        new string[] { "Divided Squares", "DividedSquaresModule" },
        new string[] { "Connection Device", "KritConnectionDev" },
        new string[] { "Instructions", "instructions" },
        new string[] { "Valves", "valves" },
        new string[] { "Blockbusters", "blockbusters" },
        new string[] { "Catchphrase", "catchphrase" },
        new string[] { "Countdown", "countdown" },
        new string[] { "Cruel Countdown", "cruelCountdown" },
        new string[] { "Encrypted Morse", "EncryptedMorse" },
        new string[] { "The Crystal Maze", "crystalMaze" },
        new string[] { "IKEA", "qSwedishMaze" },
        new string[] { "Retirement", "retirement" },
        new string[] { "Periodic Table", "periodicTable" },
        new string[] { "Schlag den Bomb", "qSchlagDenBomb" },
        new string[] { "Mahjong", "MahjongModule" },
        new string[] { "Kudosudoku", "KudosudokuModule" },
        new string[] { "The Radio", "KritRadio" },
        new string[] { "Modulo", "modulo" },
        new string[] { "Number Nimbleness", "numberNimbleness" },
        new string[] { "Challenge & Contact", "challengeAndContact" },
        new string[] { "Pay Respects", "lgndPayRespects" },
        new string[] { "The Triangle", "triangle" },
        new string[] { "Sueet Wall", "SueetWall" },
        new string[] { "Christmas Presents", "christmasPresents" },
        new string[] { "Hot Potato", "HotPotato" },
        new string[] { "Functions", "qFunctions" },
        new string[] { "Hieroglyphics", "hieroglyphics" },
        new string[] { "Needy Mrs Bob", "needyMrsBob" },
        new string[] { "Scripting", "KritScripts" },
        new string[] { "Simon Spins", "SimonSpinsModule" },
        new string[] { "Cursed Double-Oh", "CursedDoubleOhModule" },
        new string[] { "Ten-Button Color Code", "TenButtonColorCode" },
        new string[] { "Crackbox", "CrackboxModule" },
        new string[] { "Street Fighter", "streetFighter" },
        new string[] { "The Labyrinth", "labyrinth" },
        new string[] { "Color Match", "lgndColorMatch" },
        new string[] { "Spinning Buttons", "spinningButtons" },
        new string[] { "The Festive Jukebox", "festiveJukebox" },
        new string[] { "Skinny Wires", "skinnyWires" },
        new string[] { "The Hangover", "hangover" },
        new string[] { "Binary Puzzle", "BinaryPuzzleModule" },
        new string[] { "Factory Maze", "factoryMaze" },
        new string[] { "Broken Guitar Chords", "BrokenGuitarChordsModule" },
        new string[] { "Dominoes", "dominoes" },
        new string[] { "Hogwarts", "HogwartsModule" },
        new string[] { "Regular Crazy Talk", "RegularCrazyTalkModule" },
        new string[] { "Simon Speaks", "SimonSpeaksModule" },
        new string[] { "Discolored Squares", "DiscoloredSquaresModule" },
        new string[] { "Krazy Talk", "krazyTalk" },
        new string[] { "Flip The Coin", "KritFlipTheCoin" },
        new string[] { "Numbers", "Numbers" },
        new string[] { "Alchemy", "JuckAlchemy" },
        new string[] { "Cookie Jars", "cookieJars" },
        new string[] { "Free Parking", "freeParking" },
        new string[] { "Simon's Stages", "simonsStages" },
        new string[] { "Varicolored Squares", "VaricoloredSquaresModule" },
        new string[] { "Simon Squawks", "simonSquawks" },
        new string[] { "Zoni", "lgndZoni" },
        new string[] { "Mad Memory", "MadMemory" },
        new string[] { "Unrelated Anagrams", "unrelatedAnagrams" },
        new string[] { "Bartending", "BartendingModule" },
        new string[] { "Question Mark", "Questionmark" },
        new string[] { "Decolored Squares", "DecoloredSquaresModule" },
        new string[] { "Flavor Text EX", "FlavorTextCruel" },
        new string[] { "Flavor Text", "FlavorText" },
        new string[] { "Shapes And Bombs", "ShapesBombs" },
        new string[] { "Homophones", "homophones" },
        new string[] { "DetoNATO", "Detonato" },
        new string[] { "Air Traffic Controller", "NeedyAirTrafficController" },
        new string[] { "SYNC-125 [3]", "sync125_3" },
        new string[] { "Morse Identification", "lgndMorseIdentification" },
        new string[] { "Westeros", "westeros" },
        new string[] { "LED Math", "lgndLEDMath" },
        new string[] { "Pigpen Rotations", "pigpenRotations" },
        new string[] { "Alphabetical Order", "alphabeticOrder" },
        new string[] { "Simon Sounds", "simonSounds" },
        new string[] { "The Fidget Spinner", "theFidgetSpinner" },
        new string[] { "Simon's Sequence", "simonsSequence" },
        new string[] { "Harmony Sequence", "harmonySequence" },
        new string[] { "Simon Scrambles", "simonScrambles" },
        new string[] { "Unfair Cipher", "unfairCipher" },
        new string[] { "Melody Sequencer", "melodySequencer" },
        new string[] { "Colorful Insanity", "ColorfulInsanity" },
        new string[] { "Gadgetron Vendor", "lgndGadgetronVendor" },
        new string[] { "Left and Right", "leftandRight" },
        new string[] { "Passport Control", "passportControl" },
        new string[] { "Wingdings", "needyWingdings" },
        new string[] { "The Hexabutton", "hexabutton" },
        new string[] { "The Plunger", "needyPlunger" },
        new string[] { "Genetic Sequence", "geneticSequence" },
        new string[] { "Micro-Modules", "KritMicroModules" },
        new string[] { "Elder Futhark", "elderFuthark" },
        new string[] { "Module Maze", "ModuleMaze" },
        new string[] { "Tasha Squeals", "tashaSqueals" },
        new string[] { "Forget This", "forgetThis" },
        new string[] { "Digital Cipher", "digitalCipher" },
        new string[] { "Burger Alarm", "burgerAlarm" },
        new string[] { "Draw", "draw" },
        new string[] { "Grocery Store", "groceryStore" },
        new string[] { "Subscribe to Pewdiepie", "subscribeToPewdiepie" },
        new string[] { "Lombax Cubes", "lgndLombaxCubes" },
        new string[] { "Mega Man 2", "megaMan2" },
        new string[] { "Purgatory", "PurgatoryModule" },
        new string[] { "The Stare", "StareModule" },
        new string[] { "Graphic Memory", "graphicMemory" },
        new string[] { "Quiz Buzz", "quizBuzz" },
        new string[] { "Wavetapping", "Wavetapping" },
        new string[] { "The Hypercube", "TheHypercubeModule" },
        new string[] { "Speak English", "speakEnglish" },
        new string[] { "Seven Wires", "sevenWires" },
        new string[] { "Stack'em", "stackem" },
        new string[] { "Colored Keys", "lgndColoredKeys" },
        new string[] { "The Troll", "troll" },
        new string[] { "Planets", "planets" },
        new string[] { "The Necronomicon", "necronomicon" },
        new string[] { "Four-Card Monte", "Krit4CardMonte" },
        new string[] { "aa", "aa" },
        new string[] { "Alpha", "lgndAlpha" },
        new string[] { "Digit String", "digitString" },
        new string[] { "The Giant's Drink", "giantsDrink" },
        new string[] { "Hidden Colors", "lgndHiddenColors" },
        new string[] { "Snap!", "lgndSnap" },
        new string[] { "Colour Code", "colourcode" },
        new string[] { "Brush Strokes", "brushStrokes" },
        new string[] { "Vexillology", "vexillology" },
        new string[] { "Odd One Out", "OddOneOutModule" },
        new string[] { "Mazematics", "mazematics" },
        new string[] { "The Triangle Button", "theTriangleButton" },
        new string[] { "Equations X", "equationsXModule" },
        new string[] { "Maze³", "maze3" },
        new string[] { "Gryphons", "gryphons" },
        new string[] { "Arithmelogic", "arithmelogic" },
        new string[] { "Roman Art", "romanArtModule" },
        new string[] { "Faulty Sink", "FaultySink" },
        new string[] { "Simon Stops", "simonStops" },
        new string[] { "Morse Buttons", "morseButtons" },
        new string[] { "Terraria Quiz", "lgndTerrariaQuiz" },
        new string[] { "Baba Is Who?", "babaIsWho" },
        new string[] { "Daylight Directions", "daylightDirections" },
        new string[] { "Modulus Manipulation", "modulusManipulation" },
        new string[] { "Risky Wires", "riskyWires" },
        new string[] { "Simon Stores", "simonStores" },
        new string[] { "Triangle Buttons", "triangleButtons" },
        new string[] { "Cryptic Password", "CrypticPassword" },
        new string[] { "Stained Glass", "stainedGlass" },
        new string[] { "The Block", "theBlock" },
        new string[] { "Bamboozling Button", "bamboozlingButton" },
        new string[] { "Insane Talk", "insanetalk" },
        new string[] { "Transmitted Morse", "transmittedMorseModule" },
        new string[] { "A Mistake", "MistakeModule" },
        new string[] { "Green Arrows", "greenArrowsModule" },
        new string[] { "Red Arrows", "redArrowsModule" },
        new string[] { "Encrypted Equations", "EncryptedEquationsModule" },
        new string[] { "Encrypted Values", "EncryptedValuesModule" },
        new string[] { "Yellow Arrows", "yellowArrowsModule" },
        new string[] { "Forget Them All", "forgetThemAll" },
        new string[] { "Ordered Keys", "orderedKeys" },
        new string[] { "Blue Arrows", "blueArrowsModule" },
        new string[] { "Sticky Notes", "stickyNotes" },
        new string[] { "Hyperactive Numbers", "lgndHyperactiveNumbers" },
        new string[] { "Orange Arrows", "orangeArrowsModule" },
        new string[] { "Unordered Keys", "unorderedKeys" },
        new string[] { "Reordered Keys", "reorderedKeys" },
        new string[] { "Button Grid", "buttonGrid" },
        new string[] { "Find The Date", "DateFinder" },
        new string[] { "Misordered Keys", "misorderedKeys" },
        new string[] { "The Matrix", "matrix" },
        new string[] { "Purple Arrows", "purpleArrowsModule" },
        new string[] { "Bordered Keys", "borderedKeys" },
        new string[] { "The Dealmaker", "thedealmaker" },
        new string[] { "Seven Deadly Sins", "sevenDeadlySins" },
        new string[] { "The Ultracube", "TheUltracubeModule" },
        new string[] { "Symbolic Colouring", "symbolicColouring" },
        new string[] { "Recorded Keys", "recordedKeys" },
        new string[] { "The Deck of Many Things", "deckOfManyThings" },
        new string[] { "Character Codes", "characterCodes" },
        new string[] { "Disordered Keys", "disorderedKeys" },
        new string[] { "Raiding Temples", "raidingTemples" },
        new string[] { "Bomb Diffusal", "bombDiffusal" },
        new string[] { "Pong", "NeedyPong" },
        new string[] { "Tallordered Keys", "tallorderedKeys" },
        new string[] { "Cruel Ten Seconds", "cruel10sec" },
        new string[] { "Ten Seconds", "10seconds" },
        new string[] { "Boolean Keypad", "BooleanKeypad" },
        new string[] { "Calculus", "calcModule" },
        new string[] { "Double Expert", "doubleExpert" },
        new string[] { "Pictionary", "pictionaryModule" },
        new string[] { "Toon Enough", "toonEnough" },
        new string[] { "Qwirkle", "qwirkle" },
        new string[] { "Antichamber", "antichamber" },
        new string[] { "Simon Simons", "simonSimons" },
        new string[] { "Constellations", "constellations" },
        new string[] { "Forget Enigma", "forgetEnigma" },
        new string[] { "Lucky Dice", "luckyDice" },
        new string[] { "Cruel Digital Root", "cruelDigitalRootModule" },
        new string[] { "Prime Checker", "PrimeChecker" },
        new string[] { "Faulty Digital Root", "faultyDigitalRootModule" },
        new string[] { "The Crafting Table", "needycrafting" },
        new string[] { "Boot Too Big", "bootTooBig" },
        new string[] { "Vigenère Cipher", "vigenereCipher" },
        new string[] { "Langton's Ant", "langtonAnt" },
        new string[] { "Old Fogey", "oldFogey" },
        new string[] { "Insanagrams", "insanagrams" },
        new string[] { "Treasure Hunt", "treasureHunt" },
        new string[] { "Snakes and Ladders", "snakesAndLadders" },
        new string[] { "Module Movements", "moduleMovements" },
        new string[] { "Bamboozled Again", "bamboozledAgain" },
        new string[] { "Roman Numerals", "romanNumeralsModule" },
        new string[] { "Safety Square", "safetySquare" },
        new string[] { "Colo(u)r Talk", "colourTalk" },
        new string[] { "Annoying Arrows", "lgndAnnoyingArrows" },
        new string[] { "Block Stacks", "blockStacks" },
        new string[] { "Boolean Wires", "booleanWires" },
        new string[] { "Double Arrows", "doubleArrows" },
        new string[] { "Caesar Cycle", "caesarCycle" },
        new string[] { "Partial Derivatives", "partialDerivatives" },
        new string[] { "Vectors", "vectorsModule" },
        new string[] { "Forget Us Not", "forgetUsNot" },
        new string[] { "Needy Piano", "needyPiano" },
        new string[] { "Affine Cycle", "affineCycle" },
        new string[] { "Pigpen Cycle", "pigpenCycle" },
        new string[] { "Flower Patch", "flowerPatch" },
        new string[] { "Playfair Cycle", "playfairCycle" },
        new string[] { "Jumble Cycle", "jumbleCycle" },
        new string[] { "Alpha-Bits", "alphaBits" },
        new string[] { "Forget Perspective", "qkForgetPerspective" },
        new string[] { "Organization", "organizationModule" },
        new string[] { "Jack Attack", "jackAttack" },
        new string[] { "Binary", "Binary" },
        new string[] { "Hill Cycle", "hillCycle" },
        new string[] { "Ultimate Cycle", "ultimateCycle" },
        new string[] { "Chord Progressions", "chordProgressions" },
        new string[] { "Matchematics", "matchematics" },
        new string[] { "Bob Barks", "ksmBobBarks" },
        new string[] { "Simon's On First", "simonsOnFirst" },
        new string[] { "Forget Me Now", "ForgetMeNow" },
        new string[] { "Weird Al Yankovic", "weirdAlYankovic" },
        new string[] { "Simon Selects", "simonSelectsModule" },
        new string[] { "Cryptic Cycle", "crypticCycle" },
        new string[] { "Simon Literally Says", "ksmSimonLitSays" },
        new string[] { "The Witness", "thewitness" },
        new string[] { "Bone Apple Tea", "boneAppleTea" },
        new string[] { "Masyu", "masyuModule" },
        new string[] { "Robot Programming", "robotProgramming" },
        new string[] { "Hold Ups", "KritHoldUps" },
        new string[] { "Red Cipher", "redCipher" },
        new string[] { "A-maze-ing Buttons", "ksmAmazeingButtons" },
        new string[] { "Flash Memory", "FlashMemory" },
        new string[] { "Desert Bus", "desertBus" },
        new string[] { "Common Sense", "commonSense" },
        new string[] { "Orange Cipher", "orangeCipher" },
        new string[] { "Needy Flower Mash", "R4YNeedyFlowerMash" },
        new string[] { "The Very Annoying Button", "veryAnnoyingButton" },
        new string[] { "Unown Cipher", "UnownCipher" },
        new string[] { "TetraVex", "ksmTetraVex" },
        new string[] { "Meter", "meter" },
        new string[] { "The Modkit", "modkit" },
        new string[] { "Timing is Everything", "timingIsEverything" },
        new string[] { "Bamboozling Button Grid", "bamboozlingButtonGrid" },
        new string[] { "Fruits", "fruits" },
        new string[] { "The Rule", "theRule" },
        new string[] { "Footnotes", "footnotes" },
        new string[] { "Lousy Chess", "lousyChess" },
        new string[] { "Module Listening", "moduleListening" },
        new string[] { "Garfield Kart", "garfieldKart" },
        new string[] { "Green Cipher", "greenCipher" },
        new string[] { "Kooky Keypad", "kookyKeypadModule" },
        new string[] { "Yellow Cipher", "yellowCipher" },
        new string[] { "RGB Maze", "rgbMaze" },
        new string[] { "Blue Cipher", "blueCipher" },
        new string[] { "The Legendre Symbol", "legendreSymbol" },
        new string[] { "Forget Me Later", "forgetMeLater" },
        new string[] { "Keypad Lock", "keypadLock" },
        new string[] { "Heraldry", "heraldry" },
        new string[] { "Faulty RGB Maze", "faultyrgbMaze" },
        new string[] { "Indigo Cipher", "indigoCipher" },
        new string[] { "Violet Cipher", "violetCipher" },
        new string[] { "Chinese Counting", "chineseCounting" },
        new string[] { "Color Addition", "colorAddition" },
        new string[] { "Encryption Bingo", "encryptionBingo" },
        new string[] { "Tower of Hanoi", "towerOfHanoi" },
        new string[] { "Keypad Combinations", "keypadCombinations" },
        new string[] { "Kanji", "KanjiModule" },
        new string[] { "UltraStores", "UltraStores" },
        new string[] { "Geometry Dash", "geometryDashModule" },
        new string[] { "Ternary Converter", "qkTernaryConverter" },
        new string[] { "N&Ms", "NandMs" },
        new string[] { "Eight Pages", "lgndEightPages" },
        new string[] { "The Colored Maze", "coloredMaze" },
        new string[] { "White Cipher", "whiteCipher" },
        new string[] { "Gray Cipher", "grayCipher" },
        new string[] { "Black Cipher", "blackCipher" },
        new string[] { "The Hyperlink", "hyperlink" },
        new string[] { "Loopover", "loopover" },
        new string[] { "Corners", "CornersModule" },
        new string[] { "Divisible Numbers", "divisibleNumbers" },
        new string[] { "The High Score", "ksmHighScore" },
        new string[] { "Ingredients", "ingredients" },
        new string[] { "Cruel Boolean Maze", "boolMazeCruel" },
        new string[] { "Intervals", "intervals" },
        new string[] { "Jenga", "jenga" },
        new string[] { "Cheep Checkout", "cheepCheckout" },
        new string[] { "Spelling Bee", "spellingBee" },
        new string[] { "Memorable Buttons", "memorableButtons" },
        new string[] { "Thinking Wires", "thinkingWiresModule" },
        new string[] { "Object Shows", "objectShows" },
        new string[] { "Seven Choose Four", "sevenChooseFour" },
        new string[] { "Lunchtime", "lunchtime" },
        new string[] { "Natures", "mcdNatures" },
        new string[] { "Neutrinos", "neutrinos" },
        new string[] { "Scavenger Hunt", "scavengerHunt" },
        new string[] { "Polygons", "polygons" },
        new string[] { "Ultimate Cipher", "ultimateCipher" },
        new string[] { "Codenames", "codenames" },
        new string[] { "Odd Mod Out", "lgndOddModOut" },
        new string[] { "Blinkstop", "blinkstopModule" },
        new string[] { "Logic Statement", "logicStatement" },
        new string[] { "Ultimate Custom Night", "qkUCN" },
        new string[] { "Hinges", "hinges" },
        new string[] { "Answering Can Be Fun", "AnsweringCanBeFun" },
        new string[] { "BuzzFizz", "buzzfizz" },
        new string[] { "egg", "bigegg" },
        new string[] { "Forget It Not", "forgetItNot" },
        new string[] { "Time Accumulation", "timeAccumulation" },
        new string[] { "Rainbow Arrows", "ksmRainbowArrows" },
        new string[] { "Digital Dials", "digitalDials" },
        new string[] { "Multicolored Switches", "R4YMultiColoredSwitches" },
        new string[] { "Time Signatures", "timeSignatures" },
        new string[] { "Hereditary Base Notation", "hereditaryBaseNotationModule" },
        new string[] { "Passcodes", "xtrpasscodes" },
        new string[] { "Lines of Code", "linesOfCode" },
        new string[] { "The cRule", "the_cRule" },
        new string[] { "Colorful Dials", "colorfulDials" },
        new string[] { "Encrypted Dice", "EncryptedDice" },
        new string[] { "Prime Encryption", "primeEncryption" },
        new string[] { "Naughty or Nice", "lgndNaughtyOrNice" },
        new string[] { "Following Orders", "FollowingOrders" },
        new string[] { "Binary Grid", "binaryGrid" },
        new string[] { "Cruel Keypads", "CruelKeypads" },
        new string[] { "Matrices", "MatrixQuiz" },
        new string[] { "The Black Page", "TheBlackPage" },
        new string[] { "Simon Forgets", "simonForgets" },
        new string[] { "Greek Letter Grid", "greekLetterGrid" },
        new string[] { "Bamboozling Time Keeper", "bamboozlingTimeKeeper" },
        new string[] { "Scalar Dials", "scalarDials" },
        new string[] { "Keywords", "xtrkeywords" },
        new string[] { "The World's Largest Button", "WorldsLargestButton" },
        new string[] { "State of Aggregation", "stateOfAggregation" },
        new string[] { "Dreamcipher", "ksmDreamcipher" },
        new string[] { "Brainf---", "brainf" },
        new string[] { "Boozleglyph Identification", "boozleglyphIdentification" },
        new string[] { "Echolocation", "echolocation" },
        new string[] { "Hyperneedy", "hyperneedy" },
        new string[] { "Patience", "patience" },
        new string[] { "Rotating Squares", "rotatingSquares" },
        new string[] { "Boxing", "boxing" },
        new string[] { "Topsy Turvy", "topsyTurvy" },
        new string[] { "Railway Cargo Loading", "RailwayCargoLoading" },
        new string[] { "ASCII Art", "asciiArt" },
        new string[] { "Conditional Buttons", "conditionalButtons" },
        new string[] { "Semamorse", "semamorse" },
        new string[] { "Hide and Seek", "hideAndSeek" },
        new string[] { "Symbolic Tasha", "symbolicTasha" },
        new string[] { "Alphabetical Ruling", "alphabeticalRuling" },
        new string[] { "Microphone", "Microphone" },
        new string[] { "Widdershins", "widdershins" },
        new string[] { "Dimension Disruption", "dimensionDisruption" },
        new string[] { "Lockpick Maze", "KritLockpickMaze" },
        new string[] { "V", "V" },
        new string[] { "A Message", "AMessage" },
        new string[] { "Alliances", "alliances" },
        new string[] { "Silhouettes", "silhouettes" },
        new string[] { "Dungeon", "dungeon" },
        new string[] { "Unicode", "UnicodeModule" },
        new string[] { "Password Generator", "pwGenerator" },
        new string[] { "Baccarat", "baccarat" },
        new string[] { "Guess Who?", "GuessWho" },
        new string[] { "Alphabetize", "Alphabetize" },
        new string[] { "Reverse Alphabetize", "ReverseAlphabetize" },
        new string[] { "Gatekeeper", "gatekeeper" },
        new string[] { "Light Bulbs", "LightBulbs" },
        new string[] { "Five Letter Words", "FiveLetterWords" },
        new string[] { "Settlers of KTaNE", "SettlersOfKTaNE" },
        new string[] { "The Hidden Value", "theHiddenValue" },
        new string[] { "Blue", "BlueNeedy" },
        new string[] { "Red", "RedNeedy" },
        new string[] { "Directional Button", "directionalButton" },
        new string[] { "Misery Squares", "SquaresOfMisery" },
        new string[] { "The Simpleton", "SimpleButton" },
        new string[] { "Dungeon 2nd Floor", "dungeon2" },
        new string[] { "Sequences", "sequencesModule" },
        new string[] { "Vcrcs", "VCRCS" },
        new string[] { "Wire Ordering", "kataWireOrdering" },
        new string[] { "Quaternions", "quaternions" },
        new string[] { "Abstract Sequences", "abstractSequences" },
        new string[] { "osu!", "osu" },
        new string[] { "Shifting Maze", "MazeShifting" },
        new string[] { "Art Appreciation", "AppreciateArt" },
        new string[] { "Placeholder Talk", "placeholderTalk" },
        new string[] { "Role Reversal", "roleReversal" },
        new string[] { "Sorting", "sorting" },
        new string[] { "Pattern Lock", "patternLock" },
        new string[] { "Shell Game", "shellGame" },
        new string[] { "Cheat Checkout", "kataCheatCheckout" },
        new string[] { "Minecraft Cipher", "minecraftCipher" },
        new string[] { "Quick Arithmetic", "QuickArithmetic" },
        new string[] { "Forget The Colors", "ForgetTheColors" },
        new string[] { "The Samsung", "theSamsung" },
        new string[] { "Etterna", "etterna" },
        new string[] { "Cruel Garfield Kart", "CruelGarfieldKart" },
        new string[] { "Recolored Switches", "R4YRecoloredSwitches" },
        new string[] { "Reverse Polish Notation", "revPolNot" },
        new string[] { "Snowflakes", "snowflakes" },
        new string[] { "Exoplanets", "exoplanets" },
        new string[] { "Faulty Seven Segment Displays", "faulty7SegmentDisplays" },
        new string[] { "Forget Infinity", "forgetInfinity" },
        new string[] { "Simon Stages", "simonStages" },
        new string[] { "Malfunctions", "malfunctions" },
        new string[] { "Roger", "roger" },
        new string[] { "Stock Images", "StockImages" },
        new string[] { "Minecraft Parody", "minecraftParody" },
        new string[] { "Minecraft Survival", "kataMinecraftSurvival" },
        new string[] { "NumberWang", "kikiNumberWang" },
        new string[] { "Shuffled Strings", "shuffledStrings" },
        new string[] { "Fencing", "fencing" },
        new string[] { "RPS Judging", "RPSJudging" },
        new string[] { "Strike/Solve", "strikeSolve" },
        new string[] { "The Twin", "TheTwinModule" },
        new string[] { "Uncolored Switches", "R4YUncoloredSwitches" },
        new string[] { "Name Changer", "nameChanger" },
        new string[] { "Just Numbers", "JustNumbersModule" },
        new string[] { "Flag Identification", "needyFlagIdentification" },
        new string[] { "Lying Indicators", "lyingIndicators" },
        new string[] { "Training Text", "TrainingText" },
        new string[] { "Caesar's Maths", "caesarsMaths" },
        new string[] { "Wonder Cipher", "WonderCipher" },
        new string[] { "Random Access Memory", "RAM" },
        new string[] { "Triamonds", "triamonds" },
        new string[] { "Button Order", "buttonOrder" },
        new string[] { "Stars", "stars" },
        new string[] { "Elder Password", "elderPassword" },
        new string[] { "Iconic", "iconic" },
        new string[] { "Switching Maze", "MazeSwitching" },
        new string[] { "Ladder Lottery", "ladderLottery" },
        new string[] { "Mystery Module", "mysterymodule" },
        new string[] { "Co-op Harmony Sequence", "coopharmonySequence" },
        new string[] { "Arrow Talk", "ArrowTalk" },
        new string[] { "BoozleTalk", "BoozleTalk" },
        new string[] { "Crazy Talk With A K", "CrazyTalkWithAK" },
        new string[] { "Deck Creating", "kataDeckCreating" },
        new string[] { "Jaden Smith Talk", "JadenSmithTalk" },
        new string[] { "KayMazey Talk", "KMazeyTalk" },
        new string[] { "Kilo Talk", "KiloTalk" },
        new string[] { "Quote Crazy Talk End Quote", "QuoteCrazyTalkEndQuote" },
        new string[] { "Standard Crazy Talk", "StandardCrazyTalk" },
        new string[] { "Siffron", "siffron" },
        new string[] { "Audio Morse", "lgndAudioMorse" },
        new string[] { "Palindromes", "palindromes" },
        new string[] { "Pow", "powModule" },
        new string[] { "Badugi", "ksmBadugi" },
        new string[] { "Chicken Nuggets", "ChickenNuggets" },
        new string[] { "Type Racer", "typeRacer" },
        new string[] { "Masher The Bottun", "masherTheBottun" },
        new string[] { "Negativity", "Negativity" },
        new string[] { "Spot the Difference", "SpotTheDifference" },
        new string[] { "Tetriamonds", "tetriamonds" },
        new string[] { "M&Ns", "MandNs" },
        new string[] { "Yes and No", "yesandno" },
        new string[] { "Goofy's Game", "goofysgame" },
        new string[] { "Integer Trees", "IntegerTrees" },
        new string[] { "Plant Identification", "PlantIdentification" },
        new string[] { "Module Rick", "ModuleRick" },
        new string[] { "Earthbound", "EarthboundModule" },
        new string[] { "Pickup Identification", "PickupIdentification" },
        new string[] { "Life Iteration", "LifeIteration" },
        new string[] { "Accelerando", "accelerando" },
        new string[] { "Encrypted Hangman", "encryptedHangman" },
        new string[] { "Thread the Needle", "threadTheNeedle" },
        new string[] { "Color Braille", "ColorBrailleModule" },
        new string[] { "Reaction", "xtrreaction" },
        new string[] { "The Heart", "TheHeart" },
        new string[] { "Remote Math", "remotemath" },
        new string[] { "Reflex", "lgndReflex" },
        new string[] { "Password Destroyer", "pwDestroyer" },
        new string[] { "hexOS", "hexOS" },
        new string[] { "Multitask", "multitask" },
        new string[] { "Typing Tutor", "needyTypingTutor" },
        new string[] { "Brawler Database", "brawlerDatabaseModule" },
        new string[] { "Kyudoku", "kyudoku" },
        new string[] { "Simon Stashes", "simonStashes" },
        new string[] { "Shortcuts", "shortcuts" },
        new string[] { "More Code", "MoreCode" },
        new string[] { "OmegaForget", "omegaForget" },
        new string[] { "Basic Morse", "BasicMorse" },
        new string[] { "Bloxx", "bloxx" },
        new string[] { "Dictation", "Dictation" },
        new string[] { "Kugelblitz", "kugelblitz" },
        new string[] { "Mental Math", "MentalMath" },
        new string[] { "Needy Game of Life", "gameOfLifeNeedy" },
        new string[] { "Emotiguy Identification", "EmotiguyIdentification" },
        new string[] { "IPA", "ipa" },
        new string[] { "DACH Maze", "DACH" },
        new string[] { "Dumb Waiters", "dumbWaiters" },
        new string[] { "Jailbreak", "Jailbreak" },
        new string[] { "NeeDeez Nuts", "NeeDeezNuts" },
        new string[] { "Birthdays", "birthdays" },
        new string[] { "Match 'em", "matchem" },
        new string[] { "Gnomish Puzzle", "qkGnomishPuzzle" },
        new string[] { "Navinums", "navinums" },
        new string[] { "A>N<D", "ANDmodule" },
        new string[] { "Bridges", "bridges" },
        new string[] { "RGB Logic", "rgbLogic" },
        new string[] { "Juxtacolored Squares", "JuxtacoloredSquaresModule" },
        new string[] { "Shifted Maze", "shiftedMaze" },
        new string[] { "Amnesia", "Amnesia" },
        new string[] { "The Missing Letter", "theMissingLetter" },
        new string[] { "Wolf, Goat, and Cabbage", "wolfGoatCabbageModule" },
        new string[] { "Plug-Ins", "plugins" },
        new string[] { "Synesthesia", "synesthesia" },
        new string[] { "English Entries", "EnglishEntries" },
        new string[] { "The Cruel Duck", "theCruelDuck" },
        new string[] { "The Duck", "theDuck" },
        new string[] { "Identifying Soulless", "identifyingSoulless" },
        new string[] { "Factoring", "factoring" },
        new string[] { "Ultimate Tic Tac Toe", "ultimateTicTacToe" },
        new string[] { "Lyrical Nonsense", "lyricalNonsense" },
        new string[] { "NOT NOT", "notnot" },
        new string[] { "Puzzword", "PuzzwordModule" },
        new string[] { "RGB Sequences", "RGBSequences" },
        new string[] { "Deaf Alley", "deafAlleyModule" },
        new string[] { "int##", "int##" },
        new string[] { "Repo Selector", "qkRepoSelector" },
        new string[] { "Blind Arrows", "blindArrows" },
        new string[] { "D-CODE", "xelDcode" },
        new string[] { "RGB Arithmetic", "rgbArithmetic" },
        new string[] { "Sound Design", "soundDesign" },
        new string[] { "Fifteen", "fifteen" },
        new string[] { "Rapid Subtraction", "rapidSubtraction" },
        new string[] { "Don't Touch Anything", "dontTouchAnything" },
        new string[] { "Pixel Cipher", "pixelcipher" },
        new string[] { "The Great Void", "greatVoid" },
        new string[] { "Negation", "xelNegation" },
        new string[] { "Prime Time", "primeTime" },
        new string[] { "The Calculator", "TheCalculator" },
        new string[] { "ASCII Maze", "asciiMaze" },
        new string[] { "SixTen", "sixten" },
        new string[] { "Ultralogic", "Ultralogic" },
        new string[] { "Busy Beaver", "busyBeaver" },
        new string[] { "Spangled Stars", "spangledStars" },
        new string[] { "Digital Clock", "digitalClock" },
        new string[] { "Assembly Code", "assemblyCode" },
        new string[] { "Cruel Match 'em", "matchemcruel" },
        new string[] { "Simon's Ultimate Showdown", "simonsUltimateShowdownModule" },
        new string[] { "Boomdas", "boomdas" },
        new string[] { "Chinese Strokes", "zhStrokes" },
        new string[] { "Color Numbers", "colorNumbers" },
        new string[] { "Needlessly Complicated Button", "needlesslyComplicatedButton" },
        new string[] { "Chalices", "Chalices" },
        new string[] { "Pixel Art", "PixelArt" },
        new string[] { "Reversed Edgework", "ReversedEdgework" },
        new string[] { "Faulty Accelerando", "faultyAccelerandoModule" },
        new string[] { "Broken Binary", "BrokenBinary" },
        new string[] { "Connected Monitors", "ConnectedMonitorsModule" },
        new string[] { "Cruel Binary", "CruelBinary" },
        new string[] { "Faulty Binary", "FaultyBinary" },
        new string[] { "Increasing Indices", "increasingIndices" },
        new string[] { "Pitch Perfect", "pitchPerfect" },
        new string[] { "Color-Cycle Button", "colorCycleButton" },
        new string[] { "D-CRYPT", "xelDcrypt" },
        new string[] { "ReGret-B Filtering", "regretbFiltering" },
        new string[] { "Tell Me When", "GSTellMeWhen" },
        new string[] { "Totally Accurate Minecraft Simulator", "tams" },
        new string[] { "Alien Filing Colors", "AlienModule" },
        new string[] { "Entry Number Four", "GSEntryNumberFour" },
        new string[] { "The Kanye Encounter", "TheKanyeEncounter" },
        new string[] { "D-CIPHER", "xelDcipher" },
        new string[] { "Color One Two", "colorOneTwo" },
        new string[] { "Brown Bricks", "xelBrownBricks" },
        new string[] { "Burnout", "kataBurnout" },
        new string[] { "Spelling Buzzed", "SpellingBuzzed" },
        new string[] { "Toolmods", "toolmods" },
        new string[] { "Chinese Zodiac", "xelChineseZodiac" },
        new string[] { "Mystic Maze", "mysticmaze" },
        new string[] { "Duck, Duck, Goose", "DUCKDUCKGOOSE" },
        new string[] { "Four Lights", "fourLights" },
        new string[] { "One Links To All", "oneLinksToAllModule" },
        new string[] { "Toolneedy", "toolneedy" },
        new string[] { "Working Title", "workingTitle" },
        new string[] { "Rules", "Rules" },
        new string[] { "Tenpins", "tenpins" },
        new string[] { "Double Listening", "doubleListening" },
        new string[] { "Unfair's Revenge", "unfairsRevenge" },
        new string[] { "Unfair's Cruel Revenge", "unfairsRevengeCruel" },
        new string[] { "Wack Game of Life", "wackGameOfLife" },
        new string[] { "Golf", "golf" },
        new string[] { "Mindlock", "mindlock" },
        new string[] { "Literally Nothing", "literallyNothing" },
        new string[] { "Regular Hexpressions", "RegularHexpressions" },
        new string[] { "Censorship", "Censorship" },
        new string[] { "Colored Buttons", "ColoredButtons" },
        new string[] { "Mechanus Cipher", "mechanusCipher" },
        new string[] { "The Pentabutton", "GSPentabutton" },
        new string[] { "Breaktime", "breaktime" },
        new string[] { "Digisibility", "digisibility" },
        new string[] { "Kim's Game", "KimsGame" },
        new string[] { "Mazery", "Mazery" },
        new string[] { "Space Invaders Extreme", "GSSpaceInvadersExtreme" },
        new string[] { "Popufur", "popufur" },
        new string[] { "Three Cryptic Steps", "ThreeCrypticSteps" },
        new string[] { "Space", "xelSpace" },
        new string[] { "Tech Support", "TechSupport" },
        new string[] { "Metamem", "metamem" },
        new string[] { "M&Ms", "MandMs" },
        new string[] { "The Console", "console" },
        new string[] { "Pocket Planes", "pocketPlanesModule" },
        new string[] { "Bridge", "bridge" },
        new string[] { "Beans", "beans" },
        new string[] { "Beanboozled Again", "beanboozledAgain" },
        new string[] { "Cool Beans", "coolBeans" },
        new string[] { "Jellybeans", "jellybeans" },
        new string[] { "Long Beans", "longBeans" },
        new string[] { "Rotten Beans", "rottenBeans" },
        new string[] { "Broken Karaoke", "xelBrokenKaraoke" },
        new string[] { "Butterflies", "xelButterflies" },
        new string[] { "The Dials", "TheDials" },
        new string[] { "Chamber No. 5", "ChamberNoFive" },
        new string[] { "Silenced Simon", "SilencedSimon" },
        new string[] { "Teal Arrows", "tealArrowsModule" },
        new string[] { "Frankenstein's Indicator", "frankensteinsIndicator" },
        new string[] { "Keep Clicking", "keepClicking" },
        new string[] { "Alphabet Tiles", "AlphabetTiles" },
        new string[] { "Sea Bear Attacks", "seaBearAttacksModule" },
        new string[] { "Devilish Eggs", "devilishEggs" },
        new string[] { "Double Pitch", "DoublePitch" },
        new string[] { "Literally Crying", "literallyCrying" },
        new string[] { "h", "Averageh" },
        new string[] { "Rune Match I", "runeMatchI" },
        new string[] { "Rune Match II", "runeMatchII" },
        new string[] { "Rune Match III", "runeMatchIII" },
        new string[] { "Ars Goetia Identification", "arsGoetiaIdentification" },
        new string[] { "Iñupiaq Numerals", "inupiaqNumerals" },
        new string[] { "Quick Time Events", "xelQuickTimeEvents" },
        new string[] { "The Bioscanner", "TheBioscanner" },
        new string[] { "Pixel Number Base", "PixelNumberBase" },
        new string[] { "Gradually Watermelon", "graduallyWatermelon" },
        new string[] { "Silo Authorization", "siloAuthorization" },
        new string[] { "Digital Grid", "digitalGrid" },
        new string[] { "Even Or Odd", "evenOrOdd" },
        new string[] { "Higher Or Lower", "HigherOrLower" },
        new string[] { "Logical Operators", "logicalOperators" },
        new string[] { "Mastermind Restricted", "mastermindRestricted" },
        new string[] { "Reformed Role Reversal", "ReformedRoleReversal" },
        new string[] { "Whiteout", "whiteout" },
        new string[] { "Cell Lab", "cellLab" },
        new string[] { "Gettin' Funky", "gettinFunkyModule" },
        new string[] { "N&Ns", "NandNs" },
        new string[] { "Color Hexagons", "colorHexagons" },
        new string[] { "Lights On", "lightson" },
        new string[] { "Commuting", "commuting" },
        new string[] { "Look and Say", "LookAndSay" },
        new string[] { "Symmetries Of A Square", "xelSymmetriesOfASquare" },
        new string[] { "Currents", "Currents" },
        new string[] { "Partitions", "partitions" },
        new string[] { "Cruel Stars", "cruelStars" },
        new string[] { "Telepathy", "Telepathy" },
        new string[] { "Button Messer", "qkButtonMesser" },
        new string[] { "Forget Any Color", "ForgetAnyColor" },
        new string[] { "Nomai", "nomai" },
        new string[] { "Taco Tuesday", "tacoTuesday" },
        new string[] { "Melodic Message", "melodicMessage" },
        new string[] { "Table Madness", "TableMadness" },
        new string[] { "Colour Catch", "colourCatch" },
        new string[] { "Sugar Skulls", "sugarSkulls" },
        new string[] { "Cosmic", "CosmicModule" },
        new string[] { "Mislocation", "mislocation" },
        new string[] { "Semabols", "xelSemabols" },
        new string[] { "Musher the Batten", "musherTheBatten" },
        new string[] { "Simon Smiles", "SimonSmiles" },
        new string[] { "Tribal Council", "TribalCouncil" },
        new string[] { "Outrageous", "outrageous" },
        new string[] { "Faulty Chinese Counting", "faultyChineseCounting" },
        new string[] { "Press The Shape", "pressTheShape" },
        new string[] { "Baybayin Words", "BaybayinWords" },
        new string[] { "OmegaDestroyer", "omegaDestroyer" },
        new string[] { "Atbash Cipher", "AtbashCipher" },
        new string[] { "Going Backwards", "GoingBackwards" },
        new string[] { "Blue Hexabuttons", "blueHexabuttons" },
        new string[] { "Green Hexabuttons", "greenHexabuttons" },
        new string[] { "Numbered Buttons", "numberedButtonsModule" },
        new string[] { "Orange Hexabuttons", "orangeHexabuttons" },
        new string[] { "Purple Hexabuttons", "purpleHexabuttons" },
        new string[] { "Red Hexabuttons", "redHexabuttons" },
        new string[] { "Venn Diagrams", "vennDiagram" },
        new string[] { "White Hexabuttons", "whiteHexabuttons" },
        new string[] { "Yellow Hexabuttons", "yellowHexabuttons" },
        new string[] { "Video Poker", "videoPoker" },
        new string[] { "Bottom Gear", "GSBottomGear" },
        new string[] { "Johnson Solids", "xelJohnsonSolids" },
        new string[] { "White Arrows", "WhiteArrows" },
        new string[] { "Keypad Directionality", "KeypadDirectionality" },
        new string[] { "Two Persuasive Buttons", "TwoPersuasiveButtons" },
        new string[] { "Letter Layers", "xelLetterLayers" },
        new string[] { "Towers", "Towers" },
        new string[] { "The Exploding Pen", "TheExplodingPen" },
        new string[] { "ReGrettaBle Relay", "regrettablerelay" },
        new string[] { "Snack Attack", "SnackAttack" },
        new string[] { "Security Council", "SecurityCouncil" },
        new string[] { "Jackbox.TV", "jackboxServerModule" },
        new string[] { "Musical Transposition", "MusicalTransposition" },
        new string[] { "Standard Button Masher", "standardButtonMasher" },
        new string[] { "The Furloid Jukebox", "xelFurloidJukebox" },
        new string[] { "The Close Button", "TheCloseButton" },
        new string[] { "Addition", "Addition" },
        new string[] { "B-Machine", "xelBMachine" },
        new string[] { "Saimoe Pad", "SaimoePad" },
        new string[] { "Updog", "Updog" },
        new string[] { "Quaver", "Quaver" },
        new string[] { "What's on Second", "WhatsOnSecond" },
        new string[] { "Another Keypad Module", "xelAnotherKeypadModule" },
        new string[] { "Think Fast", "GSThinkFast" },
        new string[] { "Rhythm Test", "rhythmTest" },
        new string[] { "Shoddy Chess", "ShoddyChessModule" },
        new string[] { "Bad Wording", "BadWording" },
        new string[] { "Floor Lights", "FloorLights" },
        new string[] { "Validation", "ValidationNeedy" },
        new string[] { "Etch-A-Sketch", "etchASketch" },
        new string[] { "Diophantine Equations", "DiophantineEquations" },
        new string[] { "Zener Cards", "kataZenerCards" },
        new string[] { "Rullo", "rullo" },
        new string[] { "Striped Keys", "kataStripedKeys" },
        new string[] { "Ternary Tiles", "GSTernaryTiles" },
        new string[] { "Black Arrows", "blackArrowsModule" },
        new string[] { "Coloured Arrows", "colouredArrowsModule" },
        new string[] { "Cruello", "cruello" },
        new string[] { "Flashing Arrows", "flashingArrowsModule" },
        new string[] { "Double Screen", "doubleScreenModule" },
        new string[] { "Forget Maze Not", "forgetMazeNot" },
        new string[] { "Tetris Sprint", "tetrisSprint" },
        new string[] { "eeB gnillepS", "eeBgnilleps" },
        new string[] { "The Sequencyclopedia", "TheSequencyclopedia" },
        new string[] { "Number Checker", "NumberChecker" },
        new string[] { "Pandemonium Cipher", "pandemoniumCipher" },
        new string[] { "Mineswapper", "mineswapper" },
        new string[] { "Phosphorescence", "Phosphorescence" },
        new string[] { "The Klaxon", "klaxon" },
        new string[] { "Valued Keys", "valuedKeysModule" },
        new string[] { "Numerical Knight Movement", "NumericalKnightMovement" },
        new string[] { "Bandboozled Again", "bandboozledAgain" },
        new string[] { "Ramboozled Again", "ramboozledAgain" },
        new string[] { "SpriteClub Betting Simulation", "SpriteClubBettingSimulation" },
        new string[] { "Hole in One", "HoleInOne" },
        new string[] { "Simon Subdivides", "simonSubdivides" },
        new string[] { "Audio Keypad", "AudioKeypad" },
        new string[] { "Back Buttons", "backButtonsModule" },
        new string[] { "Collapse", "collapseBasic" },
        new string[] { "Hexiom", "hexiomModule" },
        new string[] { "Bean Sprouts", "beanSprouts" },
        new string[] { "Big Bean", "bigBean" },
        new string[] { "Chilli Beans", "chilliBeans" },
        new string[] { "Fake Beans", "fakeBeans" },
        new string[] { "Kidney Beans", "kidneyBeans" },
        new string[] { "Saimoe Maze", "SaimoeMaze" },
        new string[] { "Bowling", "Bowling" },
        new string[] { "Quiplash", "QLModule" },
        new string[] { "Tell Me Why", "GSTellMeWhy" },
        new string[] { "DNA Mutation", "DNAMutation" },
        new string[] { "Entry Number One", "GSEntryNumberOne" },
        new string[] { "Linq", "Linq" },
        new string[] { "Sporadic Segments", "xelSporadicSegments" },
        new string[] { "Boob Tube", "boobTubeModule" },
        new string[] { "RGB Hypermaze", "rgbhypermaze" },
        new string[] { "AAAAA", "AAAAA" },
        new string[] { "Regular Sudoku", "RegularSudoku" },
        new string[] { "Drive-In Window", "DIWindow" },
        new string[] { "Polyrhythms", "polyrhythms" },
        new string[] { "The 12 Days of Christmas", "GSTwelveDaysOfChristmas" },
        new string[] { "X", "xModule" },
        new string[] { "Y", "yModule" },
        new string[] { "Rebooting M-OS", "RebootingM-Os" },
        new string[] { "The Xenocryst", "GSXenocryst" },
        new string[] { "Complexity", "complexity" },
        new string[] { "Stacked Sequences", "stackedSequences" },
        new string[] { "Small Circle", "smallCircle" },
        new string[] { "Fractal Maze", "fractalMaze" },
        new string[] { "Simon Stumbles", "simonStumbles" },
        new string[] { "Wild Side", "WildSide" },
        new string[] { "The Octadecayotton", "TheOctadecayotton" },
        new string[] { "Colored Letters", "ColoredLetters" },
        new string[] { "Bomb Corp. Filing", "BCFilingNeedy" },
        new string[] { "Forget's Ultimate Showdown", "ForgetsUltimateShowdownModule" },
        new string[] { "Kahoot!", "Kahoot" },
        new string[] { "Mii Identification", "miiIdentification" },
        new string[] { "Ultra Digital Root", "ultraDigitalRootModule" },
        new string[] { "Simon Swindles", "simonSwindles" },
        new string[] { "Next In Line", "NextInLine" },
        new string[] { "Functional Mapping", "functionalMapping" },
        new string[] { "Keypad Maze", "KeypadMaze" },
        new string[] { "Stable Time Signatures", "StableTimeSignatures" },
        new string[] { "Astrological", "Astrological" },
        new string[] { "Corridors", "GSCorridors" },
        new string[] { "XmORse Code", "xmorse" },
        new string[] { "Decay", "decay" },
        new string[] { "Free Password", "FreePassword" },
        new string[] { "Large Free Password", "LargeFreePassword" },
        new string[] { "Large Password", "LargeVanillaPassword" },
        new string[] { "The Burnt", "burnt" },
        new string[] { "Access Codes", "GSAccessCodes" },
        new string[] { "Cistercian Numbers", "xelCistercianNumbers" },
        new string[] { "Brown Cipher", "brownCipher" },
        new string[] { "Code Cracker", "CodeCracker" },
        new string[] { "Indentation", "Indentation" },
        new string[] { "One-Line", "oneLine" },
        new string[] { "Double Knob", "GSDoubleKnob" },
        new string[] { "Interpunct", "interpunct" },
        new string[] { "The Speaker", "theSpeaker" },
        new string[] { "Name Codes", "nameCodes" },
        new string[] { "The 1, 2, 3 Game", "TheOneTwoThreeGame" },
        new string[] { "Hold On", "ashHoldOn" },
        new string[] { "Keypad Magnified", "keypadMagnified" },
        new string[] { "Papa's Pizzeria", "papasPizzeria" },
        new string[] { "Diffusion", "diffusion" },
        new string[] { "Coffee Beans", "coffeeBeans" },
        new string[] { "Soy Beans", "soyBeans" },
        new string[] { "The Shaker", "shaker" },
        new string[] { "Ghost Movement", "ghostMovement" },
        new string[] { "Letter Grid", "LetterGrid" },
        new string[] { "Newline", "newline" },
        new string[] { "Amusement Parks", "amusementParks" },
        new string[] { "RSA Cipher", "RSACipher" },
        new string[] { "Screensaver", "NeedyScreensaver" },
        new string[] { "Transmission Transposition", "transmissionTransposition" },
        new string[] { "Icon Reveal", "IconReveal" },
        new string[] { "Literally Something", "literallySomething" },
        new string[] { "hexOrbits", "hexOrbits" },
        new string[] { "Solitaire Cipher", "solitaireCipher" },
        new string[] { "Matchmaker", "matchmaker" },
        new string[] { "Hearthur", "hearthur" },
        new string[] { "Ladders", "ladders" },
        new string[] { "Color Punch", "ColorPunch" },
        new string[] { "Decimation", "decimation" },
        new string[] { "Count to 69420", "countToSixtynineThousandFourHundredAndTwenty" },
        new string[] { "Mssngv Wls", "MssngvWls" },
        new string[] { "Coinage", "Coinage" },
        new string[] { "Emoticon Math", "emoticonMathModule" },
        new string[] { "Naming Conventions", "NamingConventions" },
        new string[] { "Netherite", "Netherite" },
        new string[] { "Identifrac", "identifrac" },
        new string[] { "Simon Supports", "simonSupports" },
        new string[] { "Cruel Colour Flash", "cruelColourFlash" },
        new string[] { "Factoring Maze", "factoringMaze" },
        new string[] { "Numpath", "numpath" },
        new string[] { "The Logan Parody Jukebox", "LoganJukebox" },
        new string[] { "Binary Buttons", "BinaryButtons" },
        new string[] { "The Alteran Trail", "alteranTrail" },
        new string[] { "The Assorted Arrangement", "TheAssortedArrangement" },
        new string[] { "Needy Wires", "TDSNeedyWires" },
        new string[] { "Pathfinder", "GSPathfinder" },
        new string[] { "Turn Four", "turnFour" },
        new string[] { "Llama, Llama, Alpaca", "llamaLlamaAlpaca" },
        new string[] { "nya~", "TDSNya" },
        new string[] { "Cruel Synesthesia", "cruelSynesthesia" },
        new string[] { "Voltorb Flip", "VoltorbFlip" },
        new string[] { "Dossier Modifier", "TDSDossierModifier" },
        new string[] { "Polygrid", "polygrid" },
        new string[] { "amogus", "TDSAmogus" },
        new string[] { "Mischmodul", "mischmodul" },
        new string[] { "Connect Four", "connectFourModule" },
        new string[] { "Directing Buttons", "GSDirectingButtons" },
        new string[] { "Macro Memory", "macroMemory" },
        new string[] { "Anomia", "anomia" },
        new string[] { "Colors Maximization", "colors_maximization" },
        new string[] { "Antimatter Dimensions", "antimatterDimensions" },
        new string[] { "Blue Whale", "blueWhale" },
        new string[] { "Bottom Gear 2", "GSBottomGear2" },
        new string[] { "Doomsday Button", "doomsdayButton" },
        new string[] { "The Impostor", "impostor" },
        new string[] { "Uncoloured Buttons", "GSUncolouredButtons" },
        new string[] { "Watching Paint Dry", "watchingPaintDry" },
        new string[] { "Dice Cipher", "diceCipher" },
        new string[] { "Soulscream", "soulscream" },
        new string[] { "Weekdays", "weekDays" },
        new string[] { "Face Recognition", "xelFaceRecognition" },
        new string[] { "Infinite Loop", "InfiniteLoop" },
        new string[] { "Mazeswapper", "mazeswapper" },
        new string[] { "Salts", "salts" },
        new string[] { "Alfa-Bravo", "alfa_bravo" },
        new string[] { "Hitman", "HitmanModule" },
        new string[] { "Stoichiometry", "stoichiometryModule" },
        new string[] { "Classical Order", "classicalOrder" },
        new string[] { "Dialtones", "xelDialtones" },
        new string[] { "Needy Hotate", "needyHotate" },
        new string[] { "Space Traders", "space_traders" },
        new string[] { "Cartinese", "cartinese" },
        new string[] { "Cube Synchronization", "qkCubeSynchronization" },
        new string[] { "Eight", "eight" },
        new string[] { "Fursona", "fursona" },
        new string[] { "Notre-Dame Cipher", "notreDameCipher" },
        new string[] { "Kawaiitlyn", "kawaiitlyn" },
        new string[] { "Stupid Slots", "stupidSlots" },
        new string[] { "Red Herring", "RedHerring" },
        new string[] { "Sysadmin", "sysadmin" },
        new string[] { "Binary Shift", "binary_shift" },
        new string[] { "Rain Hell", "rainHellModule" },
        new string[] { "Rain", "rainModule" },
        new string[] { "Meteor", "meteor" },
        new string[] { "Parliament", "parliament" },
        new string[] { "Squeeze", "squeeze" },
        new string[] { "Logging", "Logging" },
        new string[] { "Maze Identification", "GSMazeIdentification" },
        new string[] { "Pink Arrows", "pinkArrows" },
        new string[] { "Anagraphy", "anagraphy" },
        new string[] { "Pawns", "pawns" },
        new string[] { "SUSadmin", "susadmin" },
        new string[] { "Simply Simon", "simplysimon" },
        new string[] { "Black Hexabuttons", "blackHexabuttons" },
        new string[] { "Encrypted Maze", "encryptedMaze" },
        new string[] { "Dimension King", "DimensionKingModule" },
        new string[] { "Fire Diamonds", "fireDiamondsModule" },
        new string[] { "Puzzle Identification", "GSPuzzleIdentification" },
        new string[] { "Face Perception", "face_perception" },
        new string[] { "IKEA Plushies", "ikeaPlushies" },
        new string[] { "Simon Shapes", "SimonShapesModule" },
        new string[] { "Breakfast Egg", "breakfastEgg" },
        new string[] { "Literally Dying", "literallyDying" },
        new string[] { "Literally Malding", "literallyMalding" },
        new string[] { "Cacti's Conundrum", "CactusPConundrum" },
        new string[] { "Simon Shouts", "SimonShoutsModule" },
        new string[] { "Marquee Morse", "marqueeMorseModule" },
        new string[] { "Line Equations", "GSLineEquations" },
        new string[] { "Starmap Reconstruction", "starmap_reconstruction" },
        new string[] { "White Hole", "WhiteHoleModule" },
        new string[] { "Pointless Machines", "PointlessMachines" },
        new string[] { "Maritime Semaphore", "MaritimeSemaphoreModule" },
        new string[] { "Stability", "stabilityModule" },
        new string[] { "Coprime Checker", "coprimeChecker" },
        new string[] { "Labeled Priorities Plus", "labeledPrioritiesPlus" },
        new string[] { "Mastermind Restricted Cruel", "mastermindRestrictedCruel" },
        new string[] { "Warning Signs", "warningSigns" },
        new string[] { "Walking Cube", "WalkingCubeModule" },
        new string[] { "Customer Identification", "xelCustomerIdentification" },
        new string[] { "Out of Time", "OutOfTime" },
        new string[] { "Custom Keys", "RemoteTurnTheKeys" },
        new string[] { "Mind Meld", "mindMeld" },
        new string[] { "Mirror", "mirror" },
        new string[] { "Phones", "phones" },
        new string[] { "Scratch-Off", "scratchOff" },
        new string[] { "Skewers", "Skewers" },
        new string[] { "The Arena", "TheArena" },
        new string[] { "Words", "Words" },
        new string[] { "Insa Ilo", "insaIlo" },
        new string[] { "Placement Roulette", "PlacementRouletteModule" },
        new string[] { "Art Pricing", "artPricing" },
        new string[] { "Brown Hexabuttons", "brownHexabuttons" },
        new string[] { "Gray Hexabuttons", "grayHexabuttons" },
        new string[] { "Perceptron", "perceptron" },
        new string[] { "Coverage", "needy_coverage" },
        new string[] { "RGB Combination", "rgbCombination" },
        new string[] { "Wire Association", "WireAssociationModule" },
        new string[] { "The Icon Kit", "theIconKitModule" },
        new string[] { "The Garnet Thief", "theGarnetThief" },
        new string[] { "Flyswatting", "flyswatting" },
        new string[] { "Ten Aliens", "ten_aliens" },
        new string[] { "Tetrahedron", "tetrahedron" },
        new string[] { "Nonbinary Puzzle", "nonbinaryPuzzle" },
        new string[] { "Simon Said", "simonSaidModule" },
        new string[] { "M-Seq", "mSeq" },
        new string[] { "SQL - Basic", "sqlBasic" },
        new string[] { "MWISort", "mwisort" },
        new string[] { "Touch Transmission", "touchTransmission" },
        new string[] { "TV", "TV" },
        new string[] { "Coordination", "Coordination" },
        new string[] { "Kusa Nihonglish", "kusaNihonglish" },
        new string[] { "SQL - Evil", "sqlEvil" },
        new string[] { "LEDs", "leds" },
        new string[] { "SQL - Cruel", "sqlCruel" },
        new string[] { "Quizbowl", "quizbowl" },
        new string[] { "Superparsing", "superparsing" },
        new string[] { "Clipping Triangles", "clippingTriangles" },
        new string[] { "Dripping Triangles", "drippingTriangles" },
        new string[] { "Flipping Triangles", "flippingTriangles" },
        new string[] { "Skipping Triangles", "skippingTriangles" },
        new string[] { "Slipping Triangles", "slippingTriangles" },
        new string[] { "Tipping Triangles", "tippingTriangles" },
        new string[] { "Tripping Triangles", "trippingTriangles" },
        new string[] { "Discolour Flash", "discolourFlash" },
        new string[] { "Go", "goModule" },
        new string[] { "Simpleton't", "notsimple" },
        new string[] { "Uncolour Flash", "uncolourFlash" },
        new string[] { "Boozlesnap", "boozlesnap" },
        new string[] { "Shogi Identification", "shogiIdentification" },
        new string[] { "The Tile Maze", "theTileMazeModule" },
        new string[] { "hexNull", "hexNull" },
        new string[] { "Shashki", "shashki" },
        new string[] { "Horsey", "qkHorsey" },
        new string[] { "Logic Chess", "logicChess" },
        new string[] { "Shut-the-Box", "ShutTheBox" },
        new string[] { "The Hypercolor", "hypercolor" },
        new string[] { "Candy Land", "candyLand" },
        new string[] { "Congkak", "congkakMQEktane1" },
        new string[] { "Cruel Candy Land", "cruelCandyLand" },
        new string[] { "Label Priorities", "LabelPrioritiesModule" },
        new string[] { "Purchasing Properties", "PurchasingProperties" },
        new string[] { "Robit Programming", "robitProgramming" },
        new string[] { "Simon", "simonSemiboss" },
        new string[] { "Sorry Sliders", "SorrySliders" },
        new string[] { "Termite", "termite" },
        new string[] { "The Board Walk", "BoardWalk" },
        new string[] { "Aquarium", "AquariumModule" },
        new string[] { "CA-RPS", "caRPS" },
        new string[] { "Melody Memory", "melodyMemory" },
        new string[] { "Eight Tiles Panic", "eightTilesPanic" },
        new string[] { "Inselectables", "inselectables" },
        new string[] { "Spongebob Birthday Identification", "spongebobBirthdayIdentification" },
        new string[] { "Binary Tango", "binaryTango" },
        new string[] { "Who's on Morse", "whosOnMorseModule" },
        new string[] { "Antistress", "antistress" },
        new string[] { "Matching Morse", "matchingMorse" },
        new string[] { "Variety", "VarietyModule" },
        new string[] { "Exploding Mittens", "explodingMittens" },
        new string[] { "Cursor Maze", "cursorMazeModule" },
        new string[] { "Gemory", "gemory" },
        new string[] { "Wendithap'n", "Wendithapn" },
        new string[] { "Royal Piano Keys", "royalPianoKeys" },
        new string[] { "Spinning Mazes", "spinningMazes" },
        new string[] { "Birthday Cake", "birthdayCake" },
        new string[] { "Prankster", "prankster" },
        new string[] { "Scrabble Scramble", "scrabbleScramble" },
        new string[] { "Base-1", "base1" },
        new string[] { "Derivatives", "derivatives" },
        new string[] { "Gray Arrows", "grayArrowsModule" },
        new string[] { "Consonants", "needy_consonants" },
        new string[] { "Vector Addition", "needy_vector_addition" },
        new string[] { "Critters", "CrittersModule" },
        new string[] { "Mazeseeker", "GSMazeseeker" },
        new string[] { "Amazing Hexabuttons", "amazingHexabuttons" },
        new string[] { "Colorful Hexabuttons", "colorfulHexabuttons" },
        new string[] { "Lettered Hexabuttons", "letteredHexabuttons" },
        new string[] { "Logical Hexabuttons", "logicalHexabuttons" },
        new string[] { "Magical Hexabuttons", "magicalHexabuttons" },
        new string[] { "Musical Hexabuttons", "musicalHexabuttons" },
        new string[] { "Puzzling Hexabuttons", "puzzlingHexabuttons" },
        new string[] { "Simple Hexabuttons", "simpleHexabuttons" },
        new string[] { "Simon's Satire", "SimonsSatire" },
        new string[] { "Symbolic Hexabuttons", "symbolicHexabuttons" },
        new string[] { "Transmitting Hexabuttons", "transmittingHexabuttons" },
        new string[] { "Voronoi Maze", "VoronoiMazeModule" },
        new string[] { "Duck Konundrum", "duckKonundrum" },
        new string[] { "IKEA Documents", "IKEADocuments" },
        new string[] { "Game of Colors", "GameOfColors" },
        new string[] { "Concentration", "ConcentrationModule" },
        new string[] { "Blaseball", "krazzBlaseball" },
        new string[] { "Metapuzzle", "metapuzzle" }
    );
}
