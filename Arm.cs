using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

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
        private float[] AngleSums;

        private (float, float) TransformPoint(float X, float Y)
        {
            const float SCALE = 10.0f;
            const float CX = 100.0f;
            const float CY = 100.0f;
            return (X * SCALE + CX, 300 - (Y * SCALE + CY));
        }

        public void Draw(Graphics g)
        {


            float PrevX = 0;
            float PrevY = 0;
            float X = 0;
            float Y = 0;
            float AngleSum = 0.0f;
            for (int i = 0; i < CurrentAngles.Length; i++)
            {
                AngleSum += CurrentAngles[i];
                X += (float)(Params[i].Length * Math.Cos(AngleSum));
                Y += (float)(Params[i].Length * Math.Sin(AngleSum));
                var Trans1 = TransformPoint(PrevX, PrevY);
                g.FillEllipse(Brushes.DarkRed, Trans1.Item1 - 3, Trans1.Item2 - 3, 6, 6);
                var Trans2 = TransformPoint(X, Y);
                g.DrawLine(Pens.Red, Trans1.Item1, Trans1.Item2, Trans2.Item1, Trans2.Item2);
                PrevX = X;
                PrevY = Y;
            }
            var Trans = TransformPoint(PrevX, PrevY);
            g.FillEllipse(Brushes.DarkRed, Trans.Item1 - 3, Trans.Item2 - 3, 6, 6);
        }

        private (float X, float Y) ForwardKinematics()
        {
            float X = 0;
            float Y = 0;
            float AngleSum = 0.0f;
            for (int i = 0; i < CurrentAngles.Length; i++)
            {
                AngleSum += CurrentAngles[i];
                AngleSums[i] = AngleSum;
                X += (float)(Params[i].Length * Math.Cos(AngleSum));
                Y += (float)(Params[i].Length * Math.Sin(AngleSum));
            }
            return (X, Y);
        }

        private float CalcGrad(float TX, float TY, float X, float Y, int i)
        {
            float SumX = 0.0f;
            float SumY = 0.0f;
            for (int a = i; a < Params.Length; a++)
            {
                SumX += Params[a].Length * (float)Math.Sin(AngleSums[a]);
                SumY += Params[a].Length * (float)Math.Cos(AngleSums[a]);
            }
            return 2 * SumX * (TX - X) - 2 * SumY * (TY - Y);
        }


        public void MoveTo(float X, float Y)
        {
            //Console.WriteLine($"({X},{Y})");
            const float LearningRate = 0.00001f;
            const int Iterations = 20;
            for (int j = 0; j < CurrentAngles.Length; j++)
            //for (int i = 0; i < Iterations; i++)
            {

                for (int i = 0; i < Iterations; i++)
                {
                    var (CurX, CurY) = ForwardKinematics();
                    float Cost = (CurX - X) * (CurX - X) + (CurY - Y) * (CurY - Y);
                    Console.WriteLine($"Cost: {Cost}");
                    float Grad = CalcGrad(X, Y, CurX, CurY, j) * LearningRate;
                    TempAngles[j] = CurrentAngles[j] - Grad;
                    Console.WriteLine($"Grad: {Grad}");
                    //Console.WriteLine($"TempAngle{j}: {TempAngles[j]}");

                    //TempAngles[j] = Math.Max(Math.Min(Params[j].MaxAngle, TempAngles[j]), Params[j].MinAngle);
                    float[] tmp = CurrentAngles;
                    CurrentAngles = TempAngles;
                    TempAngles = tmp;
                }
                if (TempAngles[j] > Params[j].MaxAngle)
                    TempAngles[j] = Params[j].MaxAngle;
                if (TempAngles[j] < Params[j].MinAngle)
                    TempAngles[j] = Params[j].MinAngle;
            }
            Console.WriteLine();
            foreach (float k in CurrentAngles)
                Console.WriteLine(k);
        }

        public Arm(params ArmPart[] Parameters)
        {
            this.Params = Parameters;
            this.CurrentAngles = new float[Parameters.Length];
            TempAngles = new float[Parameters.Length];
            AngleSums = new float[Parameters.Length];
        }
    }
}
