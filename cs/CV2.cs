using System.Runtime.InteropServices;

namespace Yolo11
{
    class CV2
    {
        [DllImport("opencv-wrapper.dll", CharSet = CharSet.Ansi, EntryPoint = "read_image")]
        public static extern IntPtr ReadImage(string imagePath);

        [DllImport("opencv-wrapper.dll", EntryPoint = "get_image_data")]
        public static extern IntPtr GetImageData(IntPtr image);

        [DllImport("opencv-wrapper.dll", EntryPoint = "get_image_width")]
        public static extern int GetImageWidth(IntPtr image);

        [DllImport("opencv-wrapper.dll", EntryPoint = "get_image_height")]
        public static extern int GetImageHeight(IntPtr image);

        [DllImport("opencv-wrapper.dll", EntryPoint = "pad_and_resize_image")]
        public static extern IntPtr PadAndResizeImage(IntPtr image, int targetSize);

        [DllImport("opencv-wrapper.dll", EntryPoint= "cvt_bgr_to_rgb")]
        public static extern IntPtr CvtBGR2RGB(IntPtr image);

        [DllImport("opencv-wrapper.dll", EntryPoint = "draw_rect")]
        public static extern void DrawRect(IntPtr image, int x1, int y1, int x2, int y2, int r, int g, int b, int thickness);

        [DllImport("opencv-wrapper.dll", EntryPoint = "show_image")]
        public static extern void ShowImage(IntPtr image, int delay);

        [DllImport("opencv-wrapper.dll", CharSet = CharSet.Ansi, EntryPoint = "open_video")]
        public static extern IntPtr OpenVideo(string videoPath);

        [DllImport("opencv-wrapper.dll", EntryPoint = "close_video")]
        public static extern void CloseVideo(IntPtr videoCap);

        [DllImport("opencv-wrapper.dll", EntryPoint = "get_next_frame")]
        public static extern IntPtr GetVideoNextFrame(IntPtr videoCap);

        [DllImport("opencv-wrapper.dll", EntryPoint = "release_image")]
        public static extern IntPtr ReleaseImage(IntPtr image);
    }
}
