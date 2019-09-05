using UnityEngine;
using UMod;
using Synth.mods.utils;
using Synth.mods.events;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;
using System.Collections.Generic;

public class SongStatusScript : ModScript, ISynthRidersEvents
{
    private string name = "";
    private string artist = "";
    private string mapper = "";
    private string bpm = "";
    private string difficulty = "";
    private int n_misses = 0;
    private string misses = "0";
    private string mode = "";

    //modifiers
    private string accuracy = "NORMAL";
    private string punchMode = "OFF";
    private string noteJumpSpeed = "1x";
    private string oneLife = "OFF";
    private string obstacles = "ON";
    private string noteSize = "NORMAL";
    private string mirrorNotes = "OFF";
    private string haloNotes = "OFF";
    private string noFailMode = "OFF";


    public override void OnModLoaded()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        log("SongStatusMod loaded");
    }

    public override void OnModUnload()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        ClearStatus();
        log("SongStatusMod unloaded");
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
        //get information
        name = trackData.name;
        artist = trackData.artist;
        bpm = trackData.bpm;
        difficulty = trackData.difficulty;
        misses = "0";
        n_misses = 0;
        mode = trackData.mode;
        if (trackData.isCustomSong)
        {
            mapper = trackData.beatmapper;
        }
        else
        {
            mapper = "Original";
        }

        foreach (string modifier in trackData.activemodifiers)//modifiers
        {
            if (modifier.Contains("No fail mode"))
            {
                noFailMode = "ON";
            }
            else if (modifier.Contains("Punch Mode"))
            {
                string temp = modifier.Substring(modifier.IndexOf("-") + 2);
                if (temp.Equals("Normal"))
                {
                    punchMode = "NORMAL";
                }
                else if (temp.Equals("Hard"))
                {
                    punchMode = "HARD";
                }
                else if (temp.Equals("Intense"))
                {
                    punchMode = "INTENSE";
                }
            }
            else if (modifier.Contains("One Life"))
            {
                oneLife = "ON";
            }
            else if (modifier.Contains("Accuracity"))
            {
                accuracy = "HARD";
            }
            else if (modifier.Contains("Note jump speed"))
            {
                noteJumpSpeed = modifier.Substring(modifier.IndexOf("-") + 2);
            }
            else if (modifier.Contains("Obstacles"))
            {
                obstacles = "OFF";
            }
            else if (modifier.Contains("Notes size"))
            {
                string temp = modifier.Substring(modifier.IndexOf("-") + 2);
                if (temp.Equals("Small"))
                {
                    noteSize = "SMALL";
                }
                else if (temp.Equals("Big"))
                {
                    noteSize = "BIG";
                }
            }
            else if (modifier.Contains("Mirror Notes"))
            {
                mirrorNotes = "ON";
            }
            else if (modifier.Contains("Halo Notes"))
            {
                haloNotes = "ON";
            }
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
        //get file path
        var dataPath = Application.dataPath;
        string filePath = dataPath.Substring(0, dataPath.LastIndexOf('/')) + "/Mods/SongStatusMod/";

        //load  
        string text = "";
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
        text = text.Replace("<mode>", mode);

        //modifiers
        text = text.Replace("<accuracy>", accuracy);
        text = text.Replace("<punchMode>", punchMode);
        text = text.Replace("<noteJumpdSpeed>", noteJumpSpeed);
        text = text.Replace("<oneLife>", oneLife);
        text = text.Replace("<obstacles>", obstacles);
        text = text.Replace("<noteSize>", noteSize);
        text = text.Replace("<mirrorNotes>", mirrorNotes);
        text = text.Replace("<haloNotes>", haloNotes);
        text = text.Replace("<noFailMode>", noFailMode);

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
        string filePath = dataPath.Substring(0, dataPath.LastIndexOf('/')) + "/Mods/SongStatusMod/";

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
        mode = "";

        //modifiers
        accuracy = "NORMAL";
        punchMode = "OFF";
        noteJumpSpeed = "1x";
        oneLife = "OFF";
        obstacles = "ON";
        noteSize = "NORMAL";
        mirrorNotes = "OFF";
        haloNotes = "OFF";
        noFailMode = "OFF";
    }

    public void log(string str)
    {
        //get file path
        var dataPath = Application.dataPath;
        var filePath = dataPath.Substring(0, dataPath.LastIndexOf('/')) + "/Novalog.txt";

        //write
        using (var streamWriter = new StreamWriter(filePath, true))
        {
            streamWriter.WriteLine(str);
        }
    }
}

