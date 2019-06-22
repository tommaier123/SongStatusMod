using UnityEngine;
using UMod;
using Synth.mods.utils;
using Synth.mods.events;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;

public class SongStatusScript : ModScript, ISynthRidersEvents
{
    private string name = "";
    private string artist = "";
    private string mapper = "";
    private string bpm = "";
    private string difficulty = "";
    private int n_misses = 0;
    private string misses = "0";
    private string filePath = null;


    public override void OnModLoaded()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    public override void OnModUnload()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        ClearStatus();
    }

    public void OnRoomLoaded()
    {

    }

    public void OnRoomUnloaded()
    {

    }

    public void OnGameStageLoaded(TrackData trackData)
    {
        //get file path
        var dataPath = Application.dataPath;
        filePath = dataPath.Substring(0, dataPath.LastIndexOf('/')) + "/Mods/SongStatusMod/";

        //create directory
        (new FileInfo(filePath)).Directory.Create();

        //get information
        name = trackData.name;
        artist = trackData.artist;
        bpm = trackData.bpm;
        difficulty = trackData.difficulty;
        misses = "0";
        n_misses = 0;
        if (trackData.isCustomSong)
        {
            mapper = trackData.beatmapper;
        }
        else
        {
            mapper = "Original";
        }
        WriteSongStatus();
    }

    public void OnGameStageUnloaded()
    {

    }

    public void OnScoreStageLoaded()
    {

    }

    public void OnScoreStageUnloaded()
    {

    }

    public void OnNoteFail(PointsData pointsData)
    {
        n_misses++;
        misses = n_misses.ToString();
        WriteSongStatus();
    }

    public void OnPointScored(PointsData pointsData)
    {

    }

    public void OnSongFinished(SongFinishedData songFinishedData)
    {

    }


    public void OnSongFailed(TrackData trackData)
    {

    }

    public void WriteSongStatus()
    {

        //load  
        string text="";
        using (StreamReader streamReader = new StreamReader(filePath + "template.txt"))
        {
            text = streamReader.ReadToEnd();
        }

        //replace
        text = text.Replace("<name>", name);
        text = text.Replace("<artist>", artist);
        text = text.Replace("<mapper>", mapper);
        text = text.Replace("<bpm>", bpm);
        text = text.Replace("<difficulty>", difficulty);
        text = text.Replace("<misses>", misses);

        //write
        using (StreamWriter streamWriter = new StreamWriter(filePath + "status.txt", false))
            {
                streamWriter.WriteLine(text);
            }
    }

    public void ClearStatus()
    {
        //get file path
        var dataPath = Application.dataPath;
        filePath = dataPath.Substring(0, dataPath.LastIndexOf('/')) + "/status/";

        //create directory
        (new FileInfo(filePath)).Directory.Create();

        //write
        using (var streamWriter = new StreamWriter(filePath + "status.txt", false))
        {
            streamWriter.Write("");
        }

        //reset
        name = "";
        artist = "";
        mapper = "";
        bpm = "";
        difficulty = "";
        n_misses = 0;
        misses = "0";

        
    }
}

