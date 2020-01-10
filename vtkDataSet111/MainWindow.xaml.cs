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
using System.Runtime.InteropServices;

namespace vtkDataSet111
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

        private void Window_Activated(object sender, EventArgs e)
        {
            vtkPolyData cube = new vtkPolyData();
            vtkPoints points = new vtkPoints();
            vtkCellArray polys = new vtkCellArray();
            vtkFloatArray scalars = new vtkFloatArray();

            Kitware.VTK.RenderWindowControl vtkControl = new Kitware.VTK.RenderWindowControl();
            vtkControl.AddTestActors = false;
            vtkControl.Location = new System.Drawing.Point(10, 10);
            vtkControl.Name = "_renwin";
            vtkControl.Size = new System.Drawing.Size(100, 100);
            vtkControl.TabIndex = 0;
            vtkControl.TestText = null;
            vtkControl.Dock = System.Windows.Forms.DockStyle.Fill;
            vtkformhost.Child = vtkControl;
            vtkformhost.Visibility = System.Windows.Visibility.Visible;


            int i;
            float[][] x = new float[8][]
            {
                new float[]{0,0,0},//第0个点的坐标
                new float[]{1,0,0},//第1个点的坐标
                new float[]{1,1,0},//第2个点的坐标
                new float[]{0,1,0},//3
                new float[]{0,0,1},//4
                new float[]{1,0,1},//5
                new float[]{1,1,1},//6
                new float[]{0,1,1} //7
            };
            for (i = 0; i < 8; i++) points.InsertPoint(i, x[i][0], x[i][1], x[i][2]);//加载点，创建数据结构的几何


            List<int[]> temp = new List<int[]>();

            int[] temparray0 = new int[4] { 0, 1, 2, 3 };//第0，1，2，3个点连接在一起，成为一个单元
            int[] temparray1 = new int[4] { 4, 5, 6, 7 };//第4，5，6，7个点连接在一起，成为一个单元
            int[] temparray2 = new int[4] { 0, 1, 5, 4 };
            int[] temparray3 = new int[4] { 1, 2, 6, 5 };
            int[] temparray4 = new int[4] { 2, 3, 7, 6 };
            int[] temparray5 = new int[4] { 3, 0, 4, 7 };
            temp.Add(temparray0);
            temp.Add(temparray1);
            temp.Add(temparray2);
            temp.Add(temparray3);
            temp.Add(temparray4);
            temp.Add(temparray5);
            //因为在activiz中没有vtkIdType这个类，所以用了其他的方法代替C++代码中的实现。
            for (int j = 0; j < temp.Count; j++)
            {
                IntPtr pP = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * 4);
                Marshal.Copy(temp[j], 0, pP, 4);
                polys.InsertNextCell(4, pP);//加载单元，定义数据集的拓扑
                Marshal.FreeHGlobal(pP);
            }
            for (i = 0; i < 8; i++) scalars.InsertTuple1(i, i);//为每一个点设置点属性。


            cube.SetPoints(points);
            cube.SetPolys(polys);
            cube.GetPointData().SetScalars(scalars);

            vtkPolyDataMapper cubemapper = new vtkPainterPolyDataMapper();
            cubemapper.SetInput(cube);
            cubemapper.SetScalarRange(0, 7);

            vtkActor cubeactor = new vtkActor();
            cubeactor.SetMapper(cubemapper);

            // Create components of the rendering subsystem
            //
            vtkRenderWindow _renwin = vtkControl.RenderWindow;
            vtkRenderer ren1 = _renwin.GetRenderers().GetFirstRenderer();


            // Add the actors to the renderer, set the window size
            //
            ren1.AddViewProp(cubeactor);
            _renwin.SetSize(250, 250);
            _renwin.Render();
            ren1.ResetCamera();

        }
    }
}
