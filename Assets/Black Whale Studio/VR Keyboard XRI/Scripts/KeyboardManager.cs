using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Keyboard
{
    public class KeyboardManager : MonoBehaviour
    {
        [Header("Keyboard Setup")]
        [SerializeField] private KeyChannel keyChannel;
        [SerializeField] private Button spacebarButton;
        [SerializeField] private Button speechButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button switchButton;
        [SerializeField] private string switchToNumbers = "Numbers";
        [SerializeField] private string switchToLetter = "Letters";

        private TextMeshProUGUI switchButtonText;
        
        [Header("Keyboards")]
        [SerializeField] private GameObject lettersKeyboard;
        [SerializeField] private GameObject numbersKeyboard;
        [SerializeField] private GameObject specialCharactersKeyboard;

        [Header("Shift/Caps Lock Button")] 
        [SerializeField] internal bool autoCapsAtStart = true;
        [SerializeField] private Button shiftButton;
        [SerializeField] private Image buttonImage;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite activeSprite;
        
        [Header("Switch Number/Special Button")]
        [SerializeField] private Button switchNumberSpecialButton;
        [SerializeField] private string numbersString = "Numbers";
        [SerializeField] private string specialString = "Special";

        private TextMeshProUGUI switchNumSpecButtonText;
        
        [Header("Keyboard Button Colors")]
        [SerializeField] private Color normalColor = Color.black;
        [SerializeField] private Color highlightedColor = Color.yellow;
        [SerializeField] private Color pressedColor = Color.red;
        [SerializeField] private Color selectedColor = Color.blue;
        
        [Header("Output Field Settings")]
        [SerializeField] private TMP_InputField outputField;
        [SerializeField] private Button enterButton;
        [SerializeField] private int maxCharacters = 15;
        [SerializeField] private int minCharacters = 3;

        [Header("Press Tracking")]
        [SerializeField] private GameObject pressCounterObject;
        [SerializeField] private TextMeshProUGUI pressCounterText; // Made serializable for direct assignment
        [SerializeField] private string counterFormat = "Presses: {0}";
        private int totalPresses;

        [System.Serializable]
        public class PressEvent : UnityEvent<int> { }
        public PressEvent onPressCountChanged;

        private ColorBlock shiftButtonColors;
        private bool isFirstKeyPress = true;
        private bool keyHasBeenPressed;
        private bool shiftActive;
        private bool capsLockActive;
        private bool specialCharactersActive;
        private float lastShiftClickTime;
        private float shiftDoubleClickDelay = 0.5f;

        public UnityEvent onKeyboardModeChanged;

        private void Awake()
        {
            Debug.Log("KeyboardManager Awake started");
            
            shiftButtonColors = shiftButton.colors;
            
            // Initialize components
            InitializePressCounter();
            
            CheckTextLength();

            speechButton.interactable = false;
            
            // Set initial keyboard states
            numbersKeyboard.SetActive(false);
            specialCharactersKeyboard.SetActive(false);
            lettersKeyboard.SetActive(true);

            // Add button listeners
            spacebarButton.onClick.AddListener(OnSpacePress);
            deleteButton.onClick.AddListener(OnDeletePress);
            switchButton.onClick.AddListener(OnSwitchPress);
            shiftButton.onClick.AddListener(OnShiftPress);
            switchNumberSpecialButton.onClick.AddListener(SwitchBetweenNumbersAndSpecialCharacters);
            
            // Get text components
            switchButtonText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
            switchNumSpecButtonText = switchNumberSpecialButton.GetComponentInChildren<TextMeshProUGUI>();
            
            // Initialize colors
            keyChannel.RaiseKeyColorsChangedEvent(normalColor, highlightedColor, pressedColor, selectedColor);
            
            // Set initial states
            switchNumberSpecialButton.gameObject.SetActive(false);
            numbersKeyboard.SetActive(false);
            specialCharactersKeyboard.SetActive(false);

            if (autoCapsAtStart)
            {
                ActivateShift();
                UpdateShiftButtonAppearance();
            }

            Debug.Log("KeyboardManager Awake completed");
        }

        private void InitializePressCounter()
        {
            Debug.Log("Initializing Press Counter");
            
            // Try to get the counter text component if not already assigned
            if (pressCounterText == null && pressCounterObject != null)
            {
                // First try to get it directly from the object
                pressCounterText = pressCounterObject.GetComponent<TextMeshProUGUI>();
                
                // If not found, try to find it in children
                if (pressCounterText == null)
                {
                    pressCounterText = pressCounterObject.GetComponentInChildren<TextMeshProUGUI>();
                }

                if (pressCounterText == null)
                {
                    Debug.LogWarning("Press counter TextMeshProUGUI component not found!");
                }
                else
                {
                    Debug.Log("Press counter TextMeshProUGUI component found and assigned");
                }
            }

            // Initialize the press event if needed
            if (onPressCountChanged == null)
            {
                onPressCountChanged = new PressEvent();
                Debug.Log("Press event initialized");
            }

            // Reset and update the counter
            totalPresses = 0;
            UpdatePressCounter();
        }

        private void OnDestroy()
        {
            // Remove button listeners
            spacebarButton.onClick.RemoveListener(OnSpacePress);
            deleteButton.onClick.RemoveListener(OnDeletePress);
            switchButton.onClick.RemoveListener(OnSwitchPress);
            shiftButton.onClick.RemoveListener(OnShiftPress);
            switchNumberSpecialButton.onClick.RemoveListener(SwitchBetweenNumbersAndSpecialCharacters);
        }

        private void OnEnable()
        {
            keyChannel.OnKeyPressed += KeyPress;
            Debug.Log("KeyboardManager Enabled - Key press listener added");
        }

        private void OnDisable()
        {
            keyChannel.OnKeyPressed -= KeyPress;
            Debug.Log("KeyboardManager Disabled - Key press listener removed");
        }

        private void KeyPress(string key)
        {
            IncrementPressCounter();
            
            keyHasBeenPressed = true;
            bool wasShiftActive = shiftActive;
            DeactivateShift();

            string textToInsert = (wasShiftActive || capsLockActive) ? key.ToUpper() : key.ToLower();

            int startPos = Mathf.Min(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);
            int endPos = Mathf.Max(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);

            outputField.text = outputField.text.Remove(startPos, endPos - startPos);
            outputField.text = outputField.text.Insert(startPos, textToInsert);

            outputField.selectionAnchorPosition = outputField.selectionFocusPosition = startPos + textToInsert.Length;

            if (isFirstKeyPress)
            {
                isFirstKeyPress = false;
                keyChannel.onFirstKeyPress.Invoke();
            }
    
            CheckTextLength();
        }

        private void OnSpacePress()
        {
            IncrementPressCounter();
            
            int startPos = Mathf.Min(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);
            int endPos = Mathf.Max(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);

            outputField.text = outputField.text.Remove(startPos, endPos - startPos);
            outputField.text = outputField.text.Insert(startPos, " ");

            outputField.selectionAnchorPosition = outputField.selectionFocusPosition = startPos + 1;
            
            CheckTextLength();
        }

        private void OnDeletePress()
        {
            IncrementPressCounter();
            
            if (string.IsNullOrEmpty(outputField.text)) return;
            
            int startPos = Mathf.Min(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);
            int endPos = Mathf.Max(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);

            if (endPos > startPos)
            {
                outputField.text = outputField.text.Remove(startPos, endPos - startPos);
                outputField.selectionAnchorPosition = outputField.selectionFocusPosition = startPos;
            }
            else if (startPos > 0)
            {
                outputField.text = outputField.text.Remove(startPos - 1, 1);
                outputField.selectionAnchorPosition = outputField.selectionFocusPosition = startPos - 1;
            }
            
            CheckTextLength();
        }

        private void CheckTextLength()
        {
            int currentLength = outputField.text.Length;

            bool keysEnabled = currentLength < maxCharacters;
            keyChannel.RaiseKeysStateChangeEvent(keysEnabled);

            enterButton.interactable = currentLength >= minCharacters;
            deleteButton.interactable = true;
            
            if (currentLength == maxCharacters)
            {
                DeactivateShift();
                capsLockActive = false;
                UpdateShiftButtonAppearance();
            }
        }

        private void OnSwitchPress()
        {
            IncrementPressCounter();
            
            if (lettersKeyboard.activeSelf)
            {
                lettersKeyboard.SetActive(false);
                numbersKeyboard.SetActive(true);
                specialCharactersKeyboard.SetActive(false);
                switchNumberSpecialButton.gameObject.SetActive(true);

                switchButtonText.text = switchToNumbers;
                switchNumSpecButtonText.text = specialString;
            }
            else
            {
                lettersKeyboard.SetActive(true);
                numbersKeyboard.SetActive(false);
                specialCharactersKeyboard.SetActive(false);
                switchNumberSpecialButton.gameObject.SetActive(false);

                switchButtonText.text = switchToLetter;
                switchNumSpecButtonText.text = specialString;
            }
            
            DeactivateShift();
            onKeyboardModeChanged?.Invoke();
        }

        private void OnShiftPress()
        {
            IncrementPressCounter();
            
            if (capsLockActive)
            {
                capsLockActive = false;
                shiftActive = false;
            }
            else
            {
                if (shiftActive && !keyHasBeenPressed)
                {
                    if (Time.time - lastShiftClickTime < shiftDoubleClickDelay)
                    {
                        capsLockActive = true;
                        shiftActive = false;
                    }
                    else
                    {
                        shiftActive = false;
                    }
                }
                else
                {
                    shiftActive = true;
                }
            }

            lastShiftClickTime = Time.time;
            UpdateShiftButtonAppearance();
            onKeyboardModeChanged?.Invoke();
        }

        private void ActivateShift()
        {
            if (!capsLockActive)
            {
                shiftActive = true;
            }

            UpdateShiftButtonAppearance();
            onKeyboardModeChanged?.Invoke();
        }

        public void DeactivateShift()
        {
            if (shiftActive && !capsLockActive && keyHasBeenPressed)
            {
                shiftActive = false;
                UpdateShiftButtonAppearance();
                onKeyboardModeChanged?.Invoke();
            }

            keyHasBeenPressed = false;
        }

        public bool IsShiftActive() => shiftActive;

        public bool IsCapsLockActive() => capsLockActive;

        private void SwitchBetweenNumbersAndSpecialCharacters()
        {
            IncrementPressCounter();
            
            if (!lettersKeyboard.activeSelf)
            {
                bool isNumbersKeyboardActive = numbersKeyboard.activeSelf;
                numbersKeyboard.SetActive(!isNumbersKeyboardActive);
                specialCharactersKeyboard.SetActive(isNumbersKeyboardActive);

                switchNumSpecButtonText.text = switchNumSpecButtonText.text == specialString ? numbersString : specialString;

                onKeyboardModeChanged?.Invoke();
            }
        }

        private void UpdateShiftButtonAppearance()
        {
            if (capsLockActive)
            {
                shiftButtonColors.normalColor = highlightedColor;
                buttonImage.sprite = activeSprite;
            }
            else if(shiftActive)
            {
                shiftButtonColors.normalColor = highlightedColor;
                buttonImage.sprite = defaultSprite;
            }
            else
            {
                shiftButtonColors.normalColor = normalColor;
                buttonImage.sprite = defaultSprite;
            }

            shiftButton.colors = shiftButtonColors;
        }

        private void IncrementPressCounter()
        {
            totalPresses++;
            Debug.Log($"Button pressed! Total presses: {totalPresses}");
            UpdatePressCounter();
        }

        private void UpdatePressCounter()
        {
            if (pressCounterText != null)
            {
                string formattedText = string.Format(counterFormat, totalPresses);
                pressCounterText.text = formattedText;
                Debug.Log($"Press Counter Updated: {formattedText}");
            }
            else
            {
                Debug.LogWarning("Press counter text component is null!");
            }

            onPressCountChanged?.Invoke(totalPresses);
        }

        public int GetTotalPresses()
        {
            return totalPresses;
        }

        public void ResetPressCounter()
        {
            totalPresses = 0;
            UpdatePressCounter();
        }

        public void SetPressCounterText(TextMeshProUGUI newCounterText)
        {
            pressCounterText = newCounterText;
            UpdatePressCounter();
        }

        public void SetPressCounterObject(GameObject counterObject)
        {
            pressCounterObject = counterObject;
            InitializePressCounter();
        }
    }
}