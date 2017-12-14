using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using _3DTools;
using System.Diagnostics;

namespace MDLUtility2
{
    /// <summary>
    /// Interaction logic for MDLViewer.xaml
    /// </summary>
    public partial class MDLViewer : UserControl
    {
        MDL.MDLFile _mdlFile;
        List<System.Drawing.Bitmap> _textures = new List<System.Drawing.Bitmap>();
        ModelVisual3D myModel;
        Point oldPoint;
        Matrix3D CurrentMatrix = Matrix3D.Identity;
        Point3D cameraPosition = new Point3D(0, 0, 20);
        Vector3D cameraLookAt = new Vector3D(0, 0, -1000);
        Rect3D _bounds;
        bool? _drawLights = true;
        bool? _drawGarages;
        bool? _drawOther = true;
        bool? _drawBackground = true;
        bool _loaded;
        public MDLViewer()
        {
            InitializeComponent();
            ImageBrush myIb = new ImageBrush(loadBitmap(Properties.Resources.stars_big));
            myGrid.Background = myIb;

            chkLights.IsChecked = _drawLights;
            chkGarages.IsChecked = _drawGarages;
            chkOther.IsChecked = _drawOther;
            chkBackground.IsChecked = _drawBackground;
            _loaded = true;
        }
        public void LoadMDLFile(MDL.MDLFile currentBinaryMDL, List<System.Drawing.Bitmap> referencedImages)
        {
            _textures = referencedImages;
            LoadMDLFile(currentBinaryMDL);
        }

        public void LoadMDLFile(MDL.MDLFile mdlfile)
        {
            CurrentMatrix = Matrix3D.Identity;
            _mdlFile = mdlfile;
            ClearViewport();
            SetCamera();
            myModel = ConvertMDL(_mdlFile);
            this.mainViewport.Children.Add(myModel);
            DefaultRotate();
        }
        public void ReloadCurrent()
        {
            LoadMDLFile(_mdlFile);
        }
        private ModelVisual3D ConvertMDL(MDL.MDLFile _mdlFile)
        {
            ModelVisual3D newmodel = new ModelVisual3D();
            Model3DGroup model = new Model3DGroup();

            List<MDL.MDLObject> meshes = new List<MDL.MDLObject>();

            if (_mdlFile.RootObject.Count(x => x.type == MDL.MDLType.mdl_group) > 0)
            {
                meshes = _mdlFile.RootObject.Where(x => x.type == MDL.MDLType.mdl_group).First().childrens.Where(x => x.type == MDL.MDLType.mdl_mesh).ToList();
            }
            else
            {
                meshes = _mdlFile.RootObject.Where(x => x.type == MDL.MDLType.mdl_mesh).ToList();
            }

            IOrderedEnumerable<MDL.MDLObject> orderedMeshes = meshes.OrderBy(x => -x.mesh.nfaces);
            for (int itemindex = 0; itemindex < orderedMeshes.Count(); itemindex++)
            {
                MDL.MDLObject item = meshes[itemindex];
                 if (item.lodval < 1 || orderedMeshes.Count() == 1 )
                {
                    MeshGeometry3D mesh = new MeshGeometry3D();
                    for (int i = 0; i < item.mesh.vertices.Count(); i++)
                    {
                        mesh.Positions.Add(new Point3D(item.mesh.vertices[i].x, item.mesh.vertices[i].y, item.mesh.vertices[i].z));
                        mesh.TextureCoordinates.Add(new Point(item.mesh.vertices[i].mx, item.mesh.vertices[i].my));
                        Vector3D normal = new Vector3D(item.mesh.vertices[i].nx, item.mesh.vertices[i].ny, item.mesh.vertices[i].nz);
                        mesh.Normals.Add(normal);
                    }
                    _bounds = mesh.Bounds;
                    for (int i = item.mesh.faces.Count() - 1; i > -1; i--)
                    {
                        mesh.TriangleIndices.Add(item.mesh.faces[i]);
                    }

                    if (_textures.Count > 0 && item.textidx > -1)
                    {
                        ImageBrush ib = new ImageBrush();
                        int textureIndex = item.textidx;
                        if (textureIndex < 0)
                            textureIndex = 0;
                        ib.ImageSource = loadBitmap(_textures[textureIndex]);
                        Material diffuse1 = new DiffuseMaterial(ib);
                        Material spec1 = new SpecularMaterial(ib, 40);
                        MaterialGroup matGroup = new MaterialGroup();
                        matGroup.Children.Add(diffuse1);
                        matGroup.Children.Add(spec1);
                        GeometryModel3D gmodel = new GeometryModel3D(mesh, matGroup);

                        model.Children.Add(gmodel);
                    }
                    else
                    {
                        Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
                        GeometryModel3D gmodel = new GeometryModel3D(mesh, material);

                        model.Children.Add(gmodel);
                    }
                }
            }

            //foreach (var item in meshes.Where(x => x.lodval < 1
            //    //&& x.textidx > -1
            //    ).OrderBy(x => -x.mesh.nfaces))//.Select(x=>x)
            //{
                
            //}
            if(_drawLights != null && _drawLights == true)
                AddLights(_mdlFile, model);
            // we control the display of hardpoints inside this method..
                AddHardPoints(_mdlFile, model);


            newmodel.Content = model;
            return newmodel;
        }

