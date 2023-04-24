using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data;

public class RoomDataManager : MonoBehaviour
{

    private static RoomDataManager _instance;
    public static RoomDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RoomDataManager>();
                if (_instance == null)
                {
                    Debug.LogError("RoomDataManager is not found");
                }
            }
            return _instance;
        }
    }
    // MySQL 연결 정보
    private string host = "localhost";
    private string port = "3306";
    private string database = "db1";
    private string user = "root";
    private string password = "eodnjs10!!";

    // MySQL 연결 객체
    private MySqlConnection connection;

    // Start is called before the first frame update
    void Start()
    {
        // MySQL 연결 문자열 생성
        string connectionString = $"Server={host};Port={port};Database={database};Uid={user};Pwd={password};";

        // MySQL 연결 객체 생성
        connection = new MySqlConnection(connectionString);

        // MySQL 연결
        connection.Open();
    }

    // 방 정보를 MySQL에 저장하는 함수
    public void SaveRoomData(RoomData roomData)
    {
        
        // SQL 쿼리 생성
        string query = $"INSERT INTO roomdata (roomname, connectplayer, maxplayers, password, hostname) VALUES ('{roomData.roomName}', {roomData.connectPlayer}, {roomData.maxPlayers}, '{roomData.password}', '{roomData.hostName}')";

        // SQL 쿼리 실행
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.CommandType = CommandType.Text;
        cmd.ExecuteNonQuery();
    }

    // MySQL에서 방 정보를 불러오는 함수
    public RoomData LoadRoomData(string roomName)
    {
        // SQL 쿼리 생성
        string query = $"SELECT * FROM roomdata WHERE roomname='{roomName}'";

        // SQL 쿼리 실행
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.CommandType = CommandType.Text;
        MySqlDataReader reader = cmd.ExecuteReader();

        // 방 정보 불러오기
        RoomData roomData = null;
        if (reader.Read())
        {
            roomData = new RoomData();
            roomData.roomName = reader.GetString("roomname");
            roomData.connectPlayer = reader.GetInt32("connectplayer");
            roomData.maxPlayers = reader.GetInt32("maxplayers");
            roomData.password = reader.GetString("password");
            roomData.hostName = reader.GetString("hostname");
        }

        // 리더 닫기
        reader.Close();

        return roomData;
    }

    // OnDestroy is called when the object is being destroyed
    private void OnDestroy()
    {
        // MySQL 연결 종료
        connection.Close();
    }
}