using System;
using System.Windows.Forms;

namespace UseArm
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            /*ArmMotorController a = new ArmMotorController();
            a.MoveMotors();*/

            const double DegToRad = Math.PI / 180.0;
            Arm a = new Arm((0, 0, Math.PI / 2, Math.PI / 2, -4 * Math.PI, 4 * Math.PI, 6.8),
                        (0, 0, -76 * DegToRad, 100 * DegToRad, 0, 0, 28.0),
                        (0, 0, -168.51 * DegToRad, -10 * DegToRad, 0, 0, 28.0),
                        (0, 12.75, -Math.PI / 2, Math.PI / 2));
            var Vis = new ArmVisualizer(a);
            Vis.Show();
            Application.Run(Vis);
        }
    }
}
