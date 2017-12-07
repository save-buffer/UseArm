using System;
using System.Diagnostics;

namespace UseArm
{
    public struct ArmPart
    {
        public float MinAngle;
        public float MaxAngle;
        public float Length;

        public ArmPart(float Min, float Max, float L)
        {
            MinAngle = Min;
            MaxAngle = Max;
            Length = L;
        }

        public static implicit operator ArmPart((int, int, int) t)
        {
            return new ArmPart((float)t.Item1, (float)t.Item2, (float)t.Item3);
        }

        public static implicit operator ArmPart((double, double, double) t)
        {
            return new ArmPart((float)t.Item1, (float)t.Item2, (float)t.Item3);
        }

        public static implicit operator ArmPart((float, float, float) t)
        {
            return new ArmPart((float)t.Item1, (float)t.Item2, (float)t.Item3);
        }
    }

    public class Arm
    {
        ArmPart[] Params;
        public float[] CurrentAngles { get; private set; }
        private float[] TempAngles;
        private (float X, float Y) ForwardKinematics()
        {
            float X = 0;
            float Y = 0;
            float AngleSum = 0.0f;
            for (int i = 0; i < CurrentAngles.Length; i++)
            {
                AngleSum += CurrentAngles[i];
                X += (float)(Params[i].Length * Math.Cos(AngleSum));
                Y += (float)(Params[i].Length * Math.Sin(AngleSum));
            }
            return (X, Y);
        }

        public void MoveTo(float X, float Y)
        {
            for (int i = 0; i < 20; i++)
            {
                var (CurX, CurY) = ForwardKinematics();
                float Cost = (CurX - X) * (CurX - X) + (CurY - Y) * (CurY - Y);
                for (int j = 0; j < CurrentAngles.Length; j++)
                {
                    float Grad = 0.1f; //Calculate grad
                    TempAngles[i] = CurrentAngles[i] - Grad;
                }
                float[] tmp = CurrentAngles;
                CurrentAngles = TempAngles;
                TempAngles = tmp;
            }
        }

        public Arm(params ArmPart[] Parameters)
        {
            this.Params = Parameters;
            this.CurrentAngles = new float[Parameters.Length];
            TempAngles = new float[Parameters.Length];
        }
    }
}
