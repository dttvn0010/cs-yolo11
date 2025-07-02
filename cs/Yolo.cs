using System.Runtime.InteropServices;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Yolo11
{
    internal class Yolo
    {
        const string INPUT_NAME = "images";
        const float IOU_THRESH = 0.45f;

        int inputSize;
        InferenceSession session;
        byte[] buffer;

        public Yolo(string modelPath, int inputSize)
        {
            session = new InferenceSession(modelPath);
            buffer = new byte[3 * inputSize * inputSize];
            this.inputSize = inputSize;
        }

        private RectF[] Predict(IntPtr image)
        {   
            IntPtr resizedImage = CV2.PadAndResizeImage(image, inputSize);
            CV2.CvtBGR2RGB(resizedImage);
            IntPtr imgData = CV2.GetImageData(resizedImage);
            
            Marshal.Copy(imgData, buffer, 0, buffer.Length);
            var inputTensor = new DenseTensor<byte>(buffer, [1, inputSize, inputSize, 3]);

            var input = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(INPUT_NAME, inputTensor) };
            var output = new List<DisposableNamedOnnxValue>(session.Run(input))[0].AsTensor<float>();
            RectF[] result = new RectF[output.Dimensions[2]];

            for (int i = 0; i < output.Dimensions[2]; i++)
            {
                float x = output[0, 0, i];
                float y = output[0, 1, i];
                float w = output[0, 2, i];
                float h = output[0, 3, i];
                float conf = output[0, 4, i];

                result[i] = new RectF(
                    x - w / 2,
                    y - h / 2,
                    x + w / 2,
                    y + h / 2,
                    conf
                );
            }

            CV2.ReleaseImage(resizedImage);
            return result;
        }

        private List<int> Nms(List<RectF> boxes)
        {
            var sorted = boxes
                .Select((x, i) => new KeyValuePair<RectF, int>(x, i))
                .OrderBy(x => x.Key.Conf)
                .ToList();

            List<int> idxs = sorted.Select(x => x.Value).ToList();
            List<int> pick = [];

            while (idxs.Count > 0)
            {
                int lastIdx = idxs[idxs.Count - 1];
                var box = boxes[lastIdx];
                float x1 = box.X1, y1 = box.Y1, x2 = box.X2, y2 = box.Y2;
                float area = box.Area;

                int bestIdx = lastIdx;
                HashSet<int> groupIdxs = [lastIdx];

                for (int i = 0; i < idxs.Count - 1; i++)
                {
                    int idx = idxs[i];
                    var box2 = boxes[idx];
                    float xx1 = Math.Max(box2.X1, x1);
                    float yy1 = Math.Max(box2.Y1, y1);
                    float xx2 = Math.Min(box2.X2, x2);
                    float yy2 = Math.Min(box2.Y2, y2);
                    float w = Math.Max(0, xx2 - xx1);
                    float h = Math.Max(0, yy2 - yy1);
                    if (w * h / area <= IOU_THRESH) continue;

                    if (boxes[idx].Conf > boxes[bestIdx].Conf)
                    {
                        bestIdx = idx;
                    }
                    groupIdxs.Add(idx);
                }

                pick.Add(bestIdx);
                idxs = idxs.Where(x => !groupIdxs.Contains(x)).ToList();
            }
            return pick;
        }

        public List<RectF> Detect(IntPtr image, float conf=0.35f)
        {
            List<RectF> boxes = Predict(image)
                .ToList()
                .Where(b => b.Conf >= conf)
                .ToList();

            var idxs = Nms(boxes);

            var scaledBoxes = new List<RectF>();
            int imageWidth = CV2.GetImageWidth(image);
            int imageHeight = CV2.GetImageHeight(image);
            int imageSize = Math.Max(imageWidth, imageHeight);
            int dx = (imageSize - imageWidth) / 2;
            int dy = (imageSize - imageHeight) / 2;

            foreach (int idx in idxs)
            {
                var box = boxes[idx];
                float x1 = box.X1 * imageSize / inputSize - dx;
                float y1 = box.Y1 * imageSize / inputSize - dy;
                float x2 = box.X2 * imageSize / inputSize - dx;
                float y2 = box.Y2 * imageSize / inputSize - dy;

                scaledBoxes.Add(new RectF(
                    Math.Max(0, x1),
                    Math.Max(0, y1),
                    Math.Min(x2, imageWidth),
                    Math.Min(y2, imageHeight),
                    box.Conf
                ));
            }

            return scaledBoxes;
        }
    }
}
