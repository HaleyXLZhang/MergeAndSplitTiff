using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace MergeAndSplitTiff
{
    public class MergeTiff
    {
        /// <summary>  
        /// 拼接两个tif文件 保存到文件2中  
        /// </summary>  
        /// <param name="filePath">tif文件1</param>  
        /// <param name="targetFile">tif文件2</param>  
        public void AppendToTiff(string filePath, string targetFile)
        {
            ArrayList list = new ArrayList(); //保存所有 tif文件路径  
            string tempDirectory1 = string.Empty;
            string tempDirectory2 = string.Empty;
            #region 分割tif文件1  
            SplitTiff splitTiff = new SplitTiff();
            list.AddRange(splitTiff.SplitTif(filePath, out tempDirectory1));
            #endregion
            #region 分割tif文件2  
            list.AddRange(splitTiff.SplitTif(targetFile, out tempDirectory2));
            #endregion
            //2. 拼接所有tif页  
            //2.1 删除原目标文件  
            File.Delete(targetFile);
            //2.2 拼接 并按原路径生成tif文件  
            MergeTiffImages(list, targetFile, EncoderValue.CompressionCCITT3);
            //3. 删除临时目录  
            DirectoryInfo di2 = new DirectoryInfo(tempDirectory2);
            di2.Delete(true);
            DirectoryInfo di1 = new DirectoryInfo(tempDirectory1);
            di1.Delete(true);
        }
        
        /// <summary>  
        /// 拼接两个tif文件 保存到文件2中  
        /// </summary>  
        /// <param name="filePath">tif文件1</param>  
        /// <param name="targetFile">tif文件2</param>  
        public void AppendToTiffParallel(string filePath, string targetFile)
        {
            ArrayList list = new ArrayList(); //保存所有 tif文件路径  
            ArrayList list1 = new ArrayList();
            ArrayList list2 = new ArrayList();
            string tempDirectory1 = string.Empty;
            string tempDirectory2 = string.Empty;

            Func<string, ArrayList> splitSourceFile = (s) =>
            {
                ArrayList temList = new ArrayList(); //保存所有 tif文件路径  
                #region 分割tif文件1  
                SplitTiff splitTiff = new SplitTiff();
                temList.AddRange(splitTiff.SplitTif(s, out tempDirectory1));
                #endregion
                return temList;
            };
            Func<string, ArrayList> splitTargetFile = (t) =>
            {
                ArrayList temList = new ArrayList(); //保存所有 tif文件路径  
                #region 分割tif文件2  
                SplitTiff splitTiff = new SplitTiff();
                temList.AddRange(splitTiff.SplitTif(t, out tempDirectory2));
                #endregion
                return temList;
            };
            Task t1 = Task.Factory.StartNew(() =>
            {
                list1 = splitSourceFile(filePath);
            });
            Task t2 = Task.Factory.StartNew(() =>
            {
                list2 = splitTargetFile(targetFile);
            });
            Task.WaitAll(t1, t2);
            list.AddRange(list1);
            list.AddRange(list2);
            //2. 拼接所有tif页  
            //2.1 删除原目标文件  
            File.Delete(targetFile);
            //2.2 拼接 并按原路径生成tif文件  
            MergeTiffImages(list, targetFile, EncoderValue.CompressionCCITT3);
            //3. 删除临时目录  
            DirectoryInfo di2 = new DirectoryInfo(tempDirectory2);
            di2.Delete(true);
            DirectoryInfo di1 = new DirectoryInfo(tempDirectory1);
            di1.Delete(true);
        }
        /// <summary>  
        /// 将给定的文件 拼接输出到指定的tif文件路径  
        /// </summary>  
        /// <param name="imageFiles">文件路径列表</param>  
        /// <param name="outFile">拼接后保存的 tif文件路径</param>  
        /// <param name="compressEncoder">压缩方式</param>  
        public void MergeTiffImages(ArrayList imageFiles, string outFile, EncoderValue compressEncoder)
        {
            //如果只有一个文件，直接复制到目标  
            if (imageFiles.Count == 1)
            {
                File.Copy((string)imageFiles[0], outFile, true);
                return;
            }
            System.Drawing.Imaging.Encoder enc = System.Drawing.Imaging.Encoder.SaveFlag;
            EncoderParameters ep = new EncoderParameters(2);
            ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.MultiFrame);
            ep.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)compressEncoder);

            Bitmap pages = null;
            int frame = 0;
            TiffHelper tiffHelper = new TiffHelper();
            ImageCodecInfo info = tiffHelper.GetEncoderInfo("image/tiff");
            foreach (string strImageFile in imageFiles)
            {
                if (frame == 0)
                {
                    pages = (Bitmap)Image.FromFile(strImageFile);
                    //保存第一个tif文件 到目标处  
                    pages.Save(outFile, info, ep);
                }
                else
                {
                    //保存好第一个tif文件后，其余 设置为添加一帧到 图像中  
                    ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.FrameDimensionPage);
                    Bitmap bm = (Bitmap)Image.FromFile(strImageFile);
                    pages.SaveAdd(bm, ep);
                    bm.Dispose();
                }
                if (frame == imageFiles.Count - 1)
                {
                    //flush and close.  
                    ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.Flush);
                    pages.SaveAdd(ep);
                }
                frame++;
            }
            pages.Dispose(); //释放资源  
            return;
        }
    }
}
