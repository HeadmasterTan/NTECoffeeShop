using System.Drawing;
using System.Linq;
using NLog;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace NTECoffeeShop.CoffeeShop
{
    internal class CharacterHandler
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 寻找源图上的人物
        /// templatePath必须是透明背景人物图的地址路径。
        /// </summary>
        /// <param name="sceneImg"></param>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        public static Point2f? FindCharacter(Bitmap sceneImg, string templatePath)
        {
            Mat templateImg = Cv2.ImRead(templatePath, ImreadModes.Unchanged);

            // 检查是否有 4 个通道（ARGB/BGRA），如果没有，说明不是透明图
            if (templateImg.Channels() != 4)
            {
                string msg = "【FindCharacter】模板图必须是带透明通道的 PNG 图片！";
                Log.Error(msg);
                throw new System.Exception(msg);
            }

            // 分离通道，提取出 Alpha 通道作为掩码 (Mask)
            Mat[] channels = Cv2.Split(templateImg);
            Mat alphaMask = channels[3]; // 第 4 个通道是透明度
            Mat sceneMat = sceneImg.ToMat();

            ORB orb = ORB.Create(nFeatures: 500); // 限制提取 500 个点即可，防止过拟合
            var keypoints1 = new KeyPoint[0];
            var keypoints2 = new KeyPoint[0];
            Mat descriptors1 = new Mat();
            Mat descriptors2 = new Mat();

            // 如果是透明图，传入 alphaMask，强制算法无视透明背景
            orb.DetectAndCompute(templateImg, alphaMask, out keypoints1, descriptors1);
            orb.DetectAndCompute(sceneMat, null, out keypoints2, descriptors2);

            alphaMask?.Dispose();

            if (keypoints1.Length == 0 || descriptors1.Empty() || descriptors2.Empty()) return null;

            // 特征点匹配
            var matcher = new BFMatcher(NormTypes.Hamming, crossCheck: true);
            var matches = matcher.Match(descriptors1, descriptors2);

            // 严格筛选匹配项 (阈值控制在 40-50 之间)
            var goodMatches = matches.Where(m => m.Distance < 45).ToArray();

            // 如果连基本的匹配数量都达不到，直接判定没有人物
            if (goodMatches.Length < 10) return null;

            // RANSAC 空间几何校验（防止散点误报）
            var srcPts = goodMatches.Select(m => keypoints1[m.QueryIdx].Pt).ToArray();
            var dstPts = goodMatches.Select(m => keypoints2[m.TrainIdx].Pt).ToArray();

            // 转换为 OpenCV 需要的 InputArray 格式
            var srcMat = Mat.FromArray(srcPts);
            var dstMat = Mat.FromArray(dstPts);
            var inliersMask = new Mat();

            // 使用 RANSAC 算法寻找单应性矩阵，剔除随机误报点
            Cv2.FindHomography(srcMat, dstMat, HomographyMethods.Ransac, 5.0, inliersMask);

            // 计算真正符合图形结构的点（内点 Inliers）的数量
            int trueInliersCount = 0;
            for (int i = 0; i < inliersMask.Rows; i++)
            {
                if (inliersMask.At<byte>(i, 0) == 1) trueInliersCount++;
            }

            // 判定只有当能够拼成正确人形的特征点大于一定数量时，才认为找到了
            if (trueInliersCount >= 8)
            {
                // 只计算通过了 RANSAC 校验的正确点的平均坐标
                float sumX = 0, sumY = 0;
                int count = 0;
                for (int i = 0; i < inliersMask.Rows; i++)
                {
                    if (inliersMask.At<byte>(i, 0) == 1)
                    {
                        sumX += dstPts[i].X;
                        sumY += dstPts[i].Y;
                        count++;
                    }
                }
                return new Point2f(sumX / count, sumY / count);
            }

            // 释放通道资源
            foreach (var ch in channels) ch.Dispose();
            templateImg?.Dispose();
            alphaMask?.Dispose();
            sceneMat?.Dispose();
            descriptors1?.Dispose();
            descriptors2?.Dispose();

            return null;
        }
    }
}
