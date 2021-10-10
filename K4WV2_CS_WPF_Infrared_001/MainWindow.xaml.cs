﻿using System;
using System.Windows;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace K4WV2_CS_WPF_Infrared_001
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //Kinect SDK
        KinectSensor kinect;

        InfraredFrameReader infraredFrameReader;
        FrameDescription infraredFrameDesc;

        //表示用
        WriteableBitmap infraredBitmap;
        ushort[] infraredBuffer;
        int infraredStride;
        Int32Rect infraredRect;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Kinectを開く
                kinect = KinectSensor.GetDefault();
                kinect.Open();

                //赤外線の情報を取得する
                infraredFrameDesc = kinect.InfraredFrameSource.FrameDescription;

                //赤外線リーダーを開く
                infraredFrameReader = kinect.InfraredFrameSource.OpenReader();
                infraredFrameReader.FrameArrived += infraredFrameReader_FrameArrived;

                //表示のためのビットマップに必要なものを作成
                infraredBuffer = new ushort[infraredFrameDesc.LengthInPixels];
                infraredBitmap = new WriteableBitmap(infraredFrameDesc.Width, infraredFrameDesc.Height, 96, 96, PixelFormats.Gray16, null);
                infraredRect = new Int32Rect(0, 0, infraredFrameDesc.Width, infraredFrameDesc.Height);
                infraredStride = infraredFrameDesc.Width * (int)infraredFrameDesc.BytesPerPixel;

                ImageInfrared.Source = infraredBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void infraredFrameReader_FrameArrived(object sender, InfraredFrameArrivedEventArgs e)
        {
            UpdateInfraredFrame(e);
            DrawInfraredFrame();
        }
        private void UpdateInfraredFrame(InfraredFrameArrivedEventArgs e)
        {
            //カラーフレームを取得する
            using (var infraredFrame=e.FrameReference.AcquireFrame())
            {
                if (infraredFrame==null)
                {
                    return;
                }

                //赤外線データを取得する
                infraredFrame.CopyFrameDataToArray(infraredBuffer);
            }
        }
        private void DrawInfraredFrame()
        {
            //ビットマップにする
            infraredBitmap.WritePixels(infraredRect, infraredBuffer, infraredStride, 0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (infraredFrameReader!=null)
            {
                infraredFrameReader.Dispose();
                infraredFrameReader = null;
            }
            if (kinect!=null)
            {
                kinect.Close();
                kinect = null;
            }
        }
    }
}