        private void AddHardPoints(MDL.MDLFile _mdlFile, Model3DGroup model)
        {
            if (_mdlFile.FrameDatas != null)
            {
                foreach (MDL.MDLFrameData frame in _mdlFile.FrameDatas)
                {
                    if (frame.name.Contains("capgarage") && _drawGarages !=null && _drawGarages ==true )
                    {
                        MeshGeometry3D newFrame = newPlane(new Vector3D(frame.nx, frame.ny, frame.nz));
                        SolidColorBrush scb = new SolidColorBrush(Colors.Green);
                        SolidColorBrush scbb = new SolidColorBrush(Colors.Red);
                        EmissiveMaterial front = new EmissiveMaterial(scb);
                        EmissiveMaterial back = new EmissiveMaterial(scbb);
                        GeometryModel3D gm3d = new GeometryModel3D(newFrame, front);
                        gm3d.BackMaterial = front;
                        gm3d.BackMaterial = back;
                        Transform3DGroup tg = new Transform3DGroup();
                        tg.Children.Add(new ScaleTransform3D(new Vector3D(4, 4, 4)));
                        tg.Children.Add(new TranslateTransform3D(frame.posx, frame.posy, frame.posz));
                        gm3d.Transform = tg;
                        model.Children.Add(gm3d);
                    }
                    else if (frame.name.Contains("garage") && _drawGarages != null && _drawGarages == true)
                    {
                        MeshGeometry3D newFrame = newPlane(new Vector3D(frame.nx, frame.ny, frame.nz));
                        SolidColorBrush scb = new SolidColorBrush(Colors.Green);
                        SolidColorBrush scbb = new SolidColorBrush(Colors.Red);
                        EmissiveMaterial front = new EmissiveMaterial(scb);
                        EmissiveMaterial back = new EmissiveMaterial(scbb);
                        GeometryModel3D gm3d = new GeometryModel3D(newFrame, front);
                        gm3d.BackMaterial = back;
                        Transform3DGroup tg = new Transform3DGroup();
                        tg.Children.Add(new ScaleTransform3D(new Vector3D(3, 3, 3)));
                        tg.Children.Add(new TranslateTransform3D(frame.posx, frame.posy, frame.posz));
                        gm3d.Transform = tg;

                        model.Children.Add(gm3d);
                    }
                    else if (_drawOther != null && _drawOther == true)
                    {
                        // lets just add a little sprite type thing.
                        MeshGeometry3D hardPoint = newSprite();
                        ImageBrush ib = new ImageBrush();
                        System.Drawing.Bitmap myBitmap = Properties.Resources.donut;
                        myBitmap.MakeTransparent(System.Drawing.Color.Black);
                        ib.ImageSource = loadBitmap(myBitmap);
                        EmissiveMaterial m = new EmissiveMaterial(ib);
                        Color myColor = new Color();
                        myColor.ScA = 1;
                        myColor.ScR = 0.4f;
                        myColor.ScG = 0.7f;
                        myColor.ScB = 0.4f;
                        m.Color = myColor;

                        MaterialGroup mg = new MaterialGroup();
                        mg.Children.Add(m);

                        GeometryModel3D hardPointModel = new GeometryModel3D(hardPoint, mg);
                        hardPointModel.BackMaterial = mg;
                        // Now we need to position it ...

                        Transform3DGroup tg = new Transform3DGroup();
                        ScaleTransform3D sc = new ScaleTransform3D(0.5f, 0.5f, 0.5f);
                        tg.Children.Add(sc);
                        tg.Children.Add(new TranslateTransform3D(frame.posx, frame.posy, frame.posz));
                        hardPointModel.Transform = tg;

                        model.Children.Add(hardPointModel);
                    }
                }
            }
        }

