using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kitware.VTK;

namespace VTK3D
{
    public partial class SceneVTK : RenderWindowControl
    {
        public SceneVTK()
        {
        }
        ~SceneVTK()
        {

        }

        public void InitializeVTK(System.Windows.Forms.Integration.WindowsFormsHost wfh)
        {
            this.AddTestActors = false;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            wfh.Child = this;
            wfh.Visibility = System.Windows.Visibility.Visible;
            vtkRendererCollection rs = this.RenderWindow.GetRenderers();
            Kitware.VTK.vtkRenderer r = rs.GetFirstRenderer();
            r.SetBackground(0.1, 0.3, 0.7);
            r.SetBackground2(0.7, 0.8, 1.0);
            r.SetGradientBackground(true);

            m_renderer = r;
        }

        public vtkRenderer Renderer()
        {
            return m_renderer;
        }

        //public vtkRenderWindow RenderWindow()
        //{
        //    return m_renderer.GetRenderWindow();
        //}

        private vtkRenderer m_renderer;

    }
}
