using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using PacketInfo;
using UnityEngine;

// 비동기 콜백 TcpIp 통신을 목적으로 하는 클래스.
public class TcpNetwork
{
    private AsyncCallback _recvCallback;
    private AsyncCallback _sendCallback;
    private Socket        _socket;
    // 멀티 스레드 환경에서 Lock이 필요하다.
    private Queue<Packet> _packetQueue;

    private bool      _isConnected = false;
    private string    _ipAddress   = "127.0.0.1";
    private int       _port        = 23452;

    public TcpNetwork(string ipAddr = "127.0.0.1", int port = 23452)
    {
        _ipAddress = ipAddr;
        _port      = port;
        _recvCallback = new AsyncCallback(RecvCallBack);
        _sendCallback = new AsyncCallback(SendCallBack);

        try
        {
            // 소켓 생성.
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        catch (Exception e)
        {
            Debug.LogError("Socket Create Failed Error : " + e.Message);
        }
    }

    // 소켓을 닫아주는 메소드.
    public void CloseNetwork()
    {
        _isConnected = false;
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        _socket = null;
    }

    // 게임서버와 접속을 시도하는 메소드.
    public void ConnectToServer()
    {
        try
        {
            // 게임 서버에 비동기 접속 요청 시작.
            // @ Param 3 : 비동기 접속이 끝났을 경우 호출될 콜백 메소드.
            _socket.BeginConnect(_ipAddress, _port, OnConnectSuccess, 0);
        }
        catch (SocketException e)
        {
            // TODO :: 재접속 시도.
            Debug.LogAssertion(e.ToString());
            _isConnected = false;
        }
    }

    // 비동기 접속이 끝났을 경우 호출되는 콜백 메소드.
    void OnConnectSuccess(IAsyncResult asyncResult)
    {
        try
        {
            // 접속이 완료되었으므로 Connect 요청을 그만둠. 
            _socket.EndConnect(asyncResult);
            _isConnected = true;
            Debug.LogFormat("Server Connect Success Ip {0}, Port : {1}", _ipAddress, _port);
        }
        catch (Exception e)
        {
            Debug.Log("Socket Connect Callback Function failed : " + e.Message);
            return;
        }

        // 연결된 소켓에 Recv를 걸어준다.
        var recvData = new AsyncRecvData(NetworkDefinition.BufferSize, _socket);

        _socket.BeginReceive(
            recvData._buffer,             // 버퍼
            0,                            // 받은 데이터를 저장할 zero-base postion
            recvData._buffer.Length,      // 버퍼 길이
            SocketFlags.None,             // SocketFlags, 여기서는 None을 지정한다.
            _recvCallback,                // Recv IO가 끝난 뒤 호출할 메소드.
            recvData                      // IO가 끝난 뒤 호출할 메소드의 인자로 들어갈 사용자 구조체.
            );
    }

    // 서버에 연결되어있는지를 반환하는 메소드.
    public bool IsConnected()
    {
        return _isConnected;
    }

    // 패킷 큐가 비어있는지를 반환하는 메소드.
    public bool IsQueueEmpty()
    {
        lock(this)
        {
            if (_packetQueue.Count == 0) return true;
            else return false;
        }
    }

    // 패킷 큐에서 패킷을 하나 빼주는 메소드.
    public Packet GetPacket()
    {
        lock(this)
        {
            return _packetQueue.Dequeue();
        }
    }

    // 지정한 구조체를 패킷으로 만들어 서버에 전달하는 메소드.
    public void SendPacket<T>(T data, PacketId packetId)
    {
        if (_isConnected == false)
        {
            Debug.LogAssertion("Socket Connection Not Completed");
            return;
        }

        var sendData = new AsyncSendData(_socket);

        // 포인터 동작 수행.
        unsafe
        {
            #region Prepare Datas
            // 보낼 데이터 구조체를 Json 형태로 바꿔줌.
            string jsonData = JsonUtility.ToJson(data);

            int id = (int)packetId;
            int bodySize = jsonData.Length;

            // Send 버퍼 생성.
            sendData._buffer = new byte[NetworkDefinition.PacketHeaderSize + bodySize];
            sendData._sendSize = NetworkDefinition.PacketHeaderSize + bodySize;
            #endregion;

            #region Copying Header Data
            // 패킷 아이디 주소의 첫 번째 자리.
            byte * packetIdPos = (byte*)&id;

            // 포인터를 옮겨가며 버퍼에 기록.
            for (int i = 0; i < NetworkDefinition.IntSize; ++i)
            {
                sendData._buffer[i] = packetIdPos[i];
            }

            // 패킷 바디사이즈 주소의 첫 번째 자리.
            byte * bodySizePos = (byte*)&bodySize;

            // 아이디를 기록했던 버퍼 자리 뒤부터 기록.
            for (int i = 0; i < NetworkDefinition.IntSize; ++i)
            {
                sendData._buffer[NetworkDefinition.IntSize + i] = bodySizePos[i]; 
            }
            #endregion;

            #region Copying Body Data
            // 패킷 바디 주소의 첫 번째 자리.
            char[] bodyPos = jsonData.ToCharArray();
            
            // 헤더를 기록했던 버퍼 자리 뒤부터 기록.
            for (int i = 0; i < bodySize; ++i)
            {
                sendData._buffer[NetworkDefinition.PacketHeaderSize + i] = (byte)bodyPos[i];
            }
            #endregion;
        }

        try
        {
            // 비동기 전송 시작. 파라미터는 BeginReceive와 대동소이하므로 생략. 
            _socket.BeginSend(
                sendData._buffer,
                0,
                sendData._buffer.Length,
                SocketFlags.None,
                _sendCallback,
                sendData);
        }
        catch (SocketException e)
        {
            HandleException(e);
            return;
        }
    }

    // Recv IO 작업이 끝났을 때 호출될 콜백 메소드.
    void RecvCallBack(IAsyncResult asyncResult)
    {
        if (_isConnected == false)
        {
            return;
        }

        // 등록해뒀던 사용자 정의 구조체를 콜백 함수의 인자로 받음.
        var recvData = (AsyncRecvData)asyncResult.AsyncState;

        try
        {
            // 비동기 IO를 이제 끝내고 받은 바이트 수를 추가해준다.
            recvData._recvSize += recvData._socket.EndReceive(asyncResult);
            // 읽었던 위치를 처음으로 돌려준다.
            recvData._readPos = 0;
        }
        catch (SocketException e)
        {
            HandleException(e);
            return;
        }

        // 받은 데이터로부터 패킷을 만듬. 포인터로 동작.
        unsafe
        {
            while (true)
            {
                // 헤더 사이즈보다 적은 데이터가 있다면 더 이상 패킷을 만들지 않음
                if (recvData._recvSize < NetworkDefinition.PacketHeaderSize)
                {
                    break;
                }

                // 가비지 컬렉터에 지워지지 않도록 fixed 선언.
                fixed(byte * packetHeaderPos = &recvData._buffer[recvData._readPos])
                {
                    // readPos 위치에서 헤더를 읽는다.
                    PacketHeader* header = (PacketHeader*)packetHeaderPos;

                    // 패킷 사이즈가 남은 데이터 보다 크다면 더 이상 패킷을 만들지 않음.
                    int packetSize = NetworkDefinition.PacketHeaderSize + header->bodySize;
                    if (recvData._recvSize < packetSize)
                    {
                        break;
                    }

                    // 패킷 조제.
                    var packet = new Packet();
                    packet.bodySize = header->bodySize;
                    packet.packetId = header->packetId;

                    packet.data = NetworkDefinition.NetworkEncoding.GetString(
                        recvData._buffer,                                       // 디코딩할 바이트 버퍼.
                        recvData._readPos + NetworkDefinition.PacketHeaderSize, // 디코딩할 첫 번째 바이트의 인덱스
                        header->bodySize                                        // 디코딩할 바이트 수
                        );

                    // 조제한 패킷을 큐에 넣어준다.
                    lock(this)
                    {
                        _packetQueue.Enqueue(packet);
                    }

                    // 조제한 데이터 만큼 갱신해준다.
                    recvData._readPos += packetSize;
                    recvData._recvSize -= packetSize;
                }
            }
        }

        // 사용한 데이터 만큼 앞으로 땡겨준다.
        for (var i = 0; i < recvData._recvSize; ++i)
        {
            recvData._buffer[i] = recvData._buffer[recvData._readPos + i];
        }

        // 다시 비동기 Recv를 걸어준다.
        recvData._socket.BeginReceive(
            recvData._buffer,
            recvData._recvSize,                           // 받아 놓은 길이에서부터 recv 시작.
            recvData._buffer.Length - recvData._recvSize, // 받아 놓은 길이만큼 버퍼길이가 줄어든 상태.
            SocketFlags.None,
            _recvCallback,
            recvData);
    }

    // Send IO 작업이 끝났을 때 호출될 콜백 메소드.
    void SendCallBack(IAsyncResult asyncResult)
    {
        if (_isConnected == false)
        {
            return;
        }

        var sendData = (AsyncSendData)asyncResult;

        var sendSize = 0;

        try
        {
            // 비동기 Send 요청을 끝내준다.
            sendSize = sendData._socket.EndSend(asyncResult);
        }
        catch (SocketException e)
        {
            HandleException(e);
        }

        // 만약 요청한 사이즈보다 보낸 데이터가 작다면
        if (sendSize < sendData._sendSize)
        {
            // 다시 비동기 Send를 요청.
            _socket.BeginSend(
                sendData._buffer,
                sendSize,
                sendData._buffer.Length - sendSize,
                SocketFlags.Truncated,              // 메시지가 너무 커서 잘렸을 경우의 플래그.
                _sendCallback,
                sendData);
        }
    }

    // SocketException을 처리하는 메소드.
    void HandleException(SocketException e)
    {
        var errorCode = (SocketError)e.ErrorCode;

        if (errorCode == SocketError.ConnectionAborted ||
            errorCode == SocketError.Disconnecting ||
            errorCode == SocketError.HostDown ||
            errorCode == SocketError.Shutdown ||
            errorCode == SocketError.SocketError ||
            errorCode == SocketError.ConnectionReset)
        {
            // TODO :: Close Connect 알림을 서버에 던짐.
            Debug.LogAssertion(e.Message);
        }
    }
}

// 비동기 수신에 사용할 구조체.
public class AsyncRecvData
{
    public byte[] _buffer;
    public Socket _socket;
    public int _recvSize;
    public int _readPos;

    public AsyncRecvData(int bufferSize, Socket socket)
    {
        _recvSize = 0;
        _readPos = 0;
        _buffer = new byte[bufferSize];
        _socket = socket;
    }
}

// 비동기 발신에 사용할 구조체.
public class AsyncSendData
{
    public byte[] _buffer;
    public Socket _socket;
    public int _sendSize;

    public AsyncSendData(Socket socket)
    {
        _socket = socket;
        _sendSize = 0;
    }
}