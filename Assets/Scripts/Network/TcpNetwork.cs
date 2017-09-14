using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using PacketInfo;
using UnityEngine;
using Util;

// 비동기 콜백 TcpIp 통신을 목적으로 하는 클래스.
public partial class TcpNetwork
{
    private AsyncCallback _recvCallback;
    private AsyncCallback _sendCallback;
    private Socket        _socket;

    private bool      _isConnected = false;
    private string    _ipAddress   = "127.0.0.1";
    private int       _port        = 23452;

    public TcpNetwork(string ipAddr = "127.0.0.1", int port = 23452)
    {
        _ipAddress    = ipAddr;
        _port         = port;
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
        {   // 접속이 완료되었으므로 Connect 요청을 그만둠. 
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

    // 지정한 구조체를 패킷으로 만들어 서버에 전달하는 메소드.
    public void SendPacket<T>(T data, PacketId packetId)
    {
        if (_isConnected == false)
        {
            Debug.LogAssertion("Socket Connection Not Completed");
            return;
        }

        var sendData = new AsyncSendData(_socket);
        int id;
        int bodySize;

        // 포인터 동작 수행.
        unsafe
        {
            #region PREPARE SENDING DATAS
            // 보낼 데이터 구조체를 Json 형태로 바꿔줌.
            string jsonData = JsonUtility.ToJson(data);

            id = (int)packetId;
            bodySize = jsonData.Length + 1;


            // Send 버퍼 생성.
            sendData._buffer = new byte[NetworkDefinition.PacketHeaderSize + bodySize + 1];
            sendData._sendSize = NetworkDefinition.PacketHeaderSize + bodySize;
            #endregion

            #region COPYING PACKET HEADER
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
            #endregion

            #region COPYING PACKET BODY
            // 패킷 바디 주소의 첫 번째 자리.
            char[] bodyPos = jsonData.ToCharArray();
            
            // 헤더를 기록했던 버퍼 자리 뒤부터 기록.
            for (int i = 0; i < bodySize - 1; ++i)
            {
                sendData._buffer[NetworkDefinition.PacketHeaderSize + i] = (byte)bodyPos[i];
            }

            // 뒤에 널 문자 추가.
            var nullChar = Convert.ToByte('\0');
            sendData._buffer[NetworkDefinition.PacketHeaderSize + bodySize] = nullChar;

            #endregion;
        }

        try
        {
            // 비동기 전송 시작. 파라미터는 BeginReceive와 대동소이하므로 생략. 
            _socket.BeginSend(
                sendData._buffer,
                0,
                NetworkDefinition.PacketHeaderSize + bodySize,
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
            Debug.LogAssertion("TcpNetwork was not connected yet");
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

        // 받은 데이터로부터 패킷을 만든다.
        while (true)
        {
            // 헤더 사이즈보다 적은 데이터가 있다면 더 이상 패킷을 만들지 않음.
            if (recvData._recvSize < NetworkDefinition.PacketHeaderSize)
            {
                break;
            }

            // 패킷 헤더 조제.
            var header = new PacketHeader();

            var id = BitConverter.ToInt32(recvData._buffer, 0);
            var bodySize = BitConverter.ToInt32(recvData._buffer, 4);
            Debug.LogFormat("Recv Packet Id {0}, size {1}", id, bodySize);

            // 패킷 조제.
            var bodyJson = NetworkDefinition.NetworkEncoding.GetString(recvData._buffer, 8, bodySize);

            var receivedPacket = new Packet
            {
                packetId = id,
                bodySize = bodySize,
                data = bodyJson
            };

            // 받은 패킷에 관심있어 했던 이벤트들을 모두 호출해준다.
            InvokePacketEvents(receivedPacket);

            // 조제한 데이터 만큼 갱신해준다.
            recvData._readPos += NetworkDefinition.PacketHeaderSize + bodySize;
            recvData._recvSize -= NetworkDefinition.PacketHeaderSize + bodySize;
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

    // 받은 패킷에 이벤트를 걸어놨던 함수들을 모두 실행시킨다.
    private void InvokePacketEvents(Packet receivedPacket)
    {
        switch ((PacketId)receivedPacket.packetId)
        {
            case PacketId.ID_LoginRes:
                this.OnLoginRes.Invoke(JsonUtility.FromJson<PacketInfo.LoginRes>(receivedPacket.data));
                break;
            case PacketId.ID_FastMatchRes:
                this.OnFastMatchRes.Invoke(JsonUtility.FromJson<PacketInfo.FastMatchRes>(receivedPacket.data));
                break;
            case PacketId.ID_MatchCancelRes:
                this.OnMatchCancelRes.Invoke(JsonUtility.FromJson<PacketInfo.MatchCancelRes>(receivedPacket.data));
                break;
            case PacketId.ID_MatchSuccessNotify:
                this.OnMatchSuccessNotify.Invoke(JsonUtility.FromJson<PacketInfo.MatchSuccessNotify>(receivedPacket.data));
                break;
            case PacketId.ID_GameStartNotify:
                this.OnGameStartNotify.Invoke(JsonUtility.FromJson<PacketInfo.GameStartNotify>(receivedPacket.data));
                break;
            case PacketId.ID_TurnStartNotify:
                this.OnTurnStartNotify.Invoke(JsonUtility.FromJson<PacketInfo.TurnStartNotify>(receivedPacket.data));
                break;
            case PacketId.ID_EnemyTurnStartNotify:
                this.OnEnemyTurnStartNotify.Invoke(JsonUtility.FromJson<PacketInfo.EnemyTurnStartNotify>(receivedPacket.data));
                break;
            case PacketId.ID_MoveAck:
                this.OnMoveAck.Invoke(JsonUtility.FromJson<PacketInfo.MoveAck>(receivedPacket.data));
                break;
            case PacketId.ID_EnemyMoveNotify:
                this.OnEnemyMoveNotify.Invoke(JsonUtility.FromJson<PacketInfo.EnemyMoveNotify>(receivedPacket.data));
                break;
            case PacketId.ID_FireAck:
                this.OnFireAck.Invoke(JsonUtility.FromJson<PacketInfo.FireAck>(receivedPacket.data));
                break;
            case PacketId.ID_EnemyFireNotify:
                this.OnEnemyFireNotify.Invoke(JsonUtility.FromJson<PacketInfo.EnemyFireNotify>(receivedPacket.data));
                break;
            case PacketId.ID_GameSetNotify:
                this.OnGameSetNotify.Invoke(JsonUtility.FromJson<PacketInfo.GameSetNotify>(receivedPacket.data));
                break;
        }
    }

    // Send IO 작업이 끝났을 때 호출될 콜백 메소드.
    private void SendCallBack(IAsyncResult asyncResult)
    {
        if (_isConnected == false)
        {
            return;
        }

        var sendData = (AsyncSendData)asyncResult;

        var sendedSize = 0;

        try
        {
            // 비동기 Send 요청을 끝내준다.
            sendedSize = sendData._socket.EndSend(asyncResult);
        }
        catch (SocketException e)
        {
            HandleException(e);
        }

        // 만약 요청한 사이즈보다 보낸 데이터가 작다면
        if (sendedSize < sendData._sendSize)
        {
            // 다시 비동기 Send를 요청.
            _socket.BeginSend(
                sendData._buffer,
                sendedSize,
                sendData._sendSize - sendedSize,
                SocketFlags.Truncated,              // 메시지가 너무 커서 잘렸을 경우의 플래그.
                _sendCallback,
                sendData);

            Debug.LogAssertion("Sended Size is small");
        }

        Debug.Log("Packet Send End!");
    }

    // SocketException을 처리하는 메소드.
    private void HandleException(SocketException e)
    {
        var errorCode = (SocketError)e.ErrorCode;

        Debug.LogAssertion(e.Message);

        if (errorCode == SocketError.ConnectionAborted ||
            errorCode == SocketError.Disconnecting ||
            errorCode == SocketError.HostDown ||
            errorCode == SocketError.Shutdown ||
            errorCode == SocketError.SocketError ||
            errorCode == SocketError.ConnectionReset)
        {
            // TODO :: Close Connect 알림을 서버에 던짐.
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