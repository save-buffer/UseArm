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
            float PrevX = 0;
            float PrevY = 0;
            float X = 0;
            float Y = 0;
            float AngleSum = 0.0f;
            for (int i = 0; i < a.CurrentPitches.Length; i++)
            {
                ArmPart[] Params = typeof(Arm).GetField("Params", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(a) as ArmPart[];
                AngleSum += a.CurrentPitches[i];
                X += (float)(Params[i].Length * Math.Cos(AngleSum));
                Y += (float)(Params[i].Length * Math.Sin(AngleSum));
                var Trans1 = ModelToScreen(PrevX, PrevY);
                g.FillEllipse(Brushes.DarkRed, Trans1.Item1 - 3, Trans1.Item2 - 3, 6, 6);
                var Trans2 = ModelToScreen(X, Y);
                g.DrawLine(Pens.Red, Trans1.Item1, Trans1.Item2, Trans2.Item1, Trans2.Item2);
                PrevX = X;
                PrevY = Y;
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

            this.MouseDown += MoveAndRedraw;
            this.MouseMove += MoveAndRedraw;
        }
        private void MoveAndRedraw(object sender, MouseEventArgs e)
        {
            var (X, Y) = ScreenToModel(e.X, e.Y);
            a.MoveTo(X, Y, 0);
            this.Invalidate();
        }
    }
}
