using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO;

[StructLayout(LayoutKind.Sequential)]
public struct FaceRect
{
    public int nLeft;
    public int nTop;
    public int nRight;
    public int nBottom;
    public int nAge;       // 年龄
    public int nGender;	// 0:出错，1：男，2：女
};

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FaceInfos
{
    public int nCount;
    public int nMax;
    public FaceRect* pFace;
    public void* pTemData;
};

[StructLayout(LayoutKind.Sequential)]
public unsafe struct DataBaseItem
{
    public int nId;
    public char* pFeature;
};


namespace FaceCard
{
    public partial class Form1 : Form
    {
        // 加载dll函数
        /**
	    * @brief      获取机器码，把机器码发给厂商注册后获得注册码
	    * @param[in]  pBuf		存放机器码的buf，大小为64字节
	    * @return     失败返回-1， 成功返回0
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cGetMachineCode", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cGetMachineCode(char* pBuf);
        /**
	    * @brief      检查授权，未授权时能用管理员身份试用60天
	    * @param[in]  pMachineSN	从厂商获得的注册码
	    * @return     失败返回负数， 成功返回0
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cMachineAuthorize", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cMachineAuthorize(char* pMachineSN);
        /**
	    * @brief      创建人脸识别对象
	    * @param[in]  nFlag		初始化标志，0：只加载人脸比对模块，1：加载人脸比对和性别年龄检测模块
	    * @param[in]  nCam		要打开的摄像头序号，-1不打开
	    * @return     失败返回NULL， 成功返回人脸识别对象
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cCreateFaceWorker", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void* cCreateFaceWorker(int nFlag, int nCam);
        /**
	    * @brief      获取摄像头的帧。注意不要释放返回的内存空间
	    * @param[in]  pWorker		人脸识别对象
	    * @return     失败返回NULL， 成功返回图像帧，实际是cv::IplImage*
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cGetFrame", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void* cGetFrame(void* pWorker);
        /**
        * @brief      读取照片,用完需要cReleaseImage释放内存
        * @param[in]  file		照片路径
        * @return     失败返回NULL， 成功返回图像帧，实际是cv::IplImage*
        */
        [DllImport("libSharpiFace.dll", EntryPoint = "cReadImage", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void* cReadImage(char* file);
        /**
	    * @brief      把帧保存为图片
	    * @param[in]  file		照片路径
	    * @param[in]  pFrame		图像帧，实际是cv::IplImage*
	    * @return     失败返回-1， 成功返回0
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cSaveImage", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cSaveImage(char* file, void* pFrame);
        /**
        * @brief      释放cReadImage返回的内存空间
        * @param[in]  pFrame		图像帧，实际是cv::IplImage*
        * @return     void
        */
        [DllImport("libSharpiFace.dll", EntryPoint = "cReleaseImage", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void cReleaseImage(void* pImage);
        /**
	    * @brief      图像格式转换，cv::IplImage*转HBITMAP。注意需要cReleasehBitmap释放返回的内存空间
	    * @param[in]  pFrame		图像帧，实际是cv::IplImage*
	    * @return     失败返回NULL， 成功返回HBITMAP
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cIplImage2hBitmap", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void* cIplImage2hBitmap(void* pImg);
        /**
	    * @brief      释放cIplImage2hBitmap返回的内存空间
	    * @param[in]  hBitmap		图像hBitmap
	    * @return     void
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cReleasehBitmap", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern void cReleasehBitmap(void* hBitmap);
        /**
	    * @brief      人脸检测
	    * @param[in]  pWorker	人脸识别对象
	    * @param[in]  pFrame	图像帧，实际是cv::IplImage*
	    * @param[in]  pFaces	用于存放检测结果
	    * @return     失败返回-1， 成功返回0
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cFaceDetect", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cFaceDetect(void* pWorker, void* pFram, FaceInfos* pFaces);
        /**
	    * @brief      年龄检测，需先进行人脸检测
	    * @param[in]  pWorker	人脸识别对象
	    * @param[in]  pFaces	人脸检测时的pFaces，同时也保存结果在FaceRect.nAge
	    * @return     失败返回-1， 成功返回0
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cAgeDetect", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cAgeDetect(void* pWorker, FaceInfos* pFaces);
        /**
	    * @brief      性别检测，需先进行人脸检测
	    * @param[in]  pWorker	人脸识别对象
	    * @param[in]  pFaces	人脸检测时的pFaces，同时也保存结果在FaceRect.nGender
	    * @return     失败返回-1， 成功返回0
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cGenderDetect", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cGenderDetect(void* pWorker, FaceInfos* pFaces);
        /**
	    * @brief      获取特征值大小
	    * @param[in]  pWorker		人脸识别对象
	    * @return     失败返回-1， 成功返回特征值大小
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cGetFeatureSize", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cGetFeatureSize(void* pWorker);
        /**
	    * @brief      提取第nIndex张脸特征值，需先进行人脸检测
	    * @param[in]  pWorker	人脸识别对象
	    * @param[in]  pFaces	人脸检测时的pFaces
	    * @param[in]  nIndex	第nIndex张脸
	    * @param[in]  pFeature	存放特征值的内存空间，应预先申请，大小为cGetFeatureSize()
	    * @return     失败返回-1， 成功返回0
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cFeatureExtract", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cFeatureExtract(void* pWorker, FaceInfos* pFaces, int nIndex, void* pFeature);
        /**
	    * @brief      提取帧中最大脸的特征值
	    * @param[in]  pWorker	人脸识别对象
	    * @param[in]  pFrame	帧
	    * @param[in]  pFeature	存放特征值的内存空间，应预先申请，大小为cGetFeatureSize()
	    * @return     失败返回-1， 成功返回脸数
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cFeatureExtractF", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cFeatureExtractF(void* pWorker, void* pFrame, void* pFeature);
        /**
	    * @brief      提取图片中最大脸的特征值
	    * @param[in]  pWorker	人脸识别对象
	    * @param[in]  file		照片
	    * @param[in]  pFeature	存放特征值的内存空间，应预先申请，大小为cGetFeatureSize()
	    * @return     失败返回-1， 成功返回脸数
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cFeatureExtractP", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cFeatureExtractP(void* pWorker, char* file, void* pFeature);
        /**
	    * @brief      特征值比对
	    * @param[in]  pWorker		人脸识别对象
	    * @param[in]  pFeature1		待比对特征值1
	    * @param[in]  pFeature2		待比对特征值2
	    * @return     失败返回-1， 成功返回相似度（0.0 - 1.0）
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cFeatureCompare", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern float cFeatureCompare(void* pWorker, void* pFeature1, void* pFeature2);
        /**
	    * @brief      比对两帧中的最大脸
	    * @param[in]  pWorker		人脸识别对象
	    * @param[in]  pFrame1		照片1
	    * @param[in]  pFrame2		照片2
	    * @return     失败返回-1， 成功返回相似度（0.0 - 1.0）
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cFrameCompare", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern float cFrameCompare(void* pWorker, void* pFrame1, void* pFrame2);
        /**
	    * @brief      比对两张图片中的最大脸
	    * @param[in]  pWorker		人脸识别对象
	    * @param[in]  file1		照片1
	    * @param[in]  file2		照片2
	    * @return     失败返回-1， 成功返回相似度（0.0 - 1.0）
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cPictureCompare", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern float cPictureCompare(void* pWorker, char* file1, char* file2);
        /**
	    * @brief      释放人脸
	    * @param[in]  pWorker	人脸识别对象
	    * @param[in]  pFaces	人脸检测时的pFaces
	    * @return     失败返回-1， 成功返回0
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cFaceRelease", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cFaceRelease(void* pWorker, FaceInfos* pFaces);
        /**
	    * @brief      反初始化SDK
	    * @param[in]  pWorker			人脸识别对象
	    * @return     失败返回-1， 成功返回0
	    */
        [DllImport("libSharpiFace.dll", EntryPoint = "cReleaseFaceWorker", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        unsafe public static extern int cReleaseFaceWorker(void* pWorker);

        public static int m_nTime = 40;
        public static float m_fVal = 0.5F;  // 阈值，取值0~1，大于该值则认为是同一个人
        public static string m_strPic1 = "face1.jpg";   // 图片1
        public static string m_strPic2 = "face2.jpg";    // 图片2
        public static string m_strACode = "4d25f257f03fe1a0fee7407cf08b019b"; // 授权码，把机器码发给我授权生成

        public Form1()
        {
            InitializeComponent();
            //指定不再捕获对错误线程的调用
            Control.CheckForIllegalCrossThreadCalls = false;

            unsafe
            {
                // 获取机器码，把机器码发给我授权生成授权码
                IntPtr pCodeBuf = Marshal.AllocHGlobal(64);
                int ret = cGetMachineCode((char*)pCodeBuf);
                string strMCode = Marshal.PtrToStringAnsi(pCodeBuf);
                string[] lines = { string.Format("机器码：{0}", strMCode)};
                System.IO.File.WriteAllLines("csConfig.ini", lines, Encoding.UTF8);

                // 校验授权码，未授权时以管理员权限运行可试用1个月；已授权可普通用户运行
                IntPtr pMachineSN = Marshal.StringToHGlobalAnsi(m_strACode);
                int nRet = cMachineAuthorize((char*)pMachineSN);
                if (nRet < 0)
                {
                    MessageBox.Show("未注册，可改系统时间试用(2018.10.1至2019.1.1)！");
                }

                // 进程启动摄像头并比对
                ThreadStart childref = new ThreadStart(CallToChildThread);
                Thread childThread = new Thread(childref);
                childThread.Start();
            }
        }

        public void CallToChildThread()
        {
            unsafe
            {
                void* pWorker = cCreateFaceWorker(0, 0);    // 只加载人脸比对模块，打开摄像头0
                if (pWorker != null)
                {
                    // 建库。本处不使用真正的数据库，用数组模拟
                    DataBaseItem[] dbTtem = new DataBaseItem[2];
                    int nSize = cGetFeatureSize(pWorker);
                    IntPtr pBuf1 = Marshal.AllocHGlobal(nSize);
                    IntPtr pBuf2 = Marshal.AllocHGlobal(nSize);
                    dbTtem[0].pFeature = (char*)pBuf1;
                    dbTtem[1].pFeature = (char*)pBuf2;

                    // 录入
                    IntPtr pPic1 = Marshal.StringToHGlobalAnsi(m_strPic1);
                    IntPtr pPic2 = Marshal.StringToHGlobalAnsi(m_strPic2);
                    dbTtem[0].nId = 1;
                    int nR1 = cFeatureExtractP(pWorker, (char*)pPic1, dbTtem[0].pFeature);
                    dbTtem[1].nId = 2;
                    int nR2 = cFeatureExtractP(pWorker, (char*)pPic2, dbTtem[1].pFeature);

                    // 摄像头抓拍并在库里搜索
                    while (true)
                    {
                        void* pFrame = cGetFrame(pWorker);
                        if (pFrame != null)
                        {
                            IntPtr pBuf = Marshal.AllocHGlobal(nSize);
                            if (cFeatureExtractF(pWorker, pFrame, (char*)pBuf) > 0)
                            {
                                float fS = 0;
                                int nId = 0;
                                for (int i = 0; i < 2; i++)
                                {
                                    float fR = cFeatureCompare(pWorker, (char*)pBuf, dbTtem[i].pFeature);
                                    if (fR > fS)
                                    {
                                        fS = fR;
                                        nId = dbTtem[i].nId;
                                    }
                                }

                                if (fS > 0.5)
                                    labelScore.Text = string.Format("人脸搜索：匹配的人脸ID是 {0}(分数：{1});\n", nId, fS);
                                else
                                    labelScore.Text = string.Format("人脸搜索：未找到匹配的人脸(分数：{0});\n", fS);
                            }
                            else
                            {
                                labelScore.Text = "人脸搜索：无人脸;";
                            }

                            // 显示画面
                            void* hBitmap = cIplImage2hBitmap(pFrame);
                            pictureBoxCam.Image = Image.FromHbitmap((IntPtr)hBitmap);
                            cReleasehBitmap(hBitmap);
                        }
                        else
                        {
                            labelScore.Text = "人脸搜索：cGetFram fail!";
                        }
                        Thread.Sleep(m_nTime);
                    }
                }
                cReleaseFaceWorker(pWorker);
            }      
        }

        private void labelMcode_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            unsafe
            {
                void* pWorker = cCreateFaceWorker(1, -1);
                if (pWorker != null)
                {
                    string strText = "";
                    IntPtr pMachineSN = Marshal.StringToHGlobalAnsi(m_strPic2);
                    // 读取图片-身份证照片
                    void* pFrame = cReadImage((char*)pMachineSN);
                    if (pFrame != null)
                    {
                        FaceInfos stFaces;
                        cFaceDetect(pWorker, pFrame, &stFaces);
                        if (stFaces.nCount > 0)
                        {
                            cAgeDetect(pWorker, &stFaces);
                            cGenderDetect(pWorker, &stFaces);
                            strText += string.Format("人脸个数：{0};\r\n", stFaces.nCount);
                            for (int i = 0; i < stFaces.nCount; i++)
                            {
                                strText += string.Format("第{0}张人脸信息：年龄{1}，性别{2},位置：<({3},{4}),({5},{6})>;\r\n\r\n",
                                    i, stFaces.pFace[i].nAge, stFaces.pFace[i].nGender, stFaces.pFace[i].nLeft, stFaces.pFace[i].nTop, stFaces.pFace[i].nRight, stFaces.pFace[i].nBottom);
                            }
                        }
                        else
                        {
                            strText = "no face!";
                        }
                        cFaceRelease(pWorker, &stFaces);
                        cReleaseImage(pFrame);
                    }
                    else
                    {
                        strText = "cGetFram fail!";
                    }
                    textBoxFInfo.Text = strText;
                    cReleaseFaceWorker(pWorker);
                }

                // 创建工作对象，参数一表示启动模式（0只人脸比对，1比对和年龄性别检测），参数二是摄像头序号，一般取0
                /*void* pWorker = cCreateFaceWorker(0, 0);
                if (pWorker != null)
                {
                    while (true)
                    {
                        // 读摄像头的帧
                        void* pFram = cGetFrame(pWorker);
                        if (pFram != null)
                        {
                            FaceInfos stFaces;
                            // 人脸检测-摄像头帧
                            cFaceDetect(pWorker, pFram, &stFaces);
                            if (stFaces.nCount > 0)
                            {
                                int nSize = cGetFeatureSize(pWorker);
                                IntPtr pBuf1 = Marshal.AllocHGlobal(nSize);
                                // 特征提取-摄像头帧
                                cFeatureExtract(pWorker, &stFaces, stFaces.nMax, (char*)pBuf1);
                                cFaceRelease(pWorker, &stFaces);

                                IntPtr pMachineSN = Marshal.StringToHGlobalAnsi(m_strPicIDC);
                                // 读取图片-身份证照片
                                void* pImg = cReadImage((char*)pMachineSN);
                                FaceInfos stFacesImg;
                                // 人脸检测-身份证照片
                                cFaceDetect(pWorker, pImg, &stFacesImg);
                                IntPtr pBuf2 = Marshal.AllocHGlobal(nSize);
                                // 特征提取-身份证照片
                                cFeatureExtract(pWorker, &stFacesImg, stFacesImg.nMax, (char*)pBuf2);
                                cFaceRelease(pWorker, &stFacesImg);
                                cReleaseImage(pImg);

                                // 特征比对相识度
                                float fRet = cFeatureCompare(pWorker, (char*)pBuf1, (char*)pBuf2);
                                labelScore.Text = string.Format("分  数：{0}", fRet);
                                if (fRet <= m_fVal && fRet >= 0)
                                {
                                    labelRet.Text = "结  论：比对不通过.";
                                }
                                else if (fRet > m_fVal && fRet <= 1)
                                {
                                    labelRet.Text = "结  论：比对通过.";
                                }
                                else
                                {
                                    labelRet.Text = "结  论：比对出错.";
                                }
                                Marshal.FreeHGlobal(pBuf1);
                                Marshal.FreeHGlobal(pBuf2);
                            }
                            else
                            {
                                cFaceRelease(pWorker, &stFaces);
                            }

                        }

                        // 显示画面
                        void* hBitmap = cIplImage2hBitmap(pFram);
                        pictureBoxCam.Image = Image.FromHbitmap((IntPtr)hBitmap);
                        cReleasehBitmap(hBitmap);

                        Thread.Sleep(m_nTime);
                    }
                    cReleaseFaceWorker(pWorker);textBox2Pic
                }*/
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            unsafe
            {
                void* pWorker = cCreateFaceWorker(0, -1);   // 只加载人脸比对模块，不打开摄像头
                if (pWorker != null)
                {
                    IntPtr pPic1= Marshal.StringToHGlobalAnsi(m_strPic1);
                    IntPtr pPic2 = Marshal.StringToHGlobalAnsi(m_strPic2);
                    float fRet = cPictureCompare(pWorker, (char*)pPic1, (char*)pPic2);
                    if (fRet <= m_fVal && fRet >= 0)
                    {
                        textBox2Pic.Text = string.Format("结  论：比对不通过, 分  数：{0}", fRet);
                    }
                    else if (fRet > m_fVal && fRet <= 1)
                    {
                        textBox2Pic.Text = string.Format("结  论：比对通过, 分  数：{0}", fRet);
                    }
                    else
                    {
                        textBox2Pic.Text = string.Format("结  论：比对出错, 分  数：{0}", fRet);
                    }
                }
                cReleaseFaceWorker(pWorker);
            } 
        }
    }
}
