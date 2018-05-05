using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterInput : MonoBehaviour {

    public enum ButtonInputType {KeyDown, KeyHold, KeyUp };
    public enum MouseInputType {MouseDown, MouseHold, MouseUp };

    [System.Serializable]
    public class InputButton
    {
        public string name;
        public ButtonInputType inputType = ButtonInputType.KeyDown;
        public KeyCode keyCode;
        public UnityEvent inputEvent;

    }

    [System.Serializable]
    public class MouseInput
    {
        public string name;
        public MouseInputType inputType = MouseInputType.MouseDown;
        public int button;
        public UnityEvent inputEvent;
    }


    public InputButton[] inputButtons;
    [Space(5)]
    public MouseInput[] mouseInputs;


    public float HorizontalAxis { get; private set; }
    public float VerticalAxis { get; private set; }

    private bool m_Actived = true;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!m_Actived)
        {
            HorizontalAxis = 0;
            VerticalAxis = 0;
            return;
        }

        HorizontalAxis = Input.GetAxisRaw("Horizontal");
        VerticalAxis = Input.GetAxisRaw("Vertical");

        //Key input
        for (int i=0;i<inputButtons.Length;i++)
        {
            switch (inputButtons[i].inputType)
            {

                case ButtonInputType.KeyDown:
                    {
                        if (Input.GetKeyDown(inputButtons[i].keyCode))
                        {
                            inputButtons[i].inputEvent.Invoke();
                        }

                        break;
                    }
                case ButtonInputType.KeyHold:
                    {
                        if (Input.GetKey(inputButtons[i].keyCode))
                        {
                            inputButtons[i].inputEvent.Invoke();
                        }

                        break;
                    }
                case ButtonInputType.KeyUp:
                    {
                        if (Input.GetKeyUp(inputButtons[i].keyCode))
                        {
                            inputButtons[i].inputEvent.Invoke();
                        }

                        break;
                    }

            }

        }

        //Mouse input
        for (int i = 0; i < mouseInputs.Length; i++)
        {
            switch (mouseInputs[i].inputType)
            {

                case MouseInputType.MouseDown:
                    {
                        if (Input.GetMouseButtonDown(mouseInputs[i].button))
                        {
                            mouseInputs[i].inputEvent.Invoke();
                        }

                        break;
                    }
                case MouseInputType.MouseHold:
                    {
                        if (Input.GetMouseButton(mouseInputs[i].button))
                        {
                            mouseInputs[i].inputEvent.Invoke();
                        }

                        break;
                    }
                case MouseInputType.MouseUp:
                    {
                        if (Input.GetMouseButtonUp(mouseInputs[i].button))
                        {
                            mouseInputs[i].inputEvent.Invoke();
                        }

                        break;
                    }

            }

        }

    }

    public void SetInputActive(bool actived)
    {
        m_Actived = actived;
    }


}
