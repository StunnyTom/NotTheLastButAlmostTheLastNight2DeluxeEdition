using UnityEngine;
using UnityEngine.InputSystem;

public class PentagramKeyboardTest : MonoBehaviour
{
    [SerializeField] private PentagramCandleGroup ritual;

    private InputAction[] numberActions;

    private void Awake()
    {
        numberActions = new InputAction[5];

        // Numpad 1 to 5
        numberActions[0] = new InputAction(binding: "<Keyboard>/numpad1");
        numberActions[1] = new InputAction(binding: "<Keyboard>/numpad2");
        numberActions[2] = new InputAction(binding: "<Keyboard>/numpad3");
        numberActions[3] = new InputAction(binding: "<Keyboard>/numpad4");
        numberActions[4] = new InputAction(binding: "<Keyboard>/numpad5");

        for (int i = 0; i < numberActions.Length; i++)
        {
            int index = i; // capture locale
            numberActions[i].performed += _ => ritual.Toggle(index);
        }
    }

    private void OnEnable()
    {
        foreach (var action in numberActions)
            action.Enable();
    }

    private void OnDisable()
    {
        foreach (var action in numberActions)
            action.Disable();
    }
}
