using System;
using System.Windows.Forms;

namespace UseArm
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Arm a = new Arm((-1.5f, 1.5f, 3), (-1.5f, 1.5f, 3), (-0.5f, 0.5f, 3), (-0.5f, 0.5f, 3));
            var Vis = new ArmVisualizer(a);
            Vis.Show();
            Application.Run(Vis);
        }
    }
}
