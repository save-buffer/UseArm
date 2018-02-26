using System;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace UseArm
{

    public class ArmVisualizer : Form
    {
        Arm a;
        const float PIXELS_PER_INCH = 5.0f;
        const float INCHES_PER_PIXEL = 1.0f / PIXELS_PER_INCH;

        float tZ = 0.0f;

        private (float, float) ScreenToModel(float X, float Y)
        {
            return ((X - Width / 2) * INCHES_PER_PIXEL, (Height / 2 - Y) * INCHES_PER_PIXEL);
        }

        //Only used for drawing. Scales and flips the point in order to 
        //provide a more human-friendly orientation. 
        private (float, float) ModelToScreen(float X, float Y)
        {
            return ((Width / 2 + X * PIXELS_PER_INCH), (Height / 2 - Y * PIXELS_PER_INCH));
        }

        //Draw is 2D and uses only Pitch. Takes a Graphics object to draw to. 
        public void Draw(Arm a, Graphics g)
        {

            float PrevX = 0.0f;
            float PrevY = 0.0f;
            float PrevZ = 0.0f;
            float X = 0.0f;
            float Y = 0.0f;
            float Z = 0.0f;
            float PitchSum = 0.0f;
            float RollSum = 0.0f;
            float YawSum = 0.0f;
            for (int i = 0; i < a.CurrentPitches.Length; i++)
            {
                RollSum += a.CurrentRolls[i];
                PitchSum += a.CurrentPitches[i];
                YawSum += a.CurrentYaws[i];

                ArmPart[] Params = typeof(Arm).GetField("Params", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as ArmPart[];

                X += (float)(Params[i].Length * Math.Cos(PitchSum) * Math.Cos(YawSum));
                Y += (float)(Params[i].Length * Math.Cos(RollSum) * Math.Sin(PitchSum));
                Z += (float)(Params[i].Length * Math.Cos(RollSum) * Math.Sin(YawSum));

                var Trans1 = ModelToScreen(PrevX, PrevY);
                float ZNorm = Z / 50.0f;
                //Color c = Color.FromArgb((int)(ZNorm * 255), 0, (int)((1 - ZNorm) * 2));
                Color c = Color.Red;
                g.FillEllipse(new SolidBrush(c), Trans1.Item1 - 3, Trans1.Item2 - 3, 6, 6);
                var Trans2 = ModelToScreen(X, Y);
                g.DrawLine(Pens.Blue, Trans1.Item1, Trans1.Item2, Trans2.Item1, Trans2.Item2);
                PrevX = X;
                PrevY = Y;
                PrevZ = Z;
            }

            var Trans = ModelToScreen(PrevX, PrevY);
            g.FillEllipse(Brushes.DarkRed, Trans.Item1 - 3, Trans.Item2 - 3, 6, 6);
        }
        public ArmVisualizer(Arm Arm)
        {
            this.a = Arm;
            this.Visible = true;
            this.Paint += (object sender, PaintEventArgs e) =>
            {
                Draw(a, e.Graphics);
            };

            this.KeyDown += (object sender, KeyEventArgs e) =>
            {
                if (e.KeyCode == Keys.A)
                    tZ += 1.0f;

                else if (e.KeyCode == Keys.D)
                    tZ -= 1.0f;
            };

            this.MouseDown += MoveAndRedraw;
            this.MouseMove += MoveAndRedraw;
        }
        private void MoveAndRedraw(object sender, MouseEventArgs e)
        {
            var (X, Y) = ScreenToModel(e.X, e.Y);
            float Z = tZ;
            //float Z = X * (float)Math.Sin(tZ);
            //X = X * (float)Math.Cos(tZ);
            Console.WriteLine($"Target: ({X}, {Y}, {Z})");
            a.MoveTo(X, Y, Z);
            var (AX, AY, AZ) = a.CalculatePosition();
            Console.WriteLine($"Actual: ({AX}, {AY}, {AZ})");
            Console.WriteLine($"Shoulder: ({a.CurrentRolls[0]}, {a.CurrentPitches[0]}, {a.CurrentYaws[0]})");
            this.Invalidate();
        }
    }
}
