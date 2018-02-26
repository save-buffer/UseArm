using System;
using System.Threading;
using Scarlet.IO.BeagleBone;
using Scarlet.IO;
using Scarlet.Components;
using Scarlet.Components.Motors;
using Scarlet.Components.Sensors;
using System.Diagnostics;

namespace UseArm
{
    public class ArmMotorController
    {
        const BBBPin ShoulderPin = BBBPin.P9_16;
        const BBBPin ElbowPin = BBBPin.P9_14;
        const BBBPin WristPin1 = BBBPin.P8_13;
        const BBBPin WristPin2 = BBBPin.P8_19;
        const BBBPin WristDirectionPin = BBBPin.P8_08;

        const BBBPin ShoulderPotPin = BBBPin.P9_38;
        const BBBPin ElbowPotPin = BBBPin.P9_40;
        const BBBPin WristPotPin = BBBPin.P9_36;

        Arm a;
        IMotor Shoulder;
        IMotor Elbow;
        IMotor Wrist1;
        IMotor Wrist2;

        ISensor ShoulderPot;
        ISensor ElbowPot;
        ISensor WristPot;

        IPWMOutput ShoulderPWM, ElbowPWM;

        public ArmMotorController()
        {


            Scarlet.Utilities.StateStore.Start("ARM");

            BBBPinManager.AddMappingPWM(ShoulderPin);
            BBBPinManager.AddMappingPWM(ElbowPin);
            BBBPinManager.AddMappingPWM(WristPin1);
            BBBPinManager.AddMappingPWM(WristPin2);
            BBBPinManager.AddMappingGPIO(WristDirectionPin, true, ResistorState.PULL_UP);

            BBBPinManager.AddMappingADC(ShoulderPotPin);
            BBBPinManager.AddMappingADC(ElbowPotPin);
            BBBPinManager.AddMappingADC(WristPotPin);
            BBBPinManager.ApplyPinSettings(BBBPinManager.ApplicationMode.APPLY_IF_NONE);
            BeagleBone.Initialize(SystemMode.DEFAULT, true);

            const double DegToRad = Math.PI / 180.0;
            a = new Arm((0, 0, Math.PI / 2, Math.PI / 2, 0, 0, 6.8),
                        (0, 0, -76 * DegToRad, 100 * DegToRad, 0, 0, 28.0),
                        (0, 0, -168.51 * DegToRad, -10 * DegToRad, 0, 0, 28.0),
                        (-2 * Math.PI, 2 * Math.PI, -Math.PI / 2, Math.PI / 2, 0, 0, 12.75));
            //(0, 12.75, -Math.PI / 2, Math.PI / 2));

            ShoulderPWM = PWMBBB.PWMDevice1.OutputB;
            ElbowPWM = PWMBBB.PWMDevice1.OutputA;
            IPWMOutput WristPWM1 = PWMBBB.PWMDevice2.OutputB;
            IPWMOutput WristPWM2 = PWMBBB.PWMDevice2.OutputA;

            IDigitalOut WristDirectionGPIO = new DigitalOutBBB(WristDirectionPin);
            IAnalogueIn ShoulderADC = new AnalogueInBBB(ShoulderPotPin);
            IAnalogueIn ElbowADC = new AnalogueInBBB(ElbowPotPin);
            IAnalogueIn WristADC = new AnalogueInBBB(WristPotPin);
            Shoulder = new TalonMC(ShoulderPWM, 0.2f);
            Elbow = new TalonMC(ElbowPWM, 0.2f);
            Wrist1 = new CytronMD30C(WristPWM1, WristDirectionGPIO, 0.3f);
            Wrist2 = new CytronMD30C(WristPWM2, WristDirectionGPIO, 0.3f);

            ShoulderPot = new Potentiometer(ShoulderADC, 300);
            ElbowPot = new Potentiometer(ElbowADC, 300);
            WristPot = new Potentiometer(WristADC, 300);

            Wrist1.SetSpeed(0.5f);
            Wrist2.SetSpeed(0.5f);
        }

        public void MoveMotors()
        {
            Shoulder.SetSpeed(0.3f);
            Elbow.SetSpeed(0.3f);
            Thread.Sleep(2000);
            Shoulder.SetSpeed(0);
            ShoulderPWM.SetEnabled(false);
            Elbow.SetSpeed(0);
            ElbowPWM.SetEnabled(false);

            /*Elbow.SetSpeed(0.2f);
            Console.ReadLine();
            Elbow.SetSpeed(0.1f);
            Thread.Sleep(1000);
            Elbow.SetSpeed(0.05f);
            Thread.Sleep(1000);
            Elbow.SetSpeed(0);*/
            //Shoulder.SetSpeed(0);
            /*Wrist1.SetSpeed(0.01f);
            Wrist2.SetSpeed(0.01f);
            Thread.Sleep(500);
            Wrist1.SetSpeed(0);
            Wrist2.SetSpeed(0);*/
        }

        public void MoveTo(float X, float Y, float Z)
        {
            a.MoveTo(X, Y, Z);
        }
    }
}
