using System;
using System.IO.Ports;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace ZplPrinter.Core
{
    public static class PrinterService
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DOCINFO { public string pDocName; public string pOutputFile; public string pDataType; }

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool OpenPrinter(string pPrinterName, out IntPtr hPrinter, IntPtr pDefault);
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFO di);
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        // 변경점: BaudRate, DataBits, Parity, StopBits 파라미터 추가
        public static bool PrintViaCom(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits, string zpl, out string errorMsg)
        {
            errorMsg = "";
            try
            {
                using var port = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
                { Handshake = Handshake.None, Encoding = Encoding.ASCII, WriteTimeout = 3000, DtrEnable = true, RtsEnable = true };

                port.Open();
                byte[] bytes = Encoding.ASCII.GetBytes(zpl);
                port.BaseStream.Write(bytes, 0, bytes.Length);
                port.BaseStream.Flush();
                return true;
            }
            catch (Exception ex) { errorMsg = ex.Message; return false; }
        }

        public static bool PrintViaUsb(string printerName, string zpl, out string errorMsg)
        {
            errorMsg = "";
            if (!OpenPrinter(printerName, out IntPtr hPrinter, IntPtr.Zero))
            {
                errorMsg = $"프린터를 열 수 없습니다. (Win32 Err: {Marshal.GetLastWin32Error()})"; return false;
            }
            try
            {
                var di = new DOCINFO { pDocName = "ZPL Print Job", pDataType = "RAW" };
                if (!StartDocPrinter(hPrinter, 1, ref di)) throw new Exception("Doc 시작 실패");
                try
                {
                    if (!StartPagePrinter(hPrinter)) throw new Exception("Page 시작 실패");
                    byte[] bytes = Encoding.UTF8.GetBytes(zpl);
                    IntPtr pBytes = Marshal.AllocHGlobal(bytes.Length);
                    try
                    {
                        Marshal.Copy(bytes, 0, pBytes, bytes.Length);
                        if (!WritePrinter(hPrinter, pBytes, bytes.Length, out int written) || written == 0)
                            throw new Exception("프린터 데이터 전송 실패");
                    }
                    finally { Marshal.FreeHGlobal(pBytes); }
                    EndPagePrinter(hPrinter);
                }
                finally { EndDocPrinter(hPrinter); }
                return true;
            }
            catch (Exception ex) { errorMsg = ex.Message; return false; }
            finally { ClosePrinter(hPrinter); }
        }

        public static bool PrintViaEthernet(string ip, int port, string zpl, out string errorMsg)
        {
            errorMsg = "";
            try
            {
                using var client = new TcpClient();
                if (!client.ConnectAsync(ip, port).Wait(TimeSpan.FromSeconds(5)))
                    throw new TimeoutException("연결 타임아웃");

                using var stream = client.GetStream();
                byte[] bytes = Encoding.ASCII.GetBytes(zpl);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                return true;
            }
            catch (Exception ex) { errorMsg = ex.Message; return false; }
        }
    }
}