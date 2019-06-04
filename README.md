# TcpKeepAliveTest

As far as I can tell neither `ServicePointManager.SetTcpKeepAlive` or `HttpWebRequest.ServicePoint.SetTcpKeepAlive` work.  This application allows both approaches to be tested (using Wireshark or similar tool to verify whether or not keep alive packets are sent).  In a Xamarin.Forms UWP app the `HttpWebRequest.ServicePoint.SetTcpKeepAlive` approach fails with an `Operation not supported on this platform` exception.
