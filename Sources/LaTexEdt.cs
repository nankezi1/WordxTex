﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordxTex.Properties;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;
using ICSharpCode.TextEditor;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Reflection;

namespace WordxTex
{
    public partial class LaTexEdt : Form
    {
        public LaTexEdt(bool BatchMode)
        {
            InitializeComponent();
            if (BatchMode) //批量模式（测试中）
            {
                btn_prvTex.Enabled = true;
                btn_nxtTeX.Enabled = true;
            }
            else
            {
                btn_nxtTeX.Visible = false;
                btn_prvTex.Visible = false;
            }
            this.TopMost = true;
        }
        string TargetImgFile;
        string TexPreFile = "";
        public Process Rprocess = new Process();//创建进程对象   
                                                //public void StartProcess(string[] execParam)
                                                //{
                                                //    string execPath = execParam[0];
                                                //    string args = execParam[1];
                                                //    runLogs = runLogs + "\n" + execPath + " " + args + "\n";
                                                //    Control.CheckForIllegalCrossThreadCalls = false;
                                                //    //args = args.Trim();
                                                //    Rprocess = new Process();
                                                //    Process_Timer.Start();
                                                //    Rprocess.StartInfo.UseShellExecute = false; //不使用CMD
                                                //    Rprocess.StartInfo.CreateNoWindow = true; //不显示黑色窗口
                                                //    Rprocess.OutputDataReceived += new DataReceivedEventHandler(Log_Receive);
                                                //    Rprocess.StartInfo.RedirectStandardOutput = true;
                                                //    Rprocess.ErrorDataReceived += new DataReceivedEventHandler(Log_Receive);
                                                //    Rprocess.StartInfo.RedirectStandardError = true;
                                                //    Rprocess.StartInfo.FileName = execPath;
                                                //    Rprocess.StartInfo.Arguments = args;
                                                //    Rprocess.EnableRaisingEvents = true;
                                                //    Rprocess.Exited += new EventHandler(ProcessInterate);
                                                //    //Rprocess.Exited += delegate (object sender, EventArgs e)
                                                //    //{
                                                //    //};            
                                                //    //Rprocess.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                                                //    //{                
                                                //    //});
                                                //    Rprocess.Start();
                                                //    Rprocess.BeginOutputReadLine();
                                                //    Rprocess.BeginErrorReadLine();
                                                //}
                                                //private void Log_Receive(object sender, DataReceivedEventArgs e)
                                                //{
                                                //    string logs = "";

        //    try
        //    {
        //        logs = e.Data;
        //    }
        //    catch (System.Exception)
        //    {

        //    }
        //    logsbox.Text = logsbox.Text + "\n" + logs;
        //    runLogs = runLogs + "\n" + logs;
        //}
        //private void ProcessInterate(object sender, EventArgs e)
        //{
        //    string tempDir = System.Environment.GetEnvironmentVariable("TEMP") + "\\WordxTex";
        //    int shapePosition = 0;
        //    InlineShape inDocPic;
        //    if (((Process)sender).ExitCode != 0)
        //    {
        //        MessageBox.Show("See Log", "ERR");
        //        btn_gen.Enabled = true;
        //        return;
        //    }
        //    if (CpQueue.Terminated())
        //    {
        //        Process_Timer.Stop();
        //        btn_gen.Enabled = true;
        //        Microsoft.Office.Interop.Word.Document ThisDoc = Globals.ThisAddIn.Application.ActiveDocument;
        //        //if (imgFile.Length == 0)
        //        //    return;
        //        if (ThisDoc.Application.Selection.Type != WdSelectionType.wdSelectionIP)
        //        {
        //            shapePosition = ThisDoc.Application.Selection.Font.Position;
        //            ThisDoc.Application.Selection.Delete();
        //        }
        //        if (Ribbon.get_param_value(WordxTex.Ribbon.Compile_Info, "grapher") == (string)"dvipng")
        //        {
        //            string pngvRes = Ribbon.get_param_value(WordxTex.Ribbon.Compile_Info, "pngvRes");
        //            //Image pngDocLocal = Image.FromFile(imgFile);
        //            //float docAwidth = pngDocLocal.Width / float.Parse(pngvRes);
        //            //float docAheight = pngDocLocal.Height / float.Parse(pngvRes);
        //            int pngdpi = int.Parse(pngvRes);
        //            //inDocPic.Width = 72 * docAwidth;
        //            //inDocPic.Height = 72 * docAheight;
        //            //pngDocLocal.Dispose();
        //            Bitmap bMp = (Bitmap)Image.FromFile(TargetImgFile);
        //            bMp.SetResolution(pngdpi, pngdpi);
        //            string R_imgFile = tempDir + "\\" + "_" + pngvRes + ".png";
        //            bMp.Save(R_imgFile, ImageFormat.Png);
        //            bMp.Dispose();
        //            inDocPic = ThisDoc.InlineShapes.AddPicture(R_imgFile);
        //        }
        //        else
        //        {
        //            inDocPic = ThisDoc.InlineShapes.AddPicture(TargetImgFile);
        //        }
        //        inDocPic.AlternativeText = TexPreFile;
        //        inDocPic.Select();
        //        ThisDoc.Application.Selection.Font.Position = shapePosition;
        //        if (cb_AutoClose.Checked)
        //            this.Close();
        //        runLogs.Trim();
        //        logsbox.Text = runLogs;
        //        //MessageBox.Show(((Process)sender).StartInfo.FileName + ": " + ((Process)sender).ExitCode.ToString());
        //    }
        //    else
        //    {
        //        StartProcess(CpQueue.ExecProgramIteration());
        //    }
        //}

