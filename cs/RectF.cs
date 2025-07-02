
namespace Yolo11
{
    internal class RectF(float x1, float y1, float x2, float y2, float conf)
    {
        public float X1
        {
            get
            {
                return x1;
            }
        }

        public float Y1
        {
            get
            {
                return y1;
            }
        }

        public float X2
        {
            get
            {
                return x2;
            }
        }

        public float Y2
        {
            get
            {
                return y2;
            }
        }

        public float Conf
        {
            get
            {
                return conf;
            }
        }

        public float Area
        {
            get
            {
                return (x2 - x1) * (y2 - y1);
            }
        }
    }
}
