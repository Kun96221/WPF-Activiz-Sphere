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

        public MainWindow()
        {
            InitializeComponent();
        }


        private void bulid3d_Click(object sender, RoutedEventArgs e)
        {
            InitializeVTK();

        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            
        }

        //private void InitializeVTK()
        //{
        //    Kitware.VTK.RenderWindowControl vtkControl = new Kitware.VTK.RenderWindowControl();
        //    vtkControl.AddTestActors = false;
        //    vtkControl.Location = new System.Drawing.Point(10, 10);
        //    vtkControl.Name = "_renwin";
        //    vtkControl.Size = new System.Drawing.Size(100, 100);
        //    vtkControl.TabIndex = 0;
        //    vtkControl.TestText = null;
        //    vtkControl.Dock = System.Windows.Forms.DockStyle.Fill;
        //    vtkformhost.Child = vtkControl;
        //    vtkformhost.Visibility = System.Windows.Visibility.Visible;

        //    vtkSphereSource sphere = vtkSphereSource.New();
        //    sphere.SetThetaResolution(8);
        //    sphere.SetPhiResolution(16);

        //    vtkShrinkPolyData shrink = vtkShrinkPolyData.New();
        //    shrink.SetInputConnection(sphere.GetOutputPort());
        //    shrink.SetShrinkFactor(0.9);

        //    vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
        //    mapper.SetInputConnection(shrink.GetOutputPort());

        //    // The actor links the data pipeline to the rendering subsystem
        //    vtkActor actor = vtkActor.New();
        //    actor.SetMapper(mapper);
        //    actor.GetProperty().SetColor(1, 0, 0);

        //    // Create components of the rendering subsystem
        //    //
        //    Kitware.VTK.vtkRendererCollection rs = vtkControl.RenderWindow.GetRenderers();
        //    vtkRenderer ren1 = vtkControl.RenderWindow.GetRenderers().GetFirstRenderer();
        //    vtkRenderWindow renWin = vtkControl.RenderWindow;

        //    // Add the actors to the renderer, set the window size
        //    //
        //    ren1.AddViewProp(actor);
        //    renWin.SetSize(500, 500);
        //    renWin.Render();
        //    vtkCamera camera = ren1.GetActiveCamera();
        //    camera.Zoom(1.5);

        //}


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

            int i, j, k, kOffset, jOffset, offset;
            float s, sp, x, y, z;

            //创建结构化点数据集,（创建点和单元）
            vtkStructuredPoints vol = new vtkStructuredPoints();
            vol.SetDimensions(26, 26, 26);//x,y,z三个坐标轴方向上各有26个点
            vol.SetOrigin(-0.5, -0.5, -0.5);//设置数据集在坐标空间内的起点
            sp = (float)(1.0 / 25.0);
            vol.SetSpacing(sp, sp, sp);//设置坐标轴上每个点的间距

            //创建标量数据（作为结构化点数据集的属性数据）
            vtkFloatArray scalars = new vtkFloatArray();
            scalars.SetNumberOfTuples(26 * 26 * 26);//设置标量个数，因为是点属性所以和点的个数相同

            for (k = 0; k < 26; k++)
            {
                z = (float)(-0.5 + k * sp);
                kOffset = k * 26 * 26;
                for (j = 0; j < 26; j++)
                {
                    y = (float)(-0.5 + j * sp);
                    jOffset = j * 26;
                    for (i = 0; i < 26; i++)
                    {
                        x = (float)(-0.5 + i * sp);
                        s = (float)(x * x + y * y + z * z - (0.4 * 0.4));
                        //计算标量值，该方程为球体方程,位于球体上的点标量值为0
                        offset = i + jOffset + kOffset;//计算id
                        scalars.InsertTuple1(offset, s);//插入标量值
                    }
                }

            }

            vol.GetPointData().SetScalars(scalars); //将标量值与点关联


            //抽取标量值为0的点所形成的面
            vtkContourFilter contour = new vtkContourFilter();
            contour.SetInput(vol);
            contour.SetValue(0, 0);

            vtkPolyDataMapper volmapper = new vtkPainterPolyDataMapper();

            volmapper.SetInput(contour.GetOutput());

            vtkActor actor = new vtkActor();
            actor.GetProperty().SetRepresentationToWireframe();
            // actor.GetProperty().SetRepresentationToSurface();
            // actor.GetProperty().SetRepresentationToPoints();
            actor.GetProperty().SetColor(0, 0, 0);
            actor.SetMapper(volmapper);

            vtkRenderWindow _renwin = rw.RenderWindow;
            vtkRenderer _render = _renwin.GetRenderers().GetFirstRenderer();

            _render.AddActor(actor);

            _renwin.Render();

            _render.ResetCamera();

        }
        private vtkActor CreateSphereActor(double radius)
        {
            Kitware.VTK.vtkActor a = new Kitware.VTK.vtkActor();

            //vtkSphereSource sphereSource3D = new vtkSphereSource();
            //sphereSource3D.SetCenter(0.0, 0.0, 0.0);
            //sphereSource3D.SetRadius(radius);
            //sphereSource3D.SetThetaResolution(10);
            //sphereSource3D.SetPhiResolution(10);

            //vtkPolyDataMapper sphereMapper3D = vtkPolyDataMapper.New();
            //sphereMapper3D.SetInputConnection(sphereSource3D.GetOutputPort());
            //a.SetMapper(sphereMapper3D);
            //a.GetProperty().SetColor(0.95, 0.5, 0.3);
            //a.GetProperty().SetOpacity(0.5);

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
