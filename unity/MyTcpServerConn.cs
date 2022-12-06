using System;
using System.Collections; 
using System.Collections.Generic; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;  

public class MyTCPServerConn : MonoBehaviour
{
	#region private members 	
	private TcpListener tcpListener; 
	private Thread tcpListenerThread;  	
	private TcpClient connectedTcpClient; 	
	#endregion 	
		
    private SynchronizationContext mainThreadContext = null;
	void Start () { 		
		tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start(); 	
        mainThreadContext = System.Threading.SynchronizationContext.Current;
        Debug.Log("Start: mainThreadContext is " + mainThreadContext);
	}  	
	
	void Update () { 		
		if (Input.GetKeyDown(KeyCode.Space)) {             
			SendMessage();         
		} 	
	}  	
	
	private void ListenForIncommingRequests () { 		
		try { 			
            var PORT = 50000;
			tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), PORT); 			
			tcpListener.Start();              
			Debug.Log("Server is listening on PORT " + PORT);              
			Byte[] bytes = new Byte[1024];  			
			while (true) { 				
				Debug.Log("waiting on: tcpListener.AcceptTcpClient()");
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
						int length; 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 							
							var incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);  							
							string clientMessage = Encoding.ASCII.GetString(incommingData); 							
							Debug.Log("client message received as: " + clientMessage); 						
                            SendMessage();
						} 					
					} 				
				} 			
				Debug.Log("Done. Ready to loop again.");
			} 		
		} 		
		catch (SocketException socketException) { 			
			Debug.Log("SocketException " + socketException.ToString()); 		
		}     
	}  	

	private void SendMessage() { 		
		if (connectedTcpClient == null) {             
			return;         
		}  		
		
		try { 			
			NetworkStream stream = connectedTcpClient.GetStream(); 			
			if (stream.CanWrite) {                 
                if (mainThreadContext==null) { Debug.Log("mainThreadContext is null"); }
                mainThreadContext.Post(_ =>  {
                    if (!gameObject==null)  { Debug.Log("gameObject is null"); }
                    if (stream==null)  { Debug.Log("stream is null"); }
                    string serverMessage = "This is a message from your server gameObject "+  gameObject.name+"."; 			
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage); 				
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
                },

                null);
				Debug.Log("Server sent message - should be received by client");           
			}       
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		} 	
	} 

}
