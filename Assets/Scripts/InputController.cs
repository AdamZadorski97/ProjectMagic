using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using System;

public class InputController : MonoBehaviour
{

    public static InputController Instance { get; private set; }
    private InputActions player1Actions;
    private InputActions player2Actions;
    public InputActions Player1Actions => player1Actions;
    public InputActions Player2Actions => player2Actions;

    public Vector2 player1MoveValue;
    public Vector2 player2MoveValue;
    public Vector2 player3MoveValue;
    public Vector2 player1LookValue;
    public Vector2 player2LookValue;
    public Vector2 player3LookValue;
    [SerializeField] private float minDeadzone = .3f;
    [SerializeField] private float maxDeadzone = 1;
    public InControlManager controlManager;

    private void Update()
    {
        if (player1Actions != null)
        {
            player1MoveValue = MoveValue(player1Actions);
            player1LookValue = LookValue(player1Actions);
        }

        if (player2Actions != null)
        {
            player2MoveValue = MoveValue(player2Actions);
            player2LookValue = LookValue(player2Actions);
        }
    }


    private void InitializePlayerActions()
    {
        player1Actions = InputActions.CreateWithDefaultBindings(minDeadzone, maxDeadzone);
        if (InputManager.Devices.Count > 0)
        {
            player1Actions.Device = InputManager.Devices[0];
        }

        player2Actions = InputActions.CreateWithDefaultBindings(minDeadzone, maxDeadzone);
        if (InputManager.Devices.Count > 1)
        {
            player2Actions.Device = InputManager.Devices[1];
        }
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


    private Vector2 MoveValue(InputActions actions)
    {
        //float horizontalValue = Utility.ApplyDeadZone(actions.moveAction.Value.x, minDeadzone, maxDeadzone) != 0 ? Mathf.Sign(actions.moveAction.Value.x) : 0;
        //float verticalValue = Utility.ApplyDeadZone(actions.moveAction.Value.y, minDeadzone, maxDeadzone) != 0 ? Mathf.Sign(actions.moveAction.Value.y) : 0;
        //return new Vector2(horizontalValue, verticalValue);

        float horizontalValue = Utility.ApplyDeadZone(actions.moveAction.Value.x, minDeadzone, maxDeadzone);
        float verticalValue = Utility.ApplyDeadZone(actions.moveAction.Value.y, minDeadzone, maxDeadzone);

        // Normalize the resultant vector to ensure consistent movement speed in all directions
        Vector2 moveVector = new Vector2(horizontalValue, verticalValue);
        if (moveVector.sqrMagnitude > 1)
        {
            moveVector.Normalize();
        }

        return moveVector;
    }

    private Vector2 LookValue(InputActions actions)
    {
        //float horizontalValue = Utility.ApplyDeadZone(actions.moveAction.Value.x, minDeadzone, maxDeadzone) != 0 ? Mathf.Sign(actions.moveAction.Value.x) : 0;
        //float verticalValue = Utility.ApplyDeadZone(actions.moveAction.Value.y, minDeadzone, maxDeadzone) != 0 ? Mathf.Sign(actions.moveAction.Value.y) : 0;
        //return new Vector2(horizontalValue, verticalValue);

        float horizontalValue = Utility.ApplyDeadZone(actions.lookAction.Value.x, minDeadzone, maxDeadzone);
        float verticalValue = Utility.ApplyDeadZone(actions.lookAction.Value.y, minDeadzone, maxDeadzone);

        // Normalize the resultant vector to ensure consistent movement speed in all directions
        Vector2 lookVector = new Vector2(horizontalValue, verticalValue);
        if (lookVector.sqrMagnitude > 1)
        {
            lookVector.Normalize();
        }

        return lookVector;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }

    private void Start()
    {
        InitializePlayerActions();
    }
}

public class InputActions : PlayerActionSet
{
    public PlayerAction menuAction;
    public PlayerAction crowlAction;
    public PlayerAction jumpAction;
    public PlayerAction shootAction;
    public PlayerAction interactionAction;
    public PlayerTwoAxisAction moveAction;
    public PlayerTwoAxisAction lookAction;

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
        //Look
        lookLeftAction = CreatePlayerAction("Look Left");
        lookRightAction = CreatePlayerAction("Look Right");
        lookUpAction = CreatePlayerAction("Look Up");
        lookDownAction = CreatePlayerAction("Look Down");
    }

