using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;


public class Networks {
    public IPAddress localAdd;

    public async Task<Response> Send(string host, int port, string message){
        Response res = new Response();
        try {
            Stream stream = null;
            byte[] be = null;

            TcpClient tcpClnt = new TcpClient();
            await tcpClnt.ConnectAsync(host, port);
            stream = tcpClnt.GetStream();

            if(stream.CanWrite){
                be = Encoding.ASCII.GetBytes(message);
                if(stream != null){
                    stream.Write(be, 0, be.Length);
                }
            }
            tcpClnt.Close();
            tcpClnt = null;
        }
        catch(Exception e){
            res.status = "error";
            res.message = e.Message;
        }
        return await Task.FromResult(res);
    }

    public void Init(ref TcpListener listener, string host, int port){
    try{
    localAdd = IPAddress.Parse(host);
    listener = new TcpListener(localAdd, port);
    listener.Start();
    }
    catch(Exception e){
        
    }
    }
        public void Close(TcpListener listener){
        if (listener != null){
            listener.Stop();
        }
    }

    public Response TestServer (string host, int port){
        Response res = new Response();

        try {
            IPAddress localAdd = IPAddress.Parse(host);
            TcpListener listener = new TcpListener(localAdd, port);
            Console.WriteLine("Listening...");

            listener.Start();
            listener.Stop();

            res.status = "ok";
        }
        catch(Exception e){
            res.status = "error";
            res.message = e.Message;
        }
        return res;
    }

    public Response Test (string host, int port){
        Response res = new Response();

        try{
            TcpClient tcpClnt = new TcpClient();
            tcpClnt.Connect(host, port);
            tcpClnt.Close();

            res.status = "ok";
        }
        catch(Exception e){
            res.status = "error";
            res.message = e.Message;
        }
        return res;
    }
}

public class Response {
    public string status {get; set;}
    public string message {get; set;}

    public Response(){
        this.message = "Connection Stablished!";
    }
}