        private void btn_gen_Click(object sender, EventArgs e)
        {
            btn_gen.Enabled = false; //防止按多次
            logsbox.Clear(); //清空日志框，防溢出
            Microsoft.Office.Interop.Word.Document ThisDoc = Globals.ThisAddIn.Application.ActiveDocument;
            string occupied_id = "param";
            if (ThisDoc == null || ThisDoc.ReadOnly)
                return;
            string tempDir = System.Environment.GetEnvironmentVariable("TEMP") + "\\WordxTex";
            latex_style_gen(tempDir);
            string TexFile = tempDir + "\\" + occupied_id + ".tex";
            TexPreFile = texCodeBox.Text;
            System.Text.UTF8Encoding UTF8Enc = new System.Text.UTF8Encoding(false);
            File.WriteAllText(TexFile, texCodeBox.Text, UTF8Enc); //将tex文件写入到磁盘
            latex_compile(WordxTex.Ribbon.Compile_Info, tempDir, occupied_id); //编译tex
        }
        private void latex_compile(string cp_param, string WkDir, string OccupiedName)
        {
            //命令准备
            string Complier = Ribbon.get_param_value(cp_param, "complier") + ".exe";
            string Grapher = Ribbon.get_param_value(cp_param, "grapher") + ".exe";
            string TexFile = WkDir + "\\" + OccupiedName + ".tex";
            string OutDviFile = WkDir + "\\" + OccupiedName + Ribbon.get_param_value(cp_param, "ctarget");
            string OutImgFile = WkDir + "\\" + OccupiedName + Ribbon.get_param_value(cp_param, "gtarget");
            TargetImgFile = OutImgFile;
            string Complier_Args = Ribbon.get_param_value(cp_param, "cp_arg");
            string Grapher_Args = Ribbon.get_param_value(cp_param, "gr_arg");
            Complier_Args = Complier_Args.Replace("%%BlankSpace", " ");
            Complier_Args = Complier_Args.Replace("%%InTexFile", TexFile);
            Complier_Args = Complier_Args.Replace("%%OutDir", WkDir);
            Complier_Args = Complier_Args.Replace("%%Equal", "=");
            Complier_Args = Complier_Args.Replace("\\", "/");
            Grapher_Args = Grapher_Args.Replace("%%BlankSpace", " ");
            Grapher_Args = Grapher_Args.Replace("%%OutImgFile", OutImgFile);
            Grapher_Args = Grapher_Args.Replace("%%InDviFile", OutDviFile);
            Grapher_Args = Grapher_Args.Replace("%%Equal", "=");
            Grapher_Args = Grapher_Args.Replace("\\", "/");

            if (false == System.IO.Directory.Exists(WkDir)) //检测工作目录是否需要创建
                System.IO.Directory.CreateDirectory(WkDir);
            latex_style_gen(WkDir); //生成自动模板
            //准备命令队列
            WordxTex.wTModule.ProgramQueue CpQueue = new WordxTex.wTModule.ProgramQueue(new string[] { Complier, Grapher }, new string[] { Complier_Args, Grapher_Args });
            CpQueue.RunAll = false; //阻止命令队列自动运行下一个
            ThreadStart thrdstart = new ThreadStart(CpQueue.Run); //使用另一线程运行
            Thread thrd = new Thread(thrdstart);
            CpQueue.ProgramsRunResult += delegate (object report, EventArgs ev) //接收运行结果
            {
                Control.CheckForIllegalCrossThreadCalls = false; //跨线程操作
                Thread thrd_c;

                //写入日志框
                int logsBoxCount = logsbox.Text.Length; 
                string programExecParam = ((wTModule.ProgramResult)report).execName + " " + ((wTModule.ProgramResult)report).execArgs;
                logsbox.Text = logsbox.Text + programExecParam + "\n";
                logsbox.Text = logsbox.Text + (((wTModule.ProgramResult)report).execLogs);
                if (((wTModule.ProgramResult)report).exitCode != 0)
                {
                    //错误运行
                    logsbox.Select(logsBoxCount, programExecParam.Length);
                    logsbox.SelectionColor = Color.Red; //标红
                    btn_gen.Enabled = true;//恢复按钮
                    return; //停止运行
                }

                if (((wTModule.ProgramResult)report).theLastProgram)
                {
                    Microsoft.Office.Interop.Word.Document ThisDoc = Globals.ThisAddIn.Application.ActiveDocument;
                    //完成运行队列
                    string tempDir = System.Environment.GetEnvironmentVariable("TEMP") + "\\WordxTex";
                    int shapePosition = 0;
                    InlineShape inDocPic;
                    Process_Timer.Stop();

                    if (ThisDoc.Application.Selection.Type != WdSelectionType.wdSelectionIP)
                    {
                        shapePosition = ThisDoc.Application.Selection.Font.Position;
                        ThisDoc.Application.Selection.Delete(); //删除选中 的数据
                    }
                    if (Ribbon.get_param_value(WordxTex.Ribbon.Compile_Info, "grapher") == (string)"dvipng")
                    { 
                        //dvipng 产出PNG分辨率为72dpi,将生成的png图片转换分辨率
                        string pngvRes = Ribbon.get_param_value(WordxTex.Ribbon.Compile_Info, "pngvRes");
                        int pngdpi = int.Parse(pngvRes);
                        Bitmap bMp = (Bitmap)Image.FromFile(TargetImgFile);
                        bMp.SetResolution(pngdpi, pngdpi);
                        string R_imgFile = tempDir + "\\" + "_" + pngvRes + ".png";
                        bMp.Save(R_imgFile, ImageFormat.Png);
                        bMp.Dispose();
                        inDocPic = ThisDoc.InlineShapes.AddPicture(R_imgFile);
                    }
                    else
                    {
                        //svg直接插入
                        inDocPic = ThisDoc.InlineShapes.AddPicture(TargetImgFile);
                    }
                    inDocPic.AlternativeText = TexPreFile; //写入Tex数据
                    inDocPic.Select(); //选择新插入的图片
                    ThisDoc.Application.Selection.Font.Position = shapePosition; //基线还原
                    if (cb_AutoClose.Checked) //检测自动关闭
                        this.Close();
                    btn_gen.Enabled = true; //复原按钮
                    return;
                }
                thrd_c = new Thread(thrdstart);
                thrd_c.Start();
            };
            thrd.Start();
        }

