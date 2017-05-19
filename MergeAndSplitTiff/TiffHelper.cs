using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace MergeAndSplitTiff
{
   public class TiffHelper
    {
        /// <summary>  
        /// 获取图像页数  
        /// </summary>  
        /// <param name="imagePath"></param>  
        /// <returns></returns>  
        public  int GetPageNumber(string imagePath)
        {
            using (Image image = Bitmap.FromFile(imagePath))
            {
                Guid objGuid = image.FrameDimensionsList[0];
                FrameDimension objDimension = new FrameDimension(objGuid);

                return image.GetFrameCount(objDimension);
            }
        }
        /// <summary>  
        /// 获取支持的编码信息  
        /// </summary>  
        /// <param name="mimeType">协议描述</param>  
        /// <returns>图像编码信息</returns>  
        public  ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; j++)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            throw new Exception(mimeType + " mime type not found in ImageCodecInfo");
        }

    }
}
