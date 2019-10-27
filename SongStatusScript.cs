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
    private int n_multiplier = 1;
    private string multiplier = "1";
    private int n_score = 0;
    private string score = "0";

    //modifiers
    private string accuracy = "NORMAL";
    private string rainbowMode = "OFF";
    private string noteJumpSpeed = "1x";
    private string limitedEnergy = "OFF";
    private string obstacles = "ON";
    private string noteSize = "NORMAL";
    private string colliderSize = "NORMAL";
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
        ClearCover();

        log("SongStatusMod unloaded");
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        ClearStatus();
        ClearCover();
    }

    public void OnRoomLoaded()
    {

    }

    public void OnRoomUnloaded()
    {

    }

    public void OnGameStageLoaded(TrackData trackData)
    {
        /*Accuracity - Hard
          No fail mode - ON
          Notes size - Small
          Note jump speed - 1.25x
          Obstacles - OFF
          Halo Notes - On
          Rainbow Mode - On
          Collider Size - On
          Limited Energy - On*/


        //get information
        name = trackData.name;
        artist = trackData.artist;
        bpm = trackData.bpm;
        difficulty = trackData.difficulty;
        misses = "0";
        n_misses = 0;
        n_multiplier = 1;
        multiplier = "1";
        n_score = 0;
        score = "0";

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
            log(modifier);
            if (modifier.Contains("No fail mode"))
            {
                noFailMode = "ON";
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
            else if (modifier.Contains("Collider Size"))
            {
                string temp = modifier.Substring(modifier.IndexOf("-") + 2);
                if (temp.Equals("Small"))
                {
                    colliderSize = "SMALL";
                }
                else if (temp.Equals("Medium"))
                {
                    colliderSize = "MEDIUM";
                }
            }
            else if (modifier.Contains("Limted Energy"))
            {
                string temp = modifier.Substring(modifier.IndexOf("-") + 2);
                if (temp.Contains("1"))
                {
                    limitedEnergy = "1 LIVE";
                }
                else if (temp.Contains("3 LIVES"))
                {
                    limitedEnergy = "3";
                }
            }
            else if (modifier.Contains("Halo Notes"))
            {
                haloNotes = "ON";
            }
            else if (modifier.Contains("Rainbow Mode"))
            {
                rainbowMode = "ON";
            }
        }

        WriteSongStatus();
        WriteCover(trackData.coverImage);

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
        n_multiplier = pointsData.currMultiplier;
        multiplier = n_multiplier.ToString();
        n_score += (int)(pointsData.points * pointsData.currMultiplier);
        score = n_score.ToString();
        WriteSongStatus();
    }

    public void OnSongFinished(SongFinishedData songFinishedData)
    {

    }


    public void OnSongFailed(TrackData trackData)
    {

    }

    public void WriteCover(Texture2D tex)
    {

        var dataPath = Application.dataPath;
        string filePath = dataPath.Substring(0, dataPath.LastIndexOf('/')) + "/Mods/SongStatusMod/";

        Texture2D MyTex = new Texture2D(tex.width, tex.height);
        MyTex.SetPixels32(tex.GetPixels32());
        MyTex.Apply();

        byte[] bytes = MyTex.EncodeToPNG();

        File.WriteAllBytes(filePath + "cover.png", bytes);

    }

    public void ClearCover()
    {
        Texture2D tex = new Texture2D(0, 0, TextureFormat.RGB24, false);

        WriteCover(tex);
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
        text = text.Replace("<multiplier>", multiplier);
        text = text.Replace("<score>", score);

        //modifiers
        text = text.Replace("<accuracy>", accuracy);
        text = text.Replace("<rainbowMode>", rainbowMode);
        text = text.Replace("<noteJumpdSpeed>", noteJumpSpeed);
        text = text.Replace("<limitedEnergy>", limitedEnergy);
        text = text.Replace("<obstacles>", obstacles);
        text = text.Replace("<noteSize>", noteSize);
        text = text.Replace("<colliderSize>", colliderSize);
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
        n_misses = 0;
        n_multiplier = 1;
        multiplier = "1";
        n_score = 0;
        score = "0";

        //modifiers
        accuracy = "NORMAL";
        rainbowMode = "OFF";
        noteJumpSpeed = "1x";
        limitedEnergy = "OFF";
        obstacles = "ON";
        noteSize = "NORMAL";
        colliderSize = "NORMAL";
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

