using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using System;

public class InputController : MonoBehaviour
{

    public static InputController Instance { get; private set; }
    private List<InputActions> playerActionsList = new List<InputActions>();

    [SerializeField] private float minDeadzone = .3f;
    [SerializeField] private float maxDeadzone = 1;
    public InControlManager controlManager;
    public BindingProperties bindingsScriptable;
    private void Update()
    {
        foreach (var actions in playerActionsList)
        {
            actions.moveValue = MoveValue(actions);
            actions.lookValue = LookValue(actions);
        }
    }


    private void InitializePlayerActions()
    {
        int numberOfPlayers = Mathf.Max(2, InputManager.Devices.Count);  // Ensure at least two players
        for (int i = 0; i < numberOfPlayers; i++)
        {
            var actions = InputActions.CreateWithDefaultBindings(0,0);
            if (i < InputManager.Devices.Count)
            {
                actions.Device = InputManager.Devices[i];  // Assign device if available
            }
            else
            {
                Debug.LogWarning($"Input device not found for player {i + 1}, player actions will not have a device.");
            }
            playerActionsList.Add(actions);
        }
    }
    public InputActions GetPlayerActions(int playerId)
    {
        if (playerId >= 0 && playerId < playerActionsList.Count)
        {
            return playerActionsList[playerId];
        }
        Debug.LogError("No actions found for player ID: " + playerId);
        return null;
    }
    public void Vibrate(float intencity,  InputActions actions, float time)
    {
        actions.Device.Vibrate(intencity, intencity);
        StartCoroutine(OnParticleSystemStopped(actions, time));
    }

    IEnumerator OnParticleSystemStopped(InputActions actions, float time)
    {
        yield return new WaitForSeconds(time);
        actions.Device.StopVibration();
    }

    private float ApplyDeadZone(float value, float deadZone = 0.2f)
    {
        // Check if the input is within the dead zone
        if (Mathf.Abs(value) < deadZone)
        {
            return 0f;
        }

        // Scale the input value from the edge of the dead zone to full input
        return (value - Mathf.Sign(value) * deadZone) / (1f - deadZone);
    }

    private Vector2 MoveValue(InputActions actions)
    {
        // Apply dead zone correctly
        float horizontalValue = ApplyDeadZone(actions.moveAction.Value.x);
        float verticalValue = ApplyDeadZone(actions.moveAction.Value.y);

        Vector2 moveVector = new Vector2(horizontalValue, verticalValue);
        //if (moveVector.sqrMagnitude > 1)
        //    moveVector.Normalize();

        return moveVector;
    }