        private void AddLights(MDL.MDLFile _mdlFile, Model3DGroup model)
        {
            if (_mdlFile.Lights != null)
            {
                foreach (MDL.MDLLight light in _mdlFile.Lights)
                {
                    MeshGeometry3D newLight = newSprite();
                    ImageBrush ib = new ImageBrush();
                    System.Drawing.Bitmap myBitmap = Properties.Resources.f101bmp;
                    myBitmap.MakeTransparent(System.Drawing.Color.Black);
                    ib.ImageSource = loadBitmap(myBitmap);
                    EmissiveMaterial m = new EmissiveMaterial(ib);
                    Color myColor = new Color();
                    myColor.ScA = 1;
                    myColor.ScR = light.red;
                    myColor.ScG = light.green;
                    myColor.ScB = light.blue;
                    m.Color = myColor;

                    MaterialGroup mg = new MaterialGroup();
                    mg.Children.Add(m);

                    GeometryModel3D lightModel = new GeometryModel3D(newLight, mg);
                    lightModel.BackMaterial = mg;
                    // Now we need to position it ...

                    Transform3DGroup tg = new Transform3DGroup();
                    ScaleTransform3D sc = new ScaleTransform3D(0.1f, 0.1f, 0.1f);
                    tg.Children.Add(sc);
                    tg.Children.Add(new TranslateTransform3D(light.posx, light.posy, light.posz));
                    lightModel.Transform = tg;

                    AnimateLight(light, sc);

                    model.Children.Add(lightModel);
                }
            }
        }

