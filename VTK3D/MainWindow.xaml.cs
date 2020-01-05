using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kitware.VTK;

namespace VTK3D
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private vtkActor m_sa;

        public static float[][] x = new float[10][];
        public MainWindow()
        {
            InitializeComponent();
        }

        private void renderWindowControl1_Load(object sender, EventArgs e)
        {
            //// Create a simple sphere. A pipeline is created.
            //vtkSphereSource sphere = vtkSphereSource.New();
            //sphere.SetThetaResolution(8);
            //sphere.SetPhiResolution(16);

            //vtkShrinkPolyData shrink = vtkShrinkPolyData.New();
            //shrink.SetInputConnection(sphere.GetOutputPort());
            //shrink.SetShrinkFactor(0.9);

            //vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            //mapper.SetInputConnection(shrink.GetOutputPort());

            //// The actor links the data pipeline to the rendering subsystem
            //vtkActor actor = vtkActor.New();
            //actor.SetMapper(mapper);
            //actor.GetProperty().SetColor(1, 0, 0);

            //// Create components of the rendering subsystem
            ////
            //vtkRenderer ren1 = VTKControl.RenderWindow.GetRenderers().GetFirstRenderer();
            //vtkRenderWindow renWin = VTKControl.RenderWindow;

            //// Add the actors to the renderer, set the window size
            ////
            //ren1.AddViewProp(actor);
            //renWin.SetSize(250, 250);
            //renWin.Render();
            //vtkCamera camera = ren1.GetActiveCamera();
            //camera.Zoom(1.5);
        }


        private void bulid3d_Click(object sender, RoutedEventArgs e)
        {
            InitializeVTK();

        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            
        }

        private void InitializeVTK()
        {
            Kitware.VTK.RenderWindowControl rw = new Kitware.VTK.RenderWindowControl();
            
            rw.AddTestActors = false;
            rw.Dock = System.Windows.Forms.DockStyle.Fill;
            vtkformhost.Child = rw;
            vtkformhost.Visibility = System.Windows.Visibility.Visible;
            Kitware.VTK.vtkRendererCollection rs = rw.RenderWindow.GetRenderers();
            int rsc = rs.GetNumberOfItems();
            Console.WriteLine(rsc + " renderers");
            Kitware.VTK.vtkRenderer r = rs.GetFirstRenderer();
            r.SetBackground(0.1, 0.3, 0.7);
            r.SetBackground2(0.7, 0.8, 1.0);
            r.SetGradientBackground(true);

            int i, j, k, kOffset, jOffset, Offset;
            float s, sp, x, y, z;
            vtkStructuredPoints vol = new vtkStructuredPoints();
            vol.SetDimensions(26, 26, 26);
            vol.SetOrigin(-0.5, -0.5, -0.5);
            sp = 1.0f / 25.0f;
            vol.SetSpacing(sp,sp,sp);
            vtkFloatArray scalars = new vtkFloatArray();
            scalars.SetNumberOfTuples(26* 26* 26);
            for (k = 0; k < 26; k++)

            {

                z = -0.5f + k * sp;

                kOffset = k * 26 * 26;

                for (j = 0; j < 26; j++)

                {

                    y = -0.5f + j * sp;

                    jOffset = j * 26;

                    for (i = 0; i < 26; i++)

                    {

                        x = -0.5f + i * sp;

                        s = x * x + y * y + z * z - (0.4f * 0.4f);

                        Offset = i + jOffset + kOffset;

                        scalars.InsertTuple1(Offset, s);

                    }

                }

            }

            vol.GetPointData().SetScalars(scalars);
            scalars.FastDelete();



            vtkContourFilter contour = new vtkContourFilter(); ;

            contour.SetInput(vol);

            contour.SetValue(0, 0);


            //vtkPolyDataMapper volMapper = new vtkPolyDataMapper();
            vtkPolyDataMapper volMapper = vtkPolyDataMapper.New();

            volMapper.SetInputConnection(contour.GetOutputPort());

            vtkActor  volActor = new vtkActor();

            volActor.SetMapper(volMapper);

            volActor.GetProperty().SetRepresentationToWireframe();

            volActor.GetProperty().SetColor(0, 0, 0);



            vtkRenderer  renderer = vtkRenderer.New();

            //vtkRenderWindow  renwin =new vtkRenderWindow();

            //renwin.AddRenderer(r);

            //vtkRenderWindowInteractor iren = new vtkRenderWindowInteractor() ;

            //iren.SetRenderWindow(renwin);



            r.AddActor(volActor);

            r.SetBackground(1, 1, 1);

            r.ResetCamera();


            //Kitware.VTK.vtkAxesActor axa = new Kitware.VTK.vtkAxesActor();
            //r.AddActor(axa);

            ////string vtkVersion = Kitware.VTK.vtkVersion.GetVTKVersion();
            ////vtkVersion = "VTK " + vtkVersion;
            ////Console.WriteLine(vtkVersion);

            ////this.label.Content = vtkVersion;

            //axa.SetTotalLength(50.0, 50.0, 50.0);
            //axa.SetConeRadius(0.1);
            ////axa.SetAxisLabels(0);

            //axa.GetXAxisCaptionActor2D().GetTextActor().SetTextScaleMode((int)Kitware.VTK.vtkTextActor.TEXT_SCALE_MODE_NONE_WrapperEnum.TEXT_SCALE_MODE_NONE);
            //axa.GetXAxisCaptionActor2D().GetTextActor().GetTextProperty().SetFontSize(32);
            //axa.GetYAxisCaptionActor2D().GetTextActor().SetTextScaleMode((int)Kitware.VTK.vtkTextActor.TEXT_SCALE_MODE_NONE_WrapperEnum.TEXT_SCALE_MODE_NONE);
            //axa.GetYAxisCaptionActor2D().GetTextActor().GetTextProperty().SetFontSize(32);
            //axa.GetZAxisCaptionActor2D().GetTextActor().SetTextScaleMode((int)Kitware.VTK.vtkTextActor.TEXT_SCALE_MODE_NONE_WrapperEnum.TEXT_SCALE_MODE_NONE);
            //axa.GetZAxisCaptionActor2D().GetTextActor().GetTextProperty().SetFontSize(32);

            //m_sa = this.CreateSphereActor(10.0);
            //r.AddActor(m_sa);

            //m_sa.SetPosition(25.0, 25.0, 25.0);
        }
        private vtkActor CreateSphereActor(double radius)
        {
            Kitware.VTK.vtkActor a = new Kitware.VTK.vtkActor();

            vtkSphereSource sphereSource3D = new vtkSphereSource();
            sphereSource3D.SetCenter(0.0, 0.0, 0.0);
            sphereSource3D.SetRadius(radius);
            sphereSource3D.SetThetaResolution(10);
            sphereSource3D.SetPhiResolution(10);

            vtkPolyDataMapper sphereMapper3D = vtkPolyDataMapper.New();
            sphereMapper3D.SetInputConnection(sphereSource3D.GetOutputPort());
            a.SetMapper(sphereMapper3D);
            a.GetProperty().SetColor(0.95, 0.5, 0.3);
            a.GetProperty().SetOpacity(0.5);

            return a;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
