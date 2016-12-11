﻿using System;

public struct ButtonInput
{
	public ButtonInputCommand buttonType;
	public ButtonState buttonState;

	public ButtonInput(ButtonInputCommand buttonType, ButtonState buttonState) {
		this.buttonType = buttonType;
		this.buttonState = buttonState;
	}
}

