/*
 *  Copyright 2013-2015 Vitaliy Fedorchenko
 *
 *  Licensed under PdfGenerator Source Code Licence (see LICENSE file).
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS 
 *  OF ANY KIND, either express or implied.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
#if !NET_STANDARD
using System.Web;
#endif
using System.Threading;
using System.Reflection;
using System.Globalization;
using System.IO.Compression;

namespace NReco.PdfGenerator
{
    /// <summary>
    /// Html to PDF converter component (C# WkHtmlToPdf process wrapper).
    /// </summary>
    public class HtmlToPdfConverter
    {
        /// <summary>
        /// Get or set path where WkHtmlToPdf tool is located
        /// </summary>
        /// <remarks>
        /// By default this property points to the folder where application assemblies are located.
        /// If WkHtmlToPdf tool files are not present PdfConverter expands them from DLL resources.
        /// </remarks>
        public string PdfToolPath { get; set; }

        /// <summary>
        /// Get or set WkHtmlToPdf tool EXE file name ('wkhtmltopdf.exe' by default)
        /// </summary>
        public string WkHtmlToPdfExeName { get; set; }

        /// <summary>
        /// Get or set location for temp files (if not specified location returned by <see cref="Path.GetTempPath"/> is used for temp files)
        /// </summary>
        /// <remarks>Temp files are used for providing cover page/header/footer HTML templates to wkhtmltopdf tool.</remarks>
        public string TempFilesPath { get; set; }

        /// <summary>
        /// Get or set PDF page orientation
        /// </summary>
        public PageOrientation Orientation { get; set; }

        /// <summary>
        /// Get or set PDF page orientation
        /// </summary>
        public PageSize Size { get; set; }

        /// <summary>
        /// Gets or sets option to generate low quality PDF (shrink the result document space)
        /// </summary>
        public bool LowQuality { get; set; }

        /// <summary>
        /// Gets or sets option to generate grayscale PDF
        /// </summary>
        public bool Grayscale { get; set; }

        /// <summary>
        /// Gets or sets zoom factor
        /// </summary>
        public float Zoom { get; set; }

        /// <summary>
        /// Gets or sets PDF page margins (in mm)
        /// </summary>
        public PageMargins Margins { get; set; }

        /// <summary>
        /// Gets or sets PDF page width (in mm)
        /// </summary>
        public float? PageWidth { get; set; }

        /// <summary>
        /// Gets or sets PDF page height (in mm)
        /// </summary>
        public float? PageHeight { get; set; }

        /// <summary>
        /// Gets or sets TOC generation flag
        /// </summary>
        public bool GenerateToc { get; set; }

        /// <summary>
        /// Gets or sets custom TOC header text (default: "Table of Contents")
        /// </summary>
        public string TocHeaderText { get; set; }

        /// <summary>
        /// Custom WkHtmlToPdf global options
        /// </summary>
        public string CustomWkHtmlArgs { get; set; }

        /// <summary>
        /// Custom WkHtmlToPdf page options
        /// </summary>
        public string CustomWkHtmlPageArgs { get; set; }

        /// <summary>
        /// Custom WkHtmlToPdf cover options (applied only if cover content is specified)
        /// </summary>
        public string CustomWkHtmlCoverArgs { get; set; }

        /// <summary>
        /// Custom WkHtmlToPdf toc options (applied only if GenerateToc is true)
        /// </summary>
        public string CustomWkHtmlTocArgs { get; set; }

        /// <summary>
        /// Get or set custom page header HTML
        /// </summary>
        public string PageHeaderHtml { get; set; }

        /// <summary>
        /// Get or set custom page footer HTML
        /// </summary>
        public string PageFooterHtml { get; set; }

        /// <summary>
        /// Get or set maximum execution time for PDF generation process (by default is null that means no timeout)
        /// </summary>
        public TimeSpan? ExecutionTimeout { get; set; }

        /// <summary>
        /// Occurs when log line is received from WkHtmlToPdf process
        /// </summary>
        /// <remarks>
        /// Quiet mode should be disabled if you want to get wkhtmltopdf info/debug messages
        /// </remarks>
        public event EventHandler<DataReceivedEventArgs> LogReceived;

        /// <summary>
        /// Suppress wkhtmltopdf debug/info log messages (by default is true)
        /// </summary>
        public bool Quiet { get; set; }

        /// <summary>
        /// Component commercial license information.
        /// </summary>
        public LicenseInfo License { get; private set; }

        private Process WkHtmlToPdfProcess = null;
        private bool batchMode = false;

        /// <summary>
        /// Create new instance of HtmlToPdfConverter
        /// </summary>
        public HtmlToPdfConverter()
        {
            License = new LicenseInfo();

#if NET_STANDARD
			string rootDir = null;
#else
            string rootDir = AppDomain.CurrentDomain.BaseDirectory;
#endif

#if !LIGHT
            if (HttpContext.Current != null)
              //  rootDir = HttpRuntime.AppDomainAppPath + "bin";
            rootDir = HttpRuntime.AppDomainAppPath + "pdftool";
#endif

            PdfToolPath = rootDir;
            TempFilesPath = null;
            WkHtmlToPdfExeName = "wkhtmltopdf.exe";
            Orientation = PageOrientation.Default;
            Size = PageSize.Letter;
            LowQuality = false;
            Grayscale = false;
            Quiet = true;
            Zoom = 1.0F;
            Margins = new PageMargins();
        }

        private const string headerFooterHtmlTpl = @"<!DOCTYPE html><html><head>
<meta http-equiv=""content-type"" content=""text/html; charset=utf-8"" />
<script>
function subst() {{
    var vars={{}};
    var x=document.location.search.substring(1).split('&');

    for(var i in x) {{var z=x[i].split('=',2);vars[z[0]] = unescape(z[1]);}}
    var x=['frompage','topage','page','webpage','section','subsection','subsubsection'];
    for(var i in x) {{
      var y = document.getElementsByClassName(x[i]);
      for(var j=0; j<y.length; ++j) y[j].textContent = vars[x[i]];
    }}
}}
</script></head><body style=""border:0; margin: 0;"" onload=""subst()"">{0}</body></html>
";

        private static object globalObj = new object();

        private void EnsureWkHtmlLibs()
        {
#if LIGHT
			License.Check();
#else
            var pdfGenAssembly = Assembly.GetExecutingAssembly();
            var resourcesList = pdfGenAssembly.GetManifestResourceNames();
            var resPrefix = "NReco.PdfGenerator.WkHtmlToPdf.";
            foreach (var resName in resourcesList)
            {
                if (!resName.StartsWith(resPrefix))
                    continue;
                var res = resName.Substring(resPrefix.Length);
                var targetFileName = Path.Combine(PdfToolPath, Path.GetFileNameWithoutExtension(res));

                lock (globalObj)
                {
                    if (File.Exists(targetFileName))
                    {
                        if (File.GetLastWriteTime(targetFileName) > File.GetLastWriteTime(pdfGenAssembly.Location))
                            continue;
                    }
                    // ensure that folder exists
                    if (!Directory.Exists(PdfToolPath))
                        Directory.CreateDirectory(PdfToolPath);

                    var resStream = pdfGenAssembly.GetManifestResourceStream(resName);
                    using (var inputStream = new GZipStream(resStream, CompressionMode.Decompress, false))
                    {
                        using (var outputStream = new FileStream(targetFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            var buf = new byte[64 * 1024];
                            int read;
                            while ((read = inputStream.Read(buf, 0, buf.Length)) > 0)
                                outputStream.Write(buf, 0, read);
                        }
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Generates PDF by specifed HTML content
        /// </summary>
        /// <param name="htmlContent">HTML content</param>
        /// <returns>PDF bytes</returns>
        public byte[] GeneratePdf(string htmlContent)
        {
            return GeneratePdf(htmlContent, null);
        }

        /// <summary>
        /// Generates PDF by specfied HTML content and prepend cover page (useful with GenerateToc option)
        /// </summary>
        /// <param name="htmlContent">HTML document</param>
        /// <param name="coverHtml">first page HTML (optional; can be null)</param>
        /// <returns>PDF bytes</returns>
        public byte[] GeneratePdf(string htmlContent, string coverHtml)
        {
            var ms = new MemoryStream();
            GeneratePdf(htmlContent, coverHtml, ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Generates PDF by specfied HTML content (optionally with the cover page).
        /// </summary>
        /// <param name="htmlContent">HTML document</param>
        /// <param name="coverHtml">first page HTML (optional; can be null)</param>
        /// <param name="output">output stream for generated PDF</param>
        public void GeneratePdf(string htmlContent, string coverHtml, Stream output)
        {
            if (htmlContent == null)
                throw new ArgumentNullException("htmlContent");
            GeneratePdfInternal(new[] { "-" }, htmlContent, coverHtml, "-", output);
        }

        /// <summary>
        /// Generates PDF by specfied HTML content (optionally with the cover page).
        /// </summary>
        /// <param name="htmlContent">HTML document</param>
        /// <param name="coverHtml">first page HTML (can be null)</param>
        /// <param name="outputPdfFilePath">path to the output PDF file (if file already exists it will be removed before PDF generation)</param>
        public void GeneratePdf(string htmlContent, string coverHtml, string outputPdfFilePath)
        {
            if (htmlContent == null)
                throw new ArgumentNullException("htmlContent");
            GeneratePdfInternal(new[] { "-" }, htmlContent, coverHtml, outputPdfFilePath, null);
        }

        /// <summary>
        /// Generate PDF by specfied HTML content and prepend cover page (useful with GenerateToc option)
        /// </summary>
        /// <param name="htmlFilePath">path to HTML file or absolute URL</param>
        /// <param name="coverHtml">first page HTML (optional, can be null)</param>
        /// <returns>PDF bytes</returns>
        public byte[] GeneratePdfFromFile(string htmlFilePath, string coverHtml)
        {
            var ms = new MemoryStream();
            GeneratePdfInternal(new[] { htmlFilePath }, coverHtml, ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Generate PDF by specfied HTML content and prepend cover page (useful with GenerateToc option)
        /// </summary>
        /// <param name="htmlFilePath">path to HTML file or absolute URL</param>
        /// <param name="coverHtml">first page HTML (optional, can be null)</param>
        /// <param name="output">output stream for generated PDF</param>
        public void GeneratePdfFromFile(string htmlFilePath, string coverHtml, Stream output)
        {
            GeneratePdfInternal(new[] { htmlFilePath }, coverHtml, output);
        }

        /// <summary>
        /// Generate PDF by specfied HTML content and prepend cover page (useful with GenerateToc option)
        /// </summary>
        /// <param name="htmlFilePath">path to HTML file or absolute URL</param>
        /// <param name="coverHtml">first page HTML (optional, can be null)</param>
        /// <param name="outputPdfFilePath">path to the output PDF file (if file already exists it will be removed before PDF generation)</param>
        public void GeneratePdfFromFile(string htmlFilePath, string coverHtml, string outputPdfFilePath)
        {
            if (File.Exists(outputPdfFilePath))
                File.Delete(outputPdfFilePath);
            GeneratePdfInternal(new[] { htmlFilePath }, null, coverHtml, outputPdfFilePath, null);
        }

        /// <summary>
        /// Generate PDF into specified <see cref="Stream"/> by several HTML documents (local files or URLs) 
        /// </summary>
        /// <param name="htmlFiles">list of HTML files or URLs</param>
        /// <param name="coverHtml">first page HTML (optional, can be null)</param>
        /// <param name="output">output stream for generated PDF</param>
        public void GeneratePdfFromFiles(string[] htmlFiles, string coverHtml, Stream output)
        {
            GeneratePdfInternal(htmlFiles, coverHtml, output);
        }

        /// <summary>
        /// Generate PDF into specified output file by several HTML documents (local files or URLs) 
        /// </summary>
        /// <param name="htmlFiles">list of HTML files or URLs</param>
        /// <param name="coverHtml">first page HTML (optional, can be null)</param>
        /// <param name="outputPdfFilePath">path to output PDF file (if file already exists it will be removed before PDF generation)</param>
        public void GeneratePdfFromFiles(string[] htmlFiles, string coverHtml, string outputPdfFilePath)
        {
            if (File.Exists(outputPdfFilePath))
                File.Delete(outputPdfFilePath);
            GeneratePdfInternal(htmlFiles, null, coverHtml, outputPdfFilePath, null);
        }

        private void GeneratePdfInternal(string[] htmlFiles, string coverHtml, Stream output)
        {
            GeneratePdfInternal(htmlFiles, null, coverHtml, "-", output);
        }

        private void CheckWkHtmlProcess()
        {
            if (!batchMode && WkHtmlToPdfProcess != null)
                throw new InvalidOperationException("WkHtmlToPdf process is already started");
        }

        private class PdfSettings
        {
            public string CoverFilePath;
            public string HeaderFilePath = null;
            public string FooterFilePath = null;
            public string[] InputFiles = null;
            public string OutputFile = null;
        }

        private string GetTempPath()
        {
            // ensure that custom temp folder exists
            if (!String.IsNullOrEmpty(TempFilesPath) && !Directory.Exists(TempFilesPath))
            {
                Directory.CreateDirectory(TempFilesPath);
            }
            return TempFilesPath ?? Path.GetTempPath();
        }

        private string GetToolExePath()
        {
            if (String.IsNullOrEmpty(PdfToolPath))
                throw new ArgumentException("PdfToolPath property is not initialized with path to wkhtmltopdf binaries");
            var toolExe = Path.Combine(PdfToolPath, WkHtmlToPdfExeName);
            if (!File.Exists(toolExe))
                throw new FileNotFoundException("Cannot find wkhtmltopdf executable: " + toolExe);
            return toolExe;
        }

        private string CreateTempFile(string content, string tempPath, List<string> tempFilesList)
        {
            var tmpFilePath = Path.Combine(tempPath, "pdfgen-" + Path.GetRandomFileName() + ".html");
            tempFilesList.Add(tmpFilePath);
            if (content != null)
                File.WriteAllBytes(tmpFilePath, Encoding.UTF8.GetBytes(content));
            return tmpFilePath;
        }

        private string ComposeArgs(PdfSettings pdfSettings)
        {
            var toolArgsSb = new StringBuilder();
            if (Quiet)
                toolArgsSb.Append(" -q ");
            //global options first
            if (Orientation != PageOrientation.Default)
                toolArgsSb.AppendFormat(" -O {0} ", Orientation.ToString());
            if (Size != PageSize.Letter)
                toolArgsSb.AppendFormat(" -s {0} ", Size.ToString());
            if (LowQuality)
                toolArgsSb.Append(" -l ");
            if (Grayscale)
                toolArgsSb.Append(" -g ");
            if (Margins != null)
            {
                if (Margins.Top != null)
                {
                    toolArgsSb.AppendFormat(CultureInfo.InvariantCulture, " -T {0}", Margins.Top);
                }
                if (Margins.Bottom != null)
                {
                    toolArgsSb.AppendFormat(CultureInfo.InvariantCulture, " -B {0}", Margins.Bottom);
                }
                if (Margins.Left != null)
                {
                    toolArgsSb.AppendFormat(CultureInfo.InvariantCulture, " -L {0}", Margins.Left);
                }
                if (Margins.Right != null)
                {
                    toolArgsSb.AppendFormat(CultureInfo.InvariantCulture, " -R {0}", Margins.Right);
                }
            }
            if (PageWidth != null)
                toolArgsSb.AppendFormat(CultureInfo.InvariantCulture, " --page-width {0}", PageWidth);
            if (PageHeight != null)
                toolArgsSb.AppendFormat(CultureInfo.InvariantCulture, " --page-height {0}", PageHeight);

            if (pdfSettings.HeaderFilePath != null)
                toolArgsSb.AppendFormat(" --header-html \"{0}\"", pdfSettings.HeaderFilePath);
            if (pdfSettings.FooterFilePath != null)
                toolArgsSb.AppendFormat(" --footer-html \"{0}\"", pdfSettings.FooterFilePath);

            if (!String.IsNullOrEmpty(CustomWkHtmlArgs))
                toolArgsSb.AppendFormat(" {0} ", CustomWkHtmlArgs);
            // -- end global options

            if (pdfSettings.CoverFilePath != null)
            {
                toolArgsSb.AppendFormat(" cover \"{0}\" ", pdfSettings.CoverFilePath);
                if (!String.IsNullOrEmpty(CustomWkHtmlCoverArgs))
                {
                    toolArgsSb.AppendFormat(" {0} ", CustomWkHtmlCoverArgs);
                }
            }

            if (GenerateToc)
            {
                toolArgsSb.Append(" toc ");
                if (!String.IsNullOrEmpty(TocHeaderText))
                    toolArgsSb.AppendFormat(" --toc-header-text \"{0}\"", TocHeaderText.Replace("\"", "\\\""));
                if (!String.IsNullOrEmpty(CustomWkHtmlTocArgs))
                {
                    toolArgsSb.AppendFormat(" {0} ", CustomWkHtmlTocArgs);
                }
            }

            foreach (var inputFile in pdfSettings.InputFiles)
            {
                toolArgsSb.AppendFormat(" \"{0}\" ", inputFile);
                if (!String.IsNullOrEmpty(CustomWkHtmlPageArgs))
                    toolArgsSb.AppendFormat(" {0} ", CustomWkHtmlPageArgs);
                if (Zoom != 1)
                    toolArgsSb.AppendFormat(CultureInfo.InvariantCulture, " --zoom {0} ", Zoom);
            }

            toolArgsSb.AppendFormat(" \"{0}\" ", pdfSettings.OutputFile);
            return toolArgsSb.ToString();
        }

        private void GeneratePdfInternal(string[] htmlFiles, string inputContent, string coverHtml, string outputPdfFilePath, Stream outputStream)
        {
            if (!batchMode)
                EnsureWkHtmlLibs(); // check wkhtmltopdf.exe only if batch mode = false

#if LIGHT
			License.Check();
#endif

            CheckWkHtmlProcess();

            var tempPath = GetTempPath();
            var pdfSettings = new PdfSettings() { InputFiles = htmlFiles, OutputFile = outputPdfFilePath };
            var tmpFiles = new List<string>();

            pdfSettings.CoverFilePath = !String.IsNullOrEmpty(coverHtml) ? CreateTempFile(coverHtml, tempPath, tmpFiles) : null;
            pdfSettings.HeaderFilePath = !String.IsNullOrEmpty(PageHeaderHtml) ?
                CreateTempFile(String.Format(headerFooterHtmlTpl, PageHeaderHtml), tempPath, tmpFiles) : null;
            pdfSettings.FooterFilePath = !String.IsNullOrEmpty(PageFooterHtml) ?
                CreateTempFile(String.Format(headerFooterHtmlTpl, PageFooterHtml), tempPath, tmpFiles) : null;

            try
            {
                // wkhtmltopdf uses temp files anyway when stdin/stdout are used for input/output
                // and it doesn't remove PDF temp file if stdout is redirected
                // lets use our temp files to workaround this issue (and avoid system temp folder usage if custom temp path is used)

                if (inputContent != null)
                {
                    pdfSettings.InputFiles = new[] {
                        CreateTempFile(inputContent, tempPath, tmpFiles)
                    };
                }
                if (outputStream != null)
                {
                    pdfSettings.OutputFile = CreateTempFile(null, tempPath, tmpFiles);
                }

                if (batchMode)
                {
                    InvokeWkHtmlToPdfInBatch(pdfSettings);
                }
                else
                {
                    InvokeWkHtmlToPdf(pdfSettings, null, null);
                }

                if (outputStream != null)
                {
                    using (var fs = new FileStream(pdfSettings.OutputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        CopyStream(fs, outputStream, 64 * 1024);
                    }
                }

            }
            catch (Exception ex)
            {
                if (!batchMode)
                    EnsureWkHtmlProcessStopped();
                throw new Exception("Cannot generate PDF: " + ex.Message, ex);
            }
            finally
            {
                foreach (var tmpFile in tmpFiles)
                    DeleteFileIfExists(tmpFile);
            }

        }

        private void InvokeWkHtmlToPdfInBatch(PdfSettings pdfSettings)
        {
            License.Check();

            var lastErrorLine = String.Empty;
            DataReceivedEventHandler errorDataHandler = (o, args) =>
            {
                if (args.Data == null)
                    return;
                if (!String.IsNullOrEmpty(args.Data))
                    lastErrorLine = args.Data;

                if (LogReceived != null)
                {
                    LogReceived(this, args);
                }
            };

            if (WkHtmlToPdfProcess == null || WkHtmlToPdfProcess.HasExited)
            {
                // first call. Initalize interactive wkhtmltopdf for batch processing
                var startInfo = new ProcessStartInfo(GetToolExePath(), "--read-args-from-stdin");
#if !NET_STANDARD
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
#endif
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory = Path.GetDirectoryName(PdfToolPath);
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = false;
                startInfo.RedirectStandardError = true;
                WkHtmlToPdfProcess = Process.Start(startInfo);
                WkHtmlToPdfProcess.BeginErrorReadLine();
            }

            WkHtmlToPdfProcess.ErrorDataReceived += errorDataHandler;
            try
            {
                // lets remove output file: it seems this is only way to check that processing is completed
                // log messages from stderr cannot be used for that because of buffered stderr output
                if (File.Exists(pdfSettings.OutputFile))
                    File.Delete(pdfSettings.OutputFile);

                var cmdArgs = ComposeArgs(pdfSettings);
                cmdArgs = cmdArgs.Replace('\\', '/');
                WkHtmlToPdfProcess.StandardInput.WriteLine(cmdArgs);

                bool working = true;
                while (working)
                {
                    Thread.Sleep(25);
                    // wkhtmltopdf exited on errors
                    if (WkHtmlToPdfProcess.HasExited)
                    {
                        working = false;
                    }
                    if (File.Exists(pdfSettings.OutputFile))
                    {
                        working = false;
                        WaitForFile(pdfSettings.OutputFile);
                    }
                }

                if (WkHtmlToPdfProcess.HasExited)
                {
                    CheckExitCode(WkHtmlToPdfProcess.ExitCode, lastErrorLine, File.Exists(pdfSettings.OutputFile));
                }
            }
            finally
            {
                if (WkHtmlToPdfProcess != null && !WkHtmlToPdfProcess.HasExited)
                {
                    WkHtmlToPdfProcess.ErrorDataReceived -= errorDataHandler;
                }
                else
                {
                    EnsureWkHtmlProcessStopped();
                }
            }
        }

        private void WaitForFile(string fullPath)
        {
            // unfortunately there is no other way to ensure that PDF file is completely written by wkhtmltopdf
            // lets check that file is generated 
            double num = ExecutionTimeout != null && ExecutionTimeout.Value != TimeSpan.Zero ? ExecutionTimeout.Value.TotalMilliseconds : 60000;
            int cnt = 0;
            while (num > 0)
            {
                cnt++;
                num -= 50;
                try
                {
                    // Attempt to open the file exclusively.
                    using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 100))
                    {
                        fs.ReadByte();
                        // If we got this far the file is ready
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Thread.Sleep(cnt < 10 ? 50 : 100);
                }
            }
            if (num == 0)
            {
                // this is strange: lets stop batch process (next call will create new process)
                if (WkHtmlToPdfProcess != null && !WkHtmlToPdfProcess.HasExited)
                {
                    WkHtmlToPdfProcess.StandardInput.Close();
                    WkHtmlToPdfProcess.WaitForExit();
                }
            }
        }

        private void InvokeWkHtmlToPdf(PdfSettings pdfSettings, string inputContent, Stream outputStream)
        {
            // inputContent can be used for passing HTML input using stdin
            // outputStream can be used for reading PDF output from stdout
            // latest release of wkhtmltopdf (0.12.2.x) uses temp files in these cases anyway (and doesn't removes tmp file for output PDF)
            // right now PdfGenerator uses own temp files to workaround this wkhtmltopdf bug
            var lastErrorLine = String.Empty;
            DataReceivedEventHandler errorDataHandler = (o, args) =>
            {
                if (args.Data == null)
                    return;
                if (!String.IsNullOrEmpty(args.Data))
                {
                    lastErrorLine = args.Data;
                }

                if (LogReceived != null)
                {
                    LogReceived(this, args);
                }
            };
            var inputBytes = inputContent != null ? Encoding.UTF8.GetBytes(inputContent) : null;

            try
            {
                var toolArgs = ComposeArgs(pdfSettings);
                var startInfo = new ProcessStartInfo(GetToolExePath(), toolArgs);
#if !NET_STANDARD
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
#endif
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory = Path.GetDirectoryName(PdfToolPath);
                startInfo.RedirectStandardInput = inputBytes != null;
                startInfo.RedirectStandardOutput = outputStream != null;
                startInfo.RedirectStandardError = true;
                WkHtmlToPdfProcess = Process.Start(startInfo);

                WkHtmlToPdfProcess.ErrorDataReceived += errorDataHandler;
                WkHtmlToPdfProcess.BeginErrorReadLine();

                if (inputBytes != null)
                {
                    WkHtmlToPdfProcess.StandardInput.BaseStream.Write(inputBytes, 0, inputBytes.Length);
                    WkHtmlToPdfProcess.StandardInput.BaseStream.Flush();
                    WkHtmlToPdfProcess.StandardInput.Close();
                }
                long outputLength = 0;
                if (outputStream != null)
                {
                    outputLength = ReadStdOutToStream(WkHtmlToPdfProcess, outputStream);
                }

                WaitWkHtmlProcessForExit();

                if (outputStream == null)
                {
                    if (File.Exists(pdfSettings.OutputFile))
                        outputLength = new FileInfo(pdfSettings.OutputFile).Length;
                }
                CheckExitCode(WkHtmlToPdfProcess.ExitCode, lastErrorLine, outputLength > 0);

            }
            finally
            {
                EnsureWkHtmlProcessStopped();
            }
        }

        /// <summary>
        /// Intiates PDF processing in the batch mode (generate several PDF documents using one wkhtmltopdf process) 
        /// </summary>
        public void BeginBatch()
        {
            if (batchMode)
                throw new InvalidOperationException("HtmlToPdfConverter is already in the batch mode.");
            batchMode = true;
            EnsureWkHtmlLibs(); // lets check wkhtmltopdf.exe prerequisite to avoid check for every individual PDF generation call
        }

        /// <summary>
        /// Ends PDF processing in the batch mode.
        /// </summary>
        public void EndBatch()
        {
            if (!batchMode)
                throw new InvalidOperationException("HtmlToPdfConverter is not in the batch mode.");
            batchMode = false;
            if (WkHtmlToPdfProcess != null)
            {
                if (!WkHtmlToPdfProcess.HasExited)
                {
                    WkHtmlToPdfProcess.StandardInput.Close();
                    WkHtmlToPdfProcess.WaitForExit();
#if !NET_STANDARD
                    WkHtmlToPdfProcess.Close();
#endif
                }
                WkHtmlToPdfProcess = null;
            }
        }

        private void WaitWkHtmlProcessForExit()
        {
            if (ExecutionTimeout.HasValue)
            {
                if (!WkHtmlToPdfProcess.WaitForExit((int)ExecutionTimeout.Value.TotalMilliseconds))
                {
                    EnsureWkHtmlProcessStopped();
                    throw new WkHtmlToPdfException(-2, String.Format("WkHtmlToPdf process exceeded execution timeout ({0}) and was aborted", ExecutionTimeout));
                }
            }
            else
            {
                WkHtmlToPdfProcess.WaitForExit();
            }
        }

        private void EnsureWkHtmlProcessStopped()
        {
            if (WkHtmlToPdfProcess == null)
                return;
            if (!WkHtmlToPdfProcess.HasExited)
            {
                // cleanup
                try
                {
                    WkHtmlToPdfProcess.Kill();
#if !NET_STANDARD
                    WkHtmlToPdfProcess.Close();
#endif
                    WkHtmlToPdfProcess = null;
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
#if !NET_STANDARD
                WkHtmlToPdfProcess.Close();
#endif
                WkHtmlToPdfProcess = null;
            }
        }

        private int ReadStdOutToStream(Process proc, Stream outputStream)
        {
            var buf = new byte[32 * 1024];
            int read;
            int totalRead = 0;
            while ((read = proc.StandardOutput.BaseStream.Read(buf, 0, buf.Length)) > 0)
            {
                outputStream.Write(buf, 0, read);
                totalRead += read;
            }
            return totalRead;
        }

        static string[] ignoreWkHtmlToPdfErrLines = new[] {
            "Exit with code 1 due to network error: ContentNotFoundError",
            "QFont::setPixelSize: Pixel size <= 0",
            "Exit with code 1 due to network error: ProtocolUnknownError",
            "Exit with code 1 due to network error: HostNotFoundError",
            "Exit with code 1 due to network error: ContentOperationNotPermittedError",
            "Exit with code 1 due to network error: UnknownContentError"
        };

        private void CheckExitCode(int exitCode, string lastErrLine, bool outputNotEmpty)
        {
            if (exitCode != 0)
            {
                // workaround for wkhtmltopdf 0.12.x issue when error is returned but PDF is generated
                // https://github.com/wkhtmltopdf/wkhtmltopdf/issues/2051
                if (exitCode == 1 && Array.IndexOf(ignoreWkHtmlToPdfErrLines, lastErrLine.Trim()) >= 0 && outputNotEmpty)
                    return;
                throw new WkHtmlToPdfException(exitCode, lastErrLine);
            }
        }

        private void DeleteFileIfExists(string filePath)
        {
            if (filePath != null && File.Exists(filePath))
                try
                {
                    File.Delete(filePath);
                }
                catch { }
        }

        private void CopyStream(Stream inputStream, Stream outputStream, int bufSize)
        {
            var buf = new byte[bufSize];
            int read;
            while ((read = inputStream.Read(buf, 0, buf.Length)) > 0)
                outputStream.Write(buf, 0, read);
        }

    }

    /// <summary>
    /// PDF page orientation
    /// </summary>
    public enum PageOrientation
    {
        Default,

        /// <summary>
        /// Landscape orientation
        /// </summary>
        Landscape,

        /// <summary>
        /// Portrait orientation (default)
        /// </summary>
        Portrait
    }

    /// <summary>
    /// PDF page size
    /// </summary>
    public enum PageSize
    {
        Default,

        /// <summary>
        /// A4
        /// </summary>
        A4,

        /// <summary>
        /// A3
        /// </summary>
        A3,

        /// <summary>
        /// Letter
        /// </summary>
        Letter
    }

    public sealed class LicenseInfo
    {

        /// <summary>
        /// Determines if component has activated license key.
        /// </summary>
        public bool IsLicensed
        {
            get { return I.IsLicensed; }
        }

        /// <summary>
        /// License owner identifier.
        /// </summary>
        public string LicenseOwner
        {
            get { return I.Owner; }
        }

        private Info I;

        internal LicenseInfo()
        {
            I = new Info();
#if LICENSE_FULL
			I.IsLicensed = true;
			I.Owner = "Full Source Code License";
#else
            I.IsLicensed = false;
#endif
        }

        internal void Check()
        {
        }

        /// <summary>
        /// Activate component license and enable restricted features.
        /// </summary>
        /// <param name="owner">license owner ID</param>
        /// <param name="key">unique license key from component order's page</param>
        public void SetLicenseKey(string owner, string key)
        {
#if LICENSE_FULL
			I.IsLicensed = true;
			I.Owner = owner;
#endif

        }

        internal sealed class Info
        {
            internal Info() { }
            internal bool IsLicensed;
            internal string Owner;
        }

       


    }

    public class PageMargins
    {

        /// <summary>
        /// Get or set top margin (in mm)
        /// </summary>
        public float? Top { get; set; }

        /// <summary>
        /// Get or set bottom margin (in mm)
        /// </summary>
        public float? Bottom { get; set; }

        /// <summary>
        /// Get or set left margin (in mm)
        /// </summary>
        public float? Left { get; set; }

        /// <summary>
        /// Get or set right margin (in mm)
        /// </summary>
        public float? Right { get; set; }
    }

    public class WkHtmlToPdfException : Exception
    {

        /// <summary>
        /// Get WkHtmlToPdf process error code
        /// </summary>
        public int ErrorCode { get; private set; }

        public WkHtmlToPdfException(int errCode, string message)
            : base(String.Format("{0} (exit code: {1})", message, errCode))
        {
            ErrorCode = errCode;
        }

    }
}
