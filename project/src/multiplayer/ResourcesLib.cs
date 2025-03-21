using System;
using System.Linq;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class ResourcesAccess
    {
        public string TmpDir = "user://tmp/";

        public (string, string) PackResourceToString<[MustBeVariant] T>(T resVariant)
        {
            var res = Variant.From(resVariant).As<Resource>();

            var packedResource = "";

            var fileType = ".tres";
            if (res is PackedScene) fileType = ".tscn";
            var tmpFilepath = TmpDir + "tmp" + fileType;
            var result = ResourceSaver.Save(res, tmpFilepath);
            if (result == Error.Ok)
            {
                var file = FileAccess.Open(tmpFilepath, FileAccess.ModeFlags.Read);
                var text = file.GetAsText();
                file.Close();
                packedResource = text;
            }

            return (packedResource, fileType);
        }
        // DirAccess.MakeDirRecursiveAbsolute(SavesDirectory);

        public void SaveResource(string packedResource, string filepath)
        {
            var file = FileAccess.Open(filepath, FileAccess.ModeFlags.Write);
            file.Seek(0);
            file.StoreString(packedResource);
            file.Close();
        }

        public string ReadResource(string filepath)
        {
            var file = FileAccess.Open(filepath, FileAccess.ModeFlags.Read);
            var text = file.GetAsText();
            file.Close();
            return text;
        }
    }
}