    private Vector2 LookValue(InputActions actions)
    {
        // Implementation remains the same
        float horizontalValue = ApplyDeadZone(actions.lookAction.Value.x);
        float verticalValue = ApplyDeadZone(actions.lookAction.Value.y);
        Vector2 lookVector = new Vector2(horizontalValue, verticalValue);
        if (lookVector.sqrMagnitude > 1)
            lookVector.Normalize();
        return lookVector;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePlayerActions();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public class InputActions : PlayerActionSet
{
    public Vector2 moveValue; // Add this line
    public Vector2 lookValue; // Add this line

    public PlayerAction menuAction;
    public PlayerAction crowlAction;
    public PlayerAction jumpAction;
    public PlayerAction shootAction;
    public PlayerAction interactionAction;
    public PlayerAction slideAction;
    public PlayerAction runAction;
    public PlayerTwoAxisAction moveAction;
    public PlayerTwoAxisAction lookAction;
    public PlayerAction swapWeaponNextAction;
    public PlayerAction swapWeaponPreviousAction;

    public PlayerAction lookLeftAction;
    public PlayerAction lookRightAction;
    public PlayerAction lookUpAction;
    public PlayerAction lookDownAction;


    public PlayerAction goLeftAction;
    public PlayerAction goRightAction;
    public PlayerAction goUpAction;
    public PlayerAction goDownAction;

    public PlayerAction menuUpAction;
    public PlayerAction menuDownAction;
    public PlayerAction menuEnterAction;

    public InputActions()
    {

        //Menu
        menuAction = CreatePlayerAction("Menu");
        menuUpAction = CreatePlayerAction("Menu Up");
        menuDownAction = CreatePlayerAction("Menu Down");
        menuEnterAction = CreatePlayerAction("Menu Enter");
        //Movement
        goLeftAction = CreatePlayerAction("Go Left");
        goRightAction = CreatePlayerAction("Go Right");
        goUpAction = CreatePlayerAction("Go Up");
        goDownAction = CreatePlayerAction("Go Down");
        jumpAction = CreatePlayerAction("Jump");
        crowlAction = CreatePlayerAction("Crouch");
        interactionAction = CreatePlayerAction("Interaction");
        shootAction = CreatePlayerAction("Shoot");
        slideAction = CreatePlayerAction("Slide");
        runAction = CreatePlayerAction("Run");
        swapWeaponNextAction = CreatePlayerAction("SwapWeaponNext");
        swapWeaponPreviousAction = CreatePlayerAction("SwapWeaponPrevious");
        //Look
        lookLeftAction = CreatePlayerAction("Look Left");
        lookRightAction = CreatePlayerAction("Look Right");
        lookUpAction = CreatePlayerAction("Look Up");
        lookDownAction = CreatePlayerAction("Look Down");


    }

    public static InputActions CreateWithDefaultBindings(float minDeadzone, float maxDeadzone)
    {
        var playerActions = new InputActions();

        playerActions.menuAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Menu").key);
        playerActions.menuAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Menu").inputControlType);

        playerActions.menuUpAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Menu Up").key);
        playerActions.menuUpAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Menu Up").inputControlType);

        playerActions.menuDownAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Menu Down").key);
        playerActions.menuDownAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Menu Down").inputControlType);

        playerActions.menuEnterAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Menu Enter").key);
        playerActions.menuEnterAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Menu Enter").inputControlType);

        playerActions.menuAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Menu").key);
        playerActions.menuAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Menu").inputControlType);

        playerActions.goLeftAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Go Left").key);
        playerActions.goLeftAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Go Left").inputControlType);

        //Movement
        playerActions.goLeftAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Go Left").key);
        playerActions.goLeftAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Go Left").inputControlType);

        playerActions.goRightAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Go Right").key);
        playerActions.goRightAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Go Right").inputControlType);

        playerActions.goUpAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Go Up").key);
        playerActions.goUpAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Go Up").inputControlType);

        playerActions.goDownAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Go Down").key);
        playerActions.goDownAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Go Down").inputControlType);


        //Look
        playerActions.lookLeftAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Look Left").key);
        playerActions.lookLeftAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Look Left").inputControlType);

        playerActions.lookRightAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Look Right").key);
        playerActions.lookRightAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Look Right").inputControlType);

        playerActions.lookUpAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Look Up").key);
        playerActions.lookUpAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Look Up").inputControlType);

        playerActions.lookDownAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Look Down").key);
        playerActions.lookDownAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Look Down").inputControlType);

        playerActions.jumpAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Jump").key);
        playerActions.jumpAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Jump").inputControlType);

        playerActions.crowlAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Crouch").key);
        playerActions.crowlAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Crouch").inputControlType);

        playerActions.interactionAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Interaction").key);
        playerActions.interactionAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Interaction").inputControlType);

        playerActions.shootAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Shoot").key);
        playerActions.shootAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Shoot").inputControlType);

        playerActions.slideAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Slide").key);
        playerActions.slideAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Slide").inputControlType);

        playerActions.runAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Run").key);
        playerActions.runAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("Run").inputControlType);

        playerActions.swapWeaponNextAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("SwapWeaponNext").key);
        playerActions.swapWeaponNextAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("SwapWeaponNext").inputControlType);

        playerActions.swapWeaponPreviousAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("SwapWeaponPrevious").key);
        playerActions.swapWeaponPreviousAction.AddDefaultBinding(InputController.Instance.bindingsScriptable.GetBinding("SwapWeaponPrevious").inputControlType);

        playerActions.moveAction = playerActions.CreateTwoAxisPlayerAction(playerActions.goLeftAction, playerActions.goRightAction, playerActions.goDownAction, playerActions.goUpAction);
        playerActions.lookAction = playerActions.CreateTwoAxisPlayerAction(playerActions.lookLeftAction, playerActions.lookRightAction, playerActions.lookDownAction, playerActions.lookUpAction);
        return playerActions;
    }
}