    public static InputActions CreateWithDefaultBindings(float minDeadzone, float maxDeadzone)
    {
        var playerActions = new InputActions();
        BindingProperties bindingsScriptable = ScriptableManager.Instance.bindingProperties;

        playerActions.menuAction.AddDefaultBinding(bindingsScriptable.GetBinding("Menu").key);
        playerActions.menuAction.AddDefaultBinding(bindingsScriptable.GetBinding("Menu").inputControlType);

        playerActions.menuUpAction.AddDefaultBinding(bindingsScriptable.GetBinding("Menu Up").key);
        playerActions.menuUpAction.AddDefaultBinding(bindingsScriptable.GetBinding("Menu Up").inputControlType);

        playerActions.menuDownAction.AddDefaultBinding(bindingsScriptable.GetBinding("Menu Down").key);
        playerActions.menuDownAction.AddDefaultBinding(bindingsScriptable.GetBinding("Menu Down").inputControlType);

        playerActions.menuEnterAction.AddDefaultBinding(bindingsScriptable.GetBinding("Menu Enter").key);
        playerActions.menuEnterAction.AddDefaultBinding(bindingsScriptable.GetBinding("Menu Enter").inputControlType);

        playerActions.menuAction.AddDefaultBinding(bindingsScriptable.GetBinding("Menu").key);
        playerActions.menuAction.AddDefaultBinding(bindingsScriptable.GetBinding("Menu").inputControlType);

        playerActions.goLeftAction.AddDefaultBinding(bindingsScriptable.GetBinding("Go Left").key);
        playerActions.goLeftAction.AddDefaultBinding(bindingsScriptable.GetBinding("Go Left").inputControlType);

        //Movement
        playerActions.goLeftAction.AddDefaultBinding(bindingsScriptable.GetBinding("Go Left").key);
        playerActions.goLeftAction.AddDefaultBinding(bindingsScriptable.GetBinding("Go Left").inputControlType);

        playerActions.goRightAction.AddDefaultBinding(bindingsScriptable.GetBinding("Go Right").key);
        playerActions.goRightAction.AddDefaultBinding(bindingsScriptable.GetBinding("Go Right").inputControlType);

        playerActions.goUpAction.AddDefaultBinding(bindingsScriptable.GetBinding("Go Up").key);
        playerActions.goUpAction.AddDefaultBinding(bindingsScriptable.GetBinding("Go Up").inputControlType);

        playerActions.goDownAction.AddDefaultBinding(bindingsScriptable.GetBinding("Go Down").key);
        playerActions.goDownAction.AddDefaultBinding(bindingsScriptable.GetBinding("Go Down").inputControlType);


        //Look
        playerActions.lookLeftAction.AddDefaultBinding(bindingsScriptable.GetBinding("Look Left").key);
        playerActions.lookLeftAction.AddDefaultBinding(bindingsScriptable.GetBinding("Look Left").inputControlType);

        playerActions.lookRightAction.AddDefaultBinding(bindingsScriptable.GetBinding("Look Right").key);
        playerActions.lookRightAction.AddDefaultBinding(bindingsScriptable.GetBinding("Look Right").inputControlType);

        playerActions.lookUpAction.AddDefaultBinding(bindingsScriptable.GetBinding("Look Up").key);
        playerActions.lookUpAction.AddDefaultBinding(bindingsScriptable.GetBinding("Look Up").inputControlType);

        playerActions.lookDownAction.AddDefaultBinding(bindingsScriptable.GetBinding("Look Down").key);
        playerActions.lookDownAction.AddDefaultBinding(bindingsScriptable.GetBinding("Look Down").inputControlType);

        playerActions.jumpAction.AddDefaultBinding(bindingsScriptable.GetBinding("Jump").key);
        playerActions.jumpAction.AddDefaultBinding(bindingsScriptable.GetBinding("Jump").inputControlType);

        playerActions.crowlAction.AddDefaultBinding(bindingsScriptable.GetBinding("Crouch").key);
        playerActions.crowlAction.AddDefaultBinding(bindingsScriptable.GetBinding("Crouch").inputControlType);

        playerActions.interactionAction.AddDefaultBinding(bindingsScriptable.GetBinding("Interaction").key);
        playerActions.interactionAction.AddDefaultBinding(bindingsScriptable.GetBinding("Interaction").inputControlType);

        playerActions.shootAction.AddDefaultBinding(bindingsScriptable.GetBinding("Shoot").key);
        playerActions.shootAction.AddDefaultBinding(bindingsScriptable.GetBinding("Shoot").inputControlType);

        playerActions.moveAction = playerActions.CreateTwoAxisPlayerAction(playerActions.goLeftAction, playerActions.goRightAction, playerActions.goDownAction, playerActions.goUpAction);
        playerActions.lookAction = playerActions.CreateTwoAxisPlayerAction(playerActions.lookLeftAction, playerActions.lookRightAction, playerActions.lookDownAction, playerActions.lookUpAction);
        return playerActions;
    }
}


