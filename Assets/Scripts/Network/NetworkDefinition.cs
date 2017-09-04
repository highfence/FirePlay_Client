using System.Runtime.InteropServices;

public enum ErrorCode : int
{
    None = 0,

    SingInInvalidId = 610,

    // 600번대는 Login Server 에러 코드.
    SignInInvalidId = 610,

    // 700번대는 MongoDB 관련 에러 코드.
    MongoDBFindError = 700,
    MongoDBAddError = 701,

    LoginUserInfoDontExist = 710,
    LoginUserInfoAlreadyExist = 711,

    // 800번대는 Redis 관련 에러 코드.
    RedisStartException = 800,
    RedisInvalidAddressString = 801,
    RedisStartSetTestFailed = 802,

    RedisUnRegistedId = 810,
    RedisInvalidToken = 811,

    RedisTokenRegistError = 820,
    RedisTokenDeleteError = 821
}

public enum LoginApiString : int
{
    Login = 0,
    SignIn = 1,
    Logout = 2,
}

static public class NetworkDefinition
{
    static public int BufferSize = 8192;
    static public int PacketHeaderSize = Marshal.SizeOf(typeof(PacketInfo.PacketHeader));
    static public int IntSize = sizeof(int);
    // WARN :: 아스키 인코딩을 사용하는데 한글이 안될 것 같다. 나중에 UTF8로 바꿀 수 있도록.
    static public System.Text.Encoding NetworkEncoding = System.Text.Encoding.ASCII;
}