        private static void AnimateLight(MDL.MDLLight light, ScaleTransform3D sc)
        {
            float scale = 2;
            DoubleAnimation da1 = new DoubleAnimation(0.001f, 0.6f, TimeSpan.FromMilliseconds(light.todo1 * 500));
            DoubleAnimation da2 = new DoubleAnimation(0.001f, 0.6f, TimeSpan.FromMilliseconds(light.todo1 * 500));
            DoubleAnimation da3 = new DoubleAnimation(0.001f, 0.6f, TimeSpan.FromMilliseconds(light.todo1 * 500));
            da1.AccelerationRatio = light.todo3 * scale;
            da2.AccelerationRatio = light.todo3 * scale;
            da3.AccelerationRatio = light.todo3 * scale;
            da1.DecelerationRatio = light.todo5 * scale;
            da2.DecelerationRatio = light.todo5 * scale;
            da3.DecelerationRatio = light.todo5 * scale;
            da1.AutoReverse = true;
            da2.AutoReverse = true;
            da3.AutoReverse = true;
            da1.RepeatBehavior = RepeatBehavior.Forever;
            da2.RepeatBehavior = RepeatBehavior.Forever;
            da3.RepeatBehavior = RepeatBehavior.Forever;
            da1.BeginTime = TimeSpan.FromMilliseconds(light.todo2);
            sc.BeginAnimation(ScaleTransform3D.ScaleXProperty, da1);
            sc.BeginAnimation(ScaleTransform3D.ScaleYProperty, da2);
            sc.BeginAnimation(ScaleTransform3D.ScaleZProperty, da3);
        }
        private MeshGeometry3D newPlane(Vector3D n)
        {
            n.Normalize();
            
            Vector3D v1;
            Vector3D v2;
            Vector3D v3;
            Vector3D v4;
            bool windUp = false;
            if (Math.Abs(n.Z) > Math.Abs(n.X) && Math.Abs(n.Z) > Math.Abs(n.Y))
            {
                v1 = Vector3D.CrossProduct(n, new Vector3D(-1, 1, 0)); // top left
                v2 = Vector3D.CrossProduct(n, new Vector3D(1, 1, 0)); // top right
                v3 = Vector3D.CrossProduct(n, new Vector3D(-1, -1, 0)); // bottom left
                v4 = Vector3D.CrossProduct(n, new Vector3D(1, -1, 0)); // bottom right
                windUp = n.Z <0;
            }
            else if (Math.Abs(n.Y) > Math.Abs(n.X) && Math.Abs(n.Y) > Math.Abs(n.Z))
            {
                v1 = Vector3D.CrossProduct(n, new Vector3D(-1,0, 1)); // top left
                v2 = Vector3D.CrossProduct(n, new Vector3D(1,0, 1)); // top right
                v3 = Vector3D.CrossProduct(n, new Vector3D(-1,0, -1)); // bottom left
                v4 = Vector3D.CrossProduct(n, new Vector3D(1,0, -1)); // bottom right
                windUp = n.Y>0;
            }
            else
            {
                v1 = Vector3D.CrossProduct(n, new Vector3D(0,-1, 1)); // top left
                v2 = Vector3D.CrossProduct(n, new Vector3D(0,1, 1)); // top right
                v3 = Vector3D.CrossProduct(n, new Vector3D(0,-1, -1)); // bottom left
                v4 = Vector3D.CrossProduct(n, new Vector3D(0,1, -1)); // bottom right
                windUp = n.X <0;
            }

            MeshGeometry3D triangleMesh = new MeshGeometry3D();
            Point3D point0 = new Point3D(v1.X, v1.Y, v1.Z); // top left
            Point3D point1 = new Point3D(v2.X, v2.Y, v2.Z); // top right
            Point3D point2 = new Point3D(v3.X, v3.Y, v3.Z); // bottom left
            Point3D point3 = new Point3D(v4.X, v4.Y, v4.Z); // bottom right
            triangleMesh.Positions.Add(point0);
            triangleMesh.Positions.Add(point1);
            triangleMesh.Positions.Add(point2);
            triangleMesh.Positions.Add(point3);
            if (windUp)
            {
                triangleMesh.TriangleIndices.Add(0);
                triangleMesh.TriangleIndices.Add(2);
                triangleMesh.TriangleIndices.Add(1);
                triangleMesh.TriangleIndices.Add(2);
                triangleMesh.TriangleIndices.Add(3);
                triangleMesh.TriangleIndices.Add(1);
            }
            else
            {
                triangleMesh.TriangleIndices.Add(1);                
                triangleMesh.TriangleIndices.Add(2);
                triangleMesh.TriangleIndices.Add(0);
                triangleMesh.TriangleIndices.Add(1);
                triangleMesh.TriangleIndices.Add(3);
                triangleMesh.TriangleIndices.Add(2);
            }
            // we need the visible normal for lighting purpose.
            triangleMesh.Normals.Add(n);
            triangleMesh.Normals.Add(n);
            triangleMesh.Normals.Add(n);
            triangleMesh.Normals.Add(n);
            triangleMesh.Normals.Add(n);
            triangleMesh.Normals.Add(n);

            Point[] tPoints = new Point[4];
            tPoints[0] = new Point(0, 0);
            tPoints[1] = new Point(1, 0);
            tPoints[2] = new Point(0, 1);
            tPoints[3] = new Point(1, 1);

            triangleMesh.TextureCoordinates = new PointCollection(tPoints);
            return triangleMesh;
        }
        private MeshGeometry3D newSprite()
        {
            MeshGeometry3D triangleMesh = new MeshGeometry3D();
            Point3D point0 = new Point3D(-.5, -.5, 0);
            Point3D point1 = new Point3D(.5, -.5, 0);
            Point3D point2 = new Point3D(-.5, .5, 0);
            Point3D point3 = new Point3D(.5, .5, 0);
            triangleMesh.Positions.Add(point0);
            triangleMesh.Positions.Add(point1);
            triangleMesh.Positions.Add(point2);
            triangleMesh.Positions.Add(point3);
            triangleMesh.TriangleIndices.Add(0);
            triangleMesh.TriangleIndices.Add(2);
            triangleMesh.TriangleIndices.Add(1);
            triangleMesh.TriangleIndices.Add(2);
            triangleMesh.TriangleIndices.Add(3);
            triangleMesh.TriangleIndices.Add(1);
            Vector3D normal = new Vector3D(0, 0, 1);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);


