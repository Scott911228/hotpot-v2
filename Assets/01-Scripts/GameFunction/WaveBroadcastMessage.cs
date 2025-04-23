[System.Serializable]
public class WaveBroadcastMessage
{
    public string stageName;      // 關卡名稱（如 第二關）
    public int waveNumber;        // 第幾波
    public string messageKey;     // 要播放的文字 key（如 tutorial2/text3）
}