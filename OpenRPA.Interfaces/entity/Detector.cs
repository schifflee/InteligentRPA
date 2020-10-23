﻿using Newtonsoft.Json;
using OpenRPA.Interfaces.entity;
using OpenRPA.Interfaces.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRPA.Interfaces;


namespace OpenRPA.Interfaces.entity
{
    public class Detector : apibase
    {
        public Detector()
        {
            _type = "detector";
            Properties = new Dictionary<string, object>();
        }
        public string Plugin { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string Filename { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public Dictionary<string, object> Properties { get { return GetProperty<Dictionary<string, object>>(); } set { SetProperty(value); } }
        [JsonIgnore]
        public string Path { get { return GetProperty<string>(); } set { SetProperty(value); } }
        [JsonIgnore]
        public string Filepath
        {
            get
            {
                if(Filename==null) { Filename = ""; }
                return System.IO.Path.Combine(Path, Filename);
            }
        }
        public static Detector[] loadDetectors(string Path)
        {
            var ProjectFiles = System.IO.Directory.EnumerateFiles(Path, "*.rpadetector", System.IO.SearchOption.AllDirectories).OrderBy((x) => x).ToArray();
            var Detectors = new List<Detector>();
            foreach (string file in ProjectFiles) { var d = FromFile(file); if (d != null) {  Detectors.Add(d); } }
            return Detectors.ToArray();
        }
        public static Detector FromFile(string Filepath)
        {
            Detector detector = JsonConvert.DeserializeObject<Detector>(System.IO.File.ReadAllText(Filepath));
            if (detector == null) return null;
            detector.Filename = System.IO.Path.GetFileName(Filepath);
            if (string.IsNullOrEmpty(detector.name)) { detector.name = System.IO.Path.GetFileNameWithoutExtension(Filepath); }
            detector.Path = System.IO.Path.GetDirectoryName(Filepath);
            detector._type = "detector";
            return detector;
        }
        public string ReplaceInvalidChars(string filename)
        {
            if (filename == null) return null;
            return string.Join("_", filename.Split(System.IO.Path.GetInvalidFileNameChars()));
        }
        public void SaveFile()
        {
            Filename = ReplaceInvalidChars(Filename);
            if (string.IsNullOrEmpty(Filename))
            {
                Filename = UniqueFilename();
            }
            var basePath = System.IO.Path.GetDirectoryName(Filepath);
            if (!System.IO.Directory.Exists(basePath)) System.IO.Directory.CreateDirectory(basePath);
            System.IO.File.WriteAllText(Filepath, JsonConvert.SerializeObject(this));
        }
        public void Delete()
        {
            if (System.IO.File.Exists(Filename)) System.IO.File.Delete(Filename);
        }
        public string UniqueFilename()
        {
            string Filename = ""; string FilePath = "";
            bool isUnique = false; int counter = 1;
            while (!isUnique)
            {
                if (counter == 1)
                {
                    Filename = System.Text.RegularExpressions.Regex.Replace(ReplaceInvalidChars(name), @"[^0-9a-zA-Z]+", "") + ".rpadetector";
                    FilePath = System.IO.Path.Combine(Path, Filename);
                }
                else
                {
                    Filename = ReplaceInvalidChars(name).Replace(" ", "_").Replace(".", "") + counter.ToString() + ".rpadetector";
                    FilePath = System.IO.Path.Combine(Path, Filename);
                }
                if (!System.IO.File.Exists(FilePath)) isUnique = true;
                counter++;
            }
            return Filename;
        }

    }
}
