/*
 * This code is based on code that was found on http://visualcsharp.webs.com/projectdownloads.htm
 * Since this code was not found with a license, it is subject
 * to the ZomB Dashboard System's license
 * Original source can be found at http://www.mediafire.com/file/ihhjztqmudy/Xbox360Controller.zip
 **/
/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2009-2010, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using SlimDX;
using SlimDX.XInput;

namespace System451.Communication.Dashboard.Libs.Xbox360Controller
{
    public class GamepadState
    {
        uint lastPacket;

        public GamepadState(int userIndex)
        {
            UserIndex = (UserIndex)userIndex;
            Controller = new Controller(UserIndex);
        }

        public readonly UserIndex UserIndex;
        public readonly Controller Controller;

        public DPadState DPad { get; private set; }

        public bool A { get; private set; }
        public bool B { get; private set; }
        public bool X { get; private set; }
        public bool Y { get; private set; }

        public bool RightShoulder { get; private set; }
        public bool LeftShoulder { get; private set; }

        public bool Start { get; private set; }
        public bool Back { get; private set; }

        public double RightTrigger { get; private set; }
        public double LeftTrigger { get; private set; }

        public double RightX { get; private set; }
        public double LeftX { get; private set; }
        public double RightY { get; private set; }
        public double LeftY { get; private set; }

        public bool Connected
        {
            get { return Controller.IsConnected; }
        }

        public void Vibrate(float leftMotor, float rightMotor)
        {
            // If not connected, dont vibrate
            if (!Connected) return;
            Controller.SetVibration(new Vibration
            {
                LeftMotorSpeed = (ushort)(MathHelper.Saturate(leftMotor) * ushort.MaxValue),
                RightMotorSpeed = (ushort)(MathHelper.Saturate(rightMotor) * ushort.MaxValue)
            });
        }

        public void Update()
        {
            // If not connected, nothing to update
            if (!Connected) return;

            // If same packet, nothing to update
            State state = Controller.GetState();
            if (lastPacket == state.PacketNumber) return;
            lastPacket = state.PacketNumber;

            var gamepadState = state.Gamepad;

            // Shoulders
            LeftShoulder = (gamepadState.Buttons & GamepadButtonFlags.LeftShoulder) != 0;
            RightShoulder = (gamepadState.Buttons & GamepadButtonFlags.RightShoulder) != 0;

            // Triggers
            LeftTrigger = gamepadState.LeftTrigger / (double)byte.MaxValue;
            RightTrigger = gamepadState.RightTrigger / (double)byte.MaxValue;

            // Buttons
            Start = (gamepadState.Buttons & GamepadButtonFlags.Start) != 0;
            Back = (gamepadState.Buttons & GamepadButtonFlags.Back) != 0;

            A = (gamepadState.Buttons & GamepadButtonFlags.A) != 0;
            B = (gamepadState.Buttons & GamepadButtonFlags.B) != 0;
            X = (gamepadState.Buttons & GamepadButtonFlags.X) != 0;
            Y = (gamepadState.Buttons & GamepadButtonFlags.Y) != 0;

            // D-Pad
            DPad = new DPadState((gamepadState.Buttons & GamepadButtonFlags.DPadUp) != 0,
                                 (gamepadState.Buttons & GamepadButtonFlags.DPadDown) != 0,
                                 (gamepadState.Buttons & GamepadButtonFlags.DPadLeft) != 0,
                                 (gamepadState.Buttons & GamepadButtonFlags.DPadRight) != 0);

            LeftX = gamepadState.LeftThumbX/ (double)short.MaxValue;
            LeftY = gamepadState.LeftThumbY / (double)short.MaxValue;
            RightX = gamepadState.RightThumbX / (double)short.MaxValue;
            RightX = gamepadState.RightThumbY / (double)short.MaxValue;
        }

        static Vector2 Normalize(short rawX, short rawY, short threshold)
        {
            var value = new Vector2(rawX, rawY);
            var magnitude = value.Length();
            var direction = value / (magnitude == 0 ? 1 : magnitude);

            var normalizedMagnitude = 0.0f;
            if (magnitude - threshold > 0)
                normalizedMagnitude = Math.Min((magnitude - threshold) / (short.MaxValue - threshold), 1);

            return direction * normalizedMagnitude;
        }

        public struct DPadState
        {
            public readonly bool Up, Down, Left, Right;

            public DPadState(bool up, bool down, bool left, bool right)
            {
                Up = up; Down = down; Left = left; Right = right;
            }
        }

        public struct ThumbstickState
        {
            public readonly Vector2 Position;
            public readonly bool Clicked;

            public ThumbstickState(Vector2 position, bool clicked)
            {
                Clicked = clicked;
                Position = position;
            }
        }
    }

    public static class MathHelper
    {
        public static float Saturate(float value)
        {
            return value < 0 ? 0 : value > 1 ? 1 : value;
        }
    }
}