using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MergeAndSplitTiff.Tests
{
    [TestClass()]
    public class MergeTests
    {
        [TestMethod()]
        public void AppendToTiffTest()
        {
            string sourceFile = @"C:\Users\li\Desktop\新建文件夹\source.tif";
            string targetFile = @"C:\Users\li\Desktop\新建文件夹\target.tif";
            MergeTiff mergeTiff = new MergeTiff();
            DateTime time1 = System.DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                mergeTiff.AppendToTiff(sourceFile, targetFile);
            }
            TimeSpan spendTime1 = System.DateTime.Now - time1;
            string sourceFile2 = @"C:\Users\li\Desktop\新建文件夹\新建文件夹\source.tif";
            string targetFile2 = @"C:\Users\li\Desktop\新建文件夹\新建文件夹\target.tif";
            DateTime time2 = System.DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                mergeTiff.AppendToTiffParallel(sourceFile2, targetFile2);
            }
            TimeSpan spendTime2 = System.DateTime.Now - time2;
        }
    }
}