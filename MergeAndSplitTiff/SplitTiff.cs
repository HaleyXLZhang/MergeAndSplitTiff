using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace MergeAndSplitTiff
{
   public  class SplitTiff
    {
        /// <summary>  
        /// 将给定文件  分割成多个tif文件 到临时目录下  
        /// </summary>  
        /// <param name="targetFile">目标文件</param>  
        /// <param name="tempDirectory">临时目录路径，删除用</param>  
        /// <returns>分割后多个文件路径集合</returns>  
        public  ArrayList SplitTif(string targetFile, out string tempDirectory)
        {
            ArrayList list = new ArrayList();
            using (Image img = Image.FromFile(targetFile))
            {
                Guid guid = img.FrameDimensionsList[0];
                System.Drawing.Imaging.FrameDimension dimension = new System.Drawing.Imaging.FrameDimension(guid);
                int nTotFrame = img.GetFrameCount(dimension); //tif总页数  
                int nLoop = 0; //索引  
                //生成临时目录 存放 单tif页  
                tempDirectory = Path.Combine(Path.GetDirectoryName(targetFile), Guid.NewGuid().ToString());
                if (!Directory.Exists(tempDirectory))
                {
                    Directory.CreateDirectory(tempDirectory);
                }
                EncoderParameters ep = new EncoderParameters(2);
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                ep.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionCCITT3);//压缩方式  CompressionCCITT3主要用于传真  
                TiffHelper tiffHelper = new TiffHelper();
                ImageCodecInfo info = tiffHelper.GetEncoderInfo("image/tiff");
                for (nLoop = 0; nLoop < nTotFrame; nLoop++)
                {
                    img.SelectActiveFrame(dimension, nLoop);
                    //保存 单tif页  
                    string newfilePath = Path.Combine(tempDirectory, nLoop.ToString() + ".tif");

                    img.Save(newfilePath, info, ep);
                    //将路径存入 list中  
                    list.Add(newfilePath);
                }
            }
            return list;
        }

    }
}
