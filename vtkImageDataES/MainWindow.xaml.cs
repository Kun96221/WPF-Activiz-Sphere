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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace vtkImageDataES
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Kitware.VTK.RenderWindowControl vtkControl = new Kitware.VTK.RenderWindowControl();

        vtkUnsignedCharArray temp = new vtkUnsignedCharArray();
        vtkImageData testimgdata = new vtkImageData();
        vtkPiecewiseFunction opacityTransferFunction = new vtkPiecewiseFunction();
        vtkColorTransferFunction colorTransferFunction = new vtkColorTransferFunction();
        vtkVolumeProperty volumeProperty = new vtkVolumeProperty();
        vtkVolumeRayCastCompositeFunction compositeFunction = new vtkVolumeRayCastCompositeFunction();
        vtkVolumeRayCastMapper volumeMapper = new vtkVolumeRayCastMapper();
        vtkVolume volume = new vtkVolume();
        vtkCamera aCamera = new vtkCamera();

        public static int imgLine = 512;                                       //图像宽度
        public static int imgPixel = 885;
        public static int imgNum = 128;                                                    //图像张数


        IntPtr temp128 = Marshal.AllocHGlobal(512 * 885 * 128);                                                      //图像高度
        public static byte[] ys = new byte[imgLine * imgPixel * 128];                 //读取数据的数组

        public static byte[] ys1 = new byte[imgLine * imgPixel * 128];
        List<double> RDouble;
        List<double> GDouble;
        List<double> BDouble;

        public MainWindow()
        {
            InitializeComponent();
            #region 读数据
            //FileStream file1 = new FileStream("20191225201705.txt", FileMode.Open);
            FileStream file1 = new FileStream("20200108172426.txt", FileMode.Open);
            file1.Read(ys1, 0, imgNum * imgLine * imgPixel);

            var Rtext = File.ReadLines("R.txt");
            RDouble = Rtext.Select(Convert.ToDouble).ToList();
            var GRtext = File.ReadLines("G.txt");
            GDouble = GRtext.Select(Convert.ToDouble).ToList();
            var Btext = File.ReadLines("B.txt");
            BDouble = Btext.Select(Convert.ToDouble).ToList();

            file1.Close();

            #endregion
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            // 一定要先 vtkformhost.Child = vtkControl;下面三条顺序不能乱 否则报错 切记！
            vtkformhost.Child = vtkControl;
            vtkRenderWindow renWin = vtkControl.RenderWindow;
            vtkRenderer aRenderer = renWin.GetRenderers().GetFirstRenderer();

            renWin.AddRenderer(aRenderer);

            Marshal.Copy(ys1, 0, temp128, imgLine * imgPixel * imgNum);

            
            temp.SetArray(temp128, imgLine * imgPixel * imgNum, 1);
            
            testimgdata.SetDimensions(imgLine, imgPixel, imgNum);
            testimgdata.SetSpacing(1,1,4);
            testimgdata.SetScalarTypeToUnsignedChar();
            testimgdata.SetNumberOfScalarComponents(1);
            testimgdata.AllocateScalars();
            testimgdata.GetPointData().SetScalars(temp);
            testimgdata.Modified();

            VtkSetColor();
            VtkSetOpacity();

            volumeProperty.SetColor(colorTransferFunction);
            volumeProperty.SetScalarOpacity(opacityTransferFunction);
            volumeProperty.ShadeOn();
            volumeProperty.SetInterpolationTypeToLinear();
          
            volumeMapper.SetVolumeRayCastFunction(compositeFunction);
            volumeMapper.SetInput(testimgdata);
                          
            volume.SetMapper(volumeMapper);
            volume.SetProperty(volumeProperty);

            aCamera.SetViewUp(0, -1, 0);
            aCamera.SetPosition(-1, 0, 1);
            aCamera.ComputeViewPlaneNormal();

            aRenderer.AddVolume(volume);
            aRenderer.SetBackground(0, 0, 0.6);
            aRenderer.SetActiveCamera(aCamera);
            aRenderer.ResetCamera();
            renWin.SetSize(800, 800);
            renWin.Render();
            

        }

        private void VtkSetColor()
        {
            for (double i = 0; i < 256; i++)
            {
                colorTransferFunction.AddRGBPoint(i, RDouble[(int)i], GDouble[(int)i], BDouble[(int)i]);
            }
        }

        private void VtkSetOpacity()
        {
            opacityTransferFunction.AddPoint(80, 0);
            opacityTransferFunction.AddPoint(255, 1);
        }

        int build3DViewFull()
        {
            Kitware.VTK.RenderWindowControl rw = new Kitware.VTK.RenderWindowControl();

            vtkRenderWindow _renwin = rw.RenderWindow;
            vtkRenderer _render = _renwin.GetRenderers().GetFirstRenderer();

            _renwin.AddRenderer(_render);

            vtkRenderWindowInteractor iren =new vtkRenderWindowInteractor();
            iren.SetRenderWindow(_renwin);

            // 新建文件读取对象，常见的有vtkBMPReader、vtkDICOMImageReader、vtkJPEGReader等
            vtkJPEGReader jpegReader =new vtkJPEGReader();
            // 不同的reader需要设置的参数是不同的 因此本例仅适合jpegreader
            jpegReader.SetFilePrefix("C:/Users/DawnWind/Desktop/000/"); // 要打开的路径
            jpegReader.SetFilePattern("%s%d.jpg"); // 图片文件名格式，此处为 0.jpg 1.jpg ...
            jpegReader.SetDataByteOrderToLittleEndian();
            jpegReader.SetDataSpacing(1, 1, 1.4);  // 设置图片中像素比，我理解得不清楚，具体请百度之
            jpegReader.SetFileNameSliceSpacing(1);

            jpegReader.SetDataExtent(0, 209, 0, 209, 0, 29);
            // 这里因为在000文件夹里面有0.jpg ~ 29.jpg，所以设置为 0，29
            // 每张图片的长宽为210 * 210 因此设置为0，209

            jpegReader.Update();
            // update这里要注意一下，对于VTK在默认情况下是在最后操作时候才一次性刷新
            // 也就是说如果没有自动刷新的话，在一些中间过程中是无法获得到数据的，因为没update进去

            vtkContourFilter skinExtractor = new vtkContourFilter();

            skinExtractor.SetInputConnection(jpegReader.GetOutputPort());
            skinExtractor.SetValue(200, 100);    //值越大，保留的部分越少。

            //重新计算法向量
            vtkPolyDataNormals skinNormals =new vtkPolyDataNormals();
            skinNormals.SetInputConnection(skinExtractor.GetOutputPort());
            skinNormals.SetFeatureAngle(60.0);
            //Specify the angle that defines a sharp edge. 
            //If the difference in angle across neighboring polygons is greater than this value, 
            //the shared edge is considered "sharp". 


            //create triangle strips and/or poly-lines 为了更快的显示速度
            vtkStripper skinStripper = new vtkStripper();
            skinStripper.SetInputConnection(skinNormals.GetOutputPort());

            vtkPolyDataMapper skinMapper = new vtkPainterPolyDataMapper();

            skinMapper.SetInputConnection(skinStripper.GetOutputPort());
            skinMapper.ScalarVisibilityOff();    //这样不会带颜色



            vtkActor skin = new vtkActor();
            skin.SetMapper(skinMapper);

            // An outline provides context around the data.
            // 一个围绕在物体的立体框，可以先忽略
            /*
            vtkOutlineFilter> outlineData =
                vtkOutlineFilter>::New();
            outlineData.SetInputConnection(dicomReader.GetOutputPort());

            vtkPolyDataMapper> mapOutline =
                vtkPolyDataMapper>::New();
            mapOutline.SetInputConnection(outlineData.GetOutputPort());

            vtkActor> outline =
                vtkActor>::New();
            outline.SetMapper(mapOutline);
            outline.GetProperty().SetColor(0,0,0);

            aRenderer.AddActor(outline);
            */
            // It is convenient to create an initial view of the data. The FocalPoint
            // and Position form a vector direction. Later on (ResetCamera() method)
            // this vector is used to position the camera to look at the data in
            // this direction.
            vtkCamera aCamera = new vtkCamera();
            aCamera.SetViewUp(0, 0, -1);
            aCamera.SetPosition(0, 1, 0);
            aCamera.SetFocalPoint(0, 0, 0);
            aCamera.ComputeViewPlaneNormal();
            aCamera.Azimuth(30.0);
            aCamera.Elevation(30.0);

            // Actors are added to the renderer. An initial camera view is created.
            // The Dolly() method moves the camera towards the FocalPoint,
            // thereby enlarging the image.
            _render.AddActor(skin);
            _render.SetActiveCamera(aCamera);
            _render.ResetCamera();
            aCamera.Dolly(1.5);

            // Set a background color for the renderer and set the size of the
            // render window (expressed in pixels).
            _render.SetBackground(.2, .3, .4);
            _renwin.SetSize(640, 480);

            // Note that when camera movement occurs (as it does in the Dolly()
            // method), the clipping planes often need adjusting. Clipping planes
            // consist of two planes: near and far along the view direction. The 
            // near plane clips out objects in front of the plane; the far plane
            // clips out objects behind the plane. This way only what is drawn
            // between the planes is actually rendered.
            _render.ResetCameraClippingRange();

            // Initialize the event loop and then start it.
            iren.Initialize();
            iren.Start();
            return 0;
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {


        }

        private void Window_Closed(object sender, EventArgs e)
        {
            temp.Dispose();
            testimgdata.Dispose();
            volumeProperty.Dispose();
            opacityTransferFunction.Dispose();
            colorTransferFunction.Dispose();
            compositeFunction.Dispose();
            volumeMapper.Dispose();
            volume.Dispose();
            aCamera.Dispose();
        }
    }
}