            Point3D point00 = new Point3D(-.5, 0, -.5);
            Point3D point10 = new Point3D(.5, 0, -.5);
            Point3D point20 = new Point3D(-.5, 0, .5);
            Point3D point30 = new Point3D(.5, 0, .5);
            triangleMesh.Positions.Add(point00);
            triangleMesh.Positions.Add(point10);
            triangleMesh.Positions.Add(point20);
            triangleMesh.Positions.Add(point30);
            triangleMesh.TriangleIndices.Add(4);
            triangleMesh.TriangleIndices.Add(6);
            triangleMesh.TriangleIndices.Add(5);
            triangleMesh.TriangleIndices.Add(6);
            triangleMesh.TriangleIndices.Add(7);
            triangleMesh.TriangleIndices.Add(5);
            normal = new Vector3D(0, 1, 0);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);




            Point3D point01 = new Point3D(0, -.5, -.5);
            Point3D point11 = new Point3D(0, .5, -.5);
            Point3D point21 = new Point3D(0, -.5, .5);
            Point3D point31 = new Point3D(0, .5, .5);
            triangleMesh.Positions.Add(point01);
            triangleMesh.Positions.Add(point11);
            triangleMesh.Positions.Add(point21);
            triangleMesh.Positions.Add(point31);
            triangleMesh.TriangleIndices.Add(8);
            triangleMesh.TriangleIndices.Add(10);
            triangleMesh.TriangleIndices.Add(9);
            triangleMesh.TriangleIndices.Add(10);
            triangleMesh.TriangleIndices.Add(11);
            triangleMesh.TriangleIndices.Add(9);
            normal = new Vector3D(1, 0, 0);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);
            triangleMesh.Normals.Add(normal);





            Point[] tPoints = new Point[12];
            tPoints[0] = new Point(0, 0);
            tPoints[1] = new Point(1, 0);
            tPoints[2] = new Point(0, 1);
            tPoints[3] = new Point(1, 1);
            tPoints[4] = new Point(0, 0);
            tPoints[5] = new Point(1, 0);
            tPoints[6] = new Point(0, 1);
            tPoints[7] = new Point(1, 1);
            tPoints[8] = new Point(0, 0);
            tPoints[9] = new Point(1, 0);
            tPoints[10] = new Point(0, 1);
            tPoints[11] = new Point(1, 1);

            triangleMesh.TextureCoordinates = new PointCollection(tPoints);
            return triangleMesh;
        }


        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }
        private Model3DGroup BuildNormals(Point3D p0, Point3D p1, Point3D p2, Vector3D normal)
        {
            Model3DGroup normalGroup = new Model3DGroup();
            Point3D p;
            ScreenSpaceLines3D normal0Wire = new ScreenSpaceLines3D();
            ScreenSpaceLines3D normal1Wire = new ScreenSpaceLines3D();
            ScreenSpaceLines3D normal2Wire = new ScreenSpaceLines3D();
            Color c = Colors.Blue;
            int width = 1;
            normal0Wire.Thickness = width;
            normal0Wire.Color = c;
            normal1Wire.Thickness = width;
            normal1Wire.Color = c;
            normal2Wire.Thickness = width;
            normal2Wire.Color = c;
            double num = 1;
            double mult = .01;
            double denom = mult * Convert.ToDouble(2);
            double factor = num / denom;
            p = Vector3D.Add(Vector3D.Divide(normal, factor), p0);
            normal0Wire.Points.Add(p0);
            normal0Wire.Points.Add(p);
            p = Vector3D.Add(Vector3D.Divide(normal, factor), p1);
            normal1Wire.Points.Add(p1);
            normal1Wire.Points.Add(p);
            p = Vector3D.Add(Vector3D.Divide(normal, factor), p2);
            normal2Wire.Points.Add(p2);
            normal2Wire.Points.Add(p);

            //Normal wires are not models, so we can't
            //add them to the normal group.  Just add them
            //to the viewport for now...
            this.mainViewport.Children.Add(normal0Wire);
            this.mainViewport.Children.Add(normal1Wire);
            this.mainViewport.Children.Add(normal2Wire);

            return normalGroup;
        }

        private void transformView(Point3D Difference, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
                cameraPosition.X -= Difference.X * 0.05;
                cameraPosition.Y += Difference.Y * 0.05;
                camera.Position = cameraPosition;
                // Use . Value...
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                CurrentMatrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), Difference.X * 0.5f));
                CurrentMatrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), Difference.Y * 0.5f));
                myModel.Transform = new MatrixTransform3D(CurrentMatrix);
            }
            // myModel.Transform.Transform( = new System.Windows.Media.Media3D.RotateTransform3D(
            //this.mainViewport.Children.Add(myModel);
        }
        private void DefaultRotate()
        {
            CurrentMatrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), -25));
            CurrentMatrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), 25));
            // now lets scale the bugger to a default size.
            double maxSize = Math.Abs( _bounds.SizeX) + Math.Abs(_bounds.X);
            //if (maxSize > 1)
           // {
            if (Math.Abs(_bounds.SizeY) + Math.Abs(_bounds.Y) > maxSize) maxSize = Math.Abs(_bounds.SizeY) + Math.Abs(_bounds.Y);
            if (Math.Abs(_bounds.SizeZ) + Math.Abs(_bounds.Z) > maxSize) maxSize = Math.Abs(_bounds.SizeZ) + Math.Abs(_bounds.Z);
                // now we want to so some math so that we could make the 
                // max size into 25, no matter what it is.
                // so we need to find what percent MaxSize is of 25
               // double percent = 15 / maxSize;
                // so we need to sake it by percent.
            maxSize *= 2;
            if (maxSize > 2)
            {
                cameraPosition.Z = maxSize;
            }
            else
            {
                cameraPosition.Z = 4;
            }
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            camera.Position = cameraPosition;

            cameraLookAt.X = (_bounds.SizeX + _bounds.X)/2;
            cameraLookAt.X = (_bounds.SizeY + _bounds.Y)/2;
            cameraLookAt.X = (_bounds.SizeZ + _bounds.Z) / 2;

            camera.LookDirection = cameraLookAt;
                //CurrentMatrix.Scale(new Vector3D(percent, percent, percent));
            //}
            myModel.Transform = new MatrixTransform3D(CurrentMatrix);
        }

        private void SetCamera()
        {
            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;

            camera.Position = cameraPosition;
            camera.LookDirection = cameraLookAt;
        }
        private void ClearViewport()
        {
            ModelVisual3D m;
            for (int i = mainViewport.Children.Count - 1; i >= 0; i--)
            {
                m = (ModelVisual3D)mainViewport.Children[i];
                if (m != lightsModel && m.IsAncestorOf(lightsModel) == false)
                    mainViewport.Children.Remove(m);
            }
        }

        private void mainViewport_MouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Move");
            Point newPoint = e.GetPosition(sender as IInputElement);
            if (oldPoint.X != 0 && oldPoint.Y != 0)
            {
                Point Difference = newPoint;
                Difference.Offset(-oldPoint.X, -oldPoint.Y);
                oldPoint = newPoint;
                transformView(new Point3D(Difference.X, Difference.Y, 0), e);
            }
            else
            {
                oldPoint = newPoint;
            }

        }

        private void mainViewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // lets move the camera to and From the center point.

            PerspectiveCamera camera = (PerspectiveCamera)mainViewport.Camera;
            cameraPosition.Z += e.Delta * 0.01;

            camera.Position = cameraPosition;
            // Use . Value...

        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsFocused == false)
                this.Focus();
        }

        private void btnToolBox_Checked(object sender, RoutedEventArgs e)
        {
            scrollToolBox.Visibility = Visibility.Visible;
        }

        private void btnToolBox_Unchecked(object sender, RoutedEventArgs e)
        {
            scrollToolBox.Visibility = Visibility.Hidden;
        }

        private void chkLights_Checked(object sender, RoutedEventArgs e)
        {
            if (_loaded)
            {
                _drawLights = chkLights.IsChecked;
                ReloadCurrent();
            }
        }

        private void chkGarages_Checked(object sender, RoutedEventArgs e)
        {
            if (_loaded)
            {
                _drawGarages = chkGarages.IsChecked;
                ReloadCurrent();
            }
        }

        private void chkOther_Checked(object sender, RoutedEventArgs e)
        {
            if (_loaded)
            {
                _drawOther = chkOther.IsChecked;
                ReloadCurrent();
            }
        }

        private void chkBackground_Checked(object sender, RoutedEventArgs e)
        {
            if (_loaded)
            {
                _drawBackground = chkBackground.IsChecked;
                if (_drawBackground != null && _drawBackground == true)
                {
                    ImageBrush myIb = new ImageBrush(loadBitmap(Properties.Resources.stars_big));
                    myGrid.Background = myIb;
                }
                else
                {
                    myGrid.Background = Brushes.Black;
                }
            }
        }
    }
}
