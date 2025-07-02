
namespace Yolo11
{
    class Program
    {
        public static void Main()
        {
            Yolo yolo = new("yolo11s.onnx", 512);
            IntPtr videoCap = CV2.OpenVideo("sample.mp4");
            IntPtr frame;
            while(true)
            {
                frame = CV2.GetVideoNextFrame(videoCap);
                if(frame == IntPtr.Zero)
                {
                    break;
                }
                List<RectF> boxes = yolo.Detect(frame);

                foreach (RectF box in boxes)
                {
                    CV2.DrawRect(frame, (int)box.X1, (int)box.Y1, (int)box.X2, (int)box.Y2, 0, 255, 0, 2);
                }
                CV2.ShowImage(frame, 30);
                CV2.ReleaseImage(frame);
            }
            CV2.CloseVideo(videoCap);
        }
    }
}