using System;
using System.Windows.Forms;

namespace UseArm
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Arm a = new Arm((0, 0, -1.5f, 1.5f, 0, 0, 3), (0.78f, 3), (0, 0, -0.5f, 0.5f, 0, 0, 3), (0, 3));
            var Vis = new ArmVisualizer(a);
            Vis.Show();
            Application.Run(Vis);
        }
    }
}
