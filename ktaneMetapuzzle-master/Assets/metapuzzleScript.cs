using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System;
using Random = UnityEngine.Random;

public class metapuzzleScript : MonoBehaviour {

    // Module stuff
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMAudio Audio;
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
    public Sprite[] iconTextures;
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

    // For displaying things
    int shownPuzzle = 0;
    int[] puzzleOrder = { 0, 1, 2, 3, 4, 5, 6 };
    static readonly string[] puzzleNames = { "Encoding Quiz", "Hangman", "Mental Math", "Nonogram", "Sorting", "Spelling Bee", "Spot the Difference" };
    bool[] solvedPuzzles = { false, false, false, false, false, false, false };

    bool inSubmissionMode = false;

    // Encoding quiz variables
    string encodedWord = "PLACEHOLDER";
    int encodingMethod = 0;
    char[][] passwordOptions = { "12345678".ToCharArray(), "12345678".ToCharArray(), "12345678".ToCharArray(), "12345678".ToCharArray(), "12345678".ToCharArray() };

    static readonly string[] encodingNames = { "Morse code", "Braille", "flag semaphore" };
    static readonly string[] morseAlphabet = { ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---", "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-", "..-", "...-", ".--", "-..-", "-.--", "--.." };
    static readonly string[] brailleAlphabet = { ".-----", "..----", ".--.--", ".--..-", ".---.-", "..-.--", "..-..-", "..--.-", "-.-.--", "-.-..-", ".-.---", "...---", ".-..--", ".-...-", ".-.-.-", "....--", ".....-", "...-.-", "-...--", "-....-", ".-.--.", "...--.", "-.-...", ".-..-.", ".-....", ".-.-.." };
    static readonly int[] semaphoreAlphabet = { 4, 5, 4, 6, 4, 7, 4, 0, 4, 1, 4, 2, 4, 3, 5, 6, 5, 7, 0, 2, 5, 0, 5, 1, 5, 2, 5, 3, 6, 7, 6, 0, 6, 1, 6, 2, 6, 3, 7, 0, 7, 1, 7, 2, 1, 2, 1, 3, 7, 2, 2, 3 };

    int[] passwordIndices = { 0, 0, 0, 0, 0 };

    // Hangman variables
    string[] validModuleNames;
    string hangmanSolution;
    char[] hangmanArray;
    string hangmanState;

    int guesses = 0;

    char[] qwerty = "QWERTYUIOPASDFGHJKLZXCVBNM".ToCharArray();
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
    static readonly string[] bannedMods = { "Not The Screw", "Not X-Ray", "Not Poker", "Not Emoji Math", "Not Symbolic Coordinates", "Not The Bulb", "Not Word Search", "Not X01", "Simpleton't", "Not Colour Flash", "Not Connection Check", "Not Coordinates", "Not Crazy Talk", "Not Morsematics", "Not Murder", "Not Knob", "Not Venting Gas", "Not the Button", "Not Capacitor Discharge", "Not Complicated Wires", "Not Keypad", "Not Maze", "Not Memory", "Not Morse Code", "Not Password", "Not Simaze", "Not Who's On First", "Not Wire Sequence", "Not Wiresword" };

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
        leftSelectable.OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); CycleAnswer(-1); return false; };
        rightSelectable.OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);  CycleAnswer(1); return false; };
        answerSelectable.OnInteract += delegate () { if (!solved) { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, Module.transform);  ToggleSubmission(); } return false; };
        // Encoding quiz selectables
        for (int i = 0; i < 5; i++)
        {
            int j = i;
            passwordButtons[i].OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, Module.transform); CycleLetter(j); return false; }; 
        }
        encodingQuizSubmitSelectable.OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); SubmitEncodingQuiz(); return false; };
        // Hangman selectables
        for (int i = 0; i < 26; i++)
        {
            int j = i;
            keyboardSelectables[i].OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, Module.transform); Guess(j); return false; };
        }
        // Mental math selectables
        for (int i = 0; i < 10; i++)
        {
            int j = i;
            numberKeypadSelectables[i].OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); EnterMath(j); return false; };
        }
        // Nonogram selectables
        fillSelectable.OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); Toggle(); return false; };
        checkSelectable.OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); CheckNonogram(); return false; };
        clearNonogramSelectable.OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); ClearNonogram();  return false; };
        for (int i = 0; i < 36; i++)
        {
            int j = i;
            cellSelectables[i].OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, Module.transform); ClickCell(j); return false; };
        }
        // Sorting selectables
        for (int i = 0; i < 4; i++)
        {
            int j = i;
            sortingButtons[i].OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); PressIcon(j); return false; };
        }
        // Spelling bee selectables
        for (int i = 0; i < 7; i++)
        {
            int j = i;
            hexagonSelectables[i].OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, Module.transform); LetterPress(j); return false; };
        }
        spellingBeeSubmitSelectable.OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); SubmitSpellingBee(); return false; };
        // Spot the difference selectables
        for (int i = 0; i < 25; i++)
        {
            int j = i;
            gridSelectables1[i].OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); PressButton(j); return false; };
            gridSelectables2[i].OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); PressButton(j); return false; };
        }
        // Submission selectables
        for (int i = 0; i < 26; i++)
        {
            int j = i;
            submitKeyboardSelectables[i].OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, Module.transform); AnswerKeyboard(j); return false; };
        }
        clearSelectable.OnInteract += delegate () { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform); ClearAnswer(); return false; };
    }

    private void Start()
    {
        submissionObject.SetActive(false);
        Init();
    }

    void Init()
    {
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

        if (extractionMethod == 0 && (sortingMethod == 0 || sortingMethod == 2))
            sortingMethod += 4;
        if (extractionMethod == 1 && (sortingMethod == 1 || sortingMethod == 3))
            sortingMethod += 4;
        if ((extractionMethod == 4 || extractionMethod == 5) && (sortingMethod == 4 || sortingMethod == 5))
            sortingMethod -= 4;
        
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
        DebugMsg("--- HANGMAN ---");
        bool nameIsValid = false;
        validModuleNames = Info.GetModuleNames().Where(x => x.All(y => Char.IsLetter(y) || y == ' ')).Distinct().ToArray();
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

        startingNum = Random.Range(0, 99);
        answerNum = startingNum;
        if (startingNum >= 10)
            startingNumText.text = startingNum.ToString();
        else
            startingNumText.text = "0" + startingNum.ToString();
        DebugMsg("The starting number is " + startingNum + ".");
        DebugMsg("The operators are:");

        for (int i = 0; i < 10; i++)
            gallowsObjects[i].SetActive(false);

        // Mental math generation
        DebugMsg("--- MENTAL MATH ---");
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
            DebugMsg(nonogramText.Substring(i*6, 6));

        // Sorting generation
        DebugMsg("--- SORTING ---");
        DebugMsg("The modules shown are:");
        for (int i = 0; i < 4; i++)
        {
            sortingIndices[i] = Random.Range(0, iconTextures.Length);
            while (sortingIndices.Where(x => x == sortingIndices[i]).Count() > 1 || bannedMods.Contains(iconTextures[sortingIndices[i]].name))
                sortingIndices[i] = Random.Range(0, iconTextures.Length);
            sortingIconNames[i] = iconTextures[sortingIndices[i]].name;
            DebugMsg(sortingIconNames[i]);
            iconRenderers[i].sprite = iconTextures[sortingIndices[i]];
        }
        sortedOrder = sortingIconNames.OrderBy(x => x).ToArray();
        for (int i = 0; i < 4; i++)
            pressOrder[i] = Array.IndexOf(sortingIconNames, sortedOrder[i]);

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

            DebugMsg("Difference #" + (i+1) + " is at position " + (impostors[i]+1) + " in reading order.");
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

    // Generic stuff
    void CycleAnswer(int direction)
    {
        shownPuzzle += direction;
        if (shownPuzzle > 6)
            shownPuzzle = 0;
        if (shownPuzzle < 0)
            shownPuzzle = 6;
        if (!solvedPuzzles[Array.IndexOf(puzzleOrder, 2)])
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
                case 0:
                    encodingQuiz.SetActive(true);
                    break;
                case 1:
                    hangman.SetActive(true);
                    break;
                case 2:
                    mentalMath.SetActive(true);
                    break;
                case 3:
                    nonogram.SetActive(true);
                    break;
                case 4:
                    sorting.SetActive(true);
                    break;
                case 5:
                    spellingBee.SetActive(true);
                    break;
                case 6:
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
            answerText.text = puzzleNames[puzzleOrder[shownPuzzle]];
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
                if (passwordOptions[0][passwordIndices[0]] != encodedWord[0]) { answerIncorrect = true; break; }
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

        else
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
        while (!solvedPuzzles[Array.IndexOf(puzzleOrder, 6)])
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
}