        private void LaTexEdt_Load(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Word.Document ThisDoc = Globals.ThisAddIn.Application.ActiveDocument;
            box_run_max_time.Value = Process_Timer.Interval / 1000;
            string tempDir = System.Environment.GetEnvironmentVariable("TEMP") + "\\WordxTex";
            for (int i = 0; i < Directory.GetFiles(tempDir).ToList().Count; i++)
                File.Delete(Directory.GetFiles(tempDir)[i]); //清空临时目录
            if (false == System.IO.Directory.Exists(tempDir))
                System.IO.Directory.CreateDirectory(tempDir);
            latex_style_gen(tempDir); //生成自动模板
        }

        private void latex_style_gen(string destDir)
        {
            Microsoft.Office.Interop.Word.Document ThisDoc = Globals.ThisAddIn.Application.ActiveDocument;
            string TexStyFile = destDir + "\\param.sty"; //自动模板途径
            using (FileStream fs = new FileStream(TexStyFile, FileMode.Create))
            {
                string FullStyle = Resources.tex_fontsize_style;
                string FontSize = ThisDoc.Application.Selection.Font.Size.ToString(); //当前应用设置的字体大小
                FullStyle = FullStyle.Replace("%%WordxTex_Font_Symbol", FontSize); //将模板内的通配符替换为字体大小
                FullStyle = FullStyle.Replace("%%WordxTex_Equation_Font_Symbal", WordxTex.Ribbon.Box_Font_Fx); //替换为所选字体样式
                byte[] TexContent = System.Text.Encoding.Default.GetBytes(FullStyle);
                fs.Write(TexContent, 0, TexContent.Length); //写入style文件
                fs.Flush();
                fs.Close();
            };
        }
        private void Process_Timer_Tick(object sender, EventArgs e)
        {
            Process_Timer.Stop();
            if (Rprocess.HasExited)
                return;
            Rprocess.Kill();
        }

        private void box_run_max_time_ValueChanged(object sender, EventArgs e)
        {
            Process_Timer.Interval = (int)box_run_max_time.Value * 1000;
        }

        private void btn_nxtTeX_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Word.Document ThisDoc = Globals.ThisAddIn.Application.ActiveDocument;
            Range TexItem = null;
            ThisDoc.Application.Selection.SetRange(ThisDoc.Application.Selection.Start, ThisDoc.Application.Selection.End + 1);
            TexItem = ThisDoc.Application.Selection.GoToNext(WdGoToItem.wdGoToGraphic);
            TexItem.SetRange(TexItem.Start, TexItem.End + 1);
            TexItem.Select();
            InlineShapes SelectedObj = ThisDoc.Application.Selection.InlineShapes;
            InlineShape SelectedObjFirst = SelectedObj[1];
            if (ThisDoc == null || ThisDoc.ReadOnly)
                return;
            if (SelectedObj.Count == 0)
                return;
            if (!SelectedObjFirst.AlternativeText.Contains("WordxTex_TexContent"))
                return;
            //if (SelectedObj[0].AlternativeText.Length == 0)
            //    return;
            texCodeBox.Clear();
            texCodeBox.Text = SelectedObjFirst.AlternativeText;
        }
    }
}
