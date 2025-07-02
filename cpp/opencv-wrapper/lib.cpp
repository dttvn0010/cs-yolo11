#include <opencv2/opencv.hpp>

#define DLL_EXPORT __declspec(dllexport)

extern "C" DLL_EXPORT cv::Mat* read_image(const char* imgPath)
{
	cv::Mat tmp_image = cv::imread(imgPath);
	cv::Mat* image = new cv::Mat(tmp_image.rows, tmp_image.cols, CV_8UC3);
	memcpy(image->data, tmp_image.data, 3 * image->rows * image->cols);
	return image;
}

extern "C" DLL_EXPORT uint8_t* get_image_data(cv::Mat* image)
{
	return image->data;
}

extern "C" DLL_EXPORT int get_image_width(cv::Mat* image)
{
	return image->cols;
}

extern "C" DLL_EXPORT int get_image_height(cv::Mat* image)
{
	return image->rows;
}

extern "C" DLL_EXPORT cv::Mat* pad_and_resize_image(cv::Mat* input_image, int target_size)
{
	int img_width = input_image->cols;
	int img_height = input_image->rows;

	int img_size = std::max(img_width, img_height);
	int pad_x = (img_size - img_width) / 2;
	int pad_y = (img_size - img_height) / 2;
	auto padded_image = cv::Mat(img_size, img_size, CV_8UC3);
	memset(padded_image.data, 0, 3 * img_size * img_size);

	uint8_t* src = input_image->data;
	uint8_t* dst = padded_image.data;

	for (int i = 0; i < img_height; i++)
	{
		memcpy(dst + (pad_x + (i + pad_y) * img_size) * 3, src + (i * img_width) * 3, 3 * img_width);
	}

	cv::Mat* resized_image = new cv::Mat();
	cv::resize(padded_image, *resized_image, cv::Size(target_size, target_size));
	return resized_image;
}

extern "C" DLL_EXPORT void cvt_bgr_to_rgb(cv::Mat* input_image)
{
	int img_width = input_image->cols;
	int img_height = input_image->rows;
	auto tmp_image = cv::Mat(img_height, img_width, CV_8UC3);
	cv::cvtColor(*input_image, tmp_image, cv::COLOR_BGR2RGB);
	memcpy(input_image->data, tmp_image.data, 3 * img_width * img_height);
}

extern "C" DLL_EXPORT void draw_rect(cv::Mat* input_image, int x1, int y1, int x2, int y2, int r, int g, int b, int thickness)
{
	cv::rectangle(*input_image, cv::Rect(cv::Point2i(x1, y1), cv::Point2i(x2, y2)), cv::Scalar(r, g, b), thickness);
}

extern "C" DLL_EXPORT void show_image(cv::Mat* input_image, int delay)
{
	cv::imshow("Image", *input_image);
	cv::waitKey(delay);
}

extern "C" DLL_EXPORT cv::VideoCapture* open_video(const char* vid_path)
{
	cv::VideoCapture* video_cap = new cv::VideoCapture(vid_path);
	return video_cap;
}

extern "C" DLL_EXPORT void close_video(cv::VideoCapture* video_cap)
{
	delete video_cap;
}

extern "C" DLL_EXPORT cv::Mat* get_next_frame(cv::VideoCapture* video_cap)
{
	if (!video_cap->isOpened())
	{
		return nullptr;
	}

	cv::Mat* frame = new cv::Mat();

	if (video_cap->read(*frame))
	{
		return frame;
	}

	return nullptr;
}

extern "C" DLL_EXPORT void release_image(cv::Mat* frame)
{
	delete frame;
}