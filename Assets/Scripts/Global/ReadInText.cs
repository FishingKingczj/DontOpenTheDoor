using UnityEngine;

public static class ReadInText
{
    public static string[] readin(string path) {
        var input = Resources.Load(path).ToString();
        string[] lines = input.Split(new char[3] { '\r', '\n', ';' });
        var i = 0;
        foreach (var line in lines)
            if (line != "") i++;
        string[] newlines = new string[i];
        i = 0;
        foreach (var line in lines)
            if (line != "")
            {
                newlines[i] = line;
                i++;
            }
        return newlines;
    }
}
