using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private float deadZone = 0.2f;

    private GameManager _gameManager;

    private Dictionary<int, bool> pressedAction;
    private Dictionary<int, bool> pressedJump;
    
    public void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;
        pressedAction = new Dictionary<int, bool>();
        pressedJump = new Dictionary<int, bool>();
        for (int i = 0; i < _gameManager.PlayerCount; i++)
        {
            pressedAction.Add(i, false);
            pressedJump.Add(i, false);
        }
    }

    void Update()
    {
        if (_gameManager.IsPlaying) {
            // requires you to set up axes "Joy0X" - "Joy3X" and "Joy0Y" - "Joy3Y" in the Input Manger

            var wasPlayer0Checked = false;
            
            for (int i = 1; i < _gameManager.PlayerCount; i++)
            {
                string joystickName = "Joy" + (i + 1);
                if (Mathf.Abs(Input.GetAxis(joystickName + "X")) > deadZone ||
                    Mathf.Abs(Input.GetAxis(joystickName + "Y")) > deadZone) {
                    _gameManager.MovePlayer(i, new Vector2(Input.GetAxis(joystickName + "X"), -Input.GetAxis(joystickName + "Y")));
                }

                if (Input.GetAxis(joystickName + "Fire") > 0&& !pressedAction[i]) {
                    _gameManager.ActionPlayer(i);
                    pressedAction[i] = true;
                } else if (pressedAction[i] && Input.GetAxis(joystickName + "Fire") <= 0) {
                    pressedAction[i] = false;
                }
                
                if (Input.GetAxis(joystickName + "Jump") > 0 && !pressedJump[i]) {
                    _gameManager.JumpPlayer(i);
                    pressedJump[i] = true;
                } else if (pressedJump[i] && Input.GetAxis(joystickName + "Jump") <= 0 ) {
                    pressedJump[i] = false;
                }
            }


            if (Input.GetJoystickNames().Length == 0) {
                if (Mathf.Abs(Input.GetAxis("Horizontal")) > deadZone || Mathf.Abs(Input.GetAxis("Vertical")) > deadZone) {
                    _gameManager.MovePlayer(0, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
                }
                if (Input.GetAxis("Fire1") > 0 && !pressedAction[0]) {
                    _gameManager.ActionPlayer(0);
                    pressedAction[0] = true;
                } else if (pressedAction[0] && Input.GetAxis("Fire1") <= 0) {
                    pressedAction[0] = false;
                }
                
                if (Input.GetAxis("Jump") > 0 && !pressedJump[0]) {
                    _gameManager.JumpPlayer(0);
                    pressedJump[0] = true;
                } else if (pressedJump[0] && Input.GetAxis("Jump") <= 0 ) {
                    pressedJump[0] = false;
                }
            }
        }
    }

}