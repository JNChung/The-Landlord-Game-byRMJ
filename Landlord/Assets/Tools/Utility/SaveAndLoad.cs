using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEngine.Networking.Types;

namespace RonTools
{
    public class SaveAndLoad
    {
        string filename;
        string fullpath;
        string fullname;
        public string FullPath => fullpath;
        public void SaveText(string val, bool append = true)
        {
            StreamWriter writer = new StreamWriter(this.fullname, append, Encoding.UTF8);
            writer.Write(val);
            writer.Close();
        }
        public string LoadText()
        {
            StreamReader reader = new StreamReader(this.fullname);
            string val = reader.ReadToEnd();
            reader.Close();
            return val;
        }
        public void SaveJson<T>(T obj)
        {
            StreamWriter writer = new StreamWriter(fullname, false, Encoding.UTF8);
            writer.Write(JsonUtility.ToJson(obj));
            writer.Close();
        }
        public T LoadJson<T>()
        {
            StreamReader reader = new StreamReader(fullname);
            string json = reader.ReadToEnd();
            reader.Close();
            return JsonUtility.FromJson<T>(json);
        }
        public SaveAndLoad()
        {
        }
        public SaveAndLoad(string fileName)
        {
            InitMember(fileName);
        }


        public SaveAndLoad(string filePath, string fileName)
        {
            InitMember(filePath, fileName);
        }

        private void InitMember(string fileName)
        {
            this.filename = fileName;
            string defaultPath = "";
#if UNITY_EDITOR
            defaultPath = Application.dataPath;
#else
            defaultPath = System.Environment.CurrentDirectory;
#endif

            InitMember(defaultPath, fileName);
        }
        private void InitMember(string filePath, string fileName)
        {
            this.fullname = Path.Combine(filePath, fileName);
            this.fullpath = Path.GetDirectoryName(this.fullname);
            if (!Directory.Exists(fullpath)) Directory.CreateDirectory(fullpath);
        }

        void CheckFieldsHaveValue()
        {

        }
        public bool FileExists()
        {
            if (string.IsNullOrEmpty(filename)) return false;

            return File.Exists(fullname);
        }
        public void EasySaveLoadButNotUseful()
        {
            PlayerPrefs.SetString("HeroName", "David");

            string heroName = PlayerPrefs.GetString("HeroName");
        }
    }